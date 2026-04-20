using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;

using water3.Reports.Render;
using System.IO;

namespace water3.Reports.Render
{
    public partial class GeneralJournalReportForm : Form
    {

        WebBrowser browser;
        DateTimePicker dtFrom, dtTo;
        Button btnLoad;

         
        private object HtmlReportRenderer;

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

            // إنشاء HTML مباشرة
            string html = $@"
<!DOCTYPE html>
<html dir='rtl'>
<head>
    <meta charset='UTF-8'>
    <title>تقرير دفتر اليومية</title>
    <style>
        body {{ font-family: Arial, sans-serif; margin: 20px; }}
        .header {{ text-align: center; margin-bottom: 30px; }}
        .title {{ font-size: 24px; font-weight: bold; }}
        .subtitle {{ font-size: 16px; color: #666; }}
        table {{ width: 100%; border-collapse: collapse; margin-top: 20px; }}
        th, td {{ border: 1px solid #ddd; padding: 8px; text-align: right; }}
        th {{ background-color: #f2f2f2; }}
        .totals {{ margin-top: 20px; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='header'>
        <div class='title'>تقرير دفتر اليومية</div>
        <div class='subtitle'>الفترة: {dtFrom.Value:dd-MM-yyyy} إلى {dtTo.Value:dd-MM-yyyy}</div>
    </div>
    
    <table>
        <thead>
            <tr>
                <th>التاريخ</th>
                <th>اسم الحساب</th>
                <th>الوصف</th>
                <th>مدين</th>
                <th>دائن</th>
            </tr>
        </thead>
        <tbody>
            {rows}
        </tbody>
        <tfoot>
            <tr>
                <td colspan='3'><strong>المجموع</strong></td>
                <td><strong>{debit:N2}</strong></td>
                <td><strong>{credit:N2}</strong></td>
            </tr>
        </tfoot>
    </table>
</body>
</html>";

            browser.DocumentText = html;
        }
    }
}
