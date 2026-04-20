using System;
using System.Data;
using System.Data.SqlClient;

namespace water3.Repositories
{
    public class SmsLogsRepository
    {
        public DataTable GetPendingInvoiceMessages(DateTime from, DateTime toExclusive, int subscriberId, string searchLike, string invoiceStatus = null)
        {
            var dt = new DataTable();

            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(BuildPendingInvoiceMessagesQuery(), con))
            {
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = from.Date;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toExclusive;
                cmd.Parameters.Add("@SubscriberID", SqlDbType.Int).Value = subscriberId;
                cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 200).Value = (object)searchLike ?? DBNull.Value;
                cmd.Parameters.Add("@InvoiceStatus", SqlDbType.NVarChar, 30).Value = (object)invoiceStatus ?? DBNull.Value;

                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }

            return dt;
        }

        public DataTable GetPendingPaymentMessages(DateTime from, DateTime toExclusive, int subscriberId, string searchLike, string paymentMethodDb = null)
        {
            var dt = new DataTable();

            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(BuildPendingPaymentMessagesQuery(), con))
            {
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = from.Date;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toExclusive;
                cmd.Parameters.Add("@SubscriberID", SqlDbType.Int).Value = subscriberId;
                cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 200).Value = (object)searchLike ?? DBNull.Value;
                cmd.Parameters.Add("@PaymentMethod", SqlDbType.NVarChar, 30).Value = (object)paymentMethodDb ?? DBNull.Value;

                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }

            return dt;
        }

        public DataTable GetLogs(DateTime from, DateTime toExclusive, int subscriberId, string searchLike)
        {
            var dt = new DataTable();

            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(BuildSmsLogsQuery(), con))
            {
                cmd.Parameters.Add("@FromDate", SqlDbType.DateTime).Value = from.Date;
                cmd.Parameters.Add("@ToDate", SqlDbType.DateTime).Value = toExclusive;
                cmd.Parameters.Add("@SubscriberID", SqlDbType.Int).Value = subscriberId;
                cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 200).Value = (object)searchLike ?? DBNull.Value;

                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }

            return dt;
        }

        public DataTable GetPendingOrFailed(int maxRetry = 3)
        {
            var dt = new DataTable();

            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
SELECT TOP (500)
    SmsID,
    InvoiceID,
    PaymentID,
    ReceiptID,
    SubscriberID,
    PhoneNumber,
    Message,
    Status,
    Reason,
    SentDate,
    CollectorID,
    TemplateID,
    RetryCount,
    CreatedAt,
    MessageType
FROM dbo.SmsLogs
WHERE (Status = N'Pending' OR Status = N'Failed')
  AND ISNULL(RetryCount, 0) < @MaxRetry
ORDER BY CreatedAt ASC;", con))
            {
                cmd.Parameters.Add("@MaxRetry", SqlDbType.Int).Value = maxRetry;

                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }

            return dt;
        }

        public void MarkSent(int smsId)
        {
            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
UPDATE dbo.SmsLogs
SET Status = N'Sent',
    Reason = NULL,
    SentDate = GETDATE()
WHERE SmsID = @ID;", con))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = smsId;
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void MarkFailed(int smsId, string reason)
        {
            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
UPDATE dbo.SmsLogs
SET Status = N'Failed',
    Reason = @R,
    RetryCount = ISNULL(RetryCount, 0) + 1
WHERE SmsID = @ID;", con))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = smsId;
                cmd.Parameters.Add("@R", SqlDbType.NVarChar, 200).Value =
                    string.IsNullOrWhiteSpace(reason) ? (object)DBNull.Value : reason.Trim();

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private string BuildPendingInvoiceMessagesQuery() => @"
SELECT
    L.SmsID,
    L.TemplateID,
    L.InvoiceID AS [رقم الفاتورة],
    S.SubscriberID AS SubscriberID,
    S.Name AS [اسم المشترك],
    ISNULL(L.PhoneNumber, ISNULL(S.PhoneNumber, N'')) AS [رقم الهاتف],
    ISNULL(I.InvoiceNumber, CAST(I.InvoiceID AS NVARCHAR(30))) AS [رقم المستند],
    CONVERT(varchar(10), I.InvoiceDate, 111) AS [تاريخ الفاتورة],

    CAST(ISNULL(I.TotalAmount,0) AS DECIMAL(18,2)) AS [مبلغ الفترة],
    CAST(ISNULL(I.Arrears,0) AS DECIMAL(18,2)) AS [المتأخرات وقت الإصدار],
    CAST(ISNULL(PA.PaidAmount,0) AS DECIMAL(18,2)) AS [المدفوع على الفاتورة],

    CAST(
        CASE
            WHEN ISNULL(VB.Balance,0) > 0 THEN ISNULL(VB.Balance,0)
            ELSE 0
        END
        AS DECIMAL(18,2)
    ) AS [إجمالي المتبقي],

    CAST(
        CASE
            WHEN ISNULL(VB.Balance,0) < 0 THEN ABS(ISNULL(VB.Balance,0))
            ELSE 0
        END
        AS DECIMAL(18,2)
    ) AS [الرصيد المقدم],

    CASE
        WHEN ISNULL(VB.Balance,0) > 0 THEN N'عليه'
        WHEN ISNULL(VB.Balance,0) < 0 THEN N'له'
        ELSE N'متوازن'
    END AS [الحالة المالية],

    ISNULL(L.Message, N'') AS [نص الرسالة],
    ISNULL(L.Status, N'') AS [الحالة],
    ISNULL(L.Reason, N'') AS [السبب],
    L.CreatedAt AS [تاريخ الإنشاء]

FROM dbo.SmsLogs L
INNER JOIN dbo.Subscribers S ON S.SubscriberID = L.SubscriberID
LEFT JOIN dbo.Invoices I ON I.InvoiceID = L.InvoiceID

OUTER APPLY
(
    SELECT SUM(ISNULL(P.Amount,0)) AS PaidAmount
    FROM dbo.Payments P
    WHERE P.InvoiceID = I.InvoiceID
) PA

LEFT JOIN dbo.vw_SubscriberBalance VB
    ON VB.SubscriberID = S.SubscriberID

WHERE L.MessageType = N'Invoice'
  AND L.Status IN (N'Pending', N'Failed')
  AND L.CreatedAt >= @FromDate
  AND L.CreatedAt < @ToDate
  AND (@SubscriberID = 0 OR L.SubscriberID = @SubscriberID)
  AND (
        @Search IS NULL
        OR ISNULL(S.Name, N'') LIKE @Search
        OR ISNULL(L.PhoneNumber, N'') LIKE @Search
        OR ISNULL(L.Message, N'') LIKE @Search
        OR CONVERT(NVARCHAR(50), L.SmsID) LIKE @Search
        OR CONVERT(NVARCHAR(50), ISNULL(L.InvoiceID, 0)) LIKE @Search
        OR ISNULL(I.InvoiceNumber, N'') LIKE @Search
      )
  AND (
        @InvoiceStatus IS NULL
        OR (
            CASE
                WHEN ISNULL(PA.PaidAmount,0) >= ISNULL(I.TotalAmount,0) AND ISNULL(I.TotalAmount,0) > 0 THEN N'مدفوعة'
                WHEN ISNULL(PA.PaidAmount,0) > 0 AND ISNULL(PA.PaidAmount,0) < ISNULL(I.TotalAmount,0) THEN N'جزئية'
                ELSE N'غير مدفوعة'
            END
          ) = @InvoiceStatus
      )
ORDER BY L.SmsID DESC;";

        private string BuildPendingPaymentMessagesQuery() => @"
WITH ReceiptAgg AS
(
    SELECT
        RA.ReceiptID,
        SUM(CASE WHEN RA.ApplicationType = N'InvoicePayment' THEN RA.AppliedAmount ELSE 0 END) AS PaidToInvoices,
        SUM(CASE WHEN RA.ApplicationType = N'AdvanceCredit' THEN RA.AppliedAmount ELSE 0 END) AS AdvanceCreditAmount,
        COUNT(DISTINCT CASE WHEN RA.ApplicationType = N'InvoicePayment' THEN RA.InvoiceID END) AS InvoicesCount
    FROM dbo.ReceiptApplications RA
    GROUP BY RA.ReceiptID
)
SELECT
    L.SmsID,
    L.TemplateID,
    L.PaymentID,
    L.ReceiptID,

    S.SubscriberID AS SubscriberID,
    S.Name AS [اسم المشترك],
    ISNULL(L.PhoneNumber, ISNULL(S.PhoneNumber, N'')) AS [رقم الهاتف],

    ISNULL(R.ReceiptNumber, N'') AS [رقم الإيصال],
    CONVERT(varchar(10), ISNULL(P.PaymentDate, R.PaymentDate), 111) AS [تاريخ السداد],

    CASE
        WHEN R.PaymentMethod = N'Cash' THEN N'نقداً'
        WHEN R.PaymentMethod = N'Transfer' THEN N'تحويل'
        WHEN R.PaymentMethod = N'Cheque' THEN N'شيك'
        WHEN R.PaymentMethod = N'Other' THEN N'أخرى'
        ELSE ISNULL(R.PaymentMethod, N'')
    END AS [طريقة السداد],

    CAST(ISNULL(R.TotalAmount,0) AS DECIMAL(18,2)) AS [إجمالي الإيصال],
    CAST(ISNULL(A.PaidToInvoices,0) AS DECIMAL(18,2)) AS [مسدد على الفواتير],
    CAST(ISNULL(A.AdvanceCreditAmount,0) AS DECIMAL(18,2)) AS [رصيد مقدم],
    CAST(ISNULL(A.InvoicesCount,0) AS INT) AS [عدد الفواتير],

    CAST(
        CASE
            WHEN ISNULL(VB.Balance,0) > 0 THEN ISNULL(VB.Balance,0)
            ELSE 0
        END
        AS DECIMAL(18,2)
    ) AS [إجمالي المتبقي],

    CAST(
        CASE
            WHEN ISNULL(VB.Balance,0) < 0 THEN ABS(ISNULL(VB.Balance,0))
            ELSE 0
        END
        AS DECIMAL(18,2)
    ) AS [الرصيد المقدم],

    CASE
        WHEN ISNULL(VB.Balance,0) > 0 THEN N'عليه'
        WHEN ISNULL(VB.Balance,0) < 0 THEN N'له'
        ELSE N'متوازن'
    END AS [الحالة المالية],

    ISNULL(L.Message, N'') AS [نص الرسالة],
    ISNULL(L.Status, N'') AS [الحالة],
    ISNULL(L.Reason, N'') AS [السبب],
    L.CreatedAt AS [تاريخ الإنشاء]

FROM dbo.SmsLogs L
INNER JOIN dbo.Subscribers S ON S.SubscriberID = L.SubscriberID
LEFT JOIN dbo.Payments P ON P.PaymentID = L.PaymentID
LEFT JOIN dbo.Receipts R ON R.ReceiptID = ISNULL(L.ReceiptID, P.ReceiptID)
LEFT JOIN ReceiptAgg A ON A.ReceiptID = R.ReceiptID
LEFT JOIN dbo.vw_SubscriberBalance VB ON VB.SubscriberID = S.SubscriberID

WHERE L.MessageType = N'Payment'
  AND L.Status IN (N'Pending', N'Failed')
  AND L.CreatedAt >= @FromDate
  AND L.CreatedAt < @ToDate
  AND (@SubscriberID = 0 OR L.SubscriberID = @SubscriberID)
  AND (
        @Search IS NULL
        OR ISNULL(S.Name, N'') LIKE @Search
        OR ISNULL(L.PhoneNumber, N'') LIKE @Search
        OR ISNULL(L.Message, N'') LIKE @Search
        OR CONVERT(NVARCHAR(50), L.SmsID) LIKE @Search
        OR CONVERT(NVARCHAR(50), ISNULL(L.PaymentID, 0)) LIKE @Search
        OR CONVERT(NVARCHAR(50), ISNULL(L.ReceiptID, 0)) LIKE @Search
        OR ISNULL(R.ReceiptNumber, N'') LIKE @Search
      )
  AND (
        @PaymentMethod IS NULL
        OR ISNULL(R.PaymentMethod, N'') = @PaymentMethod
      )
ORDER BY L.SmsID DESC;";

        private string BuildSmsLogsQuery() => @"
SELECT TOP (500)
    L.SmsID,
    L.MessageType,
    L.InvoiceID,
    L.PaymentID,
    L.ReceiptID,
    L.SubscriberID,
    ISNULL(S.Name, N'') AS SubscriberName,
    ISNULL(L.PhoneNumber, N'') AS PhoneNumber,
    ISNULL(L.Message, N'') AS Message,
    ISNULL(L.Status, N'') AS Status,
    ISNULL(L.Reason, N'') AS Reason,
    L.SentDate,
    L.RetryCount,
    L.TemplateID,
    L.CreatedAt
FROM dbo.SmsLogs L
LEFT JOIN dbo.Subscribers S ON S.SubscriberID = L.SubscriberID
WHERE L.CreatedAt >= @FromDate
  AND L.CreatedAt < @ToDate
  AND (@SubscriberID = 0 OR L.SubscriberID = @SubscriberID)
  AND (
        @Search IS NULL
        OR ISNULL(S.Name, N'') LIKE @Search
        OR ISNULL(L.PhoneNumber, N'') LIKE @Search
        OR ISNULL(L.Message, N'') LIKE @Search
        OR CONVERT(NVARCHAR(50), L.SmsID) LIKE @Search
        OR CONVERT(NVARCHAR(50), ISNULL(L.InvoiceID, 0)) LIKE @Search
        OR CONVERT(NVARCHAR(50), ISNULL(L.PaymentID, 0)) LIKE @Search
        OR CONVERT(NVARCHAR(50), ISNULL(L.ReceiptID, 0)) LIKE @Search
      )
ORDER BY L.SmsID DESC;";
    }
}
/*using System;
using System.Data;
using System.Data.SqlClient;

namespace water3.Repositories
{
    public class SmsLogsRepository
    {
        public DataTable GetLogs(DateTime from, DateTime toExclusive, int subscriberId, string searchLike)
        {
            var dt = new DataTable();

            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(BuildSmsLogsQuery(), con))
            {
                cmd.Parameters.AddWithValue("@FromDate", from.Date);
                cmd.Parameters.AddWithValue("@ToDate", toExclusive);
                cmd.Parameters.AddWithValue("@SubscriberID", subscriberId);
                cmd.Parameters.AddWithValue("@Search", (object)searchLike ?? DBNull.Value);

                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }

            return dt;
        }

        public DataTable GetPendingOrFailed(int maxRetry = 3)
        {
            var dt = new DataTable();

            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
SELECT TOP (500)
    SmsID, InvoiceID, SubscriberID, PhoneNumber, Message, Status, Reason, SentDate, CollectorID, TemplateID, RetryCount, CreatedAt
FROM dbo.SmsLogs
WHERE (Status = N'Pending' OR Status = N'Failed')
  AND RetryCount < @MaxRetry
ORDER BY CreatedAt ASC;", con))
            {
                cmd.Parameters.Add("@MaxRetry", SqlDbType.Int).Value = maxRetry;

                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }

            return dt;
        }

        public bool ExistsPendingForInvoiceTemplate(int invoiceId, int templateId)
        {
            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
SELECT COUNT(1)
FROM dbo.SmsLogs
WHERE InvoiceID=@Inv AND TemplateID=@Tpl
  AND (Status=N'Pending' OR Status=N'Failed');", con))
            {
                cmd.Parameters.Add("@Inv", SqlDbType.Int).Value = invoiceId;
                cmd.Parameters.Add("@Tpl", SqlDbType.Int).Value = templateId;

                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        public void InsertPending(int invoiceId, int subscriberId, string phone, string message, int templateId, int? collectorId = null)
        {
            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
INSERT INTO dbo.SmsLogs
(InvoiceID, SubscriberID, PhoneNumber, Message, Status, Reason, SentDate, CollectorID, TemplateID, RetryCount, CreatedAt)
VALUES
(@InvoiceID, @SubscriberID, @PhoneNumber, @Message, N'Pending', NULL, NULL, @CollectorID, @TemplateID, 0, GETDATE());", con))
            {
                cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);
                cmd.Parameters.AddWithValue("@SubscriberID", subscriberId);
                cmd.Parameters.AddWithValue("@PhoneNumber", (object)phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Message", (object)message ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@TemplateID", templateId);
                cmd.Parameters.AddWithValue("@CollectorID", (object)collectorId ?? DBNull.Value);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void MarkSent(int smsId)
        {
            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
UPDATE dbo.SmsLogs
SET Status=N'Sent', Reason=NULL, SentDate=GETDATE()
WHERE SmsID=@ID;", con))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = smsId;
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void MarkFailed(int smsId, string reason)
        {
            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
UPDATE dbo.SmsLogs
SET Status=N'Failed', Reason=@R, RetryCount = ISNULL(RetryCount,0) + 1
WHERE SmsID=@ID;", con))
            {
                cmd.Parameters.Add("@ID", SqlDbType.Int).Value = smsId;
                cmd.Parameters.Add("@R", SqlDbType.NVarChar, 200).Value = (object)reason ?? DBNull.Value;

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private string BuildSmsLogsQuery() => @"
SELECT TOP (500)
    L.SmsID,
    L.InvoiceID,
    L.SubscriberID,
    ISNULL(S.Name, N'') AS SubscriberName,
    ISNULL(L.PhoneNumber,N'') AS PhoneNumber,
    ISNULL(L.Message,N'') AS Message,
    ISNULL(L.Status,N'') AS Status,
    ISNULL(L.Reason,N'') AS Reason,
    L.SentDate,
    L.RetryCount,
    L.TemplateID,
    L.CreatedAt
FROM dbo.SmsLogs L
LEFT JOIN dbo.Subscribers S ON S.SubscriberID = L.SubscriberID
WHERE L.CreatedAt >= @FromDate AND L.CreatedAt < @ToDate
  AND (@SubscriberID = 0 OR L.SubscriberID = @SubscriberID)
  AND (
        @Search IS NULL
        OR ISNULL(S.Name, N'') LIKE @Search
        OR ISNULL(L.PhoneNumber, N'') LIKE @Search
        OR ISNULL(L.Message, N'') LIKE @Search
        OR CONVERT(NVARCHAR(50), L.SmsID) LIKE @Search
        OR CONVERT(NVARCHAR(50), ISNULL(L.InvoiceID, 0)) LIKE @Search
  )
ORDER BY L.SmsID DESC;";
    }
}
*/