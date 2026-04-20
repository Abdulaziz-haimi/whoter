using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace water3
{
    public static class Db
    {
        private const string ConnName = "WaterBillingDB";        // اسم عنصر connectionStrings في App.config
        private const string UserConnKey = "UserConnString";     // اسم المفتاح في Settings (User)

        public static string ConnectionString
        {
            get
            {
                // 1) اقرأ من User Settings (الأفضل للتثبيت على أجهزة أخرى)
                try
                {
                    var userCs = Properties.Settings.Default[UserConnKey] as string;
                    if (!string.IsNullOrWhiteSpace(userCs))
                        return userCs;
                }
                catch { /* تجاهل */ }

                // 2) fallback: اقرأ من App.config
                var cs = ConfigurationManager.ConnectionStrings[ConnName];
                if (cs == null || string.IsNullOrWhiteSpace(cs.ConnectionString))
                    throw new InvalidOperationException("ConnectionString (WaterBillingDB) غير موجود في App.config");
                return cs.ConnectionString;
            }
        }

        public static SqlConnection GetConnection() => new SqlConnection(ConnectionString);

        public static void TestConnection(string connectionString)
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand("SELECT 1", con))
                    cmd.ExecuteScalar();
            }
        }

        public static void SaveConnectionString(string newConnectionString)
        {
            // ✅ احفظ في Settings (User scope) لتجنب مشكلة عدم وجود صلاحيات كتابة في Program Files
            Properties.Settings.Default[UserConnKey] = newConnectionString;
            Properties.Settings.Default.Save();
        }

        public static DataTable ExecuteTable(string sql, Action<SqlParameterCollection> p = null)
        {
            using (var con = GetConnection())
            using (var cmd = new SqlCommand(sql, con))
            {
                p?.Invoke(cmd.Parameters);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public static int ExecuteNonQuery(string sql, Action<SqlParameterCollection> p = null)
        {
            using (var con = GetConnection())
            {
                con.Open();
                using (var cmd = new SqlCommand(sql, con))
                {
                    p?.Invoke(cmd.Parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string sql, Action<SqlParameterCollection> p = null)
        {
            using (var con = GetConnection())
            {
                con.Open();
                using (var cmd = new SqlCommand(sql, con))
                {
                    p?.Invoke(cmd.Parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}
