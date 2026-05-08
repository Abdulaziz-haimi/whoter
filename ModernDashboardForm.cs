using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using water3.Forms;
using water3.Reports;
using water3.Reports.Dynamic;

namespace water3.Forms
{
    public class ModernDashboardForm : Form
    {



            private readonly ProductionDashboardService _service = new ProductionDashboardService();

            private bool _isLoading;

            private TableLayoutPanel root;
            private Panel topCard;
            private TableLayoutPanel topLayout;
            private FlowLayoutPanel quickLinksPanel;
            private Label lblTitle;
            private Label lblSubtitle;
            private Label lblPeriod;
            private Label lblLastUpdate;
            private ComboBox cbPeriod;
            private DateTimePicker dtFrom;
            private DateTimePicker dtTo;
            private Button btnRefresh;

            private FlowLayoutPanel kpiPanel;
            private TabControl tabs;

            private DataGridView gridInvoices;
            private DataGridView gridPayments;
            private DataGridView gridDebtors;
            private DataGridView gridReadings;
            private DataGridView gridSms;
            private DataGridView gridMobile;
            private DataGridView gridExpenses;

            private Timer autoRefreshTimer;

            private static readonly Font FontRegular = new Font("Tahoma", 9.5f, FontStyle.Regular);
            private static readonly Font FontBold = new Font("Tahoma", 10f, FontStyle.Bold);
            private static readonly Font FontTitle = new Font("Tahoma", 14.5f, FontStyle.Bold);
            private static readonly Font FontSmall = new Font("Tahoma", 8.5f, FontStyle.Regular);
            private static readonly Font FontKpiTitle = new Font("Tahoma", 8.8f, FontStyle.Bold);
            private static readonly Font FontKpiValue = new Font("Tahoma", 15f, FontStyle.Bold);

            private AppThemePalette P
            {
                get { return AppThemeManager.Palette; }
            }

            public ModernDashboardForm()
            {
                AppThemeManager.LoadTheme();

                Text = "لوحة التحكم";
                Name = "ProductionDashboardForm";
                Font = FontRegular;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = false;
                AutoScaleMode = AutoScaleMode.Font;
                MinimumSize = new Size(1100, 700);

                BuildUi();
                ApplyTheme();
                InitPeriodItems();

                Load += ProductionDashboardForm_Load;
                FormClosing += ProductionDashboardForm_FormClosing;
            }

            private void ProductionDashboardForm_Load(object sender, EventArgs e)
            {
                autoRefreshTimer.Start();
                _ = RefreshDashboardAsync();
            }

            private void ProductionDashboardForm_FormClosing(object sender, FormClosingEventArgs e)
            {
                autoRefreshTimer.Stop();
            }

            public void RefreshTheme()
            {
                ApplyTheme();
                Invalidate(true);
            }

            private void BuildUi()
            {
                SuspendLayout();

                root = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 3,
                    Padding = new Padding(10),
                    RightToLeft = RightToLeft.No
                };

                root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                // صف علوي للفلاتر والروابط
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 126));

                // صف كروت المؤشرات
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 178));

                // صف الجداول
                root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

                topCard = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(12, 10, 12, 8),
                    Margin = new Padding(0, 0, 0, 8)
                };
                topCard.Paint += Card_Paint;

                topLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 1,
                    RowCount = 2,
                    RightToLeft = RightToLeft.No
                };

                topLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54));
                topLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));

                TableLayoutPanel filterLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RightToLeft = RightToLeft.No,
                    ColumnCount = 11,
                    RowCount = 1,
                    Margin = Padding.Empty,
                    Padding = Padding.Empty
                };

                filterLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

                filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));       // العنوان
                filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 135));      // القائمة
                filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52));       // الفترة
                filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 132));      // إلى
                filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));       // إلى:
                filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 132));      // من
                filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));       // من:
                filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 165));      // lblPeriod
                filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));      // آخر تحديث
                filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 8));        // مسافة
                filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 42));       // تحديث

                Panel titlePanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    RightToLeft = RightToLeft.Yes,
                    Padding = new Padding(0, 0, 8, 0)
                };

                lblTitle = new Label
                {
                    Dock = DockStyle.Top,
                    Height = 28,
                    Text = "لوحة التحكم التنفيذية",
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = FontTitle
                };

                lblSubtitle = new Label
                {
                    Dock = DockStyle.Fill,
                    Text = "ملخص التشغيل والتحصيل والمزامنة والرسائل",
                    TextAlign = ContentAlignment.TopRight,
                    Font = FontSmall
                };

                titlePanel.Controls.Add(lblSubtitle);
                titlePanel.Controls.Add(lblTitle);

                cbPeriod = new ComboBox
                {
                    Dock = DockStyle.Fill,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    RightToLeft = RightToLeft.Yes,
                    Margin = new Padding(4, 11, 4, 8),
                    FlatStyle = FlatStyle.Flat
                };
                cbPeriod.SelectedIndexChanged += cbPeriod_SelectedIndexChanged;

                Label lblPeriodText = CreateFilterLabel("الفترة:");

                dtTo = new DateTimePicker
                {
                    Dock = DockStyle.Fill,
                    Format = DateTimePickerFormat.Short,
                    RightToLeft = RightToLeft.Yes,
                    RightToLeftLayout = true,
                    Margin = new Padding(4, 11, 4, 8)
                };
                dtTo.ValueChanged += DateFilter_ValueChanged;

                Label lblTo = CreateFilterLabel("إلى:");

                dtFrom = new DateTimePicker
                {
                    Dock = DockStyle.Fill,
                    Format = DateTimePickerFormat.Short,
                    RightToLeft = RightToLeft.Yes,
                    RightToLeftLayout = true,
                    Margin = new Padding(4, 11, 4, 8)
                };
                dtFrom.ValueChanged += DateFilter_ValueChanged;

                Label lblFrom = CreateFilterLabel("من:");

                lblPeriod = new Label
                {
                    Dock = DockStyle.Fill,
                    Text = "📅 الفترة الحالية",
                    TextAlign = ContentAlignment.MiddleRight,
                    RightToLeft = RightToLeft.Yes,
                    Padding = new Padding(8, 0, 8, 0),
                    Margin = new Padding(4, 9, 4, 8),
                    AutoEllipsis = true
                };
                lblPeriod.Paint += BorderLabel_Paint;

                lblLastUpdate = new Label
                {
                    Dock = DockStyle.Fill,
                    Text = "آخر تحديث: --",
                    TextAlign = ContentAlignment.MiddleCenter,
                    RightToLeft = RightToLeft.Yes,
                    AutoEllipsis = true,
                    Font = FontSmall,
                    Margin = new Padding(4, 9, 4, 8)
                };

                btnRefresh = new Button
                {
                    Dock = DockStyle.Fill,
                    Text = "🔄",
                    Margin = new Padding(4, 9, 4, 8),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Font = new Font("Segoe UI Emoji", 11.5f, FontStyle.Regular)
                };
                btnRefresh.Click += btnRefresh_Click;

                filterLayout.Controls.Add(titlePanel, 0, 0);
                filterLayout.Controls.Add(cbPeriod, 1, 0);
                filterLayout.Controls.Add(lblPeriodText, 2, 0);
                filterLayout.Controls.Add(dtTo, 3, 0);
                filterLayout.Controls.Add(lblTo, 4, 0);
                filterLayout.Controls.Add(dtFrom, 5, 0);
                filterLayout.Controls.Add(lblFrom, 6, 0);
                filterLayout.Controls.Add(lblPeriod, 7, 0);
                filterLayout.Controls.Add(lblLastUpdate, 8, 0);
                filterLayout.Controls.Add(btnRefresh, 10, 0);

                quickLinksPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RightToLeft = RightToLeft.Yes,
                    FlowDirection = FlowDirection.RightToLeft,
                    WrapContents = false,
                    AutoScroll = false,
                    Padding = new Padding(0, 6, 0, 0),
                    Margin = Padding.Empty
                };

                AddQuickLink("المشتركين", "👥", () => OpenDialog<SubscriberForm>());
                AddQuickLink("الفواتير", "📄", () => OpenDialog<InvoicePrintPreviewForm>());
                AddQuickLink("قراءة", "🧮", () => OpenDialog<ReadingEntryForm>());
                AddQuickLink("السداد", "💰", () => OpenDialog<PaymentsForm>());
                AddQuickLink("كشف حساب", "📑", () => OpenDialog<AccountStatementForm>());
                AddQuickLink("غير المسددة", "⚠️", () => OpenDialog<UnpaidInvoicesReportForm>());
                AddQuickLink("الرسائل", "✉️", () => OpenDialog<SmsLogsForm>());
                AddQuickLink("المصروفات", "💸", () => OpenDialog<ExpensesManagementForm>());
                AddQuickLink("المزامنة", "📱", () => OpenDialog<FrmMobileSyncDashboard>());
                AddQuickLink("التعرفة", "⚙️", () => OpenDialog<BillingConstantsForm>());

                topLayout.Controls.Add(filterLayout, 0, 0);
                topLayout.Controls.Add(quickLinksPanel, 0, 1);
                topCard.Controls.Add(topLayout);

                kpiPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.RightToLeft,
                    RightToLeft = RightToLeft.Yes,
                    WrapContents = true,
                    AutoScroll = false,
                    Padding = new Padding(0, 4, 0, 4),
                    Margin = new Padding(0, 0, 0, 8)
                };

                tabs = new TabControl
                {
                    Dock = DockStyle.Fill,
                    RightToLeft = RightToLeft.Yes,
                    RightToLeftLayout = false,
                    DrawMode = TabDrawMode.OwnerDrawFixed,
                    SizeMode = TabSizeMode.Fixed,
                    ItemSize = new Size(160, 40),
                    Padding = new Point(12, 4),
                    Margin = Padding.Empty
                };
                tabs.DrawItem += Tabs_DrawItem;

                gridInvoices = CreateGrid();
                gridPayments = CreateGrid();
                gridDebtors = CreateGrid();
                gridReadings = CreateGrid();
                gridSms = CreateGrid();
                gridMobile = CreateGrid();
                gridExpenses = CreateGrid();

                tabs.TabPages.Add(CreateTab("📄 الفواتير", gridInvoices));
                tabs.TabPages.Add(CreateTab("💰 التحصيلات", gridPayments));
                tabs.TabPages.Add(CreateTab("⚠️ المديونيات", gridDebtors));
                tabs.TabPages.Add(CreateTab("🧮 القراءات", gridReadings));
                tabs.TabPages.Add(CreateTab("✉️ الرسائل", gridSms));
                tabs.TabPages.Add(CreateTab("📱 المزامنة", gridMobile));
                tabs.TabPages.Add(CreateTab("💸 المصروفات", gridExpenses));

                autoRefreshTimer = new Timer
                {
                    Interval = 300000
                };
                autoRefreshTimer.Tick += (s, e) => _ = RefreshDashboardAsync();

                root.Controls.Add(topCard, 0, 0);
                root.Controls.Add(kpiPanel, 0, 1);
                root.Controls.Add(tabs, 0, 2);

                Controls.Add(root);

                ResumeLayout(true);
            }

            private Label CreateFilterLabel(string text)
            {
                return new Label
                {
                    Dock = DockStyle.Fill,
                    Text = text,
                    TextAlign = ContentAlignment.MiddleCenter,
                    RightToLeft = RightToLeft.Yes,
                    Font = FontBold
                };
            }

            private void AddQuickLink(string text, string icon, Action action)
            {
                Button btn = new Button
                {
                    Width = 108,
                    Height = 34,
                    Margin = new Padding(3, 2, 3, 2),
                    Text = icon + " " + text,
                    TextAlign = ContentAlignment.MiddleCenter,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Font = new Font("Tahoma", 8.7f, FontStyle.Bold),
                    Tag = "QuickLink"
                };

                btn.FlatAppearance.BorderSize = 1;
                btn.Click += (s, e) => action();

                quickLinksPanel.Controls.Add(btn);
            }

            private void OpenDialog<T>() where T : Form, new()
            {
                try
                {
                    using (Form host = new Form())
                    using (T child = new T())
                    {
                        host.Text = GetArabicWindowTitle(typeof(T).Name);
                        host.StartPosition = FormStartPosition.CenterParent;
                        host.Size = new Size(1100, 700);
                        host.MinimumSize = new Size(900, 600);
                        host.RightToLeft = RightToLeft.Yes;
                        host.RightToLeftLayout = true;
                        host.FormBorderStyle = FormBorderStyle.Sizable;
                        host.ControlBox = true;
                        host.MinimizeBox = false;
                        host.MaximizeBox = true;
                        host.ShowIcon = false;
                        host.ShowInTaskbar = false;
                        host.BackColor = P.Bg;
                        host.Font = FontRegular;

                        Panel header = new Panel
                        {
                            Dock = DockStyle.Top,
                            Height = 46,
                            Padding = new Padding(10, 7, 10, 7),
                            BackColor = P.Primary,
                            RightToLeft = RightToLeft.No
                        };

                        Label title = new Label
                        {
                            Dock = DockStyle.Fill,
                            Text = host.Text,
                            ForeColor = Color.White,
                            Font = new Font("Tahoma", 10.5f, FontStyle.Bold),
                            TextAlign = ContentAlignment.MiddleRight,
                            RightToLeft = RightToLeft.Yes
                        };

                        Button btnClose = new Button
                        {
                            Dock = DockStyle.Left,
                            Width = 90,
                            Text = "إغلاق ×",
                            Cursor = Cursors.Hand,
                            FlatStyle = FlatStyle.Flat,
                            BackColor = P.PrimaryDark,
                            ForeColor = Color.White,
                            Font = new Font("Tahoma", 9.5f, FontStyle.Bold)
                        };

                        btnClose.FlatAppearance.BorderSize = 0;
                        btnClose.Click += (s, e) => host.Close();

                        header.Controls.Add(title);
                        header.Controls.Add(btnClose);

                        Panel body = new Panel
                        {
                            Dock = DockStyle.Fill,
                            Padding = new Padding(8),
                            BackColor = P.Bg
                        };

                        child.TopLevel = false;
                        child.FormBorderStyle = FormBorderStyle.None;
                        child.Dock = DockStyle.Fill;
                        child.RightToLeft = RightToLeft.Yes;
                        child.RightToLeftLayout = true;

                        body.Controls.Add(child);

                        host.Controls.Add(body);
                        host.Controls.Add(header);

                        child.Show();

                        host.ShowDialog(this);
                    }

                    _ = RefreshDashboardAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "تعذر فتح الشاشة المطلوبة.\n\n" + ex.Message,
                        "خطأ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            private string GetArabicWindowTitle(string formName)
            {
                switch (formName)
                {
                    case "SubscriberForm":
                        return "إدارة المشتركين";

                    case "InvoiceForm":
                        return "إدارة الفواتير";

                    case "ReadingEntryForm":
                        return "إدخال قراءة";

                    case "PaymentsForm":
                        return "إدارة السداد";

                    case "AccountStatementForm":
                        return "كشف حساب";

                    case "UnpaidInvoicesReportForm":
                        return "الفواتير غير المسددة";

                    case "SmsLogsForm":
                        return "سجل الرسائل";

                    case "ExpensesManagementForm":
                        return "إدارة المصروفات";

                    case "FrmMobileSyncDashboard":
                        return "مزامنة الجوال";

                    case "BillingConstantsForm":
                        return "إعدادات التعرفة";

                    default:
                        return "نافذة";
                }
            }

            private TabPage CreateTab(string title, DataGridView grid)
            {
                TabPage tab = new TabPage(title)
                {
                    RightToLeft = RightToLeft.Yes,
                    Padding = new Padding(8)
                };

                Panel card = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(8)
                };

                card.Paint += Card_Paint;
                card.Controls.Add(grid);
                tab.Controls.Add(card);

                return tab;
            }

            private DataGridView CreateGrid()
            {
                DataGridView dgv = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    RightToLeft = RightToLeft.Yes,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    AllowUserToResizeRows = false,
                    AllowUserToOrderColumns = false,
                    MultiSelect = false,
                    RowHeadersVisible = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    AutoGenerateColumns = true,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    BorderStyle = BorderStyle.None,
                    EnableHeadersVisualStyles = false,
                    CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                    ColumnHeadersHeight = 38,
                    RowTemplate = { Height = 34 },
                    Margin = Padding.Empty,
                    ScrollBars = ScrollBars.Both
                };

                typeof(DataGridView)
                    .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.SetValue(dgv, true, null);

                return dgv;
            }
            private void ApplyTheme()
            {
                BackColor = P.Bg;
                root.BackColor = P.Bg;
                topCard.BackColor = P.Card;
                kpiPanel.BackColor = P.Bg;
                tabs.BackColor = P.Card;

                lblTitle.ForeColor = P.Text;
                lblSubtitle.ForeColor = P.Muted;

                lblLastUpdate.ForeColor = P.Muted;
                lblLastUpdate.BackColor = P.Soft;

                lblPeriod.BackColor = P.Soft;
                lblPeriod.ForeColor = P.Muted;

                cbPeriod.BackColor = P.Card;
                cbPeriod.ForeColor = P.Text;
                cbPeriod.Font = FontRegular;

                dtFrom.CalendarTitleBackColor = P.Primary;
                dtFrom.CalendarTitleForeColor = Color.White;

                dtTo.CalendarTitleBackColor = P.Primary;
                dtTo.CalendarTitleForeColor = Color.White;

                btnRefresh.BackColor = P.Primary;
                btnRefresh.ForeColor = Color.White;
                btnRefresh.FlatAppearance.BorderSize = 0;
                btnRefresh.FlatAppearance.MouseOverBackColor = P.Hover;
                btnRefresh.FlatAppearance.MouseDownBackColor = P.PrimaryDark;

                foreach (Control c in topCard.Controls)
                    ApplyThemeRecursive(c);

                StyleGrid(gridInvoices);
                StyleGrid(gridPayments);
                StyleGrid(gridDebtors);
                StyleGrid(gridReadings);
                StyleGrid(gridSms);
                StyleGrid(gridMobile);
                StyleGrid(gridExpenses);

                foreach (TabPage tab in tabs.TabPages)
                {
                    tab.BackColor = P.Card;

                    foreach (Control c in tab.Controls)
                    {
                        c.BackColor = P.Card;
                        c.Invalidate();
                    }
                }

                topCard.Invalidate();
                lblPeriod.Invalidate();
                lblLastUpdate.Invalidate();
                tabs.Invalidate();
            }

            private void ApplyThemeRecursive(Control parent)
            {
                foreach (Control c in parent.Controls)
                {
                    if (c is Label label &&
                        label != lblTitle &&
                        label != lblSubtitle &&
                        label != lblPeriod &&
                        label != lblLastUpdate)
                    {
                        label.ForeColor = P.Text;
                        label.BackColor = P.Card;
                    }

                    if (c is Button btn && Convert.ToString(btn.Tag) == "QuickLink")
                    {
                        btn.BackColor = P.Soft;
                        btn.ForeColor = P.Text;
                        btn.FlatAppearance.BorderColor = P.Border;
                        btn.FlatAppearance.BorderSize = 1;
                        btn.FlatAppearance.MouseOverBackColor = P.Hover;
                        btn.FlatAppearance.MouseDownBackColor = P.PrimaryDark;
                    }

                    if (c.HasChildren)
                        ApplyThemeRecursive(c);
                }
            }

            private void StyleGrid(DataGridView dgv)
            {
                dgv.BackgroundColor = P.Card;
                dgv.GridColor = P.Border;

                dgv.ColumnHeadersDefaultCellStyle.BackColor =
                    AppThemeManager.CurrentTheme == AppThemeName.Dark
                        ? P.Soft
                        : Color.FromArgb(245, 248, 255);

                dgv.ColumnHeadersDefaultCellStyle.ForeColor = P.Text;
                dgv.ColumnHeadersDefaultCellStyle.Font = FontBold;
                dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(4);

                dgv.DefaultCellStyle.BackColor = P.Card;
                dgv.DefaultCellStyle.ForeColor = P.Text;
                dgv.DefaultCellStyle.SelectionBackColor = P.Selected;
                dgv.DefaultCellStyle.SelectionForeColor = P.SelectedText;
                dgv.DefaultCellStyle.Font = FontRegular;
                dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.DefaultCellStyle.Padding = new Padding(3);

                dgv.AlternatingRowsDefaultCellStyle.BackColor =
                    AppThemeManager.CurrentTheme == AppThemeName.Dark
                        ? P.Bg
                        : P.Soft;

                dgv.AlternatingRowsDefaultCellStyle.ForeColor = P.Text;
            }
            private void InitPeriodItems()
            {
                cbPeriod.Items.Clear();
                cbPeriod.Items.AddRange(new object[]
                {
                "اليوم",
                "أمس",
                "الأسبوع الحالي",
                "الشهر الحالي",
                "الشهر الماضي",
                "الربع الحالي",
                "السنة الحالية",
                "فترة مخصصة"
                });

                cbPeriod.SelectedIndex = 3;
                ApplyPeriodPreset();
            }

            private void cbPeriod_SelectedIndexChanged(object sender, EventArgs e)
            {
                ApplyPeriodPreset();

                if (IsHandleCreated)
                    _ = RefreshDashboardAsync();
            }

            private void DateFilter_ValueChanged(object sender, EventArgs e)
            {
                if (cbPeriod.SelectedIndex != 7)
                    return;

                if (IsHandleCreated)
                    _ = RefreshDashboardAsync();
            }

            private void btnRefresh_Click(object sender, EventArgs e)
            {
                _ = RefreshDashboardAsync();
            }

            private void ApplyPeriodPreset()
            {
                DateTime now = DateTime.Now;

                switch (cbPeriod.SelectedIndex)
                {
                    case 0:
                        dtFrom.Value = now.Date;
                        dtTo.Value = now.Date;
                        break;

                    case 1:
                        dtFrom.Value = now.AddDays(-1).Date;
                        dtTo.Value = now.AddDays(-1).Date;
                        break;

                    case 2:
                        int diff = ((int)now.DayOfWeek + 6) % 7;
                        dtFrom.Value = now.Date.AddDays(-diff);
                        dtTo.Value = now.Date;
                        break;

                    case 3:
                        dtFrom.Value = new DateTime(now.Year, now.Month, 1);
                        dtTo.Value = now.Date;
                        break;

                    case 4:
                        DateTime firstThisMonth = new DateTime(now.Year, now.Month, 1);
                        dtFrom.Value = firstThisMonth.AddMonths(-1);
                        dtTo.Value = firstThisMonth.AddDays(-1);
                        break;

                    case 5:
                        int q = (now.Month - 1) / 3;
                        dtFrom.Value = new DateTime(now.Year, q * 3 + 1, 1);
                        dtTo.Value = now.Date;
                        break;

                    case 6:
                        dtFrom.Value = new DateTime(now.Year, 1, 1);
                        dtTo.Value = now.Date;
                        break;

                    case 7:
                    default:
                        break;
                }
            }

            private async Task RefreshDashboardAsync()
            {
                if (_isLoading || IsDisposed)
                    return;

                try
                {
                    _isLoading = true;
                    Cursor = Cursors.WaitCursor;
                    btnRefresh.Enabled = false;
                    lblPeriod.Text = "🔄 جاري تحديث البيانات...";

                    DateTime from = dtFrom.Value.Date;
                    DateTime to = dtTo.Value.Date;

                    if (from > to)
                    {
                        MessageBox.Show("تاريخ البداية يجب أن يكون قبل تاريخ النهاية.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    ProductionDashboardData data = await Task.Run(() => _service.Load(from, to));

                    if (IsDisposed)
                        return;

                    RenderKpis(data.Summary);

                    BindGrid(gridInvoices, data.LastInvoices);
                    BindGrid(gridPayments, data.LastPayments);
                    BindGrid(gridDebtors, data.TopDebtors);
                    BindGrid(gridReadings, data.LastReadings);
                    BindGrid(gridSms, data.SmsLogs);
                    BindGrid(gridMobile, data.MobileSync);
                    BindGrid(gridExpenses, data.Expenses);

                    lblPeriod.Text = string.Format("📅 {0:yyyy/MM/dd} إلى {1:yyyy/MM/dd}", from, to);
                    lblLastUpdate.Text = "آخر تحديث: " + DateTime.Now.ToString("HH:mm:ss");
                }
                catch (Exception ex)
                {
                    lblPeriod.Text = "❌ فشل تحميل البيانات";

                    MessageBox.Show(
                        "حدث خطأ أثناء تحميل لوحة التحكم.\n\n" + ex.Message,
                        "خطأ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                finally
                {
                    btnRefresh.Enabled = true;
                    Cursor = Cursors.Default;
                    _isLoading = false;
                }
            }

            private void BindGrid(DataGridView dgv, DataTable table)
            {
                dgv.DataSource = null;
                dgv.DataSource = table;
                AutoFormatColumns(dgv);
                ApplyConditionalGridStyle(dgv);
            }

            private void AutoFormatColumns(DataGridView dgv)
            {
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;

                    string header = (col.HeaderText ?? string.Empty).Trim().ToLowerInvariant();

                    if (header.Contains("تاريخ") || header.Contains("date"))
                        col.DefaultCellStyle.Format = "yyyy/MM/dd";

                    if (header.Contains("مبلغ") ||
                        header.Contains("إجمالي") ||
                        header.Contains("رصيد") ||
                        header.Contains("مدفوع") ||
                        header.Contains("متبقي") ||
                        header.Contains("مستحق") ||
                        header.Contains("استهلاك"))
                    {
                        col.DefaultCellStyle.Format = "N2";
                    }
                }
            }

            private void ApplyConditionalGridStyle(DataGridView dgv)
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    string status = GetRowText(row, "الحالة");

                    if (status.Contains("Failed") || status.Contains("فشل") || status.Contains("مرفوض"))
                    {
                        row.DefaultCellStyle.ForeColor = P.Danger;
                        row.DefaultCellStyle.Font = FontBold;
                    }
                    else if (status.Contains("Pending") || status.Contains("New") || status.Contains("غير مدفوعة"))
                    {
                        row.DefaultCellStyle.ForeColor = P.Warning;
                        row.DefaultCellStyle.Font = FontBold;
                    }
                    else if (status.Contains("Sent") || status.Contains("مدفوعة") || status.Contains("Posted"))
                    {
                        row.DefaultCellStyle.ForeColor = P.Success;
                    }
                }
            }

            private string GetRowText(DataGridViewRow row, string columnName)
            {
                if (row.DataGridView.Columns.Contains(columnName))
                {
                    object value = row.Cells[columnName].Value;
                    return value == null || value == DBNull.Value ? string.Empty : value.ToString();
                }

                return string.Empty;
            }

            private void RenderKpis(ProductionDashboardSummary s)
            {
                kpiPanel.SuspendLayout();

                foreach (Control c in kpiPanel.Controls)
                    c.Dispose();

                kpiPanel.Controls.Clear();

                decimal collectionRate = 0;

                if (s.BilledAmount > 0)
                    collectionRate = Math.Round((s.CollectedAmount / s.BilledAmount) * 100m, 1);

                AddKpi("المشتركين النشطين", s.ActiveSubscribers.ToString("N0"), "👥", P.Accent, "إجمالي المشتركين");
                AddKpi("العدادات النشطة", s.ActiveMeters.ToString("N0"), "🚰", P.Primary, "عدادات فعالة");
                AddKpi("فواتير الفترة", s.InvoiceCount.ToString("N0"), "📄", P.Success, s.BilledAmount.ToString("N2") + " ريال");
                AddKpi("تحصيلات الفترة", s.CollectedAmount.ToString("N2"), "💰", P.Warning, "الإيصالات: " + s.ReceiptCount.ToString("N0"));
                AddKpi("نسبة التحصيل", collectionRate.ToString("N1") + "%", "📊", Color.FromArgb(126, 87, 194), "مقارنة بالفوترة");
                AddKpi("المديونية الحالية", s.OutstandingAmount.ToString("N2"), "⚠️", P.Danger, "مدينون: " + s.DebtorsCount.ToString("N0"));
                AddKpi("الرصيد المقدم", s.AdvanceCreditAmount.ToString("N2"), "💳", Color.FromArgb(0, 150, 136), "رصيد متبقٍ");
                AddKpi("قراءات الفترة", s.ReadingsCount.ToString("N0"), "🧮", P.Accent, "استهلاك: " + s.TotalConsumption.ToString("N2"));
                AddKpi("رسائل SMS", s.SmsTotal.ToString("N0"), "✉️", P.Primary, "مرسل: " + s.SmsSent + " | فشل: " + s.SmsFailed);
                AddKpi("مزامنة معلقة", s.MobilePending.ToString("N0"), "📱", P.Warning, "تحتاج متابعة");
                //AddKpi("المصروفات", s.ExpenseAmount.ToString("N2"), "💸", P.Danger, "عدد: " + s.ExpenseCount.ToString("N0"));
                //AddKpi("الفاقد آخر تقرير", s.LastWaterLoss.ToString("N2"), "💧", Color.FromArgb(3, 169, 244), "فرق العداد الرئيسي");

                kpiPanel.ResumeLayout(true);
            }

            private void AddKpi(string title, string value, string icon, Color accent, string subTitle)
            {
                Panel card = new Panel
                {
                    Width = 224,
                    Height = 76,
                    Margin = new Padding(5),
                    Padding = new Padding(8),
                    BackColor = P.Card,
                    Cursor = Cursors.Hand
                };

                card.Paint += (s, e) =>
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                    using (Pen border = new Pen(P.Border))
                        e.Graphics.DrawRectangle(border, 0, 0, card.Width - 1, card.Height - 1);

                    using (SolidBrush top = new SolidBrush(accent))
                        e.Graphics.FillRectangle(top, 0, 0, card.Width, 4);
                };

                Label lblIcon = new Label
                {
                    Text = icon,
                    Font = new Font("Segoe UI Emoji", 18f),
                    Size = new Size(42, 38),
                    Location = new Point(10, 16),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = accent,
                    BackColor = Color.Transparent
                };

                Label lblT = new Label
                {
                    Text = title,
                    Font = FontKpiTitle,
                    ForeColor = P.Muted,
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleRight,
                    Location = new Point(58, 5),
                    Size = new Size(156, 18)
                };

                Label lblV = new Label
                {
                    Text = value,
                    Font = new Font("Tahoma", 13.2f, FontStyle.Bold),
                    ForeColor = accent,
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleRight,
                    Location = new Point(58, 23),
                    Size = new Size(156, 27)
                };

                Label lblS = new Label
                {
                    Text = subTitle,
                    Font = FontSmall,
                    ForeColor = P.Muted,
                    BackColor = Color.Transparent,
                    TextAlign = ContentAlignment.MiddleRight,
                    Location = new Point(58, 51),
                    Size = new Size(156, 17),
                    AutoEllipsis = true
                };

                card.Controls.Add(lblIcon);
                card.Controls.Add(lblT);
                card.Controls.Add(lblV);
                card.Controls.Add(lblS);

                card.MouseEnter += (s, e) => card.BackColor = P.Soft;
                card.MouseLeave += (s, e) => card.BackColor = P.Card;

                kpiPanel.Controls.Add(card);
            }
            private void Tabs_DrawItem(object sender, DrawItemEventArgs e)
            {
                TabPage page = tabs.TabPages[e.Index];
                Rectangle rect = tabs.GetTabRect(e.Index);
                bool selected = tabs.SelectedIndex == e.Index;

                Color back = selected ? P.Primary : P.Soft;
                Color fore = selected ? Color.White : P.Text;

                using (SolidBrush b = new SolidBrush(back))
                    e.Graphics.FillRectangle(b, rect);

                using (Pen pen = new Pen(selected ? P.Primary : P.Border))
                    e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

                if (selected)
                {
                    using (Pen accentPen = new Pen(P.Accent, 3))
                        e.Graphics.DrawLine(accentPen, rect.Left + 10, rect.Bottom - 3, rect.Right - 10, rect.Bottom - 3);
                }

                TextRenderer.DrawText(
                    e.Graphics,
                    page.Text,
                    FontBold,
                    rect,
                    fore,
                    TextFormatFlags.HorizontalCenter |
                    TextFormatFlags.VerticalCenter |
                    TextFormatFlags.RightToLeft |
                    TextFormatFlags.EndEllipsis);
            }
            private void Card_Paint(object sender, PaintEventArgs e)
            {
                Control c = sender as Control;
                if (c == null) return;

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (SolidBrush shadow = new SolidBrush(
                    AppThemeManager.CurrentTheme == AppThemeName.Dark
                        ? Color.FromArgb(25, 0, 0, 0)
                        : Color.FromArgb(8, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadow, 2, 2, c.Width - 2, c.Height - 2);
                }

                using (Pen pen = new Pen(P.Border))
                    e.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
            }

            private void BorderLabel_Paint(object sender, PaintEventArgs e)
            {
                Control c = sender as Control;
                if (c == null) return;

                using (Pen pen = new Pen(P.Border))
                    e.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
            }

        }

        public class ProductionDashboardData
        {
            public ProductionDashboardSummary Summary { get; set; }
            public DataTable LastInvoices { get; set; }
            public DataTable LastPayments { get; set; }
            public DataTable TopDebtors { get; set; }
            public DataTable LastReadings { get; set; }
            public DataTable SmsLogs { get; set; }
            public DataTable MobileSync { get; set; }
            public DataTable Expenses { get; set; }
        }

        public class ProductionDashboardSummary
        {
            public int ActiveSubscribers { get; set; }
            public int ActiveMeters { get; set; }
            public int InvoiceCount { get; set; }
            public decimal BilledAmount { get; set; }
            public int ReceiptCount { get; set; }
            public decimal CollectedAmount { get; set; }
            public int DebtorsCount { get; set; }
            public decimal OutstandingAmount { get; set; }
            public decimal AdvanceCreditAmount { get; set; }
            public int ReadingsCount { get; set; }
            public decimal TotalConsumption { get; set; }
            public int SmsTotal { get; set; }
            public int SmsSent { get; set; }
            public int SmsFailed { get; set; }
            public int SmsPending { get; set; }
            public int MobilePending { get; set; }
            public int ExpenseCount { get; set; }
            public decimal ExpenseAmount { get; set; }
            public decimal LastWaterLoss { get; set; }
        }

        public class ProductionDashboardService
        {
            private readonly string _connectionString;

            public ProductionDashboardService()
            {
                _connectionString = GetConnectionString();
            }

            private static string GetConnectionString()
            {
                string[] names =
                {
                "WaterBillingDB",
                "DefaultConnection",
                "water3.Properties.Settings.WaterBillingDBConnectionString"
            };

                foreach (string name in names)
                {
                    ConnectionStringSettings cs = ConfigurationManager.ConnectionStrings[name];

                    if (cs != null && !string.IsNullOrWhiteSpace(cs.ConnectionString))
                        return cs.ConnectionString;
                }

                throw new InvalidOperationException(
                    "لم يتم العثور على ConnectionString. أضف connectionStrings باسم WaterBillingDB في App.config.");
            }

            public ProductionDashboardData Load(DateTime from, DateTime to)
            {
                DataSet ds = new DataSet();

                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("dbo.Dashboard_GetProductionSnapshot", con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 45;

                    cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = from.Date;
                    cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = to.Date;

                    da.Fill(ds);
                }

                ProductionDashboardData data = new ProductionDashboardData();
                data.Summary = ReadSummary(GetTable(ds, 0));
                data.LastInvoices = GetTable(ds, 1);
                data.LastPayments = GetTable(ds, 2);
                data.TopDebtors = GetTable(ds, 3);
                data.LastReadings = GetTable(ds, 4);
                data.SmsLogs = GetTable(ds, 5);
                data.MobileSync = GetTable(ds, 6);
                data.Expenses = GetTable(ds, 7);

                return data;
            }

            private static DataTable GetTable(DataSet ds, int index)
            {
                if (ds.Tables.Count > index)
                    return ds.Tables[index];

                return new DataTable();
            }

            private static ProductionDashboardSummary ReadSummary(DataTable table)
            {
                ProductionDashboardSummary s = new ProductionDashboardSummary();

                if (table.Rows.Count == 0)
                    return s;

                DataRow r = table.Rows[0];

                s.ActiveSubscribers = ToInt(r["ActiveSubscribers"]);
                s.ActiveMeters = ToInt(r["ActiveMeters"]);
                s.InvoiceCount = ToInt(r["InvoiceCount"]);
                s.BilledAmount = ToDecimal(r["BilledAmount"]);
                s.ReceiptCount = ToInt(r["ReceiptCount"]);
                s.CollectedAmount = ToDecimal(r["CollectedAmount"]);
                s.DebtorsCount = ToInt(r["DebtorsCount"]);
                s.OutstandingAmount = ToDecimal(r["OutstandingAmount"]);
                s.AdvanceCreditAmount = ToDecimal(r["AdvanceCreditAmount"]);
                s.ReadingsCount = ToInt(r["ReadingsCount"]);
                s.TotalConsumption = ToDecimal(r["TotalConsumption"]);
                s.SmsTotal = ToInt(r["SmsTotal"]);
                s.SmsSent = ToInt(r["SmsSent"]);
                s.SmsFailed = ToInt(r["SmsFailed"]);
                s.SmsPending = ToInt(r["SmsPending"]);
                s.MobilePending = ToInt(r["MobilePending"]);
                s.ExpenseCount = ToInt(r["ExpenseCount"]);
                s.ExpenseAmount = ToDecimal(r["ExpenseAmount"]);
                s.LastWaterLoss = ToDecimal(r["LastWaterLoss"]);

                return s;
            }

            private static int ToInt(object value)
            {
                if (value == null || value == DBNull.Value)
                    return 0;

                return Convert.ToInt32(value);
            }

            private static decimal ToDecimal(object value)
            {
                if (value == null || value == DBNull.Value)
                    return 0m;

                return Convert.ToDecimal(value);
            }
        }
    }