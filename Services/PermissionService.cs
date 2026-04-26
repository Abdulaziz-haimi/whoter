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
        public class PermissionService
        {
            public List<AppPermission> GetPermissionsByRole(int roleId)
            {
                var list = new List<AppPermission>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    P.PermissionID,
                    P.PermissionKey,
                    P.PermissionName,
                    P.Category,
                    ISNULL(RP.IsAllowed, 0) AS IsAllowed
                FROM dbo.Permissions P
                INNER JOIN dbo.RolePermissions RP ON P.PermissionID = RP.PermissionID
                WHERE RP.RoleID = @RoleID
                  AND P.IsActive = 1
                ORDER BY P.Category, P.PermissionName", con))
                {
                    cmd.Parameters.AddWithValue("@RoleID", roleId);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new AppPermission
                            {
                                PermissionID = Convert.ToInt32(dr["PermissionID"]),
                                PermissionKey = Convert.ToString(dr["PermissionKey"]),
                                PermissionName = Convert.ToString(dr["PermissionName"]),
                                Category = dr["Category"] == DBNull.Value ? null : Convert.ToString(dr["Category"]),
                                IsAllowed = Convert.ToBoolean(dr["IsAllowed"])
                            });
                        }
                    }
                }

                return list;
            }

            public bool RoleHasPermission(int roleId, string permissionKey)
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM dbo.RolePermissions RP
                INNER JOIN dbo.Permissions P ON RP.PermissionID = P.PermissionID
                WHERE RP.RoleID = @RoleID
                  AND P.PermissionKey = @PermissionKey
                  AND P.IsActive = 1
                  AND RP.IsAllowed = 1", con))
                {
                    cmd.Parameters.AddWithValue("@RoleID", roleId);
                    cmd.Parameters.AddWithValue("@PermissionKey", permissionKey.Trim());
                    con.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }
            }
        }
    }