using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using water3.Services;

namespace water3
{
    public partial class DashboardForm : Form
    {
        private readonly DashboardService _svc = new DashboardService();

        // ===== Theme =====
        private static readonly Color Primary = Color.FromArgb(0, 87, 183);
        private static readonly Color PrimaryDark = Color.FromArgb(0, 70, 150);
        private static readonly Color Bg = Color.FromArgb(245, 247, 250);
        private static readonly Color Card = Color.White;
        private static readonly Color Border = Color.FromArgb(225, 230, 235);
        private static readonly Color Muted = Color.FromArgb(120, 120, 120);
        private static readonly Color Hover = Color.FromArgb(15, 105, 205);

        private static readonly Color GridHeaderColor = Color.FromArgb(245, 248, 255);
        private static readonly Color GridAlternate = Color.FromArgb(252, 252, 252);
        private static readonly Color GridSelection = Color.FromArgb(220, 235, 255);

        private static readonly Font TitleFont = new Font("Segoe UI", 12.5f, FontStyle.Bold);
        private static readonly Font RegularFont = new Font("Segoe UI", 10f);
        private static readonly Font HeaderFont = new Font("Segoe UI", 10f, FontStyle.Bold);

        public DashboardForm()
        {
            InitializeComponent();

            BackColor = Bg;
            Font = RegularFont;

            ApplyTheme();
            InitPresetItems();
        }

        private void ApplyTheme()
        {
            // form
            this.BackColor = Bg;
            
            // cards
            filterCard.BackColor = Card;
            tabsCard.BackColor = Card;

            // period bar
            lblPeriod.BackColor = Card;
            lblPeriod.ForeColor = Muted;
            lblPeriod.Font = new Font("Segoe UI", 9.8f, FontStyle.Regular);

            // kpi
            kpiPanel.BackColor = Bg;
            kpiPanel.WrapContents = true;
            kpiPanel.FlowDirection = FlowDirection.RightToLeft;
            kpiPanel.AutoScroll = true;

            // labels
            lblFrom.ForeColor = Color.FromArgb(60, 60, 60);
            lblTo.ForeColor = Color.FromArgb(60, 60, 60);
            lblPeriodText.ForeColor = Color.FromArgb(60, 60, 60);
            lblPeriodText.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);

            // icon buttons
            ConfigureIconButton(btnRefresh, "تحديث البيانات");
            ConfigureIconButton(btnQuickReading, "قراءة سريعة");
            ConfigureIconButton(btnQuickPay, "سداد سريع");
            ConfigureIconButton(btnStatement, "كشف حساب");

            // header labels
            lblHdrInvoices.Font = TitleFont;
            lblHdrInvoices.ForeColor = Primary;
            lblHdrPayments.Font = TitleFont;
            lblHdrPayments.ForeColor = Primary;
            lblHdrOutstanding.Font = TitleFont;
            lblHdrOutstanding.ForeColor = Primary;

            // all buttons
            ConfigureActionButton(btnAllInvoices);
            ConfigureActionButton(btnAllPayments);
            ConfigureActionButton(btnAllOutstanding);

            // grids
            ConfigureGrid(gridInvoices);
            ConfigureGrid(gridPayments);
            ConfigureGrid(gridOutstanding);
        }

        private void ConfigureIconButton(Button btn, string tooltip)
        {
            btn.BackColor = Primary;
            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI Emoji", 12f, FontStyle.Regular);
            btn.Cursor = Cursors.Hand;

            toolTip1.SetToolTip(btn, tooltip);

            btn.MouseEnter += IconBtn_MouseEnter;
            btn.MouseLeave += IconBtn_MouseLeave;
            btn.MouseDown += IconBtn_MouseDown;
            btn.MouseUp += IconBtn_MouseUp;
        }

        private void IconBtn_MouseEnter(object sender, EventArgs e)
        {
            var b = sender as Button;
            if (b == null) return;
            b.BackColor = Hover;
            b.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
        }

        private void IconBtn_MouseLeave(object sender, EventArgs e)
        {
            var b = sender as Button;
            if (b == null) return;
            b.BackColor = Primary;
            b.FlatAppearance.BorderColor = Color.White;
        }

        private void IconBtn_MouseDown(object sender, MouseEventArgs e)
        {
            var b = sender as Button;
            if (b == null) return;
            b.BackColor = ControlPaint.Dark(Primary, 0.2f);
        }

        private void IconBtn_MouseUp(object sender, MouseEventArgs e)
        {
            var b = sender as Button;
            if (b == null) return;
            b.BackColor = Primary;
        }

        private void ConfigureActionButton(Button btn)
        {
            btn.BackColor = Primary;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Cursor = Cursors.Hand;
            btn.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);

            btn.MouseEnter += (s, e) => btn.BackColor = Hover;
            btn.MouseLeave += (s, e) => btn.BackColor = Primary;
        }

        private void ConfigureGrid(DataGridView dgv)
        {
            dgv.AutoGenerateColumns = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Card;
            dgv.BorderStyle = BorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 36;  // Reduce height
            dgv.RowTemplate.Height = 32;  // Reduce row height
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = Border;
            dgv.RowHeadersVisible = false;  // Hide row headers
            dgv.AllowUserToResizeRows = false;  // Prevent resizing rows
        }
        private void kpiPanel_Paint(object sender, PaintEventArgs e)
        {
            var panel = sender as Panel;
            if (panel != null)
            {
                using (var pen = new Pen(Color.FromArgb(240, 240, 240), 1))
                    e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);

                using (var shadowBrush = new SolidBrush(Color.FromArgb(5, 0, 0, 0)))
                    e.Graphics.FillRectangle(shadowBrush, 2, 2, panel.Width - 2, panel.Height - 2);
            }
        }
 
        private void InitPresetItems()
        {
            cbPeriodPreset.Items.Clear();
            cbPeriodPreset.Items.AddRange(new object[]
            {
                "اليوم", "أمس", "الأسبوع الحالي", "الشهر الحالي",
                "الشهر الماضي", "الربع الحالي", "السنة الحالية", "فترة مخصصة"
            });
            cbPeriodPreset.SelectedIndex = 3;

            // افتراضي
            ApplyPeriodPreset();
        }

        private void DashboardForm_Load(object sender, EventArgs e)
        {
            autoRefreshTimer.Start();
            _ = RefreshAllAsync();
        }

        private void DashboardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            autoRefreshTimer.Stop();
        }

        private void cbPeriodPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyPeriodPreset();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            _ = RefreshAllAsync();
        }

        private void btnQuickReading_Click(object sender, EventArgs e) => OpenQuickReadingForm();
        private void btnQuickPay_Click(object sender, EventArgs e) => OpenQuickPaymentForm();
        private void btnStatement_Click(object sender, EventArgs e) => OpenStatementForm();

        private void btnAllInvoices_Click(object sender, EventArgs e) => ShowAllRecords("آخر الفواتير");
        private void btnAllPayments_Click(object sender, EventArgs e) => ShowAllRecords("آخر المدفوعات");
        private void btnAllOutstanding_Click(object sender, EventArgs e) => ShowAllRecords("أعلى المتأخرات");

        private void autoRefreshTimer_Tick(object sender, EventArgs e)
        {
            _ = RefreshAllAsync();
        }

        // ===== paints =====
        private void card_Paint(object sender, PaintEventArgs e)
        {
            var p = sender as Panel;
            if (p == null) return;

            // ظل خفيف
            using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                e.Graphics.FillRectangle(shadowBrush, 2, 2, p.Width - 2, p.Height - 2);

            using (var pen = new Pen(Border))
                e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
        }

        private void inputWrap_Paint(object sender, PaintEventArgs e)
        {
            var p = sender as Panel;
            if (p == null) return;

            // حدود بسيطة
            using (var pen = new Pen(Border))
                e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
        }

        private void lblPeriod_Paint(object sender, PaintEventArgs e)
        {
            using (var pen = new Pen(Border))
                e.Graphics.DrawRectangle(pen, 0, 0, lblPeriod.Width - 1, lblPeriod.Height - 1);
        }

        // ===== tabs draw =====
        private void tabs_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabPage = tabs.TabPages[e.Index];
            var rect = tabs.GetTabRect(e.Index);
            bool selected = (tabs.SelectedIndex == e.Index);

            using (var bg = new SolidBrush(selected ? Primary : Color.FromArgb(245, 245, 245)))
                e.Graphics.FillRectangle(bg, rect);

            using (var pen = new Pen(selected ? Primary : Border))
                e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

            TextRenderer.DrawText(
                e.Graphics,
                tabPage.Text,
                HeaderFont,
                rect,
                selected ? Color.White : Color.FromArgb(60, 60, 60),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft
            );
        }

        // ===== grid dblclick =====
        private void gridInvoices_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) OpenDetailsForm(gridInvoices, e.RowIndex);
        }
        private void gridPayments_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) OpenDetailsForm(gridPayments, e.RowIndex);
        }
        private void gridOutstanding_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) OpenDetailsForm(gridOutstanding, e.RowIndex);
        }

        // ===== period preset =====
        private void ApplyPeriodPreset()
        {
            var now = DateTime.Now;

            switch (cbPeriodPreset.SelectedIndex)
            {
                case 0: // اليوم
                    dtFrom.Value = now.Date;
                    dtTo.Value = now.Date;
                    break;
                case 1: // أمس
                    dtFrom.Value = now.AddDays(-1).Date;
                    dtTo.Value = now.AddDays(-1).Date;
                    break;
                case 2: // الأسبوع الحالي
                    dtFrom.Value = now.AddDays(-(int)now.DayOfWeek).Date;
                    dtTo.Value = now.Date;
                    break;
                case 3: // الشهر الحالي
                    dtFrom.Value = new DateTime(now.Year, now.Month, 1);
                    dtTo.Value = now.Date;
                    break;
                case 4: // الشهر الماضي
                    dtFrom.Value = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
                    dtTo.Value = new DateTime(now.Year, now.Month, 1).AddDays(-1);
                    break;
                case 5: // الربع الحالي
                    var quarter = (now.Month - 1) / 3;
                    dtFrom.Value = new DateTime(now.Year, quarter * 3 + 1, 1);
                    dtTo.Value = now.Date;
                    break;
                case 6: // السنة الحالية
                    dtFrom.Value = new DateTime(now.Year, 1, 1);
                    dtTo.Value = now.Date;
                    break;
                case 7: // فترة مخصصة
                    break;
            }
        }

        // ===== refresh =====
        private async Task RefreshAllAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnRefresh.Enabled = false;

                var from = dtFrom.Value.Date;
                var to = dtTo.Value.Date;

                UpdatePeriodLabelDesign(from, to);
                lblPeriod.Text = "🔄 جاري تحديث البيانات...";

                await Task.WhenAll(
                    LoadKpis(from, to),
                    LoadInvoices(),
                    LoadPayments(),
                    LoadOutstanding()
                );
            }
            catch (Exception ex)
            {
                lblPeriod.Text = "❌ حدث خطأ أثناء تحديث البيانات";
                MessageBox.Show("خطأ: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRefresh.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private async Task LoadKpis(DateTime from, DateTime to)
        {
            var kpis = await Task.Run(() => _svc.GetKpis(from, to));
            var smsStats = await Task.Run(() => _svc.GetSmsStatistics(from, to));

            if (IsHandleCreated)
                BeginInvoke(new Action(() => RenderKpis(kpis, smsStats, from, to)));
        }
        private void RenderKpis(DashboardKpis k, SmsStatistics smsStats, DateTime from, DateTime to)
        {
            kpiPanel.Controls.Clear();
            kpiPanel.SuspendLayout();

            decimal collectionRate = 0;
            if (k.BilledThisMonth > 0)
                collectionRate = (k.CollectedThisMonth / k.BilledThisMonth) * 100m;

            AddKpi("المشتركين النشطين", k.ActiveSubscribers.ToString("N0"), "👥",
                Color.FromArgb(33, 150, 243), 100, "إجمالي المشتركين");

            AddKpi("فواتير الفترة", k.InvoicesThisMonth.ToString("N0"), "📄",
                Color.FromArgb(76, 175, 80), 100, "عدد الفواتير");

            AddKpi("إجمالي الفوترة", k.BilledThisMonth.ToString("N2") + " ريال", "💰",
                Color.FromArgb(255, 193, 7), 100, "قيمة الفواتير");

            AddKpi("نسبة التحصيل", collectionRate.ToString("N1") + "%", "📊",
                Color.FromArgb(156, 39, 176), (int)Math.Min(100, Math.Max(0, collectionRate)),
                $"{k.CollectedThisMonth:N2} ر.س");

            AddKpi("المستحقات الكلية", k.OutstandingTotal.ToString("N2") + " ريال ", "⚠️",
                Color.FromArgb(244, 67, 54), 100, "إجمالي المديونية");

            int smsProgress = smsStats.Total > 0 ? (smsStats.Sent * 100 / smsStats.Total) : 0;
            AddKpi("رسائل SMS", $"{smsStats.SentToday} 📨", "💬",
                Color.FromArgb(0, 150, 136), smsProgress,
                $"{smsStats.Sent} ✓ | {smsStats.Failed} ✗");

            kpiPanel.ResumeLayout();
        }

        private Panel CreateModernKpiCard(string title, string value, string subtitle, string icon, Color color, int progress)
        {
            // ✅ بطاقة أصغر وأخف
            var card = new Panel
            {
                Width = 195,
                Height = 80,
                BackColor = Color.White,
                Margin = new Padding(8, 6, 8, 6),
                Padding = new Padding(10, 8, 10, 8),
                Cursor = Cursors.Hand
            };

            card.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // ظل خفيف جدًا
                using (var shadowBrush = new SolidBrush(Color.FromArgb(12, 0, 0, 0)))
                    e.Graphics.FillRectangle(shadowBrush, 2, 2, card.Width - 2, card.Height - 2);

                // Border ناعم
                using (var borderPen = new Pen(Color.FromArgb(235, 238, 242), 1))
                    e.Graphics.DrawRectangle(borderPen, 0, 0, card.Width - 1, card.Height - 1);

                // شريط علوي بسيط
                using (var topBarBrush = new SolidBrush(color))
                    e.Graphics.FillRectangle(topBarBrush, 0, 0, card.Width, 3);
            };

            // ✅ أيقونة أصغر
            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 20f),
                Location = new Point(10, 14),
                Size = new Size(44, 44),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = color,
                BackColor = Color.Transparent
            };

            // ✅ عنوان أصغر وأوضح
            var lblTitle = new Label
            {
                Text = title,
                Location = new Point(60, 8),
                Size = new Size(140, 18),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.FromArgb(90, 90, 90),
                Font = new Font("Segoe UI", 8.8f, FontStyle.Regular),
                BackColor = Color.Transparent
            };

            // ✅ قيمة أصغر (كانت 20f كبيرة)
            var lblValue = new Label
            {
                Text = value,
                Location = new Point(60, 26),
                Size = new Size(140, 28),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 14.5f, FontStyle.Bold),
                ForeColor = color,
                BackColor = Color.Transparent
            };

            // ✅ سطر فرعي أصغر
            var lblSubtitle = new Label
            {
                Text = subtitle,
                Location = new Point(60, 54),
                Size = new Size(140, 16),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.FromArgb(135, 135, 135),
                Font = new Font("Segoe UI", 8f, FontStyle.Regular),
                BackColor = Color.Transparent
            };

            // ✅ Progress أصغر وأجمل
            var progressBar = new Panel
            {
                Location = new Point(10, 76),
                Size = new Size(card.Width - 20, 5),
                BackColor = Color.FromArgb(235, 235, 235)
            };

            int w = (int)(progressBar.Width * (Math.Min(100, Math.Max(0, progress)) / 100.0));
            var progressFill = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(w, progressBar.Height),
                BackColor = color
            };
            progressBar.Controls.Add(progressFill);

            // ✅ Hover أنعم
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(252, 252, 252);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

            // اجعل الضغط على أي عنصر داخل البطاقة يعمل Click
            void WireClick(Control c)
            {
                c.Cursor = Cursors.Hand;
                c.Click += (s, e) => OnKpiCardClick(title);
            }

            WireClick(iconLabel);
            WireClick(lblTitle);
            WireClick(lblValue);
            WireClick(lblSubtitle);
            WireClick(progressBar);

            card.Click += (s, e) =>
            {
                if (title.Contains("SMS")) ShowSmsReport();
                else if (title.Contains("المستحقات")) ShowAgingReport();
            };

            card.Controls.AddRange(new Control[] { iconLabel, lblTitle, lblValue, lblSubtitle, progressBar });
            return card;
        }
        private void OnKpiCardClick(string title)
        {
            if (title.Contains("SMS"))
                ShowSmsReport();
            else if (title.Contains("المستحقات"))
                ShowAgingReport();
        }

        private async Task LoadInvoices()
        {
            var data = await Task.Run(() => _svc.GetLastInvoices(30));
            if (IsHandleCreated)
                BeginInvoke(new Action(() =>
                {
                    gridInvoices.DataSource = data;
                    FormatInvoiceRows();
                }));
        }

        private async Task LoadPayments()
        {
            var data = await Task.Run(() => _svc.GetLastPayments(30));
            if (IsHandleCreated)
                BeginInvoke(new Action(() => gridPayments.DataSource = data));
        }

        private async Task LoadOutstanding()
        {
            var data = await Task.Run(() => _svc.GetTopOutstanding(30));
            if (IsHandleCreated)
                BeginInvoke(new Action(() => gridOutstanding.DataSource = data));
        }

        private void UpdatePeriodLabelDesign(DateTime from, DateTime to)
        {
            var periodText = $"📅 الفترة: {from:yyyy/MM/dd} إلى {to:yyyy/MM/dd}";
            lblPeriod.Text = periodText;
        }

    
        private void AddKpi(string title, string value, string icon, Color color, int progress, string subtitle)
        {
            var card = CreateModernKpiCard(title, value, subtitle, icon, color, progress);
            kpiPanel.Controls.Add(card);
        }

     
        private void KpiCard_Paint(object sender, PaintEventArgs e)
        {
            var card = sender as Panel;
            if (card == null) return;

            using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                e.Graphics.FillRectangle(shadowBrush, 2, 2, card.Width - 2, card.Height - 2);

            using (var borderPen = new Pen(Color.FromArgb(240, 240, 240), 1))
                e.Graphics.DrawRectangle(borderPen, 0, 0, card.Width - 1, card.Height - 1);

            // top bar color
            // نرسم خط علوي بسيط بلون قيمة lblValue
            using (var topBarBrush = new SolidBrush(Color.FromArgb(180, 0, 87, 183)))
                e.Graphics.FillRectangle(topBarBrush, 0, 0, card.Width, 3);
           

        }

        private void FormatInvoiceRows()
        {
            foreach (DataGridViewRow row in gridInvoices.Rows)
            {
                if ((row.Cells["الحالة"]?.Value + "") == "Unpaid")
                {
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(198, 40, 40);
                    row.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }
            }
        }

        private void OpenQuickReadingForm()
        {
            MessageBox.Show("فتح نموذج القراءة السريعة", "قراءة سريعة",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenQuickPaymentForm()
        {
            MessageBox.Show("فتح نموذج السداد السريع", "سداد سريع",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenStatementForm()
        {
            MessageBox.Show("فتح نموذج كشف الحساب", "كشف حساب",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenDetailsForm(DataGridView grid, int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= grid.Rows.Count) return;

            if (grid == gridInvoices)
            {
                var invoiceId = grid.Rows[rowIndex].Cells["رقم الفاتورة"]?.Value?.ToString();
                MessageBox.Show($"فتح تفاصيل الفاتورة رقم {invoiceId}", "تفاصيل الفاتورة");
            }
            else if (grid == gridPayments)
            {
                var paymentId = grid.Rows[rowIndex].Cells["رقم السداد"]?.Value?.ToString();
                MessageBox.Show($"فتح تفاصيل السداد رقم {paymentId}", "تفاصيل السداد");
            }
            else if (grid == gridOutstanding)
            {
                var subscriberId = grid.Rows[rowIndex].Cells["كود المشترك"]?.Value?.ToString();
                MessageBox.Show($"فتح تفاصيل المتأخرات للمشترك {subscriberId}", "تفاصيل المتأخرات");
            }
        }

        private void ShowAllRecords(string tabTitle)
        {
            DataTable data = null;

            switch (tabTitle)
            {
                case "آخر الفواتير": data = _svc.GetLastInvoices(1000); break;
                case "آخر المدفوعات": data = _svc.GetLastPayments(1000); break;
                case "أعلى المتأخرات": data = _svc.GetTopOutstanding(1000); break;
            }

            if (data == null) return;

            var form = new Form
            {
                Text = $"عرض الكل - {tabTitle}",
                Size = new Size(1000, 600),
                StartPosition = FormStartPosition.CenterParent,
                RightToLeft = RightToLeft.Yes,
                RightToLeftLayout = true
            };

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                DataSource = data
            };

            form.Controls.Add(grid);
            form.ShowDialog(this);
        }

        private void ShowSmsReport()
        {
            var reportForm = new Form
            {
                Text = "تقرير الرسائل القصيرة",
                Size = new Size(1000, 600),
                StartPosition = FormStartPosition.CenterParent,
                RightToLeft = RightToLeft.Yes,
                RightToLeftLayout = true
            };

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            grid.DataSource = _svc.GetSmsReport(dtFrom.Value, dtTo.Value);
            reportForm.Controls.Add(grid);
            reportForm.ShowDialog(this);
        }

        private void ShowAgingReport()
        {
            var reportForm = new Form
            {
                Text = "تقرير شيخوخة المدينين",
                Size = new Size(900, 600),
                StartPosition = FormStartPosition.CenterParent,
                RightToLeft = RightToLeft.Yes,
                RightToLeftLayout = true
            };

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            grid.DataSource = _svc.GetAgingReceivables(DateTime.Now);
            reportForm.Controls.Add(grid);
            reportForm.ShowDialog(this);
        }
    }
  

}

/*using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace water3
{
    public partial class DashboardForm : Form
    {
        private readonly DashboardService _svc = new DashboardService();

        private DateTimePicker dtFrom, dtTo;
        private Button btnRefresh, btnQuickReading, btnQuickPay, btnStatement;
        private FlowLayoutPanel kpiPanel;
        private Label lblPeriod;
        private TabControl tabs;
        private DataGridView gridInvoices, gridPayments, gridOutstanding;
        private ComboBox cbPeriodPreset;
        private Timer autoRefreshTimer;

        // ===== Theme (نفس فورم Form1/Collectors) =====
        private static readonly Color Primary = Color.FromArgb(0, 87, 183);
        private static readonly Color PrimaryDark = Color.FromArgb(0, 70, 150);
        private static readonly Color Bg = Color.FromArgb(245, 247, 250);
        private static readonly Color Card = Color.White;
        private static readonly Color Border = Color.FromArgb(225, 230, 235);
        private static readonly Color Muted = Color.FromArgb(120, 120, 120);
        private static readonly Color Success = Color.FromArgb(40, 167, 69);
        private static readonly Color Warning = Color.FromArgb(255, 193, 7);
        private static readonly Color Danger = Color.FromArgb(220, 53, 69);
        // تحديد اللون الخاص بالتأثير عند التمرير (Hover)
        private static readonly Color Hover = Color.FromArgb(15, 105, 205);

        private static readonly Color GridHeaderColor = Color.FromArgb(245, 248, 255);
        private static readonly Color GridAlternate = Color.FromArgb(252, 252, 252);
        private static readonly Color GridSelection = Color.FromArgb(220, 235, 255);

        private static readonly Font TitleFont = new Font("Segoe UI", 12.5f, FontStyle.Bold);
        private static readonly Font RegularFont = new Font("Segoe UI", 10f);
        private static readonly Font HeaderFont = new Font("Segoe UI", 10f, FontStyle.Bold);

        public DashboardForm()
        {
            InitializeComponent();

            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            BackColor = Bg;
            Font = RegularFont;

            InitializeDashboard();
        }

        private void InitializeDashboard()
        {
            SuspendLayout();

            // Root
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Bg,
                Padding = new Padding(10)
            };
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));   // Filters card
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 30));   // Period bar
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 155));  // KPI row
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // Tabs

            Controls.Clear();
            Controls.Add(root);

            // إنشاء عناصر التحكم
            dtFrom = new DateTimePicker();
            dtTo = new DateTimePicker();
            btnRefresh = new Button();
            btnQuickReading = new Button();
            btnQuickPay = new Button();
            btnStatement = new Button();
            cbPeriodPreset = new ComboBox();

            // Cards
            var filterCard = BuildFilterCard();
            lblPeriod = BuildPeriodBar();
            kpiPanel = BuildKpiPanel();
            var tabsCard = BuildTabsCard();

            root.Controls.Add(filterCard, 0, 0);
            root.Controls.Add(lblPeriod, 0, 1);
            root.Controls.Add(kpiPanel, 0, 2);
            root.Controls.Add(tabsCard, 0, 3);

            SetupAutoRefresh();

            Load += async (s, e) => await RefreshAllAsync();
            FormClosing += (s, e) => autoRefreshTimer?.Stop();

            ResumeLayout();
        }

        private FlowLayoutPanel BuildKpiPanel()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = false,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0),
                BackColor = Bg
            };
            return panel;
        }
        private Control BuildFilterCard()
        {
            var card = MakeCardPanel();
            card.Dock = DockStyle.Fill;
            card.Padding = new Padding(12);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 11,
                RowCount = 1,
                RightToLeft = RightToLeft.Yes,
                Padding = new Padding(0, 5, 0, 5)
            };

            // توزيع الأعمدة الجديد
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));     // كشف حساب [📊] (0)
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));     // سداد سريع [💰] (1)
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));     // قراءة سريعة [📝] (2)
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40));     // تحديث [🔄] (3)
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 15));     // spacer (4)
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));      // من picker (5)
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12));      // من label (6)
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15));      // إلى picker (7)
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 12));      // إلى label (8)
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16));      // preset (9)
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10));      // "الفترة:" (10)

            // ===== إنشاء النصوص مع محاذاة منتصف =====
            var lblPeriodText = new Label
            {
                Text = "الفترة:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,  // محاذاة في المنتصف
                ForeColor = Color.FromArgb(60, 60, 60),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            cbPeriodPreset = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = RegularFont
            };
            cbPeriodPreset.Items.AddRange(new object[] { "اليوم", "أمس", "الأسبوع الحالي", "الشهر الحالي", "الشهر الماضي", "الربع الحالي", "السنة الحالية", "فترة مخصصة" });
            cbPeriodPreset.SelectedIndex = 3;
            cbPeriodPreset.SelectedIndexChanged += (s, e) => ApplyPeriodPreset();

            // تسميات التاريخ مع محاذاة منتصف
            var lblFrom = new Label
            {
                Text = "من:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,  // محاذاة في المنتصف
                ForeColor = Color.FromArgb(60, 60, 60),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            var lblTo = new Label
            {
                Text = "إلى:",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,  // محاذاة في المنتصف
                ForeColor = Color.FromArgb(60, 60, 60),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            dtFrom = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Dock = DockStyle.Fill,
                Font = RegularFont,
                RightToLeftLayout = true
            };

            dtTo = new DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Dock = DockStyle.Fill,
                Font = RegularFont,
                RightToLeftLayout = true
            };

            ApplyPeriodPreset();

            // تحويل الأزرار إلى أيقونات فقط
            btnRefresh = MakeIconButton("🔄", "تحديث البيانات");
            btnRefresh.Click += async (s, e) => await RefreshAllAsync();

            btnQuickReading = MakeIconButton("📝", "قراءة سريعة");
            btnQuickReading.Click += (s, e) => OpenQuickReadingForm();

            btnQuickPay = MakeIconButton("💰", "سداد سريع");
            btnQuickPay.Click += (s, e) => OpenQuickPaymentForm();

            btnStatement = MakeIconButton("📊", "كشف حساب");
            btnStatement.Click += (s, e) => OpenStatementForm();

            // ===== إضافة محاذاة عمودية متساوية =====

            // استخدام TableLayoutPanel داخل كل عمود لضبط المحاذاة
            var periodLabelWrapper = CreateAlignedWrapper(lblPeriodText);
            var fromLabelWrapper = CreateAlignedWrapper(lblFrom);
            var toLabelWrapper = CreateAlignedWrapper(lblTo);
            var presetWrapper = CreateInputWrapper(cbPeriodPreset);
            var fromDateWrapper = CreateInputWrapper(dtFrom);
            var toDateWrapper = CreateInputWrapper(dtTo);

            // إضافة العناصر مع المحاذاة الصحيحة
            layout.Controls.Add(btnStatement, 0, 0);
              layout.Controls.Add(btnQuickPay, 1, 0);
                layout.Controls.Add(btnQuickReading, 2, 0);
               layout.Controls.Add(btnRefresh, 3, 0);
               layout.Controls.Add(new Panel { Dock = DockStyle.Fill }, 4, 0);
               layout.Controls.Add(lblTo, 7, 0);
                layout.Controls.Add(WrapInput(dtTo), 8, 0);
                layout.Controls.Add(lblFrom, 5, 0);
                layout.Controls.Add(WrapInput(dtFrom), 6, 0);
                layout.Controls.Add(WrapInput(cbPeriodPreset), 10, 0);
                layout.Controls.Add(lblPeriodText, 9, 0);

            // ===== تحسين إضافي: ضبط ارتفاع الصف =====
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));

            card.Controls.Add(layout);
            return card;
        }

        // دالة مساعدة لإنشاء غلاف محاذاة للنصوص
        private Panel CreateAlignedWrapper(Label label)
        {
            var wrapper = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            label.Dock = DockStyle.Fill;
            wrapper.Controls.Add(label);

            return wrapper;
        }

        // دالة مساعدة لإنشاء غلاف محاذاة لحقول الإدخال
        private Panel CreateInputWrapper(Control control)
        {
            var wrapper = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(5, 8, 5, 8), // نفس المسافة للحقول
                Margin = new Padding(0)
            };

            control.Dock = DockStyle.Fill;
            control.Height = 36; // ارتفاع موحد
            wrapper.Controls.Add(control);

            // إضافة حدود
            wrapper.Paint += (s, e) =>
            {
                using (var pen = new Pen(Border))
                    e.Graphics.DrawRectangle(pen, 0, 0, wrapper.Width - 1, wrapper.Height - 1);
            };

            return wrapper;
        }
    
        private Button MakeIconButton(string icon, string tooltip)
        {
            var btn = new Button
            {
                Text = icon,
                BackColor = Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Emoji", 12f, FontStyle.Regular),
                Cursor = Cursors.Hand,
                Height = 40,
                Width = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(3)
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.BorderColor = Color.White;

            // تحسين تأثيرات التمرير
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = Hover;
                btn.FlatAppearance.BorderColor = Color.FromArgb(200, 200, 200);
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = Primary;
                btn.FlatAppearance.BorderColor = Color.White;
            };

            // تأثير عند الضغط
            btn.MouseDown += (s, e) => btn.BackColor = ControlPaint.Dark(Primary, 0.2f);
            btn.MouseUp += (s, e) => btn.BackColor = Primary;

            var tt = new ToolTip();
            tt.SetToolTip(btn, tooltip);

            return btn;
        }

        private Label BuildPeriodBar()
        {
            var bar = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 9.8f, FontStyle.Regular),
                BackColor = Card,
                ForeColor = Muted,
                Padding = new Padding(7, 0, 7, 0)
            };

            // Border
            bar.Paint += (s, e) =>
            {
                using (var pen = new Pen(Border))
                    e.Graphics.DrawRectangle(pen, 0, 0, bar.Width - 1, bar.Height - 1);
            };

            return bar;
        }

        private Button CreateEnhancedButton(string text, Color backColor, Color foreColor)
        {
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                BackColor = backColor,
                ForeColor = foreColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                Cursor = Cursors.Hand,
                Height = 20
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor);
            btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor);

            return btn;
        }

        private void ApplyPeriodPreset()
        {
            var now = DateTime.Now;
            switch (cbPeriodPreset.SelectedIndex)
            {
                case 0: // اليوم
                    dtFrom.Value = now.Date;
                    dtTo.Value = now.Date;
                    break;
                case 1: // أمس
                    dtFrom.Value = now.AddDays(-1).Date;
                    dtTo.Value = now.AddDays(-1).Date;
                    break;
                case 2: // الأسبوع الحالي
                    dtFrom.Value = now.AddDays(-(int)now.DayOfWeek).Date;
                    dtTo.Value = now.Date;
                    break;
                case 3: // الشهر الحالي
                    dtFrom.Value = new DateTime(now.Year, now.Month, 1);
                    dtTo.Value = now.Date;
                    break;
                case 4: // الشهر الماضي
                    dtFrom.Value = new DateTime(now.Year, now.Month, 1).AddMonths(-1);
                    dtTo.Value = new DateTime(now.Year, now.Month, 1).AddDays(-1);
                    break;
                case 5: // الربع الحالي
                    var quarter = (now.Month - 1) / 3;
                    dtFrom.Value = new DateTime(now.Year, quarter * 3 + 1, 1);
                    dtTo.Value = now.Date;
                    break;
                case 6: // السنة الحالية
                    dtFrom.Value = new DateTime(now.Year, 1, 1);
                    dtTo.Value = now.Date;
                    break;
                case 7: // فترة مخصصة
                    // لا تغيير
                    break;
            }
        }

        private Control BuildTabsCard()
        {
            var card = MakeCardPanel();
            card.Dock = DockStyle.Fill;
            card.Padding = new Padding(6);

            tabs = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = RegularFont
            };

            tabs.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabs.ItemSize = new Size(160, 38);
            tabs.SizeMode = TabSizeMode.Fixed;
            tabs.DrawItem += Tabs_DrawItem;

            gridInvoices = CreateModernGrid();
            gridPayments = CreateModernGrid();
            gridOutstanding = CreateModernGrid();

            AddTabPage("📄 آخر الفواتير", gridInvoices, "آخر الفواتير");
            AddTabPage("💰 آخر المدفوعات", gridPayments, "آخر المدفوعات");
            AddTabPage("⚠️ أعلى المتأخرات", gridOutstanding, "أعلى المتأخرات");

            card.Controls.Add(tabs);
            return card;
        }

        private void Tabs_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tabPage = tabs.TabPages[e.Index];
            var rect = tabs.GetTabRect(e.Index);
            bool selected = (tabs.SelectedIndex == e.Index);

            using (var bg = new SolidBrush(selected ? Primary : Color.FromArgb(245, 245, 245)))
                e.Graphics.FillRectangle(bg, rect);

            using (var pen = new Pen(selected ? Primary : Border))
                e.Graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width - 1, rect.Height - 1);

            TextRenderer.DrawText(
                e.Graphics,
                tabPage.Text,
                HeaderFont,
                rect,
                selected ? Color.White : Color.FromArgb(60, 60, 60),
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.RightToLeft
            );
        }

        private void AddTabPage(string tabText, DataGridView grid, string title)
        {
            var page = new TabPage { Text = tabText, BackColor = Bg, Padding = new Padding(8) };

            var header = new Panel { Dock = DockStyle.Top, Height = 44, BackColor = Card, Padding = new Padding(10, 6, 10, 6) };
            header.Paint += (s, e) =>
            {
                using (var pen = new Pen(Border))
                    e.Graphics.DrawRectangle(pen, 0, 0, header.Width - 1, header.Height - 1);
            };

            var lbl = new Label
            {
                Text = title,
                Dock = DockStyle.Right,
                Width = 260,
                Font = TitleFont,
                ForeColor = Primary,
                TextAlign = ContentAlignment.MiddleRight
            };

            var btnAll = MakeActionButton("عرض الكل ↗", Primary, Color.White, false);
            btnAll.Dock = DockStyle.Left;
            btnAll.Width = 120;
            btnAll.Height = 36;
            btnAll.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            btnAll.Click += (s, e) => ShowAllRecords(title);

            header.Controls.Add(lbl);
            header.Controls.Add(btnAll);

            var body = new Panel { Dock = DockStyle.Fill, BackColor = Bg, Padding = new Padding(0, 8, 0, 0) };

            var gridCard = MakeCardPanel();
            gridCard.Dock = DockStyle.Fill;
            gridCard.Padding = new Padding(6);
            grid.Dock = DockStyle.Fill;
            gridCard.Controls.Add(grid);

            body.Controls.Add(gridCard);

            page.Controls.Add(body);
            page.Controls.Add(header);

            tabs.TabPages.Add(page);
        }

        private DataGridView CreateModernGrid()
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Card,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 36 },
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Border
            };

            dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = GridHeaderColor,
                ForeColor = Color.FromArgb(60, 60, 60),
                Font = HeaderFont,
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(6, 0, 6, 0)
            };

            dgv.DefaultCellStyle = new DataGridViewCellStyle
            {
                Font = RegularFont,
                SelectionBackColor = GridSelection,
                SelectionForeColor = Color.Black,
                Padding = new Padding(6, 0, 6, 0),
                Alignment = DataGridViewContentAlignment.MiddleRight
            };

            dgv.AlternatingRowsDefaultCellStyle.BackColor = GridAlternate;

            dgv.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0) OpenDetailsForm(dgv, e.RowIndex);
            };

            return dgv;
        }

        private Panel MakeCardPanel()
        {
            var p = new Panel
            {
                BackColor = Card,
                Margin = new Padding(0),
                Padding = new Padding(1)
            };

            p.Paint += (s, e) =>
            {
                // ظل خفيف
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 2, 2, p.Width - 2, p.Height - 2);
                }

                // الحدود
                using (var pen = new Pen(Border))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
                }
            };
            return p;
        }

        private Label MakeLabel(string text, bool bold)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.FromArgb(60, 60, 60),
                Font = new Font("Segoe UI", 9.5f, bold ? FontStyle.Bold : FontStyle.Regular),
                Padding = new Padding(0, 0, 0, 0)
            };
        }

        private Panel WrapInput(Control c)
        {
            var container = new Panel
            {
                BackColor = Color.White,
                Dock = DockStyle.Fill,
                Padding = new Padding(10, 6, 10, 6)
            };

            container.Paint += (s, e) =>
            {
                bool focused = c.Focused;
                using (var pen = new Pen(focused ? Primary : Border))
                    e.Graphics.DrawRectangle(pen, 0, 0, container.Width - 1, container.Height - 1);
            };

            c.Dock = DockStyle.Fill;
            container.Controls.Add(c);
            c.GotFocus += (s, e) => container.Invalidate();
            c.LostFocus += (s, e) => container.Invalidate();

            return container;
        }

        private Button MakeActionButton(string text, Color backColor, Color foreColor, bool outline)
        {
            var btn = new Button
            {
                Text = text,
                BackColor = outline ? Color.White : backColor,
                ForeColor = outline ? Primary : foreColor,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Regular),
                Cursor = Cursors.Hand,
                Height = 38,
                Padding = new Padding(15, 0, 15, 0),
                Margin = new Padding(5, 0, 5, 0)
            };

            btn.FlatAppearance.BorderSize = outline ? 1 : 0;
            btn.FlatAppearance.BorderColor = Primary;

            // تأثيرات التمرير المحسنة
            btn.MouseEnter += (s, e) =>
            {
                btn.BackColor = outline ? Color.FromArgb(245, 245, 245) : Hover;
                if (outline) btn.FlatAppearance.BorderColor = Hover;
            };

            btn.MouseLeave += (s, e) =>
            {
                btn.BackColor = outline ? Color.White : backColor;
                if (outline) btn.FlatAppearance.BorderColor = Primary;
            };

            return btn;
        }

        private DataGridView CreateEnhancedDataGridView()
        {
            var grid = new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(248, 250, 252)
                },
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(240, 240, 240),
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 45,
                RowTemplate = { Height = 40 }
            };

            grid.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            grid.DefaultCellStyle.ForeColor = Color.FromArgb(64, 64, 64);
            grid.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);
            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(26, 35, 126),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleRight,
                Padding = new Padding(5, 0, 5, 0)
            };

            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 245, 254);
            grid.DefaultCellStyle.SelectionForeColor = Color.FromArgb(26, 35, 126);

            grid.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.RowIndex < grid.Rows.Count)
                {
                    OpenDetailsForm(grid, e.RowIndex);
                }
            };

            grid.Paint += (s, e) =>
            {
                if (grid.Rows.Count == 0)
                {
                    var message = "لا توجد بيانات لعرضها";
                    var font = new Font("Segoe UI", 12, FontStyle.Italic);
                    var size = e.Graphics.MeasureString(message, font);

                    var x = (grid.Width - size.Width) / 2;
                    var y = (grid.Height - size.Height) / 2;

                    using (var brush = new SolidBrush(Color.FromArgb(128, 128, 128)))
                    {
                        e.Graphics.DrawString(message, font, brush, x, y);
                    }
                }
            };

            return grid;
        }

        private void CustomizeGridColumns()
        {
            gridInvoices.Columns.Clear();
            gridPayments.Columns.Clear();
            gridOutstanding.Columns.Clear();

            var invoiceColumns = new[]
            {
                new { Name = "رقم الفاتورة", Width = 100 },
                new { Name = "تاريخ الفاتورة", Width = 120 },
                new { Name = "اسم المشترك", Width = 200 },
                new { Name = "المبلغ الإجمالي", Width = 120 },
                new { Name = "الحالة", Width = 100 }
            };

            foreach (var col in invoiceColumns)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    HeaderText = col.Name,
                    Width = col.Width,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };

                if (col.Name == "المبلغ الإجمالي")
                {
                    column.DefaultCellStyle.Format = "N2";
                    column.DefaultCellStyle.ForeColor = Color.FromArgb(198, 40, 40);
                }
                else if (col.Name == "تاريخ الفاتورة")
                {
                    column.DefaultCellStyle.Format = "yyyy/MM/dd";
                }

                gridInvoices.Columns.Add(column);
            }

            var paymentColumns = new[]
            {
                new { Name = "رقم السداد", Width = 100 },
                new { Name = "تاريخ السداد", Width = 120 },
                new { Name = "اسم المشترك", Width = 200 },
                new { Name = "المبلغ", Width = 120 },
                new { Name = "المحصل", Width = 150 },
                new { Name = "طريقة الدفع", Width = 120 }
            };

            foreach (var col in paymentColumns)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    HeaderText = col.Name,
                    Width = col.Width,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };

                if (col.Name == "المبلغ")
                {
                    column.DefaultCellStyle.Format = "N2";
                    column.DefaultCellStyle.ForeColor = Color.FromArgb(0, 128, 0);
                    column.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }
                else if (col.Name == "تاريخ السداد")
                {
                    column.DefaultCellStyle.Format = "yyyy/MM/dd";
                }

                gridPayments.Columns.Add(column);
            }

            var outstandingColumns = new[]
            {
                new { Name = "كود المشترك", Width = 100 },
                new { Name = "اسم المشترك", Width = 250 },
                new { Name = "الرصيد", Width = 150 }
            };

            foreach (var col in outstandingColumns)
            {
                var column = new DataGridViewTextBoxColumn
                {
                    HeaderText = col.Name,
                    Width = col.Width,
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };

                if (col.Name == "الرصيد")
                {
                    column.DefaultCellStyle.Format = "N2";
                    column.DefaultCellStyle.ForeColor = Color.FromArgb(198, 40, 40);
                    column.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }

                gridOutstanding.Columns.Add(column);
            }

            foreach (var grid in new[] { gridInvoices, gridPayments, gridOutstanding })
            {
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

                foreach (DataGridViewColumn column in grid.Columns)
                {
                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                    column.HeaderCell.Style.Padding = new Padding(0, 0, 10, 0);
                }

                grid.GridColor = Color.FromArgb(224, 224, 224);
            }
        }

        private async Task RefreshAllAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                btnRefresh.Enabled = false;

                var from = dtFrom.Value.Date;
                var to = dtTo.Value.Date;

                UpdatePeriodLabelDesign(from, to);
                lblPeriod.Text = "🔄 جاري تحديث البيانات...";

                await Task.WhenAll(
                    LoadKpis(from, to),
                    LoadInvoices(),
                    LoadPayments(),
                    LoadOutstanding()
                );

                Cursor = Cursors.Default;
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                lblPeriod.Text = "❌ حدث خطأ أثناء تحديث البيانات";
                MessageBox.Show("خطأ: " + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnRefresh.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void SetupAutoRefresh()
        {
            autoRefreshTimer = new Timer { Interval = 300000 };
            autoRefreshTimer.Tick += async (s, e) => await RefreshAllAsync();
            autoRefreshTimer.Start();
        }

        private async Task LoadKpis(DateTime from, DateTime to)
        {
            var kpis = await Task.Run(() => _svc.GetKpis(from, to));
            var smsStats = await Task.Run(() => _svc.GetSmsStatistics(from, to));

            this.Invoke(new Action(() =>
            {
                RenderKpis(kpis, smsStats, from, to);
            }));
        }

        private async Task LoadInvoices()
        {
            var data = await Task.Run(() => _svc.GetLastInvoices(30));
            this.Invoke(new Action(() =>
            {
                gridInvoices.DataSource = data;
                FormatInvoiceRows();
            }));
        }

        private async Task LoadPayments()
        {
            var data = await Task.Run(() => _svc.GetLastPayments(30));
            this.Invoke(new Action(() =>
            {
                gridPayments.DataSource = data;
            }));
        }

        private async Task LoadOutstanding()
        {
            var data = await Task.Run(() => _svc.GetTopOutstanding(30));
            this.Invoke(new Action(() =>
            {
                gridOutstanding.DataSource = data;
            }));
        }

        private void UpdatePeriodLabelDesign(DateTime from, DateTime to)
        {
            var periodText = $"📅 الفترة: {from:yyyy/MM/dd} إلى {to:yyyy/MM/dd}";

            if (from.Date == to.Date && from.Date == DateTime.Today)
            {
                periodText = $"☀️ {periodText} (اليوم)";
            }
            else if (from.Month == to.Month && from.Year == to.Year)
            {
                periodText = $"📆 {periodText} (شهر {GetArabicMonthName(from.Month)})";
            }

            lblPeriod.Text = periodText;
        }

        private string GetArabicMonthName(int month)
        {
            var months = new[]
            {
                "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو",
                "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر"
            };
            return months[month - 1];
        }

        private void RenderKpis(DashboardKpis k, SmsStatistics smsStats, DateTime from, DateTime to)
        {
            kpiPanel.Controls.Clear();
            kpiPanel.SuspendLayout();

            decimal collectionRate = 0;
            if (k.BilledThisMonth > 0)
            {
                collectionRate = (k.CollectedThisMonth / k.BilledThisMonth) * 100;
            }

            var kpis = new[]
            {
                new {
                    Title = "المشتركين النشطين",
                    Value = k.ActiveSubscribers.ToString("N0"),
                    Icon = "👥",
                    Color = Color.FromArgb(33, 150, 243),
                    Progress = 100,
                    Subtitle = "إجمالي المشتركين"
                },
                new {
                    Title = "فواتير الفترة",
                    Value = k.InvoicesThisMonth.ToString("N0"),
                    Icon = "📄",
                    Color = Color.FromArgb(76, 175, 80),
                    Progress = 100,
                    Subtitle = $"{GetArabicMonthName(from.Month)}"
                },
                new {
                    Title = "إجمالي الفوترة",
                    Value = k.BilledThisMonth.ToString("N2") + " ر.س",
                    Icon = "💰",
                    Color = Color.FromArgb(255, 193, 7),
                    Progress = 100,
                    Subtitle = "قيمة الفواتير"
                },
                new {
                    Title = "نسبة التحصيل",
                    Value = collectionRate.ToString("N1") + "%",
                    Icon = "📊",
                    Color = Color.FromArgb(156, 39, 176),
                    Progress = (int)collectionRate,
                    Subtitle = $"{k.CollectedThisMonth:N2} ر.س"
                },
                new {
                    Title = "المستحقات الكلية",
                    Value = k.OutstandingTotal.ToString("N2") + " ر.س",
                    Icon = "⚠️",
                    Color = Color.FromArgb(244, 67, 54),
                    Progress = 100,
                    Subtitle = "إجمالي المديونية"
                },
                new {
                    Title = "رسائل SMS",
                    Value = $"{smsStats.SentToday} 📨",
                    Icon = "💬",
                    Color = Color.FromArgb(0, 150, 136),
                    Progress = smsStats.Total > 0 ? (smsStats.Sent * 100 / smsStats.Total) : 0,
                    Subtitle = $"{smsStats.Sent} ✓ | {smsStats.Failed} ✗"
                }
            };

            foreach (var kpi in kpis)
            {
                kpiPanel.Controls.Add(CreateModernKpiCard(
                    kpi.Title,
                    kpi.Value,
                    kpi.Subtitle,
                    kpi.Icon,
                    kpi.Color,
                    kpi.Progress
                ));
            }

            kpiPanel.ResumeLayout();
        }

        private Panel CreateModernKpiCard(string title, string value, string subtitle,
            string icon, Color color, int progress)
        {
            var card = new Panel
            {
                Width = 240,
                Height = 120,
                BackColor = Color.White,
                Margin = new Padding(10, 5, 10, 5),
                Padding = new Padding(15, 15, 15, 10),
                Cursor = Cursors.Hand
            };

            card.Paint += (s, e) =>
            {
                using (var shadowBrush = new SolidBrush(Color.FromArgb(10, 0, 0, 0)))
                {
                    e.Graphics.FillRectangle(shadowBrush, 2, 2, card.Width - 2, card.Height - 2);
                }

                using (var borderPen = new Pen(Color.FromArgb(240, 240, 240), 1))
                {
                    e.Graphics.DrawRectangle(borderPen, 0, 0, card.Width - 1, card.Height - 1);
                }

                using (var topBarBrush = new SolidBrush(color))
                {
                    e.Graphics.FillRectangle(topBarBrush, 0, 0, card.Width, 3);
                }
            };

            var iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 28),
                Location = new Point(15, 20),
                Size = new Size(60, 60),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = color
            };

            var lblTitle = new Label
            {
                Text = title,
                Location = new Point(85, 15),
                Size = new Size(140, 20),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.FromArgb(64, 64, 64),
                Font = new Font("Segoe UI", 9, FontStyle.Regular)
            };

            var lblValue = new Label
            {
                Text = value,
                Location = new Point(85, 40),
                Size = new Size(140, 40),
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = color
            };

            var lblSubtitle = new Label
            {
                Text = subtitle,
                Location = new Point(85, 80),
                Size = new Size(140, 20),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.FromArgb(128, 128, 128),
                Font = new Font("Segoe UI", 8, FontStyle.Regular)
            };

            var progressBar = new Panel
            {
                Location = new Point(15, 105),
                Size = new Size(210, 4),
                BackColor = Color.FromArgb(230, 230, 230)
            };

            var progressFill = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size((int)(210 * (progress / 100.0)), 4),
                BackColor = color
            };

            progressBar.Controls.Add(progressFill);

            card.Controls.AddRange(new Control[] { iconLabel, lblTitle, lblValue, lblSubtitle, progressBar });

            card.MouseEnter += (s, e) =>
            {
                card.BackColor = Color.FromArgb(250, 250, 250);
            };

            card.MouseLeave += (s, e) =>
            {
                card.BackColor = Color.White;
            };

            card.Click += (s, e) =>
            {
                if (title.Contains("SMS"))
                {
                    ShowSmsReport();
                }
                else if (title.Contains("المستحقات"))
                {
                    ShowAgingReport();
                }
            };

            return card;
        }

        private void FormatInvoiceRows()
        {
            foreach (DataGridViewRow row in gridInvoices.Rows)
            {
                if (row.Cells["الحالة"]?.Value?.ToString() == "Unpaid")
                {
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(198, 40, 40);
                    row.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }
            }
        }

        private void OpenQuickReadingForm()
        {
            MessageBox.Show("فتح نموذج القراءة السريعة", "قراءة سريعة",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenQuickPaymentForm()
        {
            MessageBox.Show("فتح نموذج السداد السريع", "سداد سريع",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenStatementForm()
        {
            MessageBox.Show("فتح نموذج كشف الحساب", "كشف حساب",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowSmsReport()
        {
            var reportForm = new Form
            {
                Text = "تقرير الرسائل القصيرة",
                Size = new Size(1000, 600),
                StartPosition = FormStartPosition.CenterParent
            };

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var data = _svc.GetSmsReport(dtFrom.Value, dtTo.Value);
            grid.DataSource = data;

            reportForm.Controls.Add(grid);
            reportForm.ShowDialog();
        }

        private void ShowAgingReport()
        {
            var reportForm = new Form
            {
                Text = "تقرير شيخوخة المدينين",
                Size = new Size(900, 600),
                StartPosition = FormStartPosition.CenterParent
            };

            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            var data = _svc.GetAgingReceivables(DateTime.Now);
            grid.DataSource = data;

            reportForm.Controls.Add(grid);
            reportForm.ShowDialog();
        }

        private void OpenDetailsForm(DataGridView grid, int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < grid.Rows.Count)
            {
                if (grid == gridInvoices)
                {
                    var invoiceId = grid.Rows[rowIndex].Cells["رقم الفاتورة"]?.Value?.ToString();
                    MessageBox.Show($"فتح تفاصيل الفاتورة رقم {invoiceId}", "تفاصيل الفاتورة");
                }
                else if (grid == gridPayments)
                {
                    var paymentId = grid.Rows[rowIndex].Cells["رقم السداد"]?.Value?.ToString();
                    MessageBox.Show($"فتح تفاصيل السداد رقم {paymentId}", "تفاصيل السداد");
                }
                else if (grid == gridOutstanding)
                {
                    var subscriberId = grid.Rows[rowIndex].Cells["كود المشترك"]?.Value?.ToString();
                    MessageBox.Show($"فتح تفاصيل المتأخرات للمشترك {subscriberId}", "تفاصيل المتأخرات");
                }
            }
        }

        private void ShowAllRecords(string tabTitle)
        {
            DataTable data = null;

            switch (tabTitle)
            {
                case "آخر الفواتير":
                    data = _svc.GetLastInvoices(1000);
                    break;
                case "آخر المدفوعات":
                    data = _svc.GetLastPayments(1000);
                    break;
                case "أعلى المتأخرات":
                    data = _svc.GetTopOutstanding(1000);
                    break;
            }

            if (data != null)
            {
                var form = new Form
                {
                    Text = $"عرض الكل - {tabTitle}",
                    Size = new Size(1000, 600),
                    StartPosition = FormStartPosition.CenterParent
                };

                var grid = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                };

                grid.DataSource = data;
                form.Controls.Add(grid);
                form.ShowDialog();
            }
        }
    }

    public class DashboardKpis
    {
        public int ActiveSubscribers { get; set; }
        public int InvoicesThisMonth { get; set; }
        public decimal BilledThisMonth { get; set; }
        public decimal CollectedThisMonth { get; set; }
        public decimal OutstandingTotal { get; set; }
        public int PendingSms { get; set; }
        public bool IsCurrentPeriodClosed { get; set; }
    }

    public class SmsStatistics
    {
        public int Total { get; set; }
        public int Sent { get; set; }
        public int Failed { get; set; }
        public int Pending { get; set; }
        public int SentToday { get; set; }
    }

    public class DashboardService
    {
        private readonly string _cs = @"Data Source=.;Initial Catalog=WaterBillingDB;Integrated Security=True";

        public DashboardKpis GetKpis(DateTime from, DateTime to)
        {
            var k = new DashboardKpis();

            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
SET NOCOUNT ON;

DECLARE @FromDate DATE = @pFrom, @ToDate DATE = @pTo;

SELECT
    (SELECT COUNT(*) FROM Subscribers WHERE IsActive = 1) AS ActiveSubscribers,
    (SELECT COUNT(*) FROM Invoices WHERE InvoiceDate BETWEEN @FromDate AND @ToDate) AS InvoicesThisMonth,
    (SELECT ISNULL(SUM(TotalAmount),0) FROM Invoices WHERE InvoiceDate BETWEEN @FromDate AND @ToDate) AS BilledThisMonth,
    (SELECT ISNULL(SUM(Amount),0) FROM Payments WHERE PaymentDate BETWEEN @FromDate AND @ToDate) AS CollectedThisMonth,
    (SELECT ISNULL(SUM(Balance),0) FROM vw_SubscriberBalance) AS OutstandingTotal,
    (SELECT COUNT(*) FROM SmsLogs WHERE Status = 'Pending') AS PendingSms,
    dbo.fn_IsPeriodClosed(GETDATE()) AS IsCurrentPeriodClosed;
", con))
            {
                cmd.Parameters.AddWithValue("@pFrom", from.Date);
                cmd.Parameters.AddWithValue("@pTo", to.Date);

                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        k.ActiveSubscribers = r["ActiveSubscribers"] != DBNull.Value ? Convert.ToInt32(r["ActiveSubscribers"]) : 0;
                        k.InvoicesThisMonth = r["InvoicesThisMonth"] != DBNull.Value ? Convert.ToInt32(r["InvoicesThisMonth"]) : 0;
                        k.BilledThisMonth = r["BilledThisMonth"] != DBNull.Value ? Convert.ToDecimal(r["BilledThisMonth"]) : 0;
                        k.CollectedThisMonth = r["CollectedThisMonth"] != DBNull.Value ? Convert.ToDecimal(r["CollectedThisMonth"]) : 0;
                        k.OutstandingTotal = r["OutstandingTotal"] != DBNull.Value ? Convert.ToDecimal(r["OutstandingTotal"]) : 0;
                        k.PendingSms = r["PendingSms"] != DBNull.Value ? Convert.ToInt32(r["PendingSms"]) : 0;
                        k.IsCurrentPeriodClosed = r["IsCurrentPeriodClosed"] != DBNull.Value && Convert.ToBoolean(r["IsCurrentPeriodClosed"]);
                    }
                }
            }

            return k;
        }

        public SmsStatistics GetSmsStatistics(DateTime from, DateTime to)
        {
            var stats = new SmsStatistics();

            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT
    (SELECT COUNT(*) FROM SmsLogs WHERE CreatedAt BETWEEN @FromDate AND @ToDate) AS Total,
    (SELECT COUNT(*) FROM SmsLogs WHERE Status = 'Sent' AND CreatedAt BETWEEN @FromDate AND @ToDate) AS Sent,
    (SELECT COUNT(*) FROM SmsLogs WHERE Status = 'Failed' AND CreatedAt BETWEEN @FromDate AND @ToDate) AS Failed,
    (SELECT COUNT(*) FROM SmsLogs WHERE Status = 'Pending') AS Pending,
    (SELECT COUNT(*) FROM SmsLogs WHERE Status = 'Sent' AND CAST(CreatedAt AS DATE) = CAST(GETDATE() AS DATE)) AS SentToday;
", con))
            {
                cmd.Parameters.AddWithValue("@FromDate", from);
                cmd.Parameters.AddWithValue("@ToDate", to);

                con.Open();
                using (SqlDataReader r = cmd.ExecuteReader())
                {
                    if (r.Read())
                    {
                        stats.Total = r["Total"] != DBNull.Value ? Convert.ToInt32(r["Total"]) : 0;
                        stats.Sent = r["Sent"] != DBNull.Value ? Convert.ToInt32(r["Sent"]) : 0;
                        stats.Failed = r["Failed"] != DBNull.Value ? Convert.ToInt32(r["Failed"]) : 0;
                        stats.Pending = r["Pending"] != DBNull.Value ? Convert.ToInt32(r["Pending"]) : 0;
                        stats.SentToday = r["SentToday"] != DBNull.Value ? Convert.ToInt32(r["SentToday"]) : 0;
                    }
                }
            }

            return stats;
        }

        public DataTable GetSmsReport(DateTime from, DateTime to)
        {
            return Fill(@"
SELECT 
    SL.SmsID AS 'رقم الرسالة',
    S.Name AS 'اسم المشترك',
    SL.PhoneNumber AS 'رقم الهاتف',
    LEFT(SL.Message, 100) + '...' AS 'معاينة الرسالة',
    SL.Status AS 'الحالة',
    SL.SentDate AS 'تاريخ الإرسال',
    C.Name AS 'المحصل'
FROM SmsLogs SL
LEFT JOIN Subscribers S ON SL.SubscriberID = S.SubscriberID
LEFT JOIN Collectors C ON SL.CollectorID = C.CollectorID
WHERE SL.CreatedAt BETWEEN @FromDate AND @ToDate
ORDER BY SL.CreatedAt DESC;",
                CommandType.Text,
                ("@FromDate", from),
                ("@ToDate", to));
        }

        public DataTable GetLastInvoices(int top = 20)
        {
            return Fill(@"
SELECT TOP (@top)
    I.InvoiceID AS 'رقم الفاتورة',
    FORMAT(I.InvoiceDate, 'yyyy/MM/dd') AS 'تاريخ الفاتورة',
    S.Name AS 'اسم المشترك',
    I.TotalAmount AS 'المبلغ الإجمالي',
    I.Status AS 'الحالة'
FROM Invoices I
JOIN Subscribers S ON S.SubscriberID = I.SubscriberID
WHERE S.IsActive = 1
ORDER BY I.InvoiceDate DESC, I.InvoiceID DESC;",
                CommandType.Text,
                ("@top", top));
        }

        public DataTable GetLastPayments(int top = 20)
        {
            return Fill(@"
SELECT TOP (@top)
    P.PaymentID AS 'رقم السداد',
    FORMAT(P.PaymentDate, 'yyyy/MM/dd') AS 'تاريخ السداد',
    S.Name AS 'اسم المشترك',
    P.Amount AS 'المبلغ',
    ISNULL(C.Name, N'غير محدد') AS 'المحصل',
    P.PaymentType AS 'طريقة الدفع'
FROM Payments P
JOIN Subscribers S ON S.SubscriberID = P.SubscriberID
LEFT JOIN Collectors C ON C.CollectorID = P.CollectorID
WHERE S.IsActive = 1
ORDER BY P.PaymentDate DESC, P.PaymentID DESC;",
                CommandType.Text,
                ("@top", top));
        }

        public DataTable GetTopOutstanding(int top = 20)
        {
            return Fill(@"
SELECT TOP (@top)
    S.SubscriberID AS 'كود المشترك',
    S.Name AS 'اسم المشترك',
    B.Balance AS 'الرصيد'
FROM vw_SubscriberBalance B
JOIN Subscribers S ON S.SubscriberID = B.SubscriberID
WHERE B.Balance > 0
    AND S.IsActive = 1
ORDER BY B.Balance DESC;",
                CommandType.Text,
                ("@top", top));
        }

        public DataTable GetAgingReceivables(DateTime asOfDate)
        {
            return Fill("rpt_AgingReceivables",
                CommandType.StoredProcedure,
                ("@AsOfDate", asOfDate));
        }

        public DataTable GetSubscriberStatement(int subscriberId, DateTime from, DateTime to)
        {
            return Fill(@"
SELECT 
    FORMAT(Date, 'yyyy/MM/dd') AS 'التاريخ',
    Details AS 'التفاصيل',
    DocumentType AS 'نوع المستند',
    DocumentNumber AS 'رقم المستند',
    Debit AS 'مدين',
    Credit AS 'دائن',
    BalanceAfter AS 'الرصيد بعد'
FROM AccountStatements
WHERE SubscriberID = @SubscriberID
    AND Date BETWEEN @FromDate AND @ToDate
ORDER BY Date, StatementID;",
                CommandType.Text,
                ("@SubscriberID", subscriberId),
                ("@FromDate", from),
                ("@ToDate", to));
        }

        private DataTable Fill(string sql, CommandType type, params (string name, object value)[] prms)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_cs))
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.CommandType = type;
                    cmd.CommandTimeout = 30;

                    foreach (var p in prms)
                    {
                        cmd.Parameters.AddWithValue(p.name, p.value ?? DBNull.Value);
                    }

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }

            return dt;
        }
    }
}
*/