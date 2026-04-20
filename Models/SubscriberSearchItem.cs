using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
    
        public class SubscriberSearchItem
        {
            public int SubscriberID { get; set; }
            public string Name { get; set; } = "";
            public string MeterNumber { get; set; } = "";
            public string PhoneNumber { get; set; } = "";

            public string Display => $"{Name}  (عداد: {MeterNumber})  (هاتف: {PhoneNumber})";
            public override string ToString() => Display;
        }
    }

