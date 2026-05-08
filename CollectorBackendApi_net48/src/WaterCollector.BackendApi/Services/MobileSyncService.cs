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

        public MobileSyncService(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<ReceivablesExportResponse> ExportReceivablesAsync(
            int collectorId,
            DateTime? asOfDate
        )
        {
            var response = new ReceivablesExportResponse();

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync().ConfigureAwait(false);

                using (var command = new SqlCommand("dbo.usp_MobileSync_ExportReceivables", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CollectorID", collectorId);
                    command.Parameters.AddWithValue(
                        "@AsOfDate",
                        (object)(asOfDate?.Date ?? (DateTime?)null) ?? DBNull.Value
                    );
                    command.Parameters.AddWithValue("@OnlyAssignedSubscribers", true);
                    command.Parameters.AddWithValue("@IncludeAllIfNoAssignments", true);

                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            response.Summary = new SyncExportSummary
                            {
                                CollectorId = Convert.ToInt32(reader["CollectorID"]),
                                AsOfDate = Convert.ToDateTime(reader["AsOfDate"]),
                                ExportedAt = Convert.ToDateTime(reader["ExportedAt"]),
                                SubscribersCount = Convert.ToInt32(reader["SubscribersCount"]),
                                OpenInvoicesCount = Convert.ToInt32(reader["OpenInvoicesCount"]),
                                OpenCreditsCount = Convert.ToInt32(reader["OpenCreditsCount"])
                            };
                        }

                        if (await reader.NextResultAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                response.Subscribers.Add(new ReceivableSubscriber
                                {
                                    SubscriberId = Convert.ToInt32(reader["SubscriberID"]),
                                    SubscriberName = Convert.ToString(reader["SubscriberName"]),
                                    PhoneNumber = reader["PhoneNumber"] == DBNull.Value ? null : Convert.ToString(reader["PhoneNumber"]),
                                    Address = reader["Address"] == DBNull.Value ? null : Convert.ToString(reader["Address"]),
                                    AccountId = reader["AccountID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["AccountID"]),
                                    TariffPlanId = reader["TariffPlanID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["TariffPlanID"]),
                                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                                    PrimaryMeterId = reader["PrimaryMeterID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["PrimaryMeterID"]),
                                    PrimaryMeterNumber = reader["PrimaryMeterNumber"] == DBNull.Value ? null : Convert.ToString(reader["PrimaryMeterNumber"]),
                                    PrimaryMeterLocation = reader["PrimaryMeterLocation"] == DBNull.Value ? null : Convert.ToString(reader["PrimaryMeterLocation"]),
                                    CurrentDue = Convert.ToDecimal(reader["CurrentDue"]),
                                    CurrentCredit = Convert.ToDecimal(reader["CurrentCredit"]),
                                    CurrentBalance = Convert.ToDecimal(reader["CurrentBalance"]),
                                    LastInvoiceId = reader["LastInvoiceID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["LastInvoiceID"]),
                                    LastInvoiceNumber = reader["LastInvoiceNumber"] == DBNull.Value ? null : Convert.ToString(reader["LastInvoiceNumber"]),
                                    LastInvoiceDate = reader["LastInvoiceDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["LastInvoiceDate"]),
                                    LastInvoiceTotal = reader["LastInvoiceTotal"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["LastInvoiceTotal"]),
                                    LastInvoiceRemaining = reader["LastInvoiceRemaining"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(reader["LastInvoiceRemaining"])
                                });
                            }
                        }

                        if (await reader.NextResultAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                response.Meters.Add(new ReceivableMeter
                                {
                                    SubscriberMeterId = Convert.ToInt32(reader["SubscriberMeterID"]),
                                    SubscriberId = Convert.ToInt32(reader["SubscriberID"]),
                                    MeterId = Convert.ToInt32(reader["MeterID"]),
                                    MeterNumber = Convert.ToString(reader["MeterNumber"]),
                                    MeterType = reader["MeterType"] == DBNull.Value ? null : Convert.ToString(reader["MeterType"]),
                                    Location = reader["Location"] == DBNull.Value ? null : Convert.ToString(reader["Location"]),
                                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                                    IsPrimary = Convert.ToBoolean(reader["IsPrimary"]),
                                    LinkedAt = reader["LinkedAt"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["LinkedAt"])
                                });
                            }
                        }

                        if (await reader.NextResultAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                response.OpenInvoices.Add(new ReceivableInvoice
                                {
                                    InvoiceId = Convert.ToInt32(reader["InvoiceID"]),
                                    SubscriberId = Convert.ToInt32(reader["SubscriberID"]),
                                    MeterId = reader["MeterID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["MeterID"]),
                                    InvoiceNumber = reader["InvoiceNumber"] == DBNull.Value ? null : Convert.ToString(reader["InvoiceNumber"]),
                                    InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]),
                                    Consumption = Convert.ToDecimal(reader["Consumption"]),
                                    UnitPrice = Convert.ToDecimal(reader["UnitPrice"]),
                                    ServiceFees = Convert.ToDecimal(reader["ServiceFees"]),
                                    Arrears = Convert.ToDecimal(reader["Arrears"]),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                                    GrandTotal = Convert.ToDecimal(reader["GrandTotal"]),
                                    PaidTotal = Convert.ToDecimal(reader["PaidTotal"]),
                                    Remaining = Convert.ToDecimal(reader["Remaining"]),
                                    Status = reader["Status"] == DBNull.Value ? null : Convert.ToString(reader["Status"]),
                                    Notes = reader["Notes"] == DBNull.Value ? null : Convert.ToString(reader["Notes"])
                                });
                            }
                        }

                        if (await reader.NextResultAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                response.OpenCredits.Add(new ReceivableCredit
                                {
                                    CreditId = Convert.ToInt32(reader["CreditID"]),
                                    SubscriberId = Convert.ToInt32(reader["SubscriberID"]),
                                    PaymentId = reader["PaymentID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["PaymentID"]),
                                    ReceiptId = reader["ReceiptID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["ReceiptID"]),
                                    MeterId = reader["MeterID"] == DBNull.Value ? (int?)null : Convert.ToInt32(reader["MeterID"]),
                                    CreditDate = Convert.ToDateTime(reader["CreditDate"]),
                                    AmountTotal = Convert.ToDecimal(reader["AmountTotal"]),
                                    AmountRemaining = Convert.ToDecimal(reader["AmountRemaining"]),
                                    Notes = reader["Notes"] == DBNull.Value ? null : Convert.ToString(reader["Notes"])
                                });
                            }
                        }
                    }
                }
            }

            return response;
        }

        public async Task<UploadBatchResponse> UploadBatchAsync(
            int collectorId,
            UploadBatchRequest request
        )
        {
            if (request == null || request.Receipts == null || request.Receipts.Count == 0)
            {
                throw new InvalidOperationException("لا توجد تحصيلات معلقة للرفع.");
            }

            if (string.IsNullOrWhiteSpace(request.DeviceCode))
            {
                throw new InvalidOperationException("DeviceCode غير موجود. سجل الدخول من التطبيق مرة أخرى.");
            }

            var response = new UploadBatchResponse();

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync().ConfigureAwait(false);
                var deviceId = await GetOrCreateDeviceIdAsync(
                    connection,
                    collectorId,
                    request
                ).ConfigureAwait(false);

                using (var command = new SqlCommand("dbo.usp_MobileSync_SaveBatch", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CollectorID", collectorId);

                    var syncBatchIdParam = command.Parameters.Add("@SyncBatchID", SqlDbType.Int);
                    syncBatchIdParam.Value = DBNull.Value;

                    command.Parameters.AddWithValue("@DeviceID", deviceId);

                    command.Parameters.AddWithValue("@DeviceCode", request.DeviceCode ?? string.Empty);
                    command.Parameters.AddWithValue("@DeviceName", (object)request.DeviceName ?? DBNull.Value);
                    command.Parameters.AddWithValue("@DeviceModel", (object)request.DeviceModel ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AppVersion", (object)request.AppVersion ?? DBNull.Value);
                    command.Parameters.AddWithValue("@AutoCreateDevice", request.AutoCreateDevice);

                    var receiptsParam = command.Parameters.AddWithValue(
                        "@Receipts",
                        BuildReceiptsTvp(request)
                    );
                    receiptsParam.SqlDbType = SqlDbType.Structured;
                    receiptsParam.TypeName = "dbo.MobileReceiptImportType";

                    var linesParam = command.Parameters.AddWithValue(
                        "@Lines",
                        BuildLinesTvp(request)
                    );
                    linesParam.SqlDbType = SqlDbType.Structured;
                    linesParam.TypeName = "dbo.MobileReceiptImportLineType";

                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            response.SyncBatchId = Convert.ToInt32(reader["SyncBatchID"]);
                            response.DeviceId = Convert.ToInt32(reader["DeviceID"]);
                            response.TotalRows = Convert.ToInt32(reader["TotalRows"]);
                            response.InsertedCount = Convert.ToInt32(reader["InsertedCount"]);
                            response.DuplicateCount = Convert.ToInt32(reader["DuplicateCount"]);
                            response.BatchStatus = Convert.ToString(reader["BatchStatus"]);
                        }

                        if (await reader.NextResultAsync().ConfigureAwait(false))
                        {
                            while (await reader.ReadAsync().ConfigureAwait(false))
                            {
                                response.RowResults.Add(new UploadRowResult
                                {
                                    RowNo = Convert.ToInt32(reader["RowNo"]),
                                    LocalPaymentGuid = Convert.ToString(reader["LocalPaymentGuid"]),
                                    ImportId = reader["ImportID"] == DBNull.Value
                                        ? (int?)null
                                        : Convert.ToInt32(reader["ImportID"]),
                                    SaveStatus = Convert.ToString(reader["SaveStatus"])
                                });
                            }
                        }
                    }
                }
            }

            return response;
        }

        public async Task<ImportDecisionsResponse> GetImportDecisionsAsync(
            int collectorId,
            string deviceCode,
            DateTime? changedAfter
        )
        {
            var response = new ImportDecisionsResponse();

            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync().ConfigureAwait(false);

                using (var command = new SqlCommand("dbo.usp_MobileSync_GetImportDecisions", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@CollectorID", collectorId);
                    command.Parameters.AddWithValue("@DeviceCode", deviceCode ?? string.Empty);
                    command.Parameters.AddWithValue("@ChangedAfter", (object)changedAfter ?? DBNull.Value);

                    using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await reader.ReadAsync().ConfigureAwait(false))
                        {
                            var item = new ImportDecisionItem
                            {
                                ImportId = Convert.ToInt32(reader["ImportID"]),
                                SyncBatchId = Convert.ToInt32(reader["SyncBatchID"]),
                                LocalPaymentGuid = Convert.ToString(reader["LocalPaymentGuid"]),
                                LocalReceiptNo = Convert.ToString(reader["LocalReceiptNo"]),
                                ImportStatus = Convert.ToString(reader["ImportStatus"]),
                                ApprovedReceiptId = reader["ApprovedReceiptID"] == DBNull.Value
                                    ? (int?)null
                                    : Convert.ToInt32(reader["ApprovedReceiptID"]),
                                ReceiptNumber = reader["ReceiptNumber"] == DBNull.Value
                                    ? null
                                    : Convert.ToString(reader["ReceiptNumber"]),
                                ApprovedAt = reader["ApprovedAt"] == DBNull.Value
                                    ? (DateTime?)null
                                    : Convert.ToDateTime(reader["ApprovedAt"]),
                                ApprovedByUserId = reader["ApprovedByUserID"] == DBNull.Value
                                    ? (int?)null
                                    : Convert.ToInt32(reader["ApprovedByUserID"]),
                                ApprovedByUserName = reader["ApprovedByUserName"] == DBNull.Value
                                    ? null
                                    : Convert.ToString(reader["ApprovedByUserName"]),
                                RejectedReason = reader["RejectedReason"] == DBNull.Value
                                    ? null
                                    : Convert.ToString(reader["RejectedReason"]),
                                CreatedAt = Convert.ToDateTime(reader["CreatedAt"]),
                                ChangedAt = Convert.ToDateTime(reader["ChangedAt"])
                            };

                            response.Decisions.Add(item);

                            if (!response.MaxChangedAt.HasValue || item.ChangedAt > response.MaxChangedAt.Value)
                            {
                                response.MaxChangedAt = item.ChangedAt;
                            }
                        }
                    }
                }
            }

            return response;
        }

        private static async Task<int> GetOrCreateDeviceIdAsync(
     SqlConnection connection,
     int collectorId,
     UploadBatchRequest request
 )
        {
            if (string.IsNullOrWhiteSpace(request.DeviceCode))
            {
                throw new InvalidOperationException("DeviceCode غير موجود. سجل الدخول من التطبيق مرة أخرى.");
            }

            var deviceCode = request.DeviceCode.Trim();

            // 1) البحث عن الجهاز لهذا المحصل
            using (var selectCommand = new SqlCommand(@"
        SELECT TOP 1 DeviceID
        FROM dbo.CollectorDevices
        WHERE CollectorID = @CollectorID
          AND DeviceCode = @DeviceCode
    ", connection))
            {
                selectCommand.CommandType = CommandType.Text;
                selectCommand.Parameters.AddWithValue("@CollectorID", collectorId);
                selectCommand.Parameters.AddWithValue("@DeviceCode", deviceCode);

                var result = await selectCommand.ExecuteScalarAsync().ConfigureAwait(false);

                if (result != null && result != DBNull.Value)
                {
                    var deviceId = Convert.ToInt32(result);

                    using (var updateCommand = new SqlCommand(@"
                UPDATE dbo.CollectorDevices
                SET LastSyncAt = GETDATE(),
                    DeviceName = COALESCE(@DeviceName, DeviceName),
                    DeviceModel = COALESCE(@DeviceModel, DeviceModel),
                    AppVersion = COALESCE(@AppVersion, AppVersion),
                    IsActive = 1
                WHERE DeviceID = @DeviceID
            ", connection))
                    {
                        updateCommand.CommandType = CommandType.Text;
                        updateCommand.Parameters.AddWithValue("@DeviceID", deviceId);
                        updateCommand.Parameters.AddWithValue("@DeviceName", string.IsNullOrWhiteSpace(request.DeviceName) ? (object)DBNull.Value : request.DeviceName);
                        updateCommand.Parameters.AddWithValue("@DeviceModel", string.IsNullOrWhiteSpace(request.DeviceModel) ? (object)DBNull.Value : request.DeviceModel);
                        updateCommand.Parameters.AddWithValue("@AppVersion", string.IsNullOrWhiteSpace(request.AppVersion) ? (object)DBNull.Value : request.AppVersion);

                        await updateCommand.ExecuteNonQueryAsync().ConfigureAwait(false);
                    }

                    return deviceId;
                }
            }

            // 2) إذا لم يوجد الجهاز، ننشئه ونرجع DeviceID مباشرة
            using (var insertCommand = new SqlCommand(@"
        INSERT INTO dbo.CollectorDevices
        (
            CollectorID,
            DeviceCode,
            DeviceName,
            IsApproved,
            LastSyncAt,
            CreatedAt,
            DeviceModel,
            AppVersion,
            IsActive,
            PhoneNumber
        )
        OUTPUT INSERTED.DeviceID
        VALUES
        (
            @CollectorID,
            @DeviceCode,
            @DeviceName,
            1,
            GETDATE(),
            GETDATE(),
            @DeviceModel,
            @AppVersion,
            1,
            NULL
        );
    ", connection))
            {
                insertCommand.CommandType = CommandType.Text;
                insertCommand.Parameters.AddWithValue("@CollectorID", collectorId);
                insertCommand.Parameters.AddWithValue("@DeviceCode", deviceCode);
                insertCommand.Parameters.AddWithValue("@DeviceName", string.IsNullOrWhiteSpace(request.DeviceName) ? (object)DBNull.Value : request.DeviceName);
                insertCommand.Parameters.AddWithValue("@DeviceModel", string.IsNullOrWhiteSpace(request.DeviceModel) ? (object)DBNull.Value : request.DeviceModel);
                insertCommand.Parameters.AddWithValue("@AppVersion", string.IsNullOrWhiteSpace(request.AppVersion) ? (object)DBNull.Value : request.AppVersion);

                var newDeviceId = await insertCommand.ExecuteScalarAsync().ConfigureAwait(false);

                if (newDeviceId != null && newDeviceId != DBNull.Value)
                {
                    return Convert.ToInt32(newDeviceId);
                }
            }

            // 3) محاولة أخيرة: إعادة البحث بعد الإدخال
            using (var reselectCommand = new SqlCommand(@"
        SELECT TOP 1 DeviceID
        FROM dbo.CollectorDevices
        WHERE CollectorID = @CollectorID
          AND DeviceCode = @DeviceCode
        ORDER BY DeviceID DESC
    ", connection))
            {
                reselectCommand.CommandType = CommandType.Text;
                reselectCommand.Parameters.AddWithValue("@CollectorID", collectorId);
                reselectCommand.Parameters.AddWithValue("@DeviceCode", deviceCode);

                var result = await reselectCommand.ExecuteScalarAsync().ConfigureAwait(false);

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }
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
                    item.LocalPaymentGuid,
                    item.LocalReceiptNo,
                    item.SubscriberId,
                    item.PaymentDate,
                    item.TotalReceived,
                    item.PaymentMethod,
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
            {
                return table;
            }

            foreach (var item in request.Lines)
            {
                var row = table.NewRow();

                row["ReceiptRowNo"] = item.ReceiptRowNo;
                row["InvoiceID"] = item.InvoiceId.HasValue
                    ? (object)item.InvoiceId.Value
                    : DBNull.Value;
                row["AppliedAmount"] = item.AppliedAmount;
                row["ApplicationType"] = item.ApplicationType;
                row["Notes"] = (object)item.Notes ?? DBNull.Value;

                table.Rows.Add(row);
            }

            return table;
        }
    }
}