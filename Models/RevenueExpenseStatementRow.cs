using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
   
        public class RevenueExpenseStatementRow
        {
            public DateTime MovementDate { get; set; }
            public string RefNo { get; set; }
            public string MovementKind { get; set; }
            public string CategoryName { get; set; }
            public string PartyName { get; set; }
            public string Description { get; set; }
            public string PaymentMethod { get; set; }
            public decimal RevenueAmount { get; set; }
            public decimal ExpenseAmount { get; set; }
            public decimal Balance { get; set; }
            public string SourceType { get; set; }
            public int SourceId { get; set; }
        }
    }