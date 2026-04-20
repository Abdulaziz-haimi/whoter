using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using water3.DB;
using water3.Forms;
using water3.Reports;

namespace water3
{
    public partial class Form1 : Form
    {
        // Cache للفورمات المفتوحة
        private readonly Dictionary<Type, Form> formsCache = new Dictionary<Type, Form>();
        private Button _activeButton;
        private bool _sidebarCollapsed = false;

        // ===== Theme =====
        private readonly Color Primary = Color.FromArgb(0, 87, 183);
        private readonly Color PrimaryDark = Color.FromArgb(0, 70, 150);
        private readonly Color Hover = Color.FromArgb(15, 105, 205);
        private readonly Color Bg = Color.FromArgb(245, 247, 250);
        private readonly Color Card = Color.White;
        private readonly Color Border = Color.FromArgb(225, 230, 235);
        private readonly Color Selected = Color.White;
        private readonly Color SelectedText = Color.FromArgb(0, 87, 183);

        public Form1()
        {
            InitializeComponent();

            // خصائص عامة
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            WindowState = FormWindowState.Maximized;
            Font = new Font("Segoe UI", 10f);
            BackColor = Bg;
            KeyPreview = true;

            // تلوين عناصر التصميم
            ApplyThemeAndBaseStyles();

            // بناء عناصر السايدبار ديناميكياً (الأزرار) — خارج Designer
            BuildSidebarContent();

            // إعداد شريط الحالة
            stClock.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            stVersion.Text = "v" + Application.ProductVersion;

            updateTimer.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDashboard();
        }

        private void ApplyThemeAndBaseStyles()
        {
            layoutRoot.BackColor = Bg;

            topBar.BackColor = Card;

            btnToggleSidebar.BackColor = Color.White;
            btnToggleSidebar.ForeColor = PrimaryDark;
            btnToggleSidebar.FlatAppearance.BorderColor = Border;
            btnToggleSidebar.FlatAppearance.BorderSize = 1;
            btnToggleSidebar.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 248, 252);

            sidebar.BackColor = Primary;

            contentHost.BackColor = Bg;
            contentCard.BackColor = Card;

            statusBar.BackColor = Color.White;
        }

        // ====== Paint events (بدون Lambda في Designer) ======
        private void topBar_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(Border))
                e.Graphics.DrawLine(pen, 0, topBar.Height - 1, topBar.Width, topBar.Height - 1);
        }

        private void contentCard_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var pen = new Pen(Border))
                e.Graphics.DrawRectangle(pen, 0, 0, contentCard.Width - 1, contentCard.Height - 1);
        }

        // ====== Timer tick ======
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            stClock.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        // ====== Toggle sidebar ======
        //private void btnToggleSidebar_Click(object sender, EventArgs e)
        //{
        //    _sidebarCollapsed = !_sidebarCollapsed;
        //    layoutRoot.ColumnStyles[1].Width = _sidebarCollapsed ? 0 : 290;
        //    sidebar.Visible = !_sidebarCollapsed;
        //}
        private void btnToggleSidebar_Click(object sender, EventArgs e)
        {
            _sidebarCollapsed = !_sidebarCollapsed;

            layoutRoot.SuspendLayout();
            try
            {
                layoutRoot.ColumnStyles[1].Width = _sidebarCollapsed ? 0 : 290;
                sidebar.Visible = !_sidebarCollapsed;
                contentHost.PerformLayout();
                contentHost.Invalidate(true);

                if (mainTabControl.SelectedTab?.Tag is Form currentForm && !currentForm.IsDisposed)
                {
                    currentForm.SuspendLayout();
                    try
                    {
                        currentForm.Dock = DockStyle.Fill;
                        currentForm.WindowState = FormWindowState.Normal;

                        if (currentForm is SubscriberForm subscriberForm)
                            subscriberForm.ApplyLayoutAfterSidebarToggle();
                        else
                        {
                            currentForm.PerformLayout();
                            currentForm.Invalidate(true);
                        }
                    }
                    finally
                    {
                        currentForm.ResumeLayout(true);
                    }
                }
            }
            finally
            {
                layoutRoot.ResumeLayout(true);
            }
        }
        // ====== Tabs Selected changed ======
        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectedTabForm();
        }

        // ================== SIDEBAR CONTENT ==================
        private void BuildSidebarContent()
        {
            sidebar.SuspendLayout();
            sidebar.Controls.Clear();

            // Header
            var header = new Panel
            {
                Width = 255,
                Height = 86,
                BackColor = PrimaryDark,
                Margin = new Padding(0, 0, 0, 12),
                Padding = new Padding(12)
            };
            header.Paint += Header_Paint;

            var headerTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 26,
                Text = "القائمة الرئيسية",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };

            var headerSub = new Label
            {
                Dock = DockStyle.Fill,
                Text = "إدارة الفواتير والمشتركين والتحصيل",
                ForeColor = Color.FromArgb(220, 255, 255, 255),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                TextAlign = ContentAlignment.TopRight
            };

            header.Controls.Add(headerSub);
            header.Controls.Add(headerTitle);
            sidebar.Controls.Add(header);

            // Nav buttons (كما عندك)
            AddNavButton("📊 لوحة التحكم", typeof(DashboardForm), () => OpenForm<DashboardForm>());
            AddNavButton("📄 الفواتير", typeof(InvoiceForm), () => OpenForm<InvoiceForm>());
            AddNavButton("👥 المشتركين", typeof(SubscriberForm), () => OpenForm<SubscriberForm>());
            AddNavButton("🧾 المدفوعات", typeof(PaymentsForm), () => OpenForm<PaymentsForm>());
            AddNavButton("🧮 قراءة العدادات", typeof(ReadingF), () => OpenForm<ReadingF>());
            AddNavButton("📌 تقرير المشتركين", typeof(SubscribersBillingReportForm), () => OpenForm<SubscribersBillingReportForm>());
            AddNavButton("📈 تقرير تحصيلات المحصلين", typeof(CollectorsReportForm), () => OpenForm<CollectorsReportForm>());
            AddNavButton("📑 كشف حساب مشترك", typeof(AccountStatementForm), () => OpenForm<AccountStatementForm>());
            AddNavButton("🖨️ طباعة الفواتير", typeof(InvoicePrintPreviewForm), () => OpenForm<InvoicePrintPreviewForm>());
            AddNavButton("👤 إدارة المحصلين", typeof(CollectorsForm), () => OpenForm<CollectorsForm>());
            AddNavButton("⚙️ إدارة الثوابت", typeof(BillingConstantsForm), () => OpenForm<BillingConstantsForm>());
            AddNavButton("✉️ سجل الرسائل", typeof(SmsLogsForm), () => OpenForm<SmsLogsForm>());
            AddNavButton("📨 تقرير الرسائل", typeof(SmsReportForm), () => OpenForm<SmsReportForm>());
            AddNavButton("🧩 إدارة الرسائل", typeof(MessagesManagementForm), () => OpenForm<MessagesManagementForm>());
            AddNavButton("🧩 إدارة الاتصال", typeof(DbSettingsForm), () => OpenForm<DbSettingsForm>());
            AddNavButton("🧩  الفواتير ", typeof(UnpaidInvoicesReportForm), () => OpenForm<UnpaidInvoicesReportForm>());
            AddNavButton("🧩  القرات  ", typeof(ReadingEntryForm), () => OpenForm<ReadingEntryForm>());
            AddNavButton("🧩  المزمنة  ", typeof(FrmMobileSyncDashboard), () => OpenForm<FrmMobileSyncDashboard>());

            sidebar.Controls.Add(new Panel { Height = 10, Width = 255, BackColor = Primary, Margin = new Padding(0, 4, 0, 4) });

            AddSideToolButton("✖ إغلاق التبويب الحالي", () => CloseCurrentTab());
            AddSideToolButton("🧹 إغلاق كل التبويبات", () => CloseAllTabs());
            AddSideToolButton("🚪 خروج", () => Close());

            sidebar.ResumeLayout();
        }

        private void Header_Paint(object sender, PaintEventArgs e)
        {
            var header = sender as Panel;
            if (header == null) return;

            using (var pen = new Pen(Color.FromArgb(40, 255, 255, 255)))
                e.Graphics.DrawRectangle(pen, 0, 0, header.Width - 1, header.Height - 1);
        }

        private void AddNavButton(string text, Type formType, Action action)
        {
            var btn = new Button
            {
                Width = 255,
                Height = 46,
                Text = "   " + text,
                TextAlign = ContentAlignment.MiddleLeft,
                FlatStyle = FlatStyle.Flat,
                BackColor = Primary,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 8),
                Tag = formType,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold)
            };

            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.FromArgb(30, 255, 255, 255);

            // Hover بدون lambda داخل Designer (هنا مسموح)
            btn.MouseEnter += NavBtn_MouseEnter;
            btn.MouseLeave += NavBtn_MouseLeave;
            btn.Click += NavBtn_Click;

            // نخزن الـ action داخل Tag2 بطريقة بسيطة:
            btn.Tag = new NavTag { FormType = formType, Action = action };

            sidebar.Controls.Add(btn);
        }

        private class NavTag
        {
            public Type FormType { get; set; }
            public Action Action { get; set; }
        }

        private void NavBtn_MouseEnter(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            if (_activeButton != btn) btn.BackColor = Hover;
        }

        private void NavBtn_MouseLeave(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;
            if (_activeButton != btn) btn.BackColor = Primary;
        }

        private void NavBtn_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            SetActiveButton(btn);

            var tag = btn.Tag as NavTag;
            tag?.Action?.Invoke();
        }

        private void AddSideToolButton(string text, Action action)
        {
            var btn = new Button
            {
                Width = 255,
                Height = 40,
                Text = "   " + text,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(14, 0, 12, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(12, 255, 255, 255),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 8)
            };
            btn.FlatAppearance.BorderSize = 0;

            btn.MouseEnter += ToolBtn_MouseEnter;
            btn.MouseLeave += ToolBtn_MouseLeave;
            btn.Click += (s, e) => action();

            sidebar.Controls.Add(btn);
        }

        private void ToolBtn_MouseEnter(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null) btn.BackColor = Color.FromArgb(25, 255, 255, 255);
        }

        private void ToolBtn_MouseLeave(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn != null) btn.BackColor = Color.FromArgb(12, 255, 255, 255);
        }

        // ================== OPEN / SHOW FORMS ==================
        //private void OpenForm<T>() where T : Form, new()
        //{
        //    var type = typeof(T);

        //    if (formsCache.ContainsKey(type) && (formsCache[type] == null || formsCache[type].IsDisposed))
        //        formsCache.Remove(type);

        //    if (formsCache.ContainsKey(type))
        //    {
        //        var existingForm = formsCache[type];
        //        var tab = mainTabControl.TabPages.Cast<TabPage>()
        //            .FirstOrDefault(p => ReferenceEquals(p.Tag, existingForm));

        //        if (tab != null)
        //        {
        //            mainTabControl.SelectedTab = tab;
        //            UpdateStatus();
        //            return;
        //        }

        //        formsCache.Remove(type);
        //    }

        //    var form = new T
        //    {
        //        TopLevel = false,
        //        FormBorderStyle = FormBorderStyle.None,
        //        Dock = DockStyle.Fill
        //    };

        //    var page = new TabPage(form.Text) { Tag = form };
        //    mainTabControl.TabPages.Add(page);
        //    mainTabControl.SelectedTab = page;

        //    formsCache[type] = form;

        //    ShowFormInContentHost(form);
        //    form.Show();

        //    UpdateStatus();
        //}
        private void OpenForm<T>() where T : Form, new()
        {
            var type = typeof(T);

            if (formsCache.ContainsKey(type) && (formsCache[type] == null || formsCache[type].IsDisposed))
                formsCache.Remove(type);

            if (formsCache.ContainsKey(type))
            {
                var existingForm = formsCache[type];
                var tab = mainTabControl.TabPages.Cast<TabPage>()
                    .FirstOrDefault(p => ReferenceEquals(p.Tag, existingForm));

                if (tab != null)
                {
                    mainTabControl.SelectedTab = tab;
                    ShowFormInContentHost(existingForm);
                    UpdateStatus();

                    if (existingForm is SubscriberForm existingSubscriber)
                        existingSubscriber.ApplyLayoutAfterSidebarToggle();

                    return;
                }

                formsCache.Remove(type);
            }

            var form = new T();
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            form.WindowState = FormWindowState.Normal;

            var page = new TabPage(form.Text) { Tag = form };
            mainTabControl.TabPages.Add(page);
            mainTabControl.SelectedTab = page;

            formsCache[type] = form;

            ShowFormInContentHost(form);
            form.Show();

            if (form is SubscriberForm subscriberForm)
                subscriberForm.ApplyLayoutAfterSidebarToggle();

            UpdateStatus();
        }
        private void ShowSelectedTabForm()
        {
            if (mainTabControl.SelectedTab == null) return;

            var form = mainTabControl.SelectedTab.Tag as Form;
            if (form == null || form.IsDisposed) return;

            ShowFormInContentHost(form);
            UpdateStatus();
        }
        private void ShowFormInContentHost(Form form)
        {
            if (form == null || form.IsDisposed) return;

            contentHost.SuspendLayout();
            try
            {
                foreach (Control c in contentHost.Controls.Cast<Control>().ToList())
                {
                    c.Visible = false;
                    if (!ReferenceEquals(c, form))
                        contentHost.Controls.Remove(c);
                }

                if (!contentHost.Controls.Contains(form))
                    contentHost.Controls.Add(form);

                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.Dock = DockStyle.Fill;
                form.WindowState = FormWindowState.Normal;
                form.Margin = new Padding(0);
                form.Padding = new Padding(0);

                form.Visible = true;
                form.BringToFront();
                form.PerformLayout();
                form.Invalidate(true);

                if (form is SubscriberForm subscriberForm)
                    subscriberForm.ApplyLayoutAfterSidebarToggle();
            }
            finally
            {
                contentHost.ResumeLayout(true);
            }
        }
        //private void ShowFormInContentHost(Form form)
        //{
        //    contentHost.SuspendLayout();

        //    foreach (Control c in contentHost.Controls.Cast<Control>().ToList())
        //    {
        //        c.Visible = false;
        //        contentHost.Controls.Remove(c);
        //    }

        //    if (!contentHost.Controls.Contains(form))
        //        contentHost.Controls.Add(form);

        //    form.Visible = true;
        //    form.BringToFront();

        //    contentHost.ResumeLayout();
        //}

        // ================== TABS UI EVENTS ==================
        private void MainTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tab = mainTabControl.TabPages[e.Index];
            var rect = e.Bounds;

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            using (var b = new SolidBrush(selected ? Color.White : SystemColors.Control))
                e.Graphics.FillRectangle(b, rect);

            if (selected)
            {
                using (var pen = new Pen(Primary, 3))
                    e.Graphics.DrawLine(pen, rect.X + 8, rect.Bottom - 2, rect.Right - 8, rect.Bottom - 2);
            }

            TextRenderer.DrawText(
                e.Graphics,
                tab.Text,
                new Font("Segoe UI", 9.5f, selected ? FontStyle.Bold : FontStyle.Regular),
                new Rectangle(rect.X + 10, rect.Y + 7, rect.Width - 34, rect.Height),
                Color.Black,
                TextFormatFlags.RightToLeft | TextFormatFlags.VerticalCenter);

            var xRect = new Rectangle(rect.Right - 22, rect.Y + 8, 14, 14);
            TextRenderer.DrawText(e.Graphics, "✕", new Font("Segoe UI", 9f, FontStyle.Bold), xRect, Color.DimGray);
        }

        private void MainTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < mainTabControl.TabPages.Count; i++)
            {
                var rect = mainTabControl.GetTabRect(i);
                var xRect = new Rectangle(rect.Right - 22, rect.Y + 8, 14, 14);

                if (xRect.Contains(e.Location))
                {
                    CloseTab(i);
                    return;
                }

                if (e.Button == MouseButtons.Middle && rect.Contains(e.Location))
                {
                    CloseTab(i);
                    return;
                }
            }
        }

        private void CloseCurrentTab()
        {
            if (mainTabControl.TabPages.Count == 0) return;
            CloseTab(mainTabControl.SelectedIndex);
        }

        private void CloseAllTabs()
        {
            while (mainTabControl.TabPages.Count > 0)
                CloseTab(0);
        }

        private void CloseTab(int index)
        {
            if (index < 0 || index >= mainTabControl.TabPages.Count) return;

            var page = mainTabControl.TabPages[index];
            var form = page.Tag as Form;

            if (form != null)
            {
                var type = form.GetType();
                if (formsCache.ContainsKey(type)) formsCache.Remove(type);

                try { if (contentHost.Controls.Contains(form)) contentHost.Controls.Remove(form); } catch { }
                try { form.Close(); } catch { }
                try { form.Dispose(); } catch { }
            }

            mainTabControl.TabPages.RemoveAt(index);

            if (mainTabControl.TabPages.Count > 0)
                ShowSelectedTabForm();

            UpdateStatus();
        }

        // ================== STATE ==================
        private void SetActiveButton(Button btn)
        {
            if (_activeButton != null)
            {
                _activeButton.BackColor = Primary;
                _activeButton.ForeColor = Color.White;
            }

            _activeButton = btn;
            _activeButton.BackColor = Selected;
            _activeButton.ForeColor = SelectedText;
        }

        private void UpdateStatus()
        {
            if (statusBar != null && statusBar.Items.Count > 0)
                stReady.Text = "📑 التبويبات: " + mainTabControl.TabCount;
        }

        private void LoadDashboard()
        {
            // أول زر بعد الهيدر غالباً لوحة التحكم
            var dashBtn = sidebar.Controls.OfType<Button>()
                .FirstOrDefault(b => (b.Text ?? "").Contains("لوحة التحكم"));

            if (dashBtn != null) SetActiveButton(dashBtn);
            OpenForm<DashboardForm>();
        }

        // ================== EVENTS ==================
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.W)
            {
                CloseCurrentTab();
                e.Handled = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (updateTimer != null) { updateTimer.Stop(); updateTimer.Dispose(); }
            try { CloseAllTabs(); } catch { }
        }
    }
}



/*
using System.Drawing;
using System.Windows.Forms;

namespace water3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitializeSidebar();
            this.RightToLeftLayout = true; // لدعم تعريب الفورم بالكامل
        }

        private void InitializeSidebar()
        {
            Panel sidebar = new Panel
            {
                Dock = DockStyle.Right, // اجعل الشريط الجانبي يمين الشاشة
                Width = 220,
                BackColor = Color.FromArgb(0, 87, 183)
            };
            this.Controls.Add(sidebar);

            // عنوان البرنامج
            Label lblTitle = new Label
            {
                Text = "🦅 نظام إدارة المياه",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter
            };
            sidebar.Controls.Add(lblTitle);

            // قائمة الأزرار الرئيسية بالعربي والرموز المناسبة
            string[,] items =
            {
                {"🔍 قراءة العدادات", "قراءة العدادات"},
                {"💼 تقرير التحصيل", "تقرير تحصيلات المحصلين"},
                {"📋 كشف حساب مشترك", "كشف حساب مشترك"},
                {"👤 المشتركين", "المشتركين"},
                {"🧾 الفواتير", "الفواتير"},
                {"💵 المدفوعات", "المدفوعات"},
                {"📄 الفواتير غير المدفوعة", "الفواتير غير المدفوعة"},
                {"🖨️ طباعة", "طباعة"}
            };

            // إضافة الأزرار للقائمة
            for (int i = 0; i < items.GetLength(0); i++)
            {
                Button btn = new Button
                {
                    Text = $"   {items[i, 0]}",
                    Tag = items[i, 1],
                    TextAlign = ContentAlignment.MiddleRight, // لمحاذاة النص لليمين
                    RightToLeft = RightToLeft.Yes,
                    Dock = DockStyle.Top,
                    Height = 44,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 },
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.FromArgb(0, 87, 183),
                    Cursor = Cursors.Hand,
                    Padding = new Padding(0, 0, 12, 0),
                    Margin = new Padding(0, 0, 0, 6)
                };
                btn.MouseEnter += (s, e) => btn.BackColor = Color.FromArgb(0, 106, 204);
                btn.MouseLeave += (s, e) => btn.BackColor = Color.FromArgb(0, 87, 183);

                // مثال: عند الضغط على زر معين افتح نافذة جديدة
                btn.Click += SidebarButton_Click;
                sidebar.Controls.Add(btn);
            }

            // شعار أو تذييل
            Label lblFooter = new Label
            {
                Text = "جميع الحقوق محفوظة © 2025",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.White,
                Dock = DockStyle.Bottom,
                Height = 38,
                TextAlign = ContentAlignment.MiddleCenter
            };
            sidebar.Controls.Add(lblFooter);

            // ترتيب العناصر: اجعل العنوان دومًا أعلى القائمة
            sidebar.Controls.SetChildIndex(lblTitle, 0);
        }

        // معالج ضغطة الأزرار (يمكنك تخصيصه حسب كل زر)
        private void SidebarButton_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            string tag = btn.Tag as string;

            // يمكنك هنا فتح كل نافذة حسب اسم الزر
            switch (tag)
            {
                case "المشتركين":
                    new SubscriberForm().ShowDialog();
                    break;
                case "قراءة العدادات":
                    new ReadingForm().ShowDialog();
                    break;
                case "تقرير تحصيلات المحصلين":
                    new CollectorsReportForm().ShowDialog();
                    break;
                case "كشف حساب مشترك":
                    new AccountStatementForm().ShowDialog();
                    break;
                case "الفواتير":
                    new InvoiceForm().ShowDialog();
                    break;
                case "المدفوعات":
                    new PaymentsForm().ShowDialog();
                    break;
                case "الفواتير غير المدفوعة":
                    new UnpaidInvoicesReportForm().ShowDialog();
                    break;
                case "طباعة":
                    new InvoicePrintPreviewForm().ShowDialog();
                    break;
                default:
                    MessageBox.Show("زر غير معرف بعد!");
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // خصص ملاءمة الشاشة هنا إن احتجت
            this.WindowState = FormWindowState.Maximized; // يبدأ البرنامج بكامل الشاشة
        }
    }
}


*/
