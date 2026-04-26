using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{

        public class CollectorCollectionsReportItem
        {
            public int CollectorID { get; set; }
            public string CollectorName { get; set; }
            public int PaymentsCount { get; set; }
            public int ReceiptsCount { get; set; }
            public decimal TotalCollected { get; set; }
            public decimal InvoicePayments { get; set; }
            public decimal AdvanceCredits { get; set; }
            public decimal CreditSettlements { get; set; }
        }
    }
