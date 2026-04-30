using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{

        public class MainMeterReportResult
        {
            public int MainMeterID { get; set; }
            public DateTime ReportDate { get; set; }

            public decimal MainMeterPrev { get; set; }
            public decimal MainMeterCurr { get; set; }
            public decimal MainMeterDiff { get; set; }

            public decimal TotalSubMetersPrev { get; set; }
            public decimal TotalSubMetersCurr { get; set; }
            public decimal TotalSubMetersDiff { get; set; }

            public decimal WaterLoss { get; set; }
            public decimal WaterLossPercent { get; set; }

            public decimal TotalConsumptionAmount { get; set; }
            public decimal TotalServiceFees { get; set; }
            public decimal TotalDue { get; set; }
            public decimal Arrears { get; set; }
        }
    }