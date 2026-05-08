using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using water3.Models.Reports;

namespace water3.Forms.Reports
{
    public partial class ReportViewerForm : Form
    {
        private ReportViewer reportViewer;

        // Constructor فارغ حتى لا يتعطل Designer
        public ReportViewerForm()
        {
            BuildUi();
        }

        public ReportViewerForm(
            string reportTitle,
            string embeddedOrLocalReportPath,
            IEnumerable<ReportDataSource> dataSources,
            IEnumerable<ReportParameterItem> parameters = null,
            bool useEmbeddedReport = false)
        {
            BuildUi();

            Text = string.IsNullOrWhiteSpace(reportTitle)
                ? "معاينة التقرير"
                : reportTitle;

            if (IsInDesignMode())
                return;

            LoadReport(
                embeddedOrLocalReportPath,
                dataSources,
                parameters,
                useEmbeddedReport);
        }

        private bool IsInDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime
                   || DesignMode;
        }

        private void BuildUi()
        {
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            Font = new Font("Tahoma", 10F);
            BackColor = Color.White;

            reportViewer = new ReportViewer();
            reportViewer.Dock = DockStyle.Fill;
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.ZoomMode = ZoomMode.PageWidth;

            Controls.Add(reportViewer);
        }

        private void LoadReport(
     string reportPath,
     IEnumerable<ReportDataSource> dataSources,
     IEnumerable<ReportParameterItem> parameters,
     bool useEmbeddedReport)
        {
            try
            {
                if (reportViewer == null)
                    return;

                if (string.IsNullOrWhiteSpace(reportPath))
                {
                    MessageBox.Show(
                        "مسار التقرير غير محدد.",
                        "تنبيه",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                reportViewer.Reset();
                reportViewer.LocalReport.DataSources.Clear();

                if (useEmbeddedReport)
                {
                    reportViewer.LocalReport.ReportEmbeddedResource = reportPath;
                }
                else
                {
                    string fullPath = reportPath;

                    if (!System.IO.Path.IsPathRooted(fullPath))
                    {
                        fullPath = System.IO.Path.Combine(
                            Application.StartupPath,
                            reportPath);
                    }

                    if (!System.IO.File.Exists(fullPath))
                    {
                        MessageBox.Show(
                            "لم يتم العثور على ملف التقرير:\n" + fullPath,
                            "خطأ",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        return;
                    }

                    reportViewer.LocalReport.ReportPath = fullPath;
                }

                if (dataSources != null)
                {
                    foreach (ReportDataSource ds in dataSources)
                    {
                        if (ds != null)
                            reportViewer.LocalReport.DataSources.Add(ds);
                    }
                }

                if (parameters != null)
                {
                    List<ReportParameter> rParams = parameters
                        .Where(p => p != null && !string.IsNullOrWhiteSpace(p.Name))
                        .Select(p => new ReportParameter(p.Name, p.Value ?? string.Empty))
                        .ToList();

                    if (rParams.Count > 0)
                        reportViewer.LocalReport.SetParameters(rParams);
                }

                reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                string msg = ex.Message;

                if (ex.InnerException != null)
                    msg += "\n\nInner Error:\n" + ex.InnerException.Message;

                if (ex.InnerException != null && ex.InnerException.InnerException != null)
                    msg += "\n\nInner Inner Error:\n" + ex.InnerException.InnerException.Message;

                MessageBox.Show(
                    msg,
                    "خطأ في معالجة التقرير",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}