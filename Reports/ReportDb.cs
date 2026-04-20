using System.Data;
using System.Data.SqlClient;

namespace water3.Reports
{
    public static class ReportDb
    {
        public static DataTable ExecProc(string procName, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(Db.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(procName, con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                if (parameters != null && parameters.Length > 0)
                    cmd.Parameters.AddRange(parameters);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }
            }

            return ReportDataMapper.EnsureSchema(dt);
        }
    }
}
