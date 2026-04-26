using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using water3.Models;
namespace water3.Services
{

        public class AuditLogService
        {
            public void Log(string action, string tableName, int? recordId = null, string details = null, string entityName = null)
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO dbo.AuditLog
                (
                    UserID,
                    Action,
                    TableName,
                    RecordID,
                    ActionDate,
                    Details,
                    EntityName,
                    DeviceName,
                    UserName
                )
                VALUES
                (
                    @UserID,
                    @Action,
                    @TableName,
                    @RecordID,
                    GETDATE(),
                    @Details,
                    @EntityName,
                    @DeviceName,
                    @UserName
                )", con))
                {
                    cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserID == 0 ? (object)DBNull.Value : CurrentUser.UserID);
                    cmd.Parameters.AddWithValue("@Action", (object)(action ?? string.Empty));
                    cmd.Parameters.AddWithValue("@TableName", string.IsNullOrWhiteSpace(tableName) ? (object)DBNull.Value : tableName);
                    cmd.Parameters.AddWithValue("@RecordID", recordId.HasValue ? (object)recordId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Details", string.IsNullOrWhiteSpace(details) ? (object)DBNull.Value : details);
                    cmd.Parameters.AddWithValue("@EntityName", string.IsNullOrWhiteSpace(entityName) ? (object)DBNull.Value : entityName);
                    cmd.Parameters.AddWithValue("@DeviceName", Environment.MachineName);
                    cmd.Parameters.AddWithValue("@UserName", string.IsNullOrWhiteSpace(CurrentUser.UserName) ? (object)DBNull.Value : CurrentUser.UserName);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            public List<AuditLogItem> GetLogs(DateTime? fromDate = null, DateTime? toDate = null, string search = null)
            {
                var list = new List<AuditLogItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    LogID,
                    UserID,
                    UserName,
                    Action,
                    TableName,
                    RecordID,
                    ActionDate,
                    Details,
                    EntityName,
                    DeviceName
                FROM dbo.AuditLog
                WHERE (@FromDate IS NULL OR CAST(ActionDate AS date) >= @FromDate)
                  AND (@ToDate IS NULL OR CAST(ActionDate AS date) <= @ToDate)
                  AND (
                        @Search IS NULL OR @Search = ''
                        OR ISNULL(Action, N'') LIKE N'%' + @Search + N'%'
                        OR ISNULL(TableName, N'') LIKE N'%' + @Search + N'%'
                        OR ISNULL(UserName, N'') LIKE N'%' + @Search + N'%'
                        OR ISNULL(Details, N'') LIKE N'%' + @Search + N'%'
                        OR ISNULL(EntityName, N'') LIKE N'%' + @Search + N'%'
                  )
                ORDER BY ActionDate DESC, LogID DESC", con))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.HasValue ? (object)fromDate.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", toDate.HasValue ? (object)toDate.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@Search", string.IsNullOrWhiteSpace(search) ? (object)DBNull.Value : search.Trim());

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new AuditLogItem
                            {
                                LogID = Convert.ToInt32(dr["LogID"]),
                                UserID = dr["UserID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["UserID"]),
                                UserName = dr["UserName"] == DBNull.Value ? null : Convert.ToString(dr["UserName"]),
                                Action = dr["Action"] == DBNull.Value ? null : Convert.ToString(dr["Action"]),
                                TableName = dr["TableName"] == DBNull.Value ? null : Convert.ToString(dr["TableName"]),
                                RecordID = dr["RecordID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["RecordID"]),
                                ActionDate = dr["ActionDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["ActionDate"]),
                                Details = dr["Details"] == DBNull.Value ? null : Convert.ToString(dr["Details"]),
                                EntityName = dr["EntityName"] == DBNull.Value ? null : Convert.ToString(dr["EntityName"]),
                                DeviceName = dr["DeviceName"] == DBNull.Value ? null : Convert.ToString(dr["DeviceName"])
                            });
                        }
                    }
                }

                return list;
            }
        }
    }