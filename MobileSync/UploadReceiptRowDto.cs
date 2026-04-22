using System;
using System.Collections.Generic;

namespace water3.MobileSync
{
    public sealed class UploadReceiptRowDto
    {
        public int RowNo { get; set; }
        public string LocalPaymentGuid { get; set; } = string.Empty;
        public string LocalReceiptNo { get; set; } = string.Empty;
        public int SubscriberID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal TotalReceived { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string Notes { get; set; }
        public long LocalReceiptID { get; set; }
    }

    public sealed class UploadReceiptLineRowDto
    {
        public int ReceiptRowNo { get; set; }
        public int? InvoiceID { get; set; }
        public decimal AppliedAmount { get; set; }
        public string ApplicationType { get; set; } = "InvoicePayment";
        public string Notes { get; set; }
    }

    public sealed class UploadBatchRequestDto
    {
        public int CollectorID { get; set; }
        public string DeviceCode { get; set; } = string.Empty;
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string AppVersion { get; set; }
        public List<UploadReceiptRowDto> Receipts { get; set; } = new List<UploadReceiptRowDto>();
        public List<UploadReceiptLineRowDto> Lines { get; set; } = new List<UploadReceiptLineRowDto>();
        public bool AutoCreateDevice { get; set; } = true;
    }

    public sealed class UploadBatchResultDto
    {
        public int SyncBatchID { get; set; }
        public int DeviceID { get; set; }
        public int TotalRows { get; set; }
        public int InsertedCount { get; set; }
        public int DuplicateCount { get; set; }
        public string BatchStatus { get; set; } = string.Empty;
        public List<UploadRowResultDto> RowResults { get; set; } = new List<UploadRowResultDto>();
    }

    public sealed class UploadRowResultDto
    {
        public int RowNo { get; set; }
        public string LocalPaymentGuid { get; set; } = string.Empty;
        public int? ImportID { get; set; }
        public string SaveStatus { get; set; } = string.Empty;
    }
}