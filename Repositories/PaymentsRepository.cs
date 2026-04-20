using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace water3.Repositories
{
    public class PaymentsRepository
    {
        public DataTable GetPaymentsTable()
        {
            var sql = @"
;WITH ReceiptAgg AS
(
    SELECT
        RA.ReceiptID,
        SUM(CASE WHEN RA.ApplicationType = N'InvoicePayment' THEN RA.AppliedAmount ELSE 0 END) AS PaidToInvoices,
        SUM(CASE WHEN RA.ApplicationType = N'AdvanceCredit' THEN RA.AppliedAmount ELSE 0 END) AS AdvanceCreditAmount,
        COUNT(CASE WHEN RA.ApplicationType = N'InvoicePayment' THEN 1 END) AS InvoicePaymentRows,
        COUNT(CASE WHEN RA.ApplicationType = N'AdvanceCredit' THEN 1 END) AS AdvanceCreditRows
    FROM dbo.ReceiptApplications RA
    GROUP BY RA.ReceiptID
),
ReceiptMeter AS
(
    SELECT
        X.ReceiptID,
        CASE
            WHEN COUNT(DISTINCT X.MeterNumber) = 0 THEN N''
            WHEN COUNT(DISTINCT X.MeterNumber) = 1 THEN MIN(X.MeterNumber)
            ELSE N'متعدد'
        END AS MeterNumber
    FROM
    (
        -- العدادات القادمة من الفواتير المسددة بهذا الإيصال
        SELECT
            RA.ReceiptID,
            M.MeterNumber
        FROM dbo.ReceiptApplications RA
        INNER JOIN dbo.Invoices I ON I.InvoiceID = RA.InvoiceID
        LEFT JOIN dbo.Meters M ON M.MeterID = I.MeterID
        WHERE RA.ApplicationType = N'InvoicePayment'
          AND M.MeterNumber IS NOT NULL

        UNION ALL

        -- إذا كان الإيصال كله رصيدًا مقدمًا ولم يكن فيه فواتير
        SELECT
            R.ReceiptID,
            M2.MeterNumber
        FROM dbo.Receipts R
        LEFT JOIN
        (
            SELECT
                SM.SubscriberID,
                M.MeterNumber,
                ROW_NUMBER() OVER
                (
                    PARTITION BY SM.SubscriberID
                    ORDER BY SM.IsPrimary DESC, SM.SubscriberMeterID DESC
                ) AS rn
            FROM dbo.SubscriberMeters SM
            INNER JOIN dbo.Meters M ON M.MeterID = SM.MeterID
        ) M2
            ON M2.SubscriberID = R.SubscriberID
           AND M2.rn = 1
    ) X
    GROUP BY X.ReceiptID
)
SELECT
    R.ReceiptID,
    R.ReceiptNumber AS [رقم الإيصال],
    R.PaymentDate AS [التاريخ],
    S.Name AS [المشترك],
    ISNULL(RM.MeterNumber, N'') AS [رقم العداد],
    CAST(R.TotalAmount AS DECIMAL(12,2)) AS [المبلغ],
    ISNULL(C.Name, N'') AS [المحصل],

    CASE
        WHEN R.PaymentMethod = N'Cash' THEN N'نقداً'
        WHEN R.PaymentMethod = N'Transfer' THEN N'تحويل'
        WHEN R.PaymentMethod = N'Cheque' THEN N'شيك'
        WHEN R.PaymentMethod = N'Other' THEN N'أخرى'
        ELSE ISNULL(R.PaymentMethod, N'')
    END AS [الطريقة],

    CASE
        WHEN ISNULL(A.InvoicePaymentRows, 0) > 0 AND ISNULL(A.AdvanceCreditRows, 0) > 0 THEN N'سداد + رصيد مقدم'
        WHEN ISNULL(A.InvoicePaymentRows, 0) > 0 THEN N'سداد'
        WHEN ISNULL(A.AdvanceCreditRows, 0) > 0 THEN N'رصيد مقدم'
        ELSE N'إيصال'
    END AS [النوع],

    CAST(ISNULL(A.PaidToInvoices, 0) AS DECIMAL(12,2)) AS [مسدد على الفواتير],
    CAST(ISNULL(A.AdvanceCreditAmount, 0) AS DECIMAL(12,2)) AS [رصيد مقدم],
    ISNULL(R.Notes, N'') AS [ملاحظات]

FROM dbo.Receipts R
INNER JOIN dbo.Subscribers S ON S.SubscriberID = R.SubscriberID
LEFT JOIN dbo.Collectors C ON C.CollectorID = R.CollectorID
LEFT JOIN ReceiptAgg A ON A.ReceiptID = R.ReceiptID
LEFT JOIN ReceiptMeter RM ON RM.ReceiptID = R.ReceiptID
ORDER BY R.PaymentDate DESC, R.ReceiptID DESC;";

            using (var con = Db.GetConnection())
            using (var da = new SqlDataAdapter(sql, con))
            {
                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public string GetReceiptDetails(int receiptId)
        {
            var sql = @"
SELECT
    X.SortOrder,
    X.Detail
FROM
(
    SELECT
        1 AS SortOrder,
        N'• سداد فاتورة رقم '
        + ISNULL(I.InvoiceNumber, CAST(RA.InvoiceID AS NVARCHAR(20)))
        + N': '
        + FORMAT(RA.AppliedAmount, 'N2')
        + N' ريال' AS Detail
    FROM dbo.ReceiptApplications RA
    INNER JOIN dbo.Invoices I ON I.InvoiceID = RA.InvoiceID
    WHERE RA.ReceiptID = @ReceiptID
      AND RA.ApplicationType = N'InvoicePayment'

    UNION ALL

    SELECT
        2 AS SortOrder,
        N'• رصيد مقدم: '
        + FORMAT(RA.AppliedAmount, 'N2')
        + N' ريال' AS Detail
    FROM dbo.ReceiptApplications RA
    WHERE RA.ReceiptID = @ReceiptID
      AND RA.ApplicationType = N'AdvanceCredit'
) X
ORDER BY X.SortOrder, X.Detail;";

            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.Add("@ReceiptID", SqlDbType.Int).Value = receiptId;
                con.Open();

                var details = new List<string>();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        details.Add(reader["Detail"].ToString());
                }

                if (details.Count == 0)
                    return "لا توجد تفاصيل";

                return string.Join(Environment.NewLine, details);
            }
        }
        public void PayOldestInvoice(
            int subscriberId,
            int collectorId,
            DateTime paymentDate,
            decimal amount,
            string paymentTypeDb,
            string notes)
        {
            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand("PayOldestInvoice", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.Add("@SubscriberID", SqlDbType.Int).Value = subscriberId;
                cmd.Parameters.Add("@CollectorID", SqlDbType.Int).Value = collectorId;
                cmd.Parameters.Add("@PaymentDate", SqlDbType.Date).Value = paymentDate.Date;

                var pAmt = cmd.Parameters.Add("@Amount", SqlDbType.Decimal);
                pAmt.Precision = 12;
                pAmt.Scale = 2;
                pAmt.Value = amount;

                cmd.Parameters.Add("@PaymentType", SqlDbType.NVarChar, 30).Value =
                    string.IsNullOrWhiteSpace(paymentTypeDb) ? "Cash" : paymentTypeDb.Trim();

                cmd.Parameters.Add("@Notes", SqlDbType.NVarChar, 200).Value =
                    string.IsNullOrWhiteSpace(notes) ? (object)DBNull.Value : notes.Trim();

                con.Open();

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException ex)
                {
                    if (ex.Message.Contains("Cannot insert duplicate key row") &&
                        ex.Message.Contains("dbo.Payments"))
                    {
                        throw new InvalidOperationException("حدث تعارض في رقم الإيصال داخل سجل المدفوعات. تم تعديل النظام ليعتمد على الإيصال الرئيسي، تأكد من تحديث الإجراء المخزن PayOldestInvoice.");
                    }

                    throw;
                }
            }
        }

        public bool IsPeriodClosed(DateTime date)
        {
            using (var con = Db.GetConnection())
            using (var cmd = new SqlCommand("SELECT dbo.fn_IsPeriodClosed(@D)", con))
            {
                cmd.Parameters.Add("@D", SqlDbType.Date).Value = date.Date;
                con.Open();

                var result = cmd.ExecuteScalar();
                return result != null && result != DBNull.Value && Convert.ToBoolean(result);
            }
        }

        internal object GetPaymentStatistics()
        {
            throw new NotImplementedException();
        }
    }
}