using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using water3.Models;

namespace water3.Services
{
    public class CollectorDeviceService
    {
        private readonly AuditLogService _audit = new AuditLogService();

        public List<CollectorDeviceItem> GetDevices(string search = null, int? collectorId = null)
        {
            var list = new List<CollectorDeviceItem>();

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    D.DeviceID,
                    D.CollectorID,
                    C.Name AS CollectorName,
                    D.DeviceCode,
                    D.DeviceName,
                    D.PhoneNumber,
                    D.DeviceModel,
                    D.AppVersion,
                    D.IsApproved,
                    D.IsActive,
                    D.CreatedAt,
                    D.LastSyncAt
                FROM dbo.CollectorDevices D
                INNER JOIN dbo.Collectors C ON D.CollectorID = C.CollectorID
                WHERE (@CollectorID IS NULL OR D.CollectorID = @CollectorID)
                  AND (
                        @Search IS NULL OR @Search = N''
                        OR ISNULL(C.Name, N'') LIKE N'%' + @Search + N'%'
                        OR ISNULL(D.DeviceCode, N'') LIKE N'%' + @Search + N'%'
                        OR ISNULL(D.DeviceName, N'') LIKE N'%' + @Search + N'%'
                        OR ISNULL(D.PhoneNumber, N'') LIKE N'%' + @Search + N'%'
                        OR ISNULL(D.DeviceModel, N'') LIKE N'%' + @Search + N'%'
                        OR ISNULL(D.AppVersion, N'') LIKE N'%' + @Search + N'%'
                  )
                ORDER BY C.Name, D.CreatedAt DESC, D.DeviceID DESC;", con))
            {
                cmd.Parameters.Add("@CollectorID", SqlDbType.Int).Value =
                    collectorId.HasValue ? (object)collectorId.Value : DBNull.Value;

                cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 100).Value =
                    string.IsNullOrWhiteSpace(search) ? (object)DBNull.Value : search.Trim();

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new CollectorDeviceItem
                        {
                            DeviceID = Convert.ToInt32(dr["DeviceID"]),
                            CollectorID = Convert.ToInt32(dr["CollectorID"]),
                            CollectorName = Convert.ToString(dr["CollectorName"]),
                            DeviceCode = Convert.ToString(dr["DeviceCode"]),
                            DeviceName = dr["DeviceName"] == DBNull.Value ? null : Convert.ToString(dr["DeviceName"]),
                            PhoneNumber = dr["PhoneNumber"] == DBNull.Value ? null : Convert.ToString(dr["PhoneNumber"]),
                            DeviceModel = dr["DeviceModel"] == DBNull.Value ? null : Convert.ToString(dr["DeviceModel"]),
                            AppVersion = dr["AppVersion"] == DBNull.Value ? null : Convert.ToString(dr["AppVersion"]),
                            IsApproved = Convert.ToBoolean(dr["IsApproved"]),
                            IsActive = Convert.ToBoolean(dr["IsActive"]),
                            CreatedAt = Convert.ToDateTime(dr["CreatedAt"]),
                            LastSyncAt = dr["LastSyncAt"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(dr["LastSyncAt"])
                        });
                    }
                }
            }

            return list;
        }

        public List<CollectorItem> GetCollectors()
        {
            var list = new List<CollectorItem>();

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT CollectorID, Name, Phone
                FROM dbo.Collectors
                ORDER BY Name;", con))
            {
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new CollectorItem
                        {
                            CollectorID = Convert.ToInt32(dr["CollectorID"]),
                            Name = Convert.ToString(dr["Name"]),
                            Phone = dr["Phone"] == DBNull.Value ? null : Convert.ToString(dr["Phone"])
                        });
                    }
                }
            }

            return list;
        }

        public void AddDevice(
            int collectorId,
            string deviceName,
            string phoneNumber,
            string deviceCode,
            string deviceModel,
            string appVersion,
            bool isApproved,
            bool isActive)
        {
            if (collectorId <= 0)
                throw new InvalidOperationException("يجب اختيار المحصل.");

            deviceCode = Normalize(deviceCode);

            if (string.IsNullOrWhiteSpace(deviceCode))
                throw new InvalidOperationException("كود الجهاز مطلوب.");

            string collectorName = null;
            int newDeviceId = 0;

            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();

                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand checkCollectorCmd = new SqlCommand(@"
                            SELECT Name
                            FROM dbo.Collectors
                            WHERE CollectorID = @CollectorID;", con, tran))
                        {
                            checkCollectorCmd.Parameters.Add("@CollectorID", SqlDbType.Int).Value = collectorId;

                            object result = checkCollectorCmd.ExecuteScalar();
                            if (result == null || result == DBNull.Value)
                                throw new InvalidOperationException("المحصل غير موجود.");

                            collectorName = Convert.ToString(result);
                        }

                        using (SqlCommand checkDeviceCmd = new SqlCommand(@"
                            SELECT COUNT(1)
                            FROM dbo.CollectorDevices
                            WHERE DeviceCode = @DeviceCode;", con, tran))
                        {
                            checkDeviceCmd.Parameters.Add("@DeviceCode", SqlDbType.NVarChar, 100).Value = deviceCode;

                            int exists = Convert.ToInt32(checkDeviceCmd.ExecuteScalar());
                            if (exists > 0)
                                throw new InvalidOperationException("كود الجهاز مسجل مسبقًا.");
                        }

                        using (SqlCommand insertCmd = new SqlCommand(@"
                            INSERT INTO dbo.CollectorDevices
                            (
                                CollectorID,
                                DeviceCode,
                                DeviceName,
                                PhoneNumber,
                                DeviceModel,
                                AppVersion,
                                IsApproved,
                                IsActive,
                                CreatedAt
                            )
                            VALUES
                            (
                                @CollectorID,
                                @DeviceCode,
                                @DeviceName,
                                @PhoneNumber,
                                @DeviceModel,
                                @AppVersion,
                                @IsApproved,
                                @IsActive,
                                GETDATE()
                            );

                            SELECT CAST(SCOPE_IDENTITY() AS INT);", con, tran))
                        {
                            insertCmd.Parameters.Add("@CollectorID", SqlDbType.Int).Value = collectorId;
                            insertCmd.Parameters.Add("@DeviceCode", SqlDbType.NVarChar, 100).Value = deviceCode;
                            insertCmd.Parameters.Add("@DeviceName", SqlDbType.NVarChar, 100).Value = ToDb(deviceName);
                            insertCmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value = ToDb(phoneNumber);
                            insertCmd.Parameters.Add("@DeviceModel", SqlDbType.NVarChar, 100).Value = ToDb(deviceModel);
                            insertCmd.Parameters.Add("@AppVersion", SqlDbType.NVarChar, 30).Value = ToDb(appVersion);
                            insertCmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = isApproved;
                            insertCmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive;

                            newDeviceId = Convert.ToInt32(insertCmd.ExecuteScalar());
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

            _audit.Log(
                action: "ADD_COLLECTOR_DEVICE",
                tableName: "CollectorDevices",
                recordId: newDeviceId,
                details: "تم إضافة جهاز للمحصل: " + deviceCode,
                entityName: collectorName);
        }

        public void UpdateDevice(
            int deviceId,
            string deviceName,
            string phoneNumber,
            string deviceCode,
            string deviceModel,
            string appVersion,
            bool isApproved,
            bool isActive)
        {
            if (deviceId <= 0)
                throw new InvalidOperationException("معرف الجهاز غير صحيح.");

            deviceCode = Normalize(deviceCode);

            if (string.IsNullOrWhiteSpace(deviceCode))
                throw new InvalidOperationException("كود الجهاز مطلوب.");

            string collectorName = null;
            string oldDeviceCode = null;

            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();

                using (SqlTransaction tran = con.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand infoCmd = new SqlCommand(@"
                            SELECT TOP 1
                                D.DeviceCode,
                                C.Name AS CollectorName
                            FROM dbo.CollectorDevices D
                            INNER JOIN dbo.Collectors C ON D.CollectorID = C.CollectorID
                            WHERE D.DeviceID = @DeviceID;", con, tran))
                        {
                            infoCmd.Parameters.Add("@DeviceID", SqlDbType.Int).Value = deviceId;

                            using (SqlDataReader dr = infoCmd.ExecuteReader())
                            {
                                if (!dr.Read())
                                    throw new InvalidOperationException("الجهاز غير موجود.");

                                oldDeviceCode = Convert.ToString(dr["DeviceCode"]);
                                collectorName = Convert.ToString(dr["CollectorName"]);
                            }
                        }

                        using (SqlCommand duplicateCmd = new SqlCommand(@"
                            SELECT COUNT(1)
                            FROM dbo.CollectorDevices
                            WHERE DeviceCode = @DeviceCode
                              AND DeviceID <> @DeviceID;", con, tran))
                        {
                            duplicateCmd.Parameters.Add("@DeviceID", SqlDbType.Int).Value = deviceId;
                            duplicateCmd.Parameters.Add("@DeviceCode", SqlDbType.NVarChar, 100).Value = deviceCode;

                            int exists = Convert.ToInt32(duplicateCmd.ExecuteScalar());
                            if (exists > 0)
                                throw new InvalidOperationException("كود الجهاز مستخدم لجهاز آخر.");
                        }

                        using (SqlCommand updateCmd = new SqlCommand(@"
                            UPDATE dbo.CollectorDevices
                            SET
                                DeviceCode = @DeviceCode,
                                DeviceName = @DeviceName,
                                PhoneNumber = @PhoneNumber,
                                DeviceModel = @DeviceModel,
                                AppVersion = @AppVersion,
                                IsApproved = @IsApproved,
                                IsActive = @IsActive
                            WHERE DeviceID = @DeviceID;", con, tran))
                        {
                            updateCmd.Parameters.Add("@DeviceID", SqlDbType.Int).Value = deviceId;
                            updateCmd.Parameters.Add("@DeviceCode", SqlDbType.NVarChar, 100).Value = deviceCode;
                            updateCmd.Parameters.Add("@DeviceName", SqlDbType.NVarChar, 100).Value = ToDb(deviceName);
                            updateCmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value = ToDb(phoneNumber);
                            updateCmd.Parameters.Add("@DeviceModel", SqlDbType.NVarChar, 100).Value = ToDb(deviceModel);
                            updateCmd.Parameters.Add("@AppVersion", SqlDbType.NVarChar, 30).Value = ToDb(appVersion);
                            updateCmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = isApproved;
                            updateCmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive;

                            int rows = updateCmd.ExecuteNonQuery();
                            if (rows == 0)
                                throw new InvalidOperationException("لم يتم تعديل الجهاز.");
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

            _audit.Log(
                action: "UPDATE_COLLECTOR_DEVICE",
                tableName: "CollectorDevices",
                recordId: deviceId,
                details: "تم تعديل بيانات الجهاز من " + oldDeviceCode + " إلى " + deviceCode,
                entityName: collectorName);
        }

        public void SetApproved(int deviceId, bool isApproved)
        {
            if (deviceId <= 0)
                throw new InvalidOperationException("معرف الجهاز غير صحيح.");

            string deviceCode;
            string collectorName;

            GetDeviceAuditInfo(deviceId, out deviceCode, out collectorName);

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE dbo.CollectorDevices
                SET IsApproved = @IsApproved
                WHERE DeviceID = @DeviceID;", con))
            {
                cmd.Parameters.Add("@DeviceID", SqlDbType.Int).Value = deviceId;
                cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = isApproved;

                con.Open();

                int rows = cmd.ExecuteNonQuery();
                if (rows == 0)
                    throw new InvalidOperationException("الجهاز غير موجود.");
            }

            _audit.Log(
                action: isApproved ? "APPROVE_DEVICE" : "UNAPPROVE_DEVICE",
                tableName: "CollectorDevices",
                recordId: deviceId,
                details: (isApproved ? "تم اعتماد الجهاز" : "تم إلغاء اعتماد الجهاز") + ": " + deviceCode,
                entityName: collectorName);
        }

        public void SetActive(int deviceId, bool isActive)
        {
            if (deviceId <= 0)
                throw new InvalidOperationException("معرف الجهاز غير صحيح.");

            string deviceCode;
            string collectorName;

            GetDeviceAuditInfo(deviceId, out deviceCode, out collectorName);

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE dbo.CollectorDevices
                SET IsActive = @IsActive
                WHERE DeviceID = @DeviceID;", con))
            {
                cmd.Parameters.Add("@DeviceID", SqlDbType.Int).Value = deviceId;
                cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive;

                con.Open();

                int rows = cmd.ExecuteNonQuery();
                if (rows == 0)
                    throw new InvalidOperationException("الجهاز غير موجود.");
            }

            _audit.Log(
                action: isActive ? "ACTIVATE_DEVICE" : "DEACTIVATE_DEVICE",
                tableName: "CollectorDevices",
                recordId: deviceId,
                details: (isActive ? "تم تفعيل الجهاز" : "تم تعطيل الجهاز") + ": " + deviceCode,
                entityName: collectorName);
        }

        public void UpdateDeviceName(int deviceId, string deviceName)
        {
            if (deviceId <= 0)
                throw new InvalidOperationException("معرف الجهاز غير صحيح.");

            string deviceCode;
            string collectorName;

            GetDeviceAuditInfo(deviceId, out deviceCode, out collectorName);

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
                UPDATE dbo.CollectorDevices
                SET DeviceName = @DeviceName
                WHERE DeviceID = @DeviceID;", con))
            {
                cmd.Parameters.Add("@DeviceID", SqlDbType.Int).Value = deviceId;
                cmd.Parameters.Add("@DeviceName", SqlDbType.NVarChar, 100).Value = ToDb(deviceName);

                con.Open();

                int rows = cmd.ExecuteNonQuery();
                if (rows == 0)
                    throw new InvalidOperationException("الجهاز غير موجود.");
            }

            _audit.Log(
                action: "UPDATE_DEVICE_NAME",
                tableName: "CollectorDevices",
                recordId: deviceId,
                details: "تم تعديل اسم الجهاز إلى: " + deviceName,
                entityName: collectorName + " - " + deviceCode);
        }

        private void GetDeviceAuditInfo(int deviceId, out string deviceCode, out string collectorName)
        {
            deviceCode = null;
            collectorName = null;

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 1
                    D.DeviceCode,
                    C.Name AS CollectorName
                FROM dbo.CollectorDevices D
                INNER JOIN dbo.Collectors C ON D.CollectorID = C.CollectorID
                WHERE D.DeviceID = @DeviceID;", con))
            {
                cmd.Parameters.Add("@DeviceID", SqlDbType.Int).Value = deviceId;

                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        throw new InvalidOperationException("الجهاز غير موجود.");

                    deviceCode = Convert.ToString(dr["DeviceCode"]);
                    collectorName = Convert.ToString(dr["CollectorName"]);
                }
            }
        }

        private static object ToDb(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DBNull.Value;

            return value.Trim();
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
/*using System;
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
    public class CollectorDeviceService
    {
        private readonly AuditLogService _audit = new AuditLogService();

        public List<CollectorDeviceItem> GetDevices(string search = null, int? collectorId = null)
        {
            var list = new List<CollectorDeviceItem>();

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    D.DeviceID,
                    D.CollectorID,
                    C.Name AS CollectorName,
                    D.DeviceCode,
                    D.DeviceName,
                    D.DeviceModel,
                    D.AppVersion,
                    D.IsApproved,
                    D.IsActive,
                    D.CreatedAt,
                    D.LastSyncAt
                FROM dbo.CollectorDevices D
                INNER JOIN dbo.Collectors C ON D.CollectorID = C.CollectorID
                WHERE (@CollectorID IS NULL OR D.CollectorID = @CollectorID)
                  AND (
                        @Search IS NULL OR @Search = ''
                        OR ISNULL(C.Name, N'') LIKE N'%' + @Search + N'%'
                        OR ISNULL(D.DeviceCode, N'') LIKE N'%' + @Search + N'%'
                        OR ISNULL(D.DeviceName, N'') LIKE N'%' + @Search + N'%'
                        OR ISNULL(D.DeviceModel, N'') LIKE N'%' + @Search + N'%'
                        OR ISNULL(D.AppVersion, N'') LIKE N'%' + @Search + N'%'
                  )
                ORDER BY C.Name, D.CreatedAt DESC, D.DeviceID DESC", con))
            {
                cmd.Parameters.AddWithValue("@CollectorID", collectorId.HasValue ? (object)collectorId.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Search", string.IsNullOrWhiteSpace(search) ? (object)DBNull.Value : search.Trim());
                con.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new CollectorDeviceItem
                        {
                            DeviceID = Convert.ToInt32(dr["DeviceID"]),
                            CollectorID = Convert.ToInt32(dr["CollectorID"]),
                            CollectorName = Convert.ToString(dr["CollectorName"]),
                            DeviceCode = Convert.ToString(dr["DeviceCode"]),
                            DeviceName = dr["DeviceName"] == DBNull.Value ? null : Convert.ToString(dr["DeviceName"]),
                            DeviceModel = dr["DeviceModel"] == DBNull.Value ? null : Convert.ToString(dr["DeviceModel"]),
                            AppVersion = dr["AppVersion"] == DBNull.Value ? null : Convert.ToString(dr["AppVersion"]),
                            IsApproved = Convert.ToBoolean(dr["IsApproved"]),
                            IsActive = Convert.ToBoolean(dr["IsActive"]),
                            CreatedAt = Convert.ToDateTime(dr["CreatedAt"]),
                            LastSyncAt = dr["LastSyncAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["LastSyncAt"])
                        });
                    }
                }
            }

            return list;
        }

        public List<CollectorItem> GetCollectors()
        {
            var list = new List<CollectorItem>();

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT CollectorID, Name, Phone
                FROM dbo.Collectors
                ORDER BY Name", con))
            {
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new CollectorItem
                        {
                            CollectorID = Convert.ToInt32(dr["CollectorID"]),
                            Name = Convert.ToString(dr["Name"]),
                            Phone = dr["Phone"] == DBNull.Value ? null : Convert.ToString(dr["Phone"])
                        });
                    }
                }
            }

            return list;
        }

        public void SetApproved(int deviceId, bool isApproved)
        {
            if (deviceId <= 0)
                throw new InvalidOperationException("معرف الجهاز غير صحيح.");

            string deviceCode = null;
            string collectorName = null;

            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();

                using (SqlCommand infoCmd = new SqlCommand(@"
                    SELECT TOP 1 D.DeviceCode, C.Name
                    FROM dbo.CollectorDevices D
                    INNER JOIN dbo.Collectors C ON D.CollectorID = C.CollectorID
                    WHERE D.DeviceID = @DeviceID", con))
                {
                    infoCmd.Parameters.AddWithValue("@DeviceID", deviceId);
                    using (SqlDataReader dr = infoCmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            deviceCode = Convert.ToString(dr[0]);
                            collectorName = Convert.ToString(dr[1]);
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE dbo.CollectorDevices
                    SET IsApproved = @IsApproved
                    WHERE DeviceID = @DeviceID", con))
                {
                    cmd.Parameters.AddWithValue("@DeviceID", deviceId);
                    cmd.Parameters.AddWithValue("@IsApproved", isApproved);
                    cmd.ExecuteNonQuery();
                }
            }

            _audit.Log(
                action: isApproved ? "APPROVE_DEVICE" : "UNAPPROVE_DEVICE",
                tableName: "CollectorDevices",
                recordId: deviceId,
                details: (isApproved ? "تم اعتماد الجهاز" : "تم إلغاء اعتماد الجهاز") + $": {deviceCode}",
                entityName: collectorName);
        }

        public void SetActive(int deviceId, bool isActive)
        {
            if (deviceId <= 0)
                throw new InvalidOperationException("معرف الجهاز غير صحيح.");

            string deviceCode = null;
            string collectorName = null;

            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();

                using (SqlCommand infoCmd = new SqlCommand(@"
                    SELECT TOP 1 D.DeviceCode, C.Name
                    FROM dbo.CollectorDevices D
                    INNER JOIN dbo.Collectors C ON D.CollectorID = C.CollectorID
                    WHERE D.DeviceID = @DeviceID", con))
                {
                    infoCmd.Parameters.AddWithValue("@DeviceID", deviceId);
                    using (SqlDataReader dr = infoCmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            deviceCode = Convert.ToString(dr[0]);
                            collectorName = Convert.ToString(dr[1]);
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE dbo.CollectorDevices
                    SET IsActive = @IsActive
                    WHERE DeviceID = @DeviceID", con))
                {
                    cmd.Parameters.AddWithValue("@DeviceID", deviceId);
                    cmd.Parameters.AddWithValue("@IsActive", isActive);
                    cmd.ExecuteNonQuery();
                }
            }

            _audit.Log(
                action: isActive ? "ACTIVATE_DEVICE" : "DEACTIVATE_DEVICE",
                tableName: "CollectorDevices",
                recordId: deviceId,
                details: (isActive ? "تم تفعيل الجهاز" : "تم تعطيل الجهاز") + $": {deviceCode}",
                entityName: collectorName);
        }

        public void UpdateDeviceName(int deviceId, string deviceName)
        {
            if (deviceId <= 0)
                throw new InvalidOperationException("معرف الجهاز غير صحيح.");

            string deviceCode = null;
            string collectorName = null;

            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();

                using (SqlCommand infoCmd = new SqlCommand(@"
                    SELECT TOP 1 D.DeviceCode, C.Name
                    FROM dbo.CollectorDevices D
                    INNER JOIN dbo.Collectors C ON D.CollectorID = C.CollectorID
                    WHERE D.DeviceID = @DeviceID", con))
                {
                    infoCmd.Parameters.AddWithValue("@DeviceID", deviceId);
                    using (SqlDataReader dr = infoCmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            deviceCode = Convert.ToString(dr[0]);
                            collectorName = Convert.ToString(dr[1]);
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(@"
                    UPDATE dbo.CollectorDevices
                    SET DeviceName = @DeviceName
                    WHERE DeviceID = @DeviceID", con))
                {
                    cmd.Parameters.AddWithValue("@DeviceID", deviceId);
                    cmd.Parameters.AddWithValue("@DeviceName", string.IsNullOrWhiteSpace(deviceName) ? (object)DBNull.Value : deviceName.Trim());
                    cmd.ExecuteNonQuery();
                }
            }

            _audit.Log(
                action: "UPDATE_DEVICE_NAME",
                tableName: "CollectorDevices",
                recordId: deviceId,
                details: $"تم تعديل اسم الجهاز إلى: {deviceName}",
                entityName: collectorName + " - " + deviceCode);
        }
    }
}
*/