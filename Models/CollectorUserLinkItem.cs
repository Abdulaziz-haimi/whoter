using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{

        public class CollectorUserLinkItem
        {
            public int CollectorID { get; set; }
            public string CollectorName { get; set; }
            public string CollectorPhone { get; set; }

            public int? UserID { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string RoleName { get; set; }
            public bool IsUserActive { get; set; }
        }
    }