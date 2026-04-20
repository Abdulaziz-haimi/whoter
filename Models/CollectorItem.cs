using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
  
        public class CollectorItem
        {
            public int CollectorID { get; set; }
            public string Name { get; set; } = "";
            public override string ToString() => Name;
        }
    }

