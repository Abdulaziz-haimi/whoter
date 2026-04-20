using System;
using System.Data;
using System.Data.SqlClient;
using water3.Models;

namespace water3.Repositories
{
    public class TariffRepository
    {
        public TariffInfo GetDefaultTariff(DateTime asOfDate)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("dbo.GetActiveBillingConstant", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@AsOfDate", SqlDbType.Date).Value = asOfDate.Date;

                con.Open();

                using (var dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                    {
                        return new TariffInfo
                        {
                            PricingModel = "DEFAULT",
                            TariffPlanID = null,
                            UnitPrice = 0,
                            ServiceFees = 0
                        };
                    }

                    return new TariffInfo
                    {
                        PricingModel = "DEFAULT",
                        TariffPlanID = null,
                        UnitPrice = dr["UnitPrice"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["UnitPrice"]),
                        ServiceFees = dr["ServiceFees"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["ServiceFees"])
                    };
                }
            }
        }

        public TariffInfo GetTariffForSubscriber(int subscriberId, DateTime asOfDate)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("dbo.GetTariffForSubscriber", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@SubscriberID", SqlDbType.Int).Value = subscriberId;
                cmd.Parameters.Add("@AsOfDate", SqlDbType.Date).Value = asOfDate.Date;

                con.Open();

                using (var dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        return null;

                    return new TariffInfo
                    {
                        TariffPlanID = dr["TariffPlanID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["TariffPlanID"]),
                        PricingModel = dr["PricingModel"] == DBNull.Value ? "DEFAULT" : dr["PricingModel"].ToString(),
                        UnitPrice = dr["UnitPrice"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["UnitPrice"]),
                        ServiceFees = dr["ServiceFees"] == DBNull.Value ? 0m : Convert.ToDecimal(dr["ServiceFees"])
                    };
                }
            }
        }

        public decimal ResolveTieredUnitPrice(int tariffPlanId, decimal consumption, decimal fallbackUnitPrice)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 UnitPrice
FROM TariffRates
WHERE TariffPlanID = @Plan
  AND @C >= FromQty
  AND (ToQty IS NULL OR @C <= ToQty)
ORDER BY FromQty DESC, TariffRateID DESC;", con))
            {
                cmd.Parameters.Add("@Plan", SqlDbType.Int).Value = tariffPlanId;

                var pC = cmd.Parameters.Add("@C", SqlDbType.Decimal);
                pC.Precision = 18;
                pC.Scale = 2;
                pC.Value = consumption;

                con.Open();

                object o = cmd.ExecuteScalar();
                if (o == null || o == DBNull.Value)
                    return fallbackUnitPrice;

                decimal price = Convert.ToDecimal(o);
                return price > 0 ? price : fallbackUnitPrice;
            }
        }
    }
}
/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using water3.Models;

namespace water3.Repositories
{
 
        public class TariffRepository
        {
            public TariffInfo GetDefaultTariff()
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.GetActiveBillingConstant", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read())
                            return new TariffInfo { PricingModel = "DEFAULT", TariffPlanID = null, UnitPrice = 0, ServiceFees = 0 };

                        return new TariffInfo
                        {
                            PricingModel = "DEFAULT",
                            TariffPlanID = null,
                            UnitPrice = Convert.ToDecimal(dr["UnitPrice"]),
                            ServiceFees = Convert.ToDecimal(dr["ServiceFees"])
                        };
                    }
                }
            }

            public TariffInfo GetTariffForSubscriber(int subscriberId, DateTime asOfDate)
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.GetTariffForSubscriber", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SubscriberID", subscriberId);
                    cmd.Parameters.AddWithValue("@AsOfDate", asOfDate.Date);

                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read()) return null;

                        return new TariffInfo
                        {
                            TariffPlanID = dr["TariffPlanID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["TariffPlanID"]),
                            PricingModel = dr["PricingModel"]?.ToString() ?? "DEFAULT",
                            UnitPrice = Convert.ToDecimal(dr["UnitPrice"]),
                            ServiceFees = Convert.ToDecimal(dr["ServiceFees"])
                        };
                    }
                }
            }

            public decimal ResolveTieredUnitPrice(int tariffPlanId, decimal consumption, decimal fallbackUnitPrice)
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 UnitPrice
FROM TariffRates
WHERE TariffPlanID = @Plan
  AND @C >= FromQty
  AND (ToQty IS NULL OR @C <= ToQty)
ORDER BY FromQty DESC, TariffRateID DESC;", con))
                {
                    cmd.Parameters.Add("@Plan", SqlDbType.Int).Value = tariffPlanId;

                    var pC = cmd.Parameters.Add("@C", SqlDbType.Decimal);
                    pC.Precision = 18; pC.Scale = 3;
                    pC.Value = consumption;

                    con.Open();
                    object o = cmd.ExecuteScalar();
                    if (o == null || o == DBNull.Value) return fallbackUnitPrice;
                    return Convert.ToDecimal(o);
                }
            }
        }
    }


*/