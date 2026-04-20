using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Newtonsoft.Json;
using water3.Models;

namespace water3.Services
{
    public class AccountStatementService
    {
        public const string RunningBalanceCol = "الرصيد التراكمي";

        private readonly string _connStr;
        private readonly string _formKey;

        public AccountStatementService(string connStr, string formKey)
        {
            _connStr = connStr;
            _formKey = formKey;
        }

        public static readonly Dictionary<string, string> AllowedCols = new Dictionary<string, string>
        {
            ["التاريخ"] = "V.[Date]",
            ["البيان"] = "ISNULL(V.Details,N'')",
            ["البند"] = "ISNULL(V.Item,N'')",
            ["نوع المستند"] = "ISNULL(V.DocumentType,N'')",
            ["رقم المستند"] = "ISNULL(V.DocumentNumber,N'')",
            ["العداد"] = "ISNULL(V.MeterNumber,N'')",
            ["مدين"] = "CAST(ISNULL(V.Debit,0) AS DECIMAL(18,2))",
            ["دائن"] = "CAST(ISNULL(V.Credit,0) AS DECIMAL(18,2))",
            ["رقم الفاتورة"] = "V.InvoiceID",
            ["رقم السداد"] = "V.PaymentID",
            ["رقم القيد"] = "V.JournalID"
        };

        public static readonly Dictionary<string, string> AllowedSort = new Dictionary<string, string>
        {
            ["التاريخ"] = "V.[Date]",
            ["رقم المستند"] = "V.DocumentNumber",
            ["نوع المستند"] = "V.DocumentType",
            ["البند"] = "V.Item",
            ["مدين"] = "V.Debit",
            ["دائن"] = "V.Credit"
        };

        public class SubscriberInfo
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            public string MeterNumber { get; set; }
        }

        public class ReportResult
        {
            public SubscriberInfo Info { get; set; }
            public decimal Opening { get; set; }
            public decimal TotalDebit { get; set; }
            public decimal TotalCredit { get; set; }
            public decimal Net => TotalDebit - TotalCredit;
            public decimal Closing { get; set; }
            public DataTable Data { get; set; }
        }

        public List<ComboBoxItem> SearchSubscribers(string searchKey, int top = 200)
        {
            var list = new List<ComboBoxItem>();

            string sql = $@"
SELECT TOP ({top})
    S.SubscriberID,
    S.Name,
    ISNULL(M.MeterNumber, N'بدون') AS MeterNumber
FROM dbo.Subscribers S
OUTER APPLY
(
    SELECT TOP 1 SM.MeterID
    FROM dbo.SubscriberMeters SM
    WHERE SM.SubscriberID = S.SubscriberID
    ORDER BY SM.IsPrimary DESC, SM.SubscriberMeterID DESC
) X
LEFT JOIN dbo.Meters M ON M.MeterID = X.MeterID
WHERE S.IsActive = 1
  AND
  (
      @q = N''
      OR S.Name LIKE @likeq
      OR ISNULL(M.MeterNumber, N'') LIKE @likeq
      OR ISNULL(S.PhoneNumber, N'') LIKE @likeq
  )
ORDER BY S.Name;";

            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                //cmd.Parameters.AddWithValue("@q", (object)(searchKey ?? string.Empty));
                //cmd.Parameters.AddWithValue("@likeq", "%" + (searchKey ?? string.Empty).Trim() + "%");
                cmd.Parameters.Add("@q", SqlDbType.NVarChar, 200).Value = (object)(searchKey ?? string.Empty);
                cmd.Parameters.Add("@likeq", SqlDbType.NVarChar, 210).Value = "%" + (searchKey ?? string.Empty).Trim() + "%";
                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new ComboBoxItem(
                            $"{Convert.ToString(r["Name"])} (عداد: {Convert.ToString(r["MeterNumber"])})",
                            Convert.ToString(r["SubscriberID"])
                        ));
                    }
                }
            }

            return list;
        }

        public ReportResult GetReport(int subscriberId, DateTime fromDate, DateTime toDate, string reportType, ReportOptions opt)
        {
            using (var con = new SqlConnection(_connStr))
            {
                con.Open();

                var info = GetSubscriberInfo(con, subscriberId);
                if (info == null)
                {
                    return new ReportResult
                    {
                        Info = null,
                        Data = new DataTable(),
                        Opening = 0,
                        TotalDebit = 0,
                        TotalCredit = 0,
                        Closing = 0
                    };
                }

                decimal opening = GetOpeningFromView(con, subscriberId, fromDate);
                var totals = GetTotalsFromView(con, subscriberId, fromDate, toDate);
                decimal closing = opening + (totals.debit - totals.credit);

                bool forceSummary = (reportType == "إجمالي") || (opt.GroupBy != "بدون");

                if (reportType == "فواتير فقط")
                {
                    opt.OnlyInvoices = true;
                    opt.OnlyPayments = false;
                }
                else if (reportType == "مدفوعات فقط")
                {
                    opt.OnlyPayments = true;
                    opt.OnlyInvoices = false;
                }

                string sql = forceSummary
                    ? BuildSummarySql_View(opt)
                    : BuildDetailSql_View(opt);

                var dt = new DataTable();
                using (var da = new SqlDataAdapter(sql, con))
                {
                    da.SelectCommand.Parameters.AddWithValue("@sid", subscriberId);
                    da.SelectCommand.Parameters.AddWithValue("@from", fromDate.Date);
                    da.SelectCommand.Parameters.AddWithValue("@to", toDate.Date);
                    da.Fill(dt);
                }

                return new ReportResult
                {
                    Info = info,
                    Opening = opening,
                    TotalDebit = totals.debit,
                    TotalCredit = totals.credit,
                    Closing = closing,
                    Data = dt
                };
            }
        }

        private SubscriberInfo GetSubscriberInfo(SqlConnection con, int subscriberId)
        {
            const string sql = @"
SELECT
    S.Name,
    S.Address,
    S.PhoneNumber,
    ISNULL(M.MeterNumber, N'بدون') AS MeterNumber
FROM dbo.Subscribers S
OUTER APPLY
(
    SELECT TOP 1 SM.MeterID
    FROM dbo.SubscriberMeters SM
    WHERE SM.SubscriberID = S.SubscriberID
    ORDER BY SM.IsPrimary DESC, SM.SubscriberMeterID DESC
) X
LEFT JOIN dbo.Meters M ON M.MeterID = X.MeterID
WHERE S.SubscriberID = @sid;";

            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@sid", subscriberId);

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;

                    return new SubscriberInfo
                    {
                        Name = Convert.ToString(r["Name"]),
                        Address = Convert.ToString(r["Address"]),
                        Phone = Convert.ToString(r["PhoneNumber"]),
                        MeterNumber = Convert.ToString(r["MeterNumber"])
                    };
                }
            }
        }

        private decimal GetOpeningFromView(SqlConnection con, int subscriberId, DateTime fromDate)
        {
            const string sql = @"
SELECT CAST(ISNULL(SUM(ISNULL(V.Debit,0) - ISNULL(V.Credit,0)),0) AS DECIMAL(18,2))
FROM dbo.vw_AccountStatement V
WHERE V.SubscriberID = @sid
  AND V.[Date] < @from;";

            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@sid", subscriberId);
                cmd.Parameters.AddWithValue("@from", fromDate.Date);

                var obj = cmd.ExecuteScalar();
                return (obj == null || obj == DBNull.Value) ? 0m : Convert.ToDecimal(obj);
            }
        }

        private (decimal debit, decimal credit) GetTotalsFromView(SqlConnection con, int subscriberId, DateTime fromDate, DateTime toDate)
        {
            const string sql = @"
SELECT
    CAST(ISNULL(SUM(ISNULL(V.Debit,0)),0) AS DECIMAL(18,2)) AS TotalDebit,
    CAST(ISNULL(SUM(ISNULL(V.Credit,0)),0) AS DECIMAL(18,2)) AS TotalCredit
FROM dbo.vw_AccountStatement V
WHERE V.SubscriberID = @sid
  AND V.[Date] >= @from
  AND V.[Date] <= @to;";

            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@sid", subscriberId);
                cmd.Parameters.AddWithValue("@from", fromDate.Date);
                cmd.Parameters.AddWithValue("@to", toDate.Date);

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return (0m, 0m);

                    decimal debit = r["TotalDebit"] == DBNull.Value ? 0m : Convert.ToDecimal(r["TotalDebit"]);
                    decimal credit = r["TotalCredit"] == DBNull.Value ? 0m : Convert.ToDecimal(r["TotalCredit"]);

                    return (debit, credit);
                }
            }
        }

        public string BuildDetailSql_View(ReportOptions opt)
        {
            var selectParts = new List<string> { "V.StatementID" };

            foreach (var c in opt.SelectedColumns)
            {
                if (c == RunningBalanceCol) continue;
                if (!AllowedCols.TryGetValue(c, out var expr)) continue;

                selectParts.Add($"{expr} AS [{c}]");
            }

            if (opt.SelectedColumns.Contains(RunningBalanceCol))
            {
                selectParts.Add($"CAST(ISNULL(V.RunningBalance,0) AS DECIMAL(18,2)) AS [{RunningBalanceCol}]");
            }

            var where = new StringBuilder(@"
WHERE V.SubscriberID = @sid
  AND V.[Date] >= @from
  AND V.[Date] <= @to
");

            // مهم: فواتير فقط لا يعني InvoiceID IS NOT NULL
            // لأن بعض سطور المدفوعات مرتبطة بفواتير أيضًا
            if (opt.OnlyInvoices)
                where.AppendLine("  AND V.PaymentID IS NULL AND V.InvoiceID IS NOT NULL");

            if (opt.OnlyPayments)
                where.AppendLine("  AND V.PaymentID IS NOT NULL");

            var orderExpr = AllowedSort.TryGetValue(opt.SortBy ?? "التاريخ", out var s)
                ? s
                : "V.[Date]";

            string dir = opt.SortDesc ? "DESC" : "ASC";

            return $@"
SELECT
    {string.Join("," + Environment.NewLine + "    ", selectParts)}
FROM dbo.vw_AccountStatement V
{where}
ORDER BY {orderExpr} {dir}, V.StatementID;";
        }

        public string BuildSummarySql_View(ReportOptions opt)
        {
            string groupExpr;
            string groupLabel;

            switch (opt.GroupBy)
            {
                case "شهري":
                    groupLabel = "الشهر";
                    groupExpr = "CONCAT(YEAR(V.[Date]), '/', RIGHT('0' + CAST(MONTH(V.[Date]) AS VARCHAR(2)), 2))";
                    break;

                case "نوع المستند":
                    groupLabel = "نوع المستند";
                    groupExpr = "ISNULL(V.DocumentType, N'')";
                    break;

                case "البند":
                    groupLabel = "البند";
                    groupExpr = "ISNULL(V.Item, N'')";
                    break;

                default:
                    return @"
SELECT
    CAST(ISNULL(SUM(ISNULL(V.Debit,0)),0) AS DECIMAL(18,2)) AS [إجمالي مدين],
    CAST(ISNULL(SUM(ISNULL(V.Credit,0)),0) AS DECIMAL(18,2)) AS [إجمالي دائن],
    CAST(ISNULL(SUM(ISNULL(V.Debit,0) - ISNULL(V.Credit,0)),0) AS DECIMAL(18,2)) AS [صافي الفترة]
FROM dbo.vw_AccountStatement V
WHERE V.SubscriberID = @sid
  AND V.[Date] >= @from
  AND V.[Date] <= @to;";
            }

            var where = new StringBuilder(@"
WHERE V.SubscriberID = @sid
  AND V.[Date] >= @from
  AND V.[Date] <= @to
");

            if (opt.OnlyInvoices)
                where.AppendLine("  AND V.PaymentID IS NULL AND V.InvoiceID IS NOT NULL");

            if (opt.OnlyPayments)
                where.AppendLine("  AND V.PaymentID IS NOT NULL");

            string dir = opt.SortDesc ? "DESC" : "ASC";

            return $@"
SELECT
    {groupExpr} AS [{groupLabel}],
    CAST(ISNULL(SUM(ISNULL(V.Debit,0)),0) AS DECIMAL(18,2)) AS [إجمالي مدين],
    CAST(ISNULL(SUM(ISNULL(V.Credit,0)),0) AS DECIMAL(18,2)) AS [إجمالي دائن],
    CAST(ISNULL(SUM(ISNULL(V.Debit,0) - ISNULL(V.Credit,0)),0) AS DECIMAL(18,2)) AS [الصافي]
FROM dbo.vw_AccountStatement V
{where}
GROUP BY {groupExpr}
ORDER BY {groupExpr} {dir};";
        }

        public void SavePreset(string presetName, ReportOptions optWithoutSubscriber)
        {
            string json = JsonConvert.SerializeObject(optWithoutSubscriber);

            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(@"
INSERT INTO dbo.ReportPresets (PresetName, FormKey, JsonOptions)
VALUES (@n, @k, @j);", con))
            {
                cmd.Parameters.AddWithValue("@n", presetName);
                cmd.Parameters.AddWithValue("@k", _formKey);
                cmd.Parameters.AddWithValue("@j", json);

                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetPresetsList()
        {
            var dt = new DataTable();

            using (var con = new SqlConnection(_connStr))
            using (var da = new SqlDataAdapter(@"
SELECT PresetID, PresetName
FROM dbo.ReportPresets
WHERE FormKey = @k
ORDER BY CreatedAt DESC;", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@k", _formKey);
                da.Fill(dt);
            }

            return dt;
        }

        public ReportOptions LoadPresetOptions(int presetId)
        {
            string json = null;

            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(@"
SELECT JsonOptions
FROM dbo.ReportPresets
WHERE PresetID = @id
  AND FormKey = @k;", con))
            {
                cmd.Parameters.AddWithValue("@id", presetId);
                cmd.Parameters.AddWithValue("@k", _formKey);

                con.Open();
                json = cmd.ExecuteScalar() as string;
            }

            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonConvert.DeserializeObject<ReportOptions>(json);
        }
    }
}
/*using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Newtonsoft.Json;
using water3.Models;

namespace water3.Services
{
    public class AccountStatementService
    {
        public const string RunningBalanceCol = "الرصيد التراكمي";

        private readonly string _connStr;
        private readonly string _formKey;

        public AccountStatementService(string connStr, string formKey)
        {
            _connStr = connStr;
            _formKey = formKey;
        }

        // ✅ كل الأعمدة من الـ View فقط
        public static readonly Dictionary<string, string> AllowedCols = new Dictionary<string, string>
        {
            ["التاريخ"] = "V.[Date]",
            ["البيان"] = "ISNULL(V.Details,N'')",
            ["البند"] = "ISNULL(V.Item,N'')",
            ["نوع المستند"] = "ISNULL(V.DocumentType,N'')",
            ["رقم المستند"] = "ISNULL(V.DocumentNumber,N'')",
            ["العداد"] = "ISNULL(V.MeterNumber,N'')",
            ["مدين"] = "CAST(ISNULL(V.Debit,0) AS DECIMAL(18,2))",
            ["دائن"] = "CAST(ISNULL(V.Credit,0) AS DECIMAL(18,2))",
            ["رقم الفاتورة"] = "V.InvoiceID",
            ["رقم السداد"] = "V.PaymentID",
            ["رقم القيد"] = "V.JournalID",
        };

        public static readonly Dictionary<string, string> AllowedSort = new Dictionary<string, string>
        {
            ["التاريخ"] = "V.[Date]",
            ["رقم المستند"] = "V.DocumentNumber",
            ["نوع المستند"] = "V.DocumentType",
            ["البند"] = "V.Item",
            ["مدين"] = "V.Debit",
            ["دائن"] = "V.Credit",
        };

        // ===== DTOs =====
        public class SubscriberInfo
        {
            public string Name { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            public string MeterNumber { get; set; }
        }

        public class ReportResult
        {
            public SubscriberInfo Info { get; set; }
            public decimal Opening { get; set; }
            public decimal TotalDebit { get; set; }
            public decimal TotalCredit { get; set; }
            public decimal Net => TotalDebit - TotalCredit;
            public decimal Closing { get; set; }
            public DataTable Data { get; set; }
        }

        // ===== Subscribers lookup =====
        public List<ComboBoxItem> SearchSubscribers(string searchKey, int top = 200)
        {
            var list = new List<ComboBoxItem>();
            string sql = $@"
SELECT TOP {top}
    S.SubscriberID,
    S.Name,
    ISNULL(M.MeterNumber, N'بدون') AS MeterNumber
FROM Subscribers S
OUTER APPLY (
    SELECT TOP 1 sm.MeterID
    FROM SubscriberMeters sm
    WHERE sm.SubscriberID = S.SubscriberID
    ORDER BY sm.IsPrimary DESC, sm.SubscriberMeterID DESC
) smx
LEFT JOIN Meters M ON M.MeterID = smx.MeterID
WHERE S.IsActive = 1
  AND (@q = N'' OR S.Name LIKE @likeq OR M.MeterNumber LIKE @likeq)
ORDER BY S.Name;";

            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@q", searchKey ?? "");
                cmd.Parameters.AddWithValue("@likeq", "%" + (searchKey ?? "") + "%");

                con.Open();
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new ComboBoxItem(
                            $"{r["Name"]} (عداد: {r["MeterNumber"]})",
                            r["SubscriberID"].ToString()
                        ));
                    }
                }
            }
            return list;
        }

        // ===== تقرير كامل (View only) =====
        public ReportResult GetReport(int subscriberId, DateTime fromDate, DateTime toDate, string reportType, ReportOptions opt)
        {
            using (var con = new SqlConnection(_connStr))
            {
                con.Open();

                // 1) info
                var info = GetSubscriberInfo(con, subscriberId);
                if (info == null)
                    return new ReportResult { Info = null, Data = new DataTable() };

                // 2) opening (من الـ View)
                decimal opening = GetOpeningFromView(con, subscriberId, fromDate);

                // 3) totals (من الـ View)
                (decimal totalDebit, decimal totalCredit) = GetTotalsFromView(con, subscriberId, fromDate, toDate);

                decimal closing = opening + (totalDebit - totalCredit);

                // 4) force options by reportType
                bool forceSummary = (reportType == "إجمالي") || (opt.GroupBy != "بدون");

                if (reportType == "فواتير فقط")
                {
                    opt.OnlyInvoices = true;
                    opt.OnlyPayments = false;
                }
                else if (reportType == "مدفوعات فقط")
                {
                    opt.OnlyPayments = true;
                    opt.OnlyInvoices = false;
                }

                string sql = forceSummary ? BuildSummarySql_View(opt) : BuildDetailSql_View(opt);

                var dt = new DataTable();
                using (var da = new SqlDataAdapter(sql, con))
                {
                    da.SelectCommand.Parameters.AddWithValue("@sid", subscriberId);
                    da.SelectCommand.Parameters.AddWithValue("@from", fromDate);
                    da.SelectCommand.Parameters.AddWithValue("@to", toDate);
                    da.Fill(dt);
                }

                return new ReportResult
                {
                    Info = info,
                    Opening = opening,
                    TotalDebit = totalDebit,
                    TotalCredit = totalCredit,
                    Closing = closing,
                    Data = dt
                };
            }
        }

        // ===== Info =====
        private SubscriberInfo GetSubscriberInfo(SqlConnection con, int subscriberId)
        {
            // نجيب العداد الأساسي (نفس كودك)
            string infoSql = @"
SELECT
    S.Name,
    S.Address,
    S.PhoneNumber,
    ISNULL(M.MeterNumber, N'بدون') AS MeterNumber
FROM Subscribers S
OUTER APPLY (
    SELECT TOP 1 sm.MeterID
    FROM SubscriberMeters sm
    WHERE sm.SubscriberID = S.SubscriberID
    ORDER BY sm.IsPrimary DESC, sm.SubscriberMeterID DESC
) smx
LEFT JOIN Meters M ON M.MeterID = smx.MeterID
WHERE S.SubscriberID = @sid;";

            using (var cmd = new SqlCommand(infoSql, con))
            {
                cmd.Parameters.AddWithValue("@sid", subscriberId);
                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return null;

                    return new SubscriberInfo
                    {
                        Name = Convert.ToString(r["Name"]),
                        Address = Convert.ToString(r["Address"]),
                        Phone = Convert.ToString(r["PhoneNumber"]),
                        MeterNumber = Convert.ToString(r["MeterNumber"])
                    };
                }
            }
        }

        // ===== Opening/Totals من الـ View =====
        private decimal GetOpeningFromView(SqlConnection con, int subscriberId, DateTime fromDate)
        {
            // الافتتاحي = مجموع (مدين-دائن) قبل from
            using (var cmd = new SqlCommand(@"
SELECT CAST(ISNULL(SUM(ISNULL(V.Debit,0) - ISNULL(V.Credit,0)),0) AS DECIMAL(18,2))
FROM vw_AccountStatement V
WHERE V.SubscriberID=@sid AND V.[Date] < @from;", con))
            {
                cmd.Parameters.AddWithValue("@sid", subscriberId);
                cmd.Parameters.AddWithValue("@from", fromDate);
                var obj = cmd.ExecuteScalar();
                return obj == null || obj == DBNull.Value ? 0m : Convert.ToDecimal(obj);
            }
        }

        private (decimal debit, decimal credit) GetTotalsFromView(SqlConnection con, int subscriberId, DateTime fromDate, DateTime toDate)
        {
            using (var cmd = new SqlCommand(@"
SELECT
    CAST(ISNULL(SUM(ISNULL(V.Debit,0)),0) AS DECIMAL(18,2)) AS TotalDebit,
    CAST(ISNULL(SUM(ISNULL(V.Credit,0)),0) AS DECIMAL(18,2)) AS TotalCredit
FROM vw_AccountStatement V
WHERE V.SubscriberID=@sid AND V.[Date] >= @from AND V.[Date] <= @to;", con))
            {
                cmd.Parameters.AddWithValue("@sid", subscriberId);
                cmd.Parameters.AddWithValue("@from", fromDate);
                cmd.Parameters.AddWithValue("@to", toDate);

                using (var r = cmd.ExecuteReader())
                {
                    if (!r.Read()) return (0m, 0m);
                    return (Convert.ToDecimal(r["TotalDebit"]), Convert.ToDecimal(r["TotalCredit"]));
                }
            }
        }

        // ===== SQL Builders (View) =====
        public string BuildDetailSql_View(ReportOptions opt)
        {
            var selectParts = new List<string> { "V.StatementID" };

            foreach (var c in opt.SelectedColumns)
            {
                if (c == RunningBalanceCol) continue;
                if (!AllowedCols.TryGetValue(c, out var expr)) continue;
                selectParts.Add($"{expr} AS [{c}]");
            }

            if (opt.SelectedColumns.Contains(RunningBalanceCol))
            {
                // RunningBalance موجود داخل الـ View
                selectParts.Add($"CAST(ISNULL(V.RunningBalance,0) AS DECIMAL(18,2)) AS [{RunningBalanceCol}]");
            }

            var where = new StringBuilder(@"
WHERE V.SubscriberID=@sid
  AND V.[Date] >= @from AND V.[Date] <= @to
");
            if (opt.OnlyInvoices) where.AppendLine("  AND V.InvoiceID IS NOT NULL");
            if (opt.OnlyPayments) where.AppendLine("  AND V.PaymentID IS NOT NULL");

            var orderExpr = AllowedSort.TryGetValue(opt.SortBy ?? "التاريخ", out var s) ? s : "V.[Date]";
            var dir = opt.SortDesc ? "DESC" : "ASC";

            return $@"
SELECT
    {string.Join(",\n    ", selectParts)}
FROM vw_AccountStatement V
{where}
ORDER BY {orderExpr} {dir}, V.StatementID;";
        }

        public string BuildSummarySql_View(ReportOptions opt)
        {
            string groupExpr, groupLabel;

            switch (opt.GroupBy)
            {
                case "شهري":
                    groupLabel = "الشهر";
                    groupExpr = "CONCAT(YEAR(V.[Date]), '/', RIGHT('0'+CAST(MONTH(V.[Date]) AS VARCHAR(2)),2))";
                    break;
                case "نوع المستند":
                    groupLabel = "نوع المستند";
                    groupExpr = "ISNULL(V.DocumentType,N'')";
                    break;
                case "البند":
                    groupLabel = "البند";
                    groupExpr = "ISNULL(V.Item,N'')";
                    break;
                default:
                    // إجمالي بدون تجميع
                    return @"
SELECT
    CAST(ISNULL(SUM(ISNULL(V.Debit,0)),0) AS DECIMAL(18,2)) AS [إجمالي مدين],
    CAST(ISNULL(SUM(ISNULL(V.Credit,0)),0) AS DECIMAL(18,2)) AS [إجمالي دائن],
    CAST(ISNULL(SUM(ISNULL(V.Debit,0)-ISNULL(V.Credit,0)),0) AS DECIMAL(18,2)) AS [صافي الفترة]
FROM vw_AccountStatement V
WHERE V.SubscriberID=@sid
  AND V.[Date] >= @from AND V.[Date] <= @to;";
            }

            var where = new StringBuilder(@"
WHERE V.SubscriberID=@sid
  AND V.[Date] >= @from AND V.[Date] <= @to
");
            if (opt.OnlyInvoices) where.AppendLine("  AND V.InvoiceID IS NOT NULL");
            if (opt.OnlyPayments) where.AppendLine("  AND V.PaymentID IS NOT NULL");

            string orderBy = opt.SortDesc ? "DESC" : "ASC";

            return $@"
SELECT
    {groupExpr} AS [{groupLabel}],
    CAST(ISNULL(SUM(ISNULL(V.Debit,0)),0) AS DECIMAL(18,2)) AS [إجمالي مدين],
    CAST(ISNULL(SUM(ISNULL(V.Credit,0)),0) AS DECIMAL(18,2)) AS [إجمالي دائن],
    CAST(ISNULL(SUM(ISNULL(V.Debit,0)-ISNULL(V.Credit,0)),0) AS DECIMAL(18,2)) AS [الصافي]
FROM vw_AccountStatement V
{where}
GROUP BY {groupExpr}
ORDER BY {groupExpr} {orderBy};";
        }

        // ===== Presets (كما هي) =====
        public void SavePreset(string presetName, ReportOptions optWithoutSubscriber)
        {
            string json = JsonConvert.SerializeObject(optWithoutSubscriber);

            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(@"
INSERT INTO dbo.ReportPresets (PresetName, FormKey, JsonOptions)
VALUES (@n, @k, @j);", con))
            {
                cmd.Parameters.AddWithValue("@n", presetName);
                cmd.Parameters.AddWithValue("@k", _formKey);
                cmd.Parameters.AddWithValue("@j", json);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetPresetsList()
        {
            var dt = new DataTable();
            using (var con = new SqlConnection(_connStr))
            using (var da = new SqlDataAdapter(@"
SELECT PresetID, PresetName
FROM dbo.ReportPresets
WHERE FormKey=@k
ORDER BY CreatedAt DESC;", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@k", _formKey);
                da.Fill(dt);
            }
            return dt;
        }

        public ReportOptions LoadPresetOptions(int presetId)
        {
            string json = null;
            using (var con = new SqlConnection(_connStr))
            using (var cmd = new SqlCommand(@"
SELECT JsonOptions
FROM dbo.ReportPresets
WHERE PresetID=@id AND FormKey=@k;", con))
            {
                cmd.Parameters.AddWithValue("@id", presetId);
                cmd.Parameters.AddWithValue("@k", _formKey);
                con.Open();
                json = cmd.ExecuteScalar() as string;
            }

            if (string.IsNullOrWhiteSpace(json)) return null;
            return JsonConvert.DeserializeObject<ReportOptions>(json);
        }
    }
}
*/