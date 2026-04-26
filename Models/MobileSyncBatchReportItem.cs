using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    namespace water3.Models
    {
        public class MobileSyncBatchReportItem
        {
            public int SyncBatchID { get; set; }
            public int CollectorID { get; set; }
            public string CollectorName { get; set; }
            public int DeviceID { get; set; }
            public string DeviceCode { get; set; }
            public string DeviceName { get; set; }
            public DateTime BatchDate { get; set; }
            public string BatchStatus { get; set; }
            public int ItemsCount { get; set; }
            public int ApprovedCount { get; set; }
            public int RejectedCount { get; set; }
            public DateTime? ProcessedAt { get; set; }
        }
    }
