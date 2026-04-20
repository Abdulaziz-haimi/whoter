using System.Collections.Generic;

namespace water3.Reports
{
    public class ReportPresetOptions
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string FooterNote { get; set; }
        public string LogoPath { get; set; }   // مثال: C:\WaterApp\logo.png
        public bool ShowTotals { get; set; }

        public List<ColumnOption> Columns { get; set; }

        public ReportPresetOptions()
        {
            Title = "تقرير";
            SubTitle = "";
            FooterNote = "";
            LogoPath = "";
            ShowTotals = true;
            Columns = new List<ColumnOption>();
        }

        public class ColumnOption
        {
            public string Key { get; set; }      // Col1..Col10 أو Num1..Num5 (من المصدر)
            public string Caption { get; set; }
            public bool Visible { get; set; }
            public int Order { get; set; }

            public ColumnOption()
            {
                Key = "";
                Caption = "";
                Visible = true;
                Order = 0;
            }
        }
    }
}
