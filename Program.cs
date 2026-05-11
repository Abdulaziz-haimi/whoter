using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using OfficeOpenXml;
using water3.ApiHost;
using WaterCollector.BackendApi.Hosting;

namespace water3
{
    static class Program
    {
        private static ApiDiscoveryService _discoveryService;
        private static WaterCollector.BackendApi.Hosting.ApiHostService _apiHost;
        private static Mutex _singleInstanceMutex;

        private const string AppName = "Water3";
        private const string MutexName = "Water3_Whoter_Single_Instance";

        [STAThread]
        static void Main()
        {
            ConfigureUnhandledErrors();
            ConfigureExcelPackage();
            ConfigureCulture();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!EnsureSingleInstance())
                return;

            Application.ApplicationExit += delegate
            {
                StopInternalServices();
                ReleaseSingleInstance();
            };

            try
            {
                if (!EnsureDbConnection())
                    return;

                StartInternalServices();

                using (var login = new LoginForm())
                {
                    if (login.ShowDialog() == DialogResult.OK)
                    {
                        Application.Run(new Form1());
                    }
                }
            }
            catch (Exception ex)
            {
                HandleFatalError(ex, "Main");
            }
            finally
            {
                StopInternalServices();
                ReleaseSingleInstance();
            }
        }

        private static bool EnsureSingleInstance()
        {
            try
            {
                bool createdNew;
                _singleInstanceMutex = new Mutex(true, MutexName, out createdNew);

                if (!createdNew)
                {
                    MessageBox.Show(
                        "النظام يعمل بالفعل.\n\nلا يمكن فتح أكثر من نسخة في نفس الوقت.",
                        "تنبيه",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    return false;
                }

                return true;
            }
            catch
            {
                // لا توقف النظام إذا فشل Mutex
                return true;
            }
        }

        private static void ReleaseSingleInstance()
        {
            try
            {
                if (_singleInstanceMutex != null)
                {
                    _singleInstanceMutex.ReleaseMutex();
                    _singleInstanceMutex.Dispose();
                    _singleInstanceMutex = null;
                }
            }
            catch
            {
                _singleInstanceMutex = null;
            }
        }

        private static void ConfigureCulture()
        {
            CultureInfo culture;

            try
            {
                culture = new CultureInfo("ar-YE");
            }
            catch
            {
                culture = new CultureInfo("ar-SA");
            }

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        private static void ConfigureExcelPackage()
        {
            try
            {
                Type excelType = typeof(ExcelPackage);

                // EPPlus 8+
                PropertyInfo licenseProperty = excelType.GetProperty(
                    "License",
                    BindingFlags.Public | BindingFlags.Static
                );

                if (licenseProperty != null)
                {
                    object licenseObject = licenseProperty.GetValue(null, null);

                    if (licenseObject != null)
                    {
                        MethodInfo method = licenseObject.GetType().GetMethod(
                            "SetNonCommercialOrganization",
                            new Type[] { typeof(string) }
                        );

                        if (method != null)
                        {
                            method.Invoke(
                                licenseObject,
                                new object[] { "WaterBillingDB Project" }
                            );

                            return;
                        }

                        MethodInfo personalMethod = licenseObject.GetType().GetMethod(
                            "SetNonCommercialPersonal",
                            new Type[] { typeof(string) }
                        );

                        if (personalMethod != null)
                        {
                            personalMethod.Invoke(
                                licenseObject,
                                new object[] { "WaterBillingDB Project" }
                            );

                            return;
                        }
                    }
                }

                // EPPlus 5 - 7
                PropertyInfo licenseContextProperty = excelType.GetProperty(
                    "LicenseContext",
                    BindingFlags.Public | BindingFlags.Static
                );

                if (licenseContextProperty != null &&
                    licenseContextProperty.PropertyType.IsEnum)
                {
                    object nonCommercialValue = Enum.Parse(
                        licenseContextProperty.PropertyType,
                        "NonCommercial"
                    );

                    licenseContextProperty.SetValue(null, nonCommercialValue, null);
                }
            }
            catch
            {
                // لا توقف تشغيل النظام بسبب إعداد EPPlus
            }
        }

        private static bool EnsureDbConnection()
        {
            if (TryTestCurrentConnection())
                return true;

            DialogResult result;

            try
            {
                using (var f = new water3.DB.DbSettingsForm())
                {
                    result = f.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                HandleFatalError(ex, "DbSettingsForm");
                return false;
            }

            if (result != DialogResult.OK)
                return false;

            if (TryTestCurrentConnection())
                return true;

            MessageBox.Show(
                "تعذر الاتصال بقاعدة البيانات حتى بعد حفظ الإعدادات.\n\n" +
                "تأكد من:\n" +
                "- اسم السيرفر.\n" +
                "- اسم قاعدة البيانات.\n" +
                "- صلاحيات المستخدم.\n" +
                "- تفعيل SQL Server Authentication إذا كنت تستخدم SQL Login.\n" +
                "- تشغيل خدمة SQL Server.\n" +
                "- فتح منفذ SQL Server في الجدار الناري إذا كان السيرفر على جهاز آخر.",
                "خطأ اتصال",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );

            return false;
        }

        private static bool TryTestCurrentConnection()
        {
            try
            {
                string cs = Db.ConnectionString;

                if (string.IsNullOrWhiteSpace(cs))
                    return false;

                Db.TestConnection(cs);
                return true;
            }
            catch (Exception ex)
            {
                TryLogError(ex, "TryTestCurrentConnection");
                return false;
            }
        }

        private static void StartInternalServices()
        {
            bool apiStarted = StartInternalApi();

            if (apiStarted)
                StartDiscoveryService();
        }

        private static bool StartInternalApi()
        {
            try
            {
                if (_apiHost != null)
                    return true;

                _apiHost = new WaterCollector.BackendApi.Hosting.ApiHostService();
                _apiHost.Start();

                return true;
            }
            catch (Exception ex)
            {
                TryLogError(ex, "StartInternalApi");

                MessageBox.Show(
                    "تم تشغيل النظام، لكن تعذر تشغيل API الداخلي.\n\n" +
                    "السبب:\n" + GetRealErrorMessage(ex),
                    "تنبيه API",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                SafeDisposeApiHost();
                return false;
            }
        }

        private static void StartDiscoveryService()
        {
            try
            {
                if (_discoveryService != null)
                    return;

                _discoveryService = new ApiDiscoveryService();
                _discoveryService.Start();
            }
            catch (Exception ex)
            {
                TryLogError(ex, "StartDiscoveryService");

                MessageBox.Show(
                    "تم تشغيل النظام، لكن تعذر تشغيل خدمة اكتشاف السيرفر.\n\n" +
                    "السبب:\n" + GetRealErrorMessage(ex),
                    "فشل تشغيل خدمة اكتشاف السيرفر",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                SafeDisposeDiscoveryService();
            }
        }

        private static void StopInternalServices()
        {
            SafeDisposeDiscoveryService();
            SafeDisposeApiHost();
        }

        private static void SafeDisposeDiscoveryService()
        {
            try
            {
                if (_discoveryService != null)
                {
                    _discoveryService.Dispose();
                    _discoveryService = null;
                }
            }
            catch (Exception ex)
            {
                TryLogError(ex, "SafeDisposeDiscoveryService");
                _discoveryService = null;
            }
        }

        private static void SafeDisposeApiHost()
        {
            try
            {
                if (_apiHost != null)
                {
                    _apiHost.Dispose();
                    _apiHost = null;
                }
            }
            catch (Exception ex)
            {
                TryLogError(ex, "SafeDisposeApiHost");
                _apiHost = null;
            }
        }

        private static void ConfigureUnhandledErrors()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.ThreadException += delegate (object sender, ThreadExceptionEventArgs e)
            {
                TryLogError(e.Exception, "Application.ThreadException");

                MessageBox.Show(
                    "حدث خطأ غير متوقع وتم تسجيله.\n\n" +
                    GetRealErrorMessage(e.Exception),
                    "خطأ غير متوقع",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            };

            AppDomain.CurrentDomain.UnhandledException += delegate (object sender, UnhandledExceptionEventArgs e)
            {
                Exception ex = e.ExceptionObject as Exception;

                TryLogError(ex, "AppDomain.UnhandledException");

                MessageBox.Show(
                    "حدث خطأ عام وتم تسجيله.\n\n" +
                    GetRealErrorMessage(ex),
                    "خطأ عام",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            };
        }

        private static void HandleFatalError(Exception ex, string source)
        {
            TryLogError(ex, source);

            MessageBox.Show(
                "حدث خطأ يمنع تشغيل النظام.\n\n" +
                GetRealErrorMessage(ex),
                "خطأ تشغيل",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        private static void TryLogError(Exception ex, string source)
        {
            if (ex == null)
                return;

            try
            {
                // إذا أضفت AppErrorLogger من الحزمة السابقة سيتم استخدامه تلقائيًا بدون كسر الكود
                Type loggerType = Type.GetType("water3.Utils.AppErrorLogger");

                if (loggerType != null)
                {
                    MethodInfo logMethod = loggerType.GetMethod(
                        "Log",
                        BindingFlags.Public | BindingFlags.Static,
                        null,
                        new Type[] { typeof(Exception), typeof(string) },
                        null
                    );

                    if (logMethod != null)
                    {
                        logMethod.Invoke(null, new object[] { ex, source });
                        return;
                    }
                }
            }
            catch
            {
                // تجاهل وانتقل للتسجيل المحلي
            }

            TryWriteLocalErrorLog(ex, source);
        }

        private static void TryWriteLocalErrorLog(Exception ex, string source)
        {
            try
            {
                string folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    AppName,
                    "Logs"
                );

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string file = Path.Combine(folder, "errors.log");

                string text =
                    "==================================================" + Environment.NewLine +
                    "Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine +
                    "Source: " + source + Environment.NewLine +
                    "Message: " + GetRealErrorMessage(ex) + Environment.NewLine +
                    "Exception:" + Environment.NewLine +
                    ex + Environment.NewLine +
                    Environment.NewLine;

                File.AppendAllText(file, text);
            }
            catch
            {
                // لا توقف النظام بسبب فشل تسجيل الخطأ
            }
        }

        private static string GetRealErrorMessage(Exception ex)
        {
            if (ex == null)
                return "حدث خطأ غير معروف.";

            while (ex is TargetInvocationException && ex.InnerException != null)
                ex = ex.InnerException;

            while (ex is TypeInitializationException && ex.InnerException != null)
                ex = ex.InnerException;

            while (ex is AggregateException && ex.InnerException != null)
                ex = ex.InnerException;

            if (ex.InnerException != null)
            {
                return ex.Message +
                       "\n\nتفاصيل إضافية:\n" +
                       ex.InnerException.Message;
            }

            return ex.Message;
        }
    }
}
/*
using System;
using System.Windows.Forms;
using OfficeOpenXml;
using System.Globalization;
using System.Threading;

namespace water3
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ExcelPackage.License.SetNonCommercialOrganization("WaterBillingDB Project");
            var culture = new CultureInfo("ar-YE"); // أو ar-SA
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            EnsureDbConnectionOrExit();

            using (var login = new LoginForm())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new Form1());
                }
            }




        }

        private static void EnsureDbConnectionOrExit()
        {
            if (TryTestCurrentConnection())
                return;

            using (var f = new water3.DB.DbSettingsForm())
            {
                var res = f.ShowDialog();
                if (res != DialogResult.OK)
                    Environment.Exit(0);
            }

            if (!TryTestCurrentConnection())
            {
                MessageBox.Show(
                    "تعذر الاتصال بقاعدة البيانات حتى بعد حفظ الإعدادات.\n" +
                    "تأكد من السيرفر/اسم القاعدة/الصلاحيات أو استخدم SQL Login.",
                    "خطأ اتصال", MessageBoxButtons.OK, MessageBoxIcon.Error);

                Environment.Exit(0);
            }
        }

        private static bool TryTestCurrentConnection()
        {
            try
            {
                Db.TestConnection(Db.ConnectionString);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

*/