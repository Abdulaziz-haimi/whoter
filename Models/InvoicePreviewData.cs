using System;

namespace water3.Models
{
    public class InvoicePreviewData
    {
        public int InvoiceID { get; set; }
        public DateTime InvoiceDate { get; set; }

        public string SubscriberName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string MeterNumber { get; set; }

        public decimal PreviousReading { get; set; }
        public decimal CurrentReading { get; set; }
        public decimal Consumption { get; set; }

        public decimal UnitPrice { get; set; }
        public decimal ServiceFees { get; set; }
        public decimal Arrears { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }

        public string StatusText { get; set; }
    }
}
