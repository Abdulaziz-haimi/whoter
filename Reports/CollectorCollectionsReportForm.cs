using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using water3.Models;
using water3.Services;

namespace water3.Reports
{
    //public partial class CollectorCollectionsReportForm : Form
    //{
     


        public class CollectorCollectionsReportForm : BaseReportForm
        {
            private readonly OperationsReportsService _service = new OperationsReportsService();
            private DateTimePicker dtFrom;
            private DateTimePicker dtTo;
            private ComboBox cboCollectors;
            private List<CollectorItem> _collectors;

            public CollectorCollectionsReportForm()
            {
                Text = "تقرير تحصيلات المحصلين";
                lblTitle.Text = Text;
                ApplyReportPermission("REPORT_COLLECTOR_COLLECTIONS_VIEW");
                BuildFilters();
                LoadCollectors();
            }

            private void BuildFilters()
            {
                pnlFilters.Controls.Add(MakeFilterLabel("من تاريخ", 1120, 20));
                pnlFilters.Controls.Add(MakeFilterLabel("إلى تاريخ", 850, 20));
                pnlFilters.Controls.Add(MakeFilterLabel("المحصل", 580, 20));

                dtFrom = new DateTimePicker { Location = new System.Drawing.Point(950, 18), Size = new System.Drawing.Size(150, 28), Format = DateTimePickerFormat.Short, Value = DateTime.Today.AddMonths(-1) };
                dtTo = new DateTimePicker { Location = new System.Drawing.Point(680, 18), Size = new System.Drawing.Size(150, 28), Format = DateTimePickerFormat.Short, Value = DateTime.Today };
                cboCollectors = new ComboBox { Location = new System.Drawing.Point(260, 18), Size = new System.Drawing.Size(300, 28), DropDownStyle = ComboBoxStyle.DropDownList };

                pnlFilters.Controls.AddRange(new Control[] { dtFrom, dtTo, cboCollectors });
            }

            private void LoadCollectors()
            {
                _collectors = _service.GetCollectors();
                _collectors.Insert(0, new CollectorItem { CollectorID = 0, Name = "الكل" });
                cboCollectors.DataSource = _collectors;
                cboCollectors.DisplayMember = "Name";
                cboCollectors.ValueMember = "CollectorID";
            }

            protected override void LoadReportData()
            {
                try
                {
                    int selectedId = cboCollectors.SelectedValue is int ? (int)cboCollectors.SelectedValue : 0;
                    dgvReport.DataSource = _service.GetCollectorCollections(dtFrom.Value, dtTo.Value, selectedId > 0 ? (int?)selectedId : null);
                    lblStatus.Text = "تم تحميل تقرير تحصيلات المحصلين.";
                    _service.LogReportOpen("CollectorCollectionsReport", $"From={dtFrom.Value:yyyy-MM-dd}, To={dtTo.Value:yyyy-MM-dd}");
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = System.Drawing.Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }
        }
    }