using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using water3.Reports.Render;

namespace water3.Forms
{
    public partial class GeneralJournalReportForm : Form
    {
     
        WebBrowser browser;
        DateTimePicker dtFrom, dtTo;
        Button btnLoad;

         

        public GeneralJournalReportForm()
        {
            Text = "تقرير دفتر اليومية";
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;

            BuildUI();
        }

        private void BuildUI()
        {
            Panel top = new Panel { Dock = DockStyle.Top, Height = 45 };

            top.Controls.Add(new Label { Text = "من", Location = new Point(10, 12) });
            dtFrom = new DateTimePicker { Location = new Point(40, 8), Width = 120 };

            top.Controls.Add(new Label { Text = "إلى", Location = new Point(180, 12) });
            dtTo = new DateTimePicker { Location = new Point(210, 8), Width = 120 };

            btnLoad = new Button
            {
                Text = "عرض التقرير",
                Location = new Point(360, 7),
                Width = 120,
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White
            };
            btnLoad.Click += BtnLoad_Click;

            top.Controls.AddRange(new Control[] { dtFrom, dtTo, btnLoad });
            Controls.Add(top);

            browser = new WebBrowser
            {
                Dock = DockStyle.Fill,
                ScriptErrorsSuppressed = true
            };
            Controls.Add(browser);
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            DataTable dt = GetJournalData();
            RenderReport(dt);
        }

        private DataTable GetJournalData()
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("rpt_GeneralJournal", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@FromDate", dtFrom.Value.Date);
                cmd.Parameters.AddWithValue("@ToDate", dtTo.Value.Date);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private void RenderReport(DataTable dt)
        {
            decimal debit = 0, credit = 0;
            StringBuilder rows = new StringBuilder();

            foreach (DataRow r in dt.Rows)
            {
                debit += r.Field<decimal>("Debit");
                credit += r.Field<decimal>("Credit");

                rows.Append($@"
<tr>
<td>{Convert.ToDateTime(r["EntryDate"]):dd-MM-yyyy}</td>
<td>{r["AccountName"]}</td>
<td>{r["Description"]}</td>
<td>{r["Debit"]:N2}</td>
<td>{r["Credit"]:N2}</td>
</tr>");
            }

            string content = HtmlReportRenderer.Render(
                "Reports/Templates/general_journal.html",
                new Dictionary<string, string>
                {
                    ["ROWS"] = rows.ToString(),
                    ["TOTAL_DEBIT"] = debit.ToString("N2"),
                    ["TOTAL_CREDIT"] = credit.ToString("N2")
                });

            string html = HtmlReportRenderer.Render(
                "Reports/Templates/report_base.html",
                new Dictionary<string, string>
                {
                    ["TITLE"] = "تقرير دفتر اليومية",
                    ["DATE_RANGE"] = $"{dtFrom.Value:dd-MM-yyyy} إلى {dtTo.Value:dd-MM-yyyy}",
                    ["CONTENT"] = content
                });

            browser.DocumentText = html;
        }



private void GeneralJournalReportForm_Load(object sender, EventArgs e)
    {

    }
}
}
