using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using water3.Models;
using water3.Models.Reports;
using water3.Services;
using water3.Utils;

namespace water3.Forms.Reports
{
    public partial class ExpenseCategorySummaryForm : Form
    {
        private readonly ExpenseReportsService _service = new ExpenseReportsService();

        private DateTimePicker dtFrom;
        private DateTimePicker dtTo;
        private ComboBox cboType;
        private Button btnLoad;
        private Button btnPrint;
        private DataGridView dgv;
        private Label lblStatus;
        private Label lblTotal;

        public ExpenseCategorySummaryForm()
        {
            BuildUi();

            if (IsInDesignMode())
                return;

            PermissionHelper.EnforceFormPermission(this, "EXPENSE_REPORTS_VIEW");
            PermissionHelper.ApplyControlPermission(btnPrint, "EXPENSE_REPORTS_PRINT");

            LoadData();
        }

        private bool IsInDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime
                   || DesignMode;
        }

        private void BuildUi()
        {
            Text = "إجمالي الحركات حسب التصنيف";
            StartPosition = FormStartPosition.CenterParent;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            WindowState = FormWindowState.Maximized;
            Font = new Font("Tahoma", 10F);
            BackColor = Color.White;

            Label lblTitle = new Label();
            lblTitle.Text = "إجمالي الحركات حسب التصنيف";
            lblTitle.Font = new Font("Tahoma", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(0, 102, 204);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 20);

            Panel pnl = new Panel();
            pnl.Location = new Point(20, 60);
            pnl.Size = new Size(1240, 80);
            pnl.BackColor = Color.FromArgb(248, 250, 252);
            pnl.BorderStyle = BorderStyle.FixedSingle;

            Label lblFrom = new Label();
            lblFrom.Text = "من تاريخ";
            lblFrom.AutoSize = true;
            lblFrom.Location = new Point(1120, 25);

            Label lblTo = new Label();
            lblTo.Text = "إلى تاريخ";
            lblTo.AutoSize = true;
            lblTo.Location = new Point(900, 25);

            Label lblType = new Label();
            lblType.Text = "النوع";
            lblType.AutoSize = true;
            lblType.Location = new Point(690, 25);

            dtFrom = new DateTimePicker();
            dtFrom.Location = new Point(990, 20);
            dtFrom.Size = new Size(120, 27);
            dtFrom.Format = DateTimePickerFormat.Short;
            dtFrom.Value = DateTime.Today.AddMonths(-1);

            dtTo = new DateTimePicker();
            dtTo.Location = new Point(770, 20);
            dtTo.Size = new Size(120, 27);
            dtTo.Format = DateTimePickerFormat.Short;
            dtTo.Value = DateTime.Today;

            cboType = new ComboBox();
            cboType.Location = new Point(500, 20);
            cboType.Size = new Size(170, 27);
            cboType.DropDownStyle = ComboBoxStyle.DropDownList;
            cboType.Items.AddRange(new object[] { "الكل", "Expense", "Purchase", "Loss" });
            cboType.SelectedIndex = 0;

            btnLoad = new Button();
            btnLoad.Text = "تحميل";
            btnLoad.Location = new Point(20, 18);
            btnLoad.Size = new Size(100, 30);
            btnLoad.BackColor = Color.SteelBlue;
            btnLoad.ForeColor = Color.White;
            btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.Click += BtnLoad_Click;

            btnPrint = new Button();
            btnPrint.Text = "طباعة";
            btnPrint.Location = new Point(130, 18);
            btnPrint.Size = new Size(100, 30);
            btnPrint.BackColor = Color.FromArgb(0, 122, 204);
            btnPrint.ForeColor = Color.White;
            btnPrint.FlatStyle = FlatStyle.Flat;
            btnPrint.Click += BtnPrint_Click;

            pnl.Controls.Add(lblFrom);
            pnl.Controls.Add(lblTo);
            pnl.Controls.Add(lblType);
            pnl.Controls.Add(dtFrom);
            pnl.Controls.Add(dtTo);
            pnl.Controls.Add(cboType);
            pnl.Controls.Add(btnLoad);
            pnl.Controls.Add(btnPrint);

            dgv = new DataGridView();
            dgv.Location = new Point(20, 155);
            dgv.Size = new Size(1240, 470);
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AutoGenerateColumns = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.FixedSingle;

            DataGridViewTextBoxColumn colCategory = new DataGridViewTextBoxColumn();
            colCategory.HeaderText = "التصنيف";
            colCategory.DataPropertyName = "Col1";
            colCategory.FillWeight = 180;

            DataGridViewTextBoxColumn colType = new DataGridViewTextBoxColumn();
            colType.HeaderText = "النوع";
            colType.DataPropertyName = "Col2";
            colType.FillWeight = 100;

            DataGridViewTextBoxColumn colCount = new DataGridViewTextBoxColumn();
            colCount.HeaderText = "عدد الحركات";
            colCount.DataPropertyName = "Num1";
            colCount.FillWeight = 90;
            colCount.DefaultCellStyle.Format = "N0";

            DataGridViewTextBoxColumn colTotal = new DataGridViewTextBoxColumn();
            colTotal.HeaderText = "إجمالي المبالغ";
            colTotal.DataPropertyName = "Num2";
            colTotal.FillWeight = 120;
            colTotal.DefaultCellStyle.Format = "N2";

            dgv.Columns.Add(colCategory);
            dgv.Columns.Add(colType);
            dgv.Columns.Add(colCount);
            dgv.Columns.Add(colTotal);

            lblStatus = new Label();
            lblStatus.Location = new Point(20, 640);
            lblStatus.Size = new Size(800, 30);
            lblStatus.TextAlign = ContentAlignment.MiddleRight;
            lblStatus.ForeColor = Color.DarkGreen;

            lblTotal = new Label();
            lblTotal.Location = new Point(850, 640);
            lblTotal.Size = new Size(410, 30);
            lblTotal.TextAlign = ContentAlignment.MiddleLeft;
            lblTotal.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(0, 102, 204);

            Controls.Add(lblTitle);
            Controls.Add(pnl);
            Controls.Add(dgv);
            Controls.Add(lblStatus);
            Controls.Add(lblTotal);
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                if (dtFrom == null || dtTo == null || cboType == null || dgv == null)
                    return;

                string type = cboType.SelectedIndex <= 0
                    ? null
                    : Convert.ToString(cboType.SelectedItem);

                List<ExpenseSummaryRow> rows =
                    _service.GetCategorySummary(dtFrom.Value, dtTo.Value, type);

                dgv.DataSource = null;
                dgv.DataSource = rows;

                lblStatus.ForeColor = Color.DarkGreen;
                lblStatus.Text = $"عدد التصنيفات: {rows.Count}";
                lblTotal.Text = $"الإجمالي العام: {rows.Sum(x => x.Num2):N2}";

                _service.LogReportOpen(
                    "ExpenseCategorySummary",
                    $"From={dtFrom.Value:yyyy-MM-dd}, To={dtTo.Value:yyyy-MM-dd}, Type={type}");
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.DarkRed;
                lblStatus.Text = ex.Message;
                lblTotal.Text = string.Empty;
            }
        }

        private void PrintSummaryReport()
        {
            try
            {
                List<ExpenseSummaryRow> rows = dgv.DataSource as List<ExpenseSummaryRow>;

                if (rows == null || rows.Count == 0)
                {
                    MessageBox.Show(
                        "لا توجد بيانات للطباعة.",
                        "تنبيه",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                string reportTitle = "إجمالي الحركات حسب التصنيف";
                string reportPath = @"Reports\RDLC\ExpenseCategorySummaryReport.rdlc";

                string fullReportPath = Path.Combine(Application.StartupPath, reportPath);

                if (!File.Exists(fullReportPath))
                {
                    fullReportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, reportPath);
                }

                if (!File.Exists(fullReportPath))
                {
                    MessageBox.Show(
                        "لم يتم العثور على ملف التقرير:\n" + fullReportPath,
                        "خطأ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                using (Form frm = new Form())
                {
                    frm.Text = reportTitle;
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.WindowState = FormWindowState.Maximized;
                    frm.RightToLeft = RightToLeft.Yes;
                    frm.RightToLeftLayout = true;

                    ReportViewer viewer = new ReportViewer();
                    viewer.Dock = DockStyle.Fill;
                    viewer.ProcessingMode = ProcessingMode.Local;

                    viewer.LocalReport.ReportPath = fullReportPath;
                    viewer.LocalReport.DataSources.Clear();

                    viewer.LocalReport.DataSources.Add(
                        new ReportDataSource("dsExpenseCategorySummary", rows)
                    );

                    string printedBy = string.Empty;

                    if (CurrentUser.IsLoggedIn)
                        printedBy = CurrentUser.FullName;

                    ReportParameter[] parameters =
                    {
                        new ReportParameter("pReportTitle", reportTitle),
                        new ReportParameter("pFromDate", dtFrom.Value.ToString("yyyy-MM-dd")),
                        new ReportParameter("pToDate", dtTo.Value.ToString("yyyy-MM-dd")),
                        new ReportParameter("pPrintedBy", printedBy),
                        new ReportParameter("pPrintedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    };

                    viewer.LocalReport.SetParameters(parameters);
                    viewer.RefreshReport();

                    frm.Controls.Add(viewer);
                    frm.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.DarkRed;
                lblStatus.Text = ex.Message;

                MessageBox.Show(
                    ex.Message,
                    "خطأ في عرض التقرير",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            PrintSummaryReport();
        }
    }
}