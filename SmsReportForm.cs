using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace water3
{
    public partial class SmsReportForm : Form
    {
        public SmsReportForm()
        {
            InitializeComponent();

            Text = "تقرير SMS حسب المحصل";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            WindowState = FormWindowState.Maximized;

            ApplyTheme();

            // Events
            btnLoad.Click += (s, e) => LoadReport();
            Load += SmsReportForm_Load;
        }

        private void SmsReportForm_Load(object sender, EventArgs e)
        {
            dtFrom.Value = DateTime.Today.AddDays(-7);
            dtTo.Value = DateTime.Today;

            // fill status (مهم هنا بدل ما يكون بالـDesigner عشان أي تعديل لاحق أسهل)
            ddlStatus.Items.Clear();
            ddlStatus.Items.AddRange(new object[] { "الكل", "Sent", "Skipped", "Failed" });
            ddlStatus.SelectedIndex = 0;

            LoadCollectors();
            LoadReport();
        }

        private void ApplyTheme()
        {
            // زر عرض
            btnLoad.BackColor = Color.DarkGreen;
            btnLoad.ForeColor = Color.White;
            btnLoad.FlatStyle = FlatStyle.Flat;
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.Cursor = Cursors.Hand;

            // Counters
            lblSent.ForeColor = Color.Green;
            lblSent.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            lblFailed.ForeColor = Color.Red;
            lblFailed.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            lblSkipped.ForeColor = Color.DarkOrange;
            lblSkipped.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            lblTotalAmount.ForeColor = Color.Blue;
            lblTotalAmount.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // Grid
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 246, 248);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 40;

            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(225, 235, 250);
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgv.RowTemplate.Height = 34;
        }

        // ===== تحميل المحصلين =====
        private void LoadCollectors()
        {
            ddlCollectors.Items.Clear();
            ddlCollectors.Items.Add(new ComboItem("الكل", 0));

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("SELECT CollectorID, Name FROM Collectors ORDER BY Name", con))
            {
                con.Open();
                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        ddlCollectors.Items.Add(
                            new ComboItem(dr["Name"].ToString(), Convert.ToInt32(dr["CollectorID"])));
                    }
                }
            }

            ddlCollectors.SelectedIndex = 0;
        }

        // ===== تحميل التقرير =====
        private void LoadReport()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = Db.GetConnection())
            {
                string sql = @"
SELECT
    L.SentDate AS [التاريخ],
    S.Name AS [المشترك],
    C.Name AS [المحصل],
    L.PhoneNumber AS [الهاتف],
    L.InvoiceID AS [الفاتورة],
    ISNULL(I.TotalAmount,0) AS [مبلغ الفاتورة],
    ISNULL(L.Status,'Skipped') AS [الحالة],
    ISNULL(L.Reason,'') AS [السبب]
FROM SmsLogs L
LEFT JOIN Subscribers S ON L.SubscriberID = S.SubscriberID
LEFT JOIN Collectors C ON L.CollectorID = C.CollectorID
LEFT JOIN Invoices I ON L.InvoiceID = I.InvoiceID
WHERE L.SentDate BETWEEN @From AND @To
";

                if (ddlStatus.SelectedIndex > 0)
                    sql += " AND L.Status = @Status";

                ComboItem col = ddlCollectors.SelectedItem as ComboItem;
                if (col != null && col.Value > 0)
                    sql += " AND L.CollectorID = @CollectorID";

                sql += " ORDER BY L.SentDate DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.SelectCommand.Parameters.AddWithValue("@From", dtFrom.Value.Date);
                da.SelectCommand.Parameters.AddWithValue("@To", dtTo.Value.Date.AddDays(1));

                if (ddlStatus.SelectedIndex > 0)
                    da.SelectCommand.Parameters.AddWithValue("@Status", ddlStatus.SelectedItem.ToString());

                if (col != null && col.Value > 0)
                    da.SelectCommand.Parameters.AddWithValue("@CollectorID", col.Value);

                da.Fill(dt);
            }

            dgv.DataSource = dt;

            Colorize();
            UpdateCounters();
        }

        // ===== تلوين الصفوف =====
        private void Colorize()
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                string status = row.Cells["الحالة"].Value?.ToString() ?? "";

                if (status == "Sent")
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                else if (status == "Skipped")
                    row.DefaultCellStyle.BackColor = Color.Khaki;
                else if (status == "Failed")
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                else
                    row.DefaultCellStyle.BackColor = Color.White;
            }
        }

        // ===== تحديث العدّادات =====
        private void UpdateCounters()
        {
            int sent = 0, failed = 0, skipped = 0;
            decimal totalAmount = 0;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                string status = row.Cells["الحالة"].Value?.ToString() ?? "";
                decimal amount = 0;
                decimal.TryParse(row.Cells["مبلغ الفاتورة"].Value?.ToString(), out amount);

                totalAmount += amount;

                switch (status)
                {
                    case "Sent": sent++; break;
                    case "Failed": failed++; break;
                    default: skipped++; break;
                }
            }

            lblSent.Text = $"Sent: {sent}";
            lblFailed.Text = $"Failed: {failed}";
            lblSkipped.Text = $"Skipped: {skipped}";
            lblTotalAmount.Text = $"إجمالي المبالغ: {totalAmount:N2}";
        }
    }

    // ===== ComboBox Item =====
    public class ComboItem
    {
        public string Text { get; set; }
        public int Value { get; set; }

        public ComboItem(string text, int value)
        {
            Text = text;
            Value = value;
        }

        public override string ToString() => Text;
    }
}

/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;

namespace water3
{

    public partial class SmsReportForm : Form
    {


         

        DateTimePicker dtFrom, dtTo;
        ComboBox ddlStatus, ddlCollectors;
        Button btnLoad;
        DataGridView dgv;

        public SmsReportForm()
        {
            Text = "تقرير SMS حسب المحصل";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            WindowState = FormWindowState.Maximized;

            var panelTop = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 70,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(10)
            };

            dtFrom = new DateTimePicker { Width = 120, Format = DateTimePickerFormat.Short };
            dtTo = new DateTimePicker { Width = 120, Format = DateTimePickerFormat.Short };

            ddlStatus = new ComboBox { Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            ddlStatus.Items.AddRange(new object[] { "الكل", "Sent", "Skipped", "Failed" });
            ddlStatus.SelectedIndex = 0;

            ddlCollectors = new ComboBox { Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };

            btnLoad = new Button
            {
                Text = "عرض",
                Width = 100,
                BackColor = Color.DarkGreen,
                ForeColor = Color.White
            };
            btnLoad.Click += (s, e) => LoadReport();

            panelTop.Controls.AddRange(new Control[]
            {
            new Label{ Text="من" }, dtFrom,
            new Label{ Text="إلى" }, dtTo,
            new Label{ Text="الحالة" }, ddlStatus,
            new Label{ Text="المحصل" }, ddlCollectors,
            btnLoad
            });

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };

            Controls.Add(dgv);
            Controls.Add(panelTop);

            Load += (s, e) =>
            {
                dtFrom.Value = DateTime.Today.AddDays(-7);
                dtTo.Value = DateTime.Today;
                LoadCollectors();
                LoadReport();
            };
        }

        // ===== تحميل المحصلين =====
        private void LoadCollectors()
        {
            ddlCollectors.Items.Clear();
            ddlCollectors.Items.Add(new ComboItem("الكل", 0));

            using (SqlConnection con = Db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand("SELECT CollectorID, Name FROM Collectors", con);
                con.Open();
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    ddlCollectors.Items.Add(
                        new ComboItem(dr["Name"].ToString(), Convert.ToInt32(dr["CollectorID"])));
                }
            }
            ddlCollectors.SelectedIndex = 0;
        }

        // ===== تحميل التقرير =====
        private void LoadReport()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = Db.GetConnection())
            {
                string sql = @"
            SELECT
                L.SentDate     AS [التاريخ],
                S.Name         AS [المشترك],
                C.Name         AS [المحصل],
                L.PhoneNumber AS [الهاتف],
                L.InvoiceID   AS [الفاتورة],
                L.Status      AS [الحالة],
                L.Reason      AS [السبب]
            FROM SmsLogs L
            LEFT JOIN Subscribers S ON L.SubscriberID = S.SubscriberID
            LEFT JOIN Collectors C ON L.CollectorID = C.CollectorID
            WHERE L.SentDate BETWEEN @From AND @To
            ";

                if (ddlStatus.SelectedIndex > 0)
                    sql += " AND L.Status = @Status";

                ComboItem col = ddlCollectors.SelectedItem as ComboItem;
                if (col.Value > 0)
                    sql += " AND L.CollectorID = @CollectorID";

                sql += " ORDER BY L.SentDate DESC";

                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.SelectCommand.Parameters.AddWithValue("@From", dtFrom.Value.Date);
                da.SelectCommand.Parameters.AddWithValue("@To", dtTo.Value.Date.AddDays(1));

                if (ddlStatus.SelectedIndex > 0)
                    da.SelectCommand.Parameters.AddWithValue("@Status", ddlStatus.SelectedItem.ToString());

                if (col.Value > 0)
                    da.SelectCommand.Parameters.AddWithValue("@CollectorID", col.Value);

                da.Fill(dt);
            }

            dgv.DataSource = dt;
            Colorize();
        }

        // ===== تلوين الصفوف =====
        private void Colorize()
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                var cell = row.Cells["الحالة"];

                if (cell == null || cell.Value == null)
                    continue;

                string status = cell.Value.ToString();

                if (status == "Sent")
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                else if (status == "Skipped")
                    row.DefaultCellStyle.BackColor = Color.Khaki;
                else if (status == "Failed")
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
            }
        }

    }
    
    public class ComboItem
    {
        public string Text { get; set; }
        public int Value { get; set; }

        public ComboItem(string text, int value)
        {
            Text = text;
            Value = value;
        }

        public override string ToString() => Text;
    }
}
*/
