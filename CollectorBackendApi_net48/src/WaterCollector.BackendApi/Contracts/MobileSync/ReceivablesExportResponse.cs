using System;
using System.Collections.Generic;

namespace WaterCollector.BackendApi.Contracts.MobileSync
{
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
}
