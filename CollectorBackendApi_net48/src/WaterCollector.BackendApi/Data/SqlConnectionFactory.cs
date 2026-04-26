using System.Configuration;
using System.Data.SqlClient;

namespace WaterCollector.BackendApi.Data
{
    public sealed class SqlConnectionFactory : ISqlConnectionFactory
    {
        public SqlConnection CreateConnection()
        {
            var cs = ConfigurationManager.ConnectionStrings["WaterBillingDb"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cs))
                throw new ConfigurationErrorsException("ConnectionStrings: DefaultConnection غير موجود في Web.config");
            return new SqlConnection(cs);
        }
    }
}
