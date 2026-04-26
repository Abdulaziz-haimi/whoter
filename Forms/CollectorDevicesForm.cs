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
using System.Linq;
using System.Windows.Forms;
using water3.Models;
using water3.Services;
using water3.Utils;
namespace water3.Forms
{
    public partial class CollectorDevicesForm : Form
    {


        //public class CollectorDevicesForm : Form
        //{
            private readonly CollectorDeviceService _service = new CollectorDeviceService();
            private List<CollectorDeviceItem> _devices = new List<CollectorDeviceItem>();
            private List<CollectorItem> _collectors = new List<CollectorItem>();

            private Label lblTitle;
            private ComboBox cboCollectors;
            private TextBox txtSearch;
            private Button btnSearch;
            private Button btnRefresh;
            private Button btnApprove;
            private Button btnUnapprove;
            private Button btnActivate;
            private Button btnDeactivate;
            private Button btnEditDeviceName;
            private DataGridView dgvDevices;
            private Label lblStatus;

            public CollectorDevicesForm()
            {
                InitializeComponent();
                PermissionHelper.EnforceFormPermission(this, "COLLECTOR_DEVICES_VIEW");
                ApplyPermissions();
                LoadCollectors();
                LoadDevices();
            }

            private void InitializeComponent()
            {
                Text = "إدارة الأجهزة الجوالة للمحصلين";
                StartPosition = FormStartPosition.CenterScreen;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;
                WindowState = FormWindowState.Maximized;

                lblTitle = new Label
                {
                    Text = "إدارة الأجهزة الجوالة للمحصلين",
                    Font = new Font("Tahoma", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };

                cboCollectors = new ComboBox
                {
                    Location = new Point(20, 60),
                    Size = new Size(220, 28),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };
                cboCollectors.SelectedIndexChanged += (s, e) => LoadDevices();

                txtSearch = new TextBox
                {
                    Location = new Point(250, 60),
                    Size = new Size(250, 27)
                };
                txtSearch.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                        LoadDevices();
                };

                btnSearch = MakeButton("بحث", 510, 58, 90);
                btnSearch.Click += (s, e) => LoadDevices();

                btnRefresh = MakeButton("تحديث", 610, 58, 90);
                btnRefresh.Click += (s, e) =>
                {
                    txtSearch.Clear();
                    cboCollectors.SelectedIndex = 0;
                    LoadDevices();
                };

                btnApprove = MakeButton("اعتماد", 720, 58, 90);
                btnApprove.Click += BtnApprove_Click;

                btnUnapprove = MakeButton("إلغاء الاعتماد", 820, 58, 120);
                btnUnapprove.Click += BtnUnapprove_Click;

                btnActivate = MakeButton("تفعيل", 950, 58, 90);
                btnActivate.Click += BtnActivate_Click;

                btnDeactivate = MakeButton("تعطيل", 1050, 58, 90);
                btnDeactivate.Click += BtnDeactivate_Click;

                btnEditDeviceName = MakeButton("تعديل اسم الجهاز", 1150, 58, 130);
                btnEditDeviceName.Click += BtnEditDeviceName_Click;

                dgvDevices = new DataGridView
                {
                    Location = new Point(20, 100),
                    Size = new Size(1260, 560),
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    AutoGenerateColumns = false,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle
                };
                dgvDevices.DoubleClick += (s, e) => EditSelectedDeviceName();

                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDeviceID", HeaderText = "DeviceID", DataPropertyName = "DeviceID", FillWeight = 60 });
                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCollectorName", HeaderText = "المحصل", DataPropertyName = "CollectorName", FillWeight = 120 });
                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDeviceCode", HeaderText = "كود الجهاز", DataPropertyName = "DeviceCode", FillWeight = 150 });
                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDeviceName", HeaderText = "اسم الجهاز", DataPropertyName = "DeviceName", FillWeight = 120 });
                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDeviceModel", HeaderText = "موديل الجهاز", DataPropertyName = "DeviceModel", FillWeight = 110 });
                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colAppVersion", HeaderText = "إصدار التطبيق", DataPropertyName = "AppVersion", FillWeight = 90 });
                dgvDevices.Columns.Add(new DataGridViewCheckBoxColumn { Name = "colIsApproved", HeaderText = "معتمد", DataPropertyName = "IsApproved", FillWeight = 70 });
                dgvDevices.Columns.Add(new DataGridViewCheckBoxColumn { Name = "colIsActive", HeaderText = "نشط", DataPropertyName = "IsActive", FillWeight = 70 });
                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCreatedAt", HeaderText = "تاريخ الإنشاء", DataPropertyName = "CreatedAt", FillWeight = 110 });
                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colLastSyncAt", HeaderText = "آخر مزامنة", DataPropertyName = "LastSyncAt", FillWeight = 110 });

                lblStatus = new Label
                {
                    Location = new Point(20, 670),
                    Size = new Size(1260, 30),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.DarkGreen
                };

                Controls.AddRange(new Control[]
                {
                lblTitle,
                cboCollectors,
                txtSearch,
                btnSearch,
                btnRefresh,
                btnApprove,
                btnUnapprove,
                btnActivate,
                btnDeactivate,
                btnEditDeviceName,
                dgvDevices,
                lblStatus
                });
            }

            private Button MakeButton(string text, int left, int top, int width)
            {
                return new Button
                {
                    Text = text,
                    Location = new Point(left, top),
                    Size = new Size(width, 32),
                    BackColor = Color.FromArgb(0, 122, 204),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
            }

            private void ApplyPermissions()
            {
                PermissionHelper.ApplyControlPermission(btnEditDeviceName, "COLLECTOR_DEVICES_MANAGE");
                PermissionHelper.ApplyControlPermission(btnActivate, "COLLECTOR_DEVICES_MANAGE");
                PermissionHelper.ApplyControlPermission(btnDeactivate, "COLLECTOR_DEVICES_MANAGE");
                PermissionHelper.ApplyControlPermission(btnApprove, "COLLECTOR_DEVICES_APPROVE");
                PermissionHelper.ApplyControlPermission(btnUnapprove, "COLLECTOR_DEVICES_APPROVE");
            }

            private void LoadCollectors()
            {
                _collectors = _service.GetCollectors();
                _collectors.Insert(0, new CollectorItem { CollectorID = 0, Name = "الكل" });
                cboCollectors.DataSource = _collectors;
                cboCollectors.DisplayMember = "Name";
                cboCollectors.ValueMember = "CollectorID";
            }

            private void LoadDevices()
            {
                try
                {
                    int selectedCollectorId = cboCollectors.SelectedValue is int ? (int)cboCollectors.SelectedValue : 0;
                    int? filterCollectorId = selectedCollectorId > 0 ? (int?)selectedCollectorId : null;

                    _devices = _service.GetDevices(txtSearch.Text, filterCollectorId);
                    dgvDevices.DataSource = null;
                    dgvDevices.DataSource = _devices;
                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = $"عدد الأجهزة: {_devices.Count}";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private CollectorDeviceItem GetSelectedDevice()
            {
                return dgvDevices.CurrentRow?.DataBoundItem as CollectorDeviceItem;
            }
        private void btnOpenSyncToPhone_Click(object sender, EventArgs e)
        {
            using (var frm = new MobileSyncToPhoneForm())
            {
                frm.ShowDialog();
            }
        }

        private void BtnApprove_Click(object sender, EventArgs e)
            {
                try
                {
                    var item = GetSelectedDevice();
                    if (item == null)
                    {
                        MessageBox.Show("اختر جهازًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    _service.SetApproved(item.DeviceID, true);
                    LoadDevices();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void BtnUnapprove_Click(object sender, EventArgs e)
            {
                try
                {
                    var item = GetSelectedDevice();
                    if (item == null)
                    {
                        MessageBox.Show("اختر جهازًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    _service.SetApproved(item.DeviceID, false);
                    LoadDevices();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void BtnActivate_Click(object sender, EventArgs e)
            {
                try
                {
                    var item = GetSelectedDevice();
                    if (item == null)
                    {
                        MessageBox.Show("اختر جهازًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    _service.SetActive(item.DeviceID, true);
                    LoadDevices();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void BtnDeactivate_Click(object sender, EventArgs e)
            {
                try
                {
                    var item = GetSelectedDevice();
                    if (item == null)
                    {
                        MessageBox.Show("اختر جهازًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    _service.SetActive(item.DeviceID, false);
                    LoadDevices();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void BtnEditDeviceName_Click(object sender, EventArgs e)
            {
                EditSelectedDeviceName();
            }

            private void EditSelectedDeviceName()
            {
                var item = GetSelectedDevice();
                if (item == null)
                {
                    MessageBox.Show("اختر جهازًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var frm = new DeviceNameEditForm(item.DeviceID, item.DeviceName))
                {
                    frm.ShowDialog();
                }
                LoadDevices();
            }
        }
    }