/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{
   
        public class RevenueExpenseStatementFilter
        {
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }

            public string MovementType { get; set; }     // All, Revenue, Out, Expense, Purchase, Loss
            public string PaymentMethod { get; set; }    // All, Cash, Transfer, Cheque, Credit, Other

            public string SearchText { get; set; }

            public decimal? MinAmount { get; set; }
            public decimal? MaxAmount { get; set; }

            public RevenueExpenseStatementFilter()
            {
                MovementType = "All";
                PaymentMethod = "All";
            }
        }
}*/
using System;

namespace water3.Models
{
    public class RevenueExpenseStatementFilter
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        // null / empty = الكل
        // Revenue = الإيرادات فقط
        // Outflow = كل المنصرفات
        // Expense / Purchase / Loss = حسب نوع تصنيف المنصرف
        public string MovementType { get; set; }

        public int? CategoryId { get; set; }

        // Cash / Transfer / Cheque / Credit / Other
        public string PaymentMethod { get; set; }

        // بحث عام: رقم مرجع، اسم مشترك/جهة، بيان، تصنيف
        public string SearchText { get; set; }
    }
}
