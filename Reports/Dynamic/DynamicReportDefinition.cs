using System.Collections.Generic;

namespace water3.Reports.Dynamic
{
    public class DynamicReportDefinition
    {
        public string ReportKey { get; set; }
        public string Title { get; set; }

        public string BaseSql { get; set; }
        public string OrderBySql { get; set; }

        public string DateExpression { get; set; }
        public string SubscriberIdExpression { get; set; }
        public string CategoryTypeExpression { get; set; }

        public bool HasDateFilter
        {
            get { return !string.IsNullOrWhiteSpace(DateExpression); }
        }

        public bool HasSubscriberFilter
        {
            get { return !string.IsNullOrWhiteSpace(SubscriberIdExpression); }
        }

        public bool HasCategoryTypeFilter
        {
            get { return !string.IsNullOrWhiteSpace(CategoryTypeExpression); }
        }

        public List<string> SearchExpressions { get; set; }
        public List<DynamicReportColumn> Columns { get; set; }

        public DynamicReportDefinition()
        {
            SearchExpressions = new List<string>();
            Columns = new List<DynamicReportColumn>();
        }

        public override string ToString()
        {
            return Title;
        }
    }
}