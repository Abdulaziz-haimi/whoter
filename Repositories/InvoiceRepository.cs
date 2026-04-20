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
    

   
        public class InvoiceRepository
        {
            public DataTable GetInvoicesTable(InvoiceFilters f)
            {
                var dt = new DataTable();

                using (var con = Db.GetConnection())
                using (var cmd = new SqlCommand(BuildInvoiceQuery(), con))
                {
                    cmd.Parameters.AddWithValue("@FromDate", f.From.Date);
                    cmd.Parameters.AddWithValue("@ToDate", f.ToExclusive);
                    cmd.Parameters.AddWithValue("@SubscriberID", f.SubscriberId);
                    cmd.Parameters.AddWithValue("@Status", (object)f.Status ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Search", (object)f.SearchLike ?? DBNull.Value);

                    using (var da = new SqlDataAdapter(cmd))
                        da.Fill(dt);
                }

                return dt;
            }

            private string BuildInvoiceQuery() => @"
SELECT
    I.InvoiceID AS [رقم الفاتورة],
    S.SubscriberID AS SubscriberID,
    S.Name AS [اسم المشترك],
    ISNULL(S.PhoneNumber, N'') AS [رقم الهاتف],
    ISNULL(S.Address, N'') AS [العنوان],
    CONVERT(varchar(10), I.InvoiceDate, 111) AS [تاريخ الفاتورة],

    CAST(ISNULL(R.PreviousReading, 0) AS DECIMAL(18,2)) AS [القراءة السابقة],
    CAST(ISNULL(R.CurrentReading, 0) AS DECIMAL(18,2)) AS [القراءة الحالية],
    CAST(ISNULL(I.Consumption, ISNULL(R.Consumption,0)) AS DECIMAL(18,2)) AS [الاستهلاك],

    CAST(ISNULL(I.UnitPrice,0) AS DECIMAL(18,2)) AS [سعر الوحدة],
    CAST(ISNULL(I.ServiceFees,0) AS DECIMAL(18,2)) AS [رسوم الخدمة],
    CAST(ISNULL(I.Arrears,0) AS DECIMAL(18,2)) AS [المتأخرات],
    CAST(ISNULL(I.TotalAmount,0) AS DECIMAL(18,2)) AS [المبلغ الإجمالي],

    CAST(ISNULL(PA.PaidAmount,0) AS DECIMAL(18,2)) AS [المدفوع],
    CAST((ISNULL(I.TotalAmount,0) - ISNULL(PA.PaidAmount,0)) AS DECIMAL(18,2)) AS [المتبقي],

    CASE
        WHEN ISNULL(PA.PaidAmount,0) >= ISNULL(I.TotalAmount,0) AND ISNULL(I.TotalAmount,0) > 0 THEN N'مدفوعة'
        WHEN ISNULL(PA.PaidAmount,0) > 0 AND ISNULL(PA.PaidAmount,0) < ISNULL(I.TotalAmount,0) THEN N'جزئية'
        ELSE N'غير مدفوعة'
    END AS [الحالة],

    ISNULL(I.Status, N'') AS [حالة النظام]
FROM Invoices I
INNER JOIN Subscribers S ON S.SubscriberID = I.SubscriberID
LEFT JOIN Readings R ON R.ReadingID = I.ReadingID

OUTER APPLY (
    SELECT SUM(ISNULL(P.Amount,0)) AS PaidAmount
    FROM Payments P
    WHERE P.InvoiceID = I.InvoiceID
) PA

WHERE I.InvoiceDate >= @FromDate AND I.InvoiceDate < @ToDate
  AND (@SubscriberID = 0 OR S.SubscriberID = @SubscriberID)
  AND (
        @Search IS NULL
        OR S.Name LIKE @Search
        OR ISNULL(S.PhoneNumber,N'') LIKE @Search
        OR ISNULL(S.Address,N'') LIKE @Search
        OR CONVERT(nvarchar(50), I.InvoiceID) LIKE @Search
  )
  AND (
        @Status IS NULL
        OR (
            CASE
                WHEN ISNULL(PA.PaidAmount,0) >= ISNULL(I.TotalAmount,0) AND ISNULL(I.TotalAmount,0) > 0 THEN N'مدفوعة'
                WHEN ISNULL(PA.PaidAmount,0) > 0 AND ISNULL(PA.PaidAmount,0) < ISNULL(I.TotalAmount,0) THEN N'جزئية'
                ELSE N'غير مدفوعة'
            END
        ) = @Status
  )
ORDER BY I.InvoiceDate DESC, I.InvoiceID DESC;";
        }
    }

