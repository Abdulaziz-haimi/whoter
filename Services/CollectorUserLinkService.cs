using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using water3.Models;
namespace water3.Services
{
  
        public class CollectorUserLinkService
        {
            private readonly AuditLogService _audit = new AuditLogService();

            public List<CollectorUserLinkItem> GetCollectorsWithUsers(string search = null)
            {
                var list = new List<CollectorUserLinkItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    C.CollectorID,
                    C.Name AS CollectorName,
                    C.Phone AS CollectorPhone,
                    C.UserID,
                    U.UserName,
                    U.FullName,
                    R.RoleName,
                    CAST(ISNULL(U.IsActive, 0) AS bit) AS IsUserActive
                FROM dbo.Collectors C
                LEFT JOIN dbo.Users U ON C.UserID = U.UserID
                LEFT JOIN dbo.Roles R ON U.RoleID = R.RoleID
                WHERE @Search IS NULL OR @Search = ''
                      OR C.Name LIKE N'%' + @Search + N'%'
                      OR ISNULL(C.Phone, N'') LIKE N'%' + @Search + N'%'
                      OR ISNULL(U.UserName, N'') LIKE N'%' + @Search + N'%'
                      OR ISNULL(U.FullName, N'') LIKE N'%' + @Search + N'%'
                ORDER BY C.Name", con))
                {
                    cmd.Parameters.AddWithValue("@Search", string.IsNullOrWhiteSpace(search) ? (object)DBNull.Value : search.Trim());
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new CollectorUserLinkItem
                            {
                                CollectorID = Convert.ToInt32(dr["CollectorID"]),
                                CollectorName = Convert.ToString(dr["CollectorName"]),
                                CollectorPhone = dr["CollectorPhone"] == DBNull.Value ? null : Convert.ToString(dr["CollectorPhone"]),
                                UserID = dr["UserID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["UserID"]),
                                UserName = dr["UserName"] == DBNull.Value ? null : Convert.ToString(dr["UserName"]),
                                FullName = dr["FullName"] == DBNull.Value ? null : Convert.ToString(dr["FullName"]),
                                RoleName = dr["RoleName"] == DBNull.Value ? null : Convert.ToString(dr["RoleName"]),
                                IsUserActive = dr["IsUserActive"] != DBNull.Value && Convert.ToBoolean(dr["IsUserActive"])
                            });
                        }
                    }
                }

                return list;
            }

            public List<UserLookupItem> GetAvailableUsers(string search = null, bool onlyActive = true)
            {
                var list = new List<UserLookupItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    U.UserID,
                    U.UserName,
                    U.FullName,
                    R.RoleName,
                    CAST(ISNULL(U.IsActive, 1) AS bit) AS IsActive
                FROM dbo.Users U
                INNER JOIN dbo.Roles R ON U.RoleID = R.RoleID
                WHERE (@OnlyActive = 0 OR ISNULL(U.IsActive, 1) = 1)
                  AND (
                        @Search IS NULL OR @Search = ''
                        OR U.UserName LIKE N'%' + @Search + N'%'
                        OR ISNULL(U.FullName, N'') LIKE N'%' + @Search + N'%'
                        OR ISNULL(U.Phone, N'') LIKE N'%' + @Search + N'%'
                      )
                ORDER BY U.UserName", con))
                {
                    cmd.Parameters.AddWithValue("@OnlyActive", onlyActive);
                    cmd.Parameters.AddWithValue("@Search", string.IsNullOrWhiteSpace(search) ? (object)DBNull.Value : search.Trim());
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new UserLookupItem
                            {
                                UserID = Convert.ToInt32(dr["UserID"]),
                                UserName = Convert.ToString(dr["UserName"]),
                                FullName = dr["FullName"] == DBNull.Value ? null : Convert.ToString(dr["FullName"]),
                                RoleName = dr["RoleName"] == DBNull.Value ? null : Convert.ToString(dr["RoleName"]),
                                IsActive = Convert.ToBoolean(dr["IsActive"])
                            });
                        }
                    }
                }

                return list;
            }

            public void AssignUserToCollector(int collectorId, int userId)
            {
                if (collectorId <= 0)
                    throw new InvalidOperationException("معرف المحصل غير صحيح.");

                if (userId <= 0)
                    throw new InvalidOperationException("معرف المستخدم غير صحيح.");

                using (SqlConnection con = Db.GetConnection())
                {
                    con.Open();

                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            // تأكد أن المستخدم غير مربوط بمحصل آخر
                            using (SqlCommand checkCmd = new SqlCommand(@"
                            SELECT TOP 1 CollectorID
                            FROM dbo.Collectors
                            WHERE UserID = @UserID
                              AND CollectorID <> @CollectorID", con, tran))
                            {
                                checkCmd.Parameters.AddWithValue("@UserID", userId);
                                checkCmd.Parameters.AddWithValue("@CollectorID", collectorId);

                                object existing = checkCmd.ExecuteScalar();
                                if (existing != null && existing != DBNull.Value)
                                    throw new InvalidOperationException("هذا المستخدم مربوط بمحصل آخر بالفعل.");
                            }

                            string collectorName = null;
                            string userName = null;

                            using (SqlCommand infoCmd = new SqlCommand(@"
                            SELECT Name FROM dbo.Collectors WHERE CollectorID = @CollectorID;
                            SELECT UserName FROM dbo.Users WHERE UserID = @UserID;", con, tran))
                            {
                                infoCmd.Parameters.AddWithValue("@CollectorID", collectorId);
                                infoCmd.Parameters.AddWithValue("@UserID", userId);

                                using (SqlDataReader dr = infoCmd.ExecuteReader())
                                {
                                    if (dr.Read())
                                        collectorName = Convert.ToString(dr[0]);

                                    if (dr.NextResult() && dr.Read())
                                        userName = Convert.ToString(dr[0]);
                                }
                            }

                            using (SqlCommand updateCmd = new SqlCommand(@"
                            UPDATE dbo.Collectors
                            SET UserID = @UserID
                            WHERE CollectorID = @CollectorID", con, tran))
                            {
                                updateCmd.Parameters.AddWithValue("@CollectorID", collectorId);
                                updateCmd.Parameters.AddWithValue("@UserID", userId);
                                updateCmd.ExecuteNonQuery();
                            }

                            tran.Commit();

                            _audit.Log(
                                action: "LINK_COLLECTOR_USER",
                                tableName: "Collectors",
                                recordId: collectorId,
                                details: $"تم ربط المحصل بالمستخدم: {userName}",
                                entityName: collectorName);
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }

            public void UnassignUserFromCollector(int collectorId)
            {
                if (collectorId <= 0)
                    throw new InvalidOperationException("معرف المحصل غير صحيح.");

                using (SqlConnection con = Db.GetConnection())
                {
                    con.Open();

                    string collectorName = null;
                    string userName = null;

                    using (SqlCommand infoCmd = new SqlCommand(@"
                    SELECT TOP 1 C.Name, U.UserName
                    FROM dbo.Collectors C
                    LEFT JOIN dbo.Users U ON C.UserID = U.UserID
                    WHERE C.CollectorID = @CollectorID", con))
                    {
                        infoCmd.Parameters.AddWithValue("@CollectorID", collectorId);

                        using (SqlDataReader dr = infoCmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                collectorName = dr[0] == DBNull.Value ? null : Convert.ToString(dr[0]);
                                userName = dr[1] == DBNull.Value ? null : Convert.ToString(dr[1]);
                            }
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE dbo.Collectors
                    SET UserID = NULL
                    WHERE CollectorID = @CollectorID", con))
                    {
                        cmd.Parameters.AddWithValue("@CollectorID", collectorId);
                        cmd.ExecuteNonQuery();
                    }

                    _audit.Log(
                        action: "UNLINK_COLLECTOR_USER",
                        tableName: "Collectors",
                        recordId: collectorId,
                        details: $"تم فك ربط المستخدم: {userName}",
                        entityName: collectorName);
                }
            }
        }
    }