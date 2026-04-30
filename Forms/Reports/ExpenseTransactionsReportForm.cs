using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using water3.Models;
using water3.Models.Reports;
using water3.Services;
using water3.Utils;

namespace water3.Forms.Reports
{
    public partial class ExpenseTransactionsReportForm : Form
    {

            private readonly ExpenseReportsService _service = new ExpenseReportsService();
            private readonly ExpenseService _expenseService = new ExpenseService();
            private readonly string _reportType;
            private readonly string _title;

            private DateTimePicker dtFrom;
            private DateTimePicker dtTo;
            private ComboBox cboCategory;
            private Button btnLoad;
            private Button btnPrint;
            private DataGridView dgv;
            private Label lblStatus;
            private Label lblTotal;
            private List<ExpenseCategoryItem> _categories = new List<ExpenseCategoryItem>();

            public ExpenseTransactionsReportForm(string reportType, string title)
            {
                _reportType = reportType;
                _title = title;
                InitializeComponent();
                PermissionHelper.EnforceFormPermission(this, "EXPENSE_REPORTS_VIEW");
                PermissionHelper.ApplyControlPermission(btnPrint, "EXPENSE_REPORTS_PRINT");
                LoadCategories();
                LoadData();
            }

            private void InitializeComponent()
            {
                Text = _title;
                StartPosition = FormStartPosition.CenterParent;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                WindowState = FormWindowState.Maximized;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;

                Controls.Add(new Label
                {
                    Text = _title,
                    Font = new Font("Tahoma", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204),
                    AutoSize = true,
                    Location = new Point(20, 20)
                });

                Panel pnl = new Panel
                {
                    Location = new Point(20, 60),
                    Size = new Size(1240, 80),
                    BackColor = Color.FromArgb(248, 250, 252),
                    BorderStyle = BorderStyle.FixedSingle
                };

                pnl.Controls.Add(MakeLabel("من تاريخ", 1120, 20));
                pnl.Controls.Add(MakeLabel("إلى تاريخ", 900, 20));
                pnl.Controls.Add(MakeLabel("التصنيف", 660, 20));

                dtFrom = new DateTimePicker
                {
                    Location = new Point(990, 18),
                    Size = new Size(120, 27),
                    Format = DateTimePickerFormat.Short,
                    Value = DateTime.Today.AddMonths(-1)
                };

                dtTo = new DateTimePicker
                {
                    Location = new Point(770, 18),
                    Size = new Size(120, 27),
                    Format = DateTimePickerFormat.Short,
                    Value = DateTime.Today
                };

                cboCategory = new ComboBox
                {
                    Location = new Point(390, 18),
                    Size = new Size(250, 27),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                btnLoad = MakeButton("تحميل", 20, 15, 100, Color.SteelBlue);
                btnPrint = MakeButton("طباعة", 130, 15, 100, Color.FromArgb(0, 122, 204));

                btnLoad.Click += (s, e) => LoadData();
                btnPrint.Click += BtnPrint_Click;

                pnl.Controls.AddRange(new Control[] { dtFrom, dtTo, cboCategory, btnLoad, btnPrint });

                dgv = new DataGridView
                {
                    Location = new Point(20, 155),
                    Size = new Size(1240, 470),
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AutoGenerateColumns = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "التاريخ", DataPropertyName = "Dt1", FillWeight = 90, DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" } });
                dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "رقم السند", DataPropertyName = "Col1", FillWeight = 110 });
                dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "التصنيف", DataPropertyName = "Col2", FillWeight = 130 });
                dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "النوع", DataPropertyName = "Col3", FillWeight = 90 });
                dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "المورد/الجهة", DataPropertyName = "Col4", FillWeight = 140 });
                dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "البيان", DataPropertyName = "Col5", FillWeight = 220 });
                dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "المبلغ", DataPropertyName = "Num1", FillWeight = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });

                lblStatus = new Label
                {
                    Location = new Point(20, 640),
                    Size = new Size(820, 30),
                    TextAlign = ContentAlignment.MiddleRight,
                    ForeColor = Color.DarkGreen
                };

                lblTotal = new Label
                {
                    Location = new Point(850, 640),
                    Size = new Size(410, 30),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Tahoma", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204)
                };

                Controls.AddRange(new Control[] { pnl, dgv, lblStatus, lblTotal });
            }

            private Label MakeLabel(string text, int left, int top)
            {
                return new Label { Text = text, AutoSize = true, Location = new Point(left, top + 5) };
            }

            private Button MakeButton(string text, int left, int top, int width, Color color)
            {
                return new Button
                {
                    Text = text,
                    Location = new Point(left, top),
                    Size = new Size(width, 30),
                    BackColor = color,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
            }

        private void LoadCategories()
        {
            _categories = _expenseService.GetCategories()
                .Where(x => string.Equals(x.CategoryType, _reportType, StringComparison.OrdinalIgnoreCase))
                .ToList();

            _categories.Insert(0, new ExpenseCategoryItem
            {
                CategoryID = 0,
                CategoryName = "الكل"
            });

            cboCategory.DataSource = null;
            cboCategory.DataSource = _categories;
            cboCategory.DisplayMember = "CategoryName";
            cboCategory.ValueMember = "CategoryID";
        }

        private void LoadData()
            {
                try
                {
                    int selectedCategoryId = cboCategory.SelectedValue is int ? (int)cboCategory.SelectedValue : 0;
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
                    _service.LogReportOpen(_title, $"From={dtFrom.Value:yyyy-MM-dd}, To={dtTo.Value:yyyy-MM-dd}, Type={_reportType}");
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
                List<ExpenseReportRow> rows = dgv.DataSource as List<ExpenseReportRow>;

                if (rows == null || rows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات للطباعة.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

                    ReportParameter[] parameters =
                    {
                new ReportParameter("pReportTitle", reportTitle),
                new ReportParameter("pFromDate", dtFrom.Value.ToString("yyyy-MM-dd")),
                new ReportParameter("pToDate", dtTo.Value.ToString("yyyy-MM-dd")),
                new ReportParameter("pPrintedBy", CurrentUser.IsLoggedIn ? CurrentUser.FullName : string.Empty),
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
                if (_reportType == "Expense")
                    PrintTransactionsReport(@"Reports\RDLC\ExpenseReport.rdlc", "تقرير المصروفات");
                else if (_reportType == "Purchase")
                    PrintTransactionsReport(@"Reports\RDLC\PurchaseReport.rdlc", "تقرير المشتريات");
                else if (_reportType == "Loss")
                    PrintTransactionsReport(@"Reports\RDLC\LossReport.rdlc", "تقرير الخسائر");
            }
        }
    }
