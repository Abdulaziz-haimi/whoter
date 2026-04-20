using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Models
{

   
        public class TariffInfo
        {
            public int? TariffPlanID { get; set; }
            public string PricingModel { get; set; } = "DEFAULT"; // DEFAULT / Fixed / Tiered
            public decimal UnitPrice { get; set; }
            public decimal ServiceFees { get; set; }

            public string ToLabelText()
            {
                string planText = TariffPlanID.HasValue ? $"خطة:{PricingModel}" : "عامة";
                return $"التعرفة: ({planText}) سعر {UnitPrice:N2} + رسوم {ServiceFees:N2}";
            }
        }
    }
