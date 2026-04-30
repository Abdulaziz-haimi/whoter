using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{

        public class ExpenseVoucherHeaderRow
        {
            public int ExpenseID { get; set; }
            public string ExpenseNumber { get; set; }
            public DateTime ExpenseDate { get; set; }
            public string CategoryName { get; set; }
            public string CategoryType { get; set; }
            public string SupplierName { get; set; }
            public string Description { get; set; }
            public string Notes { get; set; }
            public decimal TotalAmount { get; set; }
            public string PaymentMethod { get; set; }
            public bool IsPosted { get; set; }
            public string Status { get; set; }
            public string CreatedByName { get; set; }
        }
    }
