using System;

namespace water3.Models
{
    
        public class InvoiceFilters
        {
            public DateTime From { get; set; }
            public DateTime To { get; set; }                 // inclusive from UI
            public DateTime ToExclusive => To.Date.AddDays(1);

            public int SubscriberId { get; set; }            // 0 => All
            public string Status { get; set; }               // null => All
            public string SearchLike { get; set; }           // "%x%" or null
        }
    }
