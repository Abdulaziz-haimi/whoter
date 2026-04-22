using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace water3.MobileSync
{
    public sealed class LocalReceiptRepository
    {
        private readonly string _connectionString;

        public LocalReceiptRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task<long> CreateDraftAsync(LocalReceiptDraftDto draft, CancellationToken cancellationToken = default)
        {
            if (draft == null) throw new ArgumentNullException(nameof(draft));
            if (draft.Lines == null || draft.Lines.Count == 0)
                throw new InvalidOperationException("Draft must contain at least one line.");

            using (var con = new SqliteConnection(_connectionString))
            {
                await con.OpenAsync(cancellationToken);

                using (var tx = con.BeginTransaction())
                {
                    try
                    {
                        long localReceiptId;

                        using (var cmd = con.CreateCommand())
                        {
                            cmd.Transaction = tx;
                            cmd.CommandText = @"
INSERT INTO LocalReceiptDrafts
(
    LocalPaymentGuid, LocalReceiptNo, SubscriberID, CollectorID, PaymentDate,
    TotalReceived, PaymentMethod, Notes,
    SyncStatus, SyncBatchRef, ServerImportID, ServerStatus, RejectedReason,
    CreatedAt, UpdatedAt, SentAt
)
VALUES
(
    $LocalPaymentGuid, $LocalReceiptNo, $SubscriberID, $CollectorID, $PaymentDate,
    $TotalReceived, $PaymentMethod, $Notes,
    $SyncStatus, NULL, NULL, NULL, NULL,
    $CreatedAt, NULL, NULL
);
SELECT last_insert_rowid();";

                            cmd.Parameters.AddWithValue("$LocalPaymentGuid", draft.LocalPaymentGuid);
                            cmd.Parameters.AddWithValue("$LocalReceiptNo", draft.LocalReceiptNo);
                            cmd.Parameters.AddWithValue("$SubscriberID", draft.SubscriberID);
                            cmd.Parameters.AddWithValue("$CollectorID", draft.CollectorID);
                            cmd.Parameters.AddWithValue("$PaymentDate", draft.PaymentDate.ToString("yyyy-MM-dd"));
                            cmd.Parameters.AddWithValue("$TotalReceived", draft.TotalReceived);
                            cmd.Parameters.AddWithValue("$PaymentMethod", draft.PaymentMethod);
                            cmd.Parameters.AddWithValue("$Notes", (object)draft.Notes ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("$SyncStatus", draft.SyncStatus);
                            cmd.Parameters.AddWithValue("$CreatedAt", (draft.CreatedAt == default ? DateTime.UtcNow : draft.CreatedAt).ToString("O"));

                            object result = await cmd.ExecuteScalarAsync(cancellationToken);
                            localReceiptId = Convert.ToInt64(result);
                        }

                        foreach (var line in draft.Lines)
                        {
                            using (var cmd = con.CreateCommand())
                            {
                                cmd.Transaction = tx;
                                cmd.CommandText = @"
INSERT INTO LocalReceiptDraftLines
(
    LocalReceiptID, InvoiceID, AppliedAmount, ApplicationType, Notes
)
VALUES
(
    $LocalReceiptID, $InvoiceID, $AppliedAmount, $ApplicationType, $Notes
);";

                                cmd.Parameters.AddWithValue("$LocalReceiptID", localReceiptId);
                                cmd.Parameters.AddWithValue("$InvoiceID", (object)line.InvoiceID ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("$AppliedAmount", line.AppliedAmount);
                                cmd.Parameters.AddWithValue("$ApplicationType", line.ApplicationType);
                                cmd.Parameters.AddWithValue("$Notes", (object)line.Notes ?? DBNull.Value);

                                await cmd.ExecuteNonQueryAsync(cancellationToken);
                            }
                        }

                        tx.Commit();
                        return localReceiptId;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<List<LocalReceiptDraftDto>> GetPendingDraftsAsync(CancellationToken cancellationToken = default)
        {
            var result = new List<LocalReceiptDraftDto>();

            using (var con = new SqliteConnection(_connectionString))
            {
                await con.OpenAsync(cancellationToken);

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"
SELECT
    LocalReceiptID, LocalPaymentGuid, LocalReceiptNo, SubscriberID, CollectorID,
    PaymentDate, TotalReceived, PaymentMethod, Notes,
    SyncStatus, SyncBatchRef, ServerImportID, ServerStatus, RejectedReason,
    CreatedAt, UpdatedAt, SentAt
FROM LocalReceiptDrafts
WHERE SyncStatus = 'Pending'
ORDER BY PaymentDate, LocalReceiptID;";

                    using (var rd = await cmd.ExecuteReaderAsync(cancellationToken))
                    {
                        while (await rd.ReadAsync(cancellationToken))
                        {
                            var draft = new LocalReceiptDraftDto
                            {
                                LocalReceiptID = rd.GetInt64(0),
                                LocalPaymentGuid = rd.GetString(1),
                                LocalReceiptNo = rd.GetString(2),
                                SubscriberID = rd.GetInt32(3),
                                CollectorID = rd.GetInt32(4),
                                PaymentDate = DateTime.Parse(rd.GetString(5)),
                                TotalReceived = rd.GetDecimal(6),
                                PaymentMethod = rd.GetString(7),
                                Notes = rd.IsDBNull(8) ? null : rd.GetString(8),
                                SyncStatus = rd.GetString(9),
                                SyncBatchRef = rd.IsDBNull(10) ? null : rd.GetString(10),
                                ServerImportID = rd.IsDBNull(11) ? (int?)null : rd.GetInt32(11),
                                ServerStatus = rd.IsDBNull(12) ? null : rd.GetString(12),
                                RejectedReason = rd.IsDBNull(13) ? null : rd.GetString(13),
                                CreatedAt = DateTime.Parse(rd.GetString(14)),
                                UpdatedAt = rd.IsDBNull(15) ? (DateTime?)null : DateTime.Parse(rd.GetString(15)),
                                SentAt = rd.IsDBNull(16) ? (DateTime?)null : DateTime.Parse(rd.GetString(16))
                            };

                            draft.Lines = await GetDraftLinesAsync(con, draft.LocalReceiptID, cancellationToken);
                            result.Add(draft);
                        }
                    }
                }
            }

            return result;
        }

        public async Task MarkDraftAsSentAsync(
            long localReceiptId,
            string syncBatchRef,
            int? serverImportId,
            string serverStatus,
            CancellationToken cancellationToken = default)
        {
            using (var con = new SqliteConnection(_connectionString))
            {
                await con.OpenAsync(cancellationToken);

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"
UPDATE LocalReceiptDrafts
SET
    SyncStatus = 'Sent',
    SyncBatchRef = $SyncBatchRef,
    ServerImportID = $ServerImportID,
    ServerStatus = $ServerStatus,
    SentAt = $SentAt,
    UpdatedAt = $UpdatedAt
WHERE LocalReceiptID = $LocalReceiptID;";

                    cmd.Parameters.AddWithValue("$SyncBatchRef", syncBatchRef);
                    cmd.Parameters.AddWithValue("$ServerImportID", (object)serverImportId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$ServerStatus", serverStatus);
                    cmd.Parameters.AddWithValue("$SentAt", DateTime.UtcNow.ToString("O"));
                    cmd.Parameters.AddWithValue("$UpdatedAt", DateTime.UtcNow.ToString("O"));
                    cmd.Parameters.AddWithValue("$LocalReceiptID", localReceiptId);

                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }

        public async Task UpdateDraftServerDecisionAsync(
            long localReceiptId,
            string finalStatus,
            string rejectedReason,
            CancellationToken cancellationToken = default)
        {
            using (var con = new SqliteConnection(_connectionString))
            {
                await con.OpenAsync(cancellationToken);

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"
UPDATE LocalReceiptDrafts
SET
    SyncStatus = $SyncStatus,
    ServerStatus = $ServerStatus,
    RejectedReason = $RejectedReason,
    UpdatedAt = $UpdatedAt
WHERE LocalReceiptID = $LocalReceiptID;";

                    cmd.Parameters.AddWithValue("$SyncStatus", finalStatus);
                    cmd.Parameters.AddWithValue("$ServerStatus", finalStatus);
                    cmd.Parameters.AddWithValue("$RejectedReason", (object)rejectedReason ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$UpdatedAt", DateTime.UtcNow.ToString("O"));
                    cmd.Parameters.AddWithValue("$LocalReceiptID", localReceiptId);

                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }

        public async Task<UploadBatchRequestDto> BuildPendingUploadBatchAsync(
            int collectorId,
            string deviceCode,
            string deviceName,
            string deviceModel,
            string appVersion,
            CancellationToken cancellationToken = default)
        {
            var drafts = await GetPendingDraftsAsync(cancellationToken);

            var request = new UploadBatchRequestDto
            {
                CollectorID = collectorId,
                DeviceCode = deviceCode,
                DeviceName = deviceName,
                DeviceModel = deviceModel,
                AppVersion = appVersion,
                AutoCreateDevice = true
            };

            int rowNo = 1;
            foreach (var draft in drafts)
            {
                request.Receipts.Add(new UploadReceiptRowDto
                {
                    RowNo = rowNo,
                    LocalPaymentGuid = draft.LocalPaymentGuid,
                    LocalReceiptNo = draft.LocalReceiptNo,
                    SubscriberID = draft.SubscriberID,
                    PaymentDate = draft.PaymentDate,
                    TotalReceived = draft.TotalReceived,
                    PaymentMethod = draft.PaymentMethod,
                    Notes = draft.Notes,
                    LocalReceiptID = draft.LocalReceiptID
                });

                foreach (var line in draft.Lines)
                {
                    request.Lines.Add(new UploadReceiptLineRowDto
                    {
                        ReceiptRowNo = rowNo,
                        InvoiceID = line.InvoiceID,
                        AppliedAmount = line.AppliedAmount,
                        ApplicationType = line.ApplicationType,
                        Notes = line.Notes
                    });
                }

                rowNo++;
            }

            return request;
        }

        public async Task MarkDraftAsDuplicateAsync(
            long localReceiptId,
            int? serverImportId,
            CancellationToken cancellationToken = default)
        {
            using (var con = new SqliteConnection(_connectionString))
            {
                await con.OpenAsync(cancellationToken);

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"
UPDATE LocalReceiptDrafts
SET
    SyncStatus = 'Duplicate',
    ServerImportID = $ServerImportID,
    ServerStatus = 'Duplicate',
    UpdatedAt = $UpdatedAt
WHERE LocalReceiptID = $LocalReceiptID;";

                    cmd.Parameters.AddWithValue("$ServerImportID", (object)serverImportId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$UpdatedAt", DateTime.UtcNow.ToString("O"));
                    cmd.Parameters.AddWithValue("$LocalReceiptID", localReceiptId);

                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }

        public async Task<long?> GetLocalReceiptIdByGuidAsync(string localPaymentGuid, CancellationToken cancellationToken = default)
        {
            using (var con = new SqliteConnection(_connectionString))
            {
                await con.OpenAsync(cancellationToken);

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = @"
SELECT LocalReceiptID
FROM LocalReceiptDrafts
WHERE LocalPaymentGuid = $LocalPaymentGuid
LIMIT 1;";

                    cmd.Parameters.AddWithValue("$LocalPaymentGuid", localPaymentGuid);

                    object result = await cmd.ExecuteScalarAsync(cancellationToken);
                    if (result == null || result == DBNull.Value)
                        return null;

                    return Convert.ToInt64(result);
                }
            }
        }

        private static async Task<List<LocalReceiptDraftLineDto>> GetDraftLinesAsync(
            SqliteConnection con,
            long localReceiptId,
            CancellationToken cancellationToken)
        {
            var lines = new List<LocalReceiptDraftLineDto>();

            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = @"
SELECT
    LocalReceiptLineID, LocalReceiptID, InvoiceID, AppliedAmount, ApplicationType, Notes
FROM LocalReceiptDraftLines
WHERE LocalReceiptID = $LocalReceiptID
ORDER BY LocalReceiptLineID;";

                cmd.Parameters.AddWithValue("$LocalReceiptID", localReceiptId);

                using (var rd = await cmd.ExecuteReaderAsync(cancellationToken))
                {
                    while (await rd.ReadAsync(cancellationToken))
                    {
                        lines.Add(new LocalReceiptDraftLineDto
                        {
                            LocalReceiptLineID = rd.GetInt64(0),
                            LocalReceiptID = rd.GetInt64(1),
                            InvoiceID = rd.IsDBNull(2) ? (int?)null : rd.GetInt32(2),
                            AppliedAmount = rd.GetDecimal(3),
                            ApplicationType = rd.GetString(4),
                            Notes = rd.IsDBNull(5) ? null : rd.GetString(5)
                        });
                    }
                }
            }

            return lines;
        }
    }
}