using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using water3.Reports;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace water3
{
    public partial class UnpaidInvoicesReportForm : Form
    {
   

            private readonly InvoiceManagementService _service = new InvoiceManagementService();
            private bool _isLoading;

            private TableLayoutPanel root;
            private Panel headerCard;
            private TableLayoutPanel headerLayout;
            private Label lblTitle;
            private Label lblSubtitle;

            private TextBox txtSearch;
            private ComboBox cbStatus;
            private DateTimePicker dtFrom;
            private DateTimePicker dtTo;
            private Button btnSearch;
            private Button btnRefresh;
            private Button btnDetails;
            private Button btnSendSms;

            private FlowLayoutPanel summaryPanel;
            private DataGridView gridInvoices;
            private Label lblFooter;

            private static readonly Font FontRegular = new Font("Tahoma", 9.5f, FontStyle.Regular);
            private static readonly Font FontBold = new Font("Tahoma", 10f, FontStyle.Bold);
            private static readonly Font FontTitle = new Font("Tahoma", 14.5f, FontStyle.Bold);
            private static readonly Font FontSmall = new Font("Tahoma", 8.5f, FontStyle.Regular);

            private AppThemePalette P
            {
                get { return AppThemeManager.Palette; }
            }

            public UnpaidInvoicesReportForm()
            {
                AppThemeManager.LoadTheme();

                Text = "إدارة الفواتير";
                Name = "InvoiceManagementForm";
                Font = FontRegular;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = false;
                AutoScaleMode = AutoScaleMode.Font;
                MinimumSize = new Size(1050, 650);

                BuildUi();
                ApplyTheme();
                InitFilters();

                Load += InvoiceManagementForm_Load;
            }

            private void InvoiceManagementForm_Load(object sender, EventArgs e)
            {
                _ = LoadInvoicesAsync();
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
                    RowCount = 4,
                    Padding = new Padding(12),
                    RightToLeft = RightToLeft.No
                };

                root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 92));
                root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

                headerCard = new Panel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(14),
                    Margin = new Padding(0, 0, 0, 8)
                };
                headerCard.Paint += Card_Paint;

                headerLayout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 12,
                    RowCount = 2,
                    RightToLeft = RightToLeft.No
                };

                headerLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
                headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 45));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 45));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 10));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 46));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 46));
                headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 46));

                Panel titlePanel = new Panel
                {
                    Dock = DockStyle.Fill,
                    RightToLeft = RightToLeft.Yes
                };

                lblTitle = new Label
                {
                    Dock = DockStyle.Top,
                    Height = 28,
                    Text = "إدارة الفواتير",
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = FontTitle
                };

                lblSubtitle = new Label
                {
                    Dock = DockStyle.Fill,
                    Text = "استعراض الفواتير، متابعة المدفوع والمتبقي، وإرسال رسائل الفواتير",
                    TextAlign = ContentAlignment.TopRight,
                    Font = FontSmall
                };

                titlePanel.Controls.Add(lblSubtitle);
                titlePanel.Controls.Add(lblTitle);

                txtSearch = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(4, 8, 4, 4),
                    RightToLeft = RightToLeft.Yes
                };

                Label lblSearch = CreateLabel("بحث:");

                cbStatus = new ComboBox
                {
                    Dock = DockStyle.Fill,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Margin = new Padding(4, 8, 4, 4),
                    RightToLeft = RightToLeft.Yes,
                    FlatStyle = FlatStyle.Flat
                };

                Label lblStatus = CreateLabel("الحالة:");

                dtTo = new DateTimePicker
                {
                    Dock = DockStyle.Fill,
                    Format = DateTimePickerFormat.Short,
                    RightToLeft = RightToLeft.Yes,
                    RightToLeftLayout = true,
                    Margin = new Padding(4, 8, 4, 4)
                };

                Label lblTo = CreateLabel("إلى:");

                dtFrom = new DateTimePicker
                {
                    Dock = DockStyle.Fill,
                    Format = DateTimePickerFormat.Short,
                    RightToLeft = RightToLeft.Yes,
                    RightToLeftLayout = true,
                    Margin = new Padding(4, 8, 4, 4)
                };

                Label lblFrom = CreateLabel("من:");

                btnSearch = CreateIconButton("✓", "تطبيق الفلترة");
                btnRefresh = CreateIconButton("↻", "تحديث");
                btnDetails = CreateIconButton("🔎", "تفاصيل الفاتورة");
                btnSendSms = CreateIconButton("✉", "إرسال رسالة");

                btnSearch.Click += async (s, e) => await LoadInvoicesAsync();
                btnRefresh.Click += async (s, e) => await LoadInvoicesAsync();
                btnDetails.Click += btnDetails_Click;
                btnSendSms.Click += btnSendSms_Click;

                headerLayout.Controls.Add(titlePanel, 0, 0);
                headerLayout.SetRowSpan(titlePanel, 2);

                headerLayout.Controls.Add(txtSearch, 1, 0);
                headerLayout.Controls.Add(lblSearch, 2, 0);
                headerLayout.Controls.Add(cbStatus, 3, 0);
                headerLayout.Controls.Add(lblStatus, 4, 0);
                headerLayout.Controls.Add(dtTo, 5, 0);
                headerLayout.Controls.Add(lblTo, 6, 0);
                headerLayout.Controls.Add(dtFrom, 7, 0);
                headerLayout.Controls.Add(lblFrom, 8, 0);
                headerLayout.Controls.Add(btnSearch, 9, 0);
                headerLayout.Controls.Add(btnRefresh, 10, 0);
                headerLayout.Controls.Add(btnDetails, 11, 0);

                headerLayout.Controls.Add(btnSendSms, 11, 1);

                headerCard.Controls.Add(headerLayout);

                summaryPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.RightToLeft,
                    RightToLeft = RightToLeft.Yes,
                    WrapContents = false,
                    AutoScroll = false,
                    Padding = new Padding(0, 4, 0, 4),
                    Margin = new Padding(0, 0, 0, 8)
                };

                gridInvoices = CreateGrid();
                gridInvoices.CellDoubleClick += gridInvoices_CellDoubleClick;

                lblFooter = new Label
                {
                    Dock = DockStyle.Fill,
                    Text = "جاهز",
                    TextAlign = ContentAlignment.MiddleRight,
                    RightToLeft = RightToLeft.Yes,
                    Padding = new Padding(8, 0, 8, 0)
                };

                root.Controls.Add(headerCard, 0, 0);
                root.Controls.Add(summaryPanel, 0, 1);
                root.Controls.Add(gridInvoices, 0, 2);
                root.Controls.Add(lblFooter, 0, 3);

                Controls.Add(root);

                ResumeLayout(true);
            }

            private Label CreateLabel(string text)
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

            private Button CreateIconButton(string text, string tooltip)
            {
                Button btn = new Button
                {
                    Dock = DockStyle.Fill,
                    Text = text,
                    Margin = new Padding(4, 8, 4, 4),
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Font = new Font("Tahoma", 10f, FontStyle.Bold),
                    Tag = tooltip
                };

                btn.FlatAppearance.BorderSize = 0;
                return btn;
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
                    ColumnHeadersHeight = 40,
                    RowTemplate = { Height = 36 },
                    Margin = Padding.Empty
                };

                typeof(DataGridView)
                    .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                    ?.SetValue(dgv, true, null);

                return dgv;
            }

            private void InitFilters()
            {
                cbStatus.Items.Clear();
                cbStatus.Items.Add("الكل");
                cbStatus.Items.Add("غير مدفوعة");
                cbStatus.Items.Add("مدفوعة جزئياً");
                cbStatus.Items.Add("مدفوعة");
                cbStatus.Items.Add("ملغاة");
                cbStatus.SelectedIndex = 0;

                DateTime now = DateTime.Now;
                dtFrom.Value = new DateTime(now.Year, now.Month, 1);
                dtTo.Value = now.Date;
            }

            private void ApplyTheme()
            {
                BackColor = P.Bg;
                root.BackColor = P.Bg;

                headerCard.BackColor = P.Card;
                summaryPanel.BackColor = P.Bg;

                lblTitle.ForeColor = P.Text;
                lblSubtitle.ForeColor = P.Muted;

                txtSearch.BackColor = P.Card;
                txtSearch.ForeColor = P.Text;

                cbStatus.BackColor = P.Card;
                cbStatus.ForeColor = P.Text;

                dtFrom.CalendarTitleBackColor = P.Primary;
                dtFrom.CalendarTitleForeColor = Color.White;
                dtTo.CalendarTitleBackColor = P.Primary;
                dtTo.CalendarTitleForeColor = Color.White;

                foreach (Control c in headerLayout.Controls)
                {
                    if (c is Label label && label != lblTitle && label != lblSubtitle)
                    {
                        label.ForeColor = P.Text;
                        label.BackColor = P.Card;
                    }

                    if (c is Button btn)
                    {
                        btn.BackColor = P.Primary;
                        btn.ForeColor = Color.White;
                        btn.FlatAppearance.MouseOverBackColor = P.Hover;
                        btn.FlatAppearance.MouseDownBackColor = P.PrimaryDark;
                    }
                }

                StyleGrid(gridInvoices);

                lblFooter.BackColor = P.Card;
                lblFooter.ForeColor = P.Muted;

                headerCard.Invalidate();
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

            private async Task LoadInvoicesAsync()
            {
                if (_isLoading)
                    return;

                try
                {
                    _isLoading = true;
                    Cursor = Cursors.WaitCursor;
                    btnSearch.Enabled = false;
                    btnRefresh.Enabled = false;

                    lblFooter.Text = "جاري تحميل الفواتير...";

                    DateTime from = dtFrom.Value.Date;
                    DateTime to = dtTo.Value.Date;

                    if (from > to)
                    {
                        MessageBox.Show("تاريخ البداية يجب أن يكون قبل تاريخ النهاية.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string status = cbStatus.SelectedItem == null ? "الكل" : cbStatus.SelectedItem.ToString();
                    string search = txtSearch.Text.Trim();

                    InvoiceManagementResult result = await Task.Run(() =>
                        _service.GetInvoices(from, to, status, search));

                    gridInvoices.DataSource = result.Invoices;
                    FormatGridColumns();
                    ApplyRowsStyle();

                    RenderSummary(result.Summary);

                    lblFooter.Text = $"عدد الفواتير: {result.Invoices.Rows.Count:N0} | آخر تحديث: {DateTime.Now:HH:mm:ss}";
                }
                catch (Exception ex)
                {
                    lblFooter.Text = "فشل تحميل الفواتير";
                    MessageBox.Show("حدث خطأ أثناء تحميل الفواتير.\n\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    btnSearch.Enabled = true;
                    btnRefresh.Enabled = true;
                    Cursor = Cursors.Default;
                    _isLoading = false;
                }
            }

            private void RenderSummary(InvoiceSummary s)
            {
                summaryPanel.SuspendLayout();

                foreach (Control c in summaryPanel.Controls)
                    c.Dispose();

                summaryPanel.Controls.Clear();

                AddSummaryCard("عدد الفواتير", s.InvoiceCount.ToString("N0"), "📄", P.Primary);
                AddSummaryCard("إجمالي الفوترة", s.TotalAmount.ToString("N2"), "💰", P.Warning);
                AddSummaryCard("إجمالي المدفوع", s.PaidAmount.ToString("N2"), "✅", P.Success);
                AddSummaryCard("إجمالي المتبقي", s.RemainingAmount.ToString("N2"), "⚠️", P.Danger);
                AddSummaryCard("غير مدفوعة", s.UnpaidCount.ToString("N0"), "❌", P.Danger);
                AddSummaryCard("مدفوعة جزئياً", s.PartialCount.ToString("N0"), "◐", P.Warning);
                AddSummaryCard("مدفوعة", s.PaidCount.ToString("N0"), "✓", P.Success);

                summaryPanel.ResumeLayout(true);
            }

            private void AddSummaryCard(string title, string value, string icon, Color accent)
            {
                Panel card = new Panel
                {
                    Width = 180,
                    Height = 74,
                    Margin = new Padding(5),
                    BackColor = P.Card
                };

                card.Paint += (s, e) =>
                {
                    using (Pen pen = new Pen(P.Border))
                        e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);

                    using (SolidBrush b = new SolidBrush(accent))
                        e.Graphics.FillRectangle(b, 0, 0, card.Width, 4);
                };

                Label lblIcon = new Label
                {
                    Text = icon,
                    Font = new Font("Segoe UI Emoji", 18f),
                    Location = new Point(8, 16),
                    Size = new Size(38, 38),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = accent,
                    BackColor = Color.Transparent
                };

                Label lblValue = new Label
                {
                    Text = value,
                    Location = new Point(50, 18),
                    Size = new Size(120, 24),
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = new Font("Tahoma", 12f, FontStyle.Bold),
                    ForeColor = accent,
                    BackColor = Color.Transparent
                };

                Label lblTitle = new Label
                {
                    Text = title,
                    Location = new Point(50, 43),
                    Size = new Size(120, 18),
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = FontSmall,
                    ForeColor = P.Muted,
                    BackColor = Color.Transparent
                };

                card.Controls.Add(lblIcon);
                card.Controls.Add(lblValue);
                card.Controls.Add(lblTitle);

                summaryPanel.Controls.Add(card);
            }

            private void FormatGridColumns()
            {
                foreach (DataGridViewColumn col in gridInvoices.Columns)
                {
                    col.SortMode = DataGridViewColumnSortMode.NotSortable;

                    string h = col.HeaderText ?? "";

                    if (h.Contains("تاريخ"))
                        col.DefaultCellStyle.Format = "yyyy/MM/dd";

                    if (h.Contains("مبلغ") ||
                        h.Contains("مدفوع") ||
                        h.Contains("متبقي") ||
                        h.Contains("متأخرات") ||
                        h.Contains("استهلاك"))
                    {
                        col.DefaultCellStyle.Format = "N2";
                    }
                }

                HideColumn("InvoiceID");
                HideColumn("SubscriberID");
                HideColumn("MeterID");
            }

            private void HideColumn(string name)
            {
                if (gridInvoices.Columns.Contains(name))
                    gridInvoices.Columns[name].Visible = false;
            }

            private void ApplyRowsStyle()
            {
                foreach (DataGridViewRow row in gridInvoices.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    string status = GetRowText(row, "الحالة");

                    if (status.Contains("غير مدفوعة"))
                    {
                        row.DefaultCellStyle.ForeColor = P.Danger;
                        row.DefaultCellStyle.Font = FontBold;
                    }
                    else if (status.Contains("جزئ"))
                    {
                        row.DefaultCellStyle.ForeColor = P.Warning;
                        row.DefaultCellStyle.Font = FontBold;
                    }
                    else if (status.Contains("مدفوعة"))
                    {
                        row.DefaultCellStyle.ForeColor = P.Success;
                    }
                    else if (status.Contains("ملغاة"))
                    {
                        row.DefaultCellStyle.ForeColor = P.Muted;
                    }
                }
            }

            private string GetRowText(DataGridViewRow row, string columnName)
            {
                if (!row.DataGridView.Columns.Contains(columnName))
                    return string.Empty;

                object v = row.Cells[columnName].Value;
                return v == null || v == DBNull.Value ? string.Empty : v.ToString();
            }

            private int? GetSelectedInvoiceId()
            {
                if (gridInvoices.CurrentRow == null)
                    return null;

                if (!gridInvoices.Columns.Contains("InvoiceID"))
                    return null;

                object v = gridInvoices.CurrentRow.Cells["InvoiceID"].Value;

                if (v == null || v == DBNull.Value)
                    return null;

                return Convert.ToInt32(v);
            }

            private void btnDetails_Click(object sender, EventArgs e)
            {
                ShowInvoiceDetails();
            }

            private void gridInvoices_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
            {
                if (e.RowIndex >= 0)
                    ShowInvoiceDetails();
            }

            private void ShowInvoiceDetails()
            {
                int? invoiceId = GetSelectedInvoiceId();

                if (!invoiceId.HasValue)
                {
                    MessageBox.Show("اختر فاتورة أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                try
                {
                    DataTable details = _service.GetInvoiceDetails(invoiceId.Value);

                    using (Form frm = new Form())
                    {
                        frm.Text = "تفاصيل الفاتورة";
                        frm.StartPosition = FormStartPosition.CenterParent;
                        frm.Size = new Size(900, 520);
                        frm.RightToLeft = RightToLeft.Yes;
                        frm.RightToLeftLayout = true;
                        frm.BackColor = P.Bg;
                        frm.Font = FontRegular;

                        DataGridView dgv = CreateGrid();
                        dgv.DataSource = details;
                        StyleGrid(dgv);

                        frm.Controls.Add(dgv);
                        frm.ShowDialog(this);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("تعذر عرض تفاصيل الفاتورة.\n\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void btnSendSms_Click(object sender, EventArgs e)
            {
                int? invoiceId = GetSelectedInvoiceId();

                if (!invoiceId.HasValue)
                {
                    MessageBox.Show("اختر فاتورة أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (MessageBox.Show("هل تريد إرسال رسالة الفاتورة للمشترك؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;

                try
                {
                    _service.SendInvoiceSms(invoiceId.Value);

                    MessageBox.Show("تم تجهيز رسالة الفاتورة بنجاح.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("تعذر إرسال الرسالة.\n\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void Card_Paint(object sender, PaintEventArgs e)
            {
                Control c = sender as Control;
                if (c == null) return;

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

                using (Pen pen = new Pen(P.Border))
                    e.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
            }
        }

        public class InvoiceManagementResult
        {
            public DataTable Invoices { get; set; }
            public InvoiceSummary Summary { get; set; }
        }

        public class InvoiceSummary
        {
            public int InvoiceCount { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal PaidAmount { get; set; }
            public decimal RemainingAmount { get; set; }
            public int UnpaidCount { get; set; }
            public int PartialCount { get; set; }
            public int PaidCount { get; set; }
        }

        public class InvoiceManagementService
        {
            private readonly string _connectionString;

            public InvoiceManagementService()
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

                throw new InvalidOperationException("لم يتم العثور على ConnectionString باسم WaterBillingDB.");
            }

            public InvoiceManagementResult GetInvoices(DateTime from, DateTime to, string status, string search)
            {
                InvoiceManagementResult result = new InvoiceManagementResult();
                result.Invoices = new DataTable();
                result.Summary = new InvoiceSummary();

            string sql = @"
IF OBJECT_ID('tempdb..#InvoiceData') IS NOT NULL
    DROP TABLE #InvoiceData;

SELECT
    I.InvoiceID,
    I.SubscriberID,
    I.MeterID,
    ISNULL(I.InvoiceNumber, CAST(I.InvoiceID AS NVARCHAR(30))) AS [رقم الفاتورة],
    I.InvoiceDate AS [تاريخ الفاتورة],
    S.Name AS [المشترك],
    ISNULL(S.PhoneNumber, N'') AS [الهاتف],
    ISNULL(M.MeterNumber, ISNULL(S.MeterNumber, N'')) AS [رقم العداد],
    CAST(ISNULL(I.Consumption, 0) AS DECIMAL(18,2)) AS [الاستهلاك],
    CAST(ISNULL(I.UnitPrice, 0) AS DECIMAL(18,2)) AS [سعر الوحدة],
    CAST(ISNULL(I.ServiceFees, 0) AS DECIMAL(18,2)) AS [رسوم الخدمة],
    CAST(ISNULL(I.Arrears, 0) AS DECIMAL(18,2)) AS [متأخرات],
    CAST(ISNULL(I.TotalAmount, 0) AS DECIMAL(18,2)) AS [مبلغ الفترة],
    CAST(ISNULL(PA.PaidAmount, 0) AS DECIMAL(18,2)) AS [مدفوع],
    CAST((ISNULL(I.TotalAmount,0) + ISNULL(I.Arrears,0)) - ISNULL(PA.PaidAmount,0) AS DECIMAL(18,2)) AS [المتبقي],
    ISNULL(I.Status, N'') AS [الحالة],
    ISNULL(I.Notes, N'') AS [ملاحظات]
INTO #InvoiceData
FROM dbo.Invoices I
INNER JOIN dbo.Subscribers S ON S.SubscriberID = I.SubscriberID
LEFT JOIN dbo.Meters M ON M.MeterID = I.MeterID
OUTER APPLY
(
    SELECT SUM(P.Amount) AS PaidAmount
    FROM dbo.Payments P
    WHERE P.InvoiceID = I.InvoiceID
) PA
WHERE I.InvoiceDate BETWEEN @FromDate AND @ToDate
  AND (@Status = N'الكل' OR ISNULL(I.Status,N'') = @Status)
  AND
  (
      @Search = N''
      OR S.Name LIKE N'%' + @Search + N'%'
      OR ISNULL(S.PhoneNumber,N'') LIKE N'%' + @Search + N'%'
      OR ISNULL(M.MeterNumber,N'') LIKE N'%' + @Search + N'%'
      OR ISNULL(S.MeterNumber,N'') LIKE N'%' + @Search + N'%'
      OR ISNULL(I.InvoiceNumber,N'') LIKE N'%' + @Search + N'%'
      OR CAST(I.InvoiceID AS NVARCHAR(30)) LIKE N'%' + @Search + N'%'
  );

SELECT *
FROM #InvoiceData
ORDER BY [تاريخ الفاتورة] DESC, InvoiceID DESC;

SELECT
    COUNT(*) AS InvoiceCount,
    CAST(ISNULL(SUM([مبلغ الفترة] + [متأخرات]),0) AS DECIMAL(18,2)) AS TotalAmount,
    CAST(ISNULL(SUM([مدفوع]),0) AS DECIMAL(18,2)) AS PaidAmount,
    CAST(ISNULL(SUM([المتبقي]),0) AS DECIMAL(18,2)) AS RemainingAmount,
    SUM(CASE WHEN [الحالة] = N'غير مدفوعة' THEN 1 ELSE 0 END) AS UnpaidCount,
    SUM(CASE WHEN [الحالة] = N'مدفوعة جزئياً' THEN 1 ELSE 0 END) AS PartialCount,
    SUM(CASE WHEN [الحالة] = N'مدفوعة' THEN 1 ELSE 0 END) AS PaidCount
FROM #InvoiceData;

DROP TABLE #InvoiceData;
";

            using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.Add("@FromDate", SqlDbType.Date).Value = from.Date;
                    cmd.Parameters.Add("@ToDate", SqlDbType.Date).Value = to.Date;
                    cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 30).Value = status ?? "الكل";
                    cmd.Parameters.Add("@Search", SqlDbType.NVarChar, 100).Value = search ?? "";

                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables.Count > 0)
                        result.Invoices = ds.Tables[0];

                    if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
                    {
                        DataRow r = ds.Tables[1].Rows[0];

                        result.Summary.InvoiceCount = ToInt(r["InvoiceCount"]);
                        result.Summary.TotalAmount = ToDecimal(r["TotalAmount"]);
                        result.Summary.PaidAmount = ToDecimal(r["PaidAmount"]);
                        result.Summary.RemainingAmount = ToDecimal(r["RemainingAmount"]);
                        result.Summary.UnpaidCount = ToInt(r["UnpaidCount"]);
                        result.Summary.PartialCount = ToInt(r["PartialCount"]);
                        result.Summary.PaidCount = ToInt(r["PaidCount"]);
                    }
                }

                return result;
            }

            public DataTable GetInvoiceDetails(int invoiceId)
            {
                DataTable dt = new DataTable();

                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("dbo.GetInvoiceDetails", con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = invoiceId;
                    da.Fill(dt);
                }

                return dt;
            }

            public void SendInvoiceSms(int invoiceId)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("dbo.SendInvoiceSMS", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = invoiceId;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
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