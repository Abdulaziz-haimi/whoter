using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using water3.Models;
using System.Linq;
namespace water3.Services
{
    public class UserAuthService
    {
        private readonly PermissionService _permissionService = new
        PermissionService();
        private readonly AuditLogService _audit = new AuditLogService();
        public List<AppRole> GetRoles()
        {
            var list = new List<AppRole>();

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT RoleID, RoleName
                FROM Roles
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

        public bool UserNameExists(string userName)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT COUNT(1)
                FROM Users
                WHERE UserName = @UserName", con))
            {
                cmd.Parameters.AddWithValue("@UserName", userName.Trim());
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public int CreateUser(string userName, string fullName, string password, int roleId, string phone, bool isActive)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new InvalidOperationException("اسم المستخدم مطلوب.");

            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("كلمة المرور مطلوبة.");

            if (roleId <= 0)
                throw new InvalidOperationException("اختر الصلاحية.");

            if (UserNameExists(userName))
                throw new InvalidOperationException("اسم المستخدم موجود مسبقًا.");

            string hash = HashPassword(password);

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
                INSERT INTO Users
                (
                    UserName,
                    FullName,
                    PasswordHash,
                    RoleID,
                    IsActive,
                    Phone,
                    CreatedDate
                )
                VALUES
                (
                    @UserName,
                    @FullName,
                    @PasswordHash,
                    @RoleID,
                    @IsActive,
                    @Phone,
                    GETDATE()
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT);", con))
            {
                cmd.Parameters.AddWithValue("@UserName", userName.Trim());
                cmd.Parameters.AddWithValue("@FullName", (object)(fullName?.Trim() ?? string.Empty));
                cmd.Parameters.AddWithValue("@PasswordHash", hash);
                cmd.Parameters.AddWithValue("@RoleID", roleId);
                cmd.Parameters.AddWithValue("@IsActive", isActive);
                cmd.Parameters.AddWithValue("@Phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone.Trim());

                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar());

            }
        }

        public AppUser ValidateUser(string userName, string password)
        {
            string enteredHash = HashPassword(password);

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 1
                    U.UserID,
                    U.UserName,
                    U.FullName,
                    U.PasswordHash,
                    U.RoleID,
                    R.RoleName,
                    ISNULL(U.IsActive, 1) AS IsActive,
                    U.Phone
                FROM Users U
                INNER JOIN Roles R ON U.RoleID = R.RoleID
                WHERE U.UserName = @UserName
                  AND ISNULL(U.IsActive, 1) = 1", con))
            {
                cmd.Parameters.AddWithValue("@UserName", userName.Trim());
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        return null;

                    string storedHash = Convert.ToString(dr["PasswordHash"]);
                    if (!string.Equals(storedHash, enteredHash, StringComparison.OrdinalIgnoreCase))
                        return null;

                    var user = new AppUser
                    {
                        UserID = Convert.ToInt32(dr["UserID"]),
                        UserName = Convert.ToString(dr["UserName"]),
                        FullName = Convert.ToString(dr["FullName"]),
                        PasswordHash = storedHash,
                        RoleID = Convert.ToInt32(dr["RoleID"]),
                        RoleName = Convert.ToString(dr["RoleName"]),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        Phone = dr["Phone"] == DBNull.Value ? null : Convert.ToString(dr["Phone"])
                    };

                    return user;

                }
            }
        }

        public string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        public List<AppUser> GetUsers(string search = null)
        {
            var list = new List<AppUser>();

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT
            U.UserID,
            U.UserName,
            U.FullName,
            U.PasswordHash,
            U.RoleID,
            R.RoleName,
            ISNULL(U.IsActive, 1) AS IsActive,
            U.Phone
        FROM Users U
        INNER JOIN Roles R ON U.RoleID = R.RoleID
        WHERE (@Search IS NULL OR @Search = ''
               OR U.UserName LIKE N'%' + @Search + N'%'
               OR ISNULL(U.FullName, N'') LIKE N'%' + @Search + N'%'
               OR ISNULL(U.Phone, N'') LIKE N'%' + @Search + N'%')
        ORDER BY U.UserID DESC", con))
            {
                cmd.Parameters.AddWithValue("@Search", string.IsNullOrWhiteSpace(search) ? (object)DBNull.Value : search.Trim());
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new AppUser
                        {
                            UserID = Convert.ToInt32(dr["UserID"]),
                            UserName = Convert.ToString(dr["UserName"]),
                            FullName = Convert.ToString(dr["FullName"]),
                            PasswordHash = Convert.ToString(dr["PasswordHash"]),
                            RoleID = Convert.ToInt32(dr["RoleID"]),
                            RoleName = Convert.ToString(dr["RoleName"]),
                            IsActive = Convert.ToBoolean(dr["IsActive"]),
                            Phone = dr["Phone"] == DBNull.Value ? null : Convert.ToString(dr["Phone"])
                        });
                    }
                }
            }

            return list;
        }

        public AppUser GetUserById(int userId)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT TOP 1
            U.UserID,
            U.UserName,
            U.FullName,
            U.PasswordHash,
            U.RoleID,
            R.RoleName,
            ISNULL(U.IsActive, 1) AS IsActive,
            U.Phone
        FROM Users U
        INNER JOIN Roles R ON U.RoleID = R.RoleID
        WHERE U.UserID = @UserID", con))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        return null;

                    return new AppUser
                    {
                        UserID = Convert.ToInt32(dr["UserID"]),
                        UserName = Convert.ToString(dr["UserName"]),
                        FullName = Convert.ToString(dr["FullName"]),
                        PasswordHash = Convert.ToString(dr["PasswordHash"]),
                        RoleID = Convert.ToInt32(dr["RoleID"]),
                        RoleName = Convert.ToString(dr["RoleName"]),
                        IsActive = Convert.ToBoolean(dr["IsActive"]),
                        Phone = dr["Phone"] == DBNull.Value ? null : Convert.ToString(dr["Phone"])
                    };
                }
            }
        }

        public void UpdateUser(int userId, string userName, string fullName, int roleId, string phone, bool isActive)
        {
            if (userId <= 0)
                throw new InvalidOperationException("معرف المستخدم غير صحيح.");

            if (string.IsNullOrWhiteSpace(userName))
                throw new InvalidOperationException("اسم المستخدم مطلوب.");

            if (roleId <= 0)
                throw new InvalidOperationException("اختر الصلاحية.");

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
        IF EXISTS (
            SELECT 1
            FROM Users
            WHERE UserName = @UserName
              AND UserID <> @UserID
        )
        BEGIN
            RAISERROR(N'اسم المستخدم موجود لمستخدم آخر.', 16, 1);
            RETURN;
        END

        UPDATE Users
        SET
            UserName = @UserName,
            FullName = @FullName,
            RoleID = @RoleID,
            Phone = @Phone,
            IsActive = @IsActive
        WHERE UserID = @UserID", con))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@UserName", userName.Trim());
                cmd.Parameters.AddWithValue("@FullName", (object)(fullName?.Trim() ?? string.Empty));
                cmd.Parameters.AddWithValue("@RoleID", roleId);
                cmd.Parameters.AddWithValue("@Phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone.Trim());
                cmd.Parameters.AddWithValue("@IsActive", isActive);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void SetUserActive(int userId, bool isActive)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
        UPDATE Users
        SET IsActive = @IsActive
        WHERE UserID = @UserID", con))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@IsActive", isActive);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void ResetPassword(int userId, string newPassword)
        {
            if (userId <= 0)
                throw new InvalidOperationException("معرف المستخدم غير صحيح.");

            if (string.IsNullOrWhiteSpace(newPassword))
                throw new InvalidOperationException("كلمة المرور الجديدة مطلوبة.");

            string hash = HashPassword(newPassword);

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
        UPDATE Users
        SET PasswordHash = @PasswordHash
        WHERE UserID = @UserID", con))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                cmd.Parameters.AddWithValue("@PasswordHash", hash);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
  

}
