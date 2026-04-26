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
  

        public class MobileSyncToPhoneService
        {
            private readonly AuditLogService _audit = new AuditLogService();

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

            public List<CollectorDeviceItem> GetApprovedActiveDevices(int collectorId)
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
                WHERE D.CollectorID = @CollectorID
                  AND D.IsApproved = 1
                  AND D.IsActive = 1
                ORDER BY D.DeviceName, D.DeviceID", con))
                {
                    cmd.Parameters.AddWithValue("@CollectorID", collectorId);
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

            public MobileExportSummaryItem ExportPreview(
                int collectorId,
                string collectorName,
                string deviceCode,
                string deviceName,
                DateTime asOfDate,
                bool onlyAssignedSubscribers,
                bool includeAllIfNoAssignments)
            {
                int subscribersCount = 0;
                int metersCount = 0;
                int openInvoicesCount = 0;
                int creditsCount = 0;
                decimal totalOpenInvoicesAmount = 0;
                decimal totalCreditsAmount = 0;

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.usp_MobileSync_ExportReceivables", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CollectorID", collectorId);
                    cmd.Parameters.AddWithValue("@AsOfDate", asOfDate.Date);
                    cmd.Parameters.AddWithValue("@OnlyAssignedSubscribers", onlyAssignedSubscribers);
                    cmd.Parameters.AddWithValue("@IncludeAllIfNoAssignments", includeAllIfNoAssignments);

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        // النتيجة الأولى عادة: subscribers
                        while (dr.Read())
                            subscribersCount++;

                        // الثانية: meters
                        if (dr.NextResult())
                        {
                            while (dr.Read())
                                metersCount++;
                        }

                        // الثالثة: open invoices
                        if (dr.NextResult())
                        {
                            while (dr.Read())
                            {
                                openInvoicesCount++;
                                if (dr["Remaining"] != DBNull.Value)
                                    totalOpenInvoicesAmount += Convert.ToDecimal(dr["Remaining"]);
                            }
                        }

                        // الرابعة: credits
                        if (dr.NextResult())
                        {
                            while (dr.Read())
                            {
                                creditsCount++;
                                if (dr["AmountRemaining"] != DBNull.Value)
                                    totalCreditsAmount += Convert.ToDecimal(dr["AmountRemaining"]);
                            }
                        }
                    }
                }

                return new MobileExportSummaryItem
                {
                    CollectorID = collectorId,
                    CollectorName = collectorName,
                    DeviceCode = deviceCode,
                    DeviceName = deviceName,
                    AsOfDateText = asOfDate.ToString("yyyy-MM-dd"),
                    SubscribersCount = subscribersCount,
                    MetersCount = metersCount,
                    OpenInvoicesCount = openInvoicesCount,
                    CreditsCount = creditsCount,
                    TotalOpenInvoicesAmount = totalOpenInvoicesAmount,
                    TotalCreditsAmount = totalCreditsAmount
                };
            }

            public void RegisterSyncPushLog(MobileExportSummaryItem summary)
            {
                _audit.Log(
                    action: "SYNC_TO_PHONE",
                    tableName: "CollectorDevices",
                    recordId: null,
                    details: $"تم تجهيز مزامنة إلى الهاتف للمحصل {summary.CollectorName} / الجهاز {summary.DeviceCode} / مشتركون={summary.SubscribersCount} / فواتير={summary.OpenInvoicesCount}",
                    entityName: summary.DeviceName);
            }
        }
    }