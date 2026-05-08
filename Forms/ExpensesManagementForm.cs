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
namespace water3.Forms
{
    public partial class ExpensesManagementForm : Form
    {
    



        /*
         * هذه نسخة عملية مدمجة بالتقارير والطباعة.
         * إذا كان لديك ExpensesManagementForm سابقًا داخل المشروع،
         * يمكنك إما استبداله بالكامل بهذه النسخة، أو نسخ الأجزاء التالية منه:
         * - btnVoucher
         * - ApplyPermissions()
         * - PrintVoucher()
         * - BtnVoucher_Click()
         */
   
            private readonly ExpenseService _service = new ExpenseService();
            private readonly ExpenseReportsService _reportsService = new ExpenseReportsService();
            private List<ExpenseCategoryItem> _categories = new List<ExpenseCategoryItem>();
            private List<ExpenseHeaderItem> _expenses = new List<ExpenseHeaderItem>();

            private DateTimePicker dtFrom;
            private DateTimePicker dtTo;
            private ComboBox cboType;
            private ComboBox cboCategory;
            private Button btnSearch;
            private Button btnRefresh;
            private Button btnNew;
            private Button btnEdit;
            private Button btnView;
            private Button btnDelete;
            private Button btnVoucher;
            private DataGridView dgvExpenses;
            private Label lblStatus;
            private Label lblTotal;
            private Panel pnlFilters;

            public ExpensesManagementForm()
            {
                InitializeComponent();
                PermissionHelper.EnforceFormPermission(this, "EXPENSES_VIEW");
                ApplyPermissions();
                LoadCategories();
                LoadExpenses();
            }

            private void InitializeComponent()
            {
                Text = "المصروفات والمشتريات والمخاسير";
                StartPosition = FormStartPosition.CenterScreen;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                WindowState = FormWindowState.Maximized;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;

                Label lblTitle = new Label
                {
                    Text = "إدارة المصروفات والمشتريات والمخاسير",
                    Font = new Font("Tahoma", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };

                pnlFilters = new Panel
                {
                    Location = new Point(20, 60),
                    Size = new Size(1240, 85),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = Color.FromArgb(248, 250, 252)
                };

                pnlFilters.Controls.Add(MakeLabel("من تاريخ", 1140, 20));
                pnlFilters.Controls.Add(MakeLabel("إلى تاريخ", 930, 20));
                pnlFilters.Controls.Add(MakeLabel("النوع", 720, 20));
                pnlFilters.Controls.Add(MakeLabel("التصنيف", 470, 20));

                dtFrom = new DateTimePicker { Location = new Point(1010, 18), Size = new Size(120, 27), Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddMonths(-1) };
                dtTo = new DateTimePicker { Location = new Point(800, 18), Size = new Size(120, 27), Format = DateTimePickerFormat.Short, Value = DateTime.Today };
                cboType = new ComboBox { Location = new Point(560, 18), Size = new Size(150, 27), DropDownStyle = ComboBoxStyle.DropDownList };
                cboCategory = new ComboBox { Location = new Point(230, 18), Size = new Size(220, 27), DropDownStyle = ComboBoxStyle.DropDownList };

                cboType.Items.AddRange(new object[] { "الكل", "Expense", "Purchase", "Loss" });
                cboType.SelectedIndex = 0;

                btnSearch = MakeButton("بحث", 20, 15, 90, Color.FromArgb(0, 122, 204));
                btnRefresh = MakeButton("تحديث", 120, 15, 90, Color.Gray);
                btnEdit = MakeButton("تعديل", 220, 15, 90, Color.FromArgb(255, 140, 0));
                btnNew = MakeButton("جديد", 20, 50, 90, Color.FromArgb(0, 153, 76));
                btnView = MakeButton("عرض", 120, 50, 90, Color.SteelBlue);
                btnDelete = MakeButton("حذف", 220, 50, 90, Color.IndianRed);
                btnVoucher = MakeButton("سند", 320, 50, 90, Color.FromArgb(0, 122, 204));

                btnSearch.Click += (s, e) => LoadExpenses();
                btnRefresh.Click += (s, e) => ResetFilters();
                btnNew.Click += BtnNew_Click;
                btnEdit.Click += BtnEdit_Click;
                btnView.Click += BtnView_Click;
                btnDelete.Click += BtnDelete_Click;
                btnVoucher.Click += BtnVoucher_Click;

                pnlFilters.Controls.AddRange(new Control[]
                {
                dtFrom, dtTo, cboType, cboCategory,
                btnSearch, btnRefresh, btnEdit, btnNew, btnView, btnDelete, btnVoucher
                });

                dgvExpenses = new DataGridView
                {
                    Location = new Point(20, 160),
                    Size = new Size(1240, 470),
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    AutoGenerateColumns = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                dgvExpenses.DoubleClick += (s, e) => OpenSelectedExpense(true);

                dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "رقم السند", DataPropertyName = "ExpenseNumber", FillWeight = 120 });
                dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "التاريخ", DataPropertyName = "ExpenseDate", FillWeight = 90, DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy-MM-dd" } });
                dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "التصنيف", DataPropertyName = "CategoryName", FillWeight = 120 });
                dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "النوع", DataPropertyName = "CategoryType", FillWeight = 90 });
                dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "المورد/الجهة", DataPropertyName = "SupplierName", FillWeight = 140 });
                dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "البيان", DataPropertyName = "Description", FillWeight = 180 });
                dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "المبلغ", DataPropertyName = "TotalAmount", FillWeight = 90, DefaultCellStyle = new DataGridViewCellStyle { Format = "N2" } });
                dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "طريقة الدفع", DataPropertyName = "PaymentMethod", FillWeight = 90 });
                dgvExpenses.Columns.Add(new DataGridViewCheckBoxColumn { HeaderText = "مرحل", DataPropertyName = "IsPosted", FillWeight = 70 });
                dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "الحالة", DataPropertyName = "Status", FillWeight = 80 });

                lblStatus = new Label
                {
                    Location = new Point(20, 640),
                    Size = new Size(800, 30),
                    TextAlign = ContentAlignment.MiddleRight,
                    ForeColor = Color.DarkGreen
                };

                lblTotal = new Label
                {
                    Location = new Point(840, 640),
                    Size = new Size(420, 30),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("Tahoma", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204)
                };

                Controls.AddRange(new Control[] { lblTitle, pnlFilters, dgvExpenses, lblStatus, lblTotal });
            }

            private void ApplyPermissions()
            {
                PermissionHelper.ApplyControlPermission(btnNew, "EXPENSES_MANAGE");
                PermissionHelper.ApplyControlPermission(btnEdit, "EXPENSES_MANAGE");
                PermissionHelper.ApplyControlPermission(btnDelete, "EXPENSES_MANAGE");
                PermissionHelper.ApplyControlPermission(btnVoucher, "EXPENSE_REPORTS_PRINT");
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
            try
            {
                _categories = _service.GetCategories();

                if (_categories == null)
                    _categories = new List<ExpenseCategoryItem>();

                _categories.Insert(0, new ExpenseCategoryItem
                {
                    CategoryID = 0,
                    CategoryName = "الكل",
                    CategoryType = string.Empty
                });

                cboCategory.DataSource = null;
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "CategoryID";
                cboCategory.DataSource = _categories;
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.DarkRed;
                lblStatus.Text = ex.Message;
            }
        }
       
        private void LoadExpenses()
            {
                try
                {
                    int selectedCategoryId = cboCategory.SelectedValue is int ? (int)cboCategory.SelectedValue : 0;
                    string type = cboType.SelectedIndex <= 0 ? null : Convert.ToString(cboType.SelectedItem);

                    _expenses = _service.GetExpenses(dtFrom.Value, dtTo.Value, selectedCategoryId > 0 ? (int?)selectedCategoryId : null, type);
                    dgvExpenses.DataSource = null;
                    dgvExpenses.DataSource = _expenses;

                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = $"عدد الحركات: {_expenses.Count}";
                    lblTotal.Text = $"إجمالي المبالغ: {_expenses.Sum(x => x.TotalAmount):N2}";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                    lblTotal.Text = string.Empty;
                }
            }

            private void ResetFilters()
            {
                dtFrom.Value = DateTime.Today.AddMonths(-1);
                dtTo.Value = DateTime.Today;
                cboType.SelectedIndex = 0;
                cboCategory.SelectedIndex = 0;
                LoadExpenses();
            }

            private ExpenseHeaderItem GetSelectedExpense()
            {
                return dgvExpenses.CurrentRow?.DataBoundItem as ExpenseHeaderItem;
            }

            private void BtnNew_Click(object sender, EventArgs e)
            {
                using (var frm = new ExpenseEditForm())
                {
                    if (frm.ShowDialog() == DialogResult.OK)
                        LoadExpenses();
                }
            }

            private void BtnEdit_Click(object sender, EventArgs e)
            {
                OpenSelectedExpense(false);
            }

            private void BtnView_Click(object sender, EventArgs e)
            {
                OpenSelectedExpense(true);
            }

            private void OpenSelectedExpense(bool readOnly)
            {
                ExpenseHeaderItem item = GetSelectedExpense();
                if (item == null)
                {
                    MessageBox.Show("اختر حركة أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var frm = new ExpenseEditForm(item.ExpenseID, readOnly))
                {
                    if (!readOnly && frm.ShowDialog() == DialogResult.OK)
                        LoadExpenses();
                    else if (readOnly)
                        frm.ShowDialog();
                }
            }

            private void BtnDelete_Click(object sender, EventArgs e)
            {
                try
                {
                    ExpenseHeaderItem item = GetSelectedExpense();
                    if (item == null)
                    {
                        MessageBox.Show("اختر حركة أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (MessageBox.Show("هل تريد حذف الحركة المحددة؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;

                    _service.DeleteExpense(item.ExpenseID);
                    LoadExpenses();

                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = "تم حذف الحركة بنجاح.";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

        private void PrintVoucher(int expenseId)
        {
            try
            {
                ExpenseVoucherHeaderRow header = _reportsService.GetVoucherHeader(expenseId);
                List<ExpenseVoucherLineRow> lines = _reportsService.GetVoucherLines(expenseId);

                if (header == null)
                {
                    MessageBox.Show("تعذر تحميل السند.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string reportPath = @"Reports\RDLC\ExpenseVoucher.rdlc";
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
                    frm.Text = "سند صرف / شراء";
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
                        new ReportDataSource(
                            "dsExpenseVoucherHeader",
                            new List<ExpenseVoucherHeaderRow> { header }
                        )
                    );

                    viewer.LocalReport.DataSources.Add(
                        new ReportDataSource(
                            "dsExpenseVoucherLines",
                            lines
                        )
                    );

                    ReportParameter[] parameters =
                    {
                new ReportParameter("pReportTitle", "سند صرف / شراء / خسارة"),
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
                    "خطأ في عرض السند",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnVoucher_Click(object sender, EventArgs e)
            {
                ExpenseHeaderItem item = GetSelectedExpense();
                if (item == null)
                {
                    MessageBox.Show("اختر حركة أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                PrintVoucher(item.ExpenseID);
            }
        }
    }
/*
 * using Microsoft.Reporting.WinForms;
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

namespace water3.Forms
{
    public partial class ExpensesManagementForm : Form
    {
        private readonly ExpenseService _service = new ExpenseService();
        private readonly ExpenseReportsService _reportsService = new ExpenseReportsService();

        private List<ExpenseCategoryItem> _categories = new List<ExpenseCategoryItem>();
        private List<ExpenseHeaderItem> _expenses = new List<ExpenseHeaderItem>();

        private TableLayoutPanel mainLayout;
        private Panel headerPanel;
        private Panel filtersPanel;
        private Panel footerPanel;
        private FlowLayoutPanel actionsPanel;

        private DateTimePicker dtFrom;
        private DateTimePicker dtTo;
        private ComboBox cboType;
        private ComboBox cboCategory;

        private Button btnSearch;
        private Button btnRefresh;
        private Button btnNew;
        private Button btnEdit;
        private Button btnView;
        private Button btnDelete;
        private Button btnVoucher;

        private DataGridView dgvExpenses;
        private Label lblStatus;
        private Label lblTotal;
        private Label lblCount;
        private Label lblTitle;
        private Label lblSubtitle;

        public ExpensesManagementForm()
        {
            InitializeComponent();

            PermissionHelper.EnforceFormPermission(this, "EXPENSES_VIEW");
            ApplyPermissions();

            LoadCategories();
            LoadExpenses();
        }

        private void InitializeComponent()
        {
            Text = "المصروفات والمشتريات والخسائر";
            StartPosition = FormStartPosition.CenterScreen;
            WindowState = FormWindowState.Maximized;
            MinimumSize = new Size(1050, 680);
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            Font = new Font("Tahoma", 10F);
            BackColor = Color.FromArgb(245, 247, 250);

            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(18),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 128));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));

            BuildHeader();
            BuildFiltersAndActions();
            BuildGrid();
            BuildFooter();

            mainLayout.Controls.Add(headerPanel, 0, 0);
            mainLayout.Controls.Add(filtersPanel, 0, 1);
            mainLayout.Controls.Add(dgvExpenses, 0, 2);
            mainLayout.Controls.Add(footerPanel, 0, 3);

            Controls.Add(mainLayout);
        }

        private void BuildHeader()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(16),
                Margin = new Padding(0, 0, 0, 12)
            };

            lblTitle = new Label
            {
                Text = "إدارة المصروفات والمشتريات والخسائر",
                Dock = DockStyle.Top,
                Height = 34,
                Font = new Font("Tahoma", 17F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 76, 117),
                TextAlign = ContentAlignment.MiddleRight
            };

            lblSubtitle = new Label
            {
                Text = "استعراض الحركات، التصفية، الطباعة، وإدارة سندات المصروفات",
                Dock = DockStyle.Top,
                Height = 26,
                Font = new Font("Tahoma", 9.5F),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleRight
            };

            FlowLayoutPanel statsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                Width = 430,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Padding = new Padding(0),
                BackColor = Color.White
            };

            lblCount = MakeStatLabel("عدد الحركات: 0", Color.FromArgb(14, 165, 233));
            lblTotal = MakeStatLabel("الإجمالي: 0.00", Color.FromArgb(34, 197, 94));

            statsPanel.Controls.Add(lblTotal);
            statsPanel.Controls.Add(lblCount);

            headerPanel.Controls.Add(statsPanel);
            headerPanel.Controls.Add(lblSubtitle);
            headerPanel.Controls.Add(lblTitle);
        }

        private void BuildFiltersAndActions()
        {
            filtersPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(14),
                Margin = new Padding(0, 0, 0, 12)
            };

            TableLayoutPanel filtersLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 2,
                BackColor = Color.White
            };

            filtersLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
            filtersLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
            filtersLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));
            filtersLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22));
            filtersLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));
            filtersLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));

            filtersLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
            filtersLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            dtFrom = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today.AddMonths(-1)
            };

            dtTo = new DateTimePicker
            {
                Dock = DockStyle.Fill,
                Format = DateTimePickerFormat.Short,
                Value = DateTime.Today
            };

            cboType = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cboType.Items.AddRange(new object[] { "الكل", "Expense", "Purchase", "Loss" });
            cboType.SelectedIndex = 0;

            cboCategory = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            btnSearch = MakeButton("بحث", Color.FromArgb(37, 99, 235));
            btnRefresh = MakeButton("تحديث", Color.FromArgb(100, 116, 139));

            filtersLayout.Controls.Add(MakeInputBlock("من تاريخ", dtFrom), 0, 0);
            filtersLayout.Controls.Add(MakeInputBlock("إلى تاريخ", dtTo), 1, 0);
            filtersLayout.Controls.Add(MakeInputBlock("النوع", cboType), 2, 0);
            filtersLayout.Controls.Add(MakeInputBlock("التصنيف", cboCategory), 3, 0);
            filtersLayout.Controls.Add(btnSearch, 4, 0);
            filtersLayout.Controls.Add(btnRefresh, 5, 0);

            actionsPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                BackColor = Color.White,
                Padding = new Padding(0, 12, 0, 0)
            };

            btnNew = MakeButton("جديد", Color.FromArgb(22, 163, 74));
            btnEdit = MakeButton("تعديل", Color.FromArgb(245, 158, 11));
            btnView = MakeButton("عرض", Color.FromArgb(14, 165, 233));
            btnDelete = MakeButton("حذف", Color.FromArgb(220, 38, 38));
            btnVoucher = MakeButton("طباعة سند", Color.FromArgb(79, 70, 229), 115);

            actionsPanel.Controls.AddRange(new Control[]
            {
                btnVoucher,
                btnDelete,
                btnView,
                btnEdit,
                btnNew
            });

            filtersLayout.SetColumnSpan(actionsPanel, 6);
            filtersLayout.Controls.Add(actionsPanel, 0, 1);

            btnSearch.Click += (s, e) => LoadExpenses();
            btnRefresh.Click += (s, e) => ResetFilters();
            btnNew.Click += BtnNew_Click;
            btnEdit.Click += BtnEdit_Click;
            btnView.Click += BtnView_Click;
            btnDelete.Click += BtnDelete_Click;
            btnVoucher.Click += BtnVoucher_Click;

            filtersPanel.Controls.Add(filtersLayout);
        }

        private void BuildGrid()
        {
            dgvExpenses = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(226, 232, 240),
                Margin = new Padding(0, 0, 0, 12)
            };

            dgvExpenses.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 76, 117);
            dgvExpenses.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvExpenses.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            dgvExpenses.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvExpenses.ColumnHeadersHeight = 42;

            dgvExpenses.DefaultCellStyle.Font = new Font("Tahoma", 9.5F);
            dgvExpenses.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgvExpenses.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgvExpenses.DefaultCellStyle.Padding = new Padding(4);
            dgvExpenses.RowTemplate.Height = 34;
            dgvExpenses.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgvExpenses.DoubleClick += (s, e) => OpenSelectedExpense(true);

            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "رقم السند",
                DataPropertyName = "ExpenseNumber",
                FillWeight = 120
            });

            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "التاريخ",
                DataPropertyName = "ExpenseDate",
                FillWeight = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "yyyy-MM-dd",
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "التصنيف",
                DataPropertyName = "CategoryName",
                FillWeight = 120
            });

            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "النوع",
                DataPropertyName = "CategoryType",
                FillWeight = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "المورد / الجهة",
                DataPropertyName = "SupplierName",
                FillWeight = 140
            });

            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "البيان",
                DataPropertyName = "Description",
                FillWeight = 190
            });

            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "المبلغ",
                DataPropertyName = "TotalAmount",
                FillWeight = 95,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Format = "N2",
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Tahoma", 9.5F, FontStyle.Bold)
                }
            });

            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "طريقة الدفع",
                DataPropertyName = "PaymentMethod",
                FillWeight = 90,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });

            dgvExpenses.Columns.Add(new DataGridViewCheckBoxColumn
            {
                HeaderText = "مرحل",
                DataPropertyName = "IsPosted",
                FillWeight = 65
            });

            dgvExpenses.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "الحالة",
                DataPropertyName = "Status",
                FillWeight = 80,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                }
            });
        }

        private void BuildFooter()
        {
            footerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(14, 8, 14, 8),
                Margin = new Padding(0)
            };

            lblStatus = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.FromArgb(22, 101, 52),
                Font = new Font("Tahoma", 9.5F)
            };

            footerPanel.Controls.Add(lblStatus);
        }

        private Panel MakeInputBlock(string caption, Control input)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(6, 0, 6, 0),
                BackColor = Color.White
            };

            Label label = new Label
            {
                Text = caption,
                Dock = DockStyle.Top,
                Height = 20,
                ForeColor = Color.FromArgb(71, 85, 105),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Tahoma", 9F)
            };

            input.Height = 29;
            input.Dock = DockStyle.Top;
            input.Margin = new Padding(0);

            panel.Controls.Add(input);
            panel.Controls.Add(label);

            return panel;
        }

        private Label MakeStatLabel(string text, Color color)
        {
            return new Label
            {
                Text = text,
                Width = 200,
                Height = 46,
                Margin = new Padding(8, 6, 0, 0),
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = color,
                Font = new Font("Tahoma", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
        }

        private Button MakeButton(string text, Color color, int width = 96)
        {
            Button btn = new Button
            {
                Text = text,
                Width = width,
                Height = 34,
                Margin = new Padding(6, 0, 0, 0),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Tahoma", 9.5F, FontStyle.Bold)
            };

            btn.FlatAppearance.BorderSize = 0;

            return btn;
        }

        private void ApplyPermissions()
        {
            PermissionHelper.ApplyControlPermission(btnNew, "EXPENSES_MANAGE");
            PermissionHelper.ApplyControlPermission(btnEdit, "EXPENSES_MANAGE");
            PermissionHelper.ApplyControlPermission(btnDelete, "EXPENSES_MANAGE");
            PermissionHelper.ApplyControlPermission(btnVoucher, "EXPENSE_REPORTS_PRINT");
        }

        private void LoadCategories()
        {
            try
            {
                _categories = _service.GetCategories();

                if (_categories == null)
                    _categories = new List<ExpenseCategoryItem>();

                _categories.Insert(0, new ExpenseCategoryItem
                {
                    CategoryID = 0,
                    CategoryName = "الكل",
                    CategoryType = string.Empty
                });

                cboCategory.DataSource = null;
                cboCategory.DisplayMember = "CategoryName";
                cboCategory.ValueMember = "CategoryID";
                cboCategory.DataSource = _categories;
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message, true);
            }
        }

        private void LoadExpenses()
        {
            try
            {
                int selectedCategoryId = 0;

                if (cboCategory.SelectedValue != null &&
                    cboCategory.SelectedValue != DBNull.Value)
                {
                    int.TryParse(Convert.ToString(cboCategory.SelectedValue), out selectedCategoryId);
                }

                string type = cboType.SelectedIndex <= 0 ? null : Convert.ToString(cboType.SelectedItem);

                _expenses = _service.GetExpenses(
                    dtFrom.Value.Date,
                    dtTo.Value.Date,
                    selectedCategoryId > 0 ? (int?)selectedCategoryId : null,
                    type);

                if (_expenses == null)
                    _expenses = new List<ExpenseHeaderItem>();

                dgvExpenses.DataSource = null;
                dgvExpenses.DataSource = _expenses;

                lblCount.Text = "عدد الحركات: " + _expenses.Count;
                lblTotal.Text = "الإجمالي: " + _expenses.Sum(x => x.TotalAmount).ToString("N2");

                SetStatus("تم تحميل البيانات بنجاح.", false);
            }
            catch (Exception ex)
            {
                dgvExpenses.DataSource = null;
                lblCount.Text = "عدد الحركات: 0";
                lblTotal.Text = "الإجمالي: 0.00";
                SetStatus(ex.Message, true);
            }
        }

        private void ResetFilters()
        {
            dtFrom.Value = DateTime.Today.AddMonths(-1);
            dtTo.Value = DateTime.Today;

            if (cboType.Items.Count > 0)
                cboType.SelectedIndex = 0;

            if (cboCategory.Items.Count > 0)
                cboCategory.SelectedIndex = 0;

            LoadExpenses();
        }

        private ExpenseHeaderItem GetSelectedExpense()
        {
            return dgvExpenses.CurrentRow == null
                ? null
                : dgvExpenses.CurrentRow.DataBoundItem as ExpenseHeaderItem;
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            using (ExpenseEditForm frm = new ExpenseEditForm())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    LoadExpenses();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            OpenSelectedExpense(false);
        }

        private void BtnView_Click(object sender, EventArgs e)
        {
            OpenSelectedExpense(true);
        }

        private void OpenSelectedExpense(bool readOnly)
        {
            ExpenseHeaderItem item = GetSelectedExpense();

            if (item == null)
            {
                MessageBox.Show("اختر حركة أولًا.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (ExpenseEditForm frm = new ExpenseEditForm(item.ExpenseID, readOnly))
            {
                if (readOnly)
                {
                    frm.ShowDialog(this);
                    return;
                }

                if (frm.ShowDialog(this) == DialogResult.OK)
                    LoadExpenses();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                ExpenseHeaderItem item = GetSelectedExpense();

                if (item == null)
                {
                    MessageBox.Show("اختر حركة أولًا.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DialogResult result = MessageBox.Show(
                    "هل تريد حذف الحركة المحددة؟",
                    "تأكيد الحذف",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                    return;

                _service.DeleteExpense(item.ExpenseID);

                LoadExpenses();
                SetStatus("تم حذف الحركة بنجاح.", false);
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message, true);
                MessageBox.Show(ex.Message, "خطأ في الحذف",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnVoucher_Click(object sender, EventArgs e)
        {
            ExpenseHeaderItem item = GetSelectedExpense();

            if (item == null)
            {
                MessageBox.Show("اختر حركة أولًا.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrintVoucher(item.ExpenseID);
        }

        private void PrintVoucher(int expenseId)
        {
            try
            {
                ExpenseVoucherHeaderRow header = _reportsService.GetVoucherHeader(expenseId);
                List<ExpenseVoucherLineRow> lines = _reportsService.GetVoucherLines(expenseId);

                if (header == null)
                {
                    MessageBox.Show("تعذر تحميل السند.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (lines == null)
                    lines = new List<ExpenseVoucherLineRow>();

                string reportPath = @"Reports\RDLC\ExpenseVoucher.rdlc";
                string fullReportPath = Path.Combine(Application.StartupPath, reportPath);

                if (!File.Exists(fullReportPath))
                    fullReportPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, reportPath);

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
                    frm.Text = "سند صرف / شراء";
                    frm.StartPosition = FormStartPosition.CenterParent;
                    frm.WindowState = FormWindowState.Maximized;
                    frm.RightToLeft = RightToLeft.Yes;
                    frm.RightToLeftLayout = true;
                    frm.Font = new Font("Tahoma", 10F);

                    ReportViewer viewer = new ReportViewer
                    {
                        Dock = DockStyle.Fill,
                        ProcessingMode = ProcessingMode.Local
                    };

                    viewer.LocalReport.ReportPath = fullReportPath;
                    viewer.LocalReport.DataSources.Clear();

                    viewer.LocalReport.DataSources.Add(
                        new ReportDataSource(
                            "dsExpenseVoucherHeader",
                            new List<ExpenseVoucherHeaderRow> { header }));

                    viewer.LocalReport.DataSources.Add(
                        new ReportDataSource(
                            "dsExpenseVoucherLines",
                            lines));

                    ReportParameter[] parameters =
                    {
                        new ReportParameter("pReportTitle", "سند صرف / شراء / خسارة"),
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
                SetStatus(ex.Message, true);

                MessageBox.Show(
                    ex.Message,
                    "خطأ في عرض السند",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void SetStatus(string message, bool isError)
        {
            lblStatus.ForeColor = isError
                ? Color.FromArgb(185, 28, 28)
                : Color.FromArgb(22, 101, 52);

            lblStatus.Text = message;
        }
    }
}

 * */