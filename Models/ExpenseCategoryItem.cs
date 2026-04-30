using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{

        public class ExpenseCategoryItem
        {
            public int CategoryID { get; set; }
            public string CategoryName { get; set; }
            public string CategoryType { get; set; }
            public int? DefaultAccountID { get; set; }
            public string Notes { get; set; }
            public bool IsActive { get; set; }

            public override string ToString()
            {
                return CategoryName;
            }
        }
    }
//    ExpenseLineItem.cs
//namespace water3.Models
//    {
        public class ExpenseLineItem
        {
            public string ItemName { get; set; }
            public decimal Qty { get; set; }
            public decimal UnitPrice { get; set; }
            public decimal LineTotal => Qty * UnitPrice;
            public int? TargetAccountID { get; set; }
            public string Notes { get; set; }
        }
    //}
//    ExpenseHeaderItem.cs
//using System;

//namespace water3.Models
//    {
        public class ExpenseHeaderItem
        {
            public int ExpenseID { get; set; }
            public string ExpenseNumber { get; set; }
            public DateTime ExpenseDate { get; set; }
            public int CategoryID { get; set; }
            public string CategoryName { get; set; }
            public string CategoryType { get; set; }
            public string SupplierName { get; set; }
            public string Description { get; set; }
            public string Notes { get; set; }
            public decimal TotalAmount { get; set; }
            public string PaymentMethod { get; set; }
            public int? CashAccountID { get; set; }
            public int? CounterAccountID { get; set; }
            public int? JournalID { get; set; }
            public bool IsPosted { get; set; }
            public string Status { get; set; }
        }
    //}