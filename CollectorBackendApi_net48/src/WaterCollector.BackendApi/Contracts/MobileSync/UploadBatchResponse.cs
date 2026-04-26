using System.Collections.Generic;

namespace WaterCollector.BackendApi.Contracts.MobileSync
{
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
