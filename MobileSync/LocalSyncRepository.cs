using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace water3.MobileSync
{
    public sealed class LocalSyncRepository
    {
        private readonly string _connectionString;

        public LocalSyncRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public async Task ReplaceExportDataAsync(
            SyncExportBundle bundle,
            string deviceCode,
            CancellationToken cancellationToken = default)
        {
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));

            using (var con = new SqliteConnection(_connectionString))
            {
                await con.OpenAsync(cancellationToken);

                using (var tx = con.BeginTransaction())
                {
                    try
                    {
                        await ClearReferenceTablesAsync(con, tx, cancellationToken);
                        await InsertSubscribersAsync(con, tx, bundle.Subscribers, cancellationToken);
                        await InsertMetersAsync(con, tx, bundle.Meters, cancellationToken);
                        await InsertInvoicesAsync(con, tx, bundle.OpenInvoices, cancellationToken);
                        await InsertCreditsAsync(con, tx, bundle.Credits, cancellationToken);
                        await UpsertSyncInfoAsync(con, tx, "CollectorID", bundle.Summary.CollectorID.ToString(CultureInfo.InvariantCulture), cancellationToken);
                        await UpsertSyncInfoAsync(con, tx, "DeviceCode", deviceCode, cancellationToken);
                        await UpsertSyncInfoAsync(con, tx, "LastExportedAt", bundle.Summary.ExportedAt.ToString("O"), cancellationToken);
                        await UpsertSyncInfoAsync(con, tx, "AsOfDate", bundle.Summary.AsOfDate.ToString("yyyy-MM-dd"), cancellationToken);

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task<string> GetSyncInfoAsync(string key, CancellationToken cancellationToken = default)
        {
            using (var con = new SqliteConnection(_connectionString))
            {
                await con.OpenAsync(cancellationToken);

                using (var cmd = con.CreateCommand())
                {
                    cmd.CommandText = "SELECT SyncValue FROM LocalSyncInfo WHERE SyncKey = $key LIMIT 1;";
                    cmd.Parameters.AddWithValue("$key", key);

                    object result = await cmd.ExecuteScalarAsync(cancellationToken);
                    return result == null || result == DBNull.Value
                        ? null
                        : Convert.ToString(result, CultureInfo.InvariantCulture);
                }
            }
        }

        private static async Task ClearReferenceTablesAsync(SqliteConnection con, SqliteTransaction tx, CancellationToken cancellationToken)
        {
            string[] deletes =
            {
                "DELETE FROM LocalSubscriberCredits;",
                "DELETE FROM LocalOpenInvoices;",
                "DELETE FROM LocalSubscriberMeters;",
                "DELETE FROM LocalSubscribers;"
            };

            foreach (string sql in deletes)
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = sql;
                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }

        private static async Task InsertSubscribersAsync(
            SqliteConnection con,
            SqliteTransaction tx,
            IEnumerable<LocalSubscriberDto> items,
            CancellationToken cancellationToken)
        {
            foreach (var x in items)
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"
INSERT INTO LocalSubscribers
(
    SubscriberID, SubscriberName, PhoneNumber, Address, AccountID, TariffPlanID, IsActive,
    PrimaryMeterID, PrimaryMeterNumber, PrimaryMeterLocation,
    CurrentDue, CurrentCredit, CurrentBalance,
    LastInvoiceID, LastInvoiceNumber, LastInvoiceDate, LastInvoiceTotal, LastInvoiceRemaining,
    LastSyncedAt
)
VALUES
(
    $SubscriberID, $SubscriberName, $PhoneNumber, $Address, $AccountID, $TariffPlanID, $IsActive,
    $PrimaryMeterID, $PrimaryMeterNumber, $PrimaryMeterLocation,
    $CurrentDue, $CurrentCredit, $CurrentBalance,
    $LastInvoiceID, $LastInvoiceNumber, $LastInvoiceDate, $LastInvoiceTotal, $LastInvoiceRemaining,
    $LastSyncedAt
);";

                    cmd.Parameters.AddWithValue("$SubscriberID", x.SubscriberID);
                    cmd.Parameters.AddWithValue("$SubscriberName", x.SubscriberName);
                    cmd.Parameters.AddWithValue("$PhoneNumber", (object)x.PhoneNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$Address", (object)x.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$AccountID", (object)x.AccountID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$TariffPlanID", (object)x.TariffPlanID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$IsActive", x.IsActive ? 1 : 0);
                    cmd.Parameters.AddWithValue("$PrimaryMeterID", (object)x.PrimaryMeterID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$PrimaryMeterNumber", (object)x.PrimaryMeterNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$PrimaryMeterLocation", (object)x.PrimaryMeterLocation ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$CurrentDue", x.CurrentDue);
                    cmd.Parameters.AddWithValue("$CurrentCredit", x.CurrentCredit);
                    cmd.Parameters.AddWithValue("$CurrentBalance", x.CurrentBalance);
                    cmd.Parameters.AddWithValue("$LastInvoiceID", (object)x.LastInvoiceID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$LastInvoiceNumber", (object)x.LastInvoiceNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$LastInvoiceDate", x.LastInvoiceDate.HasValue ? (object)x.LastInvoiceDate.Value.ToString("yyyy-MM-dd") : DBNull.Value);
                    cmd.Parameters.AddWithValue("$LastInvoiceTotal", (object)x.LastInvoiceTotal ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$LastInvoiceRemaining", (object)x.LastInvoiceRemaining ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$LastSyncedAt", DateTime.UtcNow.ToString("O"));

                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }

        private static async Task InsertMetersAsync(
            SqliteConnection con,
            SqliteTransaction tx,
            IEnumerable<LocalSubscriberMeterDto> items,
            CancellationToken cancellationToken)
        {
            foreach (var x in items)
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"
INSERT INTO LocalSubscriberMeters
(
    SubscriberMeterID, SubscriberID, MeterID, MeterNumber, MeterType, Location,
    IsActive, IsPrimary, LinkedAt
)
VALUES
(
    $SubscriberMeterID, $SubscriberID, $MeterID, $MeterNumber, $MeterType, $Location,
    $IsActive, $IsPrimary, $LinkedAt
);";

                    cmd.Parameters.AddWithValue("$SubscriberMeterID", x.SubscriberMeterID);
                    cmd.Parameters.AddWithValue("$SubscriberID", x.SubscriberID);
                    cmd.Parameters.AddWithValue("$MeterID", x.MeterID);
                    cmd.Parameters.AddWithValue("$MeterNumber", x.MeterNumber);
                    cmd.Parameters.AddWithValue("$MeterType", (object)x.MeterType ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$Location", (object)x.Location ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$IsActive", x.IsActive ? 1 : 0);
                    cmd.Parameters.AddWithValue("$IsPrimary", x.IsPrimary ? 1 : 0);
                    cmd.Parameters.AddWithValue("$LinkedAt", x.LinkedAt.HasValue ? (object)x.LinkedAt.Value.ToString("O") : DBNull.Value);

                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }

        private static async Task InsertInvoicesAsync(
            SqliteConnection con,
            SqliteTransaction tx,
            IEnumerable<LocalOpenInvoiceDto> items,
            CancellationToken cancellationToken)
        {
            foreach (var x in items)
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"
INSERT INTO LocalOpenInvoices
(
    InvoiceID, SubscriberID, MeterID, InvoiceNumber, InvoiceDate,
    Consumption, UnitPrice, ServiceFees, Arrears,
    TotalAmount, GrandTotal, PaidTotal, Remaining, Status, Notes, LastSyncedAt
)
VALUES
(
    $InvoiceID, $SubscriberID, $MeterID, $InvoiceNumber, $InvoiceDate,
    $Consumption, $UnitPrice, $ServiceFees, $Arrears,
    $TotalAmount, $GrandTotal, $PaidTotal, $Remaining, $Status, $Notes, $LastSyncedAt
);";

                    cmd.Parameters.AddWithValue("$InvoiceID", x.InvoiceID);
                    cmd.Parameters.AddWithValue("$SubscriberID", x.SubscriberID);
                    cmd.Parameters.AddWithValue("$MeterID", (object)x.MeterID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$InvoiceNumber", (object)x.InvoiceNumber ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$InvoiceDate", x.InvoiceDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("$Consumption", x.Consumption);
                    cmd.Parameters.AddWithValue("$UnitPrice", x.UnitPrice);
                    cmd.Parameters.AddWithValue("$ServiceFees", x.ServiceFees);
                    cmd.Parameters.AddWithValue("$Arrears", x.Arrears);
                    cmd.Parameters.AddWithValue("$TotalAmount", x.TotalAmount);
                    cmd.Parameters.AddWithValue("$GrandTotal", x.GrandTotal);
                    cmd.Parameters.AddWithValue("$PaidTotal", x.PaidTotal);
                    cmd.Parameters.AddWithValue("$Remaining", x.Remaining);
                    cmd.Parameters.AddWithValue("$Status", (object)x.Status ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$Notes", (object)x.Notes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$LastSyncedAt", DateTime.UtcNow.ToString("O"));

                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }

        private static async Task InsertCreditsAsync(
            SqliteConnection con,
            SqliteTransaction tx,
            IEnumerable<LocalSubscriberCreditDto> items,
            CancellationToken cancellationToken)
        {
            foreach (var x in items)
            {
                using (var cmd = con.CreateCommand())
                {
                    cmd.Transaction = tx;
                    cmd.CommandText = @"
INSERT INTO LocalSubscriberCredits
(
    CreditID, SubscriberID, PaymentID, ReceiptID, MeterID, CreditDate,
    AmountTotal, AmountRemaining, Notes, LastSyncedAt
)
VALUES
(
    $CreditID, $SubscriberID, $PaymentID, $ReceiptID, $MeterID, $CreditDate,
    $AmountTotal, $AmountRemaining, $Notes, $LastSyncedAt
);";

                    cmd.Parameters.AddWithValue("$CreditID", x.CreditID);
                    cmd.Parameters.AddWithValue("$SubscriberID", x.SubscriberID);
                    cmd.Parameters.AddWithValue("$PaymentID", (object)x.PaymentID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$ReceiptID", (object)x.ReceiptID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$MeterID", (object)x.MeterID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$CreditDate", x.CreditDate.ToString("yyyy-MM-dd"));
                    cmd.Parameters.AddWithValue("$AmountTotal", x.AmountTotal);
                    cmd.Parameters.AddWithValue("$AmountRemaining", x.AmountRemaining);
                    cmd.Parameters.AddWithValue("$Notes", (object)x.Notes ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("$LastSyncedAt", DateTime.UtcNow.ToString("O"));

                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }
            }
        }

        private static async Task UpsertSyncInfoAsync(
            SqliteConnection con,
            SqliteTransaction tx,
            string key,
            string value,
            CancellationToken cancellationToken)
        {
            using (var cmd = con.CreateCommand())
            {
                cmd.Transaction = tx;
                cmd.CommandText = @"
INSERT INTO LocalSyncInfo (SyncKey, SyncValue, UpdatedAt)
VALUES ($key, $value, $updatedAt)
ON CONFLICT(SyncKey) DO UPDATE SET
    SyncValue = excluded.SyncValue,
    UpdatedAt = excluded.UpdatedAt;";

                cmd.Parameters.AddWithValue("$key", key);
                cmd.Parameters.AddWithValue("$value", (object)value ?? DBNull.Value);
                cmd.Parameters.AddWithValue("$updatedAt", DateTime.UtcNow.ToString("O"));

                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
        }
    }
}