using System;
using System.Data;
using System.Data.SqlClient;
using water3.Models;

namespace water3.Repositories
{
    public class ReadingsRepository
    {
        public decimal GetPreviousReading(int subscriberId, int meterId, DateTime asOfDate)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 CurrentReading
FROM Readings
WHERE SubscriberID = @SID
  AND MeterID = @MID
  AND ReadingDate < @AsOfDate
ORDER BY ReadingDate DESC, ReadingID DESC;", con))
            {
                cmd.Parameters.Add("@SID", SqlDbType.Int).Value = subscriberId;
                cmd.Parameters.Add("@MID", SqlDbType.Int).Value = meterId;
                cmd.Parameters.Add("@AsOfDate", SqlDbType.Date).Value = asOfDate.Date;

                con.Open();

                object result = cmd.ExecuteScalar();
                if (result == null || result == DBNull.Value)
                    return 0m;

                return Convert.ToDecimal(result);
            }
        }

        public MeterReadingInfo GetLastReadingInfo(int meterId)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 ReadingDate, PreviousReading, CurrentReading
FROM Readings
WHERE MeterID = @MID
ORDER BY ReadingDate DESC, ReadingID DESC;", con))
            {
                cmd.Parameters.Add("@MID", SqlDbType.Int).Value = meterId;
                con.Open();

                using (var dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        return new MeterReadingInfo();

                    return new MeterReadingInfo
                    {
                        ReadingDate = dr["ReadingDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["ReadingDate"]),
                        PreviousReading = dr["PreviousReading"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["PreviousReading"]),
                        CurrentReading = dr["CurrentReading"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["CurrentReading"])
                    };
                }
            }
        }

        public MeterInvoiceInfo GetLastInvoiceInfo(int meterId)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 InvoiceDate, TotalAmount, Status
FROM Invoices
WHERE MeterID = @MID
ORDER BY InvoiceDate DESC, InvoiceID DESC;", con))
            {
                cmd.Parameters.Add("@MID", SqlDbType.Int).Value = meterId;
                con.Open();

                using (var dr = cmd.ExecuteReader())
                {
                    if (!dr.Read())
                        return new MeterInvoiceInfo();

                    return new MeterInvoiceInfo
                    {
                        InvoiceDate = dr["InvoiceDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr["InvoiceDate"]),
                        TotalAmount = dr["TotalAmount"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["TotalAmount"]),
                        Status = dr["Status"]?.ToString() ?? ""
                    };
                }
            }
        }

        public void AddReadingAndGenerateInvoice(AddReadingRequest req)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("dbo.AddReadingAndGenerateInvoice", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@SubscriberID", SqlDbType.Int).Value = req.SubscriberID;
                cmd.Parameters.Add("@MeterID", SqlDbType.Int).Value = req.MeterID;
                cmd.Parameters.Add("@ReadingDate", SqlDbType.Date).Value = req.ReadingDate.Date;

                var pCur = cmd.Parameters.Add("@CurrentReading", SqlDbType.Decimal);
                pCur.Precision = 18;
                pCur.Scale = 2;
                pCur.Value = req.CurrentReading;

                var pUnit = cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal);
                pUnit.Precision = 18;
                pUnit.Scale = 2;
                pUnit.Value = req.UnitPrice;

                var pFees = cmd.Parameters.Add("@ServiceFees", SqlDbType.Decimal);
                pFees.Precision = 18;
                pFees.Scale = 2;
                pFees.Value = req.ServiceFees;

                cmd.Parameters.Add("@Notes", SqlDbType.NVarChar, 500).Value =
                    string.IsNullOrWhiteSpace(req.Notes)
                        ? (object)DBNull.Value
                        : req.Notes.Trim();

                con.Open();
                cmd.ExecuteNonQuery();
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
 
        public class ReadingsRepository
        {
            public decimal GetPreviousReading(int subscriberId, int meterId)
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 CurrentReading
FROM Readings
WHERE SubscriberID=@SID AND MeterID=@MID
ORDER BY ReadingDate DESC, ReadingID DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@SID", subscriberId);
                    cmd.Parameters.AddWithValue("@MID", meterId);
                    con.Open();

                    object result = cmd.ExecuteScalar();
                    if (result == null || result == DBNull.Value) return 0m;
                    return Convert.ToDecimal(result);
                }
            }

            public MeterReadingInfo GetLastReadingInfo(int meterId)
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 ReadingDate, PreviousReading, CurrentReading
FROM Readings
WHERE MeterID=@MID
ORDER BY ReadingDate DESC, ReadingID DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@MID", meterId);
                    con.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read()) return new MeterReadingInfo();

                        return new MeterReadingInfo
                        {
                            ReadingDate = Convert.ToDateTime(dr["ReadingDate"]),
                            PreviousReading = dr["PreviousReading"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["PreviousReading"]),
                            CurrentReading = dr["CurrentReading"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["CurrentReading"])
                        };
                    }
                }
            }

            public MeterInvoiceInfo GetLastInvoiceInfo(int meterId)
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 1 InvoiceDate, TotalAmount, Status
FROM Invoices
WHERE MeterID=@MID
ORDER BY InvoiceDate DESC, InvoiceID DESC;", con))
                {
                    cmd.Parameters.AddWithValue("@MID", meterId);
                    con.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (!dr.Read()) return new MeterInvoiceInfo();

                        return new MeterInvoiceInfo
                        {
                            InvoiceDate = Convert.ToDateTime(dr["InvoiceDate"]),
                            TotalAmount = dr["TotalAmount"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dr["TotalAmount"]),
                            Status = dr["Status"]?.ToString() ?? ""
                        };
                    }
                }
            }

            public void AddReadingAndGenerateInvoice(AddReadingRequest req)
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.AddReadingAndGenerateInvoice", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SubscriberID", SqlDbType.Int).Value = req.SubscriberID;
                    cmd.Parameters.Add("@MeterID", SqlDbType.Int).Value = req.MeterID;
                    cmd.Parameters.Add("@ReadingDate", SqlDbType.Date).Value = req.ReadingDate.Date;

                    var pCur = cmd.Parameters.Add("@CurrentReading", SqlDbType.Decimal);
                    pCur.Precision = 18; pCur.Scale = 3;
                    pCur.Value = req.CurrentReading;

                    var pUnit = cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal);
                    pUnit.Precision = 18; pUnit.Scale = 3;
                    pUnit.Value = req.UnitPrice;

                    var pFees = cmd.Parameters.Add("@ServiceFees", SqlDbType.Decimal);
                    pFees.Precision = 18; pFees.Scale = 3;
                    pFees.Value = req.ServiceFees;

                    cmd.Parameters.Add("@Notes", SqlDbType.NVarChar, 500).Value =
                        string.IsNullOrWhiteSpace(req.Notes) ? (object)DBNull.Value : req.Notes.Trim();

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }


*/