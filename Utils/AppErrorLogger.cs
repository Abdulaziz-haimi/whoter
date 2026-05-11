using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Text;
using water3.Services;

namespace water3.Utils
{
    public static class AppErrorLogger
    {
        public static void Log(Exception ex, string source = null)
        {
            if (ex == null) return;

            try
            {
                AppSchemaService.EnsureProductionTables();

                using (var con = Db.GetConnection())
                using (var cmd = new SqlCommand(@"
INSERT INTO dbo.AppErrorLogs(Source, Message, StackTrace, UserName, MachineName, AppVersion)
VALUES(@Source, @Message, @Stack, @User, @Machine, @Version);", con))
                {
                    cmd.Parameters.AddWithValue("@Source", (object)source ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Message", (object)ex.Message ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Stack", (object)ex.ToString() ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@User", Environment.UserName ?? "");
                    cmd.Parameters.AddWithValue("@Machine", Environment.MachineName ?? "");
                    cmd.Parameters.AddWithValue("@Version", GetAppVersion());
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                WriteToFallbackFile(ex, source);
            }
        }

        public static string GetAppVersion()
        {
            try { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
            catch { return "Unknown"; }
        }

        private static void WriteToFallbackFile(Exception ex, string source)
        {
            try
            {
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "water3", "logs");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                string path = Path.Combine(dir, "errors_" + DateTime.Now.ToString("yyyyMMdd") + ".log");

                var sb = new StringBuilder();
                sb.AppendLine("==================================================");
                sb.AppendLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                sb.AppendLine("Source: " + (source ?? ""));
                sb.AppendLine(ex.ToString());
                File.AppendAllText(path, sb.ToString(), Encoding.UTF8);
            }
            catch { }
        }
    }
}
