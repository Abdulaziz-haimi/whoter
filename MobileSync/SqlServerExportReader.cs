using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
//using Microsoft.Data.SqlClient;

namespace water3.MobileSync
{
    public sealed class SqlServerExportReader
    {
        private readonly string _sqlServerConnectionString;

        public SqlServerExportReader(string sqlServerConnectionString)
        {
            _sqlServerConnectionString = sqlServerConnectionString ?? throw new ArgumentNullException(nameof(sqlServerConnectionString));
        }

        public async Task<SyncExportBundle> ExportReceivablesAsync(
            int collectorId,
            DateTime? asOfDate = null,
            bool onlyAssignedSubscribers = true,
            bool includeAllIfNoAssignments = true,
            CancellationToken cancellationToken = default)
        {
            var bundle = new SyncExportBundle();

            using (var con = new SqlConnection(_sqlServerConnectionString))
            {
                await con.OpenAsync(cancellationToken);

                using (var cmd = new SqlCommand("dbo.usp_MobileSync_ExportReceivables", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@CollectorID", collectorId);
                    cmd.Parameters.AddWithValue("@AsOfDate", asOfDate.HasValue ? (object)asOfDate.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@OnlyAssignedSubscribers", onlyAssignedSubscribers);
                    cmd.Parameters.AddWithValue("@IncludeAllIfNoAssignments", includeAllIfNoAssignments);

                    using (var rd = await cmd.ExecuteReaderAsync(cancellationToken))
                    {
                        if (await rd.ReadAsync(cancellationToken))
                        {
                            bundle.Summary = new SyncExportSummaryDto
                            {
                                CollectorID = GetInt32(rd, "CollectorID"),
                                AsOfDate = GetDateTime(rd, "AsOfDate"),
                                ExportedAt = GetDateTime(rd, "ExportedAt"),
                                SubscribersCount = GetInt32(rd, "SubscribersCount"),
                                OpenInvoicesCount = GetInt32(rd, "OpenInvoicesCount"),
                                OpenCreditsCount = GetInt32(rd, "OpenCreditsCount")
                            };
                        }

                        if (await rd.NextResultAsync(cancellationToken))
                        {
                            while (await rd.ReadAsync(cancellationToken))
                            {
                                bundle.Subscribers.Add(new LocalSubscriberDto
                                {
                                    SubscriberID = GetInt32(rd, "SubscriberID"),
                                    SubscriberName = GetString(rd, "SubscriberName"),
                                    PhoneNumber = GetNullableString(rd, "PhoneNumber"),
                                    Address = GetNullableString(rd, "Address"),
                                    AccountID = GetNullableInt32(rd, "AccountID"),
                                    TariffPlanID = GetNullableInt32(rd, "TariffPlanID"),
                                    IsActive = GetBool(rd, "IsActive"),
                                    PrimaryMeterID = GetNullableInt32(rd, "PrimaryMeterID"),
                                    PrimaryMeterNumber = GetNullableString(rd, "PrimaryMeterNumber"),
                                    PrimaryMeterLocation = GetNullableString(rd, "PrimaryMeterLocation"),
                                    CurrentDue = GetDecimal(rd, "CurrentDue"),
                                    CurrentCredit = GetDecimal(rd, "CurrentCredit"),
                                    CurrentBalance = GetDecimal(rd, "CurrentBalance"),
                                    LastInvoiceID = GetNullableInt32(rd, "LastInvoiceID"),
                                    LastInvoiceNumber = GetNullableString(rd, "LastInvoiceNumber"),
                                    LastInvoiceDate = GetNullableDateTime(rd, "LastInvoiceDate"),
                                    LastInvoiceTotal = GetNullableDecimal(rd, "LastInvoiceTotal"),
                                    LastInvoiceRemaining = GetNullableDecimal(rd, "LastInvoiceRemaining")
                                });
                            }
                        }

                        if (await rd.NextResultAsync(cancellationToken))
                        {
                            while (await rd.ReadAsync(cancellationToken))
                            {
                                bundle.Meters.Add(new LocalSubscriberMeterDto
                                {
                                    SubscriberMeterID = GetInt32(rd, "SubscriberMeterID"),
                                    SubscriberID = GetInt32(rd, "SubscriberID"),
                                    MeterID = GetInt32(rd, "MeterID"),
                                    MeterNumber = GetString(rd, "MeterNumber"),
                                    MeterType = GetNullableString(rd, "MeterType"),
                                    Location = GetNullableString(rd, "Location"),
                                    IsActive = GetBool(rd, "IsActive"),
                                    IsPrimary = GetBool(rd, "IsPrimary"),
                                    LinkedAt = GetNullableDateTime(rd, "LinkedAt")
                                });
                            }
                        }

                        if (await rd.NextResultAsync(cancellationToken))
                        {
                            while (await rd.ReadAsync(cancellationToken))
                            {
                                bundle.OpenInvoices.Add(new LocalOpenInvoiceDto
                                {
                                    InvoiceID = GetInt32(rd, "InvoiceID"),
                                    SubscriberID = GetInt32(rd, "SubscriberID"),
                                    MeterID = GetNullableInt32(rd, "MeterID"),
                                    InvoiceNumber = GetNullableString(rd, "InvoiceNumber"),
                                    InvoiceDate = GetDateTime(rd, "InvoiceDate"),
                                    Consumption = GetDecimal(rd, "Consumption"),
                                    UnitPrice = GetDecimal(rd, "UnitPrice"),
                                    ServiceFees = GetDecimal(rd, "ServiceFees"),
                                    Arrears = GetDecimal(rd, "Arrears"),
                                    TotalAmount = GetDecimal(rd, "TotalAmount"),
                                    GrandTotal = GetDecimal(rd, "GrandTotal"),
                                    PaidTotal = GetDecimal(rd, "PaidTotal"),
                                    Remaining = GetDecimal(rd, "Remaining"),
                                    Status = GetNullableString(rd, "Status"),
                                    Notes = GetNullableString(rd, "Notes")
                                });
                            }
                        }

                        if (await rd.NextResultAsync(cancellationToken))
                        {
                            while (await rd.ReadAsync(cancellationToken))
                            {
                                bundle.Credits.Add(new LocalSubscriberCreditDto
                                {
                                    CreditID = GetInt32(rd, "CreditID"),
                                    SubscriberID = GetInt32(rd, "SubscriberID"),
                                    PaymentID = GetNullableInt32(rd, "PaymentID"),
                                    ReceiptID = GetNullableInt32(rd, "ReceiptID"),
                                    MeterID = GetNullableInt32(rd, "MeterID"),
                                    CreditDate = GetDateTime(rd, "CreditDate"),
                                    AmountTotal = GetDecimal(rd, "AmountTotal"),
                                    AmountRemaining = GetDecimal(rd, "AmountRemaining"),
                                    Notes = GetNullableString(rd, "Notes")
                                });
                            }
                        }
                    }
                }
            }

            return bundle;
        }

        private static string GetString(SqlDataReader rd, string name)
        {
            return Convert.ToString(rd[name], CultureInfo.InvariantCulture) ?? string.Empty;
        }

        private static string GetNullableString(SqlDataReader rd, string name)
        {
            return rd[name] == DBNull.Value ? null : Convert.ToString(rd[name], CultureInfo.InvariantCulture);
        }

        private static int GetInt32(SqlDataReader rd, string name)
        {
            return Convert.ToInt32(rd[name], CultureInfo.InvariantCulture);
        }

        private static int? GetNullableInt32(SqlDataReader rd, string name)
        {
            return rd[name] == DBNull.Value ? (int?)null : Convert.ToInt32(rd[name], CultureInfo.InvariantCulture);
        }

        private static decimal GetDecimal(SqlDataReader rd, string name)
        {
            return Convert.ToDecimal(rd[name], CultureInfo.InvariantCulture);
        }

        private static decimal? GetNullableDecimal(SqlDataReader rd, string name)
        {
            return rd[name] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(rd[name], CultureInfo.InvariantCulture);
        }

        private static DateTime GetDateTime(SqlDataReader rd, string name)
        {
            return Convert.ToDateTime(rd[name], CultureInfo.InvariantCulture);
        }

        private static DateTime? GetNullableDateTime(SqlDataReader rd, string name)
        {
            return rd[name] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(rd[name], CultureInfo.InvariantCulture);
        }

        private static bool GetBool(SqlDataReader rd, string name)
        {
            return Convert.ToBoolean(rd[name], CultureInfo.InvariantCulture);
        }
    }
}