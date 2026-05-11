using System;
using System.Data.SqlClient;

namespace water3.DB
{
    public static class DbStartupChecker
    {
        public static bool CanConnect()
        {
            try
            {
                string cs = ConnectionConfigManager.GetConnectionString();

                if (string.IsNullOrWhiteSpace(cs))
                    return false;

                using (var con = new SqlConnection(cs))
                {
                    con.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}