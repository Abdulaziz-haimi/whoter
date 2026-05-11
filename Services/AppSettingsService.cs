using System;
using System.Data.SqlClient;

namespace water3.Services
{
    public static class AppSettingsService
    {
        public static string Get(string key, string defaultValue = "")
        {
            AppSchemaService.EnsureProductionTables();
            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand("SELECT SettingValue FROM dbo.AppSettings WHERE SettingKey=@Key;", con))
            {
                cmd.Parameters.AddWithValue("@Key", key);
                con.Open();
                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value) return defaultValue;
                return Convert.ToString(result);
            }
        }

        public static void Set(string key, string value)
        {
            AppSchemaService.EnsureProductionTables();
            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
MERGE dbo.AppSettings AS T
USING (SELECT @Key AS SettingKey, @Value AS SettingValue) AS S
ON T.SettingKey = S.SettingKey
WHEN MATCHED THEN UPDATE SET SettingValue = S.SettingValue, UpdatedAt = SYSUTCDATETIME()
WHEN NOT MATCHED THEN INSERT(SettingKey, SettingValue, UpdatedAt)
VALUES(S.SettingKey, S.SettingValue, SYSUTCDATETIME());", con))
            {
                cmd.Parameters.AddWithValue("@Key", key);
                cmd.Parameters.AddWithValue("@Value", (object)value ?? DBNull.Value);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static void SetMany(params Tuple<string, string>[] values)
        {
            if (values == null) return;
            foreach (var item in values)
            {
                if (item == null) continue;
                Set(item.Item1, item.Item2);
            }
        }

        public static string CompanyName { get { return Get("Company.Name", "مؤسسة المياه"); } }
        public static string CompanyPhone { get { return Get("Company.Phone", ""); } }
        public static string CompanyAddress { get { return Get("Company.Address", ""); } }
        public static string CompanyLogoPath { get { return Get("Company.LogoPath", ""); } }
        public static string InvoiceTitle { get { return Get("Invoice.Title", "فاتورة مياه"); } }
        public static string InvoiceFooter { get { return Get("Invoice.Footer", "شكراً لتعاملكم معنا"); } }
        public static string CurrencyName { get { return Get("Invoice.Currency", "ريال"); } }
        public static string DefaultPrinter { get { return Get("Print.DefaultPrinter", ""); } }
        public static string PaperSize { get { return Get("Print.PaperSize", "A4"); } }
        public static string InvoiceTemplate { get { return Get("Print.InvoiceTemplate", "Classic"); } }
    }
}
