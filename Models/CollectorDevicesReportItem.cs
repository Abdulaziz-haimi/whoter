using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



    namespace water3.Models
    {
        public class CollectorDevicesReportItem
        {
            public int DeviceID { get; set; }
            public int CollectorID { get; set; }
            public string CollectorName { get; set; }
            public string DeviceCode { get; set; }
            public string DeviceName { get; set; }
            public string DeviceModel { get; set; }
            public string AppVersion { get; set; }
            public bool IsApproved { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? LastSyncAt { get; set; }
        }
    }
