using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using water3.Models;
using water3.Models.Reports;
using System.Data;
using System.Reflection;
using System.Collections;
using Microsoft.Reporting.WinForms;
using water3.Forms.Reports;
namespace water3.Reports
{
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

            dtFrom = new DateTimePicker();
            dtFrom.Location = new Point(950, 18);
            dtFrom.Size = new Size(150, 28);
            dtFrom.Format = DateTimePickerFormat.Short;
            dtFrom.Value = DateTime.Today.AddMonths(-1);

            dtTo = new DateTimePicker();
            dtTo.Location = new Point(680, 18);
            dtTo.Size = new Size(150, 28);
            dtTo.Format = DateTimePickerFormat.Short;
            dtTo.Value = DateTime.Today;

            cboSubscribers = new ComboBox();
            cboSubscribers.Location = new Point(260, 18);
            cboSubscribers.Size = new Size(300, 28);
            cboSubscribers.DropDownStyle = ComboBoxStyle.DropDownList;

            pnlFilters.Controls.Add(dtFrom);
            pnlFilters.Controls.Add(dtTo);
            pnlFilters.Controls.Add(cboSubscribers);
        }

        private void LoadSubscribers()
        {
            try
            {
                _subs = ReportsService.GetSubscribers();

                if (_subs == null)
                    _subs = new List<SubscriberLookupItem>();

                _subs.Insert(0, new SubscriberLookupItem
                {
                    SubscriberID = 0,
                    Name = "الكل"
                });

                cboSubscribers.DataSource = null;
                cboSubscribers.DisplayMember = "Name";
                cboSubscribers.ValueMember = "SubscriberID";
                cboSubscribers.DataSource = _subs;
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.DarkRed;
                lblStatus.Text = ex.Message;
            }
        }

        protected override void LoadReportData()
        {
            try
            {
                int selectedId = 0;

                if (cboSubscribers.SelectedValue != null &&
                    cboSubscribers.SelectedValue != DBNull.Value)
                {
                    int.TryParse(cboSubscribers.SelectedValue.ToString(), out selectedId);
                }

                dgvReport.DataSource = null;

                dgvReport.DataSource = ReportsService.GetInvoicesReport(
                    dtFrom.Value,
                    dtTo.Value,
                    selectedId > 0 ? (int?)selectedId : null);

                lblStatus.ForeColor = Color.DarkGreen;
                lblStatus.Text = "تم تحميل تقرير الفواتير.";

                ReportsService.LogReportOpen(
                    "InvoicesReport",
                    $"From={dtFrom.Value:yyyy-MM-dd}, To={dtTo.Value:yyyy-MM-dd}, SubscriberID={selectedId}");
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.DarkRed;
                lblStatus.Text = ex.Message;
            }
        }

        protected override void RefreshReport()
        {
            dtFrom.Value = DateTime.Today.AddMonths(-1);
            dtTo.Value = DateTime.Today;

            if (cboSubscribers.Items.Count > 0)
                cboSubscribers.SelectedIndex = 0;

            dgvReport.DataSource = null;

            lblStatus.ForeColor = Color.DarkGreen;
            lblStatus.Text = "تمت إعادة التهيئة.";
        }

        protected override void PrintCurrentReport()
        {
            try
            {
                if (dgvReport.DataSource == null)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = "لا توجد بيانات للطباعة.";
                    return;
                }

                DataTable reportTable = BuildInvoicesRdlcTable(dgvReport.DataSource);

                if (reportTable.Rows.Count == 0)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = "لا توجد بيانات للطباعة.";
                    return;
                }

                List<ReportDataSource> dataSources = new List<ReportDataSource>
        {
            new ReportDataSource("dsInvoices", reportTable)
        };

                string printedBy = string.Empty;

                if (CurrentUser.IsLoggedIn)
                    printedBy = CurrentUser.FullName;

                List<ReportParameterItem> parameters = new List<ReportParameterItem>
        {
            new ReportParameterItem { Name = "pReportTitle", Value = "تقرير الفواتير" },
            new ReportParameterItem { Name = "pFromDate", Value = dtFrom.Value.ToString("yyyy-MM-dd") },
            new ReportParameterItem { Name = "pToDate", Value = dtTo.Value.ToString("yyyy-MM-dd") },
            new ReportParameterItem { Name = "pPrintedBy", Value = printedBy },
            new ReportParameterItem { Name = "pPrintedAt", Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
        };

                using (water3.Forms.Reports.ReportViewerForm frm =
                       new water3.Forms.Reports.ReportViewerForm(
                           "تقرير الفواتير",
                           @"Reports\RDLC\InvoiceReport.rdlc",
                           dataSources,
                           parameters,
                           false))
                {
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.DarkRed;
                lblStatus.Text = ex.Message;

                MessageBox.Show(
                    ex.Message,
                    "خطأ في طباعة التقرير",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private DataTable BuildInvoicesRdlcTable(object dataSource)
        {
            DataTable dt = new DataTable("dsInvoices");

            dt.Columns.Add("Dt1", typeof(DateTime));
            dt.Columns.Add("Col1", typeof(string));
            dt.Columns.Add("Col2", typeof(string));
            dt.Columns.Add("Col3", typeof(string));
            dt.Columns.Add("Col4", typeof(string));
            dt.Columns.Add("Col5", typeof(string));

            dt.Columns.Add("Num1", typeof(decimal));
            dt.Columns.Add("Num2", typeof(decimal));
            dt.Columns.Add("Num3", typeof(decimal));
            dt.Columns.Add("Num4", typeof(decimal));
            dt.Columns.Add("Num5", typeof(decimal));

            dt.Columns.Add("RowId", typeof(int));
            dt.Columns.Add("RefId1", typeof(int));
            dt.Columns.Add("RefId2", typeof(int));

            IEnumerable items = dataSource as IEnumerable;

            if (items == null)
                return dt;

            foreach (object item in items)
            {
                if (item == null)
                    continue;

                DataRow row = dt.NewRow();

                row["Dt1"] = GetDateValue(item, "Dt1");
                row["Col1"] = GetStringValue(item, "Col1");
                row["Col2"] = GetStringValue(item, "Col2");
                row["Col3"] = GetStringValue(item, "Col3");
                row["Col4"] = GetStringValue(item, "Col4");
                row["Col5"] = GetStringValue(item, "Col5");

                row["Num1"] = GetDecimalValue(item, "Num1");
                row["Num2"] = GetDecimalValue(item, "Num2");
                row["Num3"] = GetDecimalValue(item, "Num3");
                row["Num4"] = GetDecimalValue(item, "Num4");
                row["Num5"] = GetDecimalValue(item, "Num5");

                row["RowId"] = GetIntValue(item, "RowId");
                row["RefId1"] = GetIntValue(item, "RefId1");
                row["RefId2"] = GetIntValue(item, "RefId2");

                dt.Rows.Add(row);
            }

            return dt;
        }

        private object GetPropertyValue(object item, string propertyName)
        {
            if (item == null)
                return null;

            PropertyInfo prop = item.GetType().GetProperty(propertyName);

            if (prop == null)
                return null;

            return prop.GetValue(item, null);
        }

        private string GetStringValue(object item, string propertyName)
        {
            object value = GetPropertyValue(item, propertyName);

            if (value == null || value == DBNull.Value)
                return string.Empty;

            return Convert.ToString(value);
        }

        private decimal GetDecimalValue(object item, string propertyName)
        {
            object value = GetPropertyValue(item, propertyName);

            if (value == null || value == DBNull.Value)
                return 0m;

            decimal result;

            if (decimal.TryParse(Convert.ToString(value), out result))
                return result;

            return 0m;
        }

        private int GetIntValue(object item, string propertyName)
        {
            object value = GetPropertyValue(item, propertyName);

            if (value == null || value == DBNull.Value)
                return 0;

            int result;

            if (int.TryParse(Convert.ToString(value), out result))
                return result;

            return 0;
        }

        private object GetDateValue(object item, string propertyName)
        {
            object value = GetPropertyValue(item, propertyName);

            if (value == null || value == DBNull.Value)
                return DBNull.Value;

            DateTime result;

            if (DateTime.TryParse(Convert.ToString(value), out result))
                return result;

            return DBNull.Value;
        }
    }
}