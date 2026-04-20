using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using water3.Services;

namespace water3.Forms
{
    public partial class InvoicePrintPreviewForm : Form
    {
        // ===== Theme =====
        private static readonly Color PrimaryColor = Color.FromArgb(0, 87, 183);
        private static readonly Color PrimaryDark = Color.FromArgb(0, 70, 150);
        private static readonly Color BackgroundColor = Color.FromArgb(245, 247, 250);
        private static readonly Color CardColor = Color.White;
        private static readonly Color BorderColor = Color.FromArgb(225, 230, 235);

        private static readonly Color SuccessColor = Color.FromArgb(40, 167, 69);
        private static readonly Color DangerColor = Color.FromArgb(220, 53, 69);
        private static readonly Color InfoColor = Color.FromArgb(33, 150, 243);

        private static readonly Color TextMutedColor = Color.FromArgb(120, 120, 120);
        private static readonly Color GridHeaderColor = Color.FromArgb(245, 248, 255);
        private static readonly Color GridAlternateRowColor = Color.FromArgb(252, 252, 252);
        private static readonly Color GridSelectionColor = Color.FromArgb(220, 235, 255);

        private static readonly Font SubtitleFont = new Font("Segoe UI", 11, FontStyle.Bold);
        private static readonly Font RegularFont = new Font("Segoe UI", 10);
        private static readonly Font SmallFont = new Font("Segoe UI", 9);
        private static readonly Font GridHeaderFont = new Font("Segoe UI", 10, FontStyle.Bold);

        private readonly InvoicePrintPreviewService _pageService;

        private Timer _searchDebounceTimer;
        private DataTable smsTable;
        private DataView smsView;

        // ====== Invoices grid columns ======
        private const string COL_SELECT = "Select";
        private const string COL_SMS_ID = "SmsID";
        private const string COL_INVOICE_ID = "رقم الفاتورة";
        private const string COL_SUBSCRIBER_ID = "SubscriberID";
        private const string COL_PHONE = "رقم الهاتف";
        private const string COL_NAME = "اسم المشترك";
        private const string COL_MESSAGE = "نص الرسالة";

        // ====== Payments grid columns ======
        private const string PCOL_SELECT = "PSelect";
        private const string PCOL_SMS_ID = "SmsID";
        private const string PCOL_PAYMENT_ID = "PaymentID";
        private const string PCOL_RECEIPT_ID = "ReceiptID";
        private const string PCOL_SUB_ID = "SubscriberID";
        private const string PCOL_NAME = "اسم المشترك";
        private const string PCOL_PHONE = "رقم الهاتف";
        private const string PCOL_MESSAGE = "نص الرسالة";

        // ===== SMS Gateway (Android) =====
        private AndroidSmsGatewayClient _gatewayClient;
        private SmsGatewaySettings _gatewaySettings;

        // UI controls
        private TextBox txtGatewayIp;
        private TextBox txtGatewayKey;
        private Button btnGatewayTest;
        private Button btnGatewaySave;

        // Header select-all checkboxes
        private CheckBox _chkAllInvoices;
        private CheckBox _chkAllPayments;

        public InvoicePrintPreviewForm()
        {
            InitializeComponent();

            _pageService = new InvoicePrintPreviewService(new water3.Repositories.SmsLogsRepository());

            InitializeForm();
            ApplyTheme();
            InitializeSmsGatewayUi();
            SetupFilters();
            SetupGrids();
            WireEvents();

            LoadInitialData();
        }

        private void InitializeForm()
        {
            Text = "إرسال الفواتير والسداد";
            BackColor = BackgroundColor;
            WindowState = FormWindowState.Maximized;
            Font = RegularFont;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
        }

        private void ApplyTheme()
        {
            cardFilterStats.BackColor = CardColor;
            cardToolbar.BackColor = CardColor;
            cardContent.BackColor = CardColor;

            StyleButton(btnSearch, PrimaryColor, Color.White, outline: false);
            StyleButton(btnPrint, PrimaryColor, Color.White, outline: false);
            StyleButton(btnPrintAll, PrimaryDark, Color.White, outline: false);
            StyleButton(btnSMS, InfoColor, Color.White, outline: false);
            StyleButton(btnSMSAll, SuccessColor, Color.White, outline: false);
            StyleButton(btnExport, Color.White, PrimaryColor, outline: true);

            txtSearch.Font = RegularFont;
            cbSubscriberFilter.Font = RegularFont;
            cbStatusFilter.Font = RegularFont;
            cbPaymentTypeFilter.Font = RegularFont;
            dtFrom.Font = RegularFont;
            dtTo.Font = RegularFont;

            foreach (var lbl in new[] { lblTotalInvoices, lblTotalAmount, lblPaidAmount, lblRemainingAmount })
            {
                lbl.BackColor = CardColor;
                lbl.Font = SmallFont;
                lbl.ForeColor = TextMutedColor;
                lbl.Padding = new Padding(12);
                lbl.Margin = new Padding(6);
                lbl.Paint += (s, e) =>
                {
                    using (var pen = new Pen(BorderColor))
                        e.Graphics.DrawRectangle(pen, 0, 0, lbl.Width - 1, lbl.Height - 1);
                };
            }

            lblTotalInvoices.Paint += (s, e) => e.Graphics.FillRectangle(new SolidBrush(PrimaryColor), 0, 0, lblTotalInvoices.Width, 3);
            lblTotalAmount.Paint += (s, e) => e.Graphics.FillRectangle(new SolidBrush(InfoColor), 0, 0, lblTotalAmount.Width, 3);
            lblPaidAmount.Paint += (s, e) => e.Graphics.FillRectangle(new SolidBrush(SuccessColor), 0, 0, lblPaidAmount.Width, 3);
            lblRemainingAmount.Paint += (s, e) => e.Graphics.FillRectangle(new SolidBrush(DangerColor), 0, 0, lblRemainingAmount.Width, 3);

            loadingPanel.BackColor = Color.FromArgb(110, 0, 0, 0);
            loadingContainer.BackColor = Color.White;
            lblLoading.Font = SubtitleFont;
            lblLoading.ForeColor = PrimaryColor;

            loadingContainer.Paint += (s, e) =>
            {
                using (var pen = new Pen(BorderColor))
                    e.Graphics.DrawRectangle(pen, 0, 0, loadingContainer.Width - 1, loadingContainer.Height - 1);
            };

            loadingPanel.Resize += (s, e) =>
            {
                loadingContainer.Left = (loadingPanel.Width - loadingContainer.Width) / 2;
                loadingContainer.Top = (loadingPanel.Height - loadingContainer.Height) / 2;
            };
        }

        private void InitializeSmsGatewayUi()
        {
            _gatewaySettings = SmsGatewaySettings.Load();

            _gatewayClient = new AndroidSmsGatewayClient
            {
                PhoneIp = _gatewaySettings.PhoneIp,
                Port = _gatewaySettings.Port,
                ApiKey = _gatewaySettings.ApiKey
            };

            var pnl = new Panel
            {
                Width = 560,
                Height = 36,
                Margin = new Padding(6, 0, 6, 0),
                BackColor = Color.Transparent
            };

            var lblIp = new Label
            {
                AutoSize = true,
                Text = "IP:",
                Location = new Point(540, 9),
                ForeColor = TextMutedColor
            };

            txtGatewayIp = new TextBox
            {
                Width = 130,
                Height = 24,
                Location = new Point(405, 6),
                Text = _gatewaySettings.PhoneIp,
                Font = RegularFont
            };

            var lblKey = new Label
            {
                AutoSize = true,
                Text = "Key:",
                Location = new Point(378, 9),
                ForeColor = TextMutedColor
            };

            txtGatewayKey = new TextBox
            {
                Width = 120,
                Height = 24,
                Location = new Point(250, 6),
                Text = _gatewaySettings.ApiKey,
                Font = RegularFont,
                UseSystemPasswordChar = true
            };

            var lblPort = new Label
            {
                AutoSize = true,
                Text = ":8080",
                Location = new Point(210, 9),
                ForeColor = TextMutedColor
            };

            btnGatewayTest = new Button
            {
                Text = "Test",
                Width = 70,
                Height = 28,
                Location = new Point(135, 4)
            };
            StyleButton(btnGatewayTest, InfoColor, Color.White, outline: false);

            btnGatewaySave = new Button
            {
                Text = "Save",
                Width = 70,
                Height = 28,
                Location = new Point(60, 4)
            };
            StyleButton(btnGatewaySave, SuccessColor, Color.White, outline: false);

            pnl.Controls.Add(lblIp);
            pnl.Controls.Add(txtGatewayIp);
            pnl.Controls.Add(lblKey);
            pnl.Controls.Add(txtGatewayKey);
            pnl.Controls.Add(lblPort);
            pnl.Controls.Add(btnGatewayTest);
            pnl.Controls.Add(btnGatewaySave);

            toolbarFlow.Controls.Add(pnl);
            toolbarFlow.Controls.SetChildIndex(pnl, 0);

            btnGatewaySave.Click += (s, e) =>
            {
                ApplyGatewaySettingsFromUi(save: true);
                MessageBox.Show("تم حفظ إعدادات SMS Gateway ✅", "SMS Gateway",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnGatewayTest.Click += async (s, e) =>
            {
                ApplyGatewaySettingsFromUi(save: false);

                ShowLoading(true, "جاري اختبار الاتصال بالهاتف...");
                var (ok, raw) = await _gatewayClient.HealthAsync();
                ShowLoading(false);

                MessageBox.Show(ok ? $"✅ الاتصال ناجح\n{raw}" : $"❌ فشل الاتصال\n{raw}",
                    "SMS Gateway Test",
                    MessageBoxButtons.OK,
                    ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            };
        }

        private void ApplyGatewaySettingsFromUi(bool save)
        {
            var ip = (txtGatewayIp.Text ?? "").Trim();
            var key = (txtGatewayKey.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(ip))
                ip = "192.168.1.20";

            if (string.IsNullOrWhiteSpace(key))
                key = "123456";

            _gatewaySettings.PhoneIp = ip;
            _gatewaySettings.ApiKey = key;
            _gatewaySettings.Port = 8080;

            _gatewayClient.PhoneIp = _gatewaySettings.PhoneIp;
            _gatewayClient.ApiKey = _gatewaySettings.ApiKey;
            _gatewayClient.Port = _gatewaySettings.Port;

            if (save) _gatewaySettings.Save();
        }

        private void StyleButton(Button btn, Color backColor, Color foreColor, bool outline)
        {
            btn.Font = RegularFont;
            btn.Cursor = Cursors.Hand;
            btn.FlatStyle = FlatStyle.Flat;

            btn.BackColor = outline ? Color.White : backColor;
            btn.ForeColor = outline ? PrimaryColor : foreColor;

            btn.FlatAppearance.BorderSize = outline ? 1 : 0;
            btn.FlatAppearance.BorderColor = PrimaryColor;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.12f);
            btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.2f);
        }

        private void SetupFilters()
        {
            cbStatusFilter.Items.Clear();
            cbStatusFilter.Items.AddRange(new object[] { "جميع الحالات", "مدفوعة", "غير مدفوعة", "جزئية" });
            cbStatusFilter.SelectedIndex = 0;

            cbPaymentTypeFilter.Items.Clear();
            cbPaymentTypeFilter.Items.AddRange(new object[] { "كل طرق السداد", "نقداً", "تحويل", "شيك", "أخرى" });
            cbPaymentTypeFilter.SelectedIndex = 0;

            dtFrom.Value = DateTime.Today.AddMonths(-1);
            dtTo.Value = DateTime.Today;

            ToggleFiltersByTab();
        }

        private string MapPaymentTypeFilterToDb()
        {
            switch ((cbPaymentTypeFilter.Text ?? "").Trim())
            {
                case "نقداً": return "Cash";
                case "تحويل": return "Transfer";
                case "شيك": return "Cheque";
                case "أخرى": return "Other";
                default: return null;
            }
        }

        private void ToggleFiltersByTab()
        {
            bool isInvoices = tabControl.SelectedTab == tabInvoices;

            cbStatusFilter.Visible = isInvoices;
            lblStatus.Visible = isInvoices;

            cbPaymentTypeFilter.Visible = !isInvoices;
            lblPaymentType.Visible = !isInvoices;
        }

        private void SetupGrids()
        {
            SetupGridStyle(dgvInvoices, selectable: true, selectColumnName: COL_SELECT);
            SetupGridStyle(dgvPayments, selectable: true, selectColumnName: PCOL_SELECT);
            SetupGridStyle(dgvSmsLogs, selectable: false, selectColumnName: null);

            dgvSmsLogs.ReadOnly = true;
            dgvSmsLogs.MultiSelect = false;
            dgvSmsLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            _chkAllInvoices = InstallSelectAllHeader(dgvInvoices, COL_SELECT, _chkAllInvoices);
            _chkAllPayments = InstallSelectAllHeader(dgvPayments, PCOL_SELECT, _chkAllPayments);
        }

        private void SetupGridStyle(DataGridView grid, bool selectable, string selectColumnName)
        {
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = CardColor;
            grid.BorderStyle = BorderStyle.None;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = selectable;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersHeight = 40;
            grid.RowTemplate.Height = 34;
            grid.GridColor = BorderColor;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = GridHeaderColor,
                ForeColor = Color.FromArgb(60, 60, 60),
                Font = GridHeaderFont,
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(6, 0, 6, 0)
            };

            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = RegularFont,
                ForeColor = Color.FromArgb(60, 60, 60),
                SelectionBackColor = GridSelectionColor,
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(6, 0, 6, 0)
            };

            grid.AlternatingRowsDefaultCellStyle.BackColor = GridAlternateRowColor;

            if (selectable && !string.IsNullOrWhiteSpace(selectColumnName) && !grid.Columns.Contains(selectColumnName))
            {
                var check = new DataGridViewCheckBoxColumn
                {
                    Name = selectColumnName,
                    HeaderText = "تحديد",
                    Width = 70,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None
                };
                grid.Columns.Add(check);

                grid.CellClick += (s, e) =>
                {
                    if (e.RowIndex >= 0 && e.ColumnIndex == grid.Columns[selectColumnName].Index)
                        grid.EndEdit();
                };
            }
        }

        private CheckBox InstallSelectAllHeader(DataGridView grid, string selectColumnName, CheckBox existing)
        {
            if (existing != null) return existing;
            if (!grid.Columns.Contains(selectColumnName)) return null;

            var headerCheck = new CheckBox
            {
                Size = new Size(16, 16),
                BackColor = Color.Transparent
            };

            grid.Controls.Add(headerCheck);

            Action reposition = () =>
            {
                try
                {
                    var rect = grid.GetCellDisplayRectangle(grid.Columns[selectColumnName].Index, -1, true);
                    headerCheck.Location = new Point(
                        rect.Left + (rect.Width - headerCheck.Width) / 2,
                        rect.Top + (rect.Height - headerCheck.Height) / 2
                    );
                }
                catch { }
            };

            grid.ColumnWidthChanged += (s, e) => reposition();
            grid.Scroll += (s, e) => reposition();
            grid.SizeChanged += (s, e) => reposition();
            grid.DataBindingComplete += (s, e) => reposition();

            headerCheck.CheckedChanged += (s, e) =>
            {
                grid.EndEdit();
                bool check = headerCheck.Checked;

                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow) continue;
                    var cell = row.Cells[selectColumnName];
                    if (cell != null) cell.Value = check;
                }
            };

            reposition();
            return headerCheck;
        }

        private void WireEvents()
        {
            btnSearch.Click += async (s, e) =>
            {
                await ReloadCurrentTabAsync();
                await LoadSmsLogsAsync();
            };

            btnPrint.Click += (s, e) =>
            {
                if (tabControl.SelectedTab != tabInvoices)
                {
                    MessageBox.Show("الطباعة متاحة للفواتير فقط.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                PrintSelectedInvoices();
            };

            btnPrintAll.Click += (s, e) =>
            {
                if (tabControl.SelectedTab != tabInvoices)
                {
                    MessageBox.Show("الطباعة متاحة للفواتير فقط.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                PrintAllInvoices();
            };

            btnSMS.Click += async (s, e) => await SendSmsSelectedByTabAsync();
            btnSMSAll.Click += async (s, e) => await SendSmsAllByTabAsync();
            btnExport.Click += (s, e) => ExportCurrentGridToCsv();

            _searchDebounceTimer = new Timer { Interval = 350 };
            _searchDebounceTimer.Tick += async (s, e) =>
            {
                _searchDebounceTimer.Stop();
                await ReloadCurrentTabAsync();
                await LoadSmsLogsAsync();
            };

            txtSearch.TextChanged += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                _searchDebounceTimer.Start();
            };

            cbSubscriberFilter.SelectedIndexChanged += async (s, e) =>
            {
                await ReloadCurrentTabAsync();
                await LoadSmsLogsAsync();
            };

            cbStatusFilter.SelectedIndexChanged += async (s, e) =>
            {
                if (tabControl.SelectedTab == tabInvoices)
                {
                    await LoadInvoicesAsync();
                    await LoadSmsLogsAsync();
                }
            };

            cbPaymentTypeFilter.SelectedIndexChanged += async (s, e) =>
            {
                if (tabControl.SelectedTab == tabPayments)
                {
                    await LoadPaymentsAsync();
                    await LoadSmsLogsAsync();
                }
            };

            dtFrom.ValueChanged += async (s, e) =>
            {
                await ReloadCurrentTabAsync();
                await LoadSmsLogsAsync();
            };

            dtTo.ValueChanged += async (s, e) =>
            {
                await ReloadCurrentTabAsync();
                await LoadSmsLogsAsync();
            };

            tabControl.SelectedIndexChanged += async (s, e) =>
            {
                ToggleFiltersByTab();
                await ReloadCurrentTabAsync();
            };
        }

        private async Task ReloadCurrentTabAsync()
        {
            if (tabControl.SelectedTab == tabInvoices)
                await LoadInvoicesAsync();
            else if (tabControl.SelectedTab == tabPayments)
                await LoadPaymentsAsync();
        }

        private async void LoadInitialData()
        {
            ShowLoading(true, "جاري تحميل البيانات...");
            await LoadSubscribersAsync();
            await ReloadCurrentTabAsync();
            await LoadSmsLogsAsync();
            ShowLoading(false);
        }

        private async Task LoadSubscribersAsync()
        {
            try
            {
                using (var con = Db.GetConnection())
                {
                    await con.OpenAsync();

                    using (var cmd = new System.Data.SqlClient.SqlCommand(
                        "SELECT SubscriberID, Name FROM Subscribers WHERE IsActive = 1 ORDER BY Name", con))
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        cbSubscriberFilter.Items.Clear();
                        cbSubscriberFilter.Items.Add(new ComboItem("جميع المشتركين", "0"));

                        while (await r.ReadAsync())
                        {
                            cbSubscriberFilter.Items.Add(new ComboItem(
                                r["Name"].ToString(),
                                r["SubscriberID"].ToString()
                            ));
                        }

                        cbSubscriberFilter.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل المشتركين: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadInvoicesAsync()
        {
            try
            {
                ShowLoading(true, "جاري تحميل الفواتير غير المرسلة...");

                if (dtFrom.Value.Date > dtTo.Value.Date)
                {
                    MessageBox.Show("تاريخ (من) يجب أن يكون أقل أو يساوي (إلى).");
                    return;
                }

                string search = (txtSearch.Text ?? "").Trim();
                int sid = GetSelectedSubscriberId();
                string invoiceStatus = cbStatusFilter.SelectedIndex <= 0 ? null : cbStatusFilter.Text;

                var dt = await Task.Run(() =>
                    _pageService.GetPendingInvoiceMessages(
                        dtFrom.Value.Date,
                        dtTo.Value.Date.AddDays(1),
                        sid,
                        string.IsNullOrWhiteSpace(search) ? null : $"%{search}%",
                        invoiceStatus
                    ));

                BindInvoicesToGrid(dt);
                UpdateStatisticsFromInvoiceTable(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الفواتير غير المرسلة: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private async Task LoadPaymentsAsync()
        {
            try
            {
                ShowLoading(true, "جاري تحميل السداد غير المرسل...");

                if (dtFrom.Value.Date > dtTo.Value.Date)
                {
                    MessageBox.Show("تاريخ (من) يجب أن يكون أقل أو يساوي (إلى).");
                    return;
                }

                string search = (txtSearch.Text ?? "").Trim();
                int sid = GetSelectedSubscriberId();
                string paymentMethodDb = MapPaymentTypeFilterToDb();

                var dt = await Task.Run(() =>
                    _pageService.GetPendingPaymentMessages(
                        dtFrom.Value.Date,
                        dtTo.Value.Date.AddDays(1),
                        sid,
                        string.IsNullOrWhiteSpace(search) ? null : $"%{search}%",
                        paymentMethodDb
                    ));

                BindPaymentsToGrid(dt);
                UpdateStatisticsFromPaymentsTable(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل السداد غير المرسل: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private int GetSelectedSubscriberId()
        {
            int sid = 0;
            if (cbSubscriberFilter.SelectedItem is ComboItem it && int.TryParse(it.Value, out int parsed))
                sid = parsed;
            return sid;
        }

        private void BindInvoicesToGrid(DataTable dt)
        {
            dgvInvoices.SuspendLayout();

            for (int i = dgvInvoices.Columns.Count - 1; i >= 0; i--)
                if (dgvInvoices.Columns[i].Name != COL_SELECT)
                    dgvInvoices.Columns.RemoveAt(i);

            dgvInvoices.DataSource = null;
            dgvInvoices.AutoGenerateColumns = true;
            dgvInvoices.DataSource = dt;

            if (dgvInvoices.Columns.Contains(COL_SELECT))
            {
                dgvInvoices.Columns[COL_SELECT].DisplayIndex = 0;
                dgvInvoices.Columns[COL_SELECT].ReadOnly = false;
                dgvInvoices.Columns[COL_SELECT].HeaderText = "تحديد";
                dgvInvoices.Columns[COL_SELECT].Width = 60;
            }

            if (dgvInvoices.Columns.Contains(COL_SMS_ID)) dgvInvoices.Columns[COL_SMS_ID].Visible = false;
            if (dgvInvoices.Columns.Contains("TemplateID")) dgvInvoices.Columns["TemplateID"].Visible = false;
            if (dgvInvoices.Columns.Contains(COL_SUBSCRIBER_ID)) dgvInvoices.Columns[COL_SUBSCRIBER_ID].Visible = false;

            FormatDecimalColumn(dgvInvoices, "مبلغ الفترة");
            FormatDecimalColumn(dgvInvoices, "المتأخرات وقت الإصدار");
            FormatDecimalColumn(dgvInvoices, "المدفوع على الفاتورة");
            FormatDecimalColumn(dgvInvoices, "متبقي الفاتورة");
            FormatDecimalColumn(dgvInvoices, "الرصيد الحالي");

            foreach (DataGridViewColumn col in dgvInvoices.Columns)
            {
                if (col.Name == COL_SELECT) continue;
                col.ReadOnly = true;
            }

            dgvInvoices.ResumeLayout();

            _chkAllInvoices?.BringToFront();
            if (_chkAllInvoices != null) _chkAllInvoices.Checked = false;
        }

        private void BindPaymentsToGrid(DataTable dt)
        {
            dgvPayments.SuspendLayout();

            for (int i = dgvPayments.Columns.Count - 1; i >= 0; i--)
                if (dgvPayments.Columns[i].Name != PCOL_SELECT)
                    dgvPayments.Columns.RemoveAt(i);

            dgvPayments.DataSource = null;
            dgvPayments.AutoGenerateColumns = true;
            dgvPayments.DataSource = dt;

            if (dgvPayments.Columns.Contains(PCOL_SELECT))
            {
                dgvPayments.Columns[PCOL_SELECT].DisplayIndex = 0;
                dgvPayments.Columns[PCOL_SELECT].ReadOnly = false;
                dgvPayments.Columns[PCOL_SELECT].HeaderText = "تحديد";
                dgvPayments.Columns[PCOL_SELECT].Width = 60;
            }

            if (dgvPayments.Columns.Contains(PCOL_SMS_ID)) dgvPayments.Columns[PCOL_SMS_ID].Visible = false;
            if (dgvPayments.Columns.Contains("TemplateID")) dgvPayments.Columns["TemplateID"].Visible = false;
            if (dgvPayments.Columns.Contains(PCOL_PAYMENT_ID)) dgvPayments.Columns[PCOL_PAYMENT_ID].Visible = false;
            if (dgvPayments.Columns.Contains(PCOL_RECEIPT_ID)) dgvPayments.Columns[PCOL_RECEIPT_ID].Visible = false;
            if (dgvPayments.Columns.Contains(PCOL_SUB_ID)) dgvPayments.Columns[PCOL_SUB_ID].Visible = false;

            FormatDecimalColumn(dgvPayments, "إجمالي الإيصال");
            FormatDecimalColumn(dgvPayments, "مسدد على الفواتير");
            FormatDecimalColumn(dgvPayments, "رصيد مقدم");
            FormatDecimalColumn(dgvPayments, "الرصيد الحالي");

            foreach (DataGridViewColumn col in dgvPayments.Columns)
            {
                if (col.Name == PCOL_SELECT) continue;
                col.ReadOnly = true;
            }

            dgvPayments.ResumeLayout();

            _chkAllPayments?.BringToFront();
            if (_chkAllPayments != null) _chkAllPayments.Checked = false;
        }

        private void FormatDecimalColumn(DataGridView grid, string columnName)
        {
            if (!grid.Columns.Contains(columnName)) return;

            grid.Columns[columnName].DefaultCellStyle.Format = "N2";
            grid.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void UpdateStatisticsFromInvoiceTable(DataTable dt)
        {
            try
            {
                int totalInvoices = dt.Rows.Count;
                decimal totalPeriod = 0m;
                decimal totalArrearsAtIssue = 0m;
                decimal totalPaid = 0m;
                decimal totalRemaining = 0m;

                foreach (DataRow row in dt.Rows)
                {
                    totalPeriod += SafeDec(row["مبلغ الفترة"]);
                    totalArrearsAtIssue += SafeDec(row["المتأخرات وقت الإصدار"]);
                    totalPaid += SafeDec(row["المدفوع على الفاتورة"]);
                    totalRemaining += SafeDec(row["متبقي الفاتورة"]);
                }

                lblTotalInvoices.Text = $"📄 رسائل فواتير غير مرسلة\n{totalInvoices:N0}";
                lblTotalAmount.Text = $"💰 مبلغ الفترات\n{totalPeriod:N2}";
                lblPaidAmount.Text = $"💳 المدفوع على الفواتير\n{totalPaid:N2}";
                lblRemainingAmount.Text = $"⚠️ متبقي الفواتير\n{totalRemaining:N2}";
            }
            catch { }
        }

        private void UpdateStatisticsFromPaymentsTable(DataTable dt)
        {
            try
            {
                int total = dt.Rows.Count;
                decimal totalReceipts = 0m;
                decimal totalPaidToInvoices = 0m;
                decimal totalAdvance = 0m;

                foreach (DataRow row in dt.Rows)
                {
                    totalReceipts += SafeDec(row["إجمالي الإيصال"]);
                    totalPaidToInvoices += SafeDec(row["مسدد على الفواتير"]);
                    totalAdvance += SafeDec(row["رصيد مقدم"]);
                }

                lblTotalInvoices.Text = $"🧾 رسائل سداد غير مرسلة\n{total:N0}";
                lblTotalAmount.Text = $"💰 إجمالي الإيصالات\n{totalReceipts:N2}";
                lblPaidAmount.Text = $"💳 مسدد على الفواتير\n{totalPaidToInvoices:N2}";
                lblRemainingAmount.Text = $"🟢 رصيد مقدم\n{totalAdvance:N2}";
            }
            catch { }
        }

        private async Task LoadSmsLogsAsync()
        {
            try
            {
                string search = (txtSearch.Text ?? "").Trim();
                int sid = GetSelectedSubscriberId();

                smsTable = await Task.Run(() =>
                    _pageService.GetSmsLogs(
                        dtFrom.Value.Date,
                        dtTo.Value.Date.AddDays(1),
                        sid,
                        string.IsNullOrWhiteSpace(search) ? null : $"%{search}%"
                    ));

                smsView = smsTable.DefaultView;
                dgvSmsLogs.DataSource = smsView;

                if (dgvSmsLogs.Columns.Contains("SmsID")) dgvSmsLogs.Columns["SmsID"].HeaderText = "رقم الرسالة";
                if (dgvSmsLogs.Columns.Contains("MessageType")) dgvSmsLogs.Columns["MessageType"].HeaderText = "نوع الرسالة";
                if (dgvSmsLogs.Columns.Contains("InvoiceID")) dgvSmsLogs.Columns["InvoiceID"].HeaderText = "رقم الفاتورة";
                if (dgvSmsLogs.Columns.Contains("PaymentID")) dgvSmsLogs.Columns["PaymentID"].HeaderText = "رقم السداد";
                if (dgvSmsLogs.Columns.Contains("ReceiptID")) dgvSmsLogs.Columns["ReceiptID"].HeaderText = "رقم الإيصال";
                if (dgvSmsLogs.Columns.Contains("SubscriberName")) dgvSmsLogs.Columns["SubscriberName"].HeaderText = "اسم المشترك";
                if (dgvSmsLogs.Columns.Contains("PhoneNumber")) dgvSmsLogs.Columns["PhoneNumber"].HeaderText = "رقم الهاتف";
                if (dgvSmsLogs.Columns.Contains("Message")) dgvSmsLogs.Columns["Message"].HeaderText = "نص الرسالة";
                if (dgvSmsLogs.Columns.Contains("Status")) dgvSmsLogs.Columns["Status"].HeaderText = "الحالة";
                if (dgvSmsLogs.Columns.Contains("Reason")) dgvSmsLogs.Columns["Reason"].HeaderText = "السبب";
                if (dgvSmsLogs.Columns.Contains("CreatedAt")) dgvSmsLogs.Columns["CreatedAt"].HeaderText = "تاريخ الإنشاء";
                if (dgvSmsLogs.Columns.Contains("SentDate")) dgvSmsLogs.Columns["SentDate"].HeaderText = "تاريخ الإرسال";

                if (dgvSmsLogs.Columns.Contains("Message"))
                {
                    dgvSmsLogs.Columns["Message"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dgvSmsLogs.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل سجل الرسائل: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintSelectedInvoices()
        {
            var ids = GetSelectedInvoiceIds();
            if (ids.Length == 0)
            {
                MessageBox.Show("يرجى تحديد فواتير للطباعة", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("اربط الطباعة بـ ReportViewer أو PrintDocument حسب مشروعك.", "ملاحظة",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PrintAllInvoices()
        {
            MessageBox.Show("اربط الطباعة بـ ReportViewer أو PrintDocument حسب مشروعك.", "ملاحظة",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string[] GetSelectedInvoiceIds()
        {
            return dgvInvoices.Rows.Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Where(r => r.Cells[COL_SELECT] != null && Convert.ToBoolean(r.Cells[COL_SELECT].Value) == true)
                .Select(r => r.Cells[COL_INVOICE_ID]?.Value?.ToString())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToArray();
        }

        private async Task SendSmsSelectedByTabAsync()
        {
            if (tabControl.SelectedTab == tabInvoices)
                await SendInvoiceSmsSelectedAsync();
            else if (tabControl.SelectedTab == tabPayments)
                await SendPaymentSmsSelectedAsync();
        }

        private async Task SendSmsAllByTabAsync()
        {
            if (tabControl.SelectedTab == tabInvoices)
                await SendInvoiceSmsAllAsync();
            else if (tabControl.SelectedTab == tabPayments)
                await SendPaymentSmsAllAsync();
        }

        private async Task SendInvoiceSmsSelectedAsync()
        {
            var selected = dgvInvoices.Rows.Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Where(r => r.Cells[COL_SELECT] != null && Convert.ToBoolean(r.Cells[COL_SELECT].Value) == true)
                .ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show("يرجى تحديد رسائل فواتير للإرسال", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await SendSmsRowsAsync(selected, COL_SMS_ID, COL_PHONE, COL_MESSAGE, "الفواتير المختارة");
        }

        private async Task SendInvoiceSmsAllAsync()
        {
            var result = MessageBox.Show("هل تريد إرسال جميع رسائل الفواتير غير المرسلة المعروضة؟", "تأكيد الإرسال",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            var allRows = dgvInvoices.Rows.Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .ToList();

            await SendSmsRowsAsync(allRows, COL_SMS_ID, COL_PHONE, COL_MESSAGE, "جميع الفواتير");
        }

        private async Task SendPaymentSmsSelectedAsync()
        {
            var selected = dgvPayments.Rows.Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Where(r => r.Cells[PCOL_SELECT] != null && Convert.ToBoolean(r.Cells[PCOL_SELECT].Value) == true)
                .ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show("يرجى تحديد رسائل سداد للإرسال", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await SendSmsRowsAsync(selected, PCOL_SMS_ID, PCOL_PHONE, PCOL_MESSAGE, "السداد المختار");
        }

        private async Task SendPaymentSmsAllAsync()
        {
            var result = MessageBox.Show("هل تريد إرسال جميع رسائل السداد غير المرسلة المعروضة؟", "تأكيد الإرسال",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            var allRows = dgvPayments.Rows.Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .ToList();

            await SendSmsRowsAsync(allRows, PCOL_SMS_ID, PCOL_PHONE, PCOL_MESSAGE, "جميع السداد");
        }

        private async Task<(bool ok, string raw)> EnsureGatewayOnlineAsync()
        {
            ApplyGatewaySettingsFromUi(save: false);
            var (hOk, hRaw) = await _gatewayClient.HealthAsync();
            return (hOk, hRaw);
        }

        private async Task SendSmsRowsAsync(
            List<DataGridViewRow> rows,
            string smsIdColumn,
            string phoneColumn,
            string messageColumn,
            string description)
        {
            var (hOk, hRaw) = await EnsureGatewayOnlineAsync();
            if (!hOk)
            {
                MessageBox.Show("الهاتف غير متصل أو الخدمة مطفأة.\n\n" + hRaw,
                    "SMS Gateway", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ShowLoading(true, $"جاري إرسال الرسائل ({description})...");

            int successCount = 0;
            int failCount = 0;

            foreach (var row in rows)
            {
                int smsId = 0;

                try
                {
                    smsId = Convert.ToInt32(row.Cells[smsIdColumn].Value);
                    string phone = row.Cells[phoneColumn]?.Value?.ToString();
                    string message = row.Cells[messageColumn]?.Value?.ToString();

                    if (string.IsNullOrWhiteSpace(phone))
                    {
                        _pageService.MarkFailed(smsId, "رقم هاتف غير صالح");
                        failCount++;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        _pageService.MarkFailed(smsId, "نص الرسالة فارغ");
                        failCount++;
                        continue;
                    }

                    var (ok, status, raw, outId) = await _gatewayClient.SendSmsAsync(phone, message);

                    if (ok)
                    {
                        _pageService.MarkSent(smsId);
                        successCount++;
                    }
                    else
                    {
                        _pageService.MarkFailed(smsId, raw);
                        failCount++;
                    }

                    await Task.Delay(200);
                }
                catch (Exception ex)
                {
                    if (smsId > 0)
                        _pageService.MarkFailed(smsId, ex.Message);

                    failCount++;
                }
            }

            ShowLoading(false);

            await ReloadCurrentTabAsync();
            await LoadSmsLogsAsync();

            MessageBox.Show($"تم إرسال {successCount} رسالة بنجاح، وفشل {failCount} رسالة",
                "نتيجة الإرسال", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ExportCurrentGridToCsv()
        {
            DataGridView grid = tabControl.SelectedTab == tabPayments ? dgvPayments : dgvInvoices;

            if (grid.DataSource == null || grid.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للتصدير.");
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel CSV (*.csv)|*.csv";
                sfd.FileName = (tabControl.SelectedTab == tabPayments)
                    ? $"Payments_{DateTime.Now:yyyyMMdd_HHmm}.csv"
                    : $"Invoices_{DateTime.Now:yyyyMMdd_HHmm}.csv";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var sb = new StringBuilder();

                    var cols = grid.Columns.Cast<DataGridViewColumn>()
                        .Where(c => c.Visible && c.Name != COL_SELECT && c.Name != PCOL_SELECT)
                        .OrderBy(c => c.DisplayIndex)
                        .ToList();

                    sb.AppendLine(string.Join(",", cols.Select(c => EscapeCsv(c.HeaderText))));

                    foreach (DataGridViewRow row in grid.Rows)
                    {
                        if (row.IsNewRow) continue;

                        var line = cols.Select(c =>
                        {
                            var v = row.Cells[c.Name]?.Value?.ToString() ?? "";
                            return EscapeCsv(v);
                        });

                        sb.AppendLine(string.Join(",", line));
                    }

                    System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("تم التصدير بنجاح ✅");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("فشل التصدير: " + ex.Message);
                }
            }
        }

        private string EscapeCsv(string s)
        {
            if (s == null) return "";
            if (s.Contains(",") || s.Contains("\"") || s.Contains("\n"))
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        private decimal SafeDec(object o)
        {
            if (o == null || o == DBNull.Value) return 0m;
            decimal.TryParse(o.ToString(), out decimal x);
            return x;
        }

        private void ShowLoading(bool show, string message = "")
        {
            loadingPanel.Visible = show;
            if (!string.IsNullOrEmpty(message)) lblLoading.Text = message;
            loadingPanel.BringToFront();

            btnSearch.Enabled = !show;
            btnPrint.Enabled = !show;
            btnPrintAll.Enabled = !show;
            btnSMS.Enabled = !show;
            btnSMSAll.Enabled = !show;
            btnExport.Enabled = !show;

            dgvInvoices.Enabled = !show;
            dgvPayments.Enabled = !show;
            dgvSmsLogs.Enabled = !show;
        }

        private class ComboItem
        {
            public string Text { get; }
            public string Value { get; }

            public ComboItem(string text, string value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString() => Text;
        }
    }
}
/*using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using water3.Services;

namespace water3.Forms
{
    public partial class InvoicePrintPreviewForm : Form
    {
        // ===== Theme =====
        private static readonly Color PrimaryColor = Color.FromArgb(0, 87, 183);
        private static readonly Color PrimaryDark = Color.FromArgb(0, 70, 150);
        private static readonly Color BackgroundColor = Color.FromArgb(245, 247, 250);
        private static readonly Color CardColor = Color.White;
        private static readonly Color BorderColor = Color.FromArgb(225, 230, 235);

        private static readonly Color SuccessColor = Color.FromArgb(40, 167, 69);
        private static readonly Color DangerColor = Color.FromArgb(220, 53, 69);
        private static readonly Color InfoColor = Color.FromArgb(33, 150, 243);

        private static readonly Color TextMutedColor = Color.FromArgb(120, 120, 120);
        private static readonly Color GridHeaderColor = Color.FromArgb(245, 248, 255);
        private static readonly Color GridAlternateRowColor = Color.FromArgb(252, 252, 252);
        private static readonly Color GridSelectionColor = Color.FromArgb(220, 235, 255);

        private static readonly Font SubtitleFont = new Font("Segoe UI", 11, FontStyle.Bold);
        private static readonly Font RegularFont = new Font("Segoe UI", 10);
        private static readonly Font SmallFont = new Font("Segoe UI", 9);
        private static readonly Font GridHeaderFont = new Font("Segoe UI", 10, FontStyle.Bold);

        private Timer _searchDebounceTimer;

        private DataTable smsTable;
        private DataView smsView;

        // ====== Invoices grid columns ======
        private const string COL_SELECT = "Select";
        private const string COL_SMS_ID = "SmsID";
        private const string COL_INVOICE_ID = "رقم الفاتورة";
        private const string COL_SUBSCRIBER_ID = "SubscriberID";
        private const string COL_PHONE = "رقم الهاتف";
        private const string COL_NAME = "اسم المشترك";
        private const string COL_MESSAGE = "نص الرسالة";

        // ====== Payments grid columns ======
        private const string PCOL_SELECT = "PSelect";
        private const string PCOL_SMS_ID = "SmsID";
        private const string PCOL_PAYMENT_ID = "PaymentID";
        private const string PCOL_RECEIPT_ID = "ReceiptID";
        private const string PCOL_SUB_ID = "SubscriberID";
        private const string PCOL_NAME = "اسم المشترك";
        private const string PCOL_PHONE = "رقم الهاتف";
        private const string PCOL_MESSAGE = "نص الرسالة";
        private readonly InvoicePrintPreviewService _pageService;
        // ===== SMS Gateway (Android) =====
        private AndroidSmsGatewayClient _gatewayClient;
        private SmsGatewaySettings _gatewaySettings;

        // UI controls (داخل نفس الفورم بدون Designer)
        private TextBox txtGatewayIp;
        private TextBox txtGatewayKey;
        private Button btnGatewayTest;
        private Button btnGatewaySave;

        // Header select-all checkboxes
        private CheckBox _chkAllInvoices;
        private CheckBox _chkAllPayments;

        public InvoicePrintPreviewForm()
        {
            InitializeComponent();
            _pageService = new InvoicePrintPreviewService(new water3.Repositories.SmsLogsRepository());
            InitializeForm();
            ApplyTheme();

            InitializeSmsGatewayUi();

            SetupFilters();
            SetupGrids();
            WireEvents();

            LoadInitialData();
        }

        private void InitializeForm()
        {
            Text = "إرسال الفواتير والسداد";
            BackColor = BackgroundColor;
            WindowState = FormWindowState.Maximized;
            Font = RegularFont;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
        }

        private void ApplyTheme()
        {
            // Cards look
            cardFilterStats.BackColor = CardColor;
            cardToolbar.BackColor = CardColor;
            cardContent.BackColor = CardColor;

            // Buttons style
            StyleButton(btnSearch, PrimaryColor, Color.White, outline: false);

            StyleButton(btnPrint, PrimaryColor, Color.White, outline: false);
            StyleButton(btnPrintAll, PrimaryDark, Color.White, outline: false);
            StyleButton(btnSMS, InfoColor, Color.White, outline: false);
            StyleButton(btnSMSAll, SuccessColor, Color.White, outline: false);
            StyleButton(btnExport, Color.White, PrimaryColor, outline: true);

            // Inputs
            txtSearch.Font = RegularFont;
            cbSubscriberFilter.Font = RegularFont;
            cbStatusFilter.Font = RegularFont;
            cbPaymentTypeFilter.Font = RegularFont;
            dtFrom.Font = RegularFont;
            dtTo.Font = RegularFont;

            foreach (var lbl in new[] { lblTotalInvoices, lblTotalAmount, lblPaidAmount, lblRemainingAmount })
            {
                lbl.BackColor = CardColor;
                lbl.Font = SmallFont;
                lbl.ForeColor = TextMutedColor;
                lbl.Padding = new Padding(12);
                lbl.Margin = new Padding(6);
                lbl.Paint += (s, e) =>
                {
                    using (var pen = new Pen(BorderColor))
                        e.Graphics.DrawRectangle(pen, 0, 0, lbl.Width - 1, lbl.Height - 1);
                };
            }

            lblTotalInvoices.Paint += (s, e) => e.Graphics.FillRectangle(new SolidBrush(PrimaryColor), 0, 0, lblTotalInvoices.Width, 3);
            lblTotalAmount.Paint += (s, e) => e.Graphics.FillRectangle(new SolidBrush(InfoColor), 0, 0, lblTotalAmount.Width, 3);
            lblPaidAmount.Paint += (s, e) => e.Graphics.FillRectangle(new SolidBrush(SuccessColor), 0, 0, lblPaidAmount.Width, 3);
            lblRemainingAmount.Paint += (s, e) => e.Graphics.FillRectangle(new SolidBrush(DangerColor), 0, 0, lblRemainingAmount.Width, 3);

            // Loading panel styling
            loadingPanel.BackColor = Color.FromArgb(110, 0, 0, 0);
            loadingContainer.BackColor = Color.White;
            lblLoading.Font = SubtitleFont;
            lblLoading.ForeColor = PrimaryColor;

            loadingContainer.Paint += (s, e) =>
            {
                using (var pen = new Pen(BorderColor))
                    e.Graphics.DrawRectangle(pen, 0, 0, loadingContainer.Width - 1, loadingContainer.Height - 1);
            };

            loadingPanel.Resize += (s, e) =>
            {
                loadingContainer.Left = (loadingPanel.Width - loadingContainer.Width) / 2;
                loadingContainer.Top = (loadingPanel.Height - loadingContainer.Height) / 2;
            };
        }

        private void InitializeSmsGatewayUi()
        {
            _gatewaySettings = SmsGatewaySettings.Load();

            _gatewayClient = new AndroidSmsGatewayClient
            {
                PhoneIp = _gatewaySettings.PhoneIp,
                Port = _gatewaySettings.Port,
                ApiKey = _gatewaySettings.ApiKey
            };

            var pnl = new Panel
            {
                Width = 560,
                Height = 36,
                Margin = new Padding(6, 0, 6, 0),
                BackColor = Color.Transparent
            };

            var lblIp = new Label
            {
                AutoSize = true,
                Text = "IP:",
                Location = new Point(540, 9),
                ForeColor = TextMutedColor
            };

            txtGatewayIp = new TextBox
            {
                Width = 130,
                Height = 24,
                Location = new Point(405, 6),
                Text = _gatewaySettings.PhoneIp,
                Font = RegularFont
            };

            var lblKey = new Label
            {
                AutoSize = true,
                Text = "Key:",
                Location = new Point(378, 9),
                ForeColor = TextMutedColor
            };

            txtGatewayKey = new TextBox
            {
                Width = 120,
                Height = 24,
                Location = new Point(250, 6),
                Text = _gatewaySettings.ApiKey,
                Font = RegularFont,
                UseSystemPasswordChar = true
            };

            var lblPort = new Label
            {
                AutoSize = true,
                Text = ":8080",
                Location = new Point(210, 9),
                ForeColor = TextMutedColor
            };

            btnGatewayTest = new Button
            {
                Text = "Test",
                Width = 70,
                Height = 28,
                Location = new Point(135, 4)
            };
            StyleButton(btnGatewayTest, InfoColor, Color.White, outline: false);

            btnGatewaySave = new Button
            {
                Text = "Save",
                Width = 70,
                Height = 28,
                Location = new Point(60, 4)
            };
            StyleButton(btnGatewaySave, SuccessColor, Color.White, outline: false);

            pnl.Controls.Add(lblIp);
            pnl.Controls.Add(txtGatewayIp);
            pnl.Controls.Add(lblKey);
            pnl.Controls.Add(txtGatewayKey);
            pnl.Controls.Add(lblPort);
            pnl.Controls.Add(btnGatewayTest);
            pnl.Controls.Add(btnGatewaySave);

            toolbarFlow.Controls.Add(pnl);
            toolbarFlow.Controls.SetChildIndex(pnl, 0);

            btnGatewaySave.Click += (s, e) =>
            {
                ApplyGatewaySettingsFromUi(save: true);
                MessageBox.Show("تم حفظ إعدادات SMS Gateway ✅", "SMS Gateway",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnGatewayTest.Click += async (s, e) =>
            {
                ApplyGatewaySettingsFromUi(save: false);

                ShowLoading(true, "جاري اختبار الاتصال بالهاتف...");
                var (ok, raw) = await _gatewayClient.HealthAsync();
                ShowLoading(false);

                MessageBox.Show(ok ? $"✅ الاتصال ناجح\n{raw}" : $"❌ فشل الاتصال\n{raw}",
                    "SMS Gateway Test",
                    MessageBoxButtons.OK,
                    ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            };
        }

        private void ApplyGatewaySettingsFromUi(bool save)
        {
            var ip = (txtGatewayIp.Text ?? "").Trim();
            var key = (txtGatewayKey.Text ?? "").Trim();

            if (string.IsNullOrWhiteSpace(ip))
                ip = "192.168.1.20";

            if (string.IsNullOrWhiteSpace(key))
                key = "123456";

            _gatewaySettings.PhoneIp = ip;
            _gatewaySettings.ApiKey = key;
            _gatewaySettings.Port = 8080;

            _gatewayClient.PhoneIp = _gatewaySettings.PhoneIp;
            _gatewayClient.ApiKey = _gatewaySettings.ApiKey;
            _gatewayClient.Port = _gatewaySettings.Port;

            if (save) _gatewaySettings.Save();
        }

        private void StyleButton(Button btn, Color backColor, Color foreColor, bool outline)
        {
            btn.Font = RegularFont;
            btn.Cursor = Cursors.Hand;
            btn.FlatStyle = FlatStyle.Flat;

            btn.BackColor = outline ? Color.White : backColor;
            btn.ForeColor = outline ? PrimaryColor : foreColor;

            btn.FlatAppearance.BorderSize = outline ? 1 : 0;
            btn.FlatAppearance.BorderColor = PrimaryColor;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.12f);
            btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.2f);
        }

        private void SetupFilters()
        {
            cbStatusFilter.Items.Clear();
            cbStatusFilter.Items.AddRange(new object[] { "جميع الحالات", "مدفوعة", "غير مدفوعة", "جزئية" });
            cbStatusFilter.SelectedIndex = 0;

            cbPaymentTypeFilter.Items.Clear();
            cbPaymentTypeFilter.Items.AddRange(new object[] { "كل طرق السداد", "نقداً", "تحويل", "شيك", "أخرى" });
            cbPaymentTypeFilter.SelectedIndex = 0;

            dtFrom.Value = DateTime.Today.AddMonths(-1);
            dtTo.Value = DateTime.Today;

            ToggleFiltersByTab();
        }
        private string MapPaymentTypeFilterToDb()
        {
            switch ((cbPaymentTypeFilter.Text ?? "").Trim())
            {
                case "نقداً": return "Cash";
                case "تحويل": return "Transfer";
                case "شيك": return "Cheque";
                case "أخرى": return "Other";
                default: return null;
            }
        }

        private void ToggleFiltersByTab()
        {
            // إذا فاتورة: أظهر status، أخفِ paymentType
            // إذا سداد: أخفِ status، أظهر paymentType
            bool isInvoices = tabControl.SelectedTab == tabInvoices;

            cbStatusFilter.Visible = isInvoices;
            lblStatus.Visible = isInvoices;

            cbPaymentTypeFilter.Visible = !isInvoices;
            lblPaymentType.Visible = !isInvoices;
        }

        private void SetupGrids()
        {
            SetupGridStyle(dgvInvoices, selectable: true, selectColumnName: COL_SELECT);
            SetupGridStyle(dgvPayments, selectable: true, selectColumnName: PCOL_SELECT);
            SetupGridStyle(dgvSmsLogs, selectable: false, selectColumnName: null);

            dgvSmsLogs.ReadOnly = true;
            dgvSmsLogs.MultiSelect = false;
            dgvSmsLogs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            _chkAllInvoices = InstallSelectAllHeader(dgvInvoices, COL_SELECT, _chkAllInvoices);
            _chkAllPayments = InstallSelectAllHeader(dgvPayments, PCOL_SELECT, _chkAllPayments);
        }

        private void SetupGridStyle(DataGridView grid, bool selectable, string selectColumnName)
        {
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = CardColor;
            grid.BorderStyle = BorderStyle.None;
            grid.RowHeadersVisible = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = selectable;
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersHeight = 40;
            grid.RowTemplate.Height = 34;
            grid.GridColor = BorderColor;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = GridHeaderColor,
                ForeColor = Color.FromArgb(60, 60, 60),
                Font = GridHeaderFont,
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(6, 0, 6, 0)
            };

            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = RegularFont,
                ForeColor = Color.FromArgb(60, 60, 60),
                SelectionBackColor = GridSelectionColor,
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(6, 0, 6, 0)
            };

            grid.AlternatingRowsDefaultCellStyle.BackColor = GridAlternateRowColor;

            if (selectable && !string.IsNullOrWhiteSpace(selectColumnName) && !grid.Columns.Contains(selectColumnName))
            {
                var check = new DataGridViewCheckBoxColumn
                {
                    Name = selectColumnName,
                    HeaderText = "تحديد",
                    Width = 70,
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.None     
                };
                grid.Columns.Add(check);

                grid.CellClick += (s, e) =>
                {
                    if (e.RowIndex >= 0 && e.ColumnIndex == grid.Columns[selectColumnName].Index)
                        grid.EndEdit();
                };
            }
        }

        private CheckBox InstallSelectAllHeader(DataGridView grid, string selectColumnName, CheckBox existing)
        {
            // لو موجود مسبقاً لا تعيد إنشائه
            if (existing != null) return existing;
            if (!grid.Columns.Contains(selectColumnName)) return null;

            var headerCheck = new CheckBox
            {
                Size = new Size(16, 16),
                BackColor = Color.Transparent
            };

            grid.Controls.Add(headerCheck);

            Action reposition = () =>
            {
                try
                {
                    var rect = grid.GetCellDisplayRectangle(grid.Columns[selectColumnName].Index, -1, true);
                    headerCheck.Location = new Point(
                        rect.Left + (rect.Width - headerCheck.Width) / 2,
                        rect.Top + (rect.Height - headerCheck.Height) / 2
                    );
                }
                catch { }
            };

            // أحداث إعادة التموضع
            grid.ColumnWidthChanged += (s, e) => reposition();
            grid.Scroll += (s, e) => reposition();
            grid.SizeChanged += (s, e) => reposition();
            grid.DataBindingComplete += (s, e) => reposition();

            // تغيير التحديد
            headerCheck.CheckedChanged += (s, e) =>
            {
                grid.EndEdit();
                bool check = headerCheck.Checked;

                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow) continue;
                    var cell = row.Cells[selectColumnName];
                    if (cell != null) cell.Value = check;
                }
            };

            // أول تموضع
            reposition();

            return headerCheck;
        }
        private void WireEvents()
        {
            btnSearch.Click += async (s, e) =>
            {
                await ReloadCurrentTabAsync();
                await LoadSmsLogsAsync();
            };

            btnPrint.Click += (s, e) =>
            {
                if (tabControl.SelectedTab != tabInvoices)
                {
                    MessageBox.Show("الطباعة متاحة للفواتير فقط.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                PrintSelectedInvoices();
            };

            btnPrintAll.Click += (s, e) =>
            {
                if (tabControl.SelectedTab != tabInvoices)
                {
                    MessageBox.Show("الطباعة متاحة للفواتير فقط.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                PrintAllInvoices();
            };

            btnSMS.Click += async (s, e) => await SendSmsSelectedByTabAsync();
            btnSMSAll.Click += async (s, e) => await SendSmsAllByTabAsync();

            btnExport.Click += (s, e) => ExportCurrentGridToCsv();

            _searchDebounceTimer = new Timer { Interval = 350 };
            _searchDebounceTimer.Tick += async (s, e) =>
            {
                _searchDebounceTimer.Stop();
                await ReloadCurrentTabAsync();
                await LoadSmsLogsAsync();
            };

            txtSearch.TextChanged += (s, e) =>
            {
                _searchDebounceTimer.Stop();
                _searchDebounceTimer.Start();
            };

            cbSubscriberFilter.SelectedIndexChanged += async (s, e) =>
            {
                await ReloadCurrentTabAsync();
                await LoadSmsLogsAsync();
            };

            cbStatusFilter.SelectedIndexChanged += async (s, e) =>
            {
                if (tabControl.SelectedTab == tabInvoices)
                {
                    await LoadInvoicesAsync();
                    await LoadSmsLogsAsync();
                }
            };

            cbPaymentTypeFilter.SelectedIndexChanged += async (s, e) =>
            {
                if (tabControl.SelectedTab == tabPayments)
                {
                    await LoadPaymentsAsync();
                    await LoadSmsLogsAsync();
                }
            };

            dtFrom.ValueChanged += async (s, e) => { await ReloadCurrentTabAsync(); await LoadSmsLogsAsync(); };
            dtTo.ValueChanged += async (s, e) => { await ReloadCurrentTabAsync(); await LoadSmsLogsAsync(); };

            tabControl.SelectedIndexChanged += async (s, e) =>
            {
                ToggleFiltersByTab();
                await ReloadCurrentTabAsync();
            };
        }

        private async Task ReloadCurrentTabAsync()
        {
            if (tabControl.SelectedTab == tabInvoices)
                await LoadInvoicesAsync();
            else if (tabControl.SelectedTab == tabPayments)
                await LoadPaymentsAsync();
        }

        private async void LoadInitialData()
        {
            ShowLoading(true, "جاري تحميل البيانات...");
            await LoadSubscribersAsync();
            await ReloadCurrentTabAsync();
            await LoadSmsLogsAsync();
            ShowLoading(false);
        }

        // ==========================
        // Subscribers
        // ==========================
        private async Task LoadSubscribersAsync()
        {
            try
            {
                using (var con = Db.GetConnection())
                {
                    await con.OpenAsync();

                    using (var cmd = new SqlCommand(
                        "SELECT SubscriberID, Name FROM Subscribers WHERE IsActive = 1 ORDER BY Name", con))
                    using (var r = await cmd.ExecuteReaderAsync())
                    {
                        cbSubscriberFilter.Items.Clear();
                        cbSubscriberFilter.Items.Add(new ComboItem("جميع المشتركين", "0"));

                        while (await r.ReadAsync())
                        {
                            cbSubscriberFilter.Items.Add(new ComboItem(
                                r["Name"].ToString(),
                                r["SubscriberID"].ToString()
                            ));
                        }

                        cbSubscriberFilter.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل المشتركين: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================
        // Invoices
        // ==========================
        private async Task LoadInvoicesAsync()
        {
            try
            {
                ShowLoading(true, "جاري تحميل الفواتير غير المرسلة...");

                if (dtFrom.Value.Date > dtTo.Value.Date)
                {
                    MessageBox.Show("تاريخ (من) يجب أن يكون أقل أو يساوي (إلى).");
                    return;
                }

                string search = (txtSearch.Text ?? "").Trim();
                int sid = GetSelectedSubscriberId();
                string invoiceStatus = cbStatusFilter.SelectedIndex <= 0 ? null : cbStatusFilter.Text;

                var dt = await Task.Run(() =>
                    _pageService.GetPendingInvoiceMessages(
                        dtFrom.Value.Date,
                        dtTo.Value.Date.AddDays(1),
                        sid,
                        string.IsNullOrWhiteSpace(search) ? null : $"%{search}%",
                        invoiceStatus
                    ));

                BindInvoicesToGrid(dt);
                UpdateStatisticsFromInvoiceTable(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الفواتير غير المرسلة: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowLoading(false);
            }
        }

        private int GetSelectedSubscriberId()
        {
            int sid = 0;
            if (cbSubscriberFilter.SelectedItem is ComboItem it && int.TryParse(it.Value, out int parsed))
                sid = parsed;
            return sid;
        }

        private void BindInvoicesToGrid(DataTable dt)
        {
            dgvInvoices.SuspendLayout();

            for (int i = dgvInvoices.Columns.Count - 1; i >= 0; i--)
                if (dgvInvoices.Columns[i].Name != COL_SELECT)
                    dgvInvoices.Columns.RemoveAt(i);

            dgvInvoices.DataSource = null;
            dgvInvoices.AutoGenerateColumns = true;
            dgvInvoices.DataSource = dt;

            if (dgvInvoices.Columns.Contains(COL_SELECT))
            {
                dgvInvoices.Columns[COL_SELECT].DisplayIndex = 0;
                dgvInvoices.Columns[COL_SELECT].ReadOnly = false;
                dgvInvoices.Columns[COL_SELECT].HeaderText = "تحديد";
                dgvInvoices.Columns[COL_SELECT].Width = 60;
            }

            if (dgvInvoices.Columns.Contains(COL_SMS_ID)) dgvInvoices.Columns[COL_SMS_ID].Visible = false;
            if (dgvInvoices.Columns.Contains("TemplateID")) dgvInvoices.Columns["TemplateID"].Visible = false;
            if (dgvInvoices.Columns.Contains(COL_SUBSCRIBER_ID)) dgvInvoices.Columns[COL_SUBSCRIBER_ID].Visible = false;

            foreach (DataGridViewColumn col in dgvInvoices.Columns)
            {
                if (col.Name == COL_SELECT) continue;
                col.ReadOnly = true;
            }

            dgvInvoices.ResumeLayout();

            _chkAllInvoices?.BringToFront();
            if (_chkAllInvoices != null) _chkAllInvoices.Checked = false;
        }



        private void UpdateStatisticsFromInvoiceTable(DataTable dt)
        {
            try
            {
                int totalInvoices = dt.Rows.Count;
                decimal totalAmount = 0m;
                decimal paidAmount = 0m;
                decimal remainingAmount = 0m;

                foreach (DataRow row in dt.Rows)
                {
                    totalAmount += SafeDec(row["المبلغ الإجمالي"]);
                    paidAmount += SafeDec(row["المدفوع"]);
                    remainingAmount += SafeDec(row["المتبقي"]);
                }

                lblTotalInvoices.Text = $"📄 إجمالي الفواتير\n{totalInvoices:N0}";
                lblTotalAmount.Text = $"💰 إجمالي المبالغ\n{totalAmount:N2}";
                lblPaidAmount.Text = $"💳 المدفوع\n{paidAmount:N2}";
                lblRemainingAmount.Text = $"⚠️ المتبقي\n{remainingAmount:N2}";
            }
            catch { }
        }

        // ==========================
        // Payments (سداد)
        // ==========================
        private async Task LoadPaymentsAsync()
        {
            try
            {
                ShowLoading(true, "جاري تحميل السداد غير المرسل...");

                if (dtFrom.Value.Date > dtTo.Value.Date)
                {
                    MessageBox.Show("تاريخ (من) يجب أن يكون أقل أو يساوي (إلى).");
                    return;
                }

                string search = (txtSearch.Text ?? "").Trim();
                int sid = GetSelectedSubscriberId();
                string paymentMethodDb = MapPaymentTypeFilterToDb();

                var dt = await Task.Run(() =>
                    _pageService.GetPendingPaymentMessages(
                        dtFrom.Value.Date,
                        dtTo.Value.Date.AddDays(1),
                        sid,
                        string.IsNullOrWhiteSpace(search) ? null : $"%{search}%",
                        paymentMethodDb
                    ));

                BindPaymentsToGrid(dt);
                UpdateStatisticsFromPaymentsTable(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل السداد غير المرسل: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ShowLoading(false);
            }
        }



      private void BindPaymentsToGrid(DataTable dt)
{
    dgvPayments.SuspendLayout();

    for (int i = dgvPayments.Columns.Count - 1; i >= 0; i--)
        if (dgvPayments.Columns[i].Name != PCOL_SELECT)
            dgvPayments.Columns.RemoveAt(i);

    dgvPayments.DataSource = null;
    dgvPayments.AutoGenerateColumns = true;
    dgvPayments.DataSource = dt;

    if (dgvPayments.Columns.Contains(PCOL_SELECT))
    {
        dgvPayments.Columns[PCOL_SELECT].DisplayIndex = 0;
        dgvPayments.Columns[PCOL_SELECT].ReadOnly = false;
        dgvPayments.Columns[PCOL_SELECT].HeaderText = "تحديد";
        dgvPayments.Columns[PCOL_SELECT].Width = 60;
    }

    if (dgvPayments.Columns.Contains(PCOL_SMS_ID)) dgvPayments.Columns[PCOL_SMS_ID].Visible = false;
    if (dgvPayments.Columns.Contains("TemplateID")) dgvPayments.Columns["TemplateID"].Visible = false;
    if (dgvPayments.Columns.Contains(PCOL_PAYMENT_ID)) dgvPayments.Columns[PCOL_PAYMENT_ID].Visible = false;
    if (dgvPayments.Columns.Contains(PCOL_RECEIPT_ID)) dgvPayments.Columns[PCOL_RECEIPT_ID].Visible = false;
    if (dgvPayments.Columns.Contains(PCOL_SUB_ID)) dgvPayments.Columns[PCOL_SUB_ID].Visible = false;

    foreach (DataGridViewColumn col in dgvPayments.Columns)
    {
        if (col.Name == PCOL_SELECT) continue;
        col.ReadOnly = true;
    }

    dgvPayments.ResumeLayout();

    _chkAllPayments?.BringToFront();
    if (_chkAllPayments != null) _chkAllPayments.Checked = false;
}
      private void UpdateStatisticsFromPaymentsTable(DataTable dt)
        {
            try
            {
                int total = dt.Rows.Count;
                decimal totalReceipts = 0m;
                decimal totalPaidToInvoices = 0m;
                decimal totalAdvance = 0m;

                foreach (DataRow row in dt.Rows)
                {
                    totalReceipts += SafeDec(row["إجمالي الإيصال"]);
                    totalPaidToInvoices += SafeDec(row["مسدد على الفواتير"]);
                    totalAdvance += SafeDec(row["رصيد مقدم"]);
                }

                lblTotalInvoices.Text = $"🧾 رسائل سداد غير مرسلة\n{total:N0}";
                lblTotalAmount.Text = $"💰 إجمالي الإيصالات\n{totalReceipts:N2}";
                lblPaidAmount.Text = $"💳 مسدد على الفواتير\n{totalPaidToInvoices:N2}";
                lblRemainingAmount.Text = $"🟢 رصيد مقدم\n{totalAdvance:N2}";
            }
            catch { }
        }

        // ==========================
        // SMS Logs
        // ==========================
        private async Task LoadSmsLogsAsync()
        {
            try
            {
                string search = (txtSearch.Text ?? "").Trim();
                int sid = GetSelectedSubscriberId();

                smsTable = await Task.Run(() =>
                    _pageService.GetSmsLogs(
                        dtFrom.Value.Date,
                        dtTo.Value.Date.AddDays(1),
                        sid,
                        string.IsNullOrWhiteSpace(search) ? null : $"%{search}%"
                    ));

                smsView = smsTable.DefaultView;
                dgvSmsLogs.DataSource = smsView;

                if (dgvSmsLogs.Columns.Contains("SmsID")) dgvSmsLogs.Columns["SmsID"].HeaderText = "رقم الرسالة";
                if (dgvSmsLogs.Columns.Contains("MessageType")) dgvSmsLogs.Columns["MessageType"].HeaderText = "نوع الرسالة";
                if (dgvSmsLogs.Columns.Contains("InvoiceID")) dgvSmsLogs.Columns["InvoiceID"].HeaderText = "رقم الفاتورة";
                if (dgvSmsLogs.Columns.Contains("PaymentID")) dgvSmsLogs.Columns["PaymentID"].HeaderText = "رقم السداد";
                if (dgvSmsLogs.Columns.Contains("ReceiptID")) dgvSmsLogs.Columns["ReceiptID"].HeaderText = "رقم الإيصال";
                if (dgvSmsLogs.Columns.Contains("SubscriberName")) dgvSmsLogs.Columns["SubscriberName"].HeaderText = "اسم المشترك";
                if (dgvSmsLogs.Columns.Contains("PhoneNumber")) dgvSmsLogs.Columns["PhoneNumber"].HeaderText = "رقم الهاتف";
                if (dgvSmsLogs.Columns.Contains("Message")) dgvSmsLogs.Columns["Message"].HeaderText = "نص الرسالة";
                if (dgvSmsLogs.Columns.Contains("Status")) dgvSmsLogs.Columns["Status"].HeaderText = "الحالة";
                if (dgvSmsLogs.Columns.Contains("Reason")) dgvSmsLogs.Columns["Reason"].HeaderText = "السبب";
                if (dgvSmsLogs.Columns.Contains("CreatedAt")) dgvSmsLogs.Columns["CreatedAt"].HeaderText = "تاريخ الإنشاء";
                if (dgvSmsLogs.Columns.Contains("SentDate")) dgvSmsLogs.Columns["SentDate"].HeaderText = "تاريخ الإرسال";

                if (dgvSmsLogs.Columns.Contains("Message"))
                {
                    dgvSmsLogs.Columns["Message"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                    dgvSmsLogs.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل سجل الرسائل: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void PrintSelectedInvoices()
        {
            var ids = GetSelectedInvoiceIds();
            if (ids.Length == 0)
            {
                MessageBox.Show("يرجى تحديد فواتير للطباعة", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show("تم إلغاء تبويب المعاينة. اربط الطباعة بـ ReportViewer/PrintDocument حسب مشروعك.", "ملاحظة",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PrintAllInvoices()
        {
            MessageBox.Show("تم إلغاء تبويب المعاينة. اربط الطباعة بـ ReportViewer/PrintDocument حسب مشروعك.", "ملاحظة",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private string[] GetSelectedInvoiceIds()
        {
            return dgvInvoices.Rows.Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Where(r => r.Cells[COL_SELECT] != null && Convert.ToBoolean(r.Cells[COL_SELECT].Value) == true)
                .Select(r => r.Cells[COL_INVOICE_ID]?.Value?.ToString())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToArray();
        }

        // ==========================
        // SMS Send (Invoices + Payments حسب التبويب)
        // ==========================
        private async Task SendSmsSelectedByTabAsync()
        {
            if (tabControl.SelectedTab == tabInvoices)
                await SendInvoiceSmsSelectedAsync();
            else if (tabControl.SelectedTab == tabPayments)
                await SendPaymentSmsSelectedAsync();

            await LoadSmsLogsAsync();
        }

        private async Task SendSmsAllByTabAsync()
        {
            if (tabControl.SelectedTab == tabInvoices)
                await SendInvoiceSmsAllAsync();
            else if (tabControl.SelectedTab == tabPayments)
                await SendPaymentSmsAllAsync();

            await LoadSmsLogsAsync();
        }

        private async Task SendInvoiceSmsSelectedAsync()
        {
            var selected = dgvInvoices.Rows.Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Where(r => r.Cells[COL_SELECT] != null && Convert.ToBoolean(r.Cells[COL_SELECT].Value) == true)
                .ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show("يرجى تحديد رسائل فواتير للإرسال", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await SendSmsRowsAsync(selected, COL_SMS_ID, COL_PHONE, COL_MESSAGE, "الفواتير المختارة");
        }

        private async Task SendInvoiceSmsAllAsync()
        {
            var result = MessageBox.Show("هل تريد إرسال جميع رسائل الفواتير غير المرسلة المعروضة؟", "تأكيد الإرسال",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            var allRows = dgvInvoices.Rows.Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .ToList();

            await SendSmsRowsAsync(allRows, COL_SMS_ID, COL_PHONE, COL_MESSAGE, "جميع الفواتير");
        }

        private async Task SendPaymentSmsSelectedAsync()
        {
            var selected = dgvPayments.Rows.Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .Where(r => r.Cells[PCOL_SELECT] != null && Convert.ToBoolean(r.Cells[PCOL_SELECT].Value) == true)
                .ToList();

            if (selected.Count == 0)
            {
                MessageBox.Show("يرجى تحديد رسائل سداد للإرسال", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await SendSmsRowsAsync(selected, PCOL_SMS_ID, PCOL_PHONE, PCOL_MESSAGE, "السداد المختار");
        }

        private async Task SendPaymentSmsAllAsync()
        {
            var result = MessageBox.Show("هل تريد إرسال جميع رسائل السداد غير المرسلة المعروضة؟", "تأكيد الإرسال",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            var allRows = dgvPayments.Rows.Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .ToList();

            await SendSmsRowsAsync(allRows, PCOL_SMS_ID, PCOL_PHONE, PCOL_MESSAGE, "جميع السداد");
        }

        private async Task<(bool ok, string raw)> EnsureGatewayOnlineAsync()
        {
            ApplyGatewaySettingsFromUi(save: false);
            var (hOk, hRaw) = await _gatewayClient.HealthAsync();
            return (hOk, hRaw);
        }
  
        private async Task SendSmsRowsAsync(
       List<DataGridViewRow> rows,
       string smsIdColumn,
       string phoneColumn,
       string messageColumn,
       string description)
        {
            var (hOk, hRaw) = await EnsureGatewayOnlineAsync();
            if (!hOk)
            {
                MessageBox.Show("الهاتف غير متصل أو الخدمة مطفأة.\n\n" + hRaw,
                    "SMS Gateway", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ShowLoading(true, $"جاري إرسال الرسائل ({description})...");

            int successCount = 0;
            int failCount = 0;

            foreach (var row in rows)
            {
                try
                {
                    int smsId = Convert.ToInt32(row.Cells["SmsID"].Value);
                    string phone = row.Cells[phoneColumn]?.Value?.ToString();
                    string message = row.Cells[messageColumn]?.Value?.ToString();

                    if (string.IsNullOrWhiteSpace(phone))
                    {
                        _pageService.MarkFailed(smsId, "رقم هاتف غير صالح");
                        failCount++;
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(message))
                    {
                        _pageService.MarkFailed(smsId, "نص الرسالة فارغ");
                        failCount++;
                        continue;
                    }

                    var (ok, status, raw, outId) = await _gatewayClient.SendSmsAsync(phone, message);

                    if (ok)
                    {
                        _pageService.MarkSent(smsId);
                        successCount++;
                    }
                    else
                    {
                        _pageService.MarkFailed(smsId, raw);
                        failCount++;
                    }

                    await Task.Delay(200);
                }
                catch (Exception ex)
                {
                    failCount++;
                }
            }

            ShowLoading(false);

            await ReloadCurrentTabAsync();
            await LoadSmsLogsAsync();

            MessageBox.Show($"تم إرسال {successCount} رسالة بنجاح، وفشل {failCount} رسالة",
                "نتيجة الإرسال", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ==========================
        private void ExportCurrentGridToCsv()
        {
            DataGridView grid = tabControl.SelectedTab == tabPayments ? dgvPayments : dgvInvoices;

            if (grid.DataSource == null || grid.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للتصدير.");
                return;
            }

            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel CSV (*.csv)|*.csv";
                sfd.FileName = (tabControl.SelectedTab == tabPayments)
                    ? $"Payments_{DateTime.Now:yyyyMMdd_HHmm}.csv"
                    : $"Invoices_{DateTime.Now:yyyyMMdd_HHmm}.csv";

                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var sb = new StringBuilder();

                    // skip Select columns
                    var cols = grid.Columns.Cast<DataGridViewColumn>()
                        .Where(c => c.Visible && c.Name != COL_SELECT && c.Name != PCOL_SELECT)
                        .OrderBy(c => c.DisplayIndex)
                        .ToList();

                    sb.AppendLine(string.Join(",", cols.Select(c => EscapeCsv(c.HeaderText))));

                    foreach (DataGridViewRow row in grid.Rows)
                    {
                        if (row.IsNewRow) continue;

                        var line = cols.Select(c =>
                        {
                            var v = row.Cells[c.Name]?.Value?.ToString() ?? "";
                            return EscapeCsv(v);
                        });

                        sb.AppendLine(string.Join(",", line));
                    }

                    System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("تم التصدير بنجاح ✅");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("فشل التصدير: " + ex.Message);
                }
            }
        }

        private string EscapeCsv(string s)
        {
            if (s == null) return "";
            if (s.Contains(",") || s.Contains("\"") || s.Contains("\n"))
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        private decimal SafeDec(object o)
        {
            if (o == null || o == DBNull.Value) return 0m;
            decimal.TryParse(o.ToString(), out decimal x);
            return x;
        }

        private void ShowLoading(bool show, string message = "")
        {
            loadingPanel.Visible = show;
            if (!string.IsNullOrEmpty(message)) lblLoading.Text = message;
            loadingPanel.BringToFront();

            btnSearch.Enabled = !show;
            btnPrint.Enabled = !show;
            btnPrintAll.Enabled = !show;
            btnSMS.Enabled = !show;
            btnSMSAll.Enabled = !show;
            btnExport.Enabled = !show;

            dgvInvoices.Enabled = !show;
            dgvPayments.Enabled = !show;
            dgvSmsLogs.Enabled = !show;
        }

        // same ComboItem
        private class ComboItem
        {
            public string Text { get; }
            public string Value { get; }
            public ComboItem(string text, string value) { Text = text; Value = value; }
            public override string ToString() => Text;
        }
    }
}

*/