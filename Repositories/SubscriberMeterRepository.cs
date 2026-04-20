using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using water3.Models;
namespace water3.Repositories
{
   

        public class SubscriberMeterRepository
        {
            public List<SubscriberMeterSuggestion> SearchSubscribersAndMeters(string keyword)
            {
                var list = new List<SubscriberMeterSuggestion>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 30
    s.SubscriberID,
    m.MeterID,
    (CAST(s.SubscriberID AS NVARCHAR(20)) + ' - ' + s.Name
     + ' | عداد: ' + m.MeterNumber
     + CASE WHEN ISNULL(m.Location,'')<>'' THEN ' | موقع: ' + m.Location ELSE '' END
    ) AS DisplayText
FROM Subscribers s
JOIN SubscriberMeters sm ON sm.SubscriberID = s.SubscriberID
JOIN Meters m ON m.MeterID = sm.MeterID
WHERE s.IsActive = 1 AND m.IsActive = 1
  AND (
        s.Name LIKE @K
     OR m.MeterNumber LIKE @K
     OR CAST(s.SubscriberID AS NVARCHAR(20)) = @Exact
     OR CAST(m.MeterID AS NVARCHAR(20)) = @Exact
  )
ORDER BY s.Name, m.MeterNumber;", con))
                {
                    cmd.Parameters.AddWithValue("@K", "%" + keyword + "%");
                    cmd.Parameters.AddWithValue("@Exact", keyword);

                    con.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new SubscriberMeterSuggestion
                            {
                                SubscriberID = Convert.ToInt32(dr["SubscriberID"]),
                                MeterID = Convert.ToInt32(dr["MeterID"]),
                                DisplayText = dr["DisplayText"]?.ToString() ?? ""
                            });
                        }
                    }
                }

                return list;
            }
        }
    }

