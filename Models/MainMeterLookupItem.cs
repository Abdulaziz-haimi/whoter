using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
        public class MainMeterLookupItem
        {
            public int MeterID { get; set; }
            public string MeterNumber { get; set; }
            public string Location { get; set; }

            public override string ToString()
            {
                if (string.IsNullOrWhiteSpace(Location))
                    return MeterNumber;

                return MeterNumber + " - " + Location;
            }
        }
    }