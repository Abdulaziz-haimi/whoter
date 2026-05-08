using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace water3.Reports.Dynamic
{
    public class DynamicReportService
    {
        private readonly string _connStr = water3.Db.ConnectionString;

        public DataTable LoadReport(
            string reportKey,
            List<string> selectedColumnKeys,
            DateTime? fromDate,
            DateTime? toDate,
            int? subscriberId,
            string categoryType,
            string searchText)
        {
            DynamicReportDefinition def = DynamicReportDefinitions.GetByKey(reportKey);

            List<DynamicReportColumn> selectedColumns = def.Columns
                .Where(c => selectedColumnKeys.Contains(c.ColumnKey))
                .OrderBy(c => c.DisplayOrder)
                .ToList();

            if (selectedColumns.Count == 0)
                throw new InvalidOperationException("يجب اختيار عمود واحد على الأقل.");

            StringBuilder sql = new StringBuilder();

            sql.AppendLine("SELECT");

            for (int i = 0; i < selectedColumns.Count; i++)
            {
                DynamicReportColumn col = selectedColumns[i];

                sql.Append("    ");
                sql.Append(col.SqlExpression);
                sql.Append(" AS [");
                sql.Append(EscapeAlias(col.ColumnTitle));
                sql.Append("]");

                if (i < selectedColumns.Count - 1)
                    sql.Append(",");

                sql.AppendLine();
            }

            sql.AppendLine(def.BaseSql);

            using (SqlConnection cn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = cn;

                if (def.HasDateFilter)
                {
                    if (fromDate.HasValue)
                    {
                        sql.AppendLine("AND " + def.DateExpression + " >= @FromDate");
                        cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = fromDate.Value.Date;
                    }

                    if (toDate.HasValue)
                    {
                        sql.AppendLine("AND " + def.DateExpression + " < @ToDate");
                        cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = toDate.Value.Date.AddDays(1);
                    }
                }

                if (def.HasSubscriberFilter && subscriberId.HasValue && subscriberId.Value > 0)
                {
                    sql.AppendLine("AND " + def.SubscriberIdExpression + " = @SubscriberID");
                    cmd.Parameters.Add("@SubscriberID", SqlDbType.Int).Value = subscriberId.Value;
                }

                if (def.HasCategoryTypeFilter && !string.IsNullOrWhiteSpace(categoryType) && categoryType != "الكل")
                {
                    sql.AppendLine("AND " + def.CategoryTypeExpression + " = @CategoryType");
                    cmd.Parameters.Add("@CategoryType", SqlDbType.NVarChar, 20).Value = categoryType;
                }

                if (!string.IsNullOrWhiteSpace(searchText) &&
                    def.SearchExpressions != null &&
                    def.SearchExpressions.Count > 0)
                {
                    sql.AppendLine("AND (");

                    for (int i = 0; i < def.SearchExpressions.Count; i++)
                    {
                        sql.Append("    CONVERT(NVARCHAR(4000), ");
                        sql.Append(def.SearchExpressions[i]);
                        sql.Append(") LIKE @SearchText");

                        if (i < def.SearchExpressions.Count - 1)
                            sql.AppendLine(" OR");
                        else
                            sql.AppendLine();
                    }

                    sql.AppendLine(")");

                    cmd.Parameters.Add("@SearchText", SqlDbType.NVarChar, 500).Value =
                        "%" + searchText.Trim() + "%";
                }

                if (!string.IsNullOrWhiteSpace(def.OrderBySql))
                    sql.AppendLine(def.OrderBySql);

                cmd.CommandText = sql.ToString();

                DataTable dt = new DataTable(def.ReportKey);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(dt);
                }

                return dt;
            }
        }

        public DataTable GetSubscribersLookup()
        {
            DataTable dt = new DataTable();

            using (SqlConnection cn = new SqlConnection(_connStr))
            using (SqlDataAdapter da = new SqlDataAdapter(@"
SELECT *
FROM
(
    SELECT 0 AS SubscriberID, N'الكل' AS Name
    UNION ALL
    SELECT SubscriberID, Name
    FROM dbo.Subscribers
    WHERE ISNULL(IsActive, 1) = 1
) x
ORDER BY CASE WHEN SubscriberID = 0 THEN 0 ELSE 1 END, Name;", cn))
            {
                da.Fill(dt);
            }

            return dt;
        }

        public List<DynamicReportTemplateInfo> GetTemplates(string reportKey)
        {
            List<DynamicReportTemplateInfo> list = new List<DynamicReportTemplateInfo>();

            using (SqlConnection cn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TemplateID, TemplateName, ReportKey, CreatedAt
FROM dbo.ReportTemplates
WHERE IsActive = 1
  AND ReportKey = @ReportKey
ORDER BY TemplateName;", cn))
            {
                cmd.Parameters.Add("@ReportKey", SqlDbType.NVarChar, 100).Value = reportKey;

                cn.Open();

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new DynamicReportTemplateInfo
                        {
                            TemplateID = Convert.ToInt32(rd["TemplateID"]),
                            TemplateName = Convert.ToString(rd["TemplateName"]),
                            ReportKey = Convert.ToString(rd["ReportKey"]),
                            CreatedAt = Convert.ToDateTime(rd["CreatedAt"])
                        });
                    }
                }
            }

            return list;
        }

        public DynamicReportTemplateState GetTemplate(int templateId)
        {
            DynamicReportTemplateState state = new DynamicReportTemplateState();

            using (SqlConnection cn = new SqlConnection(_connStr))
            {
                cn.Open();

                using (SqlCommand cmd = new SqlCommand(@"
SELECT TemplateID, TemplateName, ReportKey
FROM dbo.ReportTemplates
WHERE TemplateID = @TemplateID;", cn))
                {
                    cmd.Parameters.Add("@TemplateID", SqlDbType.Int).Value = templateId;

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            state.TemplateID = Convert.ToInt32(rd["TemplateID"]);
                            state.TemplateName = Convert.ToString(rd["TemplateName"]);
                            state.ReportKey = Convert.ToString(rd["ReportKey"]);
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(@"
SELECT ColumnKey
FROM dbo.ReportTemplateColumns
WHERE TemplateID = @TemplateID
  AND IsVisible = 1
ORDER BY DisplayOrder;", cn))
                {
                    cmd.Parameters.Add("@TemplateID", SqlDbType.Int).Value = templateId;

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            state.ColumnKeys.Add(Convert.ToString(rd["ColumnKey"]));
                        }
                    }
                }

                using (SqlCommand cmd = new SqlCommand(@"
SELECT FilterKey, FilterValue
FROM dbo.ReportTemplateFilters
WHERE TemplateID = @TemplateID;", cn))
                {
                    cmd.Parameters.Add("@TemplateID", SqlDbType.Int).Value = templateId;

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            string key = Convert.ToString(rd["FilterKey"]);
                            string value = Convert.ToString(rd["FilterValue"]);

                            ApplyFilterToState(state, key, value);
                        }
                    }
                }
            }

            return state;
        }

        public int SaveTemplate(DynamicReportTemplateState state, string createdBy)
        {
            if (state == null)
                throw new InvalidOperationException("بيانات التصميم غير صحيحة.");

            if (string.IsNullOrWhiteSpace(state.TemplateName))
                throw new InvalidOperationException("أدخل اسم التصميم.");

            if (string.IsNullOrWhiteSpace(state.ReportKey))
                throw new InvalidOperationException("نوع التقرير غير محدد.");

            if (state.ColumnKeys == null || state.ColumnKeys.Count == 0)
                throw new InvalidOperationException("اختر عمودًا واحدًا على الأقل.");

            using (SqlConnection cn = new SqlConnection(_connStr))
            {
                cn.Open();

                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        int templateId;

                        using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.ReportTemplates
(
    TemplateName,
    ReportKey,
    CreatedBy,
    CreatedAt,
    IsActive
)
VALUES
(
    @TemplateName,
    @ReportKey,
    @CreatedBy,
    GETDATE(),
    1
);

SELECT CAST(SCOPE_IDENTITY() AS INT);", cn, tx))
                        {
                            cmd.Parameters.Add("@TemplateName", SqlDbType.NVarChar, 200).Value = state.TemplateName.Trim();
                            cmd.Parameters.Add("@ReportKey", SqlDbType.NVarChar, 100).Value = state.ReportKey;
                            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100).Value =
                                string.IsNullOrWhiteSpace(createdBy) ? (object)DBNull.Value : createdBy;

                            templateId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        for (int i = 0; i < state.ColumnKeys.Count; i++)
                        {
                            using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.ReportTemplateColumns
(
    TemplateID,
    ColumnKey,
    DisplayOrder,
    IsVisible
)
VALUES
(
    @TemplateID,
    @ColumnKey,
    @DisplayOrder,
    1
);", cn, tx))
                            {
                                cmd.Parameters.Add("@TemplateID", SqlDbType.Int).Value = templateId;
                                cmd.Parameters.Add("@ColumnKey", SqlDbType.NVarChar, 100).Value = state.ColumnKeys[i];
                                cmd.Parameters.Add("@DisplayOrder", SqlDbType.Int).Value = i + 1;

                                cmd.ExecuteNonQuery();
                            }
                        }

                        SaveFilter(cn, tx, templateId, "UseFromDate", state.UseFromDate ? "1" : "0");
                        SaveFilter(cn, tx, templateId, "UseToDate", state.UseToDate ? "1" : "0");

                        if (state.FromDate.HasValue)
                            SaveFilter(cn, tx, templateId, "FromDate", state.FromDate.Value.ToString("yyyy-MM-dd"));

                        if (state.ToDate.HasValue)
                            SaveFilter(cn, tx, templateId, "ToDate", state.ToDate.Value.ToString("yyyy-MM-dd"));

                        if (state.SubscriberID.HasValue)
                            SaveFilter(cn, tx, templateId, "SubscriberID", state.SubscriberID.Value.ToString());

                        if (!string.IsNullOrWhiteSpace(state.CategoryType))
                            SaveFilter(cn, tx, templateId, "CategoryType", state.CategoryType);

                        if (!string.IsNullOrWhiteSpace(state.SearchText))
                            SaveFilter(cn, tx, templateId, "SearchText", state.SearchText);

                        tx.Commit();
                        return templateId;
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        public void DeleteTemplate(int templateId)
        {
            using (SqlConnection cn = new SqlConnection(_connStr))
            {
                cn.Open();

                using (SqlTransaction tx = cn.BeginTransaction())
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(@"
DELETE FROM dbo.ReportTemplateFilters
WHERE TemplateID = @TemplateID;", cn, tx))
                        {
                            cmd.Parameters.Add("@TemplateID", SqlDbType.Int).Value = templateId;
                            cmd.ExecuteNonQuery();
                        }

                        using (SqlCommand cmd = new SqlCommand(@"
DELETE FROM dbo.ReportTemplateColumns
WHERE TemplateID = @TemplateID;", cn, tx))
                        {
                            cmd.Parameters.Add("@TemplateID", SqlDbType.Int).Value = templateId;
                            cmd.ExecuteNonQuery();
                        }

                        using (SqlCommand cmd = new SqlCommand(@"
DELETE FROM dbo.ReportTemplates
WHERE TemplateID = @TemplateID;", cn, tx))
                        {
                            cmd.Parameters.Add("@TemplateID", SqlDbType.Int).Value = templateId;
                            cmd.ExecuteNonQuery();
                        }

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }
        }

        private void SaveFilter(SqlConnection cn, SqlTransaction tx, int templateId, string key, string value)
        {
            using (SqlCommand cmd = new SqlCommand(@"
INSERT INTO dbo.ReportTemplateFilters
(
    TemplateID,
    FilterKey,
    FilterOperator,
    FilterValue
)
VALUES
(
    @TemplateID,
    @FilterKey,
    N'=',
    @FilterValue
);", cn, tx))
            {
                cmd.Parameters.Add("@TemplateID", SqlDbType.Int).Value = templateId;
                cmd.Parameters.Add("@FilterKey", SqlDbType.NVarChar, 100).Value = key;
                cmd.Parameters.Add("@FilterValue", SqlDbType.NVarChar, 500).Value =
                    string.IsNullOrWhiteSpace(value) ? (object)DBNull.Value : value;

                cmd.ExecuteNonQuery();
            }
        }

        private void ApplyFilterToState(DynamicReportTemplateState state, string key, string value)
        {
            if (key == "UseFromDate")
                state.UseFromDate = value == "1";

            else if (key == "UseToDate")
                state.UseToDate = value == "1";

            else if (key == "FromDate")
            {
                DateTime d;
                if (DateTime.TryParse(value, out d))
                    state.FromDate = d;
            }

            else if (key == "ToDate")
            {
                DateTime d;
                if (DateTime.TryParse(value, out d))
                    state.ToDate = d;
            }

            else if (key == "SubscriberID")
            {
                int id;
                if (int.TryParse(value, out id))
                    state.SubscriberID = id;
            }

            else if (key == "CategoryType")
                state.CategoryType = value;

            else if (key == "SearchText")
                state.SearchText = value;
        }

        private string EscapeAlias(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return value.Replace("]", "]]");
        }
    }
}