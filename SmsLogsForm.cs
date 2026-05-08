using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace water3
{
    public partial class SmsLogsForm : Form
    {
        private readonly string _connectionString;

        private DataTable smsLogsTable;
        private DataView smsLogsView;

        private AppThemePalette P
        {
            get { return AppThemeManager.Palette; }
        }

        public SmsLogsForm()
        {
            AppThemeManager.LoadTheme();

            _connectionString = GetConnectionString();

            InitializeComponent();

            Text = "سجلات الرسائل النصية";
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Tahoma", 9f);
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;

            cmbStatusFilter.Items.Clear();
            cmbStatusFilter.Items.AddRange(new object[]
            {
                "الكل",
                "Sent",
                "Failed",
                "Pending"
            });
            cmbStatusFilter.SelectedIndex = 0;

            dateFromPicker.Value = DateTime.Now.AddDays(-30).Date;
            dateToPicker.Value = DateTime.Now.Date;

            SetupGridColumnsAndStyle();
            ApplyTheme();
            WireEvents();
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

        private void SmsLogsForm_Load(object sender, EventArgs e)
        {
            LoadSmsLogsDataFromDb();
        }

        private void WireEvents()
        {
            cmbStatusFilter.SelectedIndexChanged -= CmbStatusFilter_SelectedIndexChanged;
            dateFromPicker.ValueChanged -= DateFromPicker_ValueChanged;
            dateToPicker.ValueChanged -= DateToPicker_ValueChanged;
            txtPhoneFilter.TextChanged -= TxtPhoneFilter_TextChanged;
            txtSearchGeneral.TextChanged -= TxtSearchGeneral_TextChanged;

            btnRefresh.Click -= BtnRefresh_Click;
            btnReset.Click -= BtnReset_Click;
            btnResend.Click -= ResendSMS_Click;
            btnExport.Click -= ExportToExcel_Click;
            btnInvoiceDetails.Click -= BtnInvoiceDetails_Click;

            smsLogsDataGridView.CellFormatting -= SmsLogsDataGridView_CellFormatting;
            smsLogsDataGridView.CellDoubleClick -= SmsLogsDataGridView_CellDoubleClick;
            smsLogsDataGridView.SelectionChanged -= SmsLogsDataGridView_SelectionChanged;

            cmbStatusFilter.SelectedIndexChanged += CmbStatusFilter_SelectedIndexChanged;
            dateFromPicker.ValueChanged += DateFromPicker_ValueChanged;
            dateToPicker.ValueChanged += DateToPicker_ValueChanged;
            txtPhoneFilter.TextChanged += TxtPhoneFilter_TextChanged;
            txtSearchGeneral.TextChanged += TxtSearchGeneral_TextChanged;

            btnRefresh.Click += BtnRefresh_Click;
            btnReset.Click += BtnReset_Click;
            btnResend.Click += ResendSMS_Click;
            btnExport.Click += ExportToExcel_Click;
            btnInvoiceDetails.Click += BtnInvoiceDetails_Click;

            smsLogsDataGridView.CellFormatting += SmsLogsDataGridView_CellFormatting;
            smsLogsDataGridView.CellDoubleClick += SmsLogsDataGridView_CellDoubleClick;
            smsLogsDataGridView.SelectionChanged += SmsLogsDataGridView_SelectionChanged;
        }

        private void ApplyTheme()
        {
            BackColor = P.Bg;

            filterPanel.BackColor = P.Card;
            filterPanel.ForeColor = P.Text;

            if (rootLayout != null)
                rootLayout.BackColor = P.Bg;

            if (filterMainLayout != null)
                filterMainLayout.BackColor = P.Card;

            if (filtersLayout != null)
                filtersLayout.BackColor = P.Card;

            if (titlePanel != null)
                titlePanel.BackColor = P.Card;

            if (gridPanel != null)
                gridPanel.BackColor = P.Card;

            if (buttonsPanel != null)
                buttonsPanel.BackColor = P.Card;

            if (lblTitle != null)
            {
                lblTitle.ForeColor = P.Text;
                lblTitle.BackColor = P.Card;
            }

            if (lblSubtitle != null)
            {
                lblSubtitle.ForeColor = P.Muted;
                lblSubtitle.BackColor = P.Card;
            }

            statsPanel.BackColor = P.Primary;
            statsPanel.ForeColor = Color.White;
            statsLabel.ForeColor = Color.White;
            statsLabel.BackColor = P.Primary;

            ApplyThemeToChildren(filterPanel);

            StyleButton(btnRefresh, P.Primary, P.Hover);
            StyleButton(btnReset, P.Muted, P.Hover);
            StyleButton(btnResend, P.Warning, P.PrimaryDark);
            StyleButton(btnExport, P.Success, P.PrimaryDark);
            StyleButton(btnInvoiceDetails, P.Accent, P.PrimaryDark);

            smsLogsDataGridView.BackgroundColor = P.Card;
            smsLogsDataGridView.GridColor = P.Border;

            smsLogsDataGridView.ColumnHeadersDefaultCellStyle.BackColor =
                AppThemeManager.CurrentTheme == AppThemeName.Dark ? P.Soft : P.Primary;

            smsLogsDataGridView.ColumnHeadersDefaultCellStyle.ForeColor =
                AppThemeManager.CurrentTheme == AppThemeName.Dark ? P.Text : Color.White;

            smsLogsDataGridView.ColumnHeadersDefaultCellStyle.Font =
                new Font("Tahoma", 9.5f, FontStyle.Bold);

            smsLogsDataGridView.ColumnHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            smsLogsDataGridView.DefaultCellStyle.BackColor = P.Card;
            smsLogsDataGridView.DefaultCellStyle.ForeColor = P.Text;
            smsLogsDataGridView.DefaultCellStyle.SelectionBackColor = P.Selected;
            smsLogsDataGridView.DefaultCellStyle.SelectionForeColor = P.SelectedText;
            smsLogsDataGridView.DefaultCellStyle.Font = new Font("Tahoma", 9.5f);

            smsLogsDataGridView.AlternatingRowsDefaultCellStyle.BackColor =
                AppThemeManager.CurrentTheme == AppThemeName.Dark ? P.Bg : P.Soft;

            smsLogsDataGridView.AlternatingRowsDefaultCellStyle.ForeColor = P.Text;

            filterPanel.Invalidate();
            gridPanel.Invalidate();
            statsPanel.Invalidate();
        }
        private void ApplyThemeToChildren(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is Label label &&
                    label != lblTitle &&
                    label != lblSubtitle &&
                    label != statsLabel)
                {
                    label.ForeColor = P.Text;
                    label.BackColor = P.Card;
                }

                if (c is TextBox txt)
                {
                    txt.BackColor = P.Card;
                    txt.ForeColor = P.Text;
                    txt.BorderStyle = BorderStyle.FixedSingle;
                }

                if (c is ComboBox cmb)
                {
                    cmb.BackColor = P.Card;
                    cmb.ForeColor = P.Text;
                    cmb.FlatStyle = FlatStyle.Flat;
                }

                if (c is DateTimePicker dt)
                {
                    dt.CalendarTitleBackColor = P.Primary;
                    dt.CalendarTitleForeColor = Color.White;
                    dt.CalendarForeColor = P.Text;
                }

                if (c.HasChildren)
                    ApplyThemeToChildren(c);
            }
        }

        private void StyleButton(Button b, Color normal, Color hover)
        {
            b.BackColor = normal;
            b.ForeColor = Color.White;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.Cursor = Cursors.Hand;

            b.MouseEnter -= Button_MouseEnter;
            b.MouseLeave -= Button_MouseLeave;

            b.MouseEnter += Button_MouseEnter;
            b.MouseLeave += Button_MouseLeave;

            b.Tag = new ButtonColors { Normal = normal, Hover = hover };
        }

        private void Button_MouseEnter(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if (b == null) return;

            ButtonColors colors = b.Tag as ButtonColors;
            if (colors != null)
                b.BackColor = colors.Hover;
        }

        private void Button_MouseLeave(object sender, EventArgs e)
        {
            Button b = sender as Button;
            if (b == null) return;

            ButtonColors colors = b.Tag as ButtonColors;
            if (colors != null)
                b.BackColor = colors.Normal;
        }

        private class ButtonColors
        {
            public Color Normal { get; set; }
            public Color Hover { get; set; }
        }

        private void SetupGridColumnsAndStyle()
        {
            smsLogsDataGridView.AutoGenerateColumns = false;
            smsLogsDataGridView.Columns.Clear();

            smsLogsDataGridView.DefaultCellStyle.Font = new Font("Tahoma", 9.5f);
            smsLogsDataGridView.DefaultCellStyle.SelectionBackColor = P.Selected;
            smsLogsDataGridView.DefaultCellStyle.SelectionForeColor = P.SelectedText;

            smsLogsDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.5f, FontStyle.Bold);
            smsLogsDataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            smsLogsDataGridView.ColumnHeadersHeight = 40;
            smsLogsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            smsLogsDataGridView.RowTemplate.Height = 34;
            smsLogsDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            smsLogsDataGridView.AllowUserToAddRows = false;
            smsLogsDataGridView.AllowUserToDeleteRows = false;
            smsLogsDataGridView.ReadOnly = true;
            smsLogsDataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            smsLogsDataGridView.MultiSelect = true;
            smsLogsDataGridView.RowHeadersVisible = false;
            smsLogsDataGridView.RightToLeft = RightToLeft.Yes;
            smsLogsDataGridView.EnableHeadersVisualStyles = false;

            AddTextColumn("SmsID", "SmsID", "رقم الرسالة", 90, DataGridViewContentAlignment.MiddleCenter);
            AddTextColumn("MessageType", "MessageType", "نوع الرسالة", 95, DataGridViewContentAlignment.MiddleCenter);
            AddTextColumn("SubscriberName", "SubscriberName", "اسم المشترك", 170, DataGridViewContentAlignment.MiddleRight);
            AddTextColumn("PhoneNumber", "PhoneNumber", "رقم الهاتف", 125, DataGridViewContentAlignment.MiddleCenter);
            AddTextColumn("Message", "Message", "نص الرسالة", 360, DataGridViewContentAlignment.MiddleRight, true);
            AddTextColumn("Status", "Status", "الحالة", 95, DataGridViewContentAlignment.MiddleCenter);
            AddTextColumn("EventDate", "EventDate", "تاريخ الرسالة", 145, DataGridViewContentAlignment.MiddleCenter);
            AddTextColumn("RetryCount", "RetryCount", "المحاولات", 80, DataGridViewContentAlignment.MiddleCenter);
            AddTextColumn("Reason", "Reason", "سبب الفشل", 180, DataGridViewContentAlignment.MiddleRight);

            AddHiddenColumn("InvoiceID", "InvoiceID");
            AddHiddenColumn("PaymentID", "PaymentID");
            AddHiddenColumn("ReceiptID", "ReceiptID");
            AddHiddenColumn("SubscriberID", "SubscriberID");
            AddHiddenColumn("CollectorID", "CollectorID");
            AddHiddenColumn("CollectorName", "CollectorName");
            AddHiddenColumn("TemplateID", "TemplateID");
            AddHiddenColumn("CreatedAt", "CreatedAt");
            AddHiddenColumn("SentDate", "SentDate");
        }

        private void AddTextColumn(string name, string dataProperty, string header, int width,
            DataGridViewContentAlignment alignment, bool wrap = false)
        {
            DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn
            {
                Name = name,
                DataPropertyName = dataProperty,
                HeaderText = header,
                Width = width,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = alignment,
                    WrapMode = wrap ? DataGridViewTriState.True : DataGridViewTriState.False
                }
            };

            if (name == "EventDate")
                col.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm";

            smsLogsDataGridView.Columns.Add(col);
        }

        private void AddHiddenColumn(string name, string dataProperty)
        {
            smsLogsDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = name,
                DataPropertyName = dataProperty,
                Visible = false
            });
        }

        private void CmbStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFiltersInstant();
        }

        private void DateFromPicker_ValueChanged(object sender, EventArgs e)
        {
            ApplyFiltersInstant();
        }

        private void DateToPicker_ValueChanged(object sender, EventArgs e)
        {
            ApplyFiltersInstant();
        }

        private void TxtPhoneFilter_TextChanged(object sender, EventArgs e)
        {
            ApplyFiltersInstant();
        }

        private void TxtSearchGeneral_TextChanged(object sender, EventArgs e)
        {
            ApplyFiltersInstant();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            LoadSmsLogsDataFromDb();
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            ResetFilter();
        }

        private void SmsLogsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateInvoiceButtonState();
        }

        private void LoadSmsLogsDataFromDb()
        {
            try
            {
                smsLogsTable = new DataTable();

                string sql = @"
SELECT
    L.SmsID,
    L.InvoiceID,
    L.PaymentID,
    L.ReceiptID,
    L.SubscriberID,
    ISNULL(S.Name, N'') AS SubscriberName,
    ISNULL(L.PhoneNumber, N'') AS PhoneNumber,
    ISNULL(L.Message, N'') AS Message,
    ISNULL(L.Status, N'') AS Status,
    ISNULL(L.Reason, N'') AS Reason,
    L.SentDate,
    L.CreatedAt,
    COALESCE(L.SentDate, L.CreatedAt) AS EventDate,
    L.CollectorID,
    ISNULL(C.Name, N'') AS CollectorName,
    L.TemplateID,
    ISNULL(L.RetryCount, 0) AS RetryCount,
    ISNULL(L.MessageType, N'') AS MessageType
FROM dbo.SmsLogs L
LEFT JOIN dbo.Subscribers S ON S.SubscriberID = L.SubscriberID
LEFT JOIN dbo.Collectors C ON C.CollectorID = L.CollectorID
ORDER BY COALESCE(L.SentDate, L.CreatedAt) DESC, L.SmsID DESC;";

                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(sql, con))
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.Fill(smsLogsTable);
                }

                smsLogsView = smsLogsTable.DefaultView;
                smsLogsDataGridView.DataSource = smsLogsView;

                ApplyFiltersInstant();
                UpdateInvoiceButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "خطأ في تحميل سجلات الرسائل:\n" + ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ApplyFiltersInstant()
        {
            if (smsLogsView == null)
                return;

            try
            {
                List<string> parts = new List<string>();

                if (cmbStatusFilter.SelectedIndex > 0)
                {
                    string st = EscapeForRowFilter(Convert.ToString(cmbStatusFilter.SelectedItem));
                    parts.Add("Status = '" + st + "'");
                }

                if (!string.IsNullOrWhiteSpace(txtPhoneFilter.Text))
                {
                    string phone = EscapeForRowFilter(txtPhoneFilter.Text.Trim());
                    parts.Add("PhoneNumber LIKE '%" + phone + "%'");
                }

                if (!string.IsNullOrWhiteSpace(txtSearchGeneral.Text))
                {
                    string q = EscapeForRowFilter(txtSearchGeneral.Text.Trim());

                    parts.Add(
                        "(SubscriberName LIKE '%" + q + "%' " +
                        "OR Message LIKE '%" + q + "%' " +
                        "OR Reason LIKE '%" + q + "%' " +
                        "OR MessageType LIKE '%" + q + "%' " +
                        "OR CollectorName LIKE '%" + q + "%' " +
                        "OR Convert(SmsID, 'System.String') LIKE '%" + q + "%' " +
                        "OR Convert(InvoiceID, 'System.String') LIKE '%" + q + "%' " +
                        "OR Convert(PaymentID, 'System.String') LIKE '%" + q + "%' " +
                        "OR Convert(ReceiptID, 'System.String') LIKE '%" + q + "%')");
                }

                DateTime from = dateFromPicker.Value.Date;
                DateTime to = dateToPicker.Value.Date.AddDays(1).AddTicks(-1);

                parts.Add(string.Format(
                    CultureInfo.InvariantCulture,
                    "EventDate >= #{0:MM/dd/yyyy HH:mm:ss}# AND EventDate <= #{1:MM/dd/yyyy HH:mm:ss}#",
                    from,
                    to));

                smsLogsView.RowFilter = string.Join(" AND ", parts);
                UpdateStatistics();
                UpdateInvoiceButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "خطأ في التصفية:\n" + ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private string EscapeForRowFilter(string input)
        {
            if (input == null)
                return string.Empty;

            return input.Replace("'", "''")
                        .Replace("[", "[[]")
                        .Replace("%", "[%]")
                        .Replace("*", "[*]");
        }

        private void ResetFilter()
        {
            cmbStatusFilter.SelectedIndex = 0;
            txtPhoneFilter.Text = string.Empty;
            txtSearchGeneral.Text = string.Empty;
            dateFromPicker.Value = DateTime.Now.AddDays(-30).Date;
            dateToPicker.Value = DateTime.Now.Date;

            ApplyFiltersInstant();
        }

        private void SmsLogsDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            if (smsLogsDataGridView.Columns[e.ColumnIndex].Name == "Status")
            {
                string v = Convert.ToString(e.Value).Trim();

                if (v.Equals("Sent", StringComparison.OrdinalIgnoreCase))
                {
                    e.Value = "ناجح";
                    e.CellStyle.ForeColor = P.Success;
                    e.CellStyle.Font = new Font(smsLogsDataGridView.Font, FontStyle.Bold);
                    e.FormattingApplied = true;
                }
                else if (v.Equals("Failed", StringComparison.OrdinalIgnoreCase))
                {
                    e.Value = "فاشل";
                    e.CellStyle.ForeColor = P.Danger;
                    e.CellStyle.Font = new Font(smsLogsDataGridView.Font, FontStyle.Bold);
                    e.FormattingApplied = true;
                }
                else if (v.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                {
                    e.Value = "معلّق";
                    e.CellStyle.ForeColor = P.Warning;
                    e.CellStyle.Font = new Font(smsLogsDataGridView.Font, FontStyle.Bold);
                    e.FormattingApplied = true;
                }
            }

            if (smsLogsDataGridView.Columns[e.ColumnIndex].Name == "MessageType")
            {
                string v = Convert.ToString(e.Value).Trim();

                if (v.Equals("Invoice", StringComparison.OrdinalIgnoreCase))
                    e.Value = "فاتورة";
                else if (v.Equals("Payment", StringComparison.OrdinalIgnoreCase))
                    e.Value = "سداد";
                else if (v.Equals("Late", StringComparison.OrdinalIgnoreCase))
                    e.Value = "متأخرات";

                e.FormattingApplied = true;
            }
        }

        private void UpdateStatistics()
        {
            if (smsLogsView == null)
            {
                statsLabel.Text = "الإحصائيات: إجمالي الرسائل: 0 | ناجح: 0 | فاشل: 0 | معلق: 0";
                return;
            }

            int total = smsLogsView.Count;
            int sent = 0;
            int failed = 0;
            int pending = 0;

            foreach (DataRowView row in smsLogsView)
            {
                string st = Convert.ToString(row["Status"]).Trim();

                if (st.Equals("Sent", StringComparison.OrdinalIgnoreCase))
                    sent++;
                else if (st.Equals("Failed", StringComparison.OrdinalIgnoreCase))
                    failed++;
                else if (st.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                    pending++;
            }

            statsLabel.Text =
                "الإحصائيات: إجمالي الرسائل: " + total.ToString("N0") +
                " | ناجح: " + sent.ToString("N0") +
                " | فاشل: " + failed.ToString("N0") +
                " | معلق: " + pending.ToString("N0");
        }

        private void SmsLogsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            ShowSmsDetails(smsLogsDataGridView.Rows[e.RowIndex]);
        }

        private void ShowSmsDetails(DataGridViewRow row)
        {
            try
            {
                string smsId = GetCell(row, "SmsID");
                string subName = GetCell(row, "SubscriberName");
                string phone = GetCell(row, "PhoneNumber");
                string msg = GetCell(row, "Message");
                string status = GetCell(row, "Status");
                string reason = GetCell(row, "Reason");
                string eventDate = GetCell(row, "EventDate");
                string messageType = GetCell(row, "MessageType");
                string invoiceId = GetCell(row, "InvoiceID");
                string paymentId = GetCell(row, "PaymentID");
                string receiptId = GetCell(row, "ReceiptID");
                string collectorName = GetCell(row, "CollectorName");
                string retry = GetCell(row, "RetryCount");

                Form f = new Form
                {
                    Text = "تفاصيل الرسالة رقم " + smsId,
                    Size = new Size(880, 560),
                    StartPosition = FormStartPosition.CenterParent,
                    Font = new Font("Tahoma", 10),
                    RightToLeft = RightToLeft.Yes,
                    RightToLeftLayout = true,
                    BackColor = P.Bg
                };

                TextBox txt = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    Dock = DockStyle.Fill,
                    ScrollBars = ScrollBars.Vertical,
                    Font = new Font("Tahoma", 10),
                    BackColor = P.Card,
                    ForeColor = P.Text
                };

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("رقم الرسالة: " + smsId);
                sb.AppendLine("نوع الرسالة: " + TranslateMessageType(messageType));
                sb.AppendLine("اسم المشترك: " + subName);
                sb.AppendLine("رقم الهاتف: " + phone);
                sb.AppendLine("الحالة: " + TranslateStatus(status));
                sb.AppendLine("تاريخ الرسالة: " + eventDate);
                sb.AppendLine("المحصل: " + collectorName);
                sb.AppendLine("المحاولات: " + retry);
                sb.AppendLine("سبب الفشل: " + reason);
                sb.AppendLine("InvoiceID: " + invoiceId);
                sb.AppendLine("PaymentID: " + paymentId);
                sb.AppendLine("ReceiptID: " + receiptId);
                sb.AppendLine(new string('-', 55));
                sb.AppendLine("نص الرسالة:");
                sb.AppendLine(msg);

                txt.Text = sb.ToString();

                FlowLayoutPanel pnl = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    Height = 60,
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = false,
                    Padding = new Padding(10),
                    BackColor = P.Card
                };

                Button close = CreateDialogButton("إغلاق", P.Muted);
                close.Click += (s, e) => f.Close();

                Button invoiceDetails = CreateDialogButton("تفاصيل الفاتورة", P.Accent);

                int invId;
                if (int.TryParse(invoiceId, out invId) && invId > 0)
                {
                    invoiceDetails.Enabled = true;
                    invoiceDetails.Click += (s, e) => ShowInvoiceDetails(invId);
                }
                else
                {
                    invoiceDetails.Enabled = false;
                }

                pnl.Controls.Add(invoiceDetails);
                pnl.Controls.Add(close);

                f.Controls.Add(txt);
                f.Controls.Add(pnl);
                f.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "خطأ في عرض تفاصيل الرسالة:\n" + ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private Button CreateDialogButton(string text, Color back)
        {
            Button b = new Button
            {
                Text = text,
                Height = 40,
                Width = 140,
                BackColor = back,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private string GetCell(DataGridViewRow row, string columnName)
        {
            if (row == null || row.DataGridView == null)
                return string.Empty;

            if (!row.DataGridView.Columns.Contains(columnName))
                return string.Empty;

            object value = row.Cells[columnName].Value;
            return value == null || value == DBNull.Value ? string.Empty : value.ToString();
        }

        private string TranslateStatus(string status)
        {
            if (status.Equals("Sent", StringComparison.OrdinalIgnoreCase))
                return "ناجح";

            if (status.Equals("Failed", StringComparison.OrdinalIgnoreCase))
                return "فاشل";

            if (status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                return "معلّق";

            return status;
        }

        private string TranslateMessageType(string type)
        {
            if (type.Equals("Invoice", StringComparison.OrdinalIgnoreCase))
                return "فاتورة";

            if (type.Equals("Payment", StringComparison.OrdinalIgnoreCase))
                return "سداد";

            if (type.Equals("Late", StringComparison.OrdinalIgnoreCase))
                return "متأخرات";

            return type;
        }

        private void BtnInvoiceDetails_Click(object sender, EventArgs e)
        {
            if (smsLogsDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("يرجى تحديد رسالة أولاً.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row = smsLogsDataGridView.SelectedRows[0];
            string invoiceIdText = GetCell(row, "InvoiceID");

            int invoiceId;
            if (!int.TryParse(invoiceIdText, out invoiceId) || invoiceId <= 0)
            {
                MessageBox.Show("هذه الرسالة غير مرتبطة بفاتورة.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ShowInvoiceDetails(invoiceId);
        }

        private void UpdateInvoiceButtonState()
        {
            btnInvoiceDetails.Enabled = false;

            if (smsLogsDataGridView.SelectedRows.Count == 0)
                return;

            DataGridViewRow row = smsLogsDataGridView.SelectedRows[0];
            string invoiceIdText = GetCell(row, "InvoiceID");

            int invoiceId;
            btnInvoiceDetails.Enabled = int.TryParse(invoiceIdText, out invoiceId) && invoiceId > 0;
        }

        private void ShowInvoiceDetails(int invoiceId)
        {
            try
            {
                DataTable dt = GetInvoiceDetailsFromDb(invoiceId);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("لم يتم العثور على تفاصيل الفاتورة.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataRow r = dt.Rows[0];

                Form f = new Form
                {
                    Text = "تفاصيل الفاتورة رقم " + invoiceId,
                    Size = new Size(900, 620),
                    StartPosition = FormStartPosition.CenterParent,
                    Font = new Font("Tahoma", 10),
                    RightToLeft = RightToLeft.Yes,
                    RightToLeftLayout = true,
                    BackColor = P.Bg
                };

                TextBox txt = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    Dock = DockStyle.Fill,
                    ScrollBars = ScrollBars.Vertical,
                    Font = new Font("Tahoma", 10),
                    BackColor = P.Card,
                    ForeColor = P.Text
                };

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("رقم الفاتورة: " + Safe(r, "InvoiceID"));
                sb.AppendLine("رقم المشترك: " + Safe(r, "SubscriberID"));
                sb.AppendLine("اسم المشترك: " + Safe(r, "SubscriberName"));
                sb.AppendLine("رقم العداد: " + Safe(r, "MeterNumber"));
                sb.AppendLine("الهاتف: " + Safe(r, "PhoneNumber"));
                sb.AppendLine("العنوان: " + Safe(r, "Address"));
                sb.AppendLine(new string('-', 55));
                sb.AppendLine("تاريخ الفاتورة: " + Safe(r, "InvoiceDate"));
                sb.AppendLine("الاستهلاك: " + Safe(r, "Consumption"));
                sb.AppendLine("سعر الوحدة: " + Safe(r, "UnitPrice"));
                sb.AppendLine("رسوم الخدمة: " + Safe(r, "ServiceFees"));
                sb.AppendLine("المتأخرات: " + Safe(r, "Arrears"));
                sb.AppendLine(new string('-', 55));
                sb.AppendLine("إجمالي الفاتورة: " + Safe(r, "TotalAmount"));
                sb.AppendLine("المدفوع: " + Safe(r, "Paid"));
                sb.AppendLine("المتبقي الحالي: " + Safe(r, "CurrentDue"));
                sb.AppendLine("الإجمالي المستحق: " + Safe(r, "TotalDue"));
                sb.AppendLine("الحالة: " + Safe(r, "Status"));
                sb.AppendLine(new string('-', 55));
                sb.AppendLine("قراءة سابقة: " + Safe(r, "PreviousReading"));
                sb.AppendLine("قراءة حالية: " + Safe(r, "CurrentReading"));
                sb.AppendLine("تاريخ القراءة: " + Safe(r, "ReadingDate"));
                sb.AppendLine(new string('-', 55));
                sb.AppendLine("ملاحظات: " + Safe(r, "Notes"));

                txt.Text = sb.ToString();

                Button close = new Button
                {
                    Text = "إغلاق",
                    Dock = DockStyle.Bottom,
                    Height = 45,
                    BackColor = P.Muted,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                close.FlatAppearance.BorderSize = 0;
                close.Click += (s, e) => f.Close();

                f.Controls.Add(txt);
                f.Controls.Add(close);
                f.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "خطأ في عرض تفاصيل الفاتورة:\n" + ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private string Safe(DataRow r, string col)
        {
            if (r == null || r.Table == null || !r.Table.Columns.Contains(col))
                return string.Empty;

            if (r[col] == DBNull.Value)
                return string.Empty;

            return r[col].ToString();
        }

        private DataTable GetInvoiceDetailsFromDb(int invoiceId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.GetInvoiceDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("@InvoiceID", SqlDbType.Int).Value = invoiceId;

                DataTable dt = new DataTable();

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    da.Fill(dt);

                return dt;
            }
        }

        private void ResendSMS_Click(object sender, EventArgs e)
        {
            if (smsLogsDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("يرجى تحديد رسائل لإعادة الإرسال.", "تحذير",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "هل تريد إعادة إرسال " + smsLogsDataGridView.SelectedRows.Count + " رسالة؟",
                "تأكيد إعادة الإرسال",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                List<int> idsToUpdate = new List<int>();

                foreach (DataGridViewRow row in smsLogsDataGridView.SelectedRows)
                {
                    string status = GetCell(row, "Status");

                    if (status.Equals("Failed", StringComparison.OrdinalIgnoreCase))
                    {
                        int id;
                        if (int.TryParse(GetCell(row, "SmsID"), out id))
                            idsToUpdate.Add(id);
                    }
                }

                if (idsToUpdate.Count == 0)
                {
                    MessageBox.Show("لا توجد رسائل فاشلة ضمن التحديد.", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    con.Open();

                    foreach (int id in idsToUpdate)
                    {
                        using (SqlCommand cmd = new SqlCommand(@"
UPDATE dbo.SmsLogs
SET RetryCount = ISNULL(RetryCount, 0) + 1,
    Status = N'Pending',
    Reason = NULL,
    SentDate = NULL
WHERE SmsID = @SmsID;", con))
                        {
                            cmd.Parameters.Add("@SmsID", SqlDbType.Int).Value = id;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                LoadSmsLogsDataFromDb();

                MessageBox.Show("تم تحويل الرسائل الفاشلة إلى معلّق لإعادة الإرسال.", "نجاح",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "خطأ في إعادة الإرسال:\n" + ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ExportToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "CSV UTF-8 (*.csv)|*.csv|جميع الملفات (*.*)|*.*",
                    FileName = "SMS_Logs_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv",
                    Title = "تصدير سجلات الرسائل"
                };

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                ExportVisibleRowsToCsv(sfd.FileName);

                MessageBox.Show("تم تصدير البيانات بنجاح.", "تم",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "خطأ في التصدير:\n" + ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ExportVisibleRowsToCsv(string path)
        {
            StringBuilder sb = new StringBuilder();

            List<DataGridViewColumn> columns = new List<DataGridViewColumn>();

            foreach (DataGridViewColumn col in smsLogsDataGridView.Columns)
            {
                if (col.Visible)
                    columns.Add(col);
            }

            for (int i = 0; i < columns.Count; i++)
            {
                if (i > 0) sb.Append(",");
                sb.Append(EscapeCsv(columns[i].HeaderText));
            }

            sb.AppendLine();

            foreach (DataGridViewRow row in smsLogsDataGridView.Rows)
            {
                if (row.IsNewRow)
                    continue;

                for (int i = 0; i < columns.Count; i++)
                {
                    if (i > 0) sb.Append(",");

                    object value = row.Cells[columns[i].Name].Value;
                    sb.Append(EscapeCsv(value == null || value == DBNull.Value ? "" : value.ToString()));
                }

                sb.AppendLine();
            }

            File.WriteAllText(path, sb.ToString(), Encoding.UTF8);
        }

        private string EscapeCsv(string value)
        {
            if (value == null)
                return "\"\"";

            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}