using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace water3
{
    public partial class CollectorsReportForm : Form
    {
        DateTimePicker dtFrom, dtTo;
        Button btnShow;
        DataGridView dgv;
        Label lblSummary;
         

        public CollectorsReportForm()
        {
            this.Text = "تقرير تحصيلات المحصلين";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Dock = DockStyle.Fill;

            // TableLayoutPanel رئيسي
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(18, 14, 18, 14),
                BackColor = Color.White
            };
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58)); // شريط التصفية
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // الجدول
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44)); // التذييل

            // شريط الفلاتر العلوي
            FlowLayoutPanel topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Padding = new Padding(0, 10, 0, 0),
                BackColor = Color.White
            };

            topPanel.Controls.Add(new Label { Text = "من تاريخ:", AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), TextAlign = ContentAlignment.MiddleRight });
            dtFrom = new DateTimePicker() { Width = 120, Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 11) };
            topPanel.Controls.Add(dtFrom);

            topPanel.Controls.Add(new Label { Text = "إلى تاريخ:", AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), TextAlign = ContentAlignment.MiddleRight, Margin = new Padding(18, 0, 0, 0) });
            dtTo = new DateTimePicker() { Width = 120, Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 11) };
            topPanel.Controls.Add(dtTo);

            btnShow = new Button()
            {
                Text = "عرض التقرير",
                Height = 30,
                Width = 120,
                BackColor = Color.FromArgb(80, 199, 110),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(18, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            btnShow.FlatAppearance.BorderSize = 0;
            btnShow.MouseEnter += (s, e) => btnShow.BackColor = Color.FromArgb(56, 170, 90);
            btnShow.MouseLeave += (s, e) => btnShow.BackColor = Color.FromArgb(80, 199, 110);
            btnShow.Click += BtnShow_Click;
            topPanel.Controls.Add(btnShow);

            // جدول البيانات
            dgv = new DataGridView()
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 11),
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                RowTemplate = { Height = 36 }
            };
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 87, 183);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 11);
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 232, 255);
            dgv.RowsDefaultCellStyle.BackColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 247, 255);

            // ملخص تحت الجدول (إجمالي)
            lblSummary = new Label()
            {
                Dock = DockStyle.Fill,
                Height = 44,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(8, 0, 0, 0),
                BackColor = Color.FromArgb(247, 251, 255)
            };

            mainLayout.Controls.Add(topPanel, 0, 0);
            mainLayout.Controls.Add(dgv, 0, 1);
            mainLayout.Controls.Add(lblSummary, 0, 2);

            this.Controls.Add(mainLayout);

            // القيم الافتراضية
            dtFrom.Value = DateTime.Today.AddMonths(-1);
            dtTo.Value = DateTime.Today;

            LoadCollectorsReport();
        }

        private void BtnShow_Click(object sender, EventArgs e)
        {
            LoadCollectorsReport();
        }

        private void LoadCollectorsReport()
        {
            string sql = @"
                SELECT C.Name AS [اسم المحصل], 
                       COUNT(P.PaymentID) AS [عدد الدفعات],
                       ISNULL(SUM(P.Amount),0) AS [إجمالي المحصل]
                FROM Payments P
                INNER JOIN Collectors C ON P.CollectorID = C.CollectorID
                WHERE P.PaymentDate BETWEEN @from AND @to
                GROUP BY C.Name
                ORDER BY [إجمالي المحصل] DESC";
            using (SqlConnection con = Db.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter(sql, con);
                da.SelectCommand.Parameters.AddWithValue("@from", dtFrom.Value.Date);
                da.SelectCommand.Parameters.AddWithValue("@to", dtTo.Value.Date);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;

                // ملخص إجمالي التحصيلات
                decimal total = 0;
                foreach (DataRow row in dt.Rows)
                {
                    total += Convert.ToDecimal(row["إجمالي المحصل"]);
                }
                lblSummary.Text = $"إجمالي كل التحصيلات خلال الفترة: {total:N2} ريال";
            }
        }


        private void CollectorsReportForm_Load(object sender, EventArgs e)
        {

        }

    }
}