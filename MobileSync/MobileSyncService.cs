using System;
using System.Threading;
using System.Threading.Tasks;

namespace water3.MobileSync
{
    public sealed class MobileSyncService
    {
        private readonly LocalSqliteSchema _schema;
        private readonly LocalSyncRepository _localSyncRepository;
        private readonly LocalReceiptRepository _localReceiptRepository;

        public MobileSyncService(
            LocalSqliteSchema schema,
            LocalSyncRepository localSyncRepository,
            LocalReceiptRepository localReceiptRepository)
        {
            _schema = schema;
            _localSyncRepository = localSyncRepository;
            _localReceiptRepository = localReceiptRepository;
        }

        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            await _schema.EnsureCreatedAsync(cancellationToken);
        }

        public async Task ImportReceivablesAsync(
            SyncExportBundle bundle,
            string deviceCode,
            CancellationToken cancellationToken = default)
        {
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            await _localSyncRepository.ReplaceExportDataAsync(bundle, deviceCode, cancellationToken);
        }

        public Task<long> SaveReceiptDraftAsync(LocalReceiptDraftDto draft, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(draft.LocalPaymentGuid))
                draft.LocalPaymentGuid = Guid.NewGuid().ToString("N");

            if (string.IsNullOrWhiteSpace(draft.LocalReceiptNo))
                draft.LocalReceiptNo = $"MOB-{DateTime.Now:yyyyMMddHHmmss}";

            if (draft.CreatedAt == default)
                draft.CreatedAt = DateTime.UtcNow;

            return _localReceiptRepository.CreateDraftAsync(draft, cancellationToken);
        }
    }
}