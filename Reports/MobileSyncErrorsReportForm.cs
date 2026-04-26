using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using water3.Services;

namespace water3.Reports
{
    //public partial class MobileSyncErrorsReportForm : Form
    //{
        public partial class MobileSyncErrorsReportForm : BaseReportForm
        {
            private readonly OperationsReportsService _service = new OperationsReportsService();
            private DateTimePicker dtFrom;
            private DateTimePicker dtTo;

            public MobileSyncErrorsReportForm()
            {
                Text = "تقرير أخطاء المزامنة";
                lblTitle.Text = Text;
                ApplyReportPermission("REPORT_MOBILE_ERRORS_VIEW");
                BuildFilters();
            }

            private void BuildFilters()
            {
                pnlFilters.Controls.Add(MakeFilterLabel("من تاريخ", 1120, 20));
                pnlFilters.Controls.Add(MakeFilterLabel("إلى تاريخ", 850, 20));

                dtFrom = new DateTimePicker { Location = new System.Drawing.Point(950, 18), Size = new System.Drawing.Size(150, 28), Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddMonths(-1) };
                dtTo = new DateTimePicker { Location = new System.Drawing.Point(680, 18), Size = new System.Drawing.Size(150, 28), Format = DateTimePickerFormat.Short, Value = DateTime.Today };

                pnlFilters.Controls.AddRange(new Control[] { dtFrom, dtTo });
            }

            protected override void LoadReportData()
            {
                try
                {
                    dgvReport.DataSource = _service.GetMobileSyncErrors(dtFrom.Value, dtTo.Value);
                    lblStatus.Text = "تم تحميل تقرير أخطاء المزامنة.";
                    _service.LogReportOpen("MobileSyncErrorsReport");
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = System.Drawing.Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }
        }
    }