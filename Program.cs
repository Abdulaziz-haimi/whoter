using System;
using System.Globalization;
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

        [STAThread]
        static void Main()
        {
            ConfigureUnhandledErrors();
            ConfigureExcelPackage();
            ConfigureCulture();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.ApplicationExit += (s, e) =>
            {
                StopInternalApi();
            };

            if (!EnsureDbConnection())
                return;

            StartInternalApi();
            _discoveryService = new ApiDiscoveryService();

            try
            {
                _discoveryService.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "فشل تشغيل خدمة اكتشاف السيرفر",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
            using (var login = new LoginForm())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new Form1());
                }
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
                    }
                }

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
                // لا توقف تشغيل البرنامج بسبب إعداد EPPlus
            }
        }

        private static bool EnsureDbConnection()
        {
            if (TryTestCurrentConnection())
                return true;

            using (var f = new water3.DB.DbSettingsForm())
            {
                DialogResult result = f.ShowDialog();

                if (result != DialogResult.OK)
                    return false;
            }

            if (TryTestCurrentConnection())
                return true;

            MessageBox.Show(
                "تعذر الاتصال بقاعدة البيانات حتى بعد حفظ الإعدادات.\n\n" +
                "تأكد من:\n" +
                "- اسم السيرفر.\n" +
                "- اسم قاعدة البيانات.\n" +
                "- صلاحيات المستخدم.\n" +
                "- تفعيل SQL Server Authentication إذا كنت تستخدم SQL Login.\n" +
                "- تشغيل خدمة SQL Server.",
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
            catch
            {
                return false;
            }
        }

        private static void StartInternalApi()
        {
            try
            {
                if (_apiHost != null)
                    return;

                _apiHost = new WaterCollector.BackendApi.Hosting.ApiHostService();
                _apiHost.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "تم تشغيل النظام، لكن تعذر تشغيل API الداخلي.\n\n" +
                    "السبب:\n" + GetRealErrorMessage(ex),
                    "تنبيه API",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                _apiHost = null;
            }
        }

        private static void StopInternalApi()
        {
            try
            {
                if (_apiHost != null)
                {
                    _apiHost.Dispose();
                    _apiHost = null;
                }
            }
            catch
            {
                if (_discoveryService != null)
                    _discoveryService.Dispose();
            }
        }

        private static void ConfigureUnhandledErrors()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            Application.ThreadException += (s, e) =>
            {
                MessageBox.Show(
                    GetRealErrorMessage(e.Exception),
                    "خطأ غير متوقع",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            };

            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                Exception ex = e.ExceptionObject as Exception;

                MessageBox.Show(
                    GetRealErrorMessage(ex),
                    "خطأ عام",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            };
        }

        private static string GetRealErrorMessage(Exception ex)
        {
            if (ex == null)
                return "حدث خطأ غير معروف.";

            while (ex is TargetInvocationException && ex.InnerException != null)
                ex = ex.InnerException;

            if (ex.InnerException != null)
                return ex.Message + "\n\nتفاصيل إضافية:\n" + ex.InnerException.Message;

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