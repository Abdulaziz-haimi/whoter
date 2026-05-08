namespace water3.Reports.Dynamic
{
    public class DynamicReportColumn
    {
        public string ColumnKey { get; set; }
        public string ColumnTitle { get; set; }
        public string SqlExpression { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsDefaultVisible { get; set; }

        public override string ToString()
        {
            return ColumnTitle;
        }
    }
}