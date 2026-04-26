using System;
using System.Collections.Generic;

namespace WaterCollector.BackendApi.Contracts.MobileSync
{
    public sealed class ImportDecisionsResponse
    {
        public List<ImportDecisionItem> Decisions { get; set; } = new List<ImportDecisionItem>();
        public DateTime? MaxChangedAt { get; set; }
    }

    public sealed class ImportDecisionItem
    {
        public int ImportId { get; set; }
        public int SyncBatchId { get; set; }
        public string LocalPaymentGuid { get; set; }
        public string LocalReceiptNo { get; set; }
        public string ImportStatus { get; set; }
        public int? ApprovedReceiptId { get; set; }
        public string ReceiptNumber { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public int? ApprovedByUserId { get; set; }
        public string ApprovedByUserName { get; set; }
        public string RejectedReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
