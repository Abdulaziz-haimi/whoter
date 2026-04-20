using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace water3.Reports
{
    public static class ReportRunner
    {
        public static void Show(
            ReportViewer viewer,
            string rdlcPath,
            DataTable sourceData,
            ReportPresetOptions opt,
            DateTime fromDate,
            DateTime toDate,
            string userName)
        {
            if (viewer == null) throw new ArgumentNullException("viewer");
            if (string.IsNullOrWhiteSpace(rdlcPath)) throw new ArgumentNullException("rdlcPath");
            if (!File.Exists(rdlcPath)) throw new FileNotFoundException("RDLC file not found", rdlcPath);

            if (opt == null) opt = new ReportPresetOptions();

            sourceData = ReportDataMapper.EnsureSchema(sourceData);
            DataTable displayData = ReportDataMapper.RemapForDisplay(sourceData, opt);

            viewer.Reset();
            viewer.ProcessingMode = ProcessingMode.Local;
            viewer.LocalReport.ReportPath = rdlcPath;

            viewer.LocalReport.DataSources.Clear();
            viewer.LocalReport.DataSources.Add(new ReportDataSource("dsReport", displayData));

            viewer.LocalReport.EnableExternalImages = true;

            List<ReportParameter> parameters = BuildParameters(opt, fromDate, toDate, userName);
            viewer.LocalReport.SetParameters(parameters);

            viewer.RefreshReport();
        }

        private static List<ReportParameter> BuildParameters(ReportPresetOptions opt, DateTime fromDate, DateTime toDate, string userName)
        {
            string logoUri = "";
            if (!string.IsNullOrWhiteSpace(opt.LogoPath))
            {
                logoUri = new Uri(opt.LogoPath).AbsoluteUri;
            }

            List<ReportParameter> ps = new List<ReportParameter>();
            ps.Add(new ReportParameter("pTitle", opt.Title ?? ""));
            ps.Add(new ReportParameter("pSubTitle", opt.SubTitle ?? ""));
            ps.Add(new ReportParameter("pFrom", fromDate.ToString("yyyy-MM-dd")));
            ps.Add(new ReportParameter("pTo", toDate.ToString("yyyy-MM-dd")));
            ps.Add(new ReportParameter("pUser", userName ?? ""));
            ps.Add(new ReportParameter("pLogoPath", logoUri));
            ps.Add(new ReportParameter("pFooterNote", opt.FooterNote ?? ""));
            ps.Add(new ReportParameter("pShowTotals", opt.ShowTotals.ToString()));

            // defaults: اخفاء
            for (int i = 1; i <= 10; i++)
            {
                ps.Add(new ReportParameter("pShowCol" + i, "False"));
                ps.Add(new ReportParameter("pCapCol" + i, "Col" + i));
            }
            for (int i = 1; i <= 5; i++)
            {
                ps.Add(new ReportParameter("pShowNum" + i, "False"));
                ps.Add(new ReportParameter("pCapNum" + i, "Num" + i));
            }

            // Remap => نعرض بالتسلسل
            int colIndex = 1, numIndex = 1;
            foreach (var c in (opt.Columns ?? new List<ReportPresetOptions.ColumnOption>()))
            {
                if (!c.Visible) continue;

                string caption = string.IsNullOrWhiteSpace(c.Caption) ? (c.Key ?? "") : c.Caption;

                if ((c.Key ?? "").StartsWith("Col", StringComparison.OrdinalIgnoreCase) && colIndex <= 10)
                {
                    Replace(ps, "pShowCol" + colIndex, "True");
                    Replace(ps, "pCapCol" + colIndex, caption);
                    colIndex++;
                }
                else if ((c.Key ?? "").StartsWith("Num", StringComparison.OrdinalIgnoreCase) && numIndex <= 5)
                {
                    Replace(ps, "pShowNum" + numIndex, "True");
                    Replace(ps, "pCapNum" + numIndex, caption);
                    numIndex++;
                }
            }

            return ps;
        }

        private static void Replace(List<ReportParameter> ps, string name, string value)
        {
            for (int i = ps.Count - 1; i >= 0; i--)
            {
                if (string.Equals(ps[i].Name, name, StringComparison.OrdinalIgnoreCase))
                    ps.RemoveAt(i);
            }
            ps.Add(new ReportParameter(name, value));
        }
    }
}
