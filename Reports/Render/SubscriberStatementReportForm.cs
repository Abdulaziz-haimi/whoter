using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace water3.Reports.Render
{
    public partial class SubscriberStatementReportForm : Form
    {
        ComboBox cmbSubscriber, cmbFilter;
        DateTimePicker dtFrom, dtTo;
        Button btnLoad, btnPrint, btnExport;
        WebBrowser browser;

         

        public SubscriberStatementReportForm()
        {
            Text = "كشف حساب مشترك";
            Size = new Size(1100, 700);
            StartPosition = FormStartPosition.CenterScreen;

            BuildUI();
            LoadSubscribers();
        }

        #region UI
        void BuildUI()
        {
            Panel top = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.WhiteSmoke };

            // اسم المشترك
            top.Controls.Add(new Label { Text = "المشترك:", Location = new Point(10, 20), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            cmbSubscriber = new ComboBox
            {
                Location = new Point(70, 16),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDown
            };
            cmbSubscriber.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbSubscriber.AutoCompleteSource = AutoCompleteSource.ListItems;
            top.Controls.Add(cmbSubscriber);

            // من تاريخ
            top.Controls.Add(new Label { Text = "من:", Location = new Point(290, 20), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            dtFrom = new DateTimePicker { Location = new Point(320, 16), Width = 120, Format = DateTimePickerFormat.Short };
            top.Controls.Add(dtFrom);

            // إلى تاريخ
            top.Controls.Add(new Label { Text = "إلى:", Location = new Point(460, 20), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            dtTo = new DateTimePicker { Location = new Point(490, 16), Width = 120, Format = DateTimePickerFormat.Short };
            top.Controls.Add(dtTo);

            // فلترة (الكل / فواتير / دفعات)
            top.Controls.Add(new Label { Text = "فلترة:", Location = new Point(630, 20), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
            cmbFilter = new ComboBox
            {
                Location = new Point(670, 16),
                Width = 120,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbFilter.Items.AddRange(new[] { "الكل", "فواتير فقط", "دفعات فقط" });
            cmbFilter.SelectedIndex = 0;
            top.Controls.Add(cmbFilter);

            // زر عرض التقرير
            btnLoad = new Button
            {
                Text = "عرض",
                Location = new Point(810, 15),
                Width = 100,
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.Click += BtnLoad_Click;
            top.Controls.Add(btnLoad);

            // زر الطباعة
            btnPrint = new Button
            {
                Text = "طباعة",
                Location = new Point(920, 15),
                Width = 80,
                BackColor = Color.Green,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Click += BtnPrint_Click;
            top.Controls.Add(btnPrint);

            // زر تصدير Excel
            btnExport = new Button
            {
                Text = "تصدير Excel",
                Location = new Point(1010, 15),
                Width = 100,
                BackColor = Color.Orange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Click += BtnExport_Click;
            top.Controls.Add(btnExport);

            Controls.Add(top);

            // WebBrowser لعرض التقرير
            browser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                ScriptErrorsSuppressed = true,
                BackColor = Color.White
            };
            Controls.Add(browser);
        }
        #endregion

        #region Data
        void LoadSubscribers()
        {
            using (SqlConnection con = Db.GetConnection())
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT SubscriberID, Name FROM Subscribers ORDER BY Name", con);
                DataTable dt = new DataTable();
                da.Fill(dt);

                cmbSubscriber.DisplayMember = "Name";
                cmbSubscriber.ValueMember = "SubscriberID";
                cmbSubscriber.DataSource = dt;
            }
        }

        decimal GetOpeningBalance(int subscriberId)
        {
            using (SqlConnection con = Db.GetConnection())
            {
                SqlCommand cmd = new SqlCommand(@"
                    SELECT ISNULL(SUM(Debit - Credit),0)
                    FROM AccountStatements
                    WHERE SubscriberID=@id AND Date < @from", con);
                cmd.Parameters.AddWithValue("@id", subscriberId);
                cmd.Parameters.AddWithValue("@from", dtFrom.Value.Date);
                con.Open();
                return Convert.ToDecimal(cmd.ExecuteScalar());
            }
        }

        DataTable GetStatement(int subscriberId)
        {
            string filter = "";

            if (cmbFilter.SelectedIndex == 1)
                filter += " AND InvoiceID IS NOT NULL";
            else if (cmbFilter.SelectedIndex == 2)
                filter += " AND PaymentID IS NOT NULL";

            using (SqlConnection con = Db.GetConnection())
            {
                string sql = $@"
                SELECT Date, Details, Debit, Credit
                FROM AccountStatements
                WHERE SubscriberID=@id
                  AND Date BETWEEN @from AND @to
                  {filter}
                ORDER BY Date, StatementID";

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@id", subscriberId);
                cmd.Parameters.AddWithValue("@from", dtFrom.Value.Date);
                cmd.Parameters.AddWithValue("@to", dtTo.Value.Date);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
        #endregion

        #region Render
        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (cmbSubscriber.SelectedValue == null)
            {
                MessageBox.Show("يرجى اختيار مشترك.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int sid = Convert.ToInt32(cmbSubscriber.SelectedValue);
            decimal opening = GetOpeningBalance(sid);
            DataTable dt = GetStatement(sid);
            RenderReport(dt, opening);
        }

        void RenderReport(DataTable dt, decimal openingBalance)
        {
            decimal running = openingBalance;
            decimal totalDebit = 0;
            decimal totalCredit = 0;

            StringBuilder rows = new StringBuilder();

            rows.Append($@"
<tr style='font-weight:bold;background:#eef'>
<td colspan='4'>الرصيد الافتتاحي</td>
<td>{openingBalance:N2}</td>
</tr>");

            foreach (DataRow r in dt.Rows)
            {
                decimal debit = Convert.ToDecimal(r["Debit"]);
                decimal credit = Convert.ToDecimal(r["Credit"]);

                totalDebit += debit;
                totalCredit += credit;
                running += debit - credit;

                rows.Append($@"
<tr>
<td>{Convert.ToDateTime(r["Date"]):dd-MM-yyyy}</td>
<td>{r["Details"]}</td>
<td>{debit:N2}</td>
<td>{credit:N2}</td>
<td>{running:N2}</td>
</tr>");
            }

            rows.Append($@"
<tr style='font-weight:bold;background:#f9f9f9'>
<td colspan='2'>الإجمالي</td>
<td>{totalDebit:N2}</td>
<td>{totalCredit:N2}</td>
<td>{running:N2}</td>
</tr>");

            string html = $@"
<html dir='rtl'>
<head>
<meta charset='UTF-8'>
<style>
body{{font-family:Tahoma;margin:20px;}}
table{{width:100%;border-collapse:collapse}}
th,td{{border:1px solid #ccc;padding:6px;text-align:right}}
th{{background:#eee}}
</style>
</head>
<body>
<h2 style='text-align:center'>كشف حساب مشترك</h2>
<p style='margin-bottom:10px'>المشترك: <b>{cmbSubscriber.Text}</b> | الفترة: <b>{dtFrom.Value:dd-MM-yyyy}</b> إلى <b>{dtTo.Value:dd-MM-yyyy}</b></p>
<table>
<tr>
<th>التاريخ</th>
<th>البيان</th>
<th>مدين</th>
<th>دائن</th>
<th>الرصيد</th>
</tr>
{rows}
</table>
</body>
</html>";

            browser.DocumentText = html;
        }
        #endregion

        #region Print & Export
        private void BtnPrint_Click(object sender, EventArgs e)
        {
            if (browser.Document != null)
            {
                browser.Print();
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV File|*.csv",
                FileName = $"كشف_حساب_{cmbSubscriber.Text}_{DateTime.Now:yyyyMMdd}.csv"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            int sid = Convert.ToInt32(cmbSubscriber.SelectedValue);
            decimal opening = GetOpeningBalance(sid);
            DataTable dt = GetStatement(sid);

            using (var sw = new StreamWriter(sfd.FileName, false, Encoding.UTF8))
            {
                sw.WriteLine("التاريخ,البيان,مدين,دائن,الرصيد");

                decimal running = opening;
                sw.WriteLine($",الرصيد الافتتاحي,,,{running:N2}");

                foreach (DataRow r in dt.Rows)
                {
                    decimal debit = Convert.ToDecimal(r["Debit"]);
                    decimal credit = Convert.ToDecimal(r["Credit"]);
                    running += debit - credit;

                    string date = Convert.ToDateTime(r["Date"]).ToString("dd-MM-yyyy");
                    string details = r["Details"].ToString();

                    sw.WriteLine($"{date},{details},{debit:N2},{credit:N2},{running:N2}");
                }
            }

            MessageBox.Show("تم تصدير التقرير بنجاح.", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}

/*using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace water3.Reports.Render
{

        public partial class SubscriberStatementReportForm : Form
        {
            ComboBox cmbSubscriber, cmbFilter;
            DateTimePicker dtFrom, dtTo;
            Button btnLoad;
            WebBrowser browser;

             

            public SubscriberStatementReportForm()
            {
                Text = "كشف حساب مشترك";
                Size = new Size(1100, 700);
                StartPosition = FormStartPosition.CenterScreen;

                BuildUI();
                LoadSubscribers();
            }

            #region UI
            void BuildUI()
            {
                Panel top = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.WhiteSmoke };

                // اسم المشترك
                top.Controls.Add(new Label { Text = "المشترك:", Location = new Point(10, 20), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
                cmbSubscriber = new ComboBox
                {
                    Location = new Point(70, 16),
                    Width = 200,
                    DropDownStyle = ComboBoxStyle.DropDown
                };
                cmbSubscriber.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                cmbSubscriber.AutoCompleteSource = AutoCompleteSource.ListItems;
                top.Controls.Add(cmbSubscriber);

                // من تاريخ
                top.Controls.Add(new Label { Text = "من:", Location = new Point(290, 20), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
                dtFrom = new DateTimePicker { Location = new Point(320, 16), Width = 120, Format = DateTimePickerFormat.Short };
                top.Controls.Add(dtFrom);

                // إلى تاريخ
                top.Controls.Add(new Label { Text = "إلى:", Location = new Point(460, 20), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
                dtTo = new DateTimePicker { Location = new Point(490, 16), Width = 120, Format = DateTimePickerFormat.Short };
                top.Controls.Add(dtTo);

                // فلترة (الكل / فواتير / دفعات)
                top.Controls.Add(new Label { Text = "فلترة:", Location = new Point(630, 20), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) });
                cmbFilter = new ComboBox
                {
                    Location = new Point(670, 16),
                    Width = 120,
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                cmbFilter.Items.AddRange(new[] { "الكل", "فواتير فقط", "دفعات فقط" });
                cmbFilter.SelectedIndex = 0;
                top.Controls.Add(cmbFilter);

                // زر عرض التقرير
                btnLoad = new Button
                {
                    Text = "عرض",
                    Location = new Point(810, 15),
                    Width = 100,
                    BackColor = Color.DodgerBlue,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnLoad.FlatAppearance.BorderSize = 0;
                btnLoad.Click += BtnLoad_Click;
                top.Controls.Add(btnLoad);

                Controls.Add(top);

                // WebBrowser لعرض التقرير
                browser = new WebBrowser
                {
                    Dock = DockStyle.Fill,
                    ScriptErrorsSuppressed = true,
                    BackColor = Color.White
                };
                Controls.Add(browser);
            }
            #endregion

            #region Data
            void LoadSubscribers()
            {
                using (SqlConnection con = Db.GetConnection())
                {
                    SqlDataAdapter da = new SqlDataAdapter("SELECT SubscriberID, Name FROM Subscribers ORDER BY Name", con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbSubscriber.DisplayMember = "Name";
                    cmbSubscriber.ValueMember = "SubscriberID";
                    cmbSubscriber.DataSource = dt;
                }
            }

            decimal GetOpeningBalance(int subscriberId)
            {
                using (SqlConnection con = Db.GetConnection())
                {
                    SqlCommand cmd = new SqlCommand(@"
                    SELECT ISNULL(SUM(Debit - Credit),0)
                    FROM AccountStatements
                    WHERE SubscriberID=@id AND Date < @from", con);
                    cmd.Parameters.AddWithValue("@id", subscriberId);
                    cmd.Parameters.AddWithValue("@from", dtFrom.Value.Date);
                    con.Open();
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }

            DataTable GetStatement(int subscriberId)
            {
                string filter = "";

                if (cmbFilter.SelectedIndex == 1)
                    filter += " AND InvoiceID IS NOT NULL";
                else if (cmbFilter.SelectedIndex == 2)
                    filter += " AND PaymentID IS NOT NULL";

                using (SqlConnection con = Db.GetConnection())
                {
                    string sql = $@"
                SELECT Date, Details, Debit, Credit
                FROM AccountStatements
                WHERE SubscriberID=@id
                  AND Date BETWEEN @from AND @to
                  {filter}
                ORDER BY Date, StatementID";

                    SqlCommand cmd = new SqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("@id", subscriberId);
                    cmd.Parameters.AddWithValue("@from", dtFrom.Value.Date);
                    cmd.Parameters.AddWithValue("@to", dtTo.Value.Date);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
            #endregion

            #region Render
            private void BtnLoad_Click(object sender, EventArgs e)
            {
                if (cmbSubscriber.SelectedValue == null)
                {
                    MessageBox.Show("يرجى اختيار مشترك.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int sid = Convert.ToInt32(cmbSubscriber.SelectedValue);
                decimal opening = GetOpeningBalance(sid);
                DataTable dt = GetStatement(sid);
                RenderReport(dt, opening);
            }

            void RenderReport(DataTable dt, decimal openingBalance)
            {
                decimal running = openingBalance;
                decimal totalDebit = 0;
                decimal totalCredit = 0;

                StringBuilder rows = new StringBuilder();

                rows.Append($@"
<tr style='font-weight:bold;background:#eef'>
<td colspan='4'>الرصيد الافتتاحي</td>
<td>{openingBalance:N2}</td>
</tr>");

                foreach (DataRow r in dt.Rows)
                {
                    decimal debit = Convert.ToDecimal(r["Debit"]);
                    decimal credit = Convert.ToDecimal(r["Credit"]);

                    totalDebit += debit;
                    totalCredit += credit;
                    running += debit - credit;

                    rows.Append($@"
<tr>
<td>{Convert.ToDateTime(r["Date"]):dd-MM-yyyy}</td>
<td>{r["Details"]}</td>
<td>{debit:N2}</td>
<td>{credit:N2}</td>
<td>{running:N2}</td>
</tr>");
                }

                rows.Append($@"
<tr style='font-weight:bold;background:#f9f9f9'>
<td colspan='2'>الإجمالي</td>
<td>{totalDebit:N2}</td>
<td>{totalCredit:N2}</td>
<td>{running:N2}</td>
</tr>");

                string html = $@"
<html dir='rtl'>
<head>
<meta charset='UTF-8'>
<style>
body{{font-family:Tahoma;margin:20px;}}
table{{width:100%;border-collapse:collapse}}
th,td{{border:1px solid #ccc;padding:6px;text-align:right}}
th{{background:#eee}}
</style>
</head>
<body>
<h2 style='text-align:center'>كشف حساب مشترك</h2>
<p style='margin-bottom:10px'>المشترك: <b>{cmbSubscriber.Text}</b> | الفترة: <b>{dtFrom.Value:dd-MM-yyyy}</b> إلى <b>{dtTo.Value:dd-MM-yyyy}</b></p>
<table>
<tr>
<th>التاريخ</th>
<th>البيان</th>
<th>مدين</th>
<th>دائن</th>
<th>الرصيد</th>
</tr>
{rows}
</table>
</body>
</html>";

                browser.DocumentText = html;
            }
            #endregion
        }
    }
*/

