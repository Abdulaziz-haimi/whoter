using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using water3.MobileSync;
using System;
using System.Collections.Generic;
namespace water3.MobileSync
{
   
    public sealed class SyncExportBundle
    {
        public SyncExportSummaryDto Summary { get; set; } = new();
        public List<LocalSubscriberDto> Subscribers { get; set; } = new();
        public List<LocalSubscriberMeterDto> Meters { get; set; } = new();
        public List<LocalOpenInvoiceDto> OpenInvoices { get; set; } = new();
        public List<LocalSubscriberCreditDto> Credits { get; set; } = new();
    }

    public sealed class SyncExportSummaryDto
    {
        public int CollectorID { get; set; }
        public DateTime AsOfDate { get; set; }
        public DateTime ExportedAt { get; set; }
        public int SubscribersCount { get; set; }
        public int OpenInvoicesCount { get; set; }
        public int OpenCreditsCount { get; set; }
    }

    public sealed class LocalSubscriberDto
    {
        public int SubscriberID { get; set; }
        public string SubscriberName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public int? AccountID { get; set; }
        public int? TariffPlanID { get; set; }
        public bool IsActive { get; set; }

        public int? PrimaryMeterID { get; set; }
        public string PrimaryMeterNumber { get; set; }
        public string PrimaryMeterLocation { get; set; }

        public decimal CurrentDue { get; set; }
        public decimal CurrentCredit { get; set; }
        public decimal CurrentBalance { get; set; }

        public int? LastInvoiceID { get; set; }
        public string LastInvoiceNumber { get; set; }
        public DateTime? LastInvoiceDate { get; set; }
        public decimal? LastInvoiceTotal { get; set; }
        public decimal? LastInvoiceRemaining { get; set; }
    }

    public sealed class LocalSubscriberMeterDto
    {
        public int SubscriberMeterID { get; set; }
        public int SubscriberID { get; set; }
        public int MeterID { get; set; }
        public string MeterNumber { get; set; } = string.Empty;
        public string MeterType { get; set; }
        public string Location { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }
        public DateTime? LinkedAt { get; set; }
    }

    public sealed class LocalOpenInvoiceDto
    {
        public int InvoiceID { get; set; }
        public int SubscriberID { get; set; }
        public int? MeterID { get; set; }
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

    public sealed class LocalSubscriberCreditDto
    {
        public int CreditID { get; set; }
        public int SubscriberID { get; set; }
        public int? PaymentID { get; set; }
        public int? ReceiptID { get; set; }
        public int? MeterID { get; set; }
        public DateTime CreditDate { get; set; }
        public decimal AmountTotal { get; set; }
        public decimal AmountRemaining { get; set; }
        public string Notes { get; set; }
    }

    public sealed class LocalReceiptDraftDto
    {
        public long LocalReceiptID { get; set; }
        public string LocalPaymentGuid { get; set; } = string.Empty;
        public string LocalReceiptNo { get; set; } = string.Empty;
        public int SubscriberID { get; set; }
        public int CollectorID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalReceived { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string Notes { get; set; }

        public string SyncStatus { get; set; } = "Pending";
        public string SyncBatchRef { get; set; }
        public int? ServerImportID { get; set; }
        public string ServerStatus { get; set; }
        public string RejectedReason { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? SentAt { get; set; }

        public List<LocalReceiptDraftLineDto> Lines { get; set; } = new();
    }

    public sealed class LocalReceiptDraftLineDto
    {
        public long LocalReceiptLineID { get; set; }
        public long LocalReceiptID { get; set; }
        public int? InvoiceID { get; set; }
        public decimal AppliedAmount { get; set; }
        public string ApplicationType { get; set; } = "InvoicePayment";
        public string Notes { get; set; }
    }
}

/*using System;
using System.Threading.Tasks;
using water3.MobileSync;

public class Demo
{
    public async Task RunAsync()
    {
        string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        MobileSyncService service = MobileSyncBootstrap.Build(appFolder);

        await service.InitializeAsync();

        // لاحقًا: bundle سيأتي من إجراء usp_MobileSync_ExportReceivables
        var bundle = new SyncExportBundle
        {
            Summary = new SyncExportSummaryDto
            {
                CollectorID = 1,
                AsOfDate = DateTime.Today,
                ExportedAt = DateTime.Now,
                SubscribersCount = 0,
                OpenInvoicesCount = 0,
                OpenCreditsCount = 0
            }
        };

        await service.ImportReceivablesAsync(bundle, "PHONE-ABC-123");

        var draft = new LocalReceiptDraftDto
        {
            SubscriberID = 1,
            CollectorID = 1,
            PaymentDate = DateTime.Today,
            TotalReceived = 500m,
            PaymentMethod = "Cash",
            Notes = "تحصيل ميداني",
            Lines =
            {
                new LocalReceiptDraftLineDto
                {
                    InvoiceID = 10,
                    AppliedAmount = 400m,
                    ApplicationType = "InvoicePayment"
                },
                new LocalReceiptDraftLineDto
                {
                    InvoiceID = null,
                    AppliedAmount = 100m,
                    ApplicationType = "AdvanceCredit"
                }
            }
        };

        long localReceiptId = await service.SaveReceiptDraftAsync(draft);
    }
}
*/