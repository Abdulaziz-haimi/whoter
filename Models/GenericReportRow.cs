using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{

        public class GenericReportRow
        {
            public DateTime? Dt1 { get; set; }

            public string Col1 { get; set; }
            public string Col2 { get; set; }
            public string Col3 { get; set; }
            public string Col4 { get; set; }
            public string Col5 { get; set; }

            public decimal? Num1 { get; set; }
            public decimal? Num2 { get; set; }
            public decimal? Num3 { get; set; }
            public decimal? Num4 { get; set; }
            public decimal? Num5 { get; set; }

            public int? RowId { get; set; }
            public int? RefId1 { get; set; }
            public int? RefId2 { get; set; }
        }
    }