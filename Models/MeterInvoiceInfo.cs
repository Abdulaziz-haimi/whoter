using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
 
        public class MeterInvoiceInfo
        {
            public DateTime? InvoiceDate { get; set; }
            public decimal? TotalAmount { get; set; }
            public string Status { get; set; } = "";
        }
    }
