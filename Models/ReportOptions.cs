
using System;
using System.Collections.Generic;

namespace water3.Models
{
    public class ReportOptions
    {
        public int SubscriberId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public List<string> SelectedColumns { get; set; } = new List<string>();
        public string SortBy { get; set; } = "التاريخ";
        public bool SortDesc { get; set; } = false;

        public bool OnlyInvoices { get; set; } = false;
        public bool OnlyPayments { get; set; } = false;

        public string GroupBy { get; set; } = "بدون"; // بدون/شهري/نوع المستند/البند
    }
}
