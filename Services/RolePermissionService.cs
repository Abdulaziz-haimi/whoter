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


        public class RolePermissionService
        {
            public List<AppRole> GetRoles()
            {
                var list = new List<AppRole>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT RoleID, RoleName
                FROM dbo.Roles
                ORDER BY RoleName", con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new AppRole
                            {
                                RoleID = Convert.ToInt32(dr["RoleID"]),
                                RoleName = Convert.ToString(dr["RoleName"])
                            });
                        }
                    }
                }

                return list;
            }

            public int CreateRole(string roleName)
            {
                if (string.IsNullOrWhiteSpace(roleName))
                    throw new InvalidOperationException("اسم الدور مطلوب.");

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                IF EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = @RoleName)
                BEGIN
                    RAISERROR(N'اسم الدور موجود مسبقًا.', 16, 1);
                    RETURN;
                END

                INSERT INTO dbo.Roles(RoleName)
                VALUES(@RoleName);

                SELECT CAST(SCOPE_IDENTITY() AS INT);", con))
                {
                    cmd.Parameters.AddWithValue("@RoleName", roleName.Trim());
                    con.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            public void UpdateRole(int roleId, string roleName)
            {
                if (roleId <= 0)
                    throw new InvalidOperationException("معرف الدور غير صحيح.");

                if (string.IsNullOrWhiteSpace(roleName))
                    throw new InvalidOperationException("اسم الدور مطلوب.");

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                IF EXISTS (SELECT 1 FROM dbo.Roles WHERE RoleName = @RoleName AND RoleID <> @RoleID)
                BEGIN
                    RAISERROR(N'اسم الدور موجود لدور آخر.', 16, 1);
                    RETURN;
                END

                UPDATE dbo.Roles
                SET RoleName = @RoleName
                WHERE RoleID = @RoleID", con))
                {
                    cmd.Parameters.AddWithValue("@RoleID", roleId);
                    cmd.Parameters.AddWithValue("@RoleName", roleName.Trim());
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            public void DeleteRole(int roleId)
            {
                if (roleId <= 0)
                    throw new InvalidOperationException("معرف الدور غير صحيح.");

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                IF EXISTS (SELECT 1 FROM dbo.Users WHERE RoleID = @RoleID)
                BEGIN
                    RAISERROR(N'لا يمكن حذف الدور لأنه مستخدم من قبل بعض المستخدمين.', 16, 1);
                    RETURN;
                END

                DELETE FROM dbo.RolePermissions WHERE RoleID = @RoleID;
                DELETE FROM dbo.Roles WHERE RoleID = @RoleID;", con))
                {
                    cmd.Parameters.AddWithValue("@RoleID", roleId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            public List<RolePermissionItem> GetRolePermissions(int roleId)
            {
                var list = new List<RolePermissionItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    P.PermissionID,
                    P.PermissionKey,
                    P.PermissionName,
                    P.Category,
                    CAST(ISNULL(RP.IsAllowed, 0) AS bit) AS IsAllowed
                FROM dbo.Permissions P
                LEFT JOIN dbo.RolePermissions RP
                    ON P.PermissionID = RP.PermissionID
                   AND RP.RoleID = @RoleID
                WHERE P.IsActive = 1
                ORDER BY P.Category, P.PermissionName", con))
                {
                    cmd.Parameters.AddWithValue("@RoleID", roleId);
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new RolePermissionItem
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

            public void SaveRolePermissions(int roleId, List<RolePermissionItem> items)
            {
                if (roleId <= 0)
                    throw new InvalidOperationException("معرف الدور غير صحيح.");

                if (items == null)
                    throw new InvalidOperationException("لا توجد صلاحيات للحفظ.");

                using (SqlConnection con = Db.GetConnection())
                {
                    con.Open();
                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand del = new SqlCommand("DELETE FROM dbo.RolePermissions WHERE RoleID = @RoleID", con, tran))
                            {
                                del.Parameters.AddWithValue("@RoleID", roleId);
                                del.ExecuteNonQuery();
                            }

                            foreach (var item in items)
                            {
                                using (SqlCommand ins = new SqlCommand(@"
                                INSERT INTO dbo.RolePermissions(RoleID, PermissionID, IsAllowed)
                                VALUES(@RoleID, @PermissionID, @IsAllowed)", con, tran))
                                {
                                    ins.Parameters.AddWithValue("@RoleID", roleId);
                                    ins.Parameters.AddWithValue("@PermissionID", item.PermissionID);
                                    ins.Parameters.AddWithValue("@IsAllowed", item.IsAllowed);
                                    ins.ExecuteNonQuery();
                                }
                            }

                            tran.Commit();
                        }
                        catch
                        {
                            tran.Rollback();
                            throw;
                        }
                    }
                }
            }
        }
    }