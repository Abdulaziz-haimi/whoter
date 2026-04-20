
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OfficeOpenXml;
//using LicenseContext = OfficeOpenXml.LicenseContext;

namespace water3
{
    public partial class SubscriberForm : Form
    {
        private int selectedSubscriberId = -1;
        private int selectedMeterId = -1;
        private DataTable _lastImportedTable;
        private readonly string connStr;
        private ToolTip toolTip;

        public SubscriberForm()
        {
            InitializeComponent();

            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            connStr = Db.ConnectionString;

            Load += SubscriberForm_Load;
            Shown += SubscriberForm_Shown;
            Resize += SubscriberForm_Resize;
            SizeChanged += SubscriberForm_Resize;
            VisibleChanged += SubscriberForm_VisibleChanged;
            Layout += SubscriberForm_Layout;
            ParentChanged += SubscriberForm_ParentChanged;

            InitializeForm();
        }

        private void InitializeForm()
        {
            try
            {
                Text = "إدارة المشتركين والعدادات";
                StartPosition = FormStartPosition.Manual;
                WindowState = FormWindowState.Normal;
                BackColor = Color.FromArgb(240, 242, 245);
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                AutoScaleMode = AutoScaleMode.Dpi;
                AutoScroll = false;

                TopLevel = false;
                FormBorderStyle = FormBorderStyle.None;
                Dock = DockStyle.Fill;
                Margin = new Padding(0);
                Padding = new Padding(0);

                ApplyCustomStyles();
                EnableDoubleBuffer();
                InitializeToolTips();
                HookEvents();
                StyleDataGridViews();
                LoadAccounts();
                UpdateButtonsState();

                txtMeterReading.Text = "0";
                dateFilter.Value = DateTime.Today;

                LoadSubscribers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تهيئة النموذج: {ex.Message}", "خطأ فادح",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SubscriberForm_Load(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                if (splitGrids != null)
                    splitGrids.SplitterWidth = 5;

                ApplyResponsiveLayout();
                SetSafeSplitterDistance();
            }));
        }

        private void SubscriberForm_Shown(object sender, EventArgs e)
        {
            BeginInvoke(new Action(ApplyResponsiveLayout));
        }

        private void SubscriberForm_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
                BeginInvoke(new Action(ApplyResponsiveLayout));
        }

        private void SubscriberForm_Layout(object sender, LayoutEventArgs e)
        {
            if (Visible && IsHandleCreated)
                BeginInvoke(new Action(ApplyResponsiveLayout));
        }

        private void SubscriberForm_ParentChanged(object sender, EventArgs e)
        {
            if (Visible && IsHandleCreated)
                BeginInvoke(new Action(ApplyResponsiveLayout));
        }

        private void SubscriberForm_Resize(object sender, EventArgs e)
        {
            if (!Visible || WindowState == FormWindowState.Minimized)
                return;

            ApplyResponsiveLayout();
        }

        private void InitializeToolTips()
        {
            try
            {
                toolTip = new ToolTip();
                toolTip.SetToolTip(btnAdd, "إضافة مشترك جديد");
                toolTip.SetToolTip(btnUpdate, "تعديل بيانات المشترك");
                toolTip.SetToolTip(btnClear, "تفريغ جميع الحقول");
                toolTip.SetToolTip(btnCreateAccount, "إنشاء حساب ذمة جديد");
                toolTip.SetToolTip(btnAddMeter, "إضافة عداد جديد للمشترك");
                toolTip.SetToolTip(btnSearch, "بحث بالاسم");
                toolTip.SetToolTip(btnFilterDate, "تصفية حسب تاريخ الإضافة");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ToolTip error: {ex.Message}");
            }
        }

        private void HookEvents()
        {
            try
            {
                btnAdd.Click += BtnAdd_Click;
                btnUpdate.Click += BtnUpdate_Click;
                btnClear.Click += BtnClear_Click;
                btnCreateAccount.Click += BtnCreateAccount_Click;

                btnAddMeter.Click += BtnAddMeter_Click;
                btnSetPrimary.Click += BtnSetPrimary_Click;
                btnUnlinkMeter.Click += BtnUnlinkMeter_Click;
                btnImportData.Click += BtnImportData_Click;
                btnSaveImported.Click += BtnSaveImported_Click;

                btnSearch.Click += BtnSearch_Click;
                btnFilterDate.Click += BtnFilterDate_Click;
                txtSearch.KeyDown += TxtSearch_KeyDown;
                txtPhone.KeyPress += TxtPhone_KeyPress;

                dgvSubscribers.CellClick += DgvSubscribers_CellClick;
                dgvSubscribers.CellContentClick += DgvSubscribers_CellContentClick;
                dgvMeters.CellClick += DgvMeters_CellClick;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في ربط الأحداث: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StyleDataGridViews()
        {
            try
            {
                StyleGrid(dgvSubscribers);
                StyleGrid(dgvMeters);
                AddDeleteButtonColumn();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Style error: {ex.Message}");
            }
        }

        private void StyleGrid(DataGridView dgv)
        {
            if (dgv == null) return;

            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.EnableHeadersVisualStyles = false;
            dgv.GridColor = Color.FromArgb(229, 231, 235);
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight = 38;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9F, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Tahoma", 9F);
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(52, 73, 94);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            dgv.RowTemplate.Height = 35;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.RightToLeft = RightToLeft.Yes;
        }

        private void AddDeleteButtonColumn()
        {
            try
            {
                if (!dgvSubscribers.Columns.Contains("btnDelete"))
                {
                    var btnDelete = new DataGridViewButtonColumn
                    {
                        Name = "btnDelete",
                        HeaderText = "تعطيل",
                        Text = "تعطيل",
                        UseColumnTextForButtonValue = true,
                        Width = 60,
                        FlatStyle = FlatStyle.Flat
                    };

                    dgvSubscribers.Columns.Add(btnDelete);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddDeleteButton error: {ex.Message}");
            }
        }

        private void UpdateButtonsState()
        {
            try
            {
                bool hasSubscriber = selectedSubscriberId > 0;
                bool hasMeter = selectedMeterId > 0;

                btnUpdate.Enabled = hasSubscriber;
                btnCreateAccount.Enabled = hasSubscriber;
                btnAddMeter.Enabled = hasSubscriber;
                btnSetPrimary.Enabled = hasSubscriber && hasMeter;
                btnUnlinkMeter.Enabled = hasSubscriber && hasMeter;
                btnSaveImported.Enabled = _lastImportedTable != null && _lastImportedTable.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateButtons error: {ex.Message}");
            }
        }

        private void ClearFields(bool clearSearch = true)
        {
            try
            {
                selectedSubscriberId = -1;
                selectedMeterId = -1;

                txtName.Clear();
                txtPhone.Clear();
                txtAddress.Clear();
                txtMeterReading.Text = "0";
                txtNewMeterNumber.Clear();
                txtNewMeterLocation.Clear();
                chkIsPrimary.Checked = true;

                if (clearSearch)
                    txtSearch.Clear();

                if (cboAccount.Items.Count > 0)
                    cboAccount.SelectedIndex = -1;

                dgvMeters.DataSource = null;
                UpdateButtonsState();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ClearFields error: {ex.Message}");
            }
        }

        private void LoadAccounts()
        {
            try
            {
                if (string.IsNullOrEmpty(connStr))
                {
                    MessageBox.Show("سلسلة اتصال قاعدة البيانات غير محددة", "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var con = new SqlConnection(connStr))
                {
                    const string query = @"
                        SELECT AccountID, AccountCode + N' - ' + AccountName AS DisplayName
                        FROM Accounts
                        WHERE AccountType <> 'Revenue'
                        ORDER BY AccountCode";

                    var da = new SqlDataAdapter(query, con);
                    var dt = new DataTable();
                    da.Fill(dt);

                    cboAccount.DataSource = dt;
                    cboAccount.DisplayMember = "DisplayName";
                    cboAccount.ValueMember = "AccountID";
                    cboAccount.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الحسابات: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSubscribers(string searchText = "", DateTime? filterDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(connStr))
                {
                    MessageBox.Show("سلسلة اتصال قاعدة البيانات غير محددة", "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (var con = new SqlConnection(connStr))
                {
                    string query = @"
                        SELECT 
                            S.SubscriberID,
                            S.Name,
                            S.PhoneNumber,
                            S.Address,
                            S.CreatedDate,
                            S.AccountID,
                            A.AccountCode + N' - ' + A.AccountName AS AccountName,
                            (
                                SELECT TOP 1 m.MeterNumber
                                FROM SubscriberMeters sm
                                JOIN Meters m ON sm.MeterID = m.MeterID
                                WHERE sm.SubscriberID = S.SubscriberID AND sm.IsPrimary = 1
                            ) AS PrimaryMeterNumber
                        FROM Subscribers S
                        LEFT JOIN Accounts A ON S.AccountID = A.AccountID
                        WHERE S.IsActive = 1";

                    if (!string.IsNullOrWhiteSpace(searchText))
                        query += " AND S.Name LIKE @SearchText";

                    if (filterDate.HasValue)
                        query += " AND CAST(S.CreatedDate AS DATE) = @FilterDate";

                    query += " ORDER BY S.SubscriberID DESC";

                    var da = new SqlDataAdapter(query, con);

                    if (!string.IsNullOrWhiteSpace(searchText))
                        da.SelectCommand.Parameters.AddWithValue("@SearchText", "%" + searchText.Trim() + "%");

                    if (filterDate.HasValue)
                        da.SelectCommand.Parameters.AddWithValue("@FilterDate", filterDate.Value.Date);

                    var dt = new DataTable();
                    da.Fill(dt);

                    dgvSubscribers.DataSource = dt;
                    FormatSubscribersGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل المشتركين: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatSubscribersGrid()
        {
            try
            {
                if (dgvSubscribers.Columns.Contains("SubscriberID"))
                    dgvSubscribers.Columns["SubscriberID"].HeaderText = "رقم";

                if (dgvSubscribers.Columns.Contains("Name"))
                    dgvSubscribers.Columns["Name"].HeaderText = "الاسم";

                if (dgvSubscribers.Columns.Contains("PrimaryMeterNumber"))
                    dgvSubscribers.Columns["PrimaryMeterNumber"].HeaderText = "العداد الأساسي";

                if (dgvSubscribers.Columns.Contains("PhoneNumber"))
                    dgvSubscribers.Columns["PhoneNumber"].HeaderText = "الهاتف";

                if (dgvSubscribers.Columns.Contains("Address"))
                    dgvSubscribers.Columns["Address"].HeaderText = "العنوان";

                if (dgvSubscribers.Columns.Contains("CreatedDate"))
                {
                    dgvSubscribers.Columns["CreatedDate"].HeaderText = "تاريخ الإضافة";
                    dgvSubscribers.Columns["CreatedDate"].DefaultCellStyle.Format = "yyyy/MM/dd";
                }

                if (dgvSubscribers.Columns.Contains("AccountName"))
                    dgvSubscribers.Columns["AccountName"].HeaderText = "حساب الذمة";

                if (dgvSubscribers.Columns.Contains("AccountID"))
                    dgvSubscribers.Columns["AccountID"].Visible = false;

                AddDeleteButtonColumn();

                if (dgvSubscribers.Columns.Contains("btnDelete"))
                    dgvSubscribers.Columns["btnDelete"].DisplayIndex = dgvSubscribers.Columns.Count - 1;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FormatSubscribersGrid error: {ex.Message}");
            }
        }

        private void LoadMetersForSubscriber(int subscriberId)
        {
            try
            {
                if (string.IsNullOrEmpty(connStr)) return;

                using (var con = new SqlConnection(connStr))
                {
                    const string query = @"
                        SELECT 
                            m.MeterID,
                            m.MeterNumber,
                            m.Location,
                            sm.IsPrimary,
                            sm.LinkedAt
                        FROM SubscriberMeters sm
                        JOIN Meters m ON sm.MeterID = m.MeterID
                        WHERE sm.SubscriberID = @SubscriberID
                        ORDER BY sm.IsPrimary DESC, m.MeterNumber";

                    var da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@SubscriberID", subscriberId);

                    var dt = new DataTable();
                    da.Fill(dt);

                    dgvMeters.DataSource = dt;
                    FormatMetersGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل العدادات: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatMetersGrid()
        {
            try
            {
                if (dgvMeters.Columns.Contains("MeterID"))
                    dgvMeters.Columns["MeterID"].Visible = false;

                if (dgvMeters.Columns.Contains("MeterNumber"))
                {
                    dgvMeters.Columns["MeterNumber"].HeaderText = "رقم العداد";
                    dgvMeters.Columns["MeterNumber"].Width = 150;
                }

                if (dgvMeters.Columns.Contains("Location"))
                {
                    dgvMeters.Columns["Location"].HeaderText = "الموقع";
                    dgvMeters.Columns["Location"].Width = 200;
                }

                if (dgvMeters.Columns.Contains("IsPrimary"))
                {
                    dgvMeters.Columns["IsPrimary"].HeaderText = "أساسي";
                    dgvMeters.Columns["IsPrimary"].Width = 80;
                }

                if (dgvMeters.Columns.Contains("LinkedAt"))
                {
                    dgvMeters.Columns["LinkedAt"].HeaderText = "تاريخ الربط";
                    dgvMeters.Columns["LinkedAt"].Width = 150;
                    dgvMeters.Columns["LinkedAt"].DefaultCellStyle.Format = "yyyy/MM/dd HH:mm";
                }

                foreach (DataGridViewRow row in dgvMeters.Rows)
                {
                    if (row.Cells["IsPrimary"].Value != null && Convert.ToBoolean(row.Cells["IsPrimary"].Value))
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
                        row.DefaultCellStyle.Font = new Font("Tahoma", 9, FontStyle.Bold);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FormatMetersGrid error: {ex.Message}");
            }
        }

        private (int? SubscriberId, int? MeterId) ExecuteStoredProcedure(string action, Action<SqlCommand> setParameters)
        {
            try
            {
                using (var con = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("dbo.usp_Subscriber_Manage", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", action);

                    var pSubscriberId = new SqlParameter("@SubscriberID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = selectedSubscriberId > 0 ? (object)selectedSubscriberId : DBNull.Value
                    };

                    var pMeterId = new SqlParameter("@MeterID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = selectedMeterId > 0 ? (object)selectedMeterId : DBNull.Value
                    };

                    cmd.Parameters.Add(pSubscriberId);
                    cmd.Parameters.Add(pMeterId);
                    setParameters?.Invoke(cmd);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    int? newSubscriberId = pSubscriberId.Value == DBNull.Value ? (int?)null : Convert.ToInt32(pSubscriberId.Value);
                    int? newMeterId = pMeterId.Value == DBNull.Value ? (int?)null : Convert.ToInt32(pMeterId.Value);

                    if (newSubscriberId.HasValue) selectedSubscriberId = newSubscriberId.Value;
                    if (newMeterId.HasValue) selectedMeterId = newMeterId.Value;

                    return (newSubscriberId, newMeterId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في تنفيذ الإجراء {action}: {ex.Message}", ex);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("اسم المشترك مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNewMeterNumber.Text))
                {
                    MessageBox.Show("رقم العداد مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNewMeterNumber.Focus();
                    return;
                }

                if (cboAccount.SelectedValue == null)
                {
                    MessageBox.Show("اختر حساب الذمة للمشترك", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboAccount.Focus();
                    return;
                }

                if (!decimal.TryParse(txtMeterReading.Text, out decimal initialReading))
                {
                    MessageBox.Show("القراءة الابتدائية غير صحيحة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMeterReading.Focus();
                    return;
                }

                ExecuteStoredProcedure("ADD_SUBSCRIBER", cmd =>
                {
                    cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(txtPhone.Text) ? DBNull.Value : (object)txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(txtAddress.Text) ? DBNull.Value : (object)txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@AccountID", Convert.ToInt32(cboAccount.SelectedValue));
                    cmd.Parameters.AddWithValue("@MeterNumber", txtNewMeterNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@MeterLocation", string.IsNullOrWhiteSpace(txtNewMeterLocation.Text) ? DBNull.Value : (object)txtNewMeterLocation.Text.Trim());
                    cmd.Parameters.AddWithValue("@IsPrimary", true);
                    cmd.Parameters.AddWithValue("@InitialReading", initialReading);
                });

                MessageBox.Show("تمت إضافة المشترك والعداد بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadSubscribers();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء الإضافة: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedSubscriberId <= 0)
                {
                    MessageBox.Show("اختر مشتركًا للتعديل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtName.Text))
                {
                    MessageBox.Show("اسم المشترك مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName.Focus();
                    return;
                }

                if (cboAccount.SelectedValue == null)
                {
                    MessageBox.Show("اختر حساب الذمة للمشترك", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboAccount.Focus();
                    return;
                }

                ExecuteStoredProcedure("UPDATE_SUBSCRIBER", cmd =>
                {
                    cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(txtPhone.Text) ? DBNull.Value : (object)txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(txtAddress.Text) ? DBNull.Value : (object)txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@AccountID", Convert.ToInt32(cboAccount.SelectedValue));
                });

                MessageBox.Show("تم تعديل بيانات المشترك بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadSubscribers();

                if (selectedSubscriberId > 0)
                    LoadMetersForSubscriber(selectedSubscriberId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء التعديل: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void BtnCreateAccount_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedSubscriberId <= 0)
                {
                    MessageBox.Show("اختر مشتركًا أولًا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var con = new SqlConnection(connStr))
                using (var cmd = new SqlCommand("dbo.CreateSubscriberAccount", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SubscriberID", selectedSubscriberId);

                    var outAcc = new SqlParameter("@AccountID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outAcc);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    int newAccountId = Convert.ToInt32(outAcc.Value);

                    MessageBox.Show("تم إنشاء وربط حساب الذمة بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadAccounts();
                    cboAccount.SelectedValue = newAccountId;
                    LoadSubscribers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAddMeter_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedSubscriberId <= 0)
                {
                    MessageBox.Show("اختر مشتركًا أولًا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNewMeterNumber.Text))
                {
                    MessageBox.Show("رقم العداد مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNewMeterNumber.Focus();
                    return;
                }

                if (!decimal.TryParse(txtMeterReading.Text, out decimal initialReading))
                {
                    MessageBox.Show("القراءة الابتدائية غير صحيحة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMeterReading.Focus();
                    return;
                }

                ExecuteStoredProcedure("ADD_METER", cmd =>
                {
                    cmd.Parameters.AddWithValue("@MeterNumber", txtNewMeterNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@MeterLocation", string.IsNullOrWhiteSpace(txtNewMeterLocation.Text) ? DBNull.Value : (object)txtNewMeterLocation.Text.Trim());
                    cmd.Parameters.AddWithValue("@IsPrimary", chkIsPrimary.Checked);
                    cmd.Parameters.AddWithValue("@InitialReading", initialReading);
                });

                MessageBox.Show("تمت إضافة العداد بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMetersForSubscriber(selectedSubscriberId);
                LoadSubscribers();

                txtNewMeterNumber.Clear();
                txtNewMeterLocation.Clear();
                chkIsPrimary.Checked = false;
                txtMeterReading.Text = "0";
                UpdateButtonsState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSetPrimary_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedSubscriberId <= 0 || selectedMeterId <= 0)
                {
                    MessageBox.Show("اختر مشتركًا ثم اختر عدادًا من جدول العدادات", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ExecuteStoredProcedure("SET_PRIMARY_METER", null);
                MessageBox.Show("تم تعيين العداد الأساسي بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMetersForSubscriber(selectedSubscriberId);
                LoadSubscribers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUnlinkMeter_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedSubscriberId <= 0 || selectedMeterId <= 0)
                {
                    MessageBox.Show("اختر مشتركًا ثم اختر عدادًا لفك الربط", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("هل تريد فك ربط هذا العداد من المشترك؟", "تأكيد",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ExecuteStoredProcedure("UNLINK_METER", null);
                    MessageBox.Show("تم فك الربط بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMetersForSubscriber(selectedSubscriberId);
                    LoadSubscribers();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnImportData_Click(object sender, EventArgs e)
        {
            try
            {
                using (var openFileDialog = new OpenFileDialog())
                {
                    // 3) غيّر فلتر الملفات
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xlsm";
                    openFileDialog.Title = "اختر ملف Excel";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        ImportExcelData(openFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSaveImported_Click(object sender, EventArgs e)
        {
            try
            {
                if (_lastImportedTable == null || _lastImportedTable.Rows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات مستوردة للحفظ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveImportedData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImportExcelData(string filePath)
        {
            try
            {
               // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    if (worksheet == null || worksheet.Dimension == null)
                    {
                        MessageBox.Show("الملف لا يحتوي على بيانات", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var dt = new DataTable();
                    dt.Columns.Add("RowNumber", typeof(int));
                    dt.Columns.Add("SubscriberName", typeof(string));
                    dt.Columns.Add("PhoneNumber", typeof(string));
                    dt.Columns.Add("Address", typeof(string));
                    dt.Columns.Add("MeterNumber", typeof(string));
                    dt.Columns.Add("MeterLocation", typeof(string));
                    dt.Columns.Add("InitialReading", typeof(decimal));
                    dt.Columns.Add("IsPrimary", typeof(bool));
                    dt.Columns.Add("AccountCode", typeof(string));
                    dt.Columns.Add("Status", typeof(string));

                    int startRow = worksheet.Dimension.Start.Row + 1;
                    int endRow = worksheet.Dimension.End.Row;

                    for (int row = startRow; row <= endRow; row++)
                    {
                        string subscriberName = worksheet.Cells[row, 1]?.Text?.Trim();
                        if (string.IsNullOrWhiteSpace(subscriberName))
                            continue;

                        DataRow dr = dt.NewRow();
                        dr["RowNumber"] = row;
                        dr["SubscriberName"] = subscriberName;
                        dr["PhoneNumber"] = worksheet.Cells[row, 2]?.Text?.Trim() ?? "";
                        dr["Address"] = worksheet.Cells[row, 3]?.Text?.Trim() ?? "";
                        dr["MeterNumber"] = worksheet.Cells[row, 4]?.Text?.Trim() ?? "";
                        dr["MeterLocation"] = worksheet.Cells[row, 5]?.Text?.Trim() ?? "";

                        if (decimal.TryParse(worksheet.Cells[row, 6]?.Text?.Trim(), out decimal reading))
                            dr["InitialReading"] = reading;
                        else
                            dr["InitialReading"] = 0;

                        dr["IsPrimary"] = true;
                        dr["AccountCode"] = worksheet.Cells[row, 7]?.Text?.Trim() ?? "";

                        var errors = new List<string>();
                        if (string.IsNullOrWhiteSpace(dr["SubscriberName"].ToString()))
                            errors.Add("اسم المشترك مطلوب");
                        if (string.IsNullOrWhiteSpace(dr["MeterNumber"].ToString()))
                            errors.Add("رقم العداد مطلوب");
                        if (Convert.ToDecimal(dr["InitialReading"]) < 0)
                            errors.Add("القراءة الابتدائية غير صحيحة");

                        dr["Status"] = errors.Count > 0 ? string.Join(" | ", errors) : "جاهز للحفظ";
                        dt.Rows.Add(dr);
                    }

                    _lastImportedTable = dt;
                    dgvSubscribers.DataSource = dt;

                    foreach (DataGridViewColumn col in dgvSubscribers.Columns)
                    {
                        if (col.Name != "btnDelete")
                            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    }

                    if (dgvSubscribers.Columns.Contains("RowNumber"))
                        dgvSubscribers.Columns["RowNumber"].HeaderText = "رقم الصف";
                    if (dgvSubscribers.Columns.Contains("SubscriberName"))
                        dgvSubscribers.Columns["SubscriberName"].HeaderText = "اسم المشترك";
                    if (dgvSubscribers.Columns.Contains("MeterNumber"))
                        dgvSubscribers.Columns["MeterNumber"].HeaderText = "رقم العداد";
                    if (dgvSubscribers.Columns.Contains("InitialReading"))
                        dgvSubscribers.Columns["InitialReading"].HeaderText = "القراءة الابتدائية";
                    if (dgvSubscribers.Columns.Contains("Status"))
                        dgvSubscribers.Columns["Status"].HeaderText = "الحالة";
                    if (dgvSubscribers.Columns.Contains("btnDelete"))
                        dgvSubscribers.Columns["btnDelete"].Visible = false;

                    UpdateButtonsState();
                    MessageBox.Show($"تم استيراد {dt.Rows.Count} صف بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في قراءة ملف Excel: {ex.Message}", ex);
            }
        }

        private void SaveImportedData()
        {
            try
            {
                var validRows = _lastImportedTable.AsEnumerable()
                    .Where(row => row["Status"].ToString() == "جاهز للحفظ")
                    .ToList();

                if (validRows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات صالحة للحفظ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int successCount = 0;
                int failCount = 0;

                foreach (var row in validRows)
                {
                    try
                    {
                        ExecuteStoredProcedure("ADD_SUBSCRIBER", cmd =>
                        {
                            cmd.Parameters.AddWithValue("@Name", row["SubscriberName"].ToString());
                            cmd.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(row["PhoneNumber"]?.ToString()) ? DBNull.Value : (object)row["PhoneNumber"].ToString());
                            cmd.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(row["Address"]?.ToString()) ? DBNull.Value : (object)row["Address"].ToString());
                            cmd.Parameters.AddWithValue("@AccountID", DBNull.Value);
                            cmd.Parameters.AddWithValue("@MeterNumber", row["MeterNumber"].ToString());
                            cmd.Parameters.AddWithValue("@MeterLocation", string.IsNullOrWhiteSpace(row["MeterLocation"]?.ToString()) ? DBNull.Value : (object)row["MeterLocation"].ToString());
                            cmd.Parameters.AddWithValue("@IsPrimary", Convert.ToBoolean(row["IsPrimary"]));
                            cmd.Parameters.AddWithValue("@InitialReading", Convert.ToDecimal(row["InitialReading"]));
                        });

                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        row["Status"] = $"خطأ: {ex.Message}";
                    }
                }

                MessageBox.Show($"تم حفظ {successCount} مشترك بنجاح\nفشل {failCount} مشترك",
                    "نتيجة الاستيراد", MessageBoxButtons.OK,
                    failCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

                LoadSubscribers();
                ClearFields();
                _lastImportedTable = null;
                UpdateButtonsState();
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في حفظ البيانات: {ex.Message}", ex);
            }
        }

        private void DgvSubscribers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0 || dgvSubscribers.Rows[e.RowIndex] == null)
                    return;

                if (!dgvSubscribers.Columns.Contains("SubscriberID")) return;
                if (dgvSubscribers.Rows[e.RowIndex].Cells["SubscriberID"].Value == null) return;
                if (dgvSubscribers.Rows[e.RowIndex].Cells["SubscriberID"].Value == DBNull.Value) return;

                selectedSubscriberId = Convert.ToInt32(dgvSubscribers.Rows[e.RowIndex].Cells["SubscriberID"].Value);

                if (dgvSubscribers.Columns.Contains("Name"))
                    txtName.Text = dgvSubscribers.Rows[e.RowIndex].Cells["Name"].Value?.ToString() ?? "";

                if (dgvSubscribers.Columns.Contains("PhoneNumber"))
                    txtPhone.Text = dgvSubscribers.Rows[e.RowIndex].Cells["PhoneNumber"].Value?.ToString() ?? "";

                if (dgvSubscribers.Columns.Contains("Address"))
                    txtAddress.Text = dgvSubscribers.Rows[e.RowIndex].Cells["Address"].Value?.ToString() ?? "";

                if (dgvSubscribers.Columns.Contains("AccountID") &&
                    dgvSubscribers.Rows[e.RowIndex].Cells["AccountID"].Value != null &&
                    dgvSubscribers.Rows[e.RowIndex].Cells["AccountID"].Value != DBNull.Value)
                {
                    int accountId = Convert.ToInt32(dgvSubscribers.Rows[e.RowIndex].Cells["AccountID"].Value);
                    if (cboAccount.Items.Count > 0)
                    {
                        try { cboAccount.SelectedValue = accountId; } catch { }
                    }
                }

                LoadMetersForSubscriber(selectedSubscriberId);
                UpdateButtonsState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحديد المشترك: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvSubscribers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                if (!dgvSubscribers.Columns.Contains("btnDelete")) return;
                if (e.ColumnIndex < 0 || e.ColumnIndex >= dgvSubscribers.Columns.Count) return;
                if (dgvSubscribers.Columns[e.ColumnIndex].Name != "btnDelete") return;

                DataGridViewRow row = dgvSubscribers.Rows[e.RowIndex];
                if (row.Cells["SubscriberID"].Value == null) return;

                int id = Convert.ToInt32(row.Cells["SubscriberID"].Value);
                string name = row.Cells["Name"].Value?.ToString() ?? "";

                if (MessageBox.Show($"هل تريد تعطيل المشترك '{name}'؟", "تأكيد",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    selectedSubscriberId = id;
                    ExecuteStoredProcedure("DISABLE_SUBSCRIBER", null);
                    MessageBox.Show("تم تعطيل المشترك بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSubscribers();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvMeters_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                if (!dgvMeters.Columns.Contains("MeterID")) return;

                DataGridViewRow row = dgvMeters.Rows[e.RowIndex];
                if (row.Cells["MeterID"].Value == null) return;

                selectedMeterId = Convert.ToInt32(row.Cells["MeterID"].Value);

                if (dgvMeters.Columns.Contains("MeterNumber"))
                    txtNewMeterNumber.Text = row.Cells["MeterNumber"].Value?.ToString() ?? "";

                if (dgvMeters.Columns.Contains("Location"))
                    txtNewMeterLocation.Text = row.Cells["Location"].Value?.ToString() ?? "";

                if (dgvMeters.Columns.Contains("IsPrimary"))
                    chkIsPrimary.Checked = row.Cells["IsPrimary"].Value != null && Convert.ToBoolean(row.Cells["IsPrimary"].Value);

                UpdateButtonsState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadSubscribers(txtSearch.Text.Trim(), null);
                ClearFields(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في البحث: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnFilterDate_Click(object sender, EventArgs e)
        {
            try
            {
                LoadSubscribers("", dateFilter.Value.Date);
                ClearFields(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في التصفية: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnSearch_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }

        private void TxtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }
    }
}


/*using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using OfficeOpenXml;
using System.Reflection;
namespace water3
{
    public partial class SubscriberForm : Form
    {
        // متغيرات الحالة
        private int selectedSubscriberId = -1;
        private int selectedMeterId = -1;
        private DataTable _lastImportedTable;
        private readonly string connStr;
        private ToolTip toolTip;

        public SubscriberForm()
        {
            InitializeComponent();
            connStr = Db.ConnectionString;
            InitializeComponentSecurity();
            InitializeForm();
        }

        private void InitializeComponentSecurity()
        {
            // التأكد من أن جميع المكونات تم إنشاؤها
            if (mainLayout == null) mainLayout = new TableLayoutPanel();
            if (pnlFieldsCard == null) pnlFieldsCard = new Panel();
            if (pnlSearchCard == null) pnlSearchCard = new Panel();
            if (fieldsAndMetersLayout == null) fieldsAndMetersLayout = new TableLayoutPanel();
            if (tblSubscriberFields == null) tblSubscriberFields = new TableLayoutPanel();
            if (pnlSubscriberButtons == null) pnlSubscriberButtons = new FlowLayoutPanel();
            if (tblMeters == null) tblMeters = new TableLayoutPanel();
            if (pnlSearch == null) pnlSearch = new FlowLayoutPanel();
            if (splitGrids == null) splitGrids = new SplitContainer();
            if (dgvSubscribers == null) dgvSubscribers = new DataGridView();
            if (dgvMeters == null) dgvMeters = new DataGridView();

            // التأكد من وجود الأزرار
            if (btnAdd == null) btnAdd = new Button();
            if (btnUpdate == null) btnUpdate = new Button();
            if (btnClear == null) btnClear = new Button();
            if (btnCreateAccount == null) btnCreateAccount = new Button();
            if (btnAddMeter == null) btnAddMeter = new Button();
            if (btnSetPrimary == null) btnSetPrimary = new Button();
            if (btnUnlinkMeter == null) btnUnlinkMeter = new Button();
            if (btnImportData == null) btnImportData = new Button();
            if (btnSaveImported == null) btnSaveImported = new Button();
            if (btnSearch == null) btnSearch = new Button();
            if (btnFilterDate == null) btnFilterDate = new Button();

            // التأكد من وجود الحقول
            if (txtName == null) txtName = new TextBox();
            if (txtPhone == null) txtPhone = new TextBox();
            if (txtAddress == null) txtAddress = new TextBox();
            if (txtMeterReading == null) txtMeterReading = new TextBox();
            if (txtNewMeterNumber == null) txtNewMeterNumber = new TextBox();
            if (txtNewMeterLocation == null) txtNewMeterLocation = new TextBox();
            if (txtSearch == null) txtSearch = new TextBox();

            if (cboAccount == null) cboAccount = new ComboBox();
            if (chkIsPrimary == null) chkIsPrimary = new CheckBox();
            if (dateFilter == null) dateFilter = new DateTimePicker();

            if (lblName == null) lblName = new Label();
            if (lblPhone == null) lblPhone = new Label();
            if (lblAddress == null) lblAddress = new Label();
            if (lblAccount == null) lblAccount = new Label();
            if (lblMeterReading == null) lblMeterReading = new Label();
            if (lblMeterNo == null) lblMeterNo = new Label();
            if (lblMeterLocation == null) lblMeterLocation = new Label();
        }

        private void InitializeForm()
        {
            try
            {
                // إعدادات النموذج
                this.Text = "إدارة المشتركين والعدادات";
                this.StartPosition = FormStartPosition.CenterScreen;
                this.WindowState = FormWindowState.Maximized;
                this.BackColor = Color.FromArgb(240, 242, 245);

                // إعداد التلميحات
                InitializeToolTips();

                // ربط الأحداث
                HookEvents();

                // تنسيق الجداول
                StyleDataGridViews();

                // تحميل الحسابات
                LoadAccounts();

                // تحديث حالة الأزرار
                UpdateButtonsState();

                // تعيين القيم الافتراضية
                if (txtMeterReading != null)
                    txtMeterReading.Text = "0";
                if (dateFilter != null)
                    dateFilter.Value = DateTime.Now;

                // تحميل البيانات
                LoadSubscribers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تهيئة النموذج: {ex.Message}", "خطأ فادح",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeToolTips()
        {
            try
            {
                toolTip = new ToolTip();
                if (btnAdd != null) toolTip.SetToolTip(btnAdd, "إضافة مشترك جديد");
                if (btnUpdate != null) toolTip.SetToolTip(btnUpdate, "تعديل بيانات المشترك");
                if (btnClear != null) toolTip.SetToolTip(btnClear, "تفريغ جميع الحقول");
                if (btnCreateAccount != null) toolTip.SetToolTip(btnCreateAccount, "إنشاء حساب ذمة جديد");
                if (btnAddMeter != null) toolTip.SetToolTip(btnAddMeter, "إضافة عداد جديد للمشترك");
                if (btnSearch != null) toolTip.SetToolTip(btnSearch, "بحث بالاسم");
                if (btnFilterDate != null) toolTip.SetToolTip(btnFilterDate, "تصفية حسب تاريخ الإضافة");
            }
            catch (Exception ex)
            {
                // تجاهل أخطاء التلميحات
                System.Diagnostics.Debug.WriteLine($"ToolTip error: {ex.Message}");
            }
        }

        private void HookEvents()
        {
            try
            {
                // أزرار المشترك
                if (btnAdd != null) btnAdd.Click += BtnAdd_Click;
                if (btnUpdate != null) btnUpdate.Click += BtnUpdate_Click;
                if (btnClear != null) btnClear.Click += BtnClear_Click;
                if (btnCreateAccount != null) btnCreateAccount.Click += BtnCreateAccount_Click;

                // أزرار العداد
                if (btnAddMeter != null) btnAddMeter.Click += BtnAddMeter_Click;
                if (btnSetPrimary != null) btnSetPrimary.Click += BtnSetPrimary_Click;
                if (btnUnlinkMeter != null) btnUnlinkMeter.Click += BtnUnlinkMeter_Click;
                if (btnImportData != null) btnImportData.Click += BtnImportData_Click;
                if (btnSaveImported != null) btnSaveImported.Click += BtnSaveImported_Click;

                // أزرار البحث
                if (btnSearch != null) btnSearch.Click += BtnSearch_Click;
                if (btnFilterDate != null) btnFilterDate.Click += BtnFilterDate_Click;
                if (txtSearch != null) txtSearch.KeyDown += TxtSearch_KeyDown;
                if (txtPhone != null) txtPhone.KeyPress += TxtPhone_KeyPress;

                // أحداث الجداول
                if (dgvSubscribers != null)
                {
                    dgvSubscribers.CellClick += DgvSubscribers_CellClick;
                    dgvSubscribers.CellContentClick += DgvSubscribers_CellContentClick;
                }
                if (dgvMeters != null)
                    dgvMeters.CellClick += DgvMeters_CellClick;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في ربط الأحداث: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void StyleDataGridViews()
        {
            try
            {
                if (dgvSubscribers != null)
                {
                    dgvSubscribers.BackgroundColor = Color.White;
                    dgvSubscribers.BorderStyle = BorderStyle.None;
                    dgvSubscribers.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                    dgvSubscribers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
                    dgvSubscribers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dgvSubscribers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    dgvSubscribers.RowTemplate.Height = 35;
                    dgvSubscribers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
                    dgvSubscribers.DefaultCellStyle.Font = new Font("Segoe UI", 9);
                    dgvSubscribers.DefaultCellStyle.ForeColor = Color.FromArgb(52, 73, 94);
                    dgvSubscribers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvSubscribers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvSubscribers.RowHeadersVisible = false;
                    dgvSubscribers.AllowUserToAddRows = false;
                    dgvSubscribers.AllowUserToDeleteRows = false;
                    dgvSubscribers.ReadOnly = true;
                }

                if (dgvMeters != null)
                {
                    dgvMeters.BackgroundColor = Color.White;
                    dgvMeters.BorderStyle = BorderStyle.None;
                    dgvMeters.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                    dgvMeters.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 73, 94);
                    dgvMeters.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dgvMeters.RowTemplate.Height = 35;
                    dgvMeters.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
                    dgvMeters.DefaultCellStyle.Font = new Font("Segoe UI", 9);
                    dgvMeters.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                    dgvMeters.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    dgvMeters.RowHeadersVisible = false;
                    dgvMeters.AllowUserToAddRows = false;
                    dgvMeters.AllowUserToDeleteRows = false;
                    dgvMeters.ReadOnly = true;
                }

                AddDeleteButtonColumn();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Style error: {ex.Message}");
            }
        }

        private void AddDeleteButtonColumn()
        {
            try
            {
                if (dgvSubscribers != null && !dgvSubscribers.Columns.Contains("btnDelete"))
                {
                    DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn
                    {
                        Name = "btnDelete",
                        HeaderText = "تعطيل",
                        Text = "تعطيل",
                        UseColumnTextForButtonValue = true,
                        Width = 60,
                        FlatStyle = FlatStyle.Flat
                    };
                    dgvSubscribers.Columns.Add(btnDelete);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AddDeleteButton error: {ex.Message}");
            }
        }

        private void UpdateButtonsState()
        {
            try
            {
                bool hasSubscriber = selectedSubscriberId > 0;
                bool hasMeter = selectedMeterId > 0;

                if (btnUpdate != null) btnUpdate.Enabled = hasSubscriber;
                if (btnCreateAccount != null) btnCreateAccount.Enabled = hasSubscriber;
                if (btnAddMeter != null) btnAddMeter.Enabled = hasSubscriber;
                if (btnSetPrimary != null) btnSetPrimary.Enabled = hasSubscriber && hasMeter;
                if (btnUnlinkMeter != null) btnUnlinkMeter.Enabled = hasSubscriber && hasMeter;
                if (btnSaveImported != null) btnSaveImported.Enabled = _lastImportedTable != null && _lastImportedTable.Rows.Count > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"UpdateButtons error: {ex.Message}");
            }
        }

        private void ClearFields(bool clearSearch = true)
        {
            try
            {
                selectedSubscriberId = -1;
                selectedMeterId = -1;

                if (txtName != null) txtName.Clear();
                if (txtPhone != null) txtPhone.Clear();
                if (txtAddress != null) txtAddress.Clear();
                if (txtMeterReading != null) txtMeterReading.Text = "0";
                if (txtNewMeterNumber != null) txtNewMeterNumber.Clear();
                if (txtNewMeterLocation != null) txtNewMeterLocation.Clear();
                if (chkIsPrimary != null) chkIsPrimary.Checked = true;

                if (clearSearch && txtSearch != null)
                    txtSearch.Clear();

                if (cboAccount != null && cboAccount.Items.Count > 0)
                    cboAccount.SelectedIndex = -1;

                if (dgvMeters != null)
                    dgvMeters.DataSource = null;

                UpdateButtonsState();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ClearFields error: {ex.Message}");
            }
        }

        private void LoadAccounts()
        {
            try
            {
                if (string.IsNullOrEmpty(connStr))
                {
                    MessageBox.Show("سلسلة اتصال قاعدة البيانات غير محددة", "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = @"
                        SELECT AccountID, AccountCode + N' - ' + AccountName AS DisplayName
                        FROM Accounts
                        WHERE AccountType <> 'Revenue'
                        ORDER BY AccountCode";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (cboAccount != null)
                    {
                        cboAccount.DataSource = dt;
                        cboAccount.DisplayMember = "DisplayName";
                        cboAccount.ValueMember = "AccountID";
                        cboAccount.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل الحسابات: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadSubscribers(string searchText = "", DateTime? filterDate = null)
        {
            try
            {
                if (string.IsNullOrEmpty(connStr))
                {
                    MessageBox.Show("سلسلة اتصال قاعدة البيانات غير محددة", "خطأ",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = @"
                        SELECT 
                            S.SubscriberID,
                            S.Name,
                            S.PhoneNumber,
                            S.Address,
                            S.CreatedDate,
                            S.AccountID,
                            A.AccountCode + N' - ' + A.AccountName AS AccountName,
                            (
                                SELECT TOP 1 m.MeterNumber
                                FROM SubscriberMeters sm
                                JOIN Meters m ON sm.MeterID = m.MeterID
                                WHERE sm.SubscriberID = S.SubscriberID AND sm.IsPrimary = 1
                            ) AS PrimaryMeterNumber
                        FROM Subscribers S
                        LEFT JOIN Accounts A ON S.AccountID = A.AccountID
                        WHERE S.IsActive = 1";

                    if (!string.IsNullOrWhiteSpace(searchText))
                        query += " AND S.Name LIKE @SearchText";

                    if (filterDate.HasValue)
                        query += " AND CAST(S.CreatedDate AS DATE) = @FilterDate";

                    query += " ORDER BY S.SubscriberID DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);

                    if (!string.IsNullOrWhiteSpace(searchText))
                        da.SelectCommand.Parameters.AddWithValue("@SearchText", "%" + searchText.Trim() + "%");

                    if (filterDate.HasValue)
                        da.SelectCommand.Parameters.AddWithValue("@FilterDate", filterDate.Value.Date);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dgvSubscribers != null)
                    {
                        dgvSubscribers.DataSource = dt;
                        FormatSubscribersGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل المشتركين: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatSubscribersGrid()
        {
            try
            {
                if (dgvSubscribers == null) return;

                if (dgvSubscribers.Columns.Contains("SubscriberID"))
                {
                    dgvSubscribers.Columns["SubscriberID"].HeaderText = "رقم";
                    //dgvSubscribers.Columns["SubscriberID"].Width = 80;
                }
                if (dgvSubscribers.Columns.Contains("Name"))
                {
                    dgvSubscribers.Columns["Name"].HeaderText = "الاسم";
                   // dgvSubscribers.Columns["Name"].Width = 200;
                }
                if (dgvSubscribers.Columns.Contains("PrimaryMeterNumber"))
                {
                    dgvSubscribers.Columns["PrimaryMeterNumber"].HeaderText = "العداد الأساسي";
                    //dgvSubscribers.Columns["PrimaryMeterNumber"].Width = 120;
                }
                if (dgvSubscribers.Columns.Contains("PhoneNumber"))
                {
                    dgvSubscribers.Columns["PhoneNumber"].HeaderText = "الهاتف";
                   // dgvSubscribers.Columns["PhoneNumber"].Width = 120;
                }
                if (dgvSubscribers.Columns.Contains("Address"))
                {
                    dgvSubscribers.Columns["Address"].HeaderText = "العنوان";
                   // dgvSubscribers.Columns["Address"].Width = 180;
                }
                if (dgvSubscribers.Columns.Contains("CreatedDate"))
                {
                    dgvSubscribers.Columns["CreatedDate"].HeaderText = "تاريخ الإضافة";
                   // dgvSubscribers.Columns["CreatedDate"].Width = 100;
                    dgvSubscribers.Columns["CreatedDate"].DefaultCellStyle.Format = "yyyy/MM/dd";
                }
                if (dgvSubscribers.Columns.Contains("AccountName"))
                {
                    dgvSubscribers.Columns["AccountName"].HeaderText = "حساب الذمة";
                   // dgvSubscribers.Columns["AccountName"].Width = 150;
                }
                if (dgvSubscribers.Columns.Contains("AccountID"))
                {
                    dgvSubscribers.Columns["AccountID"].Visible = false;
                }

                AddDeleteButtonColumn();
                if (dgvSubscribers.Columns.Contains("btnDelete"))
                {
                    dgvSubscribers.Columns["btnDelete"].DisplayIndex = dgvSubscribers.Columns.Count - 1;
                    //dgvSubscribers.Columns["btnDelete"].Width = 70;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FormatSubscribersGrid error: {ex.Message}");
            }
        }

        private void LoadMetersForSubscriber(int subscriberId)
        {
            try
            {
                if (string.IsNullOrEmpty(connStr)) return;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    string query = @"
                        SELECT 
                            m.MeterID,
                            m.MeterNumber,
                            m.Location,
                            sm.IsPrimary,
                            sm.LinkedAt
                        FROM SubscriberMeters sm
                        JOIN Meters m ON sm.MeterID = m.MeterID
                        WHERE sm.SubscriberID = @SubscriberID
                        ORDER BY sm.IsPrimary DESC, m.MeterNumber";

                    SqlDataAdapter da = new SqlDataAdapter(query, con);
                    da.SelectCommand.Parameters.AddWithValue("@SubscriberID", subscriberId);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dgvMeters != null)
                    {
                        dgvMeters.DataSource = dt;
                        FormatMetersGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل العدادات: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatMetersGrid()
        {
            try
            {
                if (dgvMeters == null) return;

                if (dgvMeters.Columns.Contains("MeterID"))
                    dgvMeters.Columns["MeterID"].Visible = false;
                if (dgvMeters.Columns.Contains("MeterNumber"))
                {
                    dgvMeters.Columns["MeterNumber"].HeaderText = "رقم العداد";
                    dgvMeters.Columns["MeterNumber"].Width = 150;
                }
                if (dgvMeters.Columns.Contains("Location"))
                {
                    dgvMeters.Columns["Location"].HeaderText = "الموقع";
                    dgvMeters.Columns["Location"].Width = 200;
                }
                if (dgvMeters.Columns.Contains("IsPrimary"))
                {
                    dgvMeters.Columns["IsPrimary"].HeaderText = "أساسي";
                    dgvMeters.Columns["IsPrimary"].Width = 80;
                }
                if (dgvMeters.Columns.Contains("LinkedAt"))
                {
                    dgvMeters.Columns["LinkedAt"].HeaderText = "تاريخ الربط";
                    dgvMeters.Columns["LinkedAt"].Width = 150;
                    dgvMeters.Columns["LinkedAt"].DefaultCellStyle.Format = "yyyy/MM/dd HH:mm";
                }

                // تلوين الصف الأساسي
                foreach (DataGridViewRow row in dgvMeters.Rows)
                {
                    if (row.Cells["IsPrimary"].Value != null && Convert.ToBoolean(row.Cells["IsPrimary"].Value))
                    {
                        row.DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
                        row.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"FormatMetersGrid error: {ex.Message}");
            }
        }

        private (int? SubscriberId, int? MeterId) ExecuteStoredProcedure(string action, Action<SqlCommand> setParameters)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("dbo.usp_Subscriber_Manage", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Action", action);

                    SqlParameter pSubscriberId = new SqlParameter("@SubscriberID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = selectedSubscriberId > 0 ? (object)selectedSubscriberId : DBNull.Value
                    };

                    SqlParameter pMeterId = new SqlParameter("@MeterID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = selectedMeterId > 0 ? (object)selectedMeterId : DBNull.Value
                    };

                    cmd.Parameters.Add(pSubscriberId);
                    cmd.Parameters.Add(pMeterId);

                    setParameters?.Invoke(cmd);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    int? newSubscriberId = pSubscriberId.Value == DBNull.Value ? (int?)null : Convert.ToInt32(pSubscriberId.Value);
                    int? newMeterId = pMeterId.Value == DBNull.Value ? (int?)null : Convert.ToInt32(pMeterId.Value);

                    if (newSubscriberId.HasValue) selectedSubscriberId = newSubscriberId.Value;
                    if (newMeterId.HasValue) selectedMeterId = newMeterId.Value;

                    return (newSubscriberId, newMeterId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في تنفيذ الإجراء {action}: {ex.Message}", ex);
            }
        }

        // أزرار المشترك
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName?.Text))
                {
                    MessageBox.Show("اسم المشترك مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName?.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNewMeterNumber?.Text))
                {
                    MessageBox.Show("رقم العداد مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNewMeterNumber?.Focus();
                    return;
                }

                if (cboAccount?.SelectedValue == null)
                {
                    MessageBox.Show("اختر حساب الذمة للمشترك", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboAccount?.Focus();
                    return;
                }

                if (!decimal.TryParse(txtMeterReading?.Text, out decimal initialReading))
                {
                    MessageBox.Show("القراءة الابتدائية غير صحيحة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMeterReading?.Focus();
                    return;
                }

                ExecuteStoredProcedure("ADD_SUBSCRIBER", cmd =>
                {
                    cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(txtPhone?.Text) ? DBNull.Value : (object)txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(txtAddress?.Text) ? DBNull.Value : (object)txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@AccountID", Convert.ToInt32(cboAccount.SelectedValue));
                    cmd.Parameters.AddWithValue("@MeterNumber", txtNewMeterNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@MeterLocation", string.IsNullOrWhiteSpace(txtNewMeterLocation?.Text) ? DBNull.Value : (object)txtNewMeterLocation.Text.Trim());
                    cmd.Parameters.AddWithValue("@IsPrimary", true);
                    cmd.Parameters.AddWithValue("@InitialReading", initialReading);
                });

                MessageBox.Show("تمت إضافة المشترك والعداد بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadSubscribers();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء الإضافة: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedSubscriberId <= 0)
                {
                    MessageBox.Show("اختر مشتركًا للتعديل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtName?.Text))
                {
                    MessageBox.Show("اسم المشترك مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtName?.Focus();
                    return;
                }

                if (cboAccount?.SelectedValue == null)
                {
                    MessageBox.Show("اختر حساب الذمة للمشترك", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboAccount?.Focus();
                    return;
                }

                ExecuteStoredProcedure("UPDATE_SUBSCRIBER", cmd =>
                {
                    cmd.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(txtPhone?.Text) ? DBNull.Value : (object)txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(txtAddress?.Text) ? DBNull.Value : (object)txtAddress.Text.Trim());
                    cmd.Parameters.AddWithValue("@AccountID", Convert.ToInt32(cboAccount.SelectedValue));
                });

                MessageBox.Show("تم تعديل بيانات المشترك بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadSubscribers();
                if (selectedSubscriberId > 0)
                    LoadMetersForSubscriber(selectedSubscriberId);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ أثناء التعديل: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void BtnCreateAccount_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedSubscriberId <= 0)
                {
                    MessageBox.Show("اختر مشتركًا أولًا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SqlConnection con = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand("dbo.CreateSubscriberAccount", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@SubscriberID", selectedSubscriberId);

                    SqlParameter outAcc = new SqlParameter("@AccountID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outAcc);

                    con.Open();
                    cmd.ExecuteNonQuery();

                    int newAccountId = Convert.ToInt32(outAcc.Value);

                    MessageBox.Show("تم إنشاء وربط حساب الذمة بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadAccounts();
                    if (cboAccount != null)
                        cboAccount.SelectedValue = newAccountId;
                    LoadSubscribers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // أزرار العداد
        private void BtnAddMeter_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedSubscriberId <= 0)
                {
                    MessageBox.Show("اختر مشتركًا أولًا", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtNewMeterNumber?.Text))
                {
                    MessageBox.Show("رقم العداد مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtNewMeterNumber?.Focus();
                    return;
                }

                if (!decimal.TryParse(txtMeterReading?.Text, out decimal initialReading))
                {
                    MessageBox.Show("القراءة الابتدائية غير صحيحة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtMeterReading?.Focus();
                    return;
                }

                ExecuteStoredProcedure("ADD_METER", cmd =>
                {
                    cmd.Parameters.AddWithValue("@MeterNumber", txtNewMeterNumber.Text.Trim());
                    cmd.Parameters.AddWithValue("@MeterLocation", string.IsNullOrWhiteSpace(txtNewMeterLocation?.Text) ? DBNull.Value : (object)txtNewMeterLocation.Text.Trim());
                    cmd.Parameters.AddWithValue("@IsPrimary", chkIsPrimary?.Checked ?? true);
                    cmd.Parameters.AddWithValue("@InitialReading", initialReading);
                });

                MessageBox.Show("تمت إضافة العداد بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMetersForSubscriber(selectedSubscriberId);
                LoadSubscribers();

                if (txtNewMeterNumber != null) txtNewMeterNumber.Clear();
                if (txtNewMeterLocation != null) txtNewMeterLocation.Clear();
                if (chkIsPrimary != null) chkIsPrimary.Checked = false;
                if (txtMeterReading != null) txtMeterReading.Text = "0";
                UpdateButtonsState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSetPrimary_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedSubscriberId <= 0 || selectedMeterId <= 0)
                {
                    MessageBox.Show("اختر مشتركًا ثم اختر عدادًا من جدول العدادات", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                ExecuteStoredProcedure("SET_PRIMARY_METER", null);
                MessageBox.Show("تم تعيين العداد الأساسي بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadMetersForSubscriber(selectedSubscriberId);
                LoadSubscribers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnUnlinkMeter_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedSubscriberId <= 0 || selectedMeterId <= 0)
                {
                    MessageBox.Show("اختر مشتركًا ثم اختر عدادًا لفك الربط", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("هل تريد فك ربط هذا العداد من المشترك؟", "تأكيد",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ExecuteStoredProcedure("UNLINK_METER", null);
                    MessageBox.Show("تم فك الربط بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMetersForSubscriber(selectedSubscriberId);
                    LoadSubscribers();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnImportData_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
                    openFileDialog.Title = "اختر ملف Excel";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        ImportExcelData(openFileDialog.FileName);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSaveImported_Click(object sender, EventArgs e)
        {
            try
            {
                if (_lastImportedTable == null || _lastImportedTable.Rows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات مستوردة للحفظ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveImportedData();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ImportExcelData(string filePath)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(new FileInfo(filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    if (worksheet == null || worksheet.Dimension == null)
                    {
                        MessageBox.Show("الملف لا يحتوي على بيانات", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    var dt = new DataTable();
                    dt.Columns.Add("RowNumber", typeof(int));
                    dt.Columns.Add("SubscriberName", typeof(string));
                    dt.Columns.Add("PhoneNumber", typeof(string));
                    dt.Columns.Add("Address", typeof(string));
                    dt.Columns.Add("MeterNumber", typeof(string));
                    dt.Columns.Add("MeterLocation", typeof(string));
                    dt.Columns.Add("InitialReading", typeof(decimal));
                    dt.Columns.Add("IsPrimary", typeof(bool));
                    dt.Columns.Add("AccountCode", typeof(string));
                    dt.Columns.Add("Status", typeof(string));

                    int startRow = worksheet.Dimension.Start.Row + 1;
                    int endRow = worksheet.Dimension.End.Row;

                    for (int row = startRow; row <= endRow; row++)
                    {
                        string subscriberName = worksheet.Cells[row, 1]?.Text?.Trim();
                        if (string.IsNullOrWhiteSpace(subscriberName))
                            continue;

                        DataRow dr = dt.NewRow();
                        dr["RowNumber"] = row;
                        dr["SubscriberName"] = subscriberName;
                        dr["PhoneNumber"] = worksheet.Cells[row, 2]?.Text?.Trim() ?? "";
                        dr["Address"] = worksheet.Cells[row, 3]?.Text?.Trim() ?? "";
                        dr["MeterNumber"] = worksheet.Cells[row, 4]?.Text?.Trim() ?? "";
                        dr["MeterLocation"] = worksheet.Cells[row, 5]?.Text?.Trim() ?? "";

                        if (decimal.TryParse(worksheet.Cells[row, 6]?.Text?.Trim(), out decimal reading))
                            dr["InitialReading"] = reading;
                        else
                            dr["InitialReading"] = 0;

                        dr["IsPrimary"] = true;
                        dr["AccountCode"] = worksheet.Cells[row, 7]?.Text?.Trim() ?? "";

                        // التحقق من صحة البيانات
                        List<string> errors = new List<string>();
                        if (string.IsNullOrWhiteSpace(dr["SubscriberName"].ToString()))
                            errors.Add("اسم المشترك مطلوب");
                        if (string.IsNullOrWhiteSpace(dr["MeterNumber"].ToString()))
                            errors.Add("رقم العداد مطلوب");
                        if (Convert.ToDecimal(dr["InitialReading"]) < 0)
                            errors.Add("القراءة الابتدائية غير صحيحة");

                        dr["Status"] = errors.Count > 0 ? string.Join(" | ", errors) : "جاهز للحفظ";
                        dt.Rows.Add(dr);
                    }

                    _lastImportedTable = dt;
                    if (dgvSubscribers != null)
                    {
                        dgvSubscribers.DataSource = dt;

                        // تنسيق جدول المعاينة
                        foreach (DataGridViewColumn col in dgvSubscribers.Columns)
                        {
                            if (col.Name != "btnDelete")
                                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        }

                        if (dgvSubscribers.Columns.Contains("RowNumber"))
                            dgvSubscribers.Columns["RowNumber"].HeaderText = "رقم الصف";
                        if (dgvSubscribers.Columns.Contains("SubscriberName"))
                            dgvSubscribers.Columns["SubscriberName"].HeaderText = "اسم المشترك";
                        if (dgvSubscribers.Columns.Contains("MeterNumber"))
                            dgvSubscribers.Columns["MeterNumber"].HeaderText = "رقم العداد";
                        if (dgvSubscribers.Columns.Contains("InitialReading"))
                            dgvSubscribers.Columns["InitialReading"].HeaderText = "القراءة الابتدائية";
                        if (dgvSubscribers.Columns.Contains("Status"))
                            dgvSubscribers.Columns["Status"].HeaderText = "الحالة";

                        if (dgvSubscribers.Columns.Contains("btnDelete"))
                            dgvSubscribers.Columns["btnDelete"].Visible = false;
                    }

                    UpdateButtonsState();
                    MessageBox.Show($"تم استيراد {dt.Rows.Count} صف بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في قراءة ملف Excel: {ex.Message}", ex);
            }
        }

        private void SaveImportedData()
        {
            try
            {
                var validRows = _lastImportedTable.AsEnumerable()
                    .Where(row => row["Status"].ToString() == "جاهز للحفظ")
                    .ToList();

                if (validRows.Count == 0)
                {
                    MessageBox.Show("لا توجد بيانات صالحة للحفظ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int successCount = 0;
                int failCount = 0;

                foreach (var row in validRows)
                {
                    try
                    {
                        ExecuteStoredProcedure("ADD_SUBSCRIBER", cmd =>
                        {
                            cmd.Parameters.AddWithValue("@Name", row["SubscriberName"].ToString());
                            cmd.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(row["PhoneNumber"]?.ToString()) ? DBNull.Value : (object)row["PhoneNumber"].ToString());
                            cmd.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(row["Address"]?.ToString()) ? DBNull.Value : (object)row["Address"].ToString());
                            cmd.Parameters.AddWithValue("@AccountID", DBNull.Value);
                            cmd.Parameters.AddWithValue("@MeterNumber", row["MeterNumber"].ToString());
                            cmd.Parameters.AddWithValue("@MeterLocation", string.IsNullOrWhiteSpace(row["MeterLocation"]?.ToString()) ? DBNull.Value : (object)row["MeterLocation"].ToString());
                            cmd.Parameters.AddWithValue("@IsPrimary", Convert.ToBoolean(row["IsPrimary"]));
                            cmd.Parameters.AddWithValue("@InitialReading", Convert.ToDecimal(row["InitialReading"]));
                        });
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        row["Status"] = $"خطأ: {ex.Message}";
                    }
                }

                MessageBox.Show($"تم حفظ {successCount} مشترك بنجاح\nفشل {failCount} مشترك",
                    "نتيجة الاستيراد", MessageBoxButtons.OK,
                    failCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

                LoadSubscribers();
                ClearFields();
                _lastImportedTable = null;
                UpdateButtonsState();
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في حفظ البيانات: {ex.Message}", ex);
            }
        }

        // أحداث الجداول
        private void DgvSubscribers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                if (dgvSubscribers == null) return;
                if (dgvSubscribers.Rows[e.RowIndex] == null) return;

                // التحقق من وجود الأعمدة المطلوبة
                if (!dgvSubscribers.Columns.Contains("SubscriberID")) return;
                if (dgvSubscribers.Rows[e.RowIndex].Cells["SubscriberID"].Value == null) return;
                if (dgvSubscribers.Rows[e.RowIndex].Cells["SubscriberID"].Value == DBNull.Value) return;

                selectedSubscriberId = Convert.ToInt32(dgvSubscribers.Rows[e.RowIndex].Cells["SubscriberID"].Value);

                // تعبئة الحقول مع التحقق من وجودها
                if (txtName != null && dgvSubscribers.Columns.Contains("Name"))
                    txtName.Text = dgvSubscribers.Rows[e.RowIndex].Cells["Name"].Value?.ToString() ?? "";

                if (txtPhone != null && dgvSubscribers.Columns.Contains("PhoneNumber"))
                    txtPhone.Text = dgvSubscribers.Rows[e.RowIndex].Cells["PhoneNumber"].Value?.ToString() ?? "";

                if (txtAddress != null && dgvSubscribers.Columns.Contains("Address"))
                    txtAddress.Text = dgvSubscribers.Rows[e.RowIndex].Cells["Address"].Value?.ToString() ?? "";

                // تعبئة حساب الذمة إذا كان موجوداً
                if (cboAccount != null && dgvSubscribers.Columns.Contains("AccountID") &&
                    dgvSubscribers.Rows[e.RowIndex].Cells["AccountID"].Value != null &&
                    dgvSubscribers.Rows[e.RowIndex].Cells["AccountID"].Value != DBNull.Value)
                {
                    int accountId = Convert.ToInt32(dgvSubscribers.Rows[e.RowIndex].Cells["AccountID"].Value);
                    if (cboAccount.Items.Count > 0)
                    {
                        try
                        {
                            cboAccount.SelectedValue = accountId;
                        }
                        catch
                        {
                            // تجاهل خطأ تحديد القيمة
                        }
                    }
                }

                // تحميل العدادات
                LoadMetersForSubscriber(selectedSubscriberId);
                UpdateButtonsState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحديد المشترك: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvSubscribers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                if (dgvSubscribers == null) return;

                // التحقق من وجود عمود btnDelete
                if (!dgvSubscribers.Columns.Contains("btnDelete")) return;
                if (e.ColumnIndex < 0 || e.ColumnIndex >= dgvSubscribers.Columns.Count) return;
                if (dgvSubscribers.Columns[e.ColumnIndex].Name != "btnDelete") return;

                DataGridViewRow row = dgvSubscribers.Rows[e.RowIndex];
                if (row.Cells["SubscriberID"].Value == null) return;

                int id = Convert.ToInt32(row.Cells["SubscriberID"].Value);
                string name = row.Cells["Name"].Value?.ToString() ?? "";

                if (MessageBox.Show($"هل تريد تعطيل المشترك '{name}'؟", "تأكيد",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    selectedSubscriberId = id;
                    ExecuteStoredProcedure("DISABLE_SUBSCRIBER", null);
                    MessageBox.Show("تم تعطيل المشترك بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadSubscribers();
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DgvMeters_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0) return;
                if (dgvMeters == null) return;
                if (!dgvMeters.Columns.Contains("MeterID")) return;

                DataGridViewRow row = dgvMeters.Rows[e.RowIndex];
                if (row.Cells["MeterID"].Value == null) return;

                selectedMeterId = Convert.ToInt32(row.Cells["MeterID"].Value);

                // تعبئة حقول العداد للتعديل
                if (txtNewMeterNumber != null && dgvMeters.Columns.Contains("MeterNumber"))
                    txtNewMeterNumber.Text = row.Cells["MeterNumber"].Value?.ToString() ?? "";

                if (txtNewMeterLocation != null && dgvMeters.Columns.Contains("Location"))
                    txtNewMeterLocation.Text = row.Cells["Location"].Value?.ToString() ?? "";

                if (chkIsPrimary != null && dgvMeters.Columns.Contains("IsPrimary"))
                    chkIsPrimary.Checked = row.Cells["IsPrimary"].Value != null && Convert.ToBoolean(row.Cells["IsPrimary"].Value);

                UpdateButtonsState();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // البحث والتصفية
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                LoadSubscribers(txtSearch?.Text?.Trim() ?? "", null);
                ClearFields(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في البحث: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnFilterDate_Click(object sender, EventArgs e)
        {
            try
            {
                LoadSubscribers("", dateFilter?.Value);
                ClearFields(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في التصفية: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnSearch_Click(sender, e);
                e.SuppressKeyPress = true;
            }
        }

        private void TxtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            // السماح فقط بالأرقام
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}

*/



