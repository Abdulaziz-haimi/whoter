using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using water3.Models;
namespace water3.Repositories
{

        public class MainMeterReportRepository
        {
            public List<MainMeterLookupItem> GetMainMeters()
            {
                var list = new List<MainMeterLookupItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT MeterID, MeterNumber, Location
                FROM dbo.Meters
                WHERE MeterType = N'Main'
                  AND ISNULL(IsActive, 1) = 1
                ORDER BY MeterNumber", con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new MainMeterLookupItem
                            {
                                MeterID = Convert.ToInt32(dr["MeterID"]),
                                MeterNumber = Convert.ToString(dr["MeterNumber"]),
                                Location = dr["Location"] == DBNull.Value ? null : Convert.ToString(dr["Location"])
                            });
                        }
                    }
                }

                return list;
            }

            public decimal GetLastMainMeterReading(int mainMeterId)
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT TOP (1) CurrentReading
                FROM dbo.MainMeterReadings
                WHERE MeterID = @MeterID
                ORDER BY ReadingDate DESC, MainReadingID DESC", con))
                {
                    cmd.Parameters.AddWithValue("@MeterID", mainMeterId);
                    con.Open();

                    object result = cmd.ExecuteScalar();
                    return result == null || result == DBNull.Value ? 0m : Convert.ToDecimal(result);
                }
            }

            public MainMeterReportResult SaveMainMeterReadingAndGenerateReport(
                int mainMeterId,
                DateTime reportDate,
                decimal currentReading,
                string notes,
                decimal? totalFromQasem)
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.usp_SaveMainMeterReadingAndGenerateReport", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MainMeterID", mainMeterId);
                    cmd.Parameters.AddWithValue("@ReportDate", reportDate.Date);
                    cmd.Parameters.AddWithValue("@CurrentReading", currentReading);
                    cmd.Parameters.AddWithValue("@Notes", string.IsNullOrWhiteSpace(notes) ? (object)DBNull.Value : notes.Trim());
                    cmd.Parameters.AddWithValue("@TotalFromQasem", totalFromQasem.HasValue ? (object)totalFromQasem.Value : DBNull.Value);

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new MainMeterReportResult
                            {
                                MainMeterID = Convert.ToInt32(dr["MainMeterID"]),
                                ReportDate = Convert.ToDateTime(dr["ReportDate"]),
                                MainMeterPrev = Convert.ToDecimal(dr["MainMeterPrev"]),
                                MainMeterCurr = Convert.ToDecimal(dr["MainMeterCurr"]),
                                MainMeterDiff = Convert.ToDecimal(dr["MainMeterDiff"]),
                                TotalSubMetersPrev = Convert.ToDecimal(dr["TotalSubMetersPrev"]),
                                TotalSubMetersCurr = Convert.ToDecimal(dr["TotalSubMetersCurr"]),
                                TotalSubMetersDiff = Convert.ToDecimal(dr["TotalSubMetersDiff"]),
                                WaterLoss = Convert.ToDecimal(dr["WaterLoss"]),
                                WaterLossPercent = Convert.ToDecimal(dr["WaterLossPercent"]),
                                TotalConsumptionAmount = Convert.ToDecimal(dr["TotalConsumptionAmount"]),
                                TotalServiceFees = Convert.ToDecimal(dr["TotalServiceFees"]),
                                TotalDue = Convert.ToDecimal(dr["TotalDue"]),
                                Arrears = Convert.ToDecimal(dr["Arrears"])
                            };
                        }
                    }
                }

                return null;
            }

            public List<SubMeterReadingSummaryItem> GetSubMeterReadingsByDate(DateTime reportDate)
            {
                var list = new List<SubMeterReadingSummaryItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT
                    R.SubscriberID,
                    S.Name AS SubscriberName,
                    R.MeterID,
                    M.MeterNumber,
                    R.PreviousReading,
                    R.CurrentReading,
                    R.Consumption
                FROM dbo.Readings R
                INNER JOIN dbo.Subscribers S ON R.SubscriberID = S.SubscriberID
                INNER JOIN dbo.Meters M ON R.MeterID = M.MeterID
                WHERE R.ReadingDate = @ReportDate
                  AND ISNULL(M.MeterType, N'Sub') = N'Sub'
                ORDER BY S.Name", con))
                {
                    cmd.Parameters.AddWithValue("@ReportDate", reportDate.Date);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new SubMeterReadingSummaryItem
                            {
                                SubscriberID = Convert.ToInt32(dr["SubscriberID"]),
                                SubscriberName = Convert.ToString(dr["SubscriberName"]),
                                MeterID = Convert.ToInt32(dr["MeterID"]),
                                MeterNumber = Convert.ToString(dr["MeterNumber"]),
                                PreviousReading = Convert.ToDecimal(dr["PreviousReading"]),
                                CurrentReading = Convert.ToDecimal(dr["CurrentReading"]),
                                Consumption = Convert.ToDecimal(dr["Consumption"])
                            });
                        }
                    }
                }

                return list;
            }
        }
    }