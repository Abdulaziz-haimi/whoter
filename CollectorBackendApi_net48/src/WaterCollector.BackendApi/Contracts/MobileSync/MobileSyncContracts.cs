using System;
using System.Collections.Generic;

namespace WaterCollector.BackendApi.Contracts.MobileSync
{
    public sealed class ImportDecisionsResponse
    {
        public List<ImportDecisionItem> Decisions { get; set; } = new List<ImportDecisionItem>();
        public DateTime? MaxChangedAt { get; set; }
    }

    public sealed class ImportDecisionItem
    {
        public int ImportId { get; set; }
        public int SyncBatchId { get; set; }
        public string LocalPaymentGuid { get; set; }
        public string LocalReceiptNo { get; set; }
        public string ImportStatus { get; set; }
        public int? ApprovedReceiptId { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int? ApprovedByUserId { get; set; }
        public string ApprovedByUserName { get; set; }
        public string RejectedReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }
    }

    public sealed class ReceivablesExportResponse
    {
        public SyncExportSummary Summary { get; set; } = new SyncExportSummary();
        public List<ReceivableSubscriber> Subscribers { get; set; } = new List<ReceivableSubscriber>();
        public List<ReceivableMeter> Meters { get; set; } = new List<ReceivableMeter>();
        public List<ReceivableInvoice> OpenInvoices { get; set; } = new List<ReceivableInvoice>();
        public List<ReceivableCredit> OpenCredits { get; set; } = new List<ReceivableCredit>();
    }

    public sealed class SyncExportSummary
    {
        public int CollectorId { get; set; }
        public DateTime AsOfDate { get; set; }
        public DateTime ExportedAt { get; set; }
        public int SubscribersCount { get; set; }
        public int OpenInvoicesCount { get; set; }
        public int OpenCreditsCount { get; set; }
    }

    public sealed class ReceivableSubscriber
    {
        public int SubscriberId { get; set; }
        public string SubscriberName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int? AccountId { get; set; }
        public int? TariffPlanId { get; set; }
        public bool IsActive { get; set; }
        public int? PrimaryMeterId { get; set; }
        public string PrimaryMeterNumber { get; set; }
        public string PrimaryMeterLocation { get; set; }
        public decimal CurrentDue { get; set; }
        public decimal CurrentCredit { get; set; }
        public decimal CurrentBalance { get; set; }
        public int? LastInvoiceId { get; set; }
        public string LastInvoiceNumber { get; set; }
        public DateTime? LastInvoiceDate { get; set; }
        public decimal? LastInvoiceTotal { get; set; }
        public decimal? LastInvoiceRemaining { get; set; }
    }

    public sealed class ReceivableMeter
    {
        public int SubscriberMeterId { get; set; }
        public int SubscriberId { get; set; }
        public int MeterId { get; set; }
        public string MeterNumber { get; set; }
        public string MeterType { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime? LinkedAt { get; set; }
    }

    public sealed class ReceivableInvoice
    {
        public int InvoiceId { get; set; }
        public int SubscriberId { get; set; }
        public int? MeterId { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Consumption { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ServiceFees { get; set; }
        public decimal Arrears { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal PaidTotal { get; set; }
        public decimal Remaining { get; set; }
        public string Status { get; set; }
        public string Notes { get; set; }
    }

    public sealed class ReceivableCredit
    {
        public int CreditId { get; set; }
        public int SubscriberId { get; set; }
        public int? PaymentId { get; set; }
        public int? ReceiptId { get; set; }
        public int? MeterId { get; set; }
        public DateTime CreditDate { get; set; }
        public decimal AmountTotal { get; set; }
        public decimal AmountRemaining { get; set; }
        public string Notes { get; set; }
    }

    public sealed class UploadBatchRequest
    {
        public string DeviceCode { get; set; }
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string AppVersion { get; set; }
        public bool AutoCreateDevice { get; set; } = true;
        public List<UploadReceiptRow> Receipts { get; set; } = new List<UploadReceiptRow>();
        public List<UploadReceiptLineRow> Lines { get; set; } = new List<UploadReceiptLineRow>();
    }

    public sealed class UploadReceiptRow
    {
        public int RowNo { get; set; }
        public string LocalPaymentGuid { get; set; }
        public string LocalReceiptNo { get; set; }
        public int SubscriberId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalReceived { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string Notes { get; set; }
    }

    public sealed class UploadReceiptLineRow
    {
        public int ReceiptRowNo { get; set; }
        public int? InvoiceId { get; set; }
        public decimal AppliedAmount { get; set; }
        public string ApplicationType { get; set; } = "InvoicePayment";
        public string Notes { get; set; }
    }

    public sealed class UploadBatchResponse
    {
        public int SyncBatchId { get; set; }
        public int DeviceId { get; set; }
        public int TotalRows { get; set; }
        public int InsertedCount { get; set; }
        public int DuplicateCount { get; set; }
        public string BatchStatus { get; set; }
        public List<UploadRowResult> RowResults { get; set; } = new List<UploadRowResult>();
    }

    public sealed class UploadRowResult
    {
        public int RowNo { get; set; }
        public string LocalPaymentGuid { get; set; }
        public int? ImportId { get; set; }
        public string SaveStatus { get; set; }
    }
}
