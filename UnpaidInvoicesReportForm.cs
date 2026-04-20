using Microsoft.Reporting.WinForms;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using water3.Reports;

namespace water3
{
    public partial class UnpaidInvoicesReportForm : Form
    {
        private readonly Color SearchNormal = Color.FromArgb(0, 106, 204);
        private readonly Color SearchHover = Color.FromArgb(0, 87, 183);

        private readonly Color ExportNormal = Color.FromArgb(80, 199, 110);
        private readonly Color ExportHover = Color.FromArgb(56, 170, 90);

        private readonly Color ReportNormal = Color.FromArgb(0, 170, 170);
        private readonly Color ReportHover = Color.FromArgb(0, 140, 140);

        public UnpaidInvoicesReportForm()
        {
            InitializeComponent();

            // خصائص عامة للفورم
            Text = "تقرير الفواتير غير المدفوعة/المتأخرة";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;

            // ربط Hover هنا (بدل InitializeComponent)
            WireHoverEffects();
        }
        //public UnpaidInvoicesReportForm()
        //{
        //    InitializeComponent();

        //    // خصائص عامة للفورم (ممكن تخليها في الديزاينر أيضاً)
        //    Text = "تقرير الفواتير غير المدفوعة/المتأخرة";
        //    RightToLeft = RightToLeft.Yes;
        //    RightToLeftLayout = true;
        //    BackColor = Color.White;
        //    FormBorderStyle = FormBorderStyle.None;
        //    Dock = DockStyle.Fill;

        //    // قيم افتراضية
        //    ddlStatus.SelectedIndex = 0;
        //    dtFrom.Value = DateTime.Today.AddMonths(-1);
        //    dtTo.Value = DateTime.Today;
        //}
        private void WireHoverEffects()
        {
            // Search
            btnSearch.BackColor = SearchNormal;
            btnSearch.MouseEnter += BtnSearch_MouseEnter;
            btnSearch.MouseLeave += BtnSearch_MouseLeave;

            // Export
            btnExport.BackColor = ExportNormal;
            btnExport.MouseEnter += BtnExport_MouseEnter;
            btnExport.MouseLeave += BtnExport_MouseLeave;

            // Report
            btnShowReport.BackColor = ReportNormal;
            btnShowReport.MouseEnter += BtnShowReport_MouseEnter;
            btnShowReport.MouseLeave += BtnShowReport_MouseLeave;
        }

        private void BtnSearch_MouseEnter(object sender, EventArgs e) => btnSearch.BackColor = SearchHover;
        private void BtnSearch_MouseLeave(object sender, EventArgs e) => btnSearch.BackColor = SearchNormal;

        private void BtnExport_MouseEnter(object sender, EventArgs e) => btnExport.BackColor = ExportHover;
        private void BtnExport_MouseLeave(object sender, EventArgs e) => btnExport.BackColor = ExportNormal;

        private void BtnShowReport_MouseEnter(object sender, EventArgs e) => btnShowReport.BackColor = ReportHover;
        private void BtnShowReport_MouseLeave(object sender, EventArgs e) => btnShowReport.BackColor = ReportNormal;
    
private void UnpaidInvoicesReportForm_Load(object sender, EventArgs e)
        {
            LoadSubscribers();
            LoadUnpaidInvoices();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadUnpaidInvoices();
        }

        private void LoadSubscribers()
        {
            ddlSubscribers.Items.Clear();
            ddlSubscribers.Items.Add(new ComboBoxItem("الكل", "0"));

            using (SqlConnection con = Db.GetConnection())
            {
                var da = new SqlDataAdapter("SELECT SubscriberID, Name FROM Subscribers", con);
                var dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                    ddlSubscribers.Items.Add(new ComboBoxItem(row["Name"].ToString(), row["SubscriberID"].ToString()));
            }

            ddlSubscribers.SelectedIndex = 0;
        }

        private void LoadUnpaidInvoices()
        {
            string statusFilter = "";
            if (ddlStatus.SelectedIndex == 1)
                statusFilter = " AND I.Status = 'Unpaid'";
            else if (ddlStatus.SelectedIndex == 2)
                statusFilter = " AND I.Status = 'Partial'";

            string subscriberFilter = "";
            ComboBoxItem selectedSubscriber = null;

            if (ddlSubscribers.SelectedIndex > 0 && ddlSubscribers.SelectedItem is ComboBoxItem selItem)
            {
                subscriberFilter = " AND I.SubscriberID = @SubscriberID";
                selectedSubscriber = selItem;
            }

            string query = $@"
                SELECT 
                    I.InvoiceID, 
                    S.Name AS [اسم المشترك], 
                    S.MeterNumber AS [رقم العداد], 
                    S.Address AS [العنوان],
                    S.PhoneNumber AS [الهاتف], 
                    I.InvoiceDate AS [تاريخ الفاتورة], 
                    I.Consumption AS [الاستهلاك], 
                    I.UnitPrice AS [سعر الوحدة], 
                    I.ServiceFees AS [رسوم الخدمة], 
                    I.Arrears AS [المتأخرات],
                    I.TotalAmount AS [الإجمالي], 
                    I.Status AS [الحالة]
                FROM Invoices I
                INNER JOIN Subscribers S ON I.SubscriberID = S.SubscriberID
                WHERE (I.Status = 'Unpaid' OR I.Status = 'Partial')
                AND I.InvoiceDate BETWEEN @DateFrom AND @DateTo
                {statusFilter}
                {subscriberFilter}
                ORDER BY I.InvoiceDate DESC, I.InvoiceID DESC
            ";

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(query, con))
            {
                cmd.Parameters.AddWithValue("@DateFrom", dtFrom.Value.Date);
                cmd.Parameters.AddWithValue("@DateTo", dtTo.Value.Date);

                if (ddlSubscribers.SelectedIndex > 0 && selectedSubscriber != null)
                    cmd.Parameters.AddWithValue("@SubscriberID", selectedSubscriber.Value);

                var da = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                da.Fill(dt);

                dgv.DataSource = dt;
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dgv.DataSource == null || dgv.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للتصدير!");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel Files|*.csv",
                FileName = "UnpaidInvoices.csv"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                var dt = (DataTable)dgv.DataSource;
                var sb = new System.Text.StringBuilder();

                for (int i = 0; i < dt.Columns.Count; i++)
                    sb.Append(dt.Columns[i].ColumnName + (i == dt.Columns.Count - 1 ? "\n" : ","));

                foreach (DataRow row in dt.Rows)
                {
                    for (int i = 0; i < dt.Columns.Count; i++)
                        sb.Append(row[i]?.ToString().Replace(",", " ") + (i == dt.Columns.Count - 1 ? "\n" : ","));
                }

                System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), System.Text.Encoding.UTF8);
                MessageBox.Show("تم تصدير البيانات بنجاح!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطأ في التصدير: " + ex.Message);
            }
        }

        private void btnShowReport_Click(object sender, EventArgs e)
        {
            if (dgv.DataSource == null || dgv.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات لعرض التقرير!");
                return;
            }

            ShowReport();
        }

        private void ShowReport()
        {
            var src = dgv.DataSource as DataTable;
            if (src == null || src.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات لعرض التقرير!");
                return;
            }

            DataTable ds = BuildDsReportFromUnpaidInvoices(src);

            var opt = new ReportPresetOptions
            {
                Title = "تقرير الفواتير غير المدفوعة/المتأخرة",
                SubTitle = "نظام المياه",
                ShowTotals = true,
                LogoPath = @"C:\WaterApp\logo.png",
                FooterNote = "هذا التقرير صادر من نظام المياه"
            };

            opt.Columns.Add(new ReportPresetOptions.ColumnOption { Key = "Col1", Caption = "اسم المشترك", Visible = true, Order = 1 });
            opt.Columns.Add(new ReportPresetOptions.ColumnOption { Key = "Col2", Caption = "رقم العداد", Visible = true, Order = 2 });
            opt.Columns.Add(new ReportPresetOptions.ColumnOption { Key = "Col3", Caption = "العنوان", Visible = true, Order = 3 });
            opt.Columns.Add(new ReportPresetOptions.ColumnOption { Key = "Col4", Caption = "الهاتف", Visible = true, Order = 4 });
            opt.Columns.Add(new ReportPresetOptions.ColumnOption { Key = "Col5", Caption = "تاريخ الفاتورة", Visible = true, Order = 5 });
            opt.Columns.Add(new ReportPresetOptions.ColumnOption { Key = "Col6", Caption = "الحالة", Visible = true, Order = 6 });

            opt.Columns.Add(new ReportPresetOptions.ColumnOption { Key = "Num1", Caption = "الاستهلاك", Visible = true, Order = 7 });
            opt.Columns.Add(new ReportPresetOptions.ColumnOption { Key = "Num2", Caption = "سعر الوحدة", Visible = true, Order = 8 });
            opt.Columns.Add(new ReportPresetOptions.ColumnOption { Key = "Num3", Caption = "رسوم الخدمة", Visible = true, Order = 9 });
            opt.Columns.Add(new ReportPresetOptions.ColumnOption { Key = "Num4", Caption = "المتأخرات", Visible = true, Order = 10 });
            opt.Columns.Add(new ReportPresetOptions.ColumnOption { Key = "Num5", Caption = "الإجمالي", Visible = true, Order = 11 });

            using (Form reportForm = new Form()
            {
                Width = 1100,
                Height = 800,
                Text = "تقرير RDLC - الفواتير غير المدفوعة/المتأخرة",
                StartPosition = FormStartPosition.CenterScreen
            })
            {
                ReportViewer rv = new ReportViewer() { Dock = DockStyle.Fill };
                reportForm.Controls.Add(rv);

                string rdlcPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports", "Report_final.rdlc");
                if (!System.IO.File.Exists(rdlcPath))
                {
                    MessageBox.Show("لم يتم العثور على ملف التقرير:\n" + rdlcPath +
                        "\n\nتأكد من Copy to Output Directory لملف Report_final.rdlc",
                        "RDLC", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ReportRunner.Show(rv, rdlcPath, ds, opt, dtFrom.Value.Date, dtTo.Value.Date, "المستخدم");
                reportForm.ShowDialog();
            }
        }

        private DataTable BuildDsReportFromUnpaidInvoices(DataTable src)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Dt1", typeof(DateTime));

            for (int i = 1; i <= 10; i++) dt.Columns.Add("Col" + i, typeof(string));
            for (int i = 1; i <= 5; i++) dt.Columns.Add("Num" + i, typeof(decimal));

            dt.Columns.Add("RowId", typeof(int));
            dt.Columns.Add("RefId1", typeof(int));
            dt.Columns.Add("RefId2", typeof(int));

            foreach (DataRow r in src.Rows)
            {
                DataRow nr = dt.NewRow();

                DateTime invDate;
                if (DateTime.TryParse(Convert.ToString(r["تاريخ الفاتورة"]), out invDate))
                    nr["Dt1"] = invDate;
                else
                    nr["Dt1"] = DateTime.Today;

                nr["Col1"] = Convert.ToString(r["اسم المشترك"]);
                nr["Col2"] = Convert.ToString(r["رقم العداد"]);
                nr["Col3"] = Convert.ToString(r["العنوان"]);
                nr["Col4"] = Convert.ToString(r["الهاتف"]);
                nr["Col5"] = invDate.ToString("yyyy-MM-dd");
                nr["Col6"] = Convert.ToString(r["الحالة"]);

                nr["Num1"] = ToDec(r["الاستهلاك"]);
                nr["Num2"] = ToDec(r["سعر الوحدة"]);
                nr["Num3"] = ToDec(r["رسوم الخدمة"]);
                nr["Num4"] = ToDec(r["المتأخرات"]);
                nr["Num5"] = ToDec(r["الإجمالي"]);

                nr["RowId"] = ToInt(r["InvoiceID"]);
                nr["RefId1"] = 0;
                nr["RefId2"] = 0;

                dt.Rows.Add(nr);
            }

            return dt;
        }

        private decimal ToDec(object v)
        {
            if (v == null || v == DBNull.Value) return 0m;
            return decimal.TryParse(v.ToString(), out var d) ? d : 0m;
        }

        private int ToInt(object v)
        {
            if (v == null || v == DBNull.Value) return 0;
            return int.TryParse(v.ToString(), out var x) ? x : 0;
        }

        // Helper ComboBox item
        private class ComboBoxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public ComboBoxItem(string text, string value) { Text = text; Value = value; }
            public override string ToString() => Text;
        }
    }
}


/*using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace water3
{
    public partial class UnpaidInvoicesReportForm : Form
    {
        ComboBox ddlStatus, ddlSubscribers;
        DateTimePicker dtFrom, dtTo;
        DataGridView dgv;
        Button btnSearch;

         

        public UnpaidInvoicesReportForm()
        {
            this.Text = "تقرير الفواتير غير المدفوعة/المتأخرة";
            this.Size = new Size(1100, 650);

            Label lblSubscriber = new Label() { Text = "المشترك:", Left = 25, Top = 30 };
            ddlSubscribers = new ComboBox() { Left = 95, Top = 27, Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            Label lblStatus = new Label() { Text = "الحالة:", Left = 300, Top = 30 };
            ddlStatus = new ComboBox() { Left = 360, Top = 27, Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            ddlStatus.Items.AddRange(new object[] { "الكل", "غير مدفوعة", "مدفوعة جزئياً" });
            ddlStatus.SelectedIndex = 0;

            Label lblFrom = new Label() { Text = "من تاريخ:", Left = 500, Top = 30 };
            dtFrom = new DateTimePicker() { Left = 570, Top = 27, Width = 120, Format = DateTimePickerFormat.Short };
            Label lblTo = new Label() { Text = "إلى تاريخ:", Left = 700, Top = 30 };
            dtTo = new DateTimePicker() { Left = 770, Top = 27, Width = 120, Format = DateTimePickerFormat.Short };
            btnSearch = new Button() { Text = "تحديث التقرير", Left = 920, Top = 25, Width = 120, Height = 34, BackColor = Color.LightSkyBlue };
            btnSearch.Click += BtnSearch_Click;

            dgv = new DataGridView()
            {
                Location = new Point(25, 80),
                Width = 1010,
                Height = 500,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            };

            this.Controls.AddRange(new Control[] {
                lblSubscriber, ddlSubscribers,
                lblStatus, ddlStatus,
                lblFrom, dtFrom, lblTo, dtTo,
                btnSearch, dgv
            });

            LoadSubscribers();
            dtFrom.Value = DateTime.Today.AddMonths(-1);
            dtTo.Value = DateTime.Today;

            LoadUnpaidInvoices();
        }

        private void LoadSubscribers()
        {
            ddlSubscribers.Items.Clear();
            ddlSubscribers.Items.Add(new ComboBoxItem("الكل", "0"));
            using (SqlConnection con = Db.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT SubscriberID, Name FROM Subscribers", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow row in dt.Rows)
                    ddlSubscribers.Items.Add(new ComboBoxItem(row["Name"].ToString(), row["SubscriberID"].ToString()));
            }
            ddlSubscribers.SelectedIndex = 0;
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadUnpaidInvoices();
        }

        private void LoadUnpaidInvoices()
        {
            string statusFilter = "";
            if (ddlStatus.SelectedIndex == 1)
                statusFilter = " AND I.Status = 'Unpaid'";
            else if (ddlStatus.SelectedIndex == 2)
                statusFilter = " AND I.Status = 'Partial'";

            string subscriberFilter = "";
            ComboBoxItem selectedSubscriber = null;
            if (ddlSubscribers.SelectedIndex > 0 && ddlSubscribers.SelectedItem is ComboBoxItem selItem)
            {
                subscriberFilter = " AND I.SubscriberID = @SubscriberID";
                selectedSubscriber = selItem;
            }

            string query = $@"
        SELECT 
            I.InvoiceID, S.Name AS SubscriberName, S.MeterNumber, S.Address,S.PhoneNumber,
            I.InvoiceDate, I.Consumption, I.UnitPrice, I.ServiceFees, I.Arrears,
            I.TotalAmount, I.Status
        FROM Invoices I
        INNER JOIN Subscribers S ON I.SubscriberID = S.SubscriberID
        WHERE (I.Status = 'Unpaid' OR I.Status = 'Partial')
        AND I.InvoiceDate BETWEEN @DateFrom AND @DateTo
        {statusFilter}
        {subscriberFilter}
        ORDER BY I.InvoiceDate DESC, I.InvoiceID DESC
    ";

            using (SqlConnection con = Db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@DateFrom", dtFrom.Value.Date);
                cmd.Parameters.AddWithValue("@DateTo", dtTo.Value.Date);
                if (selectedSubscriber != null)
                    cmd.Parameters.AddWithValue("@SubscriberID", selectedSubscriber.Value);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
            }
        }


        // Helper ComboBox item
        class ComboBoxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public ComboBoxItem(string text, string value) { Text = text; Value = value; }
            public override string ToString() { return Text; }
        }
    }
}
*/