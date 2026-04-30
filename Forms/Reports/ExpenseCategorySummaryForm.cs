using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
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
                InitializeComponent();
                PermissionHelper.EnforceFormPermission(this, "EXPENSE_REPORTS_VIEW");
                PermissionHelper.ApplyControlPermission(btnPrint, "EXPENSE_REPORTS_PRINT");
                LoadData();
            }

            private void InitializeComponent()
            {
                Text = "إجمالي الحركات حسب التصنيف";
                StartPosition = FormStartPosition.CenterParent;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                WindowState = FormWindowState.Maximized;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;

                Controls.Add(new Label
                {
                    Text = "إجمالي الحركات حسب التصنيف",
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

                pnl.Controls.Add(new Label { Text = "من تاريخ", AutoSize = true, Location = new Point(1120, 25) });
                pnl.Controls.Add(new Label { Text = "إلى تاريخ", AutoSize = true, Location = new Point(900, 25) });
                pnl.Controls.Add(new Label { Text = "النوع", AutoSize = true, Location = new Point(690, 25) });

                dtFrom = new DateTimePicker
                {
                    Location = new Point(990, 20),
                    Size = new Size(120, 27),
                    Format = DateTimePickerFormat.Short,
                    Value = DateTime.Today.AddMonths(-1)
                };

                dtTo = new DateTimePicker
                {
                    Location = new Point(770, 20),
                    Size = new Size(120, 27),
                    Format = DateTimePickerFormat.Short,
                    Value = DateTime.Today
                };

                cboType = new ComboBox
                {
                    Location = new Point(500, 20),
                    Size = new Size(170, 27),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                cboType.Items.AddRange(new object[] { "الكل", "Expense", "Purchase", "Loss" });
                cboType.SelectedIndex = 0;

                btnLoad = new Button
                {
                    Text = "تحميل",
                    Location = new Point(20, 18),
                    Size = new Size(100, 30),
                    BackColor = Color.SteelBlue,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                btnPrint = new Button
                {
                    Text = "طباعة",
                    Location = new Point(130, 18),
                    Size = new Size(100, 30),
                    BackColor = Color.FromArgb(0, 122, 204),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                btnLoad.Click += (s, e) => LoadData();
                btnPrint.Click += BtnPrint_Click;

                pnl.Controls.AddRange(new Control[] { dtFrom, dtTo, cboType, btnLoad, btnPrint });

                dgv = new DataGridView
                {
                    Location = new Point(20, 155),
                    Size = new Size(1240, 470),
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AutoGenerateColumns = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };

                dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "التصنيف", DataPropertyName = "Col1", FillWeight = 180 });
                dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "النوع", DataPropertyName = "Col2", FillWeight = 100 });
                dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "عدد الحركات", DataPropertyName = "Num1", FillWeight = 90, DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
                dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "إجمالي المبالغ", DataPropertyName = "Num2", FillWeight = 120, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });

                lblStatus = new Label
                {
                    Location = new Point(20, 640),
                    Size = new Size(800, 30),
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

            private void LoadData()
            {
                try
                {
                    string type = cboType.SelectedIndex <= 0 ? null : Convert.ToString(cboType.SelectedItem);
                    List<ExpenseSummaryRow> rows = _service.GetCategorySummary(dtFrom.Value, dtTo.Value, type);
                    dgv.DataSource = null;
                    dgv.DataSource = rows;

                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = $"عدد التصنيفات: {rows.Count}";
                    lblTotal.Text = $"الإجمالي العام: {rows.Sum(x => x.Num2):N2}";
                    _service.LogReportOpen("ExpenseCategorySummary", $"From={dtFrom.Value:yyyy-MM-dd}, To={dtTo.Value:yyyy-MM-dd}, Type={type}");
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
                    MessageBox.Show("لا توجد بيانات للطباعة.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                PrintSummaryReport();
            }
        }
    }
