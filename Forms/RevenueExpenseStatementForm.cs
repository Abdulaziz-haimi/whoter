using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using water3.Models;
using water3.Security;
using water3.Services;
using water3.Utils;
using System.Drawing.Printing;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace water3.Forms
{
    public partial class RevenueExpenseStatementForm : Form
    {




            private readonly RevenueExpenseStatementService _service =
                new RevenueExpenseStatementService();

            private DateTimePicker dtFrom;
            private DateTimePicker dtTo;
            private ComboBox cboMovementType;
            private ComboBox cboCategory;
            private ComboBox cboPaymentMethod;
            private TextBox txtSearch;

            private Button btnSearch;
            private Button btnRefresh;
            private Button btnExportExcel;
            private Button btnPrint;

            private DataGridView dgvStatement;

            private Label lblOpening;
            private Label lblRevenue;
            private Label lblExpense;
            private Label lblRemaining;
            private Label lblStatus;

            private readonly BindingSource bindingSource = new BindingSource();
            private RevenueExpenseStatementResult _currentResult = new RevenueExpenseStatementResult();

            private List<RevenueExpenseStatementRow> _printRows;
            private int _printRowIndex;
            private int _printPageNumber;

            private readonly Color PageBack = Color.FromArgb(245, 247, 250);
            private readonly Color CardBack = Color.White;
            private readonly Color Primary = Color.FromArgb(0, 102, 204);
            private readonly Color Green = Color.FromArgb(0, 153, 76);
            private readonly Color Red = Color.FromArgb(220, 38, 38);
            private readonly Color Orange = Color.FromArgb(245, 158, 11);
            private readonly Color TextDark = Color.FromArgb(30, 41, 59);
            private readonly Color Muted = Color.FromArgb(100, 116, 139);
            private readonly Color Border = Color.FromArgb(226, 232, 240);

            public RevenueExpenseStatementForm()
            {
                InitializeComponent();

                PermissionHelper.EnforceFormPermission(this, PermissionKeys.AccountStatementView);
                ApplyPermissions();

                LoadCategories();
                LoadStatement();
            }

            private void InitializeComponent()
            {
                SuspendLayout();

                Text = "كشف الإيرادات والمنصرفات";
                StartPosition = FormStartPosition.CenterScreen;
                WindowState = FormWindowState.Maximized;
                MinimumSize = new Size(1180, 720);
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 9F);
                BackColor = PageBack;
                AutoScaleMode = AutoScaleMode.Dpi;
                AutoScroll = false;

                TableLayoutPanel root = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    BackColor = PageBack,
                    ColumnCount = 1,
                    RowCount = 5,
                    Padding = new Padding(18, 10, 18, 6),
                    Margin = new Padding(0)
                };

                root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 66F));
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 132F));
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 86F));
                root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));

                root.Controls.Add(CreateHeaderPanel(), 0, 0);
                root.Controls.Add(CreateFilterPanel(), 0, 1);
                root.Controls.Add(CreateSummaryPanel(), 0, 2);

                dgvStatement = CreateGrid();
                CreateColumns();

                Panel gridPanel = CreateGridPanel();
                gridPanel.Controls.Add(dgvStatement);
                root.Controls.Add(gridPanel, 0, 3);

                lblStatus = new Label
                {
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = new Font("Tahoma", 9F, FontStyle.Bold),
                    ForeColor = Muted,
                    Padding = new Padding(8, 0, 8, 0),
                    Margin = new Padding(0)
                };

                root.Controls.Add(lblStatus, 0, 4);

                Controls.Clear();
                Controls.Add(root);

                ResumeLayout(false);
            }

            private Panel CreateHeaderPanel()
            {
                Panel outer = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = PageBack,
                    Padding = new Padding(0, 0, 0, 6),
                    Margin = new Padding(0)
                };

                Panel card = CreateCard();
                card.Padding = new Padding(18, 6, 18, 4);

                Label title = new Label
                {
                    Text = "كشف حساب الإيرادات والمنصرفات",
                    Dock = DockStyle.Top,
                    Height = 30,
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = new Font("Tahoma", 14.5F, FontStyle.Bold),
                    ForeColor = Primary
                };

                Label subtitle = new Label
                {
                    Text = "عرض الإيرادات والمنصرفات والباقي مع الفلترة والتصدير إلى Excel والطباعة مع شعار وبيانات الشركة",
                    Dock = DockStyle.Top,
                    Height = 22,
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = new Font("Tahoma", 8.5F),
                    ForeColor = Muted
                };

                card.Controls.Add(subtitle);
                card.Controls.Add(title);
                outer.Controls.Add(card);

                return outer;
            }

            private Panel CreateFilterPanel()
            {
                Panel outer = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = PageBack,
                    Padding = new Padding(0, 0, 0, 6),
                    Margin = new Padding(0)
                };

                Panel card = CreateCard();
                card.Padding = new Padding(12, 8, 12, 8);

                TableLayoutPanel layout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    BackColor = CardBack,
                    ColumnCount = 1,
                    RowCount = 2,
                    Margin = new Padding(0)
                };

                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 66F));
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));

                FlowLayoutPanel filtersFlow = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RightToLeft = RightToLeft.Yes,
                    FlowDirection = FlowDirection.RightToLeft,
                    WrapContents = false,
                    AutoScroll = true,
                    BackColor = CardBack,
                    Margin = new Padding(0),
                    Padding = new Padding(0)
                };

                dtFrom = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Short,
                    Value = DateTime.Today.AddMonths(-1),
                    Width = 155,
                    Font = new Font("Tahoma", 9F),
                    RightToLeft = RightToLeft.Yes,
                    RightToLeftLayout = true
                };

                dtTo = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Short,
                    Value = DateTime.Today,
                    Width = 155,
                    Font = new Font("Tahoma", 9F),
                    RightToLeft = RightToLeft.Yes,
                    RightToLeftLayout = true
                };

                cboMovementType = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Width = 150,
                    Font = new Font("Tahoma", 9F),
                    IntegralHeight = false,
                    DropDownHeight = 180
                };

                cboMovementType.Items.Add(new LookupItem("الكل", ""));
                cboMovementType.Items.Add(new LookupItem("إيرادات فقط", "Revenue"));
                cboMovementType.Items.Add(new LookupItem("كل المنصرفات", "Outflow"));
                cboMovementType.Items.Add(new LookupItem("مصروفات", "Expense"));
                cboMovementType.Items.Add(new LookupItem("مشتريات", "Purchase"));
                cboMovementType.Items.Add(new LookupItem("خسائر", "Loss"));
                cboMovementType.SelectedIndex = 0;

                cboCategory = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Width = 220,
                    Font = new Font("Tahoma", 9F),
                    IntegralHeight = false,
                    DropDownHeight = 220
                };

                cboPaymentMethod = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Width = 150,
                    Font = new Font("Tahoma", 9F),
                    IntegralHeight = false,
                    DropDownHeight = 180
                };

                cboPaymentMethod.Items.Add(new LookupItem("الكل", ""));
                cboPaymentMethod.Items.Add(new LookupItem("نقداً", "Cash"));
                cboPaymentMethod.Items.Add(new LookupItem("تحويل", "Transfer"));
                cboPaymentMethod.Items.Add(new LookupItem("شيك", "Cheque"));
                cboPaymentMethod.Items.Add(new LookupItem("آجل", "Credit"));
                cboPaymentMethod.Items.Add(new LookupItem("أخرى", "Other"));
                cboPaymentMethod.SelectedIndex = 0;

                txtSearch = new TextBox
                {
                    Width = 260,
                    Font = new Font("Tahoma", 9F)
                };

                filtersFlow.Controls.Add(CreateField("من تاريخ", dtFrom, 165));
                filtersFlow.Controls.Add(CreateField("إلى تاريخ", dtTo, 165));
                filtersFlow.Controls.Add(CreateField("نوع الحركة", cboMovementType, 165));
                filtersFlow.Controls.Add(CreateField("التصنيف", cboCategory, 230));
                filtersFlow.Controls.Add(CreateField("طريقة الدفع", cboPaymentMethod, 165));
                filtersFlow.Controls.Add(CreateField("بحث", txtSearch, 270));

                FlowLayoutPanel actionsFlow = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RightToLeft = RightToLeft.Yes,
                    FlowDirection = FlowDirection.RightToLeft,
                    WrapContents = false,
                    AutoScroll = false,
                    BackColor = CardBack,
                    Margin = new Padding(0),
                    Padding = new Padding(0, 4, 0, 0)
                };

                btnSearch = MakeButton("عرض الكشف", 120, Primary);
                btnRefresh = MakeButton("تحديث", 100, Muted);
                btnExportExcel = MakeButton("تصدير Excel", 120, Green);
                btnPrint = MakeButton("طباعة", 100, Orange);

                btnSearch.Click += (s, e) => LoadStatement();
                btnRefresh.Click += (s, e) => ResetFilters();
                btnExportExcel.Click += (s, e) => ExportToExcel();
                btnPrint.Click += (s, e) => PrintStatement();

                actionsFlow.Controls.Add(btnSearch);
                actionsFlow.Controls.Add(btnRefresh);
                actionsFlow.Controls.Add(btnExportExcel);
                actionsFlow.Controls.Add(btnPrint);

                layout.Controls.Add(filtersFlow, 0, 0);
                layout.Controls.Add(actionsFlow, 0, 1);

                card.Controls.Add(layout);
                outer.Controls.Add(card);

                return outer;
            }

            private Panel CreateSummaryPanel()
            {
                Panel outer = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = PageBack,
                    Padding = new Padding(0, 0, 0, 6),
                    Margin = new Padding(0)
                };

                TableLayoutPanel table = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    BackColor = PageBack,
                    ColumnCount = 4,
                    RowCount = 1,
                    Margin = new Padding(0)
                };

                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

                table.Controls.Add(CreateSummaryCard("الرصيد السابق", Orange, out lblOpening), 0, 0);
                table.Controls.Add(CreateSummaryCard("إجمالي الإيرادات", Green, out lblRevenue), 1, 0);
                table.Controls.Add(CreateSummaryCard("إجمالي المنصرفات", Red, out lblExpense), 2, 0);
                table.Controls.Add(CreateSummaryCard("الباقي", Primary, out lblRemaining), 3, 0);

                outer.Controls.Add(table);
                return outer;
            }

            private Panel CreateSummaryCard(string title, Color color, out Label valueLabel)
            {
                Panel card = CreateCard();
                card.Margin = new Padding(6, 0, 6, 0);
                card.Padding = new Padding(12, 8, 12, 8);

                Label lblTitle = new Label
                {
                    Text = title,
                    Dock = DockStyle.Top,
                    Height = 24,
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = new Font("Tahoma", 9F, FontStyle.Bold),
                    ForeColor = Muted
                };

                valueLabel = new Label
                {
                    Text = "0.00",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = new Font("Tahoma", 13.5F, FontStyle.Bold),
                    ForeColor = color
                };

                card.Controls.Add(valueLabel);
                card.Controls.Add(lblTitle);

                return card;
            }

            private Panel CreateGridPanel()
            {
                Panel panel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = CardBack,
                    Padding = new Padding(0, 0, 0, 10),
                    Margin = new Padding(0)
                };

                panel.Paint += Card_Paint;

                return panel;
            }

            private DataGridView CreateGrid()
            {
                DataGridView grid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AllowUserToResizeRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    AutoGenerateColumns = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    BackgroundColor = CardBack,
                    BorderStyle = BorderStyle.None,
                    EnableHeadersVisualStyles = false,
                    RowHeadersVisible = false,
                    RightToLeft = RightToLeft.Yes,
                    GridColor = Border,
                    ScrollBars = ScrollBars.Both,
                    ColumnHeadersHeight = 36,
                    ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                    RowTemplate = { Height = 32 },
                    Margin = new Padding(0, 0, 0, 10)
                };

                grid.ColumnHeadersDefaultCellStyle.BackColor = Primary;
                grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                grid.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9F, FontStyle.Bold);
                grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(0);

                grid.DefaultCellStyle.Font = new Font("Tahoma", 9F);
                grid.DefaultCellStyle.BackColor = Color.White;
                grid.DefaultCellStyle.ForeColor = TextDark;
                grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
                grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 64, 175);
                grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                grid.DefaultCellStyle.Padding = new Padding(2);

                grid.RowsDefaultCellStyle.BackColor = Color.White;
                grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
                grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

                grid.CellFormatting += DgvStatement_CellFormatting;
                grid.DataSource = bindingSource;

                return grid;
            }

            private void CreateColumns()
            {
                dgvStatement.Columns.Clear();

                dgvStatement.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "التاريخ",
                    DataPropertyName = "MovementDate",
                    FillWeight = 80,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "yyyy-MM-dd",
                        Alignment = DataGridViewContentAlignment.MiddleCenter
                    }
                });

                dgvStatement.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "رقم المرجع",
                    DataPropertyName = "RefNo",
                    FillWeight = 115
                });

                dgvStatement.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "النوع",
                    DataPropertyName = "MovementKind",
                    FillWeight = 85,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleCenter,
                        Font = new Font("Tahoma", 9F, FontStyle.Bold)
                    }
                });

                dgvStatement.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "التصنيف",
                    DataPropertyName = "CategoryName",
                    FillWeight = 120
                });

                dgvStatement.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "الجهة / المشترك",
                    DataPropertyName = "PartyName",
                    FillWeight = 145
                });

                dgvStatement.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "البيان",
                    DataPropertyName = "Description",
                    FillWeight = 165
                });

                dgvStatement.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "طريقة الدفع",
                    DataPropertyName = "PaymentMethod",
                    FillWeight = 90,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleCenter
                    }
                });

                dgvStatement.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "الإيراد",
                    DataPropertyName = "RevenueAmount",
                    FillWeight = 90,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "N2",
                        Alignment = DataGridViewContentAlignment.MiddleCenter,
                        ForeColor = Green,
                        Font = new Font("Tahoma", 9F, FontStyle.Bold)
                    }
                });

                dgvStatement.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "المنصرف",
                    DataPropertyName = "ExpenseAmount",
                    FillWeight = 90,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "N2",
                        Alignment = DataGridViewContentAlignment.MiddleCenter,
                        ForeColor = Red,
                        Font = new Font("Tahoma", 9F, FontStyle.Bold)
                    }
                });

                dgvStatement.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = "الرصيد",
                    DataPropertyName = "Balance",
                    FillWeight = 95,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Format = "N2",
                        Alignment = DataGridViewContentAlignment.MiddleCenter,
                        Font = new Font("Tahoma", 9F, FontStyle.Bold)
                    }
                });
            }

            private void ApplyPermissions()
            {
                PermissionHelper.ApplyControlPermission(btnExportExcel, PermissionKeys.ReportExport);
                PermissionHelper.ApplyControlPermission(btnPrint, PermissionKeys.ReportPrint);
            }

            private void LoadCategories()
            {
                try
                {
                    List<ExpenseCategoryItem> categories = _service.GetExpenseCategories();

                    if (categories == null)
                        categories = new List<ExpenseCategoryItem>();

                    categories.Insert(0, new ExpenseCategoryItem
                    {
                        CategoryID = 0,
                        CategoryName = "الكل",
                        CategoryType = string.Empty
                    });

                    cboCategory.DataSource = null;
                    cboCategory.DisplayMember = "CategoryName";
                    cboCategory.ValueMember = "CategoryID";
                    cboCategory.DataSource = categories;
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Red;
                    lblStatus.Text = ex.Message;
                }
            }

            private RevenueExpenseStatementFilter BuildFilter()
            {
                string movementType = GetSelectedLookupValue(cboMovementType);
                string paymentMethod = GetSelectedLookupValue(cboPaymentMethod);

                int categoryId = 0;

                if (cboCategory.SelectedValue != null && cboCategory.SelectedValue != DBNull.Value)
                    int.TryParse(cboCategory.SelectedValue.ToString(), out categoryId);

                return new RevenueExpenseStatementFilter
                {
                    FromDate = dtFrom.Value.Date,
                    ToDate = dtTo.Value.Date,
                    MovementType = string.IsNullOrWhiteSpace(movementType) ? null : movementType,
                    CategoryId = categoryId > 0 ? (int?)categoryId : null,
                    PaymentMethod = string.IsNullOrWhiteSpace(paymentMethod) ? null : paymentMethod,
                    SearchText = string.IsNullOrWhiteSpace(txtSearch.Text) ? null : txtSearch.Text.Trim()
                };
            }

            private void LoadStatement()
            {
                try
                {
                    RevenueExpenseStatementFilter filter = BuildFilter();

                    _currentResult = _service.GetStatement(filter);

                    if (_currentResult == null)
                        _currentResult = new RevenueExpenseStatementResult();

                    bindingSource.DataSource = null;
                    bindingSource.DataSource = _currentResult.Rows;

                    lblOpening.Text = _currentResult.OpeningBalance.ToString("N2");
                    lblRevenue.Text = _currentResult.TotalRevenue.ToString("N2");
                    lblExpense.Text = _currentResult.TotalExpense.ToString("N2");
                    lblRemaining.Text = _currentResult.RemainingBalance.ToString("N2");

                    lblRemaining.ForeColor = _currentResult.RemainingBalance >= 0 ? Green : Red;

                    lblStatus.ForeColor = Muted;
                    lblStatus.Text =
                        "عدد الحركات: " + _currentResult.Rows.Count +
                        " | من " + filter.FromDate.ToString("yyyy-MM-dd") +
                        " إلى " + filter.ToDate.ToString("yyyy-MM-dd");
                }
                catch (Exception ex)
                {
                    bindingSource.DataSource = null;

                    lblOpening.Text = "0.00";
                    lblRevenue.Text = "0.00";
                    lblExpense.Text = "0.00";
                    lblRemaining.Text = "0.00";

                    lblStatus.ForeColor = Red;
                    lblStatus.Text = ex.Message;

                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void ResetFilters()
            {
                dtFrom.Value = DateTime.Today.AddMonths(-1);
                dtTo.Value = DateTime.Today;

                if (cboMovementType.Items.Count > 0)
                    cboMovementType.SelectedIndex = 0;

                if (cboCategory.Items.Count > 0)
                    cboCategory.SelectedIndex = 0;

                if (cboPaymentMethod.Items.Count > 0)
                    cboPaymentMethod.SelectedIndex = 0;

                txtSearch.Clear();

                LoadStatement();
            }

            private void ExportToExcel()
            {
                try
                {
                    if (_currentResult == null || _currentResult.Rows == null || _currentResult.Rows.Count == 0)
                    {
                        MessageBox.Show("لا توجد بيانات للتصدير.", "تنبيه",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Title = "تصدير كشف الإيرادات والمنصرفات";
                        sfd.Filter = "Excel 97-2003 (*.xls)|*.xls";
                        sfd.FileName = "كشف_الإيرادات_والمنصرفات_" + DateTime.Now.ToString("yyyyMMdd_HHmm") + ".xls";

                        if (sfd.ShowDialog(this) != DialogResult.OK)
                            return;

                        string html = BuildExcelHtml();
                        File.WriteAllText(sfd.FileName, html, Encoding.UTF8);

                        MessageBox.Show("تم تصدير الملف بنجاح.", "تم",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ في التصدير",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private string BuildExcelHtml()
            {
                CompanyPrintInfoData company = CompanyPrintInfo.Get();
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine("<meta http-equiv='Content-Type' content='text/html; charset=utf-8' />");
                sb.AppendLine("<style>");
                sb.AppendLine("body{font-family:Tahoma;direction:rtl;}");
                sb.AppendLine("table{border-collapse:collapse;width:100%;}");
                sb.AppendLine("th{background:#0066cc;color:white;font-weight:bold;}");
                sb.AppendLine("td,th{border:1px solid #999;padding:6px;text-align:center;}");
                sb.AppendLine(".green{color:#00994c;font-weight:bold;}");
                sb.AppendLine(".red{color:#dc2626;font-weight:bold;}");
                sb.AppendLine(".blue{color:#0066cc;font-weight:bold;}");
                sb.AppendLine("</style>");
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");

                sb.AppendLine("<h2>" + WebUtility.HtmlEncode(company.CompanyName) + "</h2>");
                sb.AppendLine("<h3>كشف حساب الإيرادات والمنصرفات</h3>");
                sb.AppendLine("<p>" + WebUtility.HtmlEncode(company.Address) + " - " + WebUtility.HtmlEncode(company.Phone) + "</p>");
                sb.AppendLine("<p>من تاريخ: " + WebUtility.HtmlEncode(dtFrom.Value.ToString("yyyy-MM-dd")) + " - إلى تاريخ: " + WebUtility.HtmlEncode(dtTo.Value.ToString("yyyy-MM-dd")) + "</p>");

                sb.AppendLine("<table>");
                sb.AppendLine("<tr><th>الرصيد السابق</th><th>إجمالي الإيرادات</th><th>إجمالي المنصرفات</th><th>الباقي</th></tr>");
                sb.AppendLine("<tr>");
                sb.AppendLine("<td>" + _currentResult.OpeningBalance.ToString("N2") + "</td>");
                sb.AppendLine("<td class='green'>" + _currentResult.TotalRevenue.ToString("N2") + "</td>");
                sb.AppendLine("<td class='red'>" + _currentResult.TotalExpense.ToString("N2") + "</td>");
                sb.AppendLine("<td class='blue'>" + _currentResult.RemainingBalance.ToString("N2") + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("</table>");
                sb.AppendLine("<br/>");

                sb.AppendLine("<table>");
                sb.AppendLine("<tr>");
                sb.AppendLine("<th>التاريخ</th><th>رقم المرجع</th><th>النوع</th><th>التصنيف</th><th>الجهة / المشترك</th><th>البيان</th><th>طريقة الدفع</th><th>الإيراد</th><th>المنصرف</th><th>الرصيد</th>");
                sb.AppendLine("</tr>");

                foreach (RevenueExpenseStatementRow row in _currentResult.Rows)
                {
                    sb.AppendLine("<tr>");
                    sb.AppendLine("<td>" + row.MovementDate.ToString("yyyy-MM-dd") + "</td>");
                    sb.AppendLine("<td>" + WebUtility.HtmlEncode(row.RefNo) + "</td>");
                    sb.AppendLine("<td>" + WebUtility.HtmlEncode(row.MovementKind) + "</td>");
                    sb.AppendLine("<td>" + WebUtility.HtmlEncode(row.CategoryName) + "</td>");
                    sb.AppendLine("<td>" + WebUtility.HtmlEncode(row.PartyName) + "</td>");
                    sb.AppendLine("<td>" + WebUtility.HtmlEncode(row.Description) + "</td>");
                    sb.AppendLine("<td>" + WebUtility.HtmlEncode(row.PaymentMethod) + "</td>");
                    sb.AppendLine("<td class='green'>" + row.RevenueAmount.ToString("N2") + "</td>");
                    sb.AppendLine("<td class='red'>" + row.ExpenseAmount.ToString("N2") + "</td>");
                    sb.AppendLine("<td class='blue'>" + row.Balance.ToString("N2") + "</td>");
                    sb.AppendLine("</tr>");
                }

                sb.AppendLine("</table>");
                sb.AppendLine("</body>");
                sb.AppendLine("</html>");

                return sb.ToString();
            }
        private void PrintStatement()
        {
            try
            {
                if (_currentResult == null || _currentResult.Rows == null || _currentResult.Rows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات للطباعة.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                PrintDocument doc = new PrintDocument();
                doc.DocumentName = "كشف الإيرادات والمنصرفات";
                doc.DefaultPageSettings.Landscape = true;
                doc.DefaultPageSettings.Margins = new Margins(35, 35, 35, 45);

                doc.BeginPrint += delegate
                {
                    _printRows = _currentResult.Rows.ToList();
                    _printRowIndex = 0;
                    _printPageNumber = 1;
                };

                doc.PrintPage += PrintDocument_PrintPage;

                using (PrintPreviewDialog preview = new PrintPreviewDialog())
                {
                    preview.Document = doc;
                    preview.WindowState = FormWindowState.Maximized;
                    preview.Text = "معاينة طباعة كشف الإيرادات والمنصرفات";
                    preview.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ في الطباعة",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //private void PrintStatement()
        //{
        //    try
        //    {
        //        if (_currentResult == null || _currentResult.Rows == null || _currentResult.Rows.Count == 0)
        //        {
        //            MessageBox.Show("لا توجد بيانات للطباعة.", "تنبيه",
        //                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            return;
        //        }

        //        _printRows = _currentResult.Rows.ToList();
        //        _printRowIndex = 0;
        //        _printPageNumber = 1;

        //        PrintDocument doc = new PrintDocument();
        //        doc.DocumentName = "كشف الإيرادات والمنصرفات";
        //        doc.DefaultPageSettings.Landscape = true;
        //        doc.PrintPage += PrintDocument_PrintPage;

        //        using (PrintPreviewDialog preview = new PrintPreviewDialog())
        //        {
        //            preview.Document = doc;
        //            preview.WindowState = FormWindowState.Maximized;
        //            preview.Text = "معاينة طباعة كشف الإيرادات والمنصرفات";
        //            preview.ShowDialog(this);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "خطأ في الطباعة",
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        //private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        //    {
        //        int x = e.MarginBounds.Left;
        //        int width = e.MarginBounds.Width;

        //        int y = PrintHeaderHelper.DrawCompanyHeader(
        //            e,
        //            "كشف حساب الإيرادات والمنصرفات",
        //            dtFrom.Value.Date,
        //            dtTo.Value.Date
        //        );

        //        y += 10;

        //        Font headerFont = new Font("Tahoma", 8.5F, FontStyle.Bold);
        //        Font cellFont = new Font("Tahoma", 8F);
        //        Font smallFont = new Font("Tahoma", 8F, FontStyle.Bold);

        //        StringFormat rtlCenter = new StringFormat
        //        {
        //            Alignment = StringAlignment.Center,
        //            LineAlignment = StringAlignment.Center,
        //            FormatFlags = StringFormatFlags.DirectionRightToLeft
        //        };

        //        string summary =
        //            "الرصيد السابق: " + _currentResult.OpeningBalance.ToString("N2") +
        //            "    |    إجمالي الإيرادات: " + _currentResult.TotalRevenue.ToString("N2") +
        //            "    |    إجمالي المنصرفات: " + _currentResult.TotalExpense.ToString("N2") +
        //            "    |    الباقي: " + _currentResult.RemainingBalance.ToString("N2");

        //        e.Graphics.DrawString(summary, smallFont, Brushes.Black,
        //            new RectangleF(x, y, width, 24), rtlCenter);

        //        y += 36;

        //        int rowHeight = 28;

        //        int[] colWidths =
        //        {
        //        80,
        //        110,
        //        80,
        //        110,
        //        130,
        //        170,
        //        80,
        //        85,
        //        85,
        //        90
        //    };

        //        string[] headers =
        //        {
        //        "التاريخ",
        //        "رقم المرجع",
        //        "النوع",
        //        "التصنيف",
        //        "الجهة",
        //        "البيان",
        //        "الدفع",
        //        "الإيراد",
        //        "المنصرف",
        //        "الرصيد"
        //    };

        //        int tableWidth = colWidths.Sum();
        //        int startX = x + Math.Max(0, (width - tableWidth) / 2);

        //        DrawPrintRow(e.Graphics, headers, colWidths, startX, y, rowHeight, headerFont, rtlCenter, true);
        //        y += rowHeight;

        //        while (_printRowIndex < _printRows.Count)
        //        {
        //            if (y + rowHeight > e.MarginBounds.Bottom - 20)
        //            {
        //                PrintHeaderHelper.DrawCompanyFooter(e, _printPageNumber);
        //                _printPageNumber++;
        //                e.HasMorePages = true;
        //                return;
        //            }

        //            RevenueExpenseStatementRow r = _printRows[_printRowIndex];

        //            string[] cells =
        //            {
        //            r.MovementDate.ToString("yyyy-MM-dd"),
        //            r.RefNo,
        //            r.MovementKind,
        //            r.CategoryName,
        //            r.PartyName,
        //            r.Description,
        //            r.PaymentMethod,
        //            r.RevenueAmount.ToString("N2"),
        //            r.ExpenseAmount.ToString("N2"),
        //            r.Balance.ToString("N2")
        //        };

        //            DrawPrintRow(e.Graphics, cells, colWidths, startX, y, rowHeight, cellFont, rtlCenter, false);

        //            y += rowHeight;
        //            _printRowIndex++;
        //        }

        //        PrintHeaderHelper.DrawCompanyFooter(e, _printPageNumber);
        //        e.HasMorePages = false;
        //    }
        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.TextRenderingHint =
                System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            int x = e.MarginBounds.Left;
            int width = e.MarginBounds.Width;

            int y = PrintHeaderHelper.DrawCompanyHeader(
                e,
                "كشف حساب الإيرادات والمنصرفات",
                dtFrom.Value.Date,
                dtTo.Value.Date
            );

            y += 8;

            Font headerFont = new Font("Tahoma", 8.5F, FontStyle.Bold);
            Font cellFont = new Font("Tahoma", 8F, FontStyle.Regular);
            Font totalFont = new Font("Tahoma", 8.5F, FontStyle.Bold);
            Font summaryFont = new Font("Tahoma", 8.5F, FontStyle.Bold);

            StringFormat centerFormat = CreateArabicCenterFormat();
            StringFormat rightFormat = CreateArabicRightFormat();

            DrawPrintSummaryBox(e.Graphics, x, y, width, summaryFont);
            y += 42;

            int rowHeight = 27;
            int totalRowHeight = 30;
            int footerSpace = 35;
            int bottomLimit = e.MarginBounds.Bottom - footerSpace;

            int[] baseWidths =
            {
        80,   // التاريخ
        115,  // رقم المرجع
        80,   // النوع
        115,  // التصنيف
        140,  // الجهة / المشترك
        170,  // البيان
        85,   // الدفع
        90,   // الإيراد
        90,   // المنصرف
        95    // الرصيد
    };

            int[] colWidths = ScaleColumnWidths(baseWidths, width);

            string[] headers =
            {
        "التاريخ",
        "رقم المرجع",
        "النوع",
        "التصنيف",
        "الجهة / المشترك",
        "البيان",
        "الدفع",
        "الإيراد",
        "المنصرف",
        "الرصيد"
    };

            int tableWidth = colWidths.Sum();
            int startX = x + Math.Max(0, (width - tableWidth) / 2);

            DrawPrintRowRtl(
                e.Graphics,
                headers,
                colWidths,
                startX,
                y,
                rowHeight,
                headerFont,
                true,
                -1
            );

            y += rowHeight;

            while (_printRowIndex < _printRows.Count)
            {
                if (y + rowHeight > bottomLimit)
                {
                    PrintHeaderHelper.DrawCompanyFooter(e, _printPageNumber);
                    _printPageNumber++;
                    e.HasMorePages = true;
                    return;
                }

                RevenueExpenseStatementRow r = _printRows[_printRowIndex];

                string[] cells =
                {
            r.MovementDate.ToString("yyyy-MM-dd"),
            r.RefNo,
            r.MovementKind,
            r.CategoryName,
            r.PartyName,
            r.Description,
            r.PaymentMethod,
            r.RevenueAmount.ToString("N2"),
            r.ExpenseAmount.ToString("N2"),
            r.Balance.ToString("N2")
        };

                DrawPrintRowRtl(
                    e.Graphics,
                    cells,
                    colWidths,
                    startX,
                    y,
                    rowHeight,
                    cellFont,
                    false,
                    _printRowIndex
                );

                y += rowHeight;
                _printRowIndex++;
            }

            // صف الإجمالي في آخر صفحة فقط
            if (y + totalRowHeight > bottomLimit)
            {
                PrintHeaderHelper.DrawCompanyFooter(e, _printPageNumber);
                _printPageNumber++;
                e.HasMorePages = true;
                return;
            }

            string[] totalCells =
            {
        "",
        "",
        "",
        "",
        "",
        "الإجمالي",
        "",
        _currentResult.TotalRevenue.ToString("N2"),
        _currentResult.TotalExpense.ToString("N2"),
        _currentResult.RemainingBalance.ToString("N2")
    };

            DrawTotalRowRtl(
                e.Graphics,
                totalCells,
                colWidths,
                startX,
                y,
                totalRowHeight,
                totalFont
            );

            PrintHeaderHelper.DrawCompanyFooter(e, _printPageNumber);
            e.HasMorePages = false;
        }
        private void DrawPrintRowRtl(
    Graphics g,
    string[] cells,
    int[] widths,
    int x,
    int y,
    int height,
    Font font,
    bool isHeader,
    int rowIndex)
        {
            Color headerBack = Primary;
            Color rowBack = rowIndex % 2 == 0
                ? Color.White
                : Color.FromArgb(248, 250, 252);

            Color backColor = isHeader ? headerBack : rowBack;
            Color foreColor = isHeader ? Color.White : Color.Black;

            StringFormat centerFormat = CreateArabicCenterFormat();
            StringFormat rightFormat = CreateArabicRightFormat();

            using (SolidBrush backBrush = new SolidBrush(backColor))
            using (SolidBrush textBrush = new SolidBrush(foreColor))
            using (Pen borderPen = new Pen(Color.FromArgb(150, 150, 150)))
            {
                int right = x + widths.Sum();

                for (int i = 0; i < cells.Length; i++)
                {
                    Rectangle rect = new Rectangle(
                        right - widths[i],
                        y,
                        widths[i],
                        height
                    );

                    g.FillRectangle(backBrush, rect);
                    g.DrawRectangle(borderPen, rect);

                    string text = ShortPrintText(cells[i], 35);

                    StringFormat format = centerFormat;

                    // أعمدة نصية تحتاج محاذاة يمين
                    if (!isHeader && (i == 3 || i == 4 || i == 5))
                        format = rightFormat;

                    RectangleF textRect = new RectangleF(
                        rect.X + 4,
                        rect.Y + 2,
                        rect.Width - 8,
                        rect.Height - 4
                    );

                    g.DrawString(text, font, textBrush, textRect, format);

                    right -= widths[i];
                }
            }
        }
        private void DrawTotalRowRtl(
    Graphics g,
    string[] cells,
    int[] widths,
    int x,
    int y,
    int height,
    Font font)
        {
            StringFormat centerFormat = CreateArabicCenterFormat();
            StringFormat rightFormat = CreateArabicRightFormat();

            using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(221, 235, 247)))
            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            using (Pen borderPen = new Pen(Color.FromArgb(100, 100, 100)))
            {
                int right = x + widths.Sum();

                for (int i = 0; i < cells.Length; i++)
                {
                    Rectangle rect = new Rectangle(
                        right - widths[i],
                        y,
                        widths[i],
                        height
                    );

                    g.FillRectangle(backBrush, rect);
                    g.DrawRectangle(borderPen, rect);

                    string text = ShortPrintText(cells[i], 35);

                    StringFormat format = centerFormat;

                    if (i == 5)
                        format = rightFormat;

                    RectangleF textRect = new RectangleF(
                        rect.X + 4,
                        rect.Y + 2,
                        rect.Width - 8,
                        rect.Height - 4
                    );

                    g.DrawString(text, font, textBrush, textRect, format);

                    right -= widths[i];
                }
            }
        }
        private void DrawPrintSummaryBox(Graphics g, int x, int y, int width, Font font)
        {
            int boxHeight = 32;

            string summary =
                "الرصيد السابق: " + _currentResult.OpeningBalance.ToString("N2") +
                "    |    إجمالي الإيرادات: " + _currentResult.TotalRevenue.ToString("N2") +
                "    |    إجمالي المنصرفات: " + _currentResult.TotalExpense.ToString("N2") +
                "    |    الباقي: " + _currentResult.RemainingBalance.ToString("N2");

            Rectangle rect = new Rectangle(x, y, width, boxHeight);

            using (SolidBrush backBrush = new SolidBrush(Color.FromArgb(248, 250, 252)))
            using (Pen borderPen = new Pen(Color.FromArgb(160, 160, 160)))
            {
                g.FillRectangle(backBrush, rect);
                g.DrawRectangle(borderPen, rect);
            }

            g.DrawString(
                summary,
                font,
                Brushes.Black,
                new RectangleF(rect.X + 6, rect.Y + 2, rect.Width - 12, rect.Height - 4),
                CreateArabicCenterFormat()
            );
        }
        private StringFormat CreateArabicCenterFormat()
        {
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            format.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            format.Trimming = StringTrimming.EllipsisCharacter;
            return format;
        }

        private StringFormat CreateArabicRightFormat()
        {
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Near;
            format.LineAlignment = StringAlignment.Center;
            format.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            format.Trimming = StringTrimming.EllipsisCharacter;
            return format;
        }

        private string ShortPrintText(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            text = text.Replace("\r", " ").Replace("\n", " ").Trim();

            if (text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength) + "...";
        }

        private int[] ScaleColumnWidths(int[] baseWidths, int availableWidth)
        {
            int total = baseWidths.Sum();

            if (total <= 0)
                return baseWidths;

            int[] result = new int[baseWidths.Length];
            int used = 0;

            for (int i = 0; i < baseWidths.Length; i++)
            {
                result[i] = (int)Math.Round((baseWidths[i] / (double)total) * availableWidth);

                if (result[i] < 35)
                    result[i] = 35;

                used += result[i];
            }

            int diff = availableWidth - used;

            if (result.Length > 0)
                result[result.Length - 1] += diff;

            return result;
        }
        //private void DrawPrintRow(
        //        Graphics g,
        //        string[] cells,
        //        int[] widths,
        //        int x,
        //        int y,
        //        int height,
        //        Font font,
        //        StringFormat format,
        //        bool isHeader)
        //    {
        //        Brush backBrush = isHeader ? Brushes.LightGray : Brushes.White;
        //        Brush textBrush = Brushes.Black;

        //        int currentX = x;

        //        for (int i = 0; i < cells.Length; i++)
        //        {
        //            Rectangle rect = new Rectangle(currentX, y, widths[i], height);

        //            g.FillRectangle(backBrush, rect);
        //            g.DrawRectangle(Pens.Gray, rect);

        //            string text = cells[i] ?? string.Empty;

        //            if (text.Length > 35)
        //                text = text.Substring(0, 35) + "...";

        //            g.DrawString(text, font, textBrush, rect, format);

        //            currentX += widths[i];
        //        }
        //    }

        private Panel CreateField(string labelText, Control input, int width)
            {
                Panel panel = new Panel
                {
                    Width = width,
                    Height = 58,
                    Margin = new Padding(8, 0, 0, 0),
                    BackColor = CardBack
                };

                Label label = new Label
                {
                    Text = labelText,
                    Dock = DockStyle.Top,
                    Height = 22,
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = new Font("Tahoma", 8.8F, FontStyle.Bold),
                    ForeColor = TextDark
                };

                input.Dock = DockStyle.Bottom;
                input.Height = 28;
                input.Margin = new Padding(0);

                panel.Controls.Add(input);
                panel.Controls.Add(label);

                return panel;
            }

            private Button MakeButton(string text, int width, Color color)
            {
                Button btn = new Button
                {
                    Text = text,
                    Width = width,
                    Height = 32,
                    Margin = new Padding(8, 0, 0, 0),
                    BackColor = color,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Tahoma", 9F, FontStyle.Bold),
                    Cursor = Cursors.Hand,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(color);
                btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(color);

                return btn;
            }

            private Panel CreateCard()
            {
                Panel panel = new Panel
                {
                    Dock = DockStyle.Fill,
                    BackColor = CardBack
                };

                panel.Paint += Card_Paint;

                return panel;
            }

            private void Card_Paint(object sender, PaintEventArgs e)
            {
                Control c = sender as Control;

                if (c == null)
                    return;

                using (Pen pen = new Pen(Border))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
                }
            }

            private void DgvStatement_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0)
                    return;

                string propertyName = dgvStatement.Columns[e.ColumnIndex].DataPropertyName;

                if (propertyName == "MovementKind" && e.Value != null)
                {
                    string value = e.Value.ToString();

                    if (value == "إيراد")
                    {
                        e.CellStyle.ForeColor = Green;
                        e.CellStyle.BackColor = Color.FromArgb(220, 252, 231);
                        e.CellStyle.SelectionForeColor = Green;
                        e.CellStyle.SelectionBackColor = Color.FromArgb(187, 247, 208);
                    }
                    else
                    {
                        e.CellStyle.ForeColor = Red;
                        e.CellStyle.BackColor = Color.FromArgb(254, 226, 226);
                        e.CellStyle.SelectionForeColor = Red;
                        e.CellStyle.SelectionBackColor = Color.FromArgb(254, 202, 202);
                    }
                }

                if (propertyName == "Balance" && e.Value != null)
                {
                    decimal balance;

                    if (decimal.TryParse(e.Value.ToString(), out balance))
                        e.CellStyle.ForeColor = balance >= 0 ? Green : Red;
                }
            }

            private string GetSelectedLookupValue(ComboBox combo)
            {
                LookupItem item = combo.SelectedItem as LookupItem;

                if (item == null)
                    return null;

                return item.Value;
            }

            private class LookupItem
            {
                public string Text { get; private set; }
                public string Value { get; private set; }

                public LookupItem(string text, string value)
                {
                    Text = text;
                    Value = value;
                }

                public override string ToString()
                {
                    return Text;
                }
            }
        }
    }
