using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using water3.Models;

namespace water3.Repositories
{
    public class RevenueExpenseStatementRepository
    {
        public RevenueExpenseStatementResult GetStatement(RevenueExpenseStatementFilter filter)
        {
            var result = new RevenueExpenseStatementResult();

            using (SqlConnection con = Db.GetConnection())
            {
                con.Open();

                result.OpeningBalance = GetOpeningBalance(con, filter);

                List<RevenueExpenseStatementRow> rows = GetRows(con, filter);

                decimal runningBalance = result.OpeningBalance;

                foreach (RevenueExpenseStatementRow row in rows)
                {
                    runningBalance += row.RevenueAmount;
                    runningBalance -= row.ExpenseAmount;
                    row.Balance = runningBalance;
                    result.Rows.Add(row);
                }

                result.Recalculate();
            }

            return result;
        }

        private decimal GetOpeningBalance(SqlConnection con, RevenueExpenseStatementFilter filter)
        {
            string sql = @"
SELECT
    ISNULL((
        SELECT SUM(CAST(R.TotalAmount AS DECIMAL(18,2)))
        FROM dbo.Receipts R
        LEFT JOIN dbo.Subscribers S ON S.SubscriberID = R.SubscriberID
        WHERE CAST(R.PaymentDate AS DATE) < @FromDate
          AND @CategoryID IS NULL
          AND (@MovementType IS NULL OR @MovementType = N'Revenue')
          AND (@PaymentMethod IS NULL OR R.PaymentMethod = @PaymentMethod)
          AND (
                @SearchText IS NULL
                OR R.ReceiptNumber LIKE @SearchLike
                OR ISNULL(S.Name, N'') LIKE @SearchLike
                OR ISNULL(R.Notes, N'') LIKE @SearchLike
              )
    ), 0)
    -
    ISNULL((
        SELECT SUM(CAST(E.TotalAmount AS DECIMAL(18,2)))
        FROM dbo.Expenses E
        LEFT JOIN dbo.ExpenseCategories EC ON EC.CategoryID = E.CategoryID
        WHERE CAST(E.ExpenseDate AS DATE) < @FromDate
          AND ISNULL(E.Status, N'') <> N'Deleted'
          AND (
                @MovementType IS NULL
                OR @MovementType = N'Outflow'
                OR EC.CategoryType = @MovementType
              )
          AND (@CategoryID IS NULL OR E.CategoryID = @CategoryID)
          AND (@PaymentMethod IS NULL OR E.PaymentMethod = @PaymentMethod)
          AND (
                @SearchText IS NULL
                OR E.ExpenseNumber LIKE @SearchLike
                OR ISNULL(E.SupplierName, N'') LIKE @SearchLike
                OR ISNULL(E.Description, N'') LIKE @SearchLike
                OR ISNULL(E.Notes, N'') LIKE @SearchLike
                OR ISNULL(EC.CategoryName, N'') LIKE @SearchLike
              )
    ), 0) AS OpeningBalance;
";

            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                AddParameters(cmd, filter);

                object value = cmd.ExecuteScalar();

                if (value == null || value == DBNull.Value)
                    return 0m;

                return Convert.ToDecimal(value);
            }
        }

        private List<RevenueExpenseStatementRow> GetRows(SqlConnection con, RevenueExpenseStatementFilter filter)
        {
            var list = new List<RevenueExpenseStatementRow>();

            string sql = @"
SELECT
    X.MovementDate,
    X.RefNo,
    X.MovementKind,
    X.CategoryName,
    X.PartyName,
    X.Description,
    X.PaymentMethod,
    X.RevenueAmount,
    X.ExpenseAmount,
    X.SourceType,
    X.SourceId
FROM
(
    SELECT
        CAST(R.PaymentDate AS DATE) AS MovementDate,
        R.ReceiptNumber AS RefNo,
        N'إيراد' AS MovementKind,
        N'تحصيلات / إيصالات' AS CategoryName,
        ISNULL(S.Name, N'') AS PartyName,
        ISNULL(R.Notes, N'') AS Description,
        CASE
            WHEN R.PaymentMethod = N'Cash' THEN N'نقداً'
            WHEN R.PaymentMethod = N'Transfer' THEN N'تحويل'
            WHEN R.PaymentMethod = N'Cheque' THEN N'شيك'
            WHEN R.PaymentMethod = N'Other' THEN N'أخرى'
            ELSE ISNULL(R.PaymentMethod, N'')
        END AS PaymentMethod,
        CAST(R.TotalAmount AS DECIMAL(18,2)) AS RevenueAmount,
        CAST(0 AS DECIMAL(18,2)) AS ExpenseAmount,
        N'Receipt' AS SourceType,
        R.ReceiptID AS SourceId,
        1 AS SortOrder
    FROM dbo.Receipts R
    LEFT JOIN dbo.Subscribers S ON S.SubscriberID = R.SubscriberID
    WHERE CAST(R.PaymentDate AS DATE) BETWEEN @FromDate AND @ToDate
      AND @CategoryID IS NULL
      AND (@MovementType IS NULL OR @MovementType = N'Revenue')
      AND (@PaymentMethod IS NULL OR R.PaymentMethod = @PaymentMethod)
      AND (
            @SearchText IS NULL
            OR R.ReceiptNumber LIKE @SearchLike
            OR ISNULL(S.Name, N'') LIKE @SearchLike
            OR ISNULL(R.Notes, N'') LIKE @SearchLike
          )

    UNION ALL

    SELECT
        CAST(E.ExpenseDate AS DATE) AS MovementDate,
        E.ExpenseNumber AS RefNo,
        CASE
            WHEN EC.CategoryType = N'Purchase' THEN N'مشتريات'
            WHEN EC.CategoryType = N'Loss' THEN N'خسارة'
            ELSE N'منصرف'
        END AS MovementKind,
        ISNULL(EC.CategoryName, N'') AS CategoryName,
        ISNULL(E.SupplierName, N'') AS PartyName,
        ISNULL(E.Description, N'') AS Description,
        CASE
            WHEN E.PaymentMethod = N'Cash' THEN N'نقداً'
            WHEN E.PaymentMethod = N'Transfer' THEN N'تحويل'
            WHEN E.PaymentMethod = N'Cheque' THEN N'شيك'
            WHEN E.PaymentMethod = N'Credit' THEN N'آجل'
            WHEN E.PaymentMethod = N'Other' THEN N'أخرى'
            ELSE ISNULL(E.PaymentMethod, N'')
        END AS PaymentMethod,
        CAST(0 AS DECIMAL(18,2)) AS RevenueAmount,
        CAST(E.TotalAmount AS DECIMAL(18,2)) AS ExpenseAmount,
        N'Expense' AS SourceType,
        E.ExpenseID AS SourceId,
        2 AS SortOrder
    FROM dbo.Expenses E
    LEFT JOIN dbo.ExpenseCategories EC ON EC.CategoryID = E.CategoryID
    WHERE CAST(E.ExpenseDate AS DATE) BETWEEN @FromDate AND @ToDate
      AND ISNULL(E.Status, N'') <> N'Deleted'
      AND (
            @MovementType IS NULL
            OR @MovementType = N'Outflow'
            OR EC.CategoryType = @MovementType
          )
      AND (@CategoryID IS NULL OR E.CategoryID = @CategoryID)
      AND (@PaymentMethod IS NULL OR E.PaymentMethod = @PaymentMethod)
      AND (
            @SearchText IS NULL
            OR E.ExpenseNumber LIKE @SearchLike
            OR ISNULL(E.SupplierName, N'') LIKE @SearchLike
            OR ISNULL(E.Description, N'') LIKE @SearchLike
            OR ISNULL(E.Notes, N'') LIKE @SearchLike
            OR ISNULL(EC.CategoryName, N'') LIKE @SearchLike
          )
) X
ORDER BY
    X.MovementDate ASC,
    X.SortOrder ASC,
    X.SourceId ASC;
";

            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                AddParameters(cmd, filter);

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new RevenueExpenseStatementRow
                        {
                            MovementDate = Convert.ToDateTime(dr["MovementDate"]),
                            RefNo = Convert.ToString(dr["RefNo"]),
                            MovementKind = Convert.ToString(dr["MovementKind"]),
                            CategoryName = Convert.ToString(dr["CategoryName"]),
                            PartyName = Convert.ToString(dr["PartyName"]),
                            Description = Convert.ToString(dr["Description"]),
                            PaymentMethod = Convert.ToString(dr["PaymentMethod"]),
                            RevenueAmount = Convert.ToDecimal(dr["RevenueAmount"]),
                            ExpenseAmount = Convert.ToDecimal(dr["ExpenseAmount"]),
                            SourceType = Convert.ToString(dr["SourceType"]),
                            SourceId = Convert.ToInt32(dr["SourceId"])
                        });
                    }
                }
            }

            return list;
        }

        private void AddParameters(SqlCommand cmd, RevenueExpenseStatementFilter filter)
        {
            string movementType = string.IsNullOrWhiteSpace(filter.MovementType)
                ? null
                : filter.MovementType.Trim();

            string paymentMethod = string.IsNullOrWhiteSpace(filter.PaymentMethod)
                ? null
                : filter.PaymentMethod.Trim();

            string searchText = string.IsNullOrWhiteSpace(filter.SearchText)
                ? null
                : filter.SearchText.Trim();

            cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = filter.FromDate.Date;
            cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = filter.ToDate.Date;

            cmd.Parameters.Add("@MovementType", SqlDbType.NVarChar, 30).Value =
                string.IsNullOrWhiteSpace(movementType) ? (object)DBNull.Value : movementType;

            cmd.Parameters.Add("@CategoryID", SqlDbType.Int).Value =
                filter.CategoryId.HasValue && filter.CategoryId.Value > 0
                    ? (object)filter.CategoryId.Value
                    : DBNull.Value;

            cmd.Parameters.Add("@PaymentMethod", SqlDbType.NVarChar, 30).Value =
                string.IsNullOrWhiteSpace(paymentMethod) ? (object)DBNull.Value : paymentMethod;

            cmd.Parameters.Add("@SearchText", SqlDbType.NVarChar, 200).Value =
                string.IsNullOrWhiteSpace(searchText) ? (object)DBNull.Value : searchText;

            cmd.Parameters.Add("@SearchLike", SqlDbType.NVarChar, 230).Value =
                string.IsNullOrWhiteSpace(searchText) ? (object)DBNull.Value : "%" + searchText + "%";
        }
    }
}