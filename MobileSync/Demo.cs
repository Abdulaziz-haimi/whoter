using System;
using System.Threading.Tasks;
using water3.MobileSync;

public class Demo
{
    public async Task RunAsync()
    {
        string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        MobileSyncService service = MobileSyncBootstrap.Build(appFolder);

        await service.InitializeAsync();
        //public static HybridMobileSyncCoordinator BuildHybridCoordinator(
        //   string appDataFolder,
        //   string sqlServerConnectionString)
        //{
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

