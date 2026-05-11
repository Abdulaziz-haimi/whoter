using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using water3.DB;
using water3.Forms;
using water3.Reports;
using water3.Reports.Dynamic;
using water3.Security;

namespace water3
{
    public partial class Form1 : Form
    {
        private const int SIDEBAR_WIDTH = 318;
        private const int SIDEBAR_MARGIN = 12;
        private const int SIDEBAR_TOP = 14;
        private const int ITEM_GAP = 0;

        private readonly Dictionary<Type, Form> formsCache = new Dictionary<Type, Form>();
        private readonly Dictionary<Type, Button> _navButtons = new Dictionary<Type, Button>();

        private Button _activeButton;
        private bool _sidebarCollapsed = false;

        private AppThemePalette P
        {
            get { return AppThemeManager.Palette; }
        }

        public Form1()
        {
            AppThemeManager.LoadTheme();

            InitializeComponent();
            FixWinFormsRtlLayout();

            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = false; // مهم: نوقف Mirror الكامل ونرتب الواجهة يدوياً
            WindowState = FormWindowState.Maximized;
            Font = new Font("Tahoma", 10f);
            KeyPreview = true;

            ApplyThemeAndBaseStyles();
            BuildSidebarContent();
            RestoreSidebarLayout();
            //foreach (Form f in formsCache.Values.ToList())
            //{
            //    if (f is InvoiceForm smart && !smart.IsDisposed)
            //        smart.RefreshTheme();
            //}
            sidebarViewport.Resize += (s, e) => ReflowSidebar();
            bodyPanel.Resize += (s, e) => RestoreSidebarLayout();

            stClock.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            stVersion.Text = "الإصدار v" + Application.ProductVersion;
           

            updateTimer.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDashboard();
        }


        private void FixWinFormsRtlLayout()
        {
            // تثبيت اتجاه الحاويات حتى لا تنقلب الأعمدة مع RTL
            topBar.RightToLeft = RightToLeft.No;
            topLayout.RightToLeft = RightToLeft.No;

            btnTheme.RightToLeft = RightToLeft.No;
            btnToggleSidebar.RightToLeft = RightToLeft.No;

            mainTabControl.RightToLeft = RightToLeft.Yes;
            mainTabControl.RightToLeftLayout = false;

            bodyPanel.RightToLeft = RightToLeft.No;
            contentCard.RightToLeft = RightToLeft.Yes;
            contentHost.RightToLeft = RightToLeft.Yes;

            sidebarHost.Dock = DockStyle.Right;
            sidebarHost.Width = SIDEBAR_WIDTH;
            sidebarHost.RightToLeft = RightToLeft.No;
            sidebarHost.Padding = Padding.Empty;

            sidebarViewport.RightToLeft = RightToLeft.No;
            sidebarViewport.Padding = Padding.Empty;
            sidebarViewport.AutoScroll = true;
            sidebarViewport.AutoScrollMargin = Size.Empty;
            sidebarViewport.HorizontalScroll.Enabled = false;
            sidebarViewport.HorizontalScroll.Visible = false;

            sidebarContent.RightToLeft = RightToLeft.Yes;

            sidebarHost.BringToFront();
            contentCard.SendToBack();
        }

        private string GetTopUserText()
        {
            string name = string.IsNullOrWhiteSpace(CurrentUser.FullName)
                ? CurrentUser.UserName
                : CurrentUser.FullName;

            string role = string.IsNullOrWhiteSpace(CurrentUser.RoleName)
                ? "مستخدم"
                : CurrentUser.RoleName;

            return name + " - " + role;
        }

        private void ApplyThemeAndBaseStyles()
        {
            BackColor = P.Bg;
            rootPanel.BackColor = P.Bg;

            topBar.BackColor = P.Card;
            topLayout.BackColor = P.Card;

            bodyPanel.BackColor = P.Bg;
            contentCard.BackColor = P.Card;
            contentHost.BackColor = P.Bg;

            sidebarHost.BackColor = P.Primary;
            sidebarViewport.BackColor = P.Primary;
            sidebarContent.BackColor = P.Primary;

            statusBar.BackColor = P.Card;
            statusBar.ForeColor = P.Text;

            stReady.ForeColor = P.Success;
            stClock.ForeColor = P.Muted;
            stVersion.ForeColor = P.Muted;

            StyleTopButton(btnToggleSidebar, "☰");
            StyleTopButton(btnTheme, "◐");

            mainTabControl.BackColor = P.Card;

            UpdateThemeButtonText();
        }

        private void StyleTopButton(Button btn, string text)
        {
            btn.Text = text;
            btn.Width = 42;
            btn.Height = 34;
            btn.Margin = new Padding(4, 4, 4, 4);
            btn.FlatStyle = FlatStyle.Flat;
            btn.BackColor = P.Soft;
            btn.ForeColor = P.Text;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            btn.TextAlign = ContentAlignment.MiddleCenter;

            btn.FlatAppearance.BorderColor = P.Border;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseOverBackColor = P.Bg;
            btn.FlatAppearance.MouseDownBackColor = P.Border;
        }

        private void UpdateThemeButtonText()
        {
            if (AppThemeManager.CurrentTheme == AppThemeName.Dark)
                btnTheme.Text = "☾";
            else if (AppThemeManager.CurrentTheme == AppThemeName.Light)
                btnTheme.Text = "☼";
            else
                btnTheme.Text = "◐";
        }

        private void btnTheme_Click(object sender, EventArgs e)
        {
            Type selectedType = null;

            if (mainTabControl.SelectedTab != null && mainTabControl.SelectedTab.Tag is Form selectedForm)
                selectedType = selectedForm.GetType();

            AppThemeManager.SaveTheme(AppThemeManager.NextTheme());

            ApplyThemeAndBaseStyles();
            BuildSidebarContent();
            RestoreSidebarLayout();
            foreach (Form f in formsCache.Values.ToList())
            {
                if (f is DashboardForm dashboard && !dashboard.IsDisposed)
                    dashboard.RefreshTheme();
            }
            if (selectedType != null && _navButtons.ContainsKey(selectedType))
                SetActiveButton(_navButtons[selectedType]);

            RecreateModernDashboardIfOpen();
            contentCard.Invalidate();
            topBar.Invalidate();
            mainTabControl.Invalidate();
        }

        private void RecreateModernDashboardIfOpen()
        {
            Form dashboard = formsCache.Values.FirstOrDefault(f => f is ModernDashboardForm);

            if (dashboard == null || dashboard.IsDisposed)
                return;

            CloseTabByForm(dashboard);
            OpenForm<ModernDashboardForm>();
        }

        private void topBar_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen pen = new Pen(P.Border))
                e.Graphics.DrawRectangle(pen, 0, 0, topBar.Width - 1, topBar.Height - 1);
        }

        private void contentCard_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen pen = new Pen(P.Border))
                e.Graphics.DrawRectangle(pen, 0, 0, contentCard.Width - 1, contentCard.Height - 1);
        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            stClock.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        /*
            حل نهائي لمشكلة القائمة في RTL:
            لا نغير أعمدة ولا نترك AutoScroll يحسب عرضاً أفقياً.
            القائمة Panel ثابت Dock=Right، وعند الإخفاء نغير Visible فقط ثم نعيد ضبط العرض والترتيب.
        */
        private void btnToggleSidebar_Click(object sender, EventArgs e)
        {
            _sidebarCollapsed = !_sidebarCollapsed;
            RestoreSidebarLayout();
            RefreshCurrentFormLayout();
        }

        private void RestoreSidebarLayout()
        {
            if (bodyPanel == null || sidebarHost == null || sidebarViewport == null || sidebarContent == null)
                return;

            rootPanel.SuspendLayout();
            bodyPanel.SuspendLayout();
            sidebarHost.SuspendLayout();
            sidebarViewport.SuspendLayout();
            contentCard.SuspendLayout();

            try
            {
                bodyPanel.RightToLeft = RightToLeft.No;

                contentCard.Dock = DockStyle.Fill;
                contentCard.Margin = Padding.Empty;
                contentCard.Padding = new Padding(8);

                sidebarHost.Dock = DockStyle.Right;
                sidebarHost.Width = _sidebarCollapsed ? 0 : SIDEBAR_WIDTH;
                sidebarHost.Visible = !_sidebarCollapsed;
                sidebarHost.Padding = Padding.Empty;
                sidebarHost.Margin = Padding.Empty;
                sidebarHost.RightToLeft = RightToLeft.No;

                sidebarViewport.Dock = DockStyle.Fill;
                sidebarViewport.Padding = Padding.Empty;
                sidebarViewport.RightToLeft = RightToLeft.No;
                sidebarViewport.AutoScroll = true;
                sidebarViewport.AutoScrollMargin = Size.Empty;
                sidebarViewport.HorizontalScroll.Enabled = false;
                sidebarViewport.HorizontalScroll.Visible = false;
                sidebarViewport.HorizontalScroll.Maximum = 0;

                sidebarContent.RightToLeft = RightToLeft.Yes;

                btnToggleSidebar.Text = _sidebarCollapsed ? "☰" : "☰";

                sidebarHost.BringToFront();
                contentCard.SendToBack();

                if (!_sidebarCollapsed)
                    ReflowSidebar();

                bodyPanel.PerformLayout();
                contentCard.PerformLayout();
                contentHost.PerformLayout();
                contentHost.Invalidate(true);
            }
            finally
            {
                contentCard.ResumeLayout(true);
                sidebarViewport.ResumeLayout(true);
                sidebarHost.ResumeLayout(true);
                bodyPanel.ResumeLayout(true);
                rootPanel.ResumeLayout(true);
            }
        }

        private void RefreshCurrentFormLayout()
        {
            if (mainTabControl.SelectedTab?.Tag is Form currentForm && !currentForm.IsDisposed)
            {
                currentForm.SuspendLayout();

                try
                {
                    currentForm.Dock = DockStyle.Fill;
                    currentForm.WindowState = FormWindowState.Normal;
                    currentForm.PerformLayout();
                    currentForm.Invalidate(true);

                    if (currentForm is SubscriberForm subscriberForm)
                        subscriberForm.ApplyLayoutAfterSidebarToggle();
                }
                finally
                {
                    currentForm.ResumeLayout(true);
                }
            }
        }

        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectedTabForm();

            if (mainTabControl.SelectedTab != null && mainTabControl.SelectedTab.Tag is Form form)
            {
                Type t = form.GetType();

                if (_navButtons.ContainsKey(t))
                    SetActiveButton(_navButtons[t]);
            }
        }

        private void BuildSidebarContent()
        {
            sidebarViewport.SuspendLayout();
            sidebarContent.SuspendLayout();

            try
            {
                sidebarContent.Controls.Clear();
                _navButtons.Clear();

                sidebarContent.BackColor = P.Primary;
                sidebarHost.BackColor = P.Primary;
                sidebarViewport.BackColor = P.Primary;

                AddSidebarControl(CreateSidebarHeader());

                AddAccordionSection("الرئيسية", "🏠", true, body =>
                {
                    AddNavButton(body, "لوحة التحكم", "📊", typeof(DashboardForm), PermissionKeys.DashboardView, () => OpenForm<DashboardForm>());
                });

                AddAccordionSection("التشغيل", "⚙️", true, body =>
                {
                    //AddNavButton(body, "الفواتير", "📄", typeof(InvoiceForm), PermissionKeys.InvoicesView, () => OpenForm<InvoiceForm>());
                    AddNavButton(body, "المشتركين", "👥", typeof(SubscriberForm), PermissionKeys.SubscribersView, () => OpenForm<SubscriberForm>());
                    AddNavButton(body, "المدفوعات", "🧾", typeof(PaymentsForm), PermissionKeys.PaymentsView, () => OpenForm<PaymentsForm>());
                    AddNavButton(body, "قراءة العدادات", "🧮", typeof(ReadingF), PermissionKeys.ReadingsView, () => OpenForm<ReadingF>());
                    AddNavButton(body, "إدخال القراءات", "🧩", typeof(ReadingEntryForm), PermissionKeys.ReadingEntryView, () => OpenForm<ReadingEntryForm>());
                });

                AddAccordionSection("التقارير", "📊", false, body =>
                {
                    AddNavButton(body, "تقرير المشتركين", "📌", typeof(SubscribersBillingReportForm), PermissionKeys.SubscribersReportView, () => OpenForm<SubscribersBillingReportForm>());
                    AddNavButton(body, "تحصيلات المحصلين", "📈", typeof(CollectorsReportForm), PermissionKeys.CollectorsReportView, () => OpenForm<CollectorsReportForm>());
                    AddNavButton(body, "كشف حساب مشترك", "📑", typeof(AccountStatementForm), PermissionKeys.AccountStatementView, () => OpenForm<AccountStatementForm>());
                    AddNavButton(body, "الفواتير غير المسددة", "🧩", typeof(UnpaidInvoicesReportForm), PermissionKeys.UnpaidInvoicesView, () => OpenForm<UnpaidInvoicesReportForm>());
                    AddNavButton(body, "طباعة الفواتير", "🖨️", typeof(InvoicePrintPreviewForm), PermissionKeys.InvoicePrintView, () => OpenForm<InvoicePrintPreviewForm>());
                    AddNavButton(body, "سجل العمليات", "🕘", typeof(AuditLogForm), PermissionKeys.AuditLogView, () => OpenForm<AuditLogForm>());
                    AddNavButton(body, "تقرير العداد والفاقد", "🚰", typeof(MainMeterReportForm), PermissionKeys.MainMeterReportView, () => OpenForm<MainMeterReportForm>());
                    AddNavButton(body, "مصمم التقارير", "📚", typeof(DynamicReportDesignerForm), null, () => OpenForm<DynamicReportDesignerForm>());
                });

                AddAccordionSection("المستخدمون والصلاحيات", "🛡️", false, body =>
                {
                    AddNavButton(body, "إدارة المستخدمين", "👤", typeof(UsersManagementForm), PermissionKeys.UsersView, () => OpenForm<UsersManagementForm>());
                    AddNavButton(body, "إنشاء مستخدم", "➕", typeof(CreateUserForm), PermissionKeys.UsersCreate, () => OpenForm<CreateUserForm>());
                    AddNavButton(body, "الأدوار والصلاحيات", "🛡️", typeof(RolesManagementForm), PermissionKeys.RolesManage, () => OpenForm<RolesManagementForm>());
                });

                AddAccordionSection("المحصلون والمزامنة", "📱", false, body =>
                {
                    AddNavButton(body, "إدارة المحصلين", "👤", typeof(CollectorsManagementForm), PermissionKeys.CollectorsView, () => OpenForm<CollectorsManagementForm>());
                    AddNavButton(body, "ربط المحصل بالمستخدم", "🔗", typeof(CollectorUserLinkForm), PermissionKeys.CollectorsLinkUser, () => OpenForm<CollectorUserLinkForm>());
                    AddNavButton(body, "أجهزة المحصلين", "📱", typeof(CollectorDevicesForm), PermissionKeys.CollectorDevicesView, () => OpenForm<CollectorDevicesForm>());
                    AddNavButton(body, "لوحة المزامنة الجوالة", "🔄", typeof(FrmMobileSyncDashboard), PermissionKeys.MobileSyncView, () => OpenForm<FrmMobileSyncDashboard>());
                    AddNavButton(body, "المزامنة إلى الهاتف", "📤", typeof(MobileSyncToPhoneForm), PermissionKeys.MobileSyncToPhoneView, () => OpenForm<MobileSyncToPhoneForm>());
                });

                AddAccordionSection("الإعدادات والرسائل", "🧩", false, body =>
                {
                    AddNavButton(body, "إدارة الثوابت", "⚙️", typeof(BillingConstantsForm), PermissionKeys.BillingConstantsView, () => OpenForm<BillingConstantsForm>());
                    AddNavButton(body, "سجل الرسائل", "✉️", typeof(SmsLogsForm), PermissionKeys.SmsLogsView, () => OpenForm<SmsLogsForm>());
                    AddNavButton(body, "تقرير الرسائل", "📨", typeof(SmsReportForm), PermissionKeys.SmsReportView, () => OpenForm<SmsReportForm>());
                    AddNavButton(body, "إدارة الرسائل", "🧩", typeof(MessagesManagementForm), PermissionKeys.MessagesManage, () => OpenForm<MessagesManagementForm>());
                    AddNavButton(body, "إدارة الاتصال", "🔌", typeof(DbSettingsForm), PermissionKeys.DbSettingsView, () => OpenForm<DbSettingsForm>());
                    AddNavButton(body, "المصروفات والمشتريات", "💸", typeof(ExpensesManagementForm), PermissionKeys.ExpensesView, () => OpenForm<ExpensesManagementForm>());
                    AddNavButton(body, "تصنيفات المصروفات", "🏷️", typeof(ExpenseCategoriesForm), PermissionKeys.ExpenseCategoriesManage, () => OpenForm<ExpenseCategoriesForm>());
                    AddNavButton(body, "النسخ الاحتياطي والاستعادة", "💾", typeof(BackupRestoreForm), PermissionKeys.ExpenseCategoriesManage, () => OpenForm<BackupRestoreForm>());
                    AddNavButton(body, "بيانات الشركة", "🏢", typeof(CompanySettingsForm), null, () => OpenForm<CompanySettingsForm>());
                    AddNavButton(body, "إعداد الطابعة والفواتير", "🖨", typeof(PrinterInvoiceSettingsForm), null, () => OpenForm<PrinterInvoiceSettingsForm>());
                    AddNavButton(body, "فحص النظام", "🩺", typeof(SystemHealthForm), null, () => OpenForm<SystemHealthForm>());
                    AddNavButton(body, "سجل الأخطاء", "⚠", typeof(ErrorLogsForm), null, () => OpenForm<ErrorLogsForm>());
                    AddNavButton(body, "ترقية قاعدة البيانات", "⬆", typeof(DatabaseMigrationForm), null, () => OpenForm<DatabaseMigrationForm>());
                    AddNavButton(body, "مركز التصدير", "📤", typeof(ExportCenterForm), null, () => OpenForm<ExportCenterForm>());
                    AddNavButton(body, "إدارة النسخة والتحديثات", "🔄", typeof(AppVersionUpdateForm), null, () => OpenForm<AppVersionUpdateForm>());

                });

                AddSidebarControl(CreateSidebarSeparator());

                AddSideToolButton("إغلاق التبويب الحالي", "✖", CloseCurrentTab);
                AddSideToolButton("إغلاق كل التبويبات", "🧹", CloseAllTabs);
                AddSideToolButton("تسجيل الخروج", "🔓", Logout);
                AddSideToolButton("خروج", "🚪", () => Close());

                ReflowSidebar();
            }
            finally
            {
                sidebarContent.ResumeLayout(false);
                sidebarViewport.ResumeLayout(true);
            }
        }

        private int GetSidebarItemWidth()
        {
            int clientWidth = sidebarViewport != null && sidebarViewport.ClientSize.Width > 0
                ? sidebarViewport.ClientSize.Width
                : SIDEBAR_WIDTH;

            // نطرح عرض شريط التمرير دائماً حتى لا يظهر شريط تمرير أفقي عند ظهور العمودي.
            int width = clientWidth - (SIDEBAR_MARGIN * 2) - SystemInformation.VerticalScrollBarWidth - 4;
            return Math.Max(230, width);
        }

        private void AddSidebarControl(Control control)
        {
            control.Left = 0;
            control.Width = GetSidebarItemWidth();
            control.Margin = Padding.Empty;
            control.RightToLeft = RightToLeft.Yes;
            sidebarContent.Controls.Add(control);
        }

        private void ReflowSidebar()
        {
            if (_sidebarCollapsed || sidebarViewport == null || sidebarContent == null)
                return;

            sidebarViewport.SuspendLayout();
            sidebarContent.SuspendLayout();

            try
            {
                int itemWidth = GetSidebarItemWidth();
                int y = 0;

                sidebarContent.Location = new Point(SIDEBAR_MARGIN, SIDEBAR_TOP);
                sidebarContent.Width = itemWidth;

                foreach (Control control in sidebarContent.Controls)
                {
                    control.Left = 0;
                    control.Width = itemWidth;
                    control.Margin = Padding.Empty;
                    control.RightToLeft = RightToLeft.Yes;

                    if (control is Panel panel)
                    {
                        panel.Width = itemWidth;

                        if (panel.Tag is AccordionBodyTag)
                            ReflowAccordionBody(panel);
                    }

                    if (!control.Visible)
                        continue;

                    control.Top = y;
                    y += control.Height + ITEM_GAP;
                }

                sidebarContent.Height = Math.Max(y + SIDEBAR_TOP, sidebarViewport.ClientSize.Height - SIDEBAR_TOP);

                // عرض AutoScrollMinSize = 0 يمنع ظهور شريط التمرير الأفقي.
                sidebarViewport.AutoScrollMinSize = new Size(0, sidebarContent.Bottom + SIDEBAR_TOP);
                sidebarViewport.HorizontalScroll.Enabled = false;
                sidebarViewport.HorizontalScroll.Visible = false;
                sidebarViewport.HorizontalScroll.Maximum = 0;

                sidebarContent.Invalidate(true);
                sidebarViewport.Invalidate(true);
            }
            finally
            {
                sidebarContent.ResumeLayout(false);
                sidebarViewport.ResumeLayout(true);
            }
        }

        private void ReflowAccordionBody(Panel body)
        {
            int y = 0;
            int itemWidth = body.Width > 0 ? body.Width : GetSidebarItemWidth();

            foreach (Control child in body.Controls)
            {
                child.Left = 0;
                child.Top = y;
                child.Width = itemWidth;
                child.Margin = Padding.Empty;
                child.RightToLeft = RightToLeft.Yes;
                y += child.Height + ITEM_GAP;
            }

            body.Height = y;
        }

        private Panel CreateSidebarHeader()
        {
            Panel header = new Panel
            {
                Height = 112,
                BackColor = P.PrimaryDark,
                Padding = new Padding(14)
            };

            header.Paint += Header_Paint;

            Label icon = new Label
            {
                Dock = DockStyle.Right,
                Width = 50,
                Text = "💧",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Emoji", 25f),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label headerTitle = new Label
            {
                Dock = DockStyle.Top,
                Height = 29,
                Text = "القائمة الرئيسية",
                ForeColor = Color.White,
                Font = new Font("Tahoma", 12f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };

            Label headerSub = new Label
            {
                Dock = DockStyle.Fill,
                Text = GetTopUserText(),
                ForeColor = Color.FromArgb(226, 232, 240),
                Font = new Font("Tahoma", 9f),
                TextAlign = ContentAlignment.TopRight,
                Padding = new Padding(0, 6, 0, 0)
            };

            header.Controls.Add(headerSub);
            header.Controls.Add(headerTitle);
            header.Controls.Add(icon);

            return header;
        }

        private void AddAccordionSection(string title, string icon, bool expanded, Action<Panel> fillBody)
        {
            Button header = new Button
            {
                Height = 38,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(12, 0, 12, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = P.PrimaryDark,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Tahoma", 9.5f, FontStyle.Bold)
            };

            header.FlatAppearance.BorderSize = 0;
            header.FlatAppearance.MouseOverBackColor = P.Hover;

            Panel body = new Panel
            {
                Width = GetSidebarItemWidth(),
                BackColor = P.Primary,
                Visible = expanded,
                Tag = new AccordionBodyTag()
            };

            header.Tag = new AccordionTag
            {
                Body = body,
                Icon = icon,
                Title = title
            };

            SetAccordionHeaderText(header, expanded);
            header.Click += AccordionHeader_Click;

            AddSidebarControl(header);

            fillBody(body);
            ReflowAccordionBody(body);

            AddSidebarControl(body);
        }

        private class AccordionBodyTag { }

        private class AccordionTag
        {
            public Panel Body { get; set; }
            public string Icon { get; set; }
            public string Title { get; set; }
        }

        private void SetAccordionHeaderText(Button header, bool expanded)
        {
            AccordionTag tag = header.Tag as AccordionTag;

            if (tag == null)
                return;

            header.Text = (expanded ? "▾  " : "▸  ") + tag.Icon + "  " + tag.Title;
        }

        private void AccordionHeader_Click(object sender, EventArgs e)
        {
            Button header = sender as Button;

            if (header == null)
                return;

            AccordionTag tag = header.Tag as AccordionTag;

            if (tag == null || tag.Body == null)
                return;

            tag.Body.Visible = !tag.Body.Visible;
            SetAccordionHeaderText(header, tag.Body.Visible);
            ReflowSidebar();
        }

        private Panel CreateSidebarSeparator()
        {
            return new Panel
            {
                Height = 12,
                BackColor = P.Primary
            };
        }

        private void Header_Paint(object sender, PaintEventArgs e)
        {
            Panel header = sender as Panel;

            if (header == null)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (Pen pen = new Pen(Color.FromArgb(90, 255, 255, 255)))
                e.Graphics.DrawRectangle(pen, 0, 0, header.Width - 1, header.Height - 1);
        }

        private void AddNavButton(Panel parent, string title, string icon, Type formType, string permissionKey, Action action)
        {
            if (!HasPermission(permissionKey))
                return;

            Button btn = new Button
            {
                Height = 41,
                Text = icon + "  " + title,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(12, 0, 12, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = P.Primary,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Tahoma", 9.3f, FontStyle.Bold),
                Tag = new NavTag { FormType = formType, Action = action, PermissionKey = permissionKey }
            };

            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = P.Hover;
            btn.FlatAppearance.MouseOverBackColor = P.Hover;
            btn.FlatAppearance.MouseDownBackColor = P.PrimaryDark;
            btn.MouseEnter += NavBtn_MouseEnter;
            btn.MouseLeave += NavBtn_MouseLeave;
            btn.Click += NavBtn_Click;

            parent.Controls.Add(btn);

            if (!_navButtons.ContainsKey(formType))
                _navButtons.Add(formType, btn);
        }

        private bool HasPermission(string permissionKey)
        {
            if (string.IsNullOrWhiteSpace(permissionKey))
                return true;

            if (CurrentUser.RoleID == 1)
                return true;

            return CurrentUser.HasPermission(permissionKey);
        }

        private class NavTag
        {
            public Type FormType { get; set; }
            public Action Action { get; set; }
            public string PermissionKey { get; set; }
        }

        private void NavBtn_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null && _activeButton != btn)
                btn.BackColor = P.Hover;
        }

        private void NavBtn_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null && _activeButton != btn)
                btn.BackColor = P.Primary;
        }

        private void NavBtn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn == null)
                return;

            SetActiveButton(btn);

            NavTag tag = btn.Tag as NavTag;

            if (tag != null && tag.Action != null)
                tag.Action.Invoke();
        }

        private void AddSideToolButton(string title, string icon, Action action)
        {
            Button btn = new Button
            {
                Height = 39,
                Text = icon + "  " + title,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(12, 0, 12, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = P.Hover,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Tahoma", 9.3f)
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = P.PrimaryDark;
            btn.FlatAppearance.MouseDownBackColor = P.Accent;
            btn.MouseEnter += ToolBtn_MouseEnter;
            btn.MouseLeave += ToolBtn_MouseLeave;
            btn.Click += (s, e) => action();

            AddSidebarControl(btn);
        }

        private void ToolBtn_MouseEnter(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null)
                btn.BackColor = P.PrimaryDark;
        }

        private void ToolBtn_MouseLeave(object sender, EventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null)
                btn.BackColor = P.Hover;
        }

        private void OpenForm<T>() where T : Form, new()
        {
            Type type = typeof(T);

            if (formsCache.ContainsKey(type) && (formsCache[type] == null || formsCache[type].IsDisposed))
                formsCache.Remove(type);

            if (formsCache.ContainsKey(type))
            {
                Form existingForm = formsCache[type];

                TabPage tab = mainTabControl.TabPages.Cast<TabPage>()
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

            Form form = new T
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill,
                WindowState = FormWindowState.Normal
            };

            TabPage page = new TabPage(form.Text) { Tag = form };
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
            if (mainTabControl.SelectedTab == null)
                return;

            Form form = mainTabControl.SelectedTab.Tag as Form;

            if (form == null || form.IsDisposed)
                return;

            ShowFormInContentHost(form);
            UpdateStatus();
        }

        private void ShowFormInContentHost(Form form)
        {
            if (form == null || form.IsDisposed)
                return;

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

        private void MainTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            TabPage tab = mainTabControl.TabPages[e.Index];
            Rectangle rect = e.Bounds;
            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Color back = selected ? P.Card : P.Soft;
            Color fore = selected ? P.Accent : P.Text;

            using (SolidBrush b = new SolidBrush(back))
                e.Graphics.FillRectangle(b, rect);

            using (Pen borderPen = new Pen(P.Border))
                e.Graphics.DrawRectangle(borderPen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

            if (selected)
            {
                using (Pen pen = new Pen(P.Accent, 3))
                    e.Graphics.DrawLine(pen, rect.X + 10, rect.Bottom - 2, rect.Right - 10, rect.Bottom - 2);
            }

            TextRenderer.DrawText(
                e.Graphics,
                tab.Text,
                new Font("Tahoma", 9.3f, selected ? FontStyle.Bold : FontStyle.Regular),
                new Rectangle(rect.X + 28, rect.Y + 6, rect.Width - 58, rect.Height),
                fore,
                TextFormatFlags.RightToLeft | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);

            Rectangle xRect = new Rectangle(rect.Left + 10, rect.Y + 9, 15, 15);

            TextRenderer.DrawText(
                e.Graphics,
                "×",
                new Font("Segoe UI", 10f, FontStyle.Bold),
                xRect,
                selected ? P.Danger : P.Muted,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private void MainTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < mainTabControl.TabPages.Count; i++)
            {
                Rectangle rect = mainTabControl.GetTabRect(i);
                Rectangle xRect = new Rectangle(rect.Left + 10, rect.Y + 9, 15, 15);

                if (xRect.Contains(e.Location) || (e.Button == MouseButtons.Middle && rect.Contains(e.Location)))
                {
                    CloseTab(i);
                    return;
                }
            }
        }

        private void CloseCurrentTab()
        {
            if (mainTabControl.TabPages.Count == 0)
                return;

            CloseTab(mainTabControl.SelectedIndex);
        }

        private void CloseAllTabs()
        {
            while (mainTabControl.TabPages.Count > 0)
                CloseTab(0);
        }

        private void CloseTabByForm(Form form)
        {
            if (form == null)
                return;

            for (int i = 0; i < mainTabControl.TabPages.Count; i++)
            {
                if (ReferenceEquals(mainTabControl.TabPages[i].Tag, form))
                {
                    CloseTab(i);
                    return;
                }
            }
        }

        private void CloseTab(int index)
        {
            if (index < 0 || index >= mainTabControl.TabPages.Count)
                return;

            TabPage page = mainTabControl.TabPages[index];
            Form form = page.Tag as Form;

            if (form != null)
            {
                Type type = form.GetType();

                if (formsCache.ContainsKey(type))
                    formsCache.Remove(type);

                try
                {
                    if (contentHost.Controls.Contains(form))
                        contentHost.Controls.Remove(form);
                }
                catch { }

                try { form.Close(); } catch { }
                try { form.Dispose(); } catch { }
            }

            mainTabControl.TabPages.RemoveAt(index);

            if (mainTabControl.TabPages.Count > 0)
                ShowSelectedTabForm();

            UpdateStatus();
        }

        private void SetActiveButton(Button btn)
        {
            if (_activeButton != null)
            {
                _activeButton.BackColor = P.Primary;
                _activeButton.ForeColor = Color.White;
                _activeButton.FlatAppearance.BorderColor = P.Hover;
            }

            _activeButton = btn;

            if (_activeButton != null)
            {
                _activeButton.BackColor = P.Selected;
                _activeButton.ForeColor = P.SelectedText;
                _activeButton.FlatAppearance.BorderColor = P.Selected;
            }
        }

        private void UpdateStatus()
        {
            stReady.Text = "التبويبات المفتوحة: " + mainTabControl.TabCount;
        }

        private void LoadDashboard()
        {
            if (_navButtons.ContainsKey(typeof(ModernDashboardForm)))
                SetActiveButton(_navButtons[typeof(ModernDashboardForm)]);

            OpenForm<ModernDashboardForm>();
        }

        private void Logout()
        {
            DialogResult result = MessageBox.Show(
                "هل تريد تسجيل الخروج؟",
                "تأكيد",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            CurrentUser.Clear();
            Hide();

            using (LoginForm login = new LoginForm())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    formsCache.Clear();
                    mainTabControl.TabPages.Clear();
                    contentHost.Controls.Clear();
                    _activeButton = null;
                   

                    BuildSidebarContent();
                    Show();
                    LoadDashboard();
                    return;
                }
            }

            Close();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.W)
            {
                CloseCurrentTab();
                e.Handled = true;
            }

            if (e.Control && e.KeyCode == Keys.Tab)
            {
                if (mainTabControl.TabPages.Count > 0)
                {
                    int next = mainTabControl.SelectedIndex + 1;

                    if (next >= mainTabControl.TabPages.Count)
                        next = 0;

                    mainTabControl.SelectedIndex = next;
                }

                e.Handled = true;
            }

            if (e.Control && e.KeyCode == Keys.T)
            {
                btnTheme.PerformClick();
                e.Handled = true;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (updateTimer != null)
            {
                updateTimer.Stop();
                updateTimer.Dispose();
            }

            try { CloseAllTabs(); } catch { }
        }
    }
}



/*using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using water3.DB;
using water3.Forms;
using water3.Reports;
using water3.Reports.Dynamic;
using water3.Security;

namespace water3
{
    public partial class Form1 : Form
    {
        private readonly Dictionary<Type, Form> formsCache = new Dictionary<Type, Form>();
        private Button _activeButton;
        private bool _sidebarCollapsed = false;

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

            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            WindowState = FormWindowState.Maximized;
            Font = new Font("Segoe UI", 10f);
            BackColor = Bg;
            KeyPreview = true;

            ApplyThemeAndBaseStyles();
            BuildSidebarContent();

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

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            stClock.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

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

        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectedTabForm();
        }

        // ================== SIDEBAR ==================
        private void BuildSidebarContent()
        {
            sidebar.SuspendLayout();
            sidebar.Controls.Clear();

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
                Text = $"{CurrentUser.FullName} - {CurrentUser.RoleName}",
                ForeColor = Color.FromArgb(220, 255, 255, 255),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                TextAlign = ContentAlignment.TopRight
            };

            header.Controls.Add(headerSub);
            header.Controls.Add(headerTitle);
            sidebar.Controls.Add(header);

            AddSectionLabel("الرئيسية");
            AddNavButton("📊 لوحة التحكم", typeof(DashboardForm), PermissionKeys.DashboardView, () => OpenForm<DashboardForm>());

            AddSectionLabel("التشغيل");
            AddNavButton("📄 الفواتير", typeof(InvoiceForm), PermissionKeys.InvoicesView, () => OpenForm<InvoiceForm>());
            AddNavButton("👥 المشتركين", typeof(SubscriberForm), PermissionKeys.SubscribersView, () => OpenForm<SubscriberForm>());
            AddNavButton("🧾 المدفوعات", typeof(PaymentsForm), PermissionKeys.PaymentsView, () => OpenForm<PaymentsForm>());
            AddNavButton("🧮 قراءة العدادات", typeof(ReadingF), PermissionKeys.ReadingsView, () => OpenForm<ReadingF>());
            AddNavButton("🧩 إدخال القراءات", typeof(ReadingEntryForm), PermissionKeys.ReadingEntryView, () => OpenForm<ReadingEntryForm>());

            AddSectionLabel("التقارير");
            AddNavButton("📌 تقرير المشتركين", typeof(SubscribersBillingReportForm), PermissionKeys.SubscribersReportView, () => OpenForm<SubscribersBillingReportForm>());
            AddNavButton("📈 تقرير تحصيلات المحصلين", typeof(CollectorsReportForm), PermissionKeys.CollectorsReportView, () => OpenForm<CollectorsReportForm>());
            AddNavButton("📑 كشف حساب مشترك", typeof(AccountStatementForm), PermissionKeys.AccountStatementView, () => OpenForm<AccountStatementForm>());
            AddNavButton("🧩 الفواتير غير المسددة", typeof(UnpaidInvoicesReportForm), PermissionKeys.UnpaidInvoicesView, () => OpenForm<UnpaidInvoicesReportForm>());
            AddNavButton("🖨️ طباعة الفواتير", typeof(InvoicePrintPreviewForm), PermissionKeys.InvoicePrintView, () => OpenForm<InvoicePrintPreviewForm>());
            AddNavButton("🕘 سجل العمليات", typeof(AuditLogForm), PermissionKeys.AuditLogView, () => OpenForm<AuditLogForm>());

            AddSectionLabel("المستخدمون والصلاحيات");
            AddNavButton("👤 إدارة المستخدمين", typeof(UsersManagementForm), PermissionKeys.UsersView, () => OpenForm<UsersManagementForm>());
            AddNavButton("➕ إنشاء مستخدم", typeof(CreateUserForm), PermissionKeys.UsersCreate, () => OpenForm<CreateUserForm>());
            AddNavButton("🛡️ إدارة الأدوار والصلاحيات", typeof(RolesManagementForm), PermissionKeys.RolesManage, () => OpenForm<RolesManagementForm>());

            AddSectionLabel("المحصلون والمزامنة");
            AddNavButton("👤 إدارة المحصلين", typeof(CollectorsManagementForm), PermissionKeys.CollectorsView, () => OpenForm<CollectorsManagementForm>());
            AddNavButton("🔗 ربط المحصل بالمستخدم", typeof(CollectorUserLinkForm), PermissionKeys.CollectorsLinkUser, () => OpenForm<CollectorUserLinkForm>());
            AddNavButton("📱 أجهزة المحصلين", typeof(CollectorDevicesForm), PermissionKeys.CollectorDevicesView, () => OpenForm<CollectorDevicesForm>());
            AddNavButton("🔄 لوحة المزامنة الجوالة", typeof(FrmMobileSyncDashboard), PermissionKeys.MobileSyncView, () => OpenForm<FrmMobileSyncDashboard>());
            AddNavButton("📤 المزامنة إلى الهاتف", typeof(MobileSyncToPhoneForm), PermissionKeys.MobileSyncToPhoneView, () => OpenForm<MobileSyncToPhoneForm>());

            AddSectionLabel("الإعدادات والرسائل");
            AddNavButton("⚙️ إدارة الثوابت", typeof(BillingConstantsForm), PermissionKeys.BillingConstantsView, () => OpenForm<BillingConstantsForm>());
            AddNavButton("✉️ سجل الرسائل", typeof(SmsLogsForm), PermissionKeys.SmsLogsView, () => OpenForm<SmsLogsForm>());
            AddNavButton("📨 تقرير الرسائل", typeof(SmsReportForm), PermissionKeys.SmsReportView, () => OpenForm<SmsReportForm>());
            AddNavButton("🧩 إدارة الرسائل", typeof(MessagesManagementForm), PermissionKeys.MessagesManage, () => OpenForm<MessagesManagementForm>());
            AddNavButton("🔌 إدارة الاتصال", typeof(DbSettingsForm), PermissionKeys.DbSettingsView, () => OpenForm<DbSettingsForm>());
            //AddNavButton("📚 مركز التقارير", typeof(ReportsCenterForm), PermissionKeys.ReportsCenterView, () => OpenForm<ReportsCenterForm>());
            AddNavButton("🚰 تقرير العداد الرئيسي والفاقد", typeof(MainMeterReportForm), PermissionKeys.MainMeterReportView, () => OpenForm<MainMeterReportForm>());
            AddNavButton("💸 المصروفات والمشتريات", typeof(ExpensesManagementForm), PermissionKeys.ExpensesView, () => OpenForm<ExpensesManagementForm>());
            AddNavButton("🏷️ تصنيفات المصروفات", typeof(ExpenseCategoriesForm), PermissionKeys.ExpenseCategoriesManage, () => OpenForm<ExpenseCategoriesForm>());
            AddNavButton("📚 مركز التقارير", typeof(DynamicReportDesignerForm), PermissionKeys.DynamicReportDesignerForm, () => OpenForm<DynamicReportDesignerForm>());


            sidebar.Controls.Add(new Panel
            {
                Height = 10,
                Width = 255,
                BackColor = Primary,
                Margin = new Padding(0, 4, 0, 4)
            });

            AddSideToolButton("✖ إغلاق التبويب الحالي", CloseCurrentTab);
            AddSideToolButton("🧹 إغلاق كل التبويبات", CloseAllTabs);
            AddSideToolButton("🔓 تسجيل الخروج", Logout);
            AddSideToolButton("🚪 خروج", () => Close());

            sidebar.ResumeLayout();
        }

        private void AddSectionLabel(string text)
        {
            var lbl = new Label
            {
                Width = 255,
                Height = 28,
                Text = "   " + text,
                ForeColor = Color.FromArgb(230, 255, 255, 255),
                BackColor = Color.FromArgb(20, 255, 255, 255),
                Margin = new Padding(0, 8, 0, 8),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };

            sidebar.Controls.Add(lbl);
        }

        private void Header_Paint(object sender, PaintEventArgs e)
        {
            var header = sender as Panel;
            if (header == null) return;

            using (var pen = new Pen(Color.FromArgb(40, 255, 255, 255)))
                e.Graphics.DrawRectangle(pen, 0, 0, header.Width - 1, header.Height - 1);
        }

        private void AddNavButton(string text, Type formType, string permissionKey, Action action)
        {
            if (!HasPermission(permissionKey))
                return;

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
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Tag = new NavTag { FormType = formType, Action = action, PermissionKey = permissionKey }
            };

            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.FromArgb(30, 255, 255, 255);

            btn.MouseEnter += NavBtn_MouseEnter;
            btn.MouseLeave += NavBtn_MouseLeave;
            btn.Click += NavBtn_Click;

            sidebar.Controls.Add(btn);
        }

        private bool HasPermission(string permissionKey)
        {
            if (string.IsNullOrWhiteSpace(permissionKey))
                return true;

            if (CurrentUser.RoleID == 1)
                return true;

            return CurrentUser.HasPermission(permissionKey);
        }

        private class NavTag
        {
            public Type FormType { get; set; }
            public Action Action { get; set; }
            public string PermissionKey { get; set; }
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

            var form = new T
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill,
                WindowState = FormWindowState.Normal
            };

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

                if (xRect.Contains(e.Location) || (e.Button == MouseButtons.Middle && rect.Contains(e.Location)))
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
            stReady.Text = "📑 التبويبات: " + mainTabControl.TabCount;
        }

        private void LoadDashboard()
        {
            var dashBtn = sidebar.Controls.OfType<Button>()
                .FirstOrDefault(b => (b.Text ?? "").Contains("لوحة التحكم"));

            if (dashBtn != null) SetActiveButton(dashBtn);
            OpenForm<DashboardForm>();
        }

        private void Logout()
        {
            if (MessageBox.Show("هل تريد تسجيل الخروج؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            CurrentUser.Clear();
            Hide();

            using (var login = new LoginForm())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    formsCache.Clear();
                    mainTabControl.TabPages.Clear();
                    contentHost.Controls.Clear();
                    _activeButton = null;
                    BuildSidebarContent();
                    Show();
                    LoadDashboard();
                    return;
                }
            }

            Close();
        }

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
            if (updateTimer != null)
            {
                updateTimer.Stop();
                updateTimer.Dispose();
            }

            try { CloseAllTabs(); } catch { }
        }
    }
}
*/



