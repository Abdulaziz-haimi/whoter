using System;
using System.Collections.Generic;

namespace water3.Reports.Dynamic
{
    public class DynamicReportTemplateState
    {
        public int TemplateID { get; set; }
        public string TemplateName { get; set; }
        public string ReportKey { get; set; }

        public List<string> ColumnKeys { get; set; }

        public bool UseFromDate { get; set; }
        public bool UseToDate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int? SubscriberID { get; set; }
        public string SearchText { get; set; }
        public string CategoryType { get; set; }

        public DynamicReportTemplateState()
        {
            ColumnKeys = new List<string>();
        }
    }
}