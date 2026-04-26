using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using water3.Models;
using water3.Services;
namespace water3.Reports
{
    //public partial class PhonePushReportForm : Form
    //{
  
        public partial class PhonePushReportForm : BaseReportForm
        {
            private readonly OperationsReportsService _service = new OperationsReportsService();
            private readonly MobileSyncToPhoneService _mobileSyncService = new MobileSyncToPhoneService();

            private ComboBox cboCollectors;
            private ComboBox cboDevices;
            private DateTimePicker dtAsOf;
            private CheckBox chkOnlyAssigned;
            private CheckBox chkIncludeAll;

            private List<CollectorItem> _collectors = new List<CollectorItem>();
            private List<CollectorDeviceItem> _devices = new List<CollectorDeviceItem>();

            public PhonePushReportForm()
            {
                Text = "تقرير التنزيل إلى الهاتف";
                lblTitle.Text = Text;
                ApplyReportPermission("REPORT_PHONE_PUSH_VIEW");
                BuildFilters();
                LoadCollectors();
            }

            private void BuildFilters()
            {
                pnlFilters.Controls.Add(MakeFilterLabel("المحصل", 1120, 15));
                pnlFilters.Controls.Add(MakeFilterLabel("الجهاز", 820, 15));
                pnlFilters.Controls.Add(MakeFilterLabel("حتى تاريخ", 510, 15));

                cboCollectors = new ComboBox { Location = new System.Drawing.Point(950, 12), Size = new System.Drawing.Size(150, 28), DropDownStyle = ComboBoxStyle.DropDownList };
                cboDevices = new ComboBox { Location = new System.Drawing.Point(620, 12), Size = new System.Drawing.Size(180, 28), DropDownStyle = ComboBoxStyle.DropDownList };
                dtAsOf = new DateTimePicker { Location = new System.Drawing.Point(360, 12), Size = new System.Drawing.Size(130, 28), Format = DateTimePickerFormat.Short, Value = DateTime.Today };
                chkOnlyAssigned = new CheckBox { Location = new System.Drawing.Point(950, 48), Size = new System.Drawing.Size(190, 24), Text = "المخصص فقط", Checked = true };
                chkIncludeAll = new CheckBox { Location = new System.Drawing.Point(760, 48), Size = new System.Drawing.Size(170, 24), Text = "الكل إذا لا يوجد تخصيص", Checked = true };

                cboCollectors.SelectedIndexChanged += (s, e) => LoadDevices();

                pnlFilters.Controls.AddRange(new Control[] { cboCollectors, cboDevices, dtAsOf, chkOnlyAssigned, chkIncludeAll });
            }

            private void LoadCollectors()
            {
                _collectors = _service.GetCollectors();
                cboCollectors.DataSource = _collectors;
                cboCollectors.DisplayMember = "Name";
                cboCollectors.ValueMember = "CollectorID";
                LoadDevices();
            }

            private void LoadDevices()
            {
                if (!(cboCollectors.SelectedItem is CollectorItem collector))
                    return;

                _devices = _mobileSyncService.GetApprovedActiveDevices(collector.CollectorID);
                cboDevices.DataSource = _devices;
                cboDevices.DisplayMember = "DeviceName";
                cboDevices.ValueMember = "DeviceID";
            }

            protected override void LoadReportData()
            {
                try
                {
                    if (!(cboCollectors.SelectedItem is CollectorItem collector))
                        throw new InvalidOperationException("اختر المحصل أولًا.");

                    if (!(cboDevices.SelectedItem is CollectorDeviceItem device))
                        throw new InvalidOperationException("اختر الجهاز أولًا.");

                    dgvReport.DataSource = _service.GetPhonePushPreview(
                        dtAsOf.Value,
                        collector.CollectorID,
                        collector.Name,
                        device.DeviceCode,
                        device.DeviceName,
                        chkOnlyAssigned.Checked,
                        chkIncludeAll.Checked);

                    lblStatus.Text = "تم تحميل تقرير التنزيل إلى الهاتف.";
                    _service.LogReportOpen("PhonePushReport");
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = System.Drawing.Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }
        }
    }