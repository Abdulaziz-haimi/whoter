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
using water3.Reports;

namespace water3.Reports
{
    //public partial class CollectorDevicesReportForm : Form
    //{
     

        public partial class CollectorDevicesReportForm : BaseReportForm
        {
            private readonly OperationsReportsService _service = new OperationsReportsService();
            private ComboBox cboCollectors;
            private List<CollectorItem> _collectors;

            public CollectorDevicesReportForm()
            {
                Text = "تقرير أجهزة المحصلين";
                lblTitle.Text = Text;
                ApplyReportPermission("REPORT_COLLECTOR_DEVICES_VIEW");
                BuildFilters();
                LoadCollectors();
            }

            private void BuildFilters()
            {
                pnlFilters.Controls.Add(MakeFilterLabel("المحصل", 1120, 20));
                cboCollectors = new ComboBox { Location = new System.Drawing.Point(820, 18), Size = new System.Drawing.Size(280, 28), DropDownStyle = ComboBoxStyle.DropDownList };
                pnlFilters.Controls.Add(cboCollectors);
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
                    dgvReport.DataSource = _service.GetCollectorDevices(selectedId > 0 ? (int?)selectedId : null);
                    lblStatus.Text = "تم تحميل تقرير أجهزة المحصلين.";
                    _service.LogReportOpen("CollectorDevicesReport");
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = System.Drawing.Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }
        }
    }