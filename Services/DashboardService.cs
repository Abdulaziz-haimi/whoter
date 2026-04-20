using System;
using System.Data;
using System.Data.SqlClient;

namespace water3.Services
{
    public class DashboardService
    {
        private readonly string _cs = @"Data Source=.;Initial Catalog=WaterBillingDB;Integrated Security=True";

        public DashboardKpis GetKpis(DateTime from, DateTime to)
        {
            var k = new DashboardKpis();

            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
SET NOCOUNT ON;

DECLARE @FromDate DATE = @pFrom, @ToDate DATE = @pTo;

SELECT
    (SELECT COUNT(*) FROM Subscribers WHERE IsActive = 1) AS ActiveSubscribers,
    (SELECT COUNT(*) FROM Invoices WHERE InvoiceDate BETWEEN @FromDate AND @ToDate) AS InvoicesThisMonth,
    (SELECT ISNULL(SUM(TotalAmount),0) FROM Invoices WHERE InvoiceDate BETWEEN @FromDate AND @ToDate) AS BilledThisMonth,
    (SELECT ISNULL(SUM(Amount),0) FROM Payments WHERE PaymentDate BETWEEN @FromDate AND @ToDate) AS CollectedThisMonth,
    (SELECT ISNULL(SUM(Balance),0) FROM vw_SubscriberBalance) AS OutstandingTotal,
    (SELECT COUNT(*) FROM SmsLogs WHERE Status = 'Pending') AS PendingSms,
    dbo.fn_IsPeriodClosed(GETDATE()) AS IsCurrentPeriodClosed;
", con))
            {
                cmd.Parameters.AddWithValue("@pFrom", from.Date);
                cmd.Parameters.AddWithValue("@pTo", to.Date);

                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        k.ActiveSubscribers = r["ActiveSubscribers"] != DBNull.Value ? Convert.ToInt32(r["ActiveSubscribers"]) : 0;
                        k.InvoicesThisMonth = r["InvoicesThisMonth"] != DBNull.Value ? Convert.ToInt32(r["InvoicesThisMonth"]) : 0;
                        k.BilledThisMonth = r["BilledThisMonth"] != DBNull.Value ? Convert.ToDecimal(r["BilledThisMonth"]) : 0m;
                        k.CollectedThisMonth = r["CollectedThisMonth"] != DBNull.Value ? Convert.ToDecimal(r["CollectedThisMonth"]) : 0m;
                        k.OutstandingTotal = r["OutstandingTotal"] != DBNull.Value ? Convert.ToDecimal(r["OutstandingTotal"]) : 0m;
                        k.PendingSms = r["PendingSms"] != DBNull.Value ? Convert.ToInt32(r["PendingSms"]) : 0;
                        k.IsCurrentPeriodClosed = r["IsCurrentPeriodClosed"] != DBNull.Value && Convert.ToBoolean(r["IsCurrentPeriodClosed"]);
                    }
                }
            }

            return k;
        }

        public SmsStatistics GetSmsStatistics(DateTime from, DateTime to)
        {
            var stats = new SmsStatistics();

            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    (SELECT COUNT(*) FROM SmsLogs WHERE CreatedAt BETWEEN @FromDate AND @ToDate) AS Total,
    (SELECT COUNT(*) FROM SmsLogs WHERE Status = 'Sent' AND CreatedAt BETWEEN @FromDate AND @ToDate) AS Sent,
    (SELECT COUNT(*) FROM SmsLogs WHERE Status = 'Failed' AND CreatedAt BETWEEN @FromDate AND @ToDate) AS Failed,
    (SELECT COUNT(*) FROM SmsLogs WHERE Status = 'Pending') AS Pending,
    (SELECT COUNT(*) FROM SmsLogs WHERE Status = 'Sent' AND CAST(CreatedAt AS DATE) = CAST(GETDATE() AS DATE)) AS SentToday;
", con))
            {
                cmd.Parameters.AddWithValue("@FromDate", from);
                cmd.Parameters.AddWithValue("@ToDate", to);

                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        stats.Total = r["Total"] != DBNull.Value ? Convert.ToInt32(r["Total"]) : 0;
                        stats.Sent = r["Sent"] != DBNull.Value ? Convert.ToInt32(r["Sent"]) : 0;
                        stats.Failed = r["Failed"] != DBNull.Value ? Convert.ToInt32(r["Failed"]) : 0;
                        stats.Pending = r["Pending"] != DBNull.Value ? Convert.ToInt32(r["Pending"]) : 0;
                        stats.SentToday = r["SentToday"] != DBNull.Value ? Convert.ToInt32(r["SentToday"]) : 0;
                    }
                }
            }

            return stats;
        }

        public DataTable GetLastInvoices(int top = 20)
            => Fill(@"
SELECT TOP (@top)
    I.InvoiceID AS 'رقم الفاتورة',
    FORMAT(I.InvoiceDate,'yyyy/MM/dd') AS 'تاريخ الفاتورة',
    S.Name AS 'اسم المشترك',
    I.TotalAmount AS 'المبلغ الإجمالي',
    I.Status AS 'الحالة'
FROM Invoices I
JOIN Subscribers S ON S.SubscriberID = I.SubscriberID
ORDER BY I.InvoiceDate DESC, I.InvoiceID DESC;", CommandType.Text, ("@top", top));

        public DataTable GetLastPayments(int top = 20)
            => Fill(@"
SELECT TOP (@top)
    P.PaymentID AS 'رقم السداد',
    FORMAT(P.PaymentDate,'yyyy/MM/dd') AS 'تاريخ السداد',
    S.Name AS 'اسم المشترك',
    P.Amount AS 'المبلغ',
    ISNULL(C.Name, N'غير محدد') AS 'المحصل',
    P.PaymentType AS 'طريقة الدفع'
FROM Payments P
JOIN Subscribers S ON S.SubscriberID = P.SubscriberID
LEFT JOIN Collectors C ON C.CollectorID = P.CollectorID
ORDER BY P.PaymentDate DESC, P.PaymentID DESC;", CommandType.Text, ("@top", top));

        public DataTable GetTopOutstanding(int top = 20)
            => Fill(@"
SELECT TOP (@top)
    S.SubscriberID AS 'كود المشترك',
    S.Name AS 'اسم المشترك',
    B.Balance AS 'الرصيد'
FROM vw_SubscriberBalance B
JOIN Subscribers S ON S.SubscriberID = B.SubscriberID
ORDER BY B.Balance DESC;", CommandType.Text, ("@top", top));

        public DataTable GetSmsReport(DateTime from, DateTime to)
            => Fill(@"
SELECT 
    SL.SmsID AS 'رقم الرسالة',
    S.Name AS 'اسم المشترك',
    SL.PhoneNumber AS 'رقم الهاتف',
    LEFT(SL.Message, 100) AS 'معاينة الرسالة',
    SL.Status AS 'الحالة',
    SL.SentDate AS 'تاريخ الإرسال'
FROM SmsLogs SL
LEFT JOIN Subscribers S ON SL.SubscriberID = S.SubscriberID
WHERE SL.CreatedAt BETWEEN @FromDate AND @ToDate
ORDER BY SL.CreatedAt DESC;", CommandType.Text, ("@FromDate", from), ("@ToDate", to));

        public DataTable GetAgingReceivables(DateTime asOfDate)
            => Fill("rpt_AgingReceivables", CommandType.StoredProcedure, ("@AsOfDate", asOfDate));

        private DataTable Fill(string sql, CommandType type, params (string name, object value)[] prms)
        {
            var dt = new DataTable();
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                cmd.CommandType = type;
                cmd.CommandTimeout = 30;

                foreach (var p in prms)
                    cmd.Parameters.AddWithValue(p.name, p.value ?? DBNull.Value);

                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }
            return dt;
        }
    }

    public class DashboardKpis
    {
        public int ActiveSubscribers { get; set; }
        public int InvoicesThisMonth { get; set; }
        public decimal BilledThisMonth { get; set; }
        public decimal CollectedThisMonth { get; set; }
        public decimal OutstandingTotal { get; set; }
        public int PendingSms { get; set; }
        public bool IsCurrentPeriodClosed { get; set; }
    }

    public class SmsStatistics
    {
        public int Total { get; set; }
        public int Sent { get; set; }
        public int Failed { get; set; }
        public int Pending { get; set; }
        public int SentToday { get; set; }
    }
}
