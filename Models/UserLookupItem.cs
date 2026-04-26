using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{

        public class UserLookupItem
        {
            public int UserID { get; set; }
            public string UserName { get; set; }
            public string FullName { get; set; }
            public string RoleName { get; set; }
            public bool IsActive { get; set; }

            public override string ToString()
            {
                return string.IsNullOrWhiteSpace(FullName)
                    ? UserName
                    : FullName + " (" + UserName + ")";
            }
        }
    }