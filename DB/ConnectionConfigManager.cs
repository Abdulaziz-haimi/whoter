using System.Configuration;

namespace water3.DB
{
    public static class ConnectionConfigManager
    {
        public static void SaveConnectionString(string connectionString)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var section = config.ConnectionStrings;

            if (section.ConnectionStrings["WaterBillingDB"] == null)
            {
                section.ConnectionStrings.Add(
                    new ConnectionStringSettings(
                        "WaterBillingDB",
                        connectionString,
                        "System.Data.SqlClient"
                    )
                );
            }
            else
            {
                section.ConnectionStrings["WaterBillingDB"].ConnectionString = connectionString;
                section.ConnectionStrings["WaterBillingDB"].ProviderName = "System.Data.SqlClient";
            }

            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("connectionStrings");
        }

        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["WaterBillingDB"]?.ConnectionString;
        }
    }
}