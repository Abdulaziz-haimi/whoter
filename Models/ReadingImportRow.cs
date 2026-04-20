using System;

namespace water3.Models
{
    public class ReadingImportRow
    {
        public int RowNumber { get; set; }

        public int SubscriberID { get; set; }
        public int MeterID { get; set; }
        public DateTime ReadingDate { get; set; }
        public decimal CurrentReading { get; set; }
        public string Notes { get; set; } = string.Empty;

        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}