using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace water3.MobileSync
{
    public sealed class LocalSqliteSchema
    {
        private readonly string _dbPath;

        public LocalSqliteSchema(string dbPath)
        {
            if (string.IsNullOrWhiteSpace(dbPath))
                throw new ArgumentException("Database path is required.", nameof(dbPath));

            _dbPath = dbPath;
        }

        public string ConnectionString => new SqliteConnectionStringBuilder
        {
            DataSource = _dbPath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Shared
        }.ToString();

        public async Task EnsureCreatedAsync(CancellationToken cancellationToken = default)
        {
            string? dir = Path.GetDirectoryName(_dbPath);
            if (!string.IsNullOrWhiteSpace(dir) && !Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            await using var con = new SqliteConnection(ConnectionString);
            await con.OpenAsync(cancellationToken);

            string sql = @"
CREATE TABLE IF NOT EXISTS LocalSyncInfo
(
    SyncKey           TEXT PRIMARY KEY,
    SyncValue         TEXT,
    UpdatedAt         TEXT
);

CREATE TABLE IF NOT EXISTS LocalSubscribers
(
    SubscriberID          INTEGER PRIMARY KEY,
    SubscriberName        TEXT NOT NULL,
    PhoneNumber           TEXT,
    Address               TEXT,
    AccountID             INTEGER,
    TariffPlanID          INTEGER,
    IsActive              INTEGER NOT NULL DEFAULT 1,
    PrimaryMeterID        INTEGER,
    PrimaryMeterNumber    TEXT,
    PrimaryMeterLocation  TEXT,
    CurrentDue            REAL NOT NULL DEFAULT 0,
    CurrentCredit         REAL NOT NULL DEFAULT 0,
    CurrentBalance        REAL NOT NULL DEFAULT 0,
    LastInvoiceID         INTEGER,
    LastInvoiceNumber     TEXT,
    LastInvoiceDate       TEXT,
    LastInvoiceTotal      REAL,
    LastInvoiceRemaining  REAL,
    LastSyncedAt          TEXT
);

CREATE TABLE IF NOT EXISTS LocalSubscriberMeters
(
    SubscriberMeterID     INTEGER PRIMARY KEY,
    SubscriberID          INTEGER NOT NULL,
    MeterID               INTEGER NOT NULL,
    MeterNumber           TEXT NOT NULL,
    MeterType             TEXT,
    Location              TEXT,
    IsActive              INTEGER NOT NULL DEFAULT 1,
    IsPrimary             INTEGER NOT NULL DEFAULT 0,
    LinkedAt              TEXT
);

CREATE TABLE IF NOT EXISTS LocalOpenInvoices
(
    InvoiceID             INTEGER PRIMARY KEY,
    SubscriberID          INTEGER NOT NULL,
    MeterID               INTEGER,
    InvoiceNumber         TEXT,
    InvoiceDate           TEXT NOT NULL,
    Consumption           REAL NOT NULL DEFAULT 0,
    UnitPrice             REAL NOT NULL DEFAULT 0,
    ServiceFees           REAL NOT NULL DEFAULT 0,
    Arrears               REAL NOT NULL DEFAULT 0,
    TotalAmount           REAL NOT NULL DEFAULT 0,
    GrandTotal            REAL NOT NULL DEFAULT 0,
    PaidTotal             REAL NOT NULL DEFAULT 0,
    Remaining             REAL NOT NULL DEFAULT 0,
    Status                TEXT,
    Notes                 TEXT,
    LastSyncedAt          TEXT
);

CREATE TABLE IF NOT EXISTS LocalSubscriberCredits
(
    CreditID              INTEGER PRIMARY KEY,
    SubscriberID          INTEGER NOT NULL,
    PaymentID             INTEGER,
    ReceiptID             INTEGER,
    MeterID               INTEGER,
    CreditDate            TEXT NOT NULL,
    AmountTotal           REAL NOT NULL DEFAULT 0,
    AmountRemaining       REAL NOT NULL DEFAULT 0,
    Notes                 TEXT,
    LastSyncedAt          TEXT
);

CREATE TABLE IF NOT EXISTS LocalReceiptDrafts
(
    LocalReceiptID        INTEGER PRIMARY KEY AUTOINCREMENT,
    LocalPaymentGuid      TEXT NOT NULL,
    LocalReceiptNo        TEXT NOT NULL,
    SubscriberID          INTEGER NOT NULL,
    CollectorID           INTEGER NOT NULL,
    PaymentDate           TEXT NOT NULL,
    TotalReceived         REAL NOT NULL,
    PaymentMethod         TEXT NOT NULL,
    Notes                 TEXT,
    SyncStatus            TEXT NOT NULL DEFAULT 'Pending',
    SyncBatchRef          TEXT,
    ServerImportID        INTEGER,
    ServerStatus          TEXT,
    RejectedReason        TEXT,
    CreatedAt             TEXT NOT NULL,
    UpdatedAt             TEXT,
    SentAt                TEXT,
    UNIQUE(LocalPaymentGuid)
);

CREATE TABLE IF NOT EXISTS LocalReceiptDraftLines
(
    LocalReceiptLineID    INTEGER PRIMARY KEY AUTOINCREMENT,
    LocalReceiptID        INTEGER NOT NULL,
    InvoiceID             INTEGER,
    AppliedAmount         REAL NOT NULL,
    ApplicationType       TEXT NOT NULL,
    Notes                 TEXT
);

CREATE INDEX IF NOT EXISTS IX_LocalSubscribers_Name
ON LocalSubscribers(SubscriberName);

CREATE INDEX IF NOT EXISTS IX_LocalSubscribers_Phone
ON LocalSubscribers(PhoneNumber);

CREATE INDEX IF NOT EXISTS IX_LocalSubscribers_PrimaryMeter
ON LocalSubscribers(PrimaryMeterNumber);

CREATE INDEX IF NOT EXISTS IX_LocalSubscriberMeters_SubscriberID
ON LocalSubscriberMeters(SubscriberID);

CREATE INDEX IF NOT EXISTS IX_LocalSubscriberMeters_MeterNumber
ON LocalSubscriberMeters(MeterNumber);

CREATE INDEX IF NOT EXISTS IX_LocalOpenInvoices_SubscriberID
ON LocalOpenInvoices(SubscriberID);

CREATE INDEX IF NOT EXISTS IX_LocalOpenInvoices_InvoiceDate
ON LocalOpenInvoices(InvoiceDate);

CREATE INDEX IF NOT EXISTS IX_LocalOpenInvoices_InvoiceNumber
ON LocalOpenInvoices(InvoiceNumber);

CREATE INDEX IF NOT EXISTS IX_LocalSubscriberCredits_SubscriberID
ON LocalSubscriberCredits(SubscriberID);

CREATE INDEX IF NOT EXISTS IX_LocalSubscriberCredits_MeterID
ON LocalSubscriberCredits(MeterID);

CREATE INDEX IF NOT EXISTS IX_LocalReceiptDrafts_SubscriberID
ON LocalReceiptDrafts(SubscriberID);

CREATE INDEX IF NOT EXISTS IX_LocalReceiptDrafts_SyncStatus
ON LocalReceiptDrafts(SyncStatus);

CREATE INDEX IF NOT EXISTS IX_LocalReceiptDrafts_PaymentDate
ON LocalReceiptDrafts(PaymentDate);

CREATE INDEX IF NOT EXISTS IX_LocalReceiptDraftLines_LocalReceiptID
ON LocalReceiptDraftLines(LocalReceiptID);

CREATE INDEX IF NOT EXISTS IX_LocalReceiptDraftLines_InvoiceID
ON LocalReceiptDraftLines(InvoiceID);
";

            await using var cmd = con.CreateCommand();
            cmd.CommandText = sql;
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}