using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace water3.Reports
{
    //public partial class GeneralJournalReportForm : Form
    //{

        public partial class GeneralJournalReportForm : BaseReportForm
        {
            private DateTimePicker dtFrom;
            private DateTimePicker dtTo;
            private TextBox txtSource;

            public GeneralJournalReportForm()
            {
                Text = "دفتر اليومية";
                lblTitle.Text = Text;
                ApplyReportPermission("REPORT_GENERALJOURNAL_VIEW");
                BuildFilters();
            }

            private void BuildFilters()
            {
                pnlFilters.Controls.Add(MakeFilterLabel("من تاريخ", 1120, 20));
                pnlFilters.Controls.Add(MakeFilterLabel("إلى تاريخ", 850, 20));
                pnlFilters.Controls.Add(MakeFilterLabel("المصدر", 580, 20));

                dtFrom = new DateTimePicker { Location = new System.Drawing.Point(950, 18), Size = new System.Drawing.Size(150, 28), Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddMonths(-1) };
                dtTo = new DateTimePicker { Location = new System.Drawing.Point(680, 18), Size = new System.Drawing.Size(150, 28), Format = DateTimePickerFormat.Short, Value = DateTime.Today };
                txtSource = new TextBox { Location = new System.Drawing.Point(260, 18), Size = new System.Drawing.Size(300, 27) };

                pnlFilters.Controls.AddRange(new Control[] { dtFrom, dtTo, txtSource });
            }

            protected override void LoadReportData()
            {
                try
                {
                    dgvReport.DataSource = ReportsService.GetGeneralJournal(dtFrom.Value, dtTo.Value, txtSource.Text);
                    lblStatus.ForeColor = System.Drawing.Color.DarkGreen;
                    lblStatus.Text = "تم تحميل دفتر اليومية.";
                    ReportsService.LogReportOpen("GeneralJournalReport", $"From={dtFrom.Value:yyyy-MM-dd}, To={dtTo.Value:yyyy-MM-dd}, Source={txtSource.Text}");
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = System.Drawing.Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            protected override void RefreshReport()
            {
                dtFrom.Value = DateTime.Today.AddMonths(-1);
                dtTo.Value = DateTime.Today;
                txtSource.Clear();
                dgvReport.DataSource = null;
                lblStatus.ForeColor = System.Drawing.Color.DarkGreen;
                lblStatus.Text = "تمت إعادة التهيئة.";
            }
        }
    }