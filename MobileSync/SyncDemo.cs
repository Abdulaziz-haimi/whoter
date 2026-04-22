using System;
using System.Threading.Tasks;
using water3;
using water3.MobileSync;

public class SyncDemo
{
    private readonly string _connectionString = Db.ConnectionString;

    public async Task RunAsync()
    {
        string appFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string sqlServerConnectionString = "Server=...;Database=WaterBillingDB;User Id=...;Password=...;TrustServerCertificate=True;";
         
    HybridMobileSyncCoordinator coordinator = MobileSyncBootstrap.BuildHybridCoordinator(
            appFolder,
            sqlServerConnectionString);

        // تنزيل المستحقات من اللابتوب إلى الهاتف
        SyncExportBundle bundle = await coordinator.DownloadReceivablesToPhoneAsync(
            collectorId: 1,
            deviceCode: "PHONE-ABC-123",
            asOfDate: DateTime.Today);

        // لاحقًا بعد إنشاء مسودات Pending داخل الهاتف:
        UploadBatchResultDto uploadResult = await coordinator.UploadPendingDraftsAsync(
            collectorId: 1,
            deviceCode: "PHONE-ABC-123",
            deviceName: "Samsung A15",
            deviceModel: "SM-A155F",
            appVersion: "1.0.0");

        if (uploadResult != null)
        {
            Console.WriteLine($"Batch: {uploadResult.SyncBatchID}, Status: {uploadResult.BatchStatus}");
        }
    }
}