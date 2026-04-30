using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
 
        public class AccountLookupItem
        {
            public int AccountID { get; set; }
            public string AccountCode { get; set; }
            public string AccountName { get; set; }

            public override string ToString()
            {
                return string.IsNullOrWhiteSpace(AccountCode)
                    ? AccountName
                    : AccountCode + " - " + AccountName;
            }
        }
    }