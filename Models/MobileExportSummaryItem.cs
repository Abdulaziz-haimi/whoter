using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{

        public class MobileExportSummaryItem
        {
            public int CollectorID { get; set; }
            public string CollectorName { get; set; }
            public string DeviceCode { get; set; }
            public string DeviceName { get; set; }
            public string AsOfDateText { get; set; }

            public int SubscribersCount { get; set; }
            public int MetersCount { get; set; }
            public int OpenInvoicesCount { get; set; }
            public int CreditsCount { get; set; }

            public decimal TotalOpenInvoicesAmount { get; set; }
            public decimal TotalCreditsAmount { get; set; }
        }
    }