using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Reflection;

namespace WaterCollector.BackendApi.Data
{
    public sealed class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string _explicitConnectionString;

        public SqlConnectionFactory()
        {
        }

        public SqlConnectionFactory(string connectionString)
        {
            _explicitConnectionString = connectionString;
        }

        public SqlConnection CreateConnection()
        {
            string connectionString = _explicitConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString))
                connectionString = TryGetWater3ConnectionString();

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                var cs = ConfigurationManager.ConnectionStrings["WaterBillingDB"];

                if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
                    throw new InvalidOperationException("ConnectionString باسم WaterBillingDB غير موجود.");

                connectionString = cs.ConnectionString;
            }

            return new SqlConnection(connectionString);
        }

        private static string TryGetWater3ConnectionString()
        {
            try
            {
                Type dbType = Type.GetType("water3.Db, water3") ?? Type.GetType("water3.Db");

                if (dbType == null)
                    return null;

                PropertyInfo prop = dbType.GetProperty(
                    "ConnectionString",
                    BindingFlags.Public | BindingFlags.Static
                );

                return prop?.GetValue(null, null) as string;
            }
            catch
            {
                return null;
            }
        }
    }
}
