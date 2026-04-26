using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{

        public class MobileSyncErrorReportItem
        {
            public int ErrorID { get; set; }
            public int? SyncBatchID { get; set; }
            public int? ImportID { get; set; }
            public string ErrorCode { get; set; }
            public string ErrorMessage { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
