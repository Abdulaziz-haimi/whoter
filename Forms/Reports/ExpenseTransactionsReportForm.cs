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
    public partial class ExpenseTransactionsReportForm : Form
    {
        private ExpenseReportsService _service;
        private ExpenseService _expenseService;

        private string _reportType;
        private string _title;

        private DateTimePicker dtFrom;
        private DateTimePicker dtTo;
        private ComboBox cboCategory;
        private Button btnLoad;
        private Button btnPrint;
        private DataGridView dgv;
        private Label lblStatus;
        private Label lblTotal;

        private List<ExpenseCategoryItem> _categories = new List<ExpenseCategoryItem>();

        // Constructor فارغ حتى لا يتعطل Designer
        public ExpenseTransactionsReportForm()
            : this("Expense", "تقرير المصروفات")
        {
        }

        public ExpenseTransactionsReportForm(string reportType, string title)
        {
            _reportType = string.IsNullOrWhiteSpace(reportType) ? "Expense" : reportType;
            _title = string.IsNullOrWhiteSpace(title) ? "تقرير الحركات" : title;

            BuildUi();

            if (IsInDesignMode())
                return;

            _service = new ExpenseReportsService();
            _expenseService = new ExpenseService();

            PermissionHelper.EnforceFormPermission(this, "EXPENSE_REPORTS_VIEW");
            PermissionHelper.ApplyControlPermission(btnPrint, "EXPENSE_REPORTS_PRINT");

            LoadCategories();
            LoadData();
        }

        private bool IsInDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime || DesignMode;
        }

        private void BuildUi()
        {
            Text = _title;
            StartPosition = FormStartPosition.CenterParent;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            WindowState = FormWindowState.Maximized;
            Font = new Font("Tahoma", 10F);
            BackColor = Color.White;

            Label lblTitle = new Label();
            lblTitle.Text = _title;
            lblTitle.Font = new Font("Tahoma", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(0, 102, 204);
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 20);

            Panel pnl = new Panel();
            pnl.Location = new Point(20, 60);
            pnl.Size = new Size(1240, 80);
            pnl.BackColor = Color.FromArgb(248, 250, 252);
            pnl.BorderStyle = BorderStyle.FixedSingle;

            pnl.Controls.Add(MakeLabel("من تاريخ", 1120, 20));
            pnl.Controls.Add(MakeLabel("إلى تاريخ", 900, 20));
            pnl.Controls.Add(MakeLabel("التصنيف", 660, 20));

            dtFrom = new DateTimePicker();
            dtFrom.Location = new Point(990, 18);
            dtFrom.Size = new Size(120, 27);
            dtFrom.Format = DateTimePickerFormat.Short;
            dtFrom.Value = DateTime.Today.AddMonths(-1);

            dtTo = new DateTimePicker();
            dtTo.Location = new Point(770, 18);
            dtTo.Size = new Size(120, 27);
            dtTo.Format = DateTimePickerFormat.Short;
            dtTo.Value = DateTime.Today;

            cboCategory = new ComboBox();
            cboCategory.Location = new Point(390, 18);
            cboCategory.Size = new Size(250, 27);
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;

            btnLoad = MakeButton("تحميل", 20, 15, 100, Color.SteelBlue);
            btnPrint = MakeButton("طباعة", 130, 15, 100, Color.FromArgb(0, 122, 204));

            btnLoad.Click += BtnLoad_Click;
            btnPrint.Click += BtnPrint_Click;

            pnl.Controls.Add(dtFrom);
            pnl.Controls.Add(dtTo);
            pnl.Controls.Add(cboCategory);
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
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.FixedSingle;

            DataGridViewTextBoxColumn colDate = new DataGridViewTextBoxColumn();
            colDate.HeaderText = "التاريخ";
            colDate.DataPropertyName = "Dt1";
            colDate.FillWeight = 90;
            colDate.DefaultCellStyle.Format = "yyyy-MM-dd";

            DataGridViewTextBoxColumn colVoucher = new DataGridViewTextBoxColumn();
            colVoucher.HeaderText = "رقم السند";
            colVoucher.DataPropertyName = "Col1";
            colVoucher.FillWeight = 110;

            DataGridViewTextBoxColumn colCategory = new DataGridViewTextBoxColumn();
            colCategory.HeaderText = "التصنيف";
            colCategory.DataPropertyName = "Col2";
            colCategory.FillWeight = 130;

            DataGridViewTextBoxColumn colType = new DataGridViewTextBoxColumn();
            colType.HeaderText = "النوع";
            colType.DataPropertyName = "Col3";
            colType.FillWeight = 90;

            DataGridViewTextBoxColumn colSupplier = new DataGridViewTextBoxColumn();
            colSupplier.HeaderText = "المورد/الجهة";
            colSupplier.DataPropertyName = "Col4";
            colSupplier.FillWeight = 140;

            DataGridViewTextBoxColumn colStatement = new DataGridViewTextBoxColumn();
            colStatement.HeaderText = "البيان";
            colStatement.DataPropertyName = "Col5";
            colStatement.FillWeight = 220;

            DataGridViewTextBoxColumn colAmount = new DataGridViewTextBoxColumn();
            colAmount.HeaderText = "المبلغ";
            colAmount.DataPropertyName = "Num1";
            colAmount.FillWeight = 100;
            colAmount.DefaultCellStyle.Format = "N2";

            dgv.Columns.Add(colDate);
            dgv.Columns.Add(colVoucher);
            dgv.Columns.Add(colCategory);
            dgv.Columns.Add(colType);
            dgv.Columns.Add(colSupplier);
            dgv.Columns.Add(colStatement);
            dgv.Columns.Add(colAmount);

            lblStatus = new Label();
            lblStatus.Location = new Point(20, 640);
            lblStatus.Size = new Size(820, 30);
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

        private Label MakeLabel(string text, int left, int top)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.AutoSize = true;
            lbl.Location = new Point(left, top + 5);
            return lbl;
        }

        private Button MakeButton(string text, int left, int top, int width, Color color)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Location = new Point(left, top);
            btn.Size = new Size(width, 30);
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            return btn;
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadCategories()
        {
            try
            {
                if (_expenseService == null || cboCategory == null)
                    return;

                _categories = _expenseService.GetCategories()
                    .Where(x => string.Equals(x.CategoryType, _reportType, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                _categories.Insert(0, new ExpenseCategoryItem
                {
                    CategoryID = 0,
                    CategoryName = "الكل"
                });

                cboCategory.DataSource = null;
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "CategoryID";
                cboCategory.DataSource = _categories;
            }
            catch (Exception ex)
            {
                if (lblStatus != null)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }
        }

        private void LoadData()
        {
            try
            {
                if (_service == null || dtFrom == null || dtTo == null || cboCategory == null || dgv == null)
                    return;

                int selectedCategoryId = 0;

                if (cboCategory.SelectedValue != null && cboCategory.SelectedValue != DBNull.Value)
                    int.TryParse(cboCategory.SelectedValue.ToString(), out selectedCategoryId);

                List<ExpenseReportRow> rows = _service.GetExpenseReport(
                    dtFrom.Value,
                    dtTo.Value,
                    _reportType,
                    selectedCategoryId > 0 ? (int?)selectedCategoryId : null);

                dgv.DataSource = null;
                dgv.DataSource = rows;

                lblStatus.ForeColor = Color.DarkGreen;
                lblStatus.Text = $"عدد الحركات: {rows.Count}";
                lblTotal.Text = $"إجمالي المبالغ: {rows.Sum(x => x.Num1):N2}";

                _service.LogReportOpen(
                    _title,
                    $"From={dtFrom.Value:yyyy-MM-dd}, To={dtTo.Value:yyyy-MM-dd}, Type={_reportType}");
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.DarkRed;
                lblStatus.Text = ex.Message;
                lblTotal.Text = string.Empty;
            }
        }

        private void PrintTransactionsReport(string reportPath, string reportTitle)
        {
            try
            {
                if (dgv == null)
                    return;

                List<ExpenseReportRow> rows = dgv.DataSource as List<ExpenseReportRow>;

                if (rows == null || rows.Count == 0)
                {
                    MessageBox.Show(
                        "لا توجد بيانات للطباعة.",
                        "تنبيه",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

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
                        new ReportDataSource("dsExpenseTransactions", rows)
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
            if (string.Equals(_reportType, "Expense", StringComparison.OrdinalIgnoreCase))
            {
                PrintTransactionsReport(@"Reports\RDLC\ExpenseReport.rdlc", "تقرير المصروفات");
            }
            else if (string.Equals(_reportType, "Purchase", StringComparison.OrdinalIgnoreCase))
            {
                PrintTransactionsReport(@"Reports\RDLC\PurchaseReport.rdlc", "تقرير المشتريات");
            }
            else if (string.Equals(_reportType, "Loss", StringComparison.OrdinalIgnoreCase))
            {
                PrintTransactionsReport(@"Reports\RDLC\LossReport.rdlc", "تقرير الخسائر");
            }
        }
    }
}