using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WaterCollector.BackendApi.Services;
using WaterCollector.BackendApi.Data;
using WaterCollector.BackendApi.Security;

namespace WaterCollector.BackendApi.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public AuthService(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.UserName) || string.IsNullOrWhiteSpace(request.Password))
                throw new InvalidOperationException("اسم المستخدم وكلمة المرور مطلوبان.");

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync().ConfigureAwait(false);
                var sql = await ResolveUserCollectorQueryAsync(connection).ConfigureAwait(false);
                using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@UserName", request.UserName.Trim());
                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (!await reader.ReadAsync().ConfigureAwait(false))
                            throw new UnauthorizedAccessException("بيانات الدخول غير صحيحة.");

                        var isActive = reader["IsActive"] == DBNull.Value || Convert.ToBoolean(reader["IsActive"]);
                        if (!isActive) throw new UnauthorizedAccessException("المستخدم غير نشط.");

                        var passwordHash = Convert.ToString(reader["PasswordHash"]) ?? string.Empty;
                        var passwordMode = ConfigurationManager.AppSettings["Authentication:PasswordMode"] ?? "Sha256Hex";
                        var candidate = string.Equals(passwordMode, "PlainText", StringComparison.OrdinalIgnoreCase)
                            ? request.Password
                            : PasswordHasher.Sha256Hex(request.Password);
                        if (!string.Equals(passwordHash, candidate, StringComparison.OrdinalIgnoreCase))
                            throw new UnauthorizedAccessException("بيانات الدخول غير صحيحة.");

                        var collectorId = reader["CollectorID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CollectorID"]);
                        var collectorName = reader["CollectorName"] == DBNull.Value ? string.Empty : Convert.ToString(reader["CollectorName"]);
                        if (collectorId <= 0) throw new InvalidOperationException("المستخدم لا يملك ربطًا مع محصل.");

                        DateTime expiresAt;
                        var token = new BearerTokenService().CreateToken(
                            userId: Convert.ToInt32(reader["UserID"]),
                            userName: Convert.ToString(reader["UserName"]),
                            fullName: reader["FullName"] == DBNull.Value ? null : Convert.ToString(reader["FullName"]),
                            collectorId: collectorId,
                            collectorName: collectorName,
                            expiresAt: out expiresAt);

                        return new LoginResponse
                        {
                            Token = token,
                            ExpiresAt = expiresAt,
                            UserId = Convert.ToInt32(reader["UserID"]),
                            UserName = Convert.ToString(reader["UserName"]),
                            FullName = reader["FullName"] == DBNull.Value ? null : Convert.ToString(reader["FullName"]),
                            CollectorId = collectorId,
                            CollectorName = collectorName,
                            DeviceCode = request.DeviceCode
                        };
                    }
                }
            }
        }

        private static async Task<string> ResolveUserCollectorQueryAsync(SqlConnection connection)
        {
            const string hasCollectorIdSql = "SELECT CASE WHEN COL_LENGTH('dbo.Users','CollectorID') IS NOT NULL THEN 1 ELSE 0 END;";
            const string hasUserCollectorsSql = "SELECT CASE WHEN OBJECT_ID('dbo.UserCollectors','U') IS NOT NULL THEN 1 ELSE 0 END;";

            using (var cmd = new SqlCommand(hasCollectorIdSql, connection))
            {
                var hasCollectorId = Convert.ToInt32(await cmd.ExecuteScalarAsync().ConfigureAwait(false)) == 1;
                if (hasCollectorId)
                {
                    return @"
SELECT TOP 1 U.UserID, U.UserName, U.FullName, U.PasswordHash, U.IsActive, C.CollectorID, C.Name AS CollectorName
FROM dbo.Users U
LEFT JOIN dbo.Collectors C ON C.CollectorID = U.CollectorID
WHERE U.UserName = @UserName;";
                }
            }

            using (var cmd = new SqlCommand(hasUserCollectorsSql, connection))
            {
                var hasUserCollectors = Convert.ToInt32(await cmd.ExecuteScalarAsync().ConfigureAwait(false)) == 1;
                if (hasUserCollectors)
                {
                    return @"
SELECT TOP 1 U.UserID, U.UserName, U.FullName, U.PasswordHash, U.IsActive, C.CollectorID, C.Name AS CollectorName
FROM dbo.Users U
LEFT JOIN dbo.UserCollectors UC ON UC.UserID = U.UserID
LEFT JOIN dbo.Collectors C ON C.CollectorID = UC.CollectorID
WHERE U.UserName = @UserName;";
                }
            }

            return @"
SELECT TOP 1 U.UserID, U.UserName, U.FullName, U.PasswordHash, U.IsActive, C.CollectorID, C.Name AS CollectorName
FROM dbo.Users U
LEFT JOIN dbo.Collectors C ON C.Phone = U.Phone
WHERE U.UserName = @UserName;";
        }
    }
}
