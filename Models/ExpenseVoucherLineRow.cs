using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
 
        public class ExpenseVoucherLineRow
        {
            public int ExpenseLineID { get; set; }
            public string ItemName { get; set; }
            public decimal Qty { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal LineTotal { get; set; }
            public string TargetAccountName { get; set; }
            public string Notes { get; set; }
        }
    }