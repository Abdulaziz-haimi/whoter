using System.Data.SqlClient;

namespace WaterCollector.BackendApi.Data
{
    public interface ISqlConnectionFactory
    {
        SqlConnection CreateConnection();
    }
}
