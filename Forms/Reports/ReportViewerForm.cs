using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using water3.Models.Reports;

namespace water3.Forms.Reports
{
    public partial class ReportViewerForm : Form
    {

        //public class ReportViewerForm : Form
        //{
            private ReportViewer reportViewer;

            public ReportViewerForm(
                string reportTitle,
                string embeddedOrLocalReportPath,
                IEnumerable<ReportDataSource> dataSources,
                IEnumerable<ReportParameterItem> parameters = null,
                bool useEmbeddedReport = false)
            {
                InitializeComponent();
                Text = reportTitle;
                LoadReport(embeddedOrLocalReportPath, dataSources, parameters, useEmbeddedReport);
            }

            private void InitializeComponent()
            {
                StartPosition = FormStartPosition.CenterScreen;
                WindowState = FormWindowState.Maximized;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;

                reportViewer = new ReportViewer
                {
                    Dock = DockStyle.Fill,
                    ProcessingMode = ProcessingMode.Local,
                    ZoomMode = ZoomMode.PageWidth
                };

                Controls.Add(reportViewer);
            }

            private void LoadReport(
                string reportPath,
                IEnumerable<ReportDataSource> dataSources,
                IEnumerable<ReportParameterItem> parameters,
                bool useEmbeddedReport)
            {
                reportViewer.Reset();
                reportViewer.LocalReport.DataSources.Clear();

                if (useEmbeddedReport)
                    reportViewer.LocalReport.ReportEmbeddedResource = reportPath;
                else
                    reportViewer.LocalReport.ReportPath = reportPath;

                if (dataSources != null)
                {
                    foreach (var ds in dataSources)
                        reportViewer.LocalReport.DataSources.Add(ds);
                }

                if (parameters != null)
                {
                    var rParams = parameters
                        .Select(p => new ReportParameter(p.Name, p.Value ?? string.Empty))
                        .ToList();

                    reportViewer.LocalReport.SetParameters(rParams);
                }

                reportViewer.RefreshReport();
            }
        }
    }