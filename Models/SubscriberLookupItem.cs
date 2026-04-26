using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{

        public class SubscriberLookupItem
        {
            public int SubscriberID { get; set; }
            public string Name { get; set; }
            public string PhoneNumber { get; set; }
            public string Address { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }
    }