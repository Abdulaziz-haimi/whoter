using System.IO;

namespace water3.MobileSync
{
    public static class MobileSyncBootstrap
    {
        public static MobileSyncService Build(string appDataFolder)
        {
            string dbPath = Path.Combine(appDataFolder, "collector_local.db");

            var schema = new LocalSqliteSchema(dbPath);
            var syncRepo = new LocalSyncRepository(schema.ConnectionString);
            var receiptRepo = new LocalReceiptRepository(schema.ConnectionString);

            return new MobileSyncService(schema, syncRepo, receiptRepo);
        }
    }
}