using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{

        public class ExpenseSaveResult
        {
            public int ExpenseID { get; set; }
            public string ExpenseNumber { get; set; }
            public decimal TotalAmount { get; set; }
        }
    }