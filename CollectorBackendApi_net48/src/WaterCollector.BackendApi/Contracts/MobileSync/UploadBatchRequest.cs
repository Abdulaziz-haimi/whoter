using System;
using System.Collections.Generic;

namespace WaterCollector.BackendApi.Contracts.MobileSync
{
    public sealed class UploadBatchRequest
    {
        public string DeviceCode { get; set; }
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string AppVersion { get; set; }
        public bool AutoCreateDevice { get; set; } = true;
        public List<UploadReceiptRow> Receipts { get; set; } = new List<UploadReceiptRow>();
        public List<UploadReceiptLineRow> Lines { get; set; } = new List<UploadReceiptLineRow>();
    }

    public sealed class UploadReceiptRow
    {
        public int RowNo { get; set; }
        public string LocalPaymentGuid { get; set; }
        public string LocalReceiptNo { get; set; }
        public int SubscriberId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalReceived { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string Notes { get; set; }
    }

    public sealed class UploadReceiptLineRow
    {
        public int ReceiptRowNo { get; set; }
        public int? InvoiceId { get; set; }
        public decimal AppliedAmount { get; set; }
        public string ApplicationType { get; set; } = "InvoicePayment";
        public string Notes { get; set; }
    }
}
