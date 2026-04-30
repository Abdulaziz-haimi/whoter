using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System;
using System.Data;
using System.Data.SqlClient;
namespace water3.Repositories
{

        public class CollectorDevicesRepository
        {
            private readonly string _connStr = Db.ConnectionString;

            public DataTable GetCollectors()
            {
                using (SqlConnection con = new SqlConnection(_connStr))
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT 
                    CollectorID,
                    Name,
                    Phone
                FROM dbo.Collectors
                ORDER BY Name;", con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }

            public DataTable GetDevicesByCollector(int collectorId)
            {
                using (SqlConnection con = new SqlConnection(_connStr))
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    D.DeviceID,
                    D.CollectorID,
                    C.Name AS CollectorName,
                    D.DeviceName,
                    D.PhoneNumber,
                    D.DeviceCode,
                    D.DeviceModel,
                    D.AppVersion,
                    D.IsApproved,
                    D.IsActive,
                    D.LastSyncAt,
                    D.CreatedAt
                FROM dbo.CollectorDevices D
                INNER JOIN dbo.Collectors C ON C.CollectorID = D.CollectorID
                WHERE D.CollectorID = @CollectorID
                ORDER BY D.CreatedAt DESC, D.DeviceID DESC;", con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.Add("@CollectorID", SqlDbType.Int).Value = collectorId;

                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }

            public int InsertDevice(
                int collectorId,
                string deviceName,
                string phoneNumber,
                string deviceCode,
                string deviceModel,
                string appVersion,
                bool isApproved,
                bool isActive)
            {
                using (SqlConnection con = new SqlConnection(_connStr))
                using (SqlCommand cmd = new SqlCommand(@"
                IF NOT EXISTS (
                    SELECT 1 FROM dbo.Collectors WHERE CollectorID = @CollectorID
                )
                    THROW 51001, N'المحصل غير موجود', 1;

                IF EXISTS (
                    SELECT 1 FROM dbo.CollectorDevices WHERE DeviceCode = @DeviceCode
                )
                    THROW 51002, N'كود الجهاز مسجل مسبقًا', 1;

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

                SELECT CAST(SCOPE_IDENTITY() AS INT);", con))
                {
                    cmd.Parameters.Add("@CollectorID", SqlDbType.Int).Value = collectorId;
                    cmd.Parameters.Add("@DeviceName", SqlDbType.NVarChar, 100).Value = ToDb(deviceName);
                    cmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value = ToDb(phoneNumber);
                    cmd.Parameters.Add("@DeviceCode", SqlDbType.NVarChar, 100).Value = deviceCode.Trim();
                    cmd.Parameters.Add("@DeviceModel", SqlDbType.NVarChar, 100).Value = ToDb(deviceModel);
                    cmd.Parameters.Add("@AppVersion", SqlDbType.NVarChar, 30).Value = ToDb(appVersion);
                    cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = isApproved;
                    cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive;

                    con.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
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
            using (SqlConnection con = new SqlConnection(Db.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
        IF NOT EXISTS (
            SELECT 1 FROM dbo.Collectors WHERE CollectorID = @CollectorID
        )
            THROW 51001, N'المحصل غير موجود', 1;

        IF EXISTS (
            SELECT 1 FROM dbo.CollectorDevices WHERE DeviceCode = @DeviceCode
        )
            THROW 51002, N'كود الجهاز مسجل مسبقًا', 1;

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
        );", con))
            {
                cmd.Parameters.AddWithValue("@CollectorID", collectorId);
                cmd.Parameters.AddWithValue("@DeviceCode", deviceCode);
                cmd.Parameters.AddWithValue("@DeviceName", string.IsNullOrWhiteSpace(deviceName) ? (object)DBNull.Value : deviceName);
                cmd.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(phoneNumber) ? (object)DBNull.Value : phoneNumber);
                cmd.Parameters.AddWithValue("@DeviceModel", string.IsNullOrWhiteSpace(deviceModel) ? (object)DBNull.Value : deviceModel);
                cmd.Parameters.AddWithValue("@AppVersion", string.IsNullOrWhiteSpace(appVersion) ? (object)DBNull.Value : appVersion);
                cmd.Parameters.AddWithValue("@IsApproved", isApproved);
                cmd.Parameters.AddWithValue("@IsActive", isActive);

                con.Open();
                cmd.ExecuteNonQuery();
            }
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
            using (SqlConnection con = new SqlConnection(Db.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(@"
        IF EXISTS (
            SELECT 1 
            FROM dbo.CollectorDevices
            WHERE DeviceCode = @DeviceCode
              AND DeviceID <> @DeviceID
        )
            THROW 51003, N'كود الجهاز مستخدم لجهاز آخر', 1;

        UPDATE dbo.CollectorDevices
        SET
            DeviceCode = @DeviceCode,
            DeviceName = @DeviceName,
            PhoneNumber = @PhoneNumber,
            DeviceModel = @DeviceModel,
            AppVersion = @AppVersion,
            IsApproved = @IsApproved,
            IsActive = @IsActive
        WHERE DeviceID = @DeviceID;

        IF @@ROWCOUNT = 0
            THROW 51004, N'الجهاز غير موجود', 1;", con))
            {
                cmd.Parameters.AddWithValue("@DeviceID", deviceId);
                cmd.Parameters.AddWithValue("@DeviceCode", deviceCode);
                cmd.Parameters.AddWithValue("@DeviceName", string.IsNullOrWhiteSpace(deviceName) ? (object)DBNull.Value : deviceName);
                cmd.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(phoneNumber) ? (object)DBNull.Value : phoneNumber);
                cmd.Parameters.AddWithValue("@DeviceModel", string.IsNullOrWhiteSpace(deviceModel) ? (object)DBNull.Value : deviceModel);
                cmd.Parameters.AddWithValue("@AppVersion", string.IsNullOrWhiteSpace(appVersion) ? (object)DBNull.Value : appVersion);
                cmd.Parameters.AddWithValue("@IsApproved", isApproved);
                cmd.Parameters.AddWithValue("@IsActive", isActive);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
        //public void UpdateDevice(
        //        int deviceId,
        //        string deviceName,
        //        string phoneNumber,
        //        string deviceCode,
        //        string deviceModel,
        //        string appVersion,
        //        bool isApproved,
        //        bool isActive)
        //    {
        //        using (SqlConnection con = new SqlConnection(_connStr))
        //        using (SqlCommand cmd = new SqlCommand(@"
        //        IF EXISTS (
        //            SELECT 1 
        //            FROM dbo.CollectorDevices 
        //            WHERE DeviceCode = @DeviceCode
        //              AND DeviceID <> @DeviceID
        //        )
        //            THROW 51003, N'كود الجهاز مستخدم لجهاز آخر', 1;

        //        UPDATE dbo.CollectorDevices
        //        SET
        //            DeviceName = @DeviceName,
        //            PhoneNumber = @PhoneNumber,
        //            DeviceCode = @DeviceCode,
        //            DeviceModel = @DeviceModel,
        //            AppVersion = @AppVersion,
        //            IsApproved = @IsApproved,
        //            IsActive = @IsActive
        //        WHERE DeviceID = @DeviceID;

        //        IF @@ROWCOUNT = 0
        //            THROW 51004, N'الجهاز غير موجود', 1;", con))
        //        {
        //            cmd.Parameters.Add("@DeviceID", SqlDbType.Int).Value = deviceId;
        //            cmd.Parameters.Add("@DeviceName", SqlDbType.NVarChar, 100).Value = ToDb(deviceName);
        //            cmd.Parameters.Add("@PhoneNumber", SqlDbType.NVarChar, 20).Value = ToDb(phoneNumber);
        //            cmd.Parameters.Add("@DeviceCode", SqlDbType.NVarChar, 100).Value = deviceCode.Trim();
        //            cmd.Parameters.Add("@DeviceModel", SqlDbType.NVarChar, 100).Value = ToDb(deviceModel);
        //            cmd.Parameters.Add("@AppVersion", SqlDbType.NVarChar, 30).Value = ToDb(appVersion);
        //            cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = isApproved;
        //            cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive;

        //            con.Open();
        //            cmd.ExecuteNonQuery();
        //        }
        //    }

            public void SetActive(int deviceId, bool isActive)
            {
                using (SqlConnection con = new SqlConnection(_connStr))
                using (SqlCommand cmd = new SqlCommand(@"
                UPDATE dbo.CollectorDevices
                SET IsActive = @IsActive
                WHERE DeviceID = @DeviceID;

                IF @@ROWCOUNT = 0
                    THROW 51005, N'الجهاز غير موجود', 1;", con))
                {
                    cmd.Parameters.Add("@DeviceID", SqlDbType.Int).Value = deviceId;
                    cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = isActive;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            public void SetApproved(int deviceId, bool isApproved)
            {
                using (SqlConnection con = new SqlConnection(_connStr))
                using (SqlCommand cmd = new SqlCommand(@"
                UPDATE dbo.CollectorDevices
                SET IsApproved = @IsApproved
                WHERE DeviceID = @DeviceID;

                IF @@ROWCOUNT = 0
                    THROW 51006, N'الجهاز غير موجود', 1;", con))
                {
                    cmd.Parameters.Add("@DeviceID", SqlDbType.Int).Value = deviceId;
                    cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = isApproved;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

            private static object ToDb(string value)
            {
                if (string.IsNullOrWhiteSpace(value))
                    return DBNull.Value;

                return value.Trim();
            }
        }
    }