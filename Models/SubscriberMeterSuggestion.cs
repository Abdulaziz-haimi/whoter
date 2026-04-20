using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
  
        public class SubscriberMeterSuggestion
        {
            public int SubscriberID { get; set; }
            public int MeterID { get; set; }
            public string DisplayText { get; set; } = "";

            public override string ToString() => DisplayText;
        }
    }

