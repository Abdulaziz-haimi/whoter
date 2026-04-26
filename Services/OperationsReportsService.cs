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
        public class OperationsReportsService
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

            public List<CollectorCollectionsReportItem> GetCollectorCollections(DateTime fromDate, DateTime toDate, int? collectorId)
            {
                var list = new List<CollectorCollectionsReportItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    C.CollectorID,
                    C.Name AS CollectorName,
                    COUNT(P.PaymentID) AS PaymentsCount,
                    COUNT(DISTINCT R.ReceiptID) AS ReceiptsCount,
                    CAST(ISNULL(SUM(P.Amount),0) AS DECIMAL(18,2)) AS TotalCollected,
                    CAST(ISNULL(SUM(CASE WHEN P.PaymentCategory = N'NormalPayment' THEN P.Amount ELSE 0 END),0) AS DECIMAL(18,2)) AS InvoicePayments,
                    CAST(ISNULL(SUM(CASE WHEN P.PaymentCategory = N'AdvanceCredit' THEN P.Amount ELSE 0 END),0) AS DECIMAL(18,2)) AS AdvanceCredits,
                    CAST(ISNULL(SUM(CASE WHEN P.PaymentCategory = N'CreditSettlement' THEN P.Amount ELSE 0 END),0) AS DECIMAL(18,2)) AS CreditSettlements
                FROM dbo.Collectors C
                LEFT JOIN dbo.Payments P ON C.CollectorID = P.CollectorID
                    AND P.PaymentDate BETWEEN @FromDate AND @ToDate
                LEFT JOIN dbo.Receipts R ON P.ReceiptID = R.ReceiptID
                WHERE (@CollectorID IS NULL OR C.CollectorID = @CollectorID)
                GROUP BY C.CollectorID, C.Name
                ORDER BY C.Name", con))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Date);
                    cmd.Parameters.AddWithValue("@CollectorID", collectorId.HasValue ? (object)collectorId.Value : DBNull.Value);

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new CollectorCollectionsReportItem
                            {
                                CollectorID = Convert.ToInt32(dr["CollectorID"]),
                                CollectorName = Convert.ToString(dr["CollectorName"]),
                                PaymentsCount = Convert.ToInt32(dr["PaymentsCount"]),
                                ReceiptsCount = Convert.ToInt32(dr["ReceiptsCount"]),
                                TotalCollected = Convert.ToDecimal(dr["TotalCollected"]),
                                InvoicePayments = Convert.ToDecimal(dr["InvoicePayments"]),
                                AdvanceCredits = Convert.ToDecimal(dr["AdvanceCredits"]),
                                CreditSettlements = Convert.ToDecimal(dr["CreditSettlements"])
                            });
                        }
                    }
                }

                return list;
            }

            public List<CollectorDevicesReportItem> GetCollectorDevices(int? collectorId)
            {
                var list = new List<CollectorDevicesReportItem>();

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
                ORDER BY C.Name, D.DeviceName, D.DeviceID", con))
                {
                    cmd.Parameters.AddWithValue("@CollectorID", collectorId.HasValue ? (object)collectorId.Value : DBNull.Value);
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new CollectorDevicesReportItem
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

            public List<MobileSyncBatchReportItem> GetMobileSyncBatches(DateTime fromDate, DateTime toDate, int? collectorId)
            {
                var list = new List<MobileSyncBatchReportItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    B.SyncBatchID,
                    B.CollectorID,
                    C.Name AS CollectorName,
                    B.DeviceID,
                    D.DeviceCode,
                    D.DeviceName,
                    B.BatchDate,
                    B.BatchStatus,
                    ISNULL(B.ItemsCount,0) AS ItemsCount,
                    ISNULL(B.ApprovedCount,0) AS ApprovedCount,
                    ISNULL(B.RejectedCount,0) AS RejectedCount,
                    B.ProcessedAt
                FROM dbo.MobileSyncBatches B
                INNER JOIN dbo.Collectors C ON B.CollectorID = C.CollectorID
                INNER JOIN dbo.CollectorDevices D ON B.DeviceID = D.DeviceID
                WHERE B.BatchDate BETWEEN @FromDate AND @ToDate
                  AND (@CollectorID IS NULL OR B.CollectorID = @CollectorID)
                ORDER BY B.BatchDate DESC, B.SyncBatchID DESC", con))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Date);
                    cmd.Parameters.AddWithValue("@CollectorID", collectorId.HasValue ? (object)collectorId.Value : DBNull.Value);
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new MobileSyncBatchReportItem
                            {
                                SyncBatchID = Convert.ToInt32(dr["SyncBatchID"]),
                                CollectorID = Convert.ToInt32(dr["CollectorID"]),
                                CollectorName = Convert.ToString(dr["CollectorName"]),
                                DeviceID = Convert.ToInt32(dr["DeviceID"]),
                                DeviceCode = Convert.ToString(dr["DeviceCode"]),
                                DeviceName = dr["DeviceName"] == DBNull.Value ? null : Convert.ToString(dr["DeviceName"]),
                                BatchDate = Convert.ToDateTime(dr["BatchDate"]),
                                BatchStatus = dr["BatchStatus"] == DBNull.Value ? null : Convert.ToString(dr["BatchStatus"]),
                                ItemsCount = Convert.ToInt32(dr["ItemsCount"]),
                                ApprovedCount = Convert.ToInt32(dr["ApprovedCount"]),
                                RejectedCount = Convert.ToInt32(dr["RejectedCount"]),
                                ProcessedAt = dr["ProcessedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["ProcessedAt"])
                            });
                        }
                    }
                }

                return list;
            }

            public List<MobileSyncErrorReportItem> GetMobileSyncErrors(DateTime fromDate, DateTime toDate)
            {
                var list = new List<MobileSyncErrorReportItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    ErrorID,
                    SyncBatchID,
                    ImportID,
                    ErrorCode,
                    ErrorMessage,
                    CreatedAt
                FROM dbo.MobileSyncErrors
                WHERE CreatedAt BETWEEN @FromDate AND DATEADD(DAY, 1, @ToDate)
                ORDER BY CreatedAt DESC, ErrorID DESC", con))
                {
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Date);
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new MobileSyncErrorReportItem
                            {
                                ErrorID = Convert.ToInt32(dr["ErrorID"]),
                                SyncBatchID = dr["SyncBatchID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["SyncBatchID"]),
                                ImportID = dr["ImportID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["ImportID"]),
                                ErrorCode = dr["ErrorCode"] == DBNull.Value ? null : Convert.ToString(dr["ErrorCode"]),
                                ErrorMessage = dr["ErrorMessage"] == DBNull.Value ? null : Convert.ToString(dr["ErrorMessage"]),
                                CreatedAt = Convert.ToDateTime(dr["CreatedAt"])
                            });
                        }
                    }
                }

                return list;
            }

            public List<CollectorSubscriberReportItem> GetCollectorSubscribers(int? collectorId)
            {
                var list = new List<CollectorSubscriberReportItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    CS.CollectorID,
                    C.Name AS CollectorName,
                    S.SubscriberID,
                    S.Name AS SubscriberName,
                    S.PhoneNumber,
                    S.Address,
                    ISNULL(M.MeterNumber, S.MeterNumber) AS PrimaryMeterNumber
                FROM dbo.CollectorSubscribers CS
                INNER JOIN dbo.Collectors C ON CS.CollectorID = C.CollectorID
                INNER JOIN dbo.Subscribers S ON CS.SubscriberID = S.SubscriberID
                LEFT JOIN dbo.SubscriberMeters SM ON SM.SubscriberID = S.SubscriberID AND SM.IsPrimary = 1
                LEFT JOIN dbo.Meters M ON M.MeterID = SM.MeterID
                WHERE (@CollectorID IS NULL OR CS.CollectorID = @CollectorID)
                ORDER BY C.Name, S.Name", con))
                {
                    cmd.Parameters.AddWithValue("@CollectorID", collectorId.HasValue ? (object)collectorId.Value : DBNull.Value);
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new CollectorSubscriberReportItem
                            {
                                CollectorID = Convert.ToInt32(dr["CollectorID"]),
                                CollectorName = Convert.ToString(dr["CollectorName"]),
                                SubscriberID = Convert.ToInt32(dr["SubscriberID"]),
                                SubscriberName = Convert.ToString(dr["SubscriberName"]),
                                PhoneNumber = dr["PhoneNumber"] == DBNull.Value ? null : Convert.ToString(dr["PhoneNumber"]),
                                Address = dr["Address"] == DBNull.Value ? null : Convert.ToString(dr["Address"]),
                                PrimaryMeterNumber = dr["PrimaryMeterNumber"] == DBNull.Value ? null : Convert.ToString(dr["PrimaryMeterNumber"])
                            });
                        }
                    }
                }

                return list;
            }

            public List<PhonePushReportItem> GetPhonePushPreview(DateTime asOfDate, int collectorId, string collectorName, string deviceCode, string deviceName, bool onlyAssignedSubscribers, bool includeAllIfNoAssignments)
            {
                var mobileService = new MobileSyncToPhoneService();
                var summary = mobileService.ExportPreview(collectorId, collectorName, deviceCode, deviceName, asOfDate, onlyAssignedSubscribers, includeAllIfNoAssignments);

                return new List<PhonePushReportItem>
            {
                new PhonePushReportItem
                {
                    CollectorName = summary.CollectorName,
                    DeviceCode = summary.DeviceCode,
                    DeviceName = summary.DeviceName,
                    AsOfDate = DateTime.Parse(summary.AsOfDateText),
                    SubscribersCount = summary.SubscribersCount,
                    MetersCount = summary.MetersCount,
                    OpenInvoicesCount = summary.OpenInvoicesCount,
                    TotalOpenInvoicesAmount = summary.TotalOpenInvoicesAmount,
                    CreditsCount = summary.CreditsCount,
                    TotalCreditsAmount = summary.TotalCreditsAmount,
                    CreatedAt = DateTime.Now
                }
            };
            }

            public void LogReportOpen(string key, string details = null)
            {
                _audit.Log(
                    action: "OPEN_OPERATIONAL_REPORT",
                    tableName: "Reports",
                    recordId: null,
                    details: details ?? key,
                    entityName: key);
            }
        }
    }