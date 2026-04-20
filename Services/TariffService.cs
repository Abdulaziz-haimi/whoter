using System;
using water3.Repositories;
using water3.Models;

namespace water3.Services
{
    public class TariffService
    {
        private readonly TariffRepository _repo;

        public TariffService(TariffRepository repo)
        {
            _repo = repo;
        }

        public TariffInfo LoadDefaultTariffSafe(DateTime asOfDate)
        {
            try
            {
                var t = _repo.GetDefaultTariff(asOfDate);

                if (t == null)
                {
                    return new TariffInfo
                    {
                        PricingModel = "DEFAULT",
                        TariffPlanID = null,
                        UnitPrice = 0,
                        ServiceFees = 0
                    };
                }

                if (t.UnitPrice <= 0 && t.ServiceFees <= 0)
                {
                    return new TariffInfo
                    {
                        PricingModel = "DEFAULT",
                        TariffPlanID = null,
                        UnitPrice = 0,
                        ServiceFees = 0
                    };
                }

                return t;
            }
            catch
            {
                return new TariffInfo
                {
                    PricingModel = "DEFAULT",
                    TariffPlanID = null,
                    UnitPrice = 0,
                    ServiceFees = 0
                };
            }
        }

        public TariffInfo LoadTariffForSubscriberOrDefault(int subscriberId, DateTime asOfDate)
        {
            try
            {
                var t = _repo.GetTariffForSubscriber(subscriberId, asOfDate);

                if (t == null)
                    return LoadDefaultTariffSafe(asOfDate);

                return t;
            }
            catch
            {
                return LoadDefaultTariffSafe(asOfDate);
            }
        }

        public decimal ResolveUnitPrice(TariffInfo tariff, decimal consumption)
        {
            if (tariff == null)
                return 0m;

            if (consumption < 0)
                return 0m;

            if (tariff.TariffPlanID.HasValue &&
                string.Equals(tariff.PricingModel, "Tiered", StringComparison.OrdinalIgnoreCase))
            {
                return _repo.ResolveTieredUnitPrice(
                    tariff.TariffPlanID.Value,
                    consumption,
                    tariff.UnitPrice);
            }

            return tariff.UnitPrice;
        }
    }
}
/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using water3.Repositories;
using water3.Models;
namespace water3.Services
{
    

   
        public class TariffService
        {
            private readonly TariffRepository _repo;

            public TariffService(TariffRepository repo)
            {
                _repo = repo;
            }

            public TariffInfo LoadDefaultTariffSafe()
            {
                try
                {
                    var t = _repo.GetDefaultTariff();
                    if (t.UnitPrice <= 0 && t.ServiceFees <= 0)
                        return new TariffInfo { PricingModel = "DEFAULT", TariffPlanID = null, UnitPrice = 0, ServiceFees = 0 };
                    return t;
                }
                catch
                {
                    return new TariffInfo { PricingModel = "DEFAULT", TariffPlanID = null, UnitPrice = 0, ServiceFees = 0 };
                }
            }

            public TariffInfo LoadTariffForSubscriberOrDefault(int subscriberId, DateTime asOfDate)
            {
                try
                {
                    var t = _repo.GetTariffForSubscriber(subscriberId, asOfDate);
                    if (t == null) return LoadDefaultTariffSafe();
                    return t;
                }
                catch
                {
                    return LoadDefaultTariffSafe();
                }
            }

            public decimal ResolveUnitPrice(TariffInfo tariff, decimal consumption)
            {
                if (tariff == null) return 0m;

                if (tariff.TariffPlanID.HasValue &&
                    string.Equals(tariff.PricingModel, "Tiered", StringComparison.OrdinalIgnoreCase))
                {
                    return _repo.ResolveTieredUnitPrice(tariff.TariffPlanID.Value, consumption, tariff.UnitPrice);
                }

                return tariff.UnitPrice;
            }
        }
    }
*/
