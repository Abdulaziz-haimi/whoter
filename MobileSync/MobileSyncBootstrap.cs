using System.IO;

namespace water3.MobileSync
{
    public static partial class MobileSyncBootstrap
    {
        public static MobileSyncService Build(string appDataFolder)
        {
            string dbPath = Path.Combine(appDataFolder, "collector_local.db");

            var schema = new LocalSqliteSchema(dbPath);
            var syncRepo = new LocalSyncRepository(schema.ConnectionString);
            var receiptRepo = new LocalReceiptRepository(schema.ConnectionString);

            return new MobileSyncService(schema, syncRepo, receiptRepo);
        }

        public static HybridMobileSyncCoordinator BuildHybridCoordinator(
            string appDataFolder,
            string sqlServerConnectionString)
        {
            string dbPath = Path.Combine(appDataFolder, "collector_local.db");

            var schema = new LocalSqliteSchema(dbPath);
            var syncRepo = new LocalSyncRepository(schema.ConnectionString);
            var receiptRepo = new LocalReceiptRepository(schema.ConnectionString);

            var mobileSyncService = new MobileSyncService(schema, syncRepo, receiptRepo);
            var sqlServerExportReader = new SqlServerExportReader(sqlServerConnectionString);
            var sqlServerUploadClient = new SqlServerUploadClient(sqlServerConnectionString);

            return new HybridMobileSyncCoordinator(
                mobileSyncService,
                receiptRepo,
                sqlServerExportReader,
                sqlServerUploadClient);
        }
    }
}
/*using System.IO;

namespace water3.MobileSync
{
    public static partial class MobileSyncBootstrap
    {
        public static MobileSyncService Build(
            string appDataFolder)
        {
            string dbPath = Path.Combine(appDataFolder, "collector_local.db");

            var schema = new LocalSqliteSchema(dbPath);
            var syncRepo = new LocalSyncRepository(schema.ConnectionString);
            var receiptRepo = new LocalReceiptRepository(schema.ConnectionString);

            return new MobileSyncService(schema, syncRepo, receiptRepo);
        }
    }
}
*/