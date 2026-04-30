using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using water3.Models;
using water3.Services;
using water3.Utils;

namespace water3.Forms
{
    public partial class CollectorDevicesForm : Form
    {
        private readonly CollectorDeviceService _service = new CollectorDeviceService();

        private List<CollectorDeviceItem> _devices = new List<CollectorDeviceItem>();
        private List<CollectorItem> _collectors = new List<CollectorItem>();

        private Label lblTitle;
        private ComboBox cboCollectors;
        private TextBox txtSearch;

        private Button btnSearch;
        private Button btnRefresh;
        private Button btnAddDevice;
        private Button btnEditDevice;
        private Button btnApprove;
        private Button btnUnapprove;
        private Button btnActivate;
        private Button btnDeactivate;
        private Button btnOpenSyncToPhone;

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
            BackColor = Color.FromArgb(245, 247, 250);
            WindowState = FormWindowState.Maximized;

            // ===== Root =====
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15),
                ColumnCount = 1,
                RowCount = 4,
                BackColor = Color.FromArgb(245, 247, 250)
            };

            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            Controls.Add(root);

            // ===== Title =====
            lblTitle = new Label
            {
                Text = "إدارة الأجهزة الجوالة للمحصلين",
                Font = new Font("Tahoma", 17F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 87, 183),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight
            };

            root.Controls.Add(lblTitle, 0, 0);

            // ===== Filter Card =====
            var filterPanel = CreateCardPanel();
            root.Controls.Add(filterPanel, 0, 1);

            var filterLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 6,
                RowCount = 1,
                Padding = new Padding(10)
            };

            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
            filterLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));

            filterPanel.Controls.Add(filterLayout);

            var lblCollector = CreateLabel("المحصل");
            filterLayout.Controls.Add(lblCollector, 0, 0);

            cboCollectors = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Margin = new Padding(5)
            };
            cboCollectors.SelectedIndexChanged += CboCollectors_SelectedIndexChanged;
            filterLayout.Controls.Add(cboCollectors, 1, 0);

            var lblSearch = CreateLabel("بحث");
            filterLayout.Controls.Add(lblSearch, 2, 0);

            txtSearch = new TextBox
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };
            txtSearch.KeyDown += TxtSearch_KeyDown;
            filterLayout.Controls.Add(txtSearch, 3, 0);

            btnSearch = MakeButton("بحث");
            btnSearch.Click += BtnSearch_Click;
            filterLayout.Controls.Add(btnSearch, 4, 0);

            btnRefresh = MakeButton("تحديث");
            btnRefresh.Click += BtnRefresh_Click;
            filterLayout.Controls.Add(btnRefresh, 5, 0);

            // ===== Toolbar =====
            var toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(0, 8, 0, 8),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            root.Controls.Add(toolbar, 0, 2);

            btnAddDevice = MakePrimaryButton("إضافة جهاز");
            btnAddDevice.Click += BtnAddDevice_Click;

            btnEditDevice = MakeButton("تعديل الجهاز");
            btnEditDevice.Click += BtnEditDevice_Click;

            btnApprove = MakeButton("اعتماد");
            btnApprove.Click += BtnApprove_Click;

            btnUnapprove = MakeButton("إلغاء الاعتماد");
            btnUnapprove.Width = 125;
            btnUnapprove.Click += BtnUnapprove_Click;

            btnActivate = MakeButton("تفعيل");
            btnActivate.Click += BtnActivate_Click;

            btnDeactivate = MakeButton("تعطيل");
            btnDeactivate.Click += BtnDeactivate_Click;

            btnOpenSyncToPhone = MakeButton("مزامنة للهاتف");
            btnOpenSyncToPhone.Width = 130;
            btnOpenSyncToPhone.Click += BtnOpenSyncToPhone_Click;

            toolbar.Controls.Add(btnAddDevice);
            toolbar.Controls.Add(btnEditDevice);
            toolbar.Controls.Add(btnApprove);
            toolbar.Controls.Add(btnUnapprove);
            toolbar.Controls.Add(btnActivate);
            toolbar.Controls.Add(btnDeactivate);
            toolbar.Controls.Add(btnOpenSyncToPhone);

            // ===== Grid Card =====
            var gridPanel = CreateCardPanel();
            root.Controls.Add(gridPanel, 0, 3);

            var gridLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 2,
                ColumnCount = 1
            };

            gridLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            gridLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 35));

            gridPanel.Controls.Add(gridLayout);

            dgvDevices = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false
            };

            dgvDevices.DoubleClick += DgvDevices_DoubleClick;

            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDeviceID",
                HeaderText = "DeviceID",
                DataPropertyName = "DeviceID",
                FillWeight = 55
            });

            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCollectorName",
                HeaderText = "المحصل",
                DataPropertyName = "CollectorName",
                FillWeight = 120
            });

            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDeviceCode",
                HeaderText = "كود الجهاز",
                DataPropertyName = "DeviceCode",
                FillWeight = 150
            });

            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDeviceName",
                HeaderText = "اسم الجهاز",
                DataPropertyName = "DeviceName",
                FillWeight = 120
            });

            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colPhoneNumber",
                HeaderText = "رقم الهاتف",
                DataPropertyName = "PhoneNumber",
                FillWeight = 100
            });

            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDeviceModel",
                HeaderText = "موديل الجهاز",
                DataPropertyName = "DeviceModel",
                FillWeight = 110
            });

            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colAppVersion",
                HeaderText = "إصدار التطبيق",
                DataPropertyName = "AppVersion",
                FillWeight = 90
            });

            dgvDevices.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colIsApproved",
                HeaderText = "معتمد",
                DataPropertyName = "IsApproved",
                FillWeight = 70
            });

            dgvDevices.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colIsActive",
                HeaderText = "نشط",
                DataPropertyName = "IsActive",
                FillWeight = 70
            });

            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colCreatedAt",
                HeaderText = "تاريخ الإنشاء",
                DataPropertyName = "CreatedAt",
                FillWeight = 110
            });

            dgvDevices.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colLastSyncAt",
                HeaderText = "آخر مزامنة",
                DataPropertyName = "LastSyncAt",
                FillWeight = 110
            });

            FormatGrid();

            lblStatus = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.DarkGreen,
                Font = new Font("Tahoma", 10F, FontStyle.Bold)
            };

            gridLayout.Controls.Add(dgvDevices, 0, 0);
            gridLayout.Controls.Add(lblStatus, 0, 1);
        }

        private Panel CreateCardPanel()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(8),
                Margin = new Padding(0, 0, 0, 8)
            };
        }

        private Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Tahoma", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 50, 50)
            };
        }

        private Button MakeButton(string text)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(105, 34),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(40, 40, 40),
                FlatStyle = FlatStyle.Flat,
                Margin = new Padding(5)
            };

            btn.FlatAppearance.BorderColor = Color.FromArgb(210, 215, 220);
            return btn;
        }

        private Button MakePrimaryButton(string text)
        {
            var btn = MakeButton(text);
            btn.BackColor = Color.FromArgb(0, 87, 183);
            btn.ForeColor = Color.White;
            btn.FlatAppearance.BorderColor = Color.FromArgb(0, 87, 183);
            return btn;
        }

        private void FormatGrid()
        {
            dgvDevices.EnableHeadersVisualStyles = false;
            dgvDevices.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
            dgvDevices.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(30, 45, 60);
            dgvDevices.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            dgvDevices.RowTemplate.Height = 32;
            dgvDevices.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(250, 250, 250);
        }

        private void ApplyPermissions()
        {
            PermissionHelper.ApplyControlPermission(btnAddDevice, "COLLECTOR_DEVICES_MANAGE");
            PermissionHelper.ApplyControlPermission(btnEditDevice, "COLLECTOR_DEVICES_MANAGE");
            PermissionHelper.ApplyControlPermission(btnActivate, "COLLECTOR_DEVICES_MANAGE");
            PermissionHelper.ApplyControlPermission(btnDeactivate, "COLLECTOR_DEVICES_MANAGE");
            PermissionHelper.ApplyControlPermission(btnApprove, "COLLECTOR_DEVICES_APPROVE");
            PermissionHelper.ApplyControlPermission(btnUnapprove, "COLLECTOR_DEVICES_APPROVE");

            // إن لم تكن عندك صلاحية خاصة للمزامنة، اتركها على نفس صلاحية الإدارة.
            PermissionHelper.ApplyControlPermission(btnOpenSyncToPhone, "COLLECTOR_DEVICES_MANAGE");
        }

        private void LoadCollectors()
        {
            try
            {
                _collectors = _service.GetCollectors();
                _collectors.Insert(0, new CollectorItem
                {
                    CollectorID = 0,
                    Name = "الكل"
                });

                cboCollectors.DataSource = null;
                cboCollectors.DataSource = _collectors;
                cboCollectors.DisplayMember = "Name";
                cboCollectors.ValueMember = "CollectorID";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ تحميل المحصلين", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDevices()
        {
            try
            {
                int selectedCollectorId = 0;

                if (cboCollectors.SelectedValue is int)
                    selectedCollectorId = (int)cboCollectors.SelectedValue;

                int? filterCollectorId = selectedCollectorId > 0
                    ? (int?)selectedCollectorId
                    : null;

                _devices = _service.GetDevices(txtSearch.Text.Trim(), filterCollectorId);

                dgvDevices.DataSource = null;
                dgvDevices.DataSource = _devices;

                lblStatus.ForeColor = Color.DarkGreen;
                lblStatus.Text = "عدد الأجهزة: " + _devices.Count;
            }
            catch (Exception ex)
            {
                lblStatus.ForeColor = Color.DarkRed;
                lblStatus.Text = ex.Message;
            }
        }

        private CollectorDeviceItem GetSelectedDevice()
        {
            if (dgvDevices.CurrentRow == null)
                return null;

            return dgvDevices.CurrentRow.DataBoundItem as CollectorDeviceItem;
        }

        private int GetSelectedCollectorId()
        {
            if (cboCollectors.SelectedValue is int)
                return (int)cboCollectors.SelectedValue;

            return 0;
        }

        private void CboCollectors_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDevices();
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoadDevices();
                e.SuppressKeyPress = true;
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadDevices();
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();

            if (cboCollectors.Items.Count > 0)
                cboCollectors.SelectedIndex = 0;

            LoadDevices();
        }

        private void BtnAddDevice_Click(object sender, EventArgs e)
        {
            int collectorId = GetSelectedCollectorId();

            if (collectorId <= 0)
            {
                MessageBox.Show("اختر محصلًا محددًا أولًا، لا يمكن إضافة جهاز على خيار الكل.",
                    "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var frm = new CollectorDeviceEditForm())
            {
                if (frm.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    _service.AddDevice(
                        collectorId,
                        frm.DeviceNameValue,
                        frm.PhoneNumberValue,
                        frm.DeviceCodeValue,
                        frm.DeviceModelValue,
                        frm.AppVersionValue,
                        frm.IsApprovedValue,
                        frm.IsActiveValue);

                    LoadDevices();

                    MessageBox.Show("تم إضافة الجهاز بنجاح.",
                        "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ إضافة الجهاز", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnEditDevice_Click(object sender, EventArgs e)
        {
            EditSelectedDevice();
        }

        private void DgvDevices_DoubleClick(object sender, EventArgs e)
        {
            EditSelectedDevice();
        }

        private void EditSelectedDevice()
        {
            var item = GetSelectedDevice();

            if (item == null)
            {
                MessageBox.Show("اختر جهازًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var frm = new CollectorDeviceEditForm(
                item.DeviceName,
                item.PhoneNumber,
                item.DeviceCode,
                item.DeviceModel,
                item.AppVersion,
                item.IsApproved,
                item.IsActive))
            {
                if (frm.ShowDialog(this) != DialogResult.OK)
                    return;

                try
                {
                    _service.UpdateDevice(
                        item.DeviceID,
                        frm.DeviceNameValue,
                        frm.PhoneNumberValue,
                        frm.DeviceCodeValue,
                        frm.DeviceModelValue,
                        frm.AppVersionValue,
                        frm.IsApprovedValue,
                        frm.IsActiveValue);

                    LoadDevices();

                    MessageBox.Show("تم تعديل الجهاز بنجاح.",
                        "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ تعديل الجهاز", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnApprove_Click(object sender, EventArgs e)
        {
            SetApproved(true);
        }

        private void BtnUnapprove_Click(object sender, EventArgs e)
        {
            SetApproved(false);
        }

        private void BtnActivate_Click(object sender, EventArgs e)
        {
            SetActive(true);
        }

        private void BtnDeactivate_Click(object sender, EventArgs e)
        {
            SetActive(false);
        }

        private void SetApproved(bool approved)
        {
            try
            {
                var item = GetSelectedDevice();

                if (item == null)
                {
                    MessageBox.Show("اختر جهازًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _service.SetApproved(item.DeviceID, approved);
                LoadDevices();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetActive(bool active)
        {
            try
            {
                var item = GetSelectedDevice();

                if (item == null)
                {
                    MessageBox.Show("اختر جهازًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _service.SetActive(item.DeviceID, active);
                LoadDevices();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnOpenSyncToPhone_Click(object sender, EventArgs e)
        {
            using (var frm = new MobileSyncToPhoneForm())
            {
                frm.ShowDialog(this);
            }
        }
    }
}
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.Linq;
//using System.Windows.Forms;
//using water3.Models;
//using water3.Services;
//using water3.Utils;
//namespace water3.Forms
//{
//    public partial class CollectorDevicesForm : Form
//    {


//        //public class CollectorDevicesForm : Form
//        //{
//            private readonly CollectorDeviceService _service = new CollectorDeviceService();
//            private List<CollectorDeviceItem> _devices = new List<CollectorDeviceItem>();
//            private List<CollectorItem> _collectors = new List<CollectorItem>();

//            private Label lblTitle;
//            private ComboBox cboCollectors;
//            private TextBox txtSearch;
//            private Button btnSearch;
//            private Button btnRefresh;
//            private Button btnApprove;
//            private Button btnUnapprove;
//            private Button btnActivate;
//            private Button btnDeactivate;
//            private Button btnEditDeviceName;
//            private DataGridView dgvDevices;
//            private Label lblStatus;

//            public CollectorDevicesForm()
//            {
//                InitializeComponent();
//                PermissionHelper.EnforceFormPermission(this, "COLLECTOR_DEVICES_VIEW");
//                ApplyPermissions();
//                LoadCollectors();
//                LoadDevices();
//            }

//            private void InitializeComponent()
//            {
//                Text = "إدارة الأجهزة الجوالة للمحصلين";
//                StartPosition = FormStartPosition.CenterScreen;
//                RightToLeft = RightToLeft.Yes;
//                RightToLeftLayout = true;
//                Font = new Font("Tahoma", 10F);
//                BackColor = Color.White;
//                WindowState = FormWindowState.Maximized;

//                lblTitle = new Label
//                {
//                    Text = "إدارة الأجهزة الجوالة للمحصلين",
//                    Font = new Font("Tahoma", 16F, FontStyle.Bold),
//                    ForeColor = Color.FromArgb(0, 102, 204),
//                    AutoSize = true,
//                    Location = new Point(20, 20)
//                };

//                cboCollectors = new ComboBox
//                {
//                    Location = new Point(20, 60),
//                    Size = new Size(220, 28),
//                    DropDownStyle = ComboBoxStyle.DropDownList
//                };
//                cboCollectors.SelectedIndexChanged += (s, e) => LoadDevices();

//                txtSearch = new TextBox
//                {
//                    Location = new Point(250, 60),
//                    Size = new Size(250, 27)
//                };
//                txtSearch.KeyDown += (s, e) =>
//                {
//                    if (e.KeyCode == Keys.Enter)
//                        LoadDevices();
//                };

//                btnSearch = MakeButton("بحث", 510, 58, 90);
//                btnSearch.Click += (s, e) => LoadDevices();

//                btnRefresh = MakeButton("تحديث", 610, 58, 90);
//                btnRefresh.Click += (s, e) =>
//                {
//                    txtSearch.Clear();
//                    cboCollectors.SelectedIndex = 0;
//                    LoadDevices();
//                };

//                btnApprove = MakeButton("اعتماد", 720, 58, 90);
//                btnApprove.Click += BtnApprove_Click;

//                btnUnapprove = MakeButton("إلغاء الاعتماد", 820, 58, 120);
//                btnUnapprove.Click += BtnUnapprove_Click;

//                btnActivate = MakeButton("تفعيل", 950, 58, 90);
//                btnActivate.Click += BtnActivate_Click;

//                btnDeactivate = MakeButton("تعطيل", 1050, 58, 90);
//                btnDeactivate.Click += BtnDeactivate_Click;

//                btnEditDeviceName = MakeButton("تعديل اسم الجهاز", 1150, 58, 130);
//                btnEditDeviceName.Click += BtnEditDeviceName_Click;

//                dgvDevices = new DataGridView
//                {
//                    Location = new Point(20, 100),
//                    Size = new Size(1260, 560),
//                    ReadOnly = true,
//                    AllowUserToAddRows = false,
//                    AllowUserToDeleteRows = false,
//                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
//                    MultiSelect = false,
//                    AutoGenerateColumns = false,
//                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
//                    BackgroundColor = Color.White,
//                    BorderStyle = BorderStyle.FixedSingle
//                };
//                dgvDevices.DoubleClick += (s, e) => EditSelectedDeviceName();

//                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDeviceID", HeaderText = "DeviceID", DataPropertyName = "DeviceID", FillWeight = 60 });
//                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCollectorName", HeaderText = "المحصل", DataPropertyName = "CollectorName", FillWeight = 120 });
//                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDeviceCode", HeaderText = "كود الجهاز", DataPropertyName = "DeviceCode", FillWeight = 150 });
//                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDeviceName", HeaderText = "اسم الجهاز", DataPropertyName = "DeviceName", FillWeight = 120 });
//                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDeviceModel", HeaderText = "موديل الجهاز", DataPropertyName = "DeviceModel", FillWeight = 110 });
//                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colAppVersion", HeaderText = "إصدار التطبيق", DataPropertyName = "AppVersion", FillWeight = 90 });
//                dgvDevices.Columns.Add(new DataGridViewCheckBoxColumn { Name = "colIsApproved", HeaderText = "معتمد", DataPropertyName = "IsApproved", FillWeight = 70 });
//                dgvDevices.Columns.Add(new DataGridViewCheckBoxColumn { Name = "colIsActive", HeaderText = "نشط", DataPropertyName = "IsActive", FillWeight = 70 });
//                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCreatedAt", HeaderText = "تاريخ الإنشاء", DataPropertyName = "CreatedAt", FillWeight = 110 });
//                dgvDevices.Columns.Add(new DataGridViewTextBoxColumn { Name = "colLastSyncAt", HeaderText = "آخر مزامنة", DataPropertyName = "LastSyncAt", FillWeight = 110 });

//                lblStatus = new Label
//                {
//                    Location = new Point(20, 670),
//                    Size = new Size(1260, 30),
//                    TextAlign = ContentAlignment.MiddleCenter,
//                    ForeColor = Color.DarkGreen
//                };

//                Controls.AddRange(new Control[]
//                {
//                lblTitle,
//                cboCollectors,
//                txtSearch,
//                btnSearch,
//                btnRefresh,
//                btnApprove,
//                btnUnapprove,
//                btnActivate,
//                btnDeactivate,
//                btnEditDeviceName,
//                dgvDevices,
//                lblStatus
//                });
//            }

//            private Button MakeButton(string text, int left, int top, int width)
//            {
//                return new Button
//                {
//                    Text = text,
//                    Location = new Point(left, top),
//                    Size = new Size(width, 32),
//                    BackColor = Color.FromArgb(0, 122, 204),
//                    ForeColor = Color.White,
//                    FlatStyle = FlatStyle.Flat
//                };
//            }

//            private void ApplyPermissions()
//            {
//                PermissionHelper.ApplyControlPermission(btnEditDeviceName, "COLLECTOR_DEVICES_MANAGE");
//                PermissionHelper.ApplyControlPermission(btnActivate, "COLLECTOR_DEVICES_MANAGE");
//                PermissionHelper.ApplyControlPermission(btnDeactivate, "COLLECTOR_DEVICES_MANAGE");
//                PermissionHelper.ApplyControlPermission(btnApprove, "COLLECTOR_DEVICES_APPROVE");
//                PermissionHelper.ApplyControlPermission(btnUnapprove, "COLLECTOR_DEVICES_APPROVE");
//            }

//            private void LoadCollectors()
//            {
//                _collectors = _service.GetCollectors();
//                _collectors.Insert(0, new CollectorItem { CollectorID = 0, Name = "الكل" });
//                cboCollectors.DataSource = _collectors;
//                cboCollectors.DisplayMember = "Name";
//                cboCollectors.ValueMember = "CollectorID";
//            }

//            private void LoadDevices()
//            {
//                try
//                {
//                    int selectedCollectorId = cboCollectors.SelectedValue is int ? (int)cboCollectors.SelectedValue : 0;
//                    int? filterCollectorId = selectedCollectorId > 0 ? (int?)selectedCollectorId : null;

//                    _devices = _service.GetDevices(txtSearch.Text, filterCollectorId);
//                    dgvDevices.DataSource = null;
//                    dgvDevices.DataSource = _devices;
//                    lblStatus.ForeColor = Color.DarkGreen;
//                    lblStatus.Text = $"عدد الأجهزة: {_devices.Count}";
//                }
//                catch (Exception ex)
//                {
//                    lblStatus.ForeColor = Color.DarkRed;
//                    lblStatus.Text = ex.Message;
//                }
//            }

//            private CollectorDeviceItem GetSelectedDevice()
//            {
//                return dgvDevices.CurrentRow?.DataBoundItem as CollectorDeviceItem;
//            }
//        private void btnOpenSyncToPhone_Click(object sender, EventArgs e)
//        {
//            using (var frm = new MobileSyncToPhoneForm())
//            {
//                frm.ShowDialog();
//            }
//        }

//        private void BtnApprove_Click(object sender, EventArgs e)
//            {
//                try
//                {
//                    var item = GetSelectedDevice();
//                    if (item == null)
//                    {
//                        MessageBox.Show("اختر جهازًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                        return;
//                    }

//                    _service.SetApproved(item.DeviceID, true);
//                    LoadDevices();
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }

//            private void BtnUnapprove_Click(object sender, EventArgs e)
//            {
//                try
//                {
//                    var item = GetSelectedDevice();
//                    if (item == null)
//                    {
//                        MessageBox.Show("اختر جهازًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                        return;
//                    }

//                    _service.SetApproved(item.DeviceID, false);
//                    LoadDevices();
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }

//            private void BtnActivate_Click(object sender, EventArgs e)
//            {
//                try
//                {
//                    var item = GetSelectedDevice();
//                    if (item == null)
//                    {
//                        MessageBox.Show("اختر جهازًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                        return;
//                    }

//                    _service.SetActive(item.DeviceID, true);
//                    LoadDevices();
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }

//            private void BtnDeactivate_Click(object sender, EventArgs e)
//            {
//                try
//                {
//                    var item = GetSelectedDevice();
//                    if (item == null)
//                    {
//                        MessageBox.Show("اختر جهازًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                        return;
//                    }

//                    _service.SetActive(item.DeviceID, false);
//                    LoadDevices();
//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                }
//            }

//            private void BtnEditDeviceName_Click(object sender, EventArgs e)
//            {
//                EditSelectedDeviceName();
//            }

//            private void EditSelectedDeviceName()
//            {
//                var item = GetSelectedDevice();
//                if (item == null)
//                {
//                    MessageBox.Show("اختر جهازًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
//                    return;
//                }

//                using (var frm = new DeviceNameEditForm(item.DeviceID, item.DeviceName))
//                {
//                    frm.ShowDialog();
//                }
//                LoadDevices();
//            }
//        }
//    }