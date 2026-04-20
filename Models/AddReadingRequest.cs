using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
  
        public class AddReadingRequest
        {
            public int SubscriberID { get; set; }
            public int MeterID { get; set; }
            public DateTime ReadingDate { get; set; }
            public decimal CurrentReading { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal ServiceFees { get; set; }
            public string Notes { get; set; } // nullable allowed
        }
    }

