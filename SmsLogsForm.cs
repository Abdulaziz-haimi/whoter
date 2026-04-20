using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace water3
{
    public partial class SmsLogsForm : Form
    {
        // ====== DB ======
        private readonly string _connectionString = @"Data Source=.;Initial Catalog=WaterBillingDB;Integrated Security=True";

        private DataTable smsLogsTable;
        private DataView smsLogsView;

        public SmsLogsForm()
        {
            InitializeComponent();

            // خصائص عامة
            this.Text = "سجلات الرسائل النصية";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Tahoma", 9);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;

            // قيم افتراضية
            cmbStatusFilter.Items.Clear();
            cmbStatusFilter.Items.AddRange(new object[] { "الكل", "Sent", "Failed", "Pending" });
            cmbStatusFilter.SelectedIndex = 0;

            dateFromPicker.Value = DateTime.Now.AddDays(-30);
            dateToPicker.Value = DateTime.Now;

            // إعداد الـ Grid مرة واحدة
            SetupGridColumnsAndStyle();

            // ربط الأحداث هنا (بدون lambda داخل designer)
            WireEvents();
        }

        private void SmsLogsForm_Load(object sender, EventArgs e)
        {
            LoadSmsLogsDataFromDb();
        }

        private void WireEvents()
        {
            // Filters instant
            cmbStatusFilter.SelectedIndexChanged += CmbStatusFilter_SelectedIndexChanged;
            dateFromPicker.ValueChanged += DateFromPicker_ValueChanged;
            dateToPicker.ValueChanged += DateToPicker_ValueChanged;
            txtPhoneFilter.TextChanged += TxtPhoneFilter_TextChanged;
            txtSearchGeneral.TextChanged += TxtSearchGeneral_TextChanged;

            // Buttons
            btnRefresh.Click += BtnRefresh_Click;
            btnReset.Click += BtnReset_Click;
            btnResend.Click += ResendSMS_Click;
            btnExport.Click += ExportToExcel_Click;
            btnInvoiceDetails.Click += BtnInvoiceDetails_Click;

            // Grid events
            smsLogsDataGridView.CellFormatting += SmsLogsDataGridView_CellFormatting;
            smsLogsDataGridView.CellDoubleClick += SmsLogsDataGridView_CellDoubleClick;
            smsLogsDataGridView.SelectionChanged += SmsLogsDataGridView_SelectionChanged;
        }
        private void WireHover(Button b, Color normal, Color hover)
        {
            b.BackColor = normal;
            b.MouseEnter += (s, e) => b.BackColor = hover;
            b.MouseLeave += (s, e) => b.BackColor = normal;
        }

        private void SetupGridColumnsAndStyle()
        {
            // Styles
            smsLogsDataGridView.DefaultCellStyle.Font = new Font("Tahoma", 10);
            smsLogsDataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(220, 235, 252);
            smsLogsDataGridView.DefaultCellStyle.SelectionForeColor = Color.Black;
            smsLogsDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);

            smsLogsDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 62, 80);
            smsLogsDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            smsLogsDataGridView.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10, FontStyle.Bold);
            smsLogsDataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            smsLogsDataGridView.ColumnHeadersHeight = 44;
            smsLogsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            smsLogsDataGridView.RowTemplate.Height = 34;
            smsLogsDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            // Columns
            smsLogsDataGridView.Columns.Clear();

            smsLogsDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SmsID",
                DataPropertyName = "SmsID",
                HeaderText = "رقم الرسالة",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            smsLogsDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SubscriberName",
                DataPropertyName = "SubscriberName",
                HeaderText = "اسم المشترك",
                Width = 180,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            smsLogsDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "PhoneNumber",
                DataPropertyName = "PhoneNumber",
                HeaderText = "رقم الهاتف",
                Width = 140,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            smsLogsDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Message",
                DataPropertyName = "Message",
                HeaderText = "نص الرسالة",
                Width = 420,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    WrapMode = DataGridViewTriState.True,
                    Alignment = DataGridViewContentAlignment.MiddleRight
                }
            });

            smsLogsDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Status",
                DataPropertyName = "Status",
                HeaderText = "الحالة",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            smsLogsDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SentDate",
                DataPropertyName = "SentDate",
                HeaderText = "تاريخ الإرسال",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Format = "yyyy-MM-dd HH:mm"
                }
            });

            smsLogsDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "RetryCount",
                DataPropertyName = "RetryCount",
                HeaderText = "المحاولات",
                Width = 90,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            smsLogsDataGridView.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Reason",
                DataPropertyName = "Reason",
                HeaderText = "سبب الفشل",
                Width = 180,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight }
            });

            // Hidden cols
            smsLogsDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "InvoiceID", DataPropertyName = "InvoiceID", Visible = false });
            smsLogsDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "SubscriberID", DataPropertyName = "SubscriberID", Visible = false });
            smsLogsDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "CollectorID", DataPropertyName = "CollectorID", Visible = false });
            smsLogsDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "TemplateID", DataPropertyName = "TemplateID", Visible = false });
            smsLogsDataGridView.Columns.Add(new DataGridViewTextBoxColumn { Name = "CreatedAt", DataPropertyName = "CreatedAt", Visible = false });
        }

        // ===== Events for filters =====
        private void CmbStatusFilter_SelectedIndexChanged(object sender, EventArgs e) => ApplyFiltersInstant();
        private void DateFromPicker_ValueChanged(object sender, EventArgs e) => ApplyFiltersInstant();
        private void DateToPicker_ValueChanged(object sender, EventArgs e) => ApplyFiltersInstant();
        private void TxtPhoneFilter_TextChanged(object sender, EventArgs e) => ApplyFiltersInstant();
        private void TxtSearchGeneral_TextChanged(object sender, EventArgs e) => ApplyFiltersInstant();

        private void BtnRefresh_Click(object sender, EventArgs e) => LoadSmsLogsDataFromDb();
        private void BtnReset_Click(object sender, EventArgs e) => ResetFilter();

        private void SmsLogsDataGridView_SelectionChanged(object sender, EventArgs e) => UpdateInvoiceButtonState();

        // =========================
        // تحميل بيانات حقيقية من DB
        // =========================
        private void LoadSmsLogsDataFromDb()
        {
            try
            {
                smsLogsTable = new DataTable();

                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand(@"
SELECT
    L.SmsID,
    L.InvoiceID,
    L.SubscriberID,
    ISNULL(S.Name, N'') AS SubscriberName,
    L.PhoneNumber,
    L.Message,
    L.Status,
    L.Reason,
    L.SentDate,
    L.CollectorID,
    L.TemplateID,
    L.RetryCount,
    L.CreatedAt
FROM dbo.SmsLogs L
LEFT JOIN dbo.Subscribers S ON S.SubscriberID = L.SubscriberID
ORDER BY L.SmsID DESC;", con))
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
                MessageBox.Show($"خطأ في تحميل البيانات:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // فلترة لحظية
        // =========================
        private void ApplyFiltersInstant()
        {
            if (smsLogsView == null) return;

            try
            {
                List<string> parts = new List<string>();

                if (cmbStatusFilter.SelectedIndex > 0)
                {
                    string st = EscapeForRowFilter(cmbStatusFilter.SelectedItem.ToString());
                    parts.Add($"Status = '{st}'");
                }

                if (!string.IsNullOrWhiteSpace(txtPhoneFilter.Text))
                {
                    string phone = EscapeForRowFilter(txtPhoneFilter.Text.Trim());
                    parts.Add($"PhoneNumber LIKE '%{phone}%'");
                }

                if (!string.IsNullOrWhiteSpace(txtSearchGeneral.Text))
                {
                    string q = EscapeForRowFilter(txtSearchGeneral.Text.Trim());
                    parts.Add(
                        $"(SubscriberName LIKE '%{q}%' OR Message LIKE '%{q}%' " +
                        $"OR Convert(SmsID, 'System.String') LIKE '%{q}%' " +
                        $"OR Convert(InvoiceID, 'System.String') LIKE '%{q}%')"
                    );
                }

                DateTime from = dateFromPicker.Value.Date;
                DateTime to = dateToPicker.Value.Date.AddDays(1).AddTicks(-1);

                parts.Add($"SentDate >= #{from:MM/dd/yyyy}# AND SentDate <= #{to:MM/dd/yyyy HH:mm:ss}#");

                smsLogsView.RowFilter = string.Join(" AND ", parts);
                UpdateStatistics();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في التصفية:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string EscapeForRowFilter(string input) => input.Replace("'", "''");

        private void ResetFilter()
        {
            cmbStatusFilter.SelectedIndex = 0;
            txtPhoneFilter.Text = "";
            txtSearchGeneral.Text = "";
            dateFromPicker.Value = DateTime.Now.AddDays(-30);
            dateToPicker.Value = DateTime.Now;

            ApplyFiltersInstant();
        }

        // =========================
        // تنسيق الحالة
        // =========================
        private void SmsLogsDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (smsLogsDataGridView.Columns[e.ColumnIndex].Name == "Status")
            {
                var v = e.Value?.ToString()?.Trim().ToLower();
                if (string.IsNullOrEmpty(v)) return;

                if (v == "sent")
                {
                    e.Value = "ناجح";
                    e.CellStyle.ForeColor = Color.Green;
                    e.CellStyle.Font = new Font(smsLogsDataGridView.Font, FontStyle.Bold);
                }
                else if (v == "failed")
                {
                    e.Value = "فاشل";
                    e.CellStyle.ForeColor = Color.Red;
                    e.CellStyle.Font = new Font(smsLogsDataGridView.Font, FontStyle.Bold);
                }
                else if (v == "pending")
                {
                    e.Value = "معلّق";
                    e.CellStyle.ForeColor = Color.OrangeRed;
                    e.CellStyle.Font = new Font(smsLogsDataGridView.Font, FontStyle.Bold);
                }
            }
        }

        // =========================
        // إحصائيات
        // =========================
        private void UpdateStatistics()
        {
            if (smsLogsView == null)
            {
                statsLabel.Text = "الإحصائيات: إجمالي الرسائل: 0 | ناجح: 0 | فاشل: 0 | معلق: 0";
                return;
            }

            int total = smsLogsView.Count;
            int sent = 0, failed = 0, pending = 0;

            foreach (DataRowView row in smsLogsView)
            {
                string st = (row["Status"] + "").Trim();
                if (st == "Sent") sent++;
                else if (st == "Failed") failed++;
                else if (st == "Pending") pending++;
            }

            statsLabel.Text = $"الإحصائيات: إجمالي الرسائل: {total} | ناجح: {sent} | فاشل: {failed} | معلق: {pending}";
        }

        // =========================
        // Double click => Details
        // =========================
        private void SmsLogsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            ShowSmsDetails(smsLogsDataGridView.Rows[e.RowIndex]);
        }

        private void ShowSmsDetails(DataGridViewRow row)
        {
            try
            {
                string smsId = row.Cells["SmsID"].Value + "";
                string subName = row.Cells["SubscriberName"].Value + "";
                string phone = row.Cells["PhoneNumber"].Value + "";
                string msg = row.Cells["Message"].Value + "";
                string status = row.Cells["Status"].Value + "";
                string reason = row.Cells["Reason"].Value + "";
                string sentDate = row.Cells["SentDate"].Value + "";
                string invoiceId = row.Cells["InvoiceID"].Value + "";

                Form f = new Form
                {
                    Text = $"تفاصيل الرسالة رقم {smsId}",
                    Size = new Size(850, 520),
                    StartPosition = FormStartPosition.CenterParent,
                    Font = new Font("Tahoma", 10),
                    RightToLeft = RightToLeft.Yes,
                    RightToLeftLayout = true
                };

                TextBox txt = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    Dock = DockStyle.Fill,
                    ScrollBars = ScrollBars.Vertical,
                    Font = new Font("Tahoma", 10)
                };

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"رقم الرسالة: {smsId}");
                sb.AppendLine($"اسم المشترك: {subName}");
                sb.AppendLine($"رقم الهاتف: {phone}");
                sb.AppendLine($"الحالة: {status}");
                sb.AppendLine($"تاريخ الإرسال: {sentDate}");
                sb.AppendLine($"سبب الفشل: {reason}");
                sb.AppendLine($"InvoiceID: {invoiceId}");
                sb.AppendLine(new string('-', 50));
                sb.AppendLine("نص الرسالة:");
                sb.AppendLine(msg);

                txt.Text = sb.ToString();

                FlowLayoutPanel pnl = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    Height = 60,
                    FlowDirection = FlowDirection.LeftToRight,
                    WrapContents = false,
                    Padding = new Padding(10)
                };

                Button close = new Button
                {
                    Text = "إغلاق",
                    Height = 40,
                    Width = 120,
                    BackColor = Color.FromArgb(149, 165, 166),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                close.FlatAppearance.BorderSize = 0;
                close.Click += (s, e) => f.Close();

                Button invoiceDetails = new Button
                {
                    Text = "تفاصيل الفاتورة",
                    Height = 40,
                    Width = 140,
                    BackColor = Color.FromArgb(155, 89, 182),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                invoiceDetails.FlatAppearance.BorderSize = 0;

                if (int.TryParse(invoiceId, out int invId) && invId > 0)
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
                MessageBox.Show($"خطأ في عرض تفاصيل الرسالة:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // زر تفاصيل الفاتورة
        // =========================
        private void BtnInvoiceDetails_Click(object sender, EventArgs e)
        {
            if (smsLogsDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("يرجى تحديد رسالة أولاً.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = smsLogsDataGridView.SelectedRows[0];
            object v = row.Cells["InvoiceID"].Value;

            if (v == null || v == DBNull.Value || string.IsNullOrWhiteSpace(v.ToString()))
            {
                MessageBox.Show("هذه الرسالة غير مرتبطة بفاتورة.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!int.TryParse(v.ToString(), out int invoiceId) || invoiceId <= 0)
            {
                MessageBox.Show("رقم الفاتورة غير صحيح.", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ShowInvoiceDetails(invoiceId);
        }

        private void UpdateInvoiceButtonState()
        {
            if (smsLogsDataGridView.SelectedRows.Count == 0)
            {
                btnInvoiceDetails.Enabled = false;
                return;
            }

            var row = smsLogsDataGridView.SelectedRows[0];
            var v = row.Cells["InvoiceID"].Value;

            btnInvoiceDetails.Enabled = (v != null && v != DBNull.Value &&
                                         int.TryParse(v.ToString(), out int invId) && invId > 0);
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
                    Text = $"تفاصيل الفاتورة رقم {invoiceId}",
                    Size = new Size(900, 620),
                    StartPosition = FormStartPosition.CenterParent,
                    Font = new Font("Tahoma", 10),
                    RightToLeft = RightToLeft.Yes,
                    RightToLeftLayout = true
                };

                TextBox txt = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    Dock = DockStyle.Fill,
                    ScrollBars = ScrollBars.Vertical,
                    Font = new Font("Tahoma", 10)
                };

                string Safe(string col) => r.Table.Columns.Contains(col) ? (r[col] == DBNull.Value ? "" : r[col].ToString()) : "";

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"رقم الفاتورة: {Safe("InvoiceID")}");
                sb.AppendLine($"رقم المشترك: {Safe("SubscriberID")}");
                sb.AppendLine($"اسم المشترك: {Safe("SubscriberName")}");
                sb.AppendLine($"رقم العداد: {Safe("MeterNumber")}");
                sb.AppendLine($"الهاتف: {Safe("PhoneNumber")}");
                sb.AppendLine($"العنوان: {Safe("Address")}");
                sb.AppendLine(new string('-', 55));
                sb.AppendLine($"تاريخ الفاتورة: {Safe("InvoiceDate")}");
                sb.AppendLine($"الاستهلاك: {Safe("Consumption")}");
                sb.AppendLine($"سعر الوحدة: {Safe("UnitPrice")}");
                sb.AppendLine($"رسوم الخدمة: {Safe("ServiceFees")}");
                sb.AppendLine($"المتأخرات (حقيقية): {Safe("Arrears")}");
                sb.AppendLine(new string('-', 55));
                sb.AppendLine($"إجمالي الفاتورة: {Safe("TotalAmount")}");
                sb.AppendLine($"المدفوع: {Safe("Paid")}");
                sb.AppendLine($"المتبقي الحالي: {Safe("CurrentDue")}");
                sb.AppendLine($"الإجمالي المستحق: {Safe("TotalDue")}");
                sb.AppendLine($"الحالة: {Safe("Status")}");
                sb.AppendLine(new string('-', 55));
                sb.AppendLine($"قراءة سابقة: {Safe("PreviousReading")}");
                sb.AppendLine($"قراءة حالية: {Safe("CurrentReading")}");
                sb.AppendLine($"تاريخ القراءة: {Safe("ReadingDate")}");
                sb.AppendLine(new string('-', 55));
                sb.AppendLine($"ملاحظات: {Safe("Notes")}");

                txt.Text = sb.ToString();

                Button close = new Button
                {
                    Text = "إغلاق",
                    Dock = DockStyle.Bottom,
                    Height = 45,
                    BackColor = Color.FromArgb(149, 165, 166),
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
                MessageBox.Show($"خطأ في عرض تفاصيل الفاتورة:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private DataTable GetInvoiceDetailsFromDb(int invoiceId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("dbo.GetInvoiceDetails", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@InvoiceID", invoiceId);

                DataTable dt = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    da.Fill(dt);

                return dt;
            }
        }

        // =========================
        // إعادة إرسال
        // =========================
        private void ResendSMS_Click(object sender, EventArgs e)
        {
            if (smsLogsDataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show("يرجى تحديد رسائل لإعادة الإرسال", "تحذير",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"هل تريد إعادة إرسال {smsLogsDataGridView.SelectedRows.Count} رسالة؟",
                "تأكيد إعادة الإرسال",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            try
            {
                List<int> idsToUpdate = new List<int>();

                foreach (DataGridViewRow row in smsLogsDataGridView.SelectedRows)
                {
                    string status = row.Cells["Status"].Value + "";
                    if (status == "Failed")
                    {
                        if (int.TryParse(row.Cells["SmsID"].Value.ToString(), out int id))
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
SET RetryCount = RetryCount + 1,
    Status = 'Pending',
    Reason = NULL
WHERE SmsID = @SmsID;", con))
                        {
                            cmd.Parameters.AddWithValue("@SmsID", id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                LoadSmsLogsDataFromDb();

                MessageBox.Show("تم تحويل الرسائل الفاشلة إلى (معلّق) لإعادة الإرسال.", "نجاح",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إعادة الإرسال:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =========================
        // تصدير Excel (مكانه جاهز)
        // =========================
        private void ExportToExcel_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "ملفات Excel (*.xlsx)|*.xlsx|جميع الملفات (*.*)|*.*",
                    FileName = $"SMS_Logs_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx",
                    Title = "تصدير سجلات الرسائل"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("تم اختيار ملف التصدير. (ميزة التصدير الفعلي تحتاج مكتبة مثل ClosedXML).", "معلومة",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في التصدير:\n{ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
