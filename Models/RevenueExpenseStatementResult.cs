using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
  
        public class RevenueExpenseStatementResult
        {
            public decimal OpeningBalance { get; set; }
            public decimal TotalRevenue { get; set; }
            public decimal TotalExpense { get; set; }
            public decimal RemainingBalance { get; set; }

            public List<RevenueExpenseStatementRow> Rows { get; set; }

            public RevenueExpenseStatementResult()
            {
                Rows = new List<RevenueExpenseStatementRow>();
            }

            public void Recalculate()
            {
                TotalRevenue = Rows.Sum(x => x.RevenueAmount);
                TotalExpense = Rows.Sum(x => x.ExpenseAmount);
                RemainingBalance = OpeningBalance + TotalRevenue - TotalExpense;
            }
        }
    }