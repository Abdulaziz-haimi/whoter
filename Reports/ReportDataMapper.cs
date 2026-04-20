using System;
using System.Data;
using System.Linq;

namespace water3.Reports
{
    public static class ReportDataMapper
    {
        public static DataTable EnsureSchema(DataTable dt)
        {
            if (dt == null) dt = new DataTable();

            Add(dt, "Dt1", typeof(DateTime));
            for (int i = 1; i <= 10; i++) Add(dt, "Col" + i, typeof(string));
            for (int i = 1; i <= 5; i++) Add(dt, "Num" + i, typeof(decimal));
            Add(dt, "RowId", typeof(int));
            Add(dt, "RefId1", typeof(int));
            Add(dt, "RefId2", typeof(int));

            return dt;
        }

        public static DataTable RemapForDisplay(DataTable source, ReportPresetOptions opt)
        {
            source = EnsureSchema(source);
            if (opt == null) opt = new ReportPresetOptions();

            // clone + ensure
            DataTable output = source.Clone();
            output = EnsureSchema(output);

            var ordered = (opt.Columns ?? Enumerable.Empty<ReportPresetOptions.ColumnOption>())
                .Where(x => x.Visible && !string.IsNullOrWhiteSpace(x.Key))
                .OrderBy(x => x.Order)
                .ToList();

            foreach (DataRow r in source.Rows)
            {
                DataRow nr = output.NewRow();

                nr["Dt1"] = r["Dt1"];
                nr["RowId"] = r["RowId"];
                nr["RefId1"] = r["RefId1"];
                nr["RefId2"] = r["RefId2"];

                int colIndex = 1;
                int numIndex = 1;

                foreach (var c in ordered)
                {
                    string k = (c.Key ?? "").Trim();
                    if (!source.Columns.Contains(k)) continue;

                    if (k.StartsWith("Col", StringComparison.OrdinalIgnoreCase) && colIndex <= 10)
                    {
                        nr["Col" + colIndex] = r[k] ?? DBNull.Value;
                        colIndex++;
                    }
                    else if (k.StartsWith("Num", StringComparison.OrdinalIgnoreCase) && numIndex <= 5)
                    {
                        nr["Num" + numIndex] = (r[k] == DBNull.Value) ? 0m : r[k];
                        numIndex++;
                    }
                }

                output.Rows.Add(nr);
            }

            return output;
        }

        private static void Add(DataTable dt, string name, Type type)
        {
            if (!dt.Columns.Contains(name))
                dt.Columns.Add(name, type);
        }
    }
}
