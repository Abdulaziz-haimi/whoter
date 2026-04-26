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
    //public partial class AgingReceivablesReportForm : Form
    //{

        public partial class AgingReceivablesReportForm : BaseReportForm
        {
            private DateTimePicker dtAsOf;

            public AgingReceivablesReportForm()
            {
                Text = "تقرير أعمار الذمم";
                lblTitle.Text = Text;
                ApplyReportPermission("REPORT_AGING_VIEW");
                BuildFilters();
            }

            private void BuildFilters()
            {
                pnlFilters.Controls.Add(MakeFilterLabel("حتى تاريخ", 1120, 20));
                dtAsOf = new DateTimePicker
                {
                    Location = new System.Drawing.Point(950, 18),
                    Size = new System.Drawing.Size(150, 28),
                    Format = DateTimePickerFormat.Short,
                    Value = DateTime.Today
                };
                pnlFilters.Controls.Add(dtAsOf);
            }

            protected override void LoadReportData()
            {
                try
                {
                    dgvReport.DataSource = ReportsService.GetAgingReceivables(dtAsOf.Value);
                    lblStatus.ForeColor = System.Drawing.Color.DarkGreen;
                    lblStatus.Text = "تم تحميل تقرير أعمار الذمم.";
                    ReportsService.LogReportOpen("AgingReceivablesReport", $"AsOf={dtAsOf.Value:yyyy-MM-dd}");
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = System.Drawing.Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            protected override void RefreshReport()
            {
                dtAsOf.Value = DateTime.Today;
                dgvReport.DataSource = null;
                lblStatus.ForeColor = System.Drawing.Color.DarkGreen;
                lblStatus.Text = "تمت إعادة التهيئة.";
            }
        }
    }