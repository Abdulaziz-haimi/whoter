using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using water3.Models;

namespace water3.Repositories
{
    public class SubscribersRepository
    {
        public List<SubscriberSearchItem> Search(string searchKey)
        {
            var list = new List<SubscriberSearchItem>();

            searchKey = (searchKey ?? "").Trim();
            if (searchKey.Length == 0)
                return list;

            string like = "%" + searchKey + "%";

            const string sql = @"
SELECT TOP (30)
    s.SubscriberID,
    s.Name,
    ISNULL(m.MeterNumber, N'') AS MeterNumber,
    ISNULL(s.PhoneNumber, N'') AS PhoneNumber
FROM dbo.Subscribers s
LEFT JOIN dbo.SubscriberMeters sm
    ON sm.SubscriberID = s.SubscriberID
   AND sm.IsPrimary = 1
LEFT JOIN dbo.Meters m
    ON m.MeterID = sm.MeterID
WHERE s.IsActive = 1
  AND
  (
      s.Name LIKE @k
      OR ISNULL(m.MeterNumber, N'') LIKE @k
      OR ISNULL(s.PhoneNumber, N'') LIKE @k
  )
ORDER BY s.Name;";

            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.Add("@k", SqlDbType.NVarChar, 200).Value = like;

                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new SubscriberSearchItem
                        {
                            SubscriberID = Convert.ToInt32(rd["SubscriberID"]),
                            Name = (rd["Name"]?.ToString() ?? "").Trim(),
                            MeterNumber = (rd["MeterNumber"]?.ToString() ?? "").Trim(),
                            PhoneNumber = (rd["PhoneNumber"]?.ToString() ?? "").Trim()
                        });
                    }
                }
            }

            return list;
        }

        public decimal GetBalance(int subscriberId)
        {
            return GetBalanceAsOf(subscriberId, DateTime.Today);
        }

        public decimal GetBalanceAsOf(int subscriberId, DateTime asOfDate)
        {
            const string sql = @"
DECLARE
    @InvoicesTotal DECIMAL(18,2) = 0,
    @ReceiptsTotal DECIMAL(18,2) = 0;

-- إجمالي الفواتير حتى التاريخ
SELECT
    @InvoicesTotal = ISNULL(SUM(CAST(I.TotalAmount AS DECIMAL(18,2))), 0)
FROM dbo.Invoices I
WHERE I.SubscriberID = @SID
  AND I.InvoiceDate <= @D
  AND ISNULL(I.Status, N'') <> N'ملغاة';

-- إجمالي الإيصالات الفعلية حتى التاريخ
SELECT
    @ReceiptsTotal = ISNULL(SUM(CAST(R.TotalAmount AS DECIMAL(18,2))), 0)
FROM dbo.Receipts R
WHERE R.SubscriberID = @SID
  AND R.PaymentDate <= @D;

SELECT CAST(ROUND(@InvoicesTotal - @ReceiptsTotal, 2) AS DECIMAL(18,2));";

            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.Add("@SID", SqlDbType.Int).Value = subscriberId;
                cmd.Parameters.Add("@D", SqlDbType.Date).Value = asOfDate.Date;

                con.Open();

                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                    return 0m;

                return Convert.ToDecimal(result);
            }
        }
    }
}