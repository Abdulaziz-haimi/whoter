/*using OfficeOpenXml;
using System;
using System.Windows.Forms;

namespace water3
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ExcelPackage.License.SetNonCommercialOrganization("WaterBillingDB Project");


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
       
    }
}
*/
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

            Application.Run(new Form1());
           

            
           
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

/*
 * using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace water3
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            var culture = new CultureInfo("ar-YE"); // أو ar-SA

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
*/