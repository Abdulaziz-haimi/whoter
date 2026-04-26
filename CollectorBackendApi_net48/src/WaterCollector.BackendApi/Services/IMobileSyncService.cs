using System;
using System.Threading.Tasks;
using WaterCollector.BackendApi.Contracts.MobileSync;

namespace WaterCollector.BackendApi.Services
{
    public interface IMobileSyncService
    {
        Task<ReceivablesExportResponse> ExportReceivablesAsync(int collectorId, DateTime? asOfDate);
        Task<UploadBatchResponse> UploadBatchAsync(int collectorId, UploadBatchRequest request);
        Task<ImportDecisionsResponse> GetImportDecisionsAsync(int collectorId, string deviceCode, DateTime? changedAfter);
    }
}
