using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
        public class CollectorSubscriberReportItem
        {
            public int CollectorID { get; set; }
            public string CollectorName { get; set; }
            public int SubscriberID { get; set; }
            public string SubscriberName { get; set; }
            public string PhoneNumber { get; set; }
            public string Address { get; set; }
            public string PrimaryMeterNumber { get; set; }
        }
    }
