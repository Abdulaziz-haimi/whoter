using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
   
        public class SmsQueueItem
        {
            public int SmsID { get; set; }
            public int SubscriberID { get; set; }
            public int? InvoiceID { get; set; }
            public string SubscriberName { get; set; } = "";
            public string PhoneNumber { get; set; } = "";
            public string Message { get; set; } = "";
            public string Status { get; set; } = "Pending"; // Pending/Sent/Failed
            public string Reason { get; set; } = "";
            public DateTime CreatedAt { get; set; }
            public int TemplateType { get; set; } // enum int
        }
    }

