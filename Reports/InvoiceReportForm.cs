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
using System.Windows.Forms;
using water3.Models;
namespace water3.Reports
{
    //public partial class InvoiceReportForm : Form
    //{

        public partial class InvoiceReportForm : BaseReportForm
        {
            private DateTimePicker dtFrom;
            private DateTimePicker dtTo;
            private ComboBox cboSubscribers;
            private List<SubscriberLookupItem> _subs;

            public InvoiceReportForm()
            {
                Text = "تقرير الفواتير";
                lblTitle.Text = Text;
                ApplyReportPermission("REPORT_INVOICES_VIEW");
                BuildFilters();
                LoadSubscribers();
            }

            private void BuildFilters()
            {
                pnlFilters.Controls.Add(MakeFilterLabel("من تاريخ", 1120, 20));
                pnlFilters.Controls.Add(MakeFilterLabel("إلى تاريخ", 850, 20));
                pnlFilters.Controls.Add(MakeFilterLabel("المشترك", 580, 20));

                dtFrom = new DateTimePicker { Location = new System.Drawing.Point(950, 18), Size = new System.Drawing.Size(150, 28), Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddMonths(-1) };
                dtTo = new DateTimePicker { Location = new System.Drawing.Point(680, 18), Size = new System.Drawing.Size(150, 28), Format = DateTimePickerFormat.Short, Value = DateTime.Today };
                cboSubscribers = new ComboBox { Location = new System.Drawing.Point(260, 18), Size = new System.Drawing.Size(300, 28), DropDownStyle = ComboBoxStyle.DropDownList };

                pnlFilters.Controls.AddRange(new Control[] { dtFrom, dtTo, cboSubscribers });
            }

            private void LoadSubscribers()
            {
                _subs = ReportsService.GetSubscribers();
                _subs.Insert(0, new SubscriberLookupItem { SubscriberID = 0, Name = "الكل" });
                cboSubscribers.DataSource = _subs;
                cboSubscribers.DisplayMember = "Name";
                cboSubscribers.ValueMember = "SubscriberID";
            }

            //protected override void LoadReportData()
            //{
            //    try
            //    {
            //        int selectedId = cboSubscribers.SelectedValue is int ? (int)cboSubscribers.SelectedValue : 0;
            //        dgvReport.DataSource = ReportsService.GetInvoicesReport(dtFrom.Value, dtTo.Value, selectedId > 0 ? (int?)selectedId : null);
            //        lblStatus.Text = "تم تحميل تقرير الفواتير.";
            //        ReportsService.LogReportOpen("InvoicesReport", $"From={dtFrom.Value:yyyy-MM-dd}, To={dtTo.Value:yyyy-MM-dd}");
            //    }
            //    catch (Exception ex)
            //    {
            //        lblStatus.ForeColor = System.Drawing.Color.DarkRed;
            //        lblStatus.Text = ex.Message;
            //    }
            //}

            protected override void RefreshReport()
            {
                dtFrom.Value = DateTime.Today.AddMonths(-1);
                dtTo.Value = DateTime.Today;
                cboSubscribers.SelectedIndex = 0;
                dgvReport.DataSource = null;
                lblStatus.ForeColor = System.Drawing.Color.DarkGreen;
                lblStatus.Text = "تمت إعادة التهيئة.";
            }
        protected override void PrintCurrentReport()
        {
            try
            {
                var rows = dgvReport.DataSource as System.Collections.IEnumerable;
                if (rows == null)
                {
                    lblStatus.ForeColor = System.Drawing.Color.DarkRed;
                    lblStatus.Text = "لا توجد بيانات للطباعة.";
                    return;
                }

                var data = dgvReport.DataSource;

                var dataSources = new List<Microsoft.Reporting.WinForms.ReportDataSource>
        {
            new Microsoft.Reporting.WinForms.ReportDataSource("dsInvoices", data)
        };

                var parameters = new List<water3.Models.Reports.ReportParameterItem>
        {
            new water3.Models.Reports.ReportParameterItem { Name = "pReportTitle", Value = "تقرير الفواتير" },
            new water3.Models.Reports.ReportParameterItem { Name = "pFromDate", Value = dtFrom.Value.ToString("yyyy-MM-dd") },
            new water3.Models.Reports.ReportParameterItem { Name = "pToDate", Value = dtTo.Value.ToString("yyyy-MM-dd") },
            new water3.Models.Reports.ReportParameterItem { Name = "pPrintedBy", Value = CurrentUser.FullName },
            new water3.Models.Reports.ReportParameterItem { Name = "pPrintedAt", Value = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
        };
            }
            //    PrintService.ShowRdlcReport(
            //        title: "تقرير الفواتير",
            //        reportPath: @"Reports\RDLC\InvoiceReport.rdlc",
            //        dataSources: dataSources,
            //        parameters: parameters,
            //        useEmbeddedReport: false);
            //}
            catch (System.Exception ex)
            {
                lblStatus.ForeColor = System.Drawing.Color.DarkRed;
                lblStatus.Text = ex.Message;
            }
        }

    }
}