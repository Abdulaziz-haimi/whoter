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
using System.Drawing;
using System.Windows.Forms;
using water3.Models;
using water3.Services;
using water3.Utils;
namespace water3.Forms
{
    public partial class MobileSyncToPhoneForm : Form
    {



        //public class MobileSyncToPhoneForm : Form
        //{
            private readonly MobileSyncToPhoneService _service = new MobileSyncToPhoneService();

            private List<CollectorItem> _collectors = new List<CollectorItem>();
            private List<CollectorDeviceItem> _devices = new List<CollectorDeviceItem>();

            private Label lblTitle;
            private Label lblCollector;
            private Label lblDevice;
            private Label lblAsOfDate;
            private ComboBox cboCollectors;
            private ComboBox cboDevices;
            private DateTimePicker dtAsOfDate;
            private CheckBox chkOnlyAssignedSubscribers;
            private CheckBox chkIncludeAllIfNoAssignments;
            private Button btnLoadDevices;
            private Button btnPreview;
            private Button btnSyncToPhone;
            private GroupBox grpSummary;
            private Label lblSummary1;
            private Label lblSummary2;
            private Label lblSummary3;
            private Label lblSummary4;
            private Label lblSummary5;
            private Label lblSummary6;
            private TextBox txtLog;

            public MobileSyncToPhoneForm()
            {
                InitializeComponent();
                PermissionHelper.EnforceFormPermission(this, "MOBILE_SYNC_TO_PHONE_VIEW");
                PermissionHelper.ApplyControlPermission(btnSyncToPhone, "MOBILE_SYNC_TO_PHONE_EXECUTE");
                LoadCollectors();
            }

            private void InitializeComponent()
            {
                Text = "المزامنة إلى الهاتف";
                StartPosition = FormStartPosition.CenterScreen;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;
                ClientSize = new Size(900, 620);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;

                lblTitle = new Label
                {
                    Text = "المزامنة / الدفع إلى الهاتف",
                    Font = new Font("Tahoma", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204),
                    AutoSize = true,
                    Location = new Point(280, 20)
                };

                lblCollector = MakeLabel("المحصل", 70);
                lblDevice = MakeLabel("الجهاز", 120);
                lblAsOfDate = MakeLabel("حتى تاريخ", 170);

                cboCollectors = new ComboBox
                {
                    Location = new Point(220, 65),
                    Size = new Size(350, 28),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                btnLoadDevices = new Button
                {
                    Text = "تحميل الأجهزة",
                    Location = new Point(580, 63),
                    Size = new Size(120, 32),
                    BackColor = Color.FromArgb(0, 122, 204),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnLoadDevices.Click += BtnLoadDevices_Click;

                cboDevices = new ComboBox
                {
                    Location = new Point(220, 115),
                    Size = new Size(480, 28),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                dtAsOfDate = new DateTimePicker
                {
                    Location = new Point(220, 165),
                    Size = new Size(180, 28),
                    Format = DateTimePickerFormat.Short,
                    Value = DateTime.Today
                };

                chkOnlyAssignedSubscribers = new CheckBox
                {
                    Text = "تنزيل المشتركين المخصصين للمحصل فقط",
                    Location = new Point(220, 215),
                    AutoSize = true,
                    Checked = true
                };

                chkIncludeAllIfNoAssignments = new CheckBox
                {
                    Text = "إذا لم يوجد تخصيص، نزّل الكل",
                    Location = new Point(220, 245),
                    AutoSize = true,
                    Checked = true
                };

                btnPreview = new Button
                {
                    Text = "معاينة البيانات",
                    Location = new Point(220, 285),
                    Size = new Size(140, 36),
                    BackColor = Color.SteelBlue,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnPreview.Click += BtnPreview_Click;

                btnSyncToPhone = new Button
                {
                    Text = "تنفيذ المزامنة إلى الهاتف",
                    Location = new Point(370, 285),
                    Size = new Size(200, 36),
                    BackColor = Color.FromArgb(0, 153, 76),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnSyncToPhone.Click += BtnSyncToPhone_Click;

                grpSummary = new GroupBox
                {
                    Text = "ملخص البيانات التي ستنزل إلى الهاتف",
                    Location = new Point(40, 340),
                    Size = new Size(820, 150)
                };

                lblSummary1 = MakeSummaryLabel(20, 30);
                lblSummary2 = MakeSummaryLabel(20, 55);
                lblSummary3 = MakeSummaryLabel(20, 80);
                lblSummary4 = MakeSummaryLabel(400, 30);
                lblSummary5 = MakeSummaryLabel(400, 55);
                lblSummary6 = MakeSummaryLabel(400, 80);

                grpSummary.Controls.AddRange(new Control[]
                {
                lblSummary1, lblSummary2, lblSummary3,
                lblSummary4, lblSummary5, lblSummary6
                });

                txtLog = new TextBox
                {
                    Location = new Point(40, 505),
                    Size = new Size(820, 90),
                    Multiline = true,
                    ReadOnly = true,
                    ScrollBars = ScrollBars.Vertical,
                    BackColor = Color.White
                };

                Controls.AddRange(new Control[]
                {
                lblTitle,
                lblCollector,
                lblDevice,
                lblAsOfDate,
                cboCollectors,
                btnLoadDevices,
                cboDevices,
                dtAsOfDate,
                chkOnlyAssignedSubscribers,
                chkIncludeAllIfNoAssignments,
                btnPreview,
                btnSyncToPhone,
                grpSummary,
                txtLog
                });
            }

            private Label MakeLabel(string text, int top)
            {
                return new Label
                {
                    Text = text,
                    AutoSize = true,
                    Location = new Point(720, top + 5)
                };
            }

            private Label MakeSummaryLabel(int left, int top)
            {
                return new Label
                {
                    Location = new Point(left, top),
                    Size = new Size(350, 22),
                    Font = new Font("Tahoma", 10F, FontStyle.Bold)
                };
            }

            private void LoadCollectors()
            {
                _collectors = _service.GetCollectors();
                cboCollectors.DataSource = _collectors;
                cboCollectors.DisplayMember = "Name";
                cboCollectors.ValueMember = "CollectorID";
            }

            private void BtnLoadDevices_Click(object sender, EventArgs e)
            {
                try
                {
                    if (!(cboCollectors.SelectedItem is CollectorItem collector))
                    {
                        Log("اختر المحصل أولًا.");
                        return;
                    }

                    _devices = _service.GetApprovedActiveDevices(collector.CollectorID);
                    cboDevices.DataSource = null;
                    cboDevices.DataSource = _devices;
                    cboDevices.DisplayMember = "DeviceName";
                    cboDevices.ValueMember = "DeviceID";

                    Log($"تم تحميل {_devices.Count} جهاز/أجهزة معتمدة ونشطة للمحصل: {collector.Name}");
                }
                catch (Exception ex)
                {
                    Log("خطأ: " + ex.Message);
                }
            }

            private void BtnPreview_Click(object sender, EventArgs e)
            {
                try
                {
                    MobileExportSummaryItem summary = BuildPreview();
                    ShowSummary(summary);
                    Log("تمت معاينة البيانات بنجاح.");
                }
                catch (Exception ex)
                {
                    Log("خطأ في المعاينة: " + ex.Message);
                }
            }

            private void BtnSyncToPhone_Click(object sender, EventArgs e)
            {
                try
                {
                    MobileExportSummaryItem summary = BuildPreview();
                    ShowSummary(summary);
                    _service.RegisterSyncPushLog(summary);
                    Log("تم تسجيل عملية المزامنة إلى الهاتف في السجل بنجاح.");
                    Log("الخطوة التالية الفعلية: الهاتف يقوم بتنزيل نفس البيانات عبر API أو طبقة MobileSync.");
                }
                catch (Exception ex)
                {
                    Log("خطأ في تنفيذ المزامنة: " + ex.Message);
                }
            }

            private MobileExportSummaryItem BuildPreview()
            {
                if (!(cboCollectors.SelectedItem is CollectorItem collector))
                    throw new InvalidOperationException("اختر المحصل أولًا.");

                if (!(cboDevices.SelectedItem is CollectorDeviceItem device))
                    throw new InvalidOperationException("اختر جهازًا معتمدًا ونشطًا أولًا.");

                return _service.ExportPreview(
                    collectorId: collector.CollectorID,
                    collectorName: collector.Name,
                    deviceCode: device.DeviceCode,
                    deviceName: device.DeviceName,
                    asOfDate: dtAsOfDate.Value.Date,
                    onlyAssignedSubscribers: chkOnlyAssignedSubscribers.Checked,
                    includeAllIfNoAssignments: chkIncludeAllIfNoAssignments.Checked);
            }

            private void ShowSummary(MobileExportSummaryItem summary)
            {
                lblSummary1.Text = $"المحصل: {summary.CollectorName}";
                lblSummary2.Text = $"الجهاز: {summary.DeviceName} / {summary.DeviceCode}";
                lblSummary3.Text = $"حتى تاريخ: {summary.AsOfDateText}";
                lblSummary4.Text = $"المشتركون: {summary.SubscribersCount} | العدادات: {summary.MetersCount}";
                lblSummary5.Text = $"الفواتير المفتوحة: {summary.OpenInvoicesCount} | الإجمالي: {summary.TotalOpenInvoicesAmount:N2}";
                lblSummary6.Text = $"الأرصدة: {summary.CreditsCount} | الإجمالي: {summary.TotalCreditsAmount:N2}";
            }

            private void Log(string message)
            {
                txtLog.AppendText($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
            }
        }
    }