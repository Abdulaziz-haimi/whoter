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