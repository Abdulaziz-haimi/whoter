using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace water3.MobileSync
{
    
    public sealed class HybridMobileSyncCoordinator
    {
        private readonly MobileSyncService _mobileSyncService;
        private readonly LocalReceiptRepository _localReceiptRepository;
        private readonly SqlServerExportReader _sqlServerExportReader;
        private readonly SqlServerUploadClient _sqlServerUploadClient;

        public HybridMobileSyncCoordinator(
            MobileSyncService mobileSyncService,
            LocalReceiptRepository localReceiptRepository,
            SqlServerExportReader sqlServerExportReader,
            SqlServerUploadClient sqlServerUploadClient)
        {
            _mobileSyncService = mobileSyncService;
            _localReceiptRepository = localReceiptRepository;
            _sqlServerExportReader = sqlServerExportReader;
            _sqlServerUploadClient = sqlServerUploadClient;
        }

        public async Task<SyncExportBundle> DownloadReceivablesToPhoneAsync(
            int collectorId,
            string deviceCode,
            DateTime? asOfDate = null,
            CancellationToken cancellationToken = default)
        {
            SyncExportBundle bundle = await _sqlServerExportReader.ExportReceivablesAsync(
                collectorId,
                asOfDate,
                onlyAssignedSubscribers: true,
                includeAllIfNoAssignments: true,
                cancellationToken: cancellationToken);

            await _mobileSyncService.ImportReceivablesAsync(bundle, deviceCode, cancellationToken);
            return bundle;
        }

        public async Task<UploadBatchResultDto> UploadPendingDraftsAsync(
            int collectorId,
            string deviceCode,
            string deviceName,
            string deviceModel,
            string appVersion,
            CancellationToken cancellationToken = default)
        {
            UploadBatchRequestDto request = await _localReceiptRepository.BuildPendingUploadBatchAsync(
                collectorId,
                deviceCode,
                deviceName,
                deviceModel,
                appVersion,
                cancellationToken);

            if (request.Receipts.Count == 0)
                return null;

            UploadBatchResultDto result = await _sqlServerUploadClient.SaveBatchAsync(request, cancellationToken);

            var receiptMap = request.Receipts.ToDictionary(x => x.RowNo, x => x);

            foreach (UploadRowResultDto row in result.RowResults)
            {
                if (!receiptMap.TryGetValue(row.RowNo, out UploadReceiptRowDto receiptRow))
                    continue;

                if (row.SaveStatus.Equals("Inserted", StringComparison.OrdinalIgnoreCase))
                {
                    await _localReceiptRepository.MarkDraftAsSentAsync(
                        receiptRow.LocalReceiptID,
                        result.SyncBatchID.ToString(),
                        row.ImportID,
                        "Inserted",
                        cancellationToken);
                }
                else if (row.SaveStatus.Equals("Duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    await _localReceiptRepository.MarkDraftAsDuplicateAsync(
                        receiptRow.LocalReceiptID,
                        row.ImportID,
                        cancellationToken);
                }
            }

            return result;
        }
    }
}