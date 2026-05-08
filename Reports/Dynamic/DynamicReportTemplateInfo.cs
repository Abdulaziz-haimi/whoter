using System;

namespace water3.Reports.Dynamic
{
    public class DynamicReportTemplateInfo
    {
        public int TemplateID { get; set; }
        public string TemplateName { get; set; }
        public string ReportKey { get; set; }
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            return TemplateName;
        }
    }
}