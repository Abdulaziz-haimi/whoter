using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
   
        public class SmsLogItem
        {
            public int SmsID { get; set; }
            public int? InvoiceID { get; set; }
            public int? SubscriberID { get; set; }
            public string PhoneNumber { get; set; }
            public string Message { get; set; }
            public string Status { get; set; }      // Pending / Sent / Failed
            public string Reason { get; set; }
            public DateTime? SentDate { get; set; }
            public int? CollectorID { get; set; }
            public int? TemplateID { get; set; }
            public int RetryCount { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
