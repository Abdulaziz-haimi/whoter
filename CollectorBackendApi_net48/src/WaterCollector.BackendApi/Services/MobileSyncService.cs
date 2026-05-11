using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WaterCollector.BackendApi.Contracts.MobileSync;
using WaterCollector.BackendApi.Data;

namespace WaterCollector.BackendApi.Services
{
    public sealed class MobileSyncService : IMobileSyncService
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public MobileSyncService()
            : this(new SqlConnectionFactory())
        {
        }

        public MobileSyncService(ISqlConnectionFactory connectionFactory)
        {
            if (connectionFactory == null)
                throw new ArgumentNullException(nameof(connectionFactory));

            _connectionFactory = connectionFactory;
        }

        public async Task<ReceivablesExportResponse> ExportReceivablesAsync(int collectorId, DateTime? asOfDate)
        {
            if (collectorId <= 0)
                throw new UnauthorizedAccessException("رقم المحصل غير صحيح.");

            var response = new ReceivablesExportResponse();

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync().ConfigureAwait(false);

                using (var command = new SqlCommand("dbo.usp_MobileSync_ExportReceivables", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 0;
                    command.Parameters.AddWithValue("@CollectorID", collectorId);
                    command.Parameters.AddWithValue("@AsOfDate", (object)(asOfDate?.Date ?? (DateTime?)null) ?? DBNull.Value);
                    command.Parameters.AddWithValue("@OnlyAssignedSubscribers", true);
                    command.Parameters.AddWithValue("@IncludeAllIfNoAssignments", true);

                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            response.Summary = new SyncExportSummary
                            {
                                CollectorId = GetInt(reader, "CollectorID"),
                                AsOfDate = GetDateTime(reader, "AsOfDate"),
                                ExportedAt = GetDateTime(reader, "ExportedAt"),
                                SubscribersCount = GetInt(reader, "SubscribersCount"),
                                OpenInvoicesCount = GetInt(reader, "OpenInvoicesCount"),
                                OpenCreditsCount = GetInt(reader, "OpenCreditsCount")
                            };
                        }

                        if (await reader.NextResultAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                response.Subscribers.Add(new ReceivableSubscriber
                                {
                                    SubscriberId = GetInt(reader, "SubscriberID"),
                                    SubscriberName = GetString(reader, "SubscriberName"),
                                    PhoneNumber = GetString(reader, "PhoneNumber"),
                                    Address = GetString(reader, "Address"),
                                    AccountId = GetNullableInt(reader, "AccountID"),
                                    TariffPlanId = GetNullableInt(reader, "TariffPlanID"),
                                    IsActive = GetBool(reader, "IsActive"),
                                    PrimaryMeterId = GetNullableInt(reader, "PrimaryMeterID"),
                                    PrimaryMeterNumber = GetString(reader, "PrimaryMeterNumber"),
                                    PrimaryMeterLocation = GetString(reader, "PrimaryMeterLocation"),
                                    CurrentDue = GetDecimal(reader, "CurrentDue"),
                                    CurrentCredit = GetDecimal(reader, "CurrentCredit"),
                                    CurrentBalance = GetDecimal(reader, "CurrentBalance"),
                                    LastInvoiceId = GetNullableInt(reader, "LastInvoiceID"),
                                    LastInvoiceNumber = GetString(reader, "LastInvoiceNumber"),
                                    LastInvoiceDate = GetNullableDateTime(reader, "LastInvoiceDate"),
                                    LastInvoiceTotal = GetNullableDecimal(reader, "LastInvoiceTotal"),
                                    LastInvoiceRemaining = GetNullableDecimal(reader, "LastInvoiceRemaining")
                                });
                            }
                        }

                        if (await reader.NextResultAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                response.Meters.Add(new ReceivableMeter
                                {
                                    SubscriberMeterId = GetInt(reader, "SubscriberMeterID"),
                                    SubscriberId = GetInt(reader, "SubscriberID"),
                                    MeterId = GetInt(reader, "MeterID"),
                                    MeterNumber = GetString(reader, "MeterNumber"),
                                    MeterType = GetString(reader, "MeterType"),
                                    Location = GetString(reader, "Location"),
                                    IsActive = GetBool(reader, "IsActive"),
                                    IsPrimary = GetBool(reader, "IsPrimary"),
                                    LinkedAt = GetNullableDateTime(reader, "LinkedAt")
                                });
                            }
                        }

                        if (await reader.NextResultAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                response.OpenInvoices.Add(new ReceivableInvoice
                                {
                                    InvoiceId = GetInt(reader, "InvoiceID"),
                                    SubscriberId = GetInt(reader, "SubscriberID"),
                                    MeterId = GetNullableInt(reader, "MeterID"),
                                    InvoiceNumber = GetString(reader, "InvoiceNumber"),
                                    InvoiceDate = GetDateTime(reader, "InvoiceDate"),
                                    Consumption = GetDecimal(reader, "Consumption"),
                                    UnitPrice = GetDecimal(reader, "UnitPrice"),
                                    ServiceFees = GetDecimal(reader, "ServiceFees"),
                                    Arrears = GetDecimal(reader, "Arrears"),
                                    TotalAmount = GetDecimal(reader, "TotalAmount"),
                                    GrandTotal = GetDecimal(reader, "GrandTotal"),
                                    PaidTotal = GetDecimal(reader, "PaidTotal"),
                                    Remaining = GetDecimal(reader, "Remaining"),
                                    Status = GetString(reader, "Status"),
                                    Notes = GetString(reader, "Notes")
                                });
                            }
                        }

                        if (await reader.NextResultAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                response.OpenCredits.Add(new ReceivableCredit
                                {
                                    CreditId = GetInt(reader, "CreditID"),
                                    SubscriberId = GetInt(reader, "SubscriberID"),
                                    PaymentId = GetNullableInt(reader, "PaymentID"),
                                    ReceiptId = GetNullableInt(reader, "ReceiptID"),
                                    MeterId = GetNullableInt(reader, "MeterID"),
                                    CreditDate = GetDateTime(reader, "CreditDate"),
                                    AmountTotal = GetDecimal(reader, "AmountTotal"),
                                    AmountRemaining = GetDecimal(reader, "AmountRemaining"),
                                    Notes = GetString(reader, "Notes")
                                });
                            }
                        }
                    }
                }
            }

            return response;
        }

        public async Task<UploadBatchResponse> UploadBatchAsync(int collectorId, UploadBatchRequest request)
        {
            if (collectorId <= 0)
                throw new UnauthorizedAccessException("رقم المحصل غير صحيح.");

            if (request == null || request.Receipts == null || request.Receipts.Count == 0)
                throw new InvalidOperationException("لا توجد تحصيلات معلقة للرفع.");

            if (string.IsNullOrWhiteSpace(request.DeviceCode))
                throw new InvalidOperationException("DeviceCode غير موجود. سجل الدخول من التطبيق مرة أخرى.");

            var response = new UploadBatchResponse();

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync().ConfigureAwait(false);

                int deviceId = await GetOrCreateDeviceIdAsync(connection, collectorId, request).ConfigureAwait(false);

                using (var command = new SqlCommand("dbo.usp_MobileSync_SaveBatch", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 0;
                    command.Parameters.AddWithValue("@CollectorID", collectorId);

                    var syncBatchIdParam = command.Parameters.Add("@SyncBatchID", SqlDbType.Int);
                    syncBatchIdParam.Value = DBNull.Value;

                    command.Parameters.AddWithValue("@DeviceID", deviceId);
                    command.Parameters.AddWithValue("@DeviceCode", request.DeviceCode ?? string.Empty);
                    command.Parameters.AddWithValue("@DeviceName", (object)request.DeviceName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DeviceModel", (object)request.DeviceModel ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AppVersion", (object)request.AppVersion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AutoCreateDevice", request.AutoCreateDevice);

                    var receiptsParam = command.Parameters.AddWithValue("@Receipts", BuildReceiptsTvp(request));
                    receiptsParam.SqlDbType = SqlDbType.Structured;
                    receiptsParam.TypeName = "dbo.MobileReceiptImportType";

                    var linesParam = command.Parameters.AddWithValue("@Lines", BuildLinesTvp(request));
                    linesParam.SqlDbType = SqlDbType.Structured;
                    linesParam.TypeName = "dbo.MobileReceiptImportLineType";

                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            response.SyncBatchId = GetInt(reader, "SyncBatchID");
                            response.DeviceId = GetInt(reader, "DeviceID");
                            response.TotalRows = GetInt(reader, "TotalRows");
                            response.InsertedCount = GetInt(reader, "InsertedCount");
                            response.DuplicateCount = GetInt(reader, "DuplicateCount");
                            response.BatchStatus = GetString(reader, "BatchStatus");
                        }

                        if (await reader.NextResultAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                response.RowResults.Add(new UploadRowResult
                                {
                                    RowNo = GetInt(reader, "RowNo"),
                                    LocalPaymentGuid = GetString(reader, "LocalPaymentGuid"),
                                    ImportId = GetNullableInt(reader, "ImportID"),
                                    SaveStatus = GetString(reader, "SaveStatus")
                                });
                            }
                        }
                    }
                }
            }

            return response;
        }

        public async Task<ImportDecisionsResponse> GetImportDecisionsAsync(int collectorId, string deviceCode, DateTime? changedAfter)
        {
            if (collectorId <= 0)
                throw new UnauthorizedAccessException("رقم المحصل غير صحيح.");

            if (string.IsNullOrWhiteSpace(deviceCode))
                throw new InvalidOperationException("DeviceCode مطلوب.");

            var response = new ImportDecisionsResponse();

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync().ConfigureAwait(false);

                using (var command = new SqlCommand("dbo.usp_MobileSync_GetImportDecisions", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandTimeout = 0;
                    command.Parameters.AddWithValue("@CollectorID", collectorId);
                    command.Parameters.AddWithValue("@DeviceCode", deviceCode.Trim());
                    command.Parameters.AddWithValue("@ChangedAfter", (object)changedAfter ?? DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var item = new ImportDecisionItem
                            {
                                ImportId = GetInt(reader, "ImportID"),
                                SyncBatchId = GetInt(reader, "SyncBatchID"),
                                LocalPaymentGuid = GetString(reader, "LocalPaymentGuid"),
                                LocalReceiptNo = GetString(reader, "LocalReceiptNo"),
                                ImportStatus = GetString(reader, "ImportStatus"),
                                ApprovedReceiptId = GetNullableInt(reader, "ApprovedReceiptID"),
                                ReceiptNumber = GetString(reader, "ReceiptNumber"),
                                ApprovedAt = GetNullableDateTime(reader, "ApprovedAt"),
                                ApprovedByUserId = GetNullableInt(reader, "ApprovedByUserID"),
                                ApprovedByUserName = GetString(reader, "ApprovedByUserName"),
                                RejectedReason = GetString(reader, "RejectedReason"),
                                CreatedAt = GetDateTime(reader, "CreatedAt"),
                                ChangedAt = GetDateTime(reader, "ChangedAt")
                            };

                            response.Decisions.Add(item);

                            if (!response.MaxChangedAt.HasValue || item.ChangedAt > response.MaxChangedAt.Value)
                                response.MaxChangedAt = item.ChangedAt;
                        }
                    }
                }
            }

            return response;
        }

        private static async Task<int> GetOrCreateDeviceIdAsync(SqlConnection connection, int collectorId, UploadBatchRequest request)
        {
            string deviceCode = request.DeviceCode.Trim();

            using (var selectCommand = new SqlCommand(@"
SELECT TOP 1 DeviceID
FROM dbo.CollectorDevices
WHERE CollectorID = @CollectorID AND DeviceCode = @DeviceCode;", connection))
            {
                selectCommand.Parameters.AddWithValue("@CollectorID", collectorId);
                selectCommand.Parameters.AddWithValue("@DeviceCode", deviceCode);

                object result = await selectCommand.ExecuteScalarAsync().ConfigureAwait(false);
                if (result != null && result != DBNull.Value)
                {
                    int deviceId = Convert.ToInt32(result);

                    using (var updateCommand = new SqlCommand(@"
UPDATE dbo.CollectorDevices
SET LastSyncAt = GETDATE(),
    DeviceName = COALESCE(@DeviceName, DeviceName),
    DeviceModel = COALESCE(@DeviceModel, DeviceModel),
    AppVersion = COALESCE(@AppVersion, AppVersion),
    IsActive = 1
WHERE DeviceID = @DeviceID;", connection))
                    {
                        updateCommand.Parameters.AddWithValue("@DeviceID", deviceId);
                        updateCommand.Parameters.AddWithValue("@DeviceName", string.IsNullOrWhiteSpace(request.DeviceName) ? (object)DBNull.Value : request.DeviceName);
                        updateCommand.Parameters.AddWithValue("@DeviceModel", string.IsNullOrWhiteSpace(request.DeviceModel) ? (object)DBNull.Value : request.DeviceModel);
                        updateCommand.Parameters.AddWithValue("@AppVersion", string.IsNullOrWhiteSpace(request.AppVersion) ? (object)DBNull.Value : request.AppVersion);
                        await updateCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }

                    return deviceId;
                }
            }

            using (var insertCommand = new SqlCommand(@"
INSERT INTO dbo.CollectorDevices
(
    CollectorID, DeviceCode, DeviceName, IsApproved, LastSyncAt, CreatedAt,
    DeviceModel, AppVersion, IsActive, PhoneNumber
)
OUTPUT INSERTED.DeviceID
VALUES
(
    @CollectorID, @DeviceCode, @DeviceName, 1, GETDATE(), GETDATE(),
    @DeviceModel, @AppVersion, 1, NULL
);", connection))
            {
                insertCommand.Parameters.AddWithValue("@CollectorID", collectorId);
                insertCommand.Parameters.AddWithValue("@DeviceCode", deviceCode);
                insertCommand.Parameters.AddWithValue("@DeviceName", string.IsNullOrWhiteSpace(request.DeviceName) ? (object)DBNull.Value : request.DeviceName);
                insertCommand.Parameters.AddWithValue("@DeviceModel", string.IsNullOrWhiteSpace(request.DeviceModel) ? (object)DBNull.Value : request.DeviceModel);
                insertCommand.Parameters.AddWithValue("@AppVersion", string.IsNullOrWhiteSpace(request.AppVersion) ? (object)DBNull.Value : request.AppVersion);

                object newDeviceId = await insertCommand.ExecuteScalarAsync().ConfigureAwait(false);
                if (newDeviceId != null && newDeviceId != DBNull.Value)
                    return Convert.ToInt32(newDeviceId);
            }

            throw new InvalidOperationException("فشل إنشاء جهاز المحصل في قاعدة البيانات.");
        }

        private static DataTable BuildReceiptsTvp(UploadBatchRequest request)
        {
            var table = new DataTable();
            table.Columns.Add("RowNo", typeof(int));
            table.Columns.Add("LocalPaymentGuid", typeof(string));
            table.Columns.Add("LocalReceiptNo", typeof(string));
            table.Columns.Add("SubscriberID", typeof(int));
            table.Columns.Add("PaymentDate", typeof(DateTime));
            table.Columns.Add("TotalReceived", typeof(decimal));
            table.Columns.Add("PaymentMethod", typeof(string));
            table.Columns.Add("Notes", typeof(string));

            foreach (var item in request.Receipts)
            {
                table.Rows.Add(
                    item.RowNo,
                    item.LocalPaymentGuid ?? Guid.NewGuid().ToString("N"),
                    item.LocalReceiptNo ?? string.Empty,
                    item.SubscriberId,
                    item.PaymentDate,
                    item.TotalReceived,
                    string.IsNullOrWhiteSpace(item.PaymentMethod) ? "Cash" : item.PaymentMethod,
                    (object)item.Notes ?? DBNull.Value
                );
            }

            return table;
        }

        private static DataTable BuildLinesTvp(UploadBatchRequest request)
        {
            var table = new DataTable();
            table.Columns.Add("ReceiptRowNo", typeof(int));
            table.Columns.Add("InvoiceID", typeof(int));
            table.Columns.Add("AppliedAmount", typeof(decimal));
            table.Columns.Add("ApplicationType", typeof(string));
            table.Columns.Add("Notes", typeof(string));

            if (request.Lines == null)
                return table;

            foreach (var item in request.Lines)
            {
                DataRow row = table.NewRow();
                row["ReceiptRowNo"] = item.ReceiptRowNo;
                row["InvoiceID"] = item.InvoiceId.HasValue ? (object)item.InvoiceId.Value : DBNull.Value;
                row["AppliedAmount"] = item.AppliedAmount;
                row["ApplicationType"] = string.IsNullOrWhiteSpace(item.ApplicationType) ? "InvoicePayment" : item.ApplicationType;
                row["Notes"] = (object)item.Notes ?? DBNull.Value;
                table.Rows.Add(row);
            }

            return table;
        }

        private static bool HasColumn(SqlDataReader reader, string name)
        {
            for (int i = 0; i < reader.FieldCount; i++)
                if (string.Equals(reader.GetName(i), name, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private static object GetValue(SqlDataReader reader, string name)
        {
            if (!HasColumn(reader, name)) return DBNull.Value;
            object value = reader[name];
            return value == DBNull.Value ? DBNull.Value : value;
        }

        private static string GetString(SqlDataReader reader, string name) => GetValue(reader, name) == DBNull.Value ? null : Convert.ToString(GetValue(reader, name));
        private static int GetInt(SqlDataReader reader, string name) => GetValue(reader, name) == DBNull.Value ? 0 : Convert.ToInt32(GetValue(reader, name));
        private static int? GetNullableInt(SqlDataReader reader, string name) => GetValue(reader, name) == DBNull.Value ? (int?)null : Convert.ToInt32(GetValue(reader, name));
        private static decimal GetDecimal(SqlDataReader reader, string name) => GetValue(reader, name) == DBNull.Value ? 0m : Convert.ToDecimal(GetValue(reader, name));
        private static decimal? GetNullableDecimal(SqlDataReader reader, string name) => GetValue(reader, name) == DBNull.Value ? (decimal?)null : Convert.ToDecimal(GetValue(reader, name));
        private static bool GetBool(SqlDataReader reader, string name) => GetValue(reader, name) != DBNull.Value && Convert.ToBoolean(GetValue(reader, name));
        private static DateTime GetDateTime(SqlDataReader reader, string name) => GetValue(reader, name) == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(GetValue(reader, name));
        private static DateTime? GetNullableDateTime(SqlDataReader reader, string name) => GetValue(reader, name) == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(GetValue(reader, name));
    }
}
