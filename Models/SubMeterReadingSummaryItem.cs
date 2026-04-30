using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{

        public class SubMeterReadingSummaryItem
        {
            public int SubscriberID { get; set; }
            public string SubscriberName { get; set; }
            public int MeterID { get; set; }
            public string MeterNumber { get; set; }
            public decimal PreviousReading { get; set; }
            public decimal CurrentReading { get; set; }
            public decimal Consumption { get; set; }
        }
    }
