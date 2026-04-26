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
    public partial class CollectorUserLinkForm : Form
    {
     
            private readonly CollectorUserLinkService _service = new CollectorUserLinkService();
            private List<CollectorUserLinkItem> _collectors = new List<CollectorUserLinkItem>();
            private List<UserLookupItem> _users = new List<UserLookupItem>();

            private Label lblTitle;
            private TextBox txtCollectorSearch;
            private TextBox txtUserSearch;
            private Button btnSearchCollectors;
            private Button btnSearchUsers;
            private Button btnRefresh;
            private DataGridView dgvCollectors;
            private DataGridView dgvUsers;
            private Button btnAssign;
            private Button btnUnassign;
            private Label lblSelectedCollector;
            private Label lblStatus;

            public CollectorUserLinkForm()
            {
                InitializeComponent();
                PermissionHelper.EnforceFormPermission(this, "COLLECTORS_LINK_USER");
                LoadData();
            }

            private void InitializeComponent()
            {
                Text = "ربط المحصل بالمستخدم";
                StartPosition = FormStartPosition.CenterScreen;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;
                WindowState = FormWindowState.Maximized;

                lblTitle = new Label
                {
                    Text = "ربط المحصل بالمستخدم",
                    Font = new Font("Tahoma", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };

                txtCollectorSearch = new TextBox { Location = new Point(20, 60), Size = new Size(240, 27) };
                btnSearchCollectors = MakeButton("بحث المحصلين", 270, 58, 120);
                btnSearchCollectors.Click += (s, e) => LoadCollectors();

                txtUserSearch = new TextBox { Location = new Point(780, 60), Size = new Size(240, 27) };
                btnSearchUsers = MakeButton("بحث المستخدمين", 1030, 58, 120);
                btnSearchUsers.Click += (s, e) => LoadUsers();

                btnRefresh = MakeButton("تحديث الكل", 1160, 58, 110);
                btnRefresh.Click += (s, e) => LoadData();

                dgvCollectors = new DataGridView
                {
                    Location = new Point(20, 100),
                    Size = new Size(600, 520),
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
                dgvCollectors.SelectionChanged += (s, e) => UpdateSelectedCollectorInfo();

                dgvCollectors.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCollectorID", HeaderText = "الرقم", DataPropertyName = "CollectorID", FillWeight = 60 });
                dgvCollectors.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCollectorName", HeaderText = "المحصل", DataPropertyName = "CollectorName", FillWeight = 150 });
                dgvCollectors.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCollectorPhone", HeaderText = "الهاتف", DataPropertyName = "CollectorPhone", FillWeight = 110 });
                dgvCollectors.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUserName", HeaderText = "اسم المستخدم", DataPropertyName = "UserName", FillWeight = 110 });
                dgvCollectors.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRoleName", HeaderText = "الدور", DataPropertyName = "RoleName", FillWeight = 90 });

                dgvUsers = new DataGridView
                {
                    Location = new Point(690, 100),
                    Size = new Size(580, 520),
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

                dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUserID", HeaderText = "الرقم", DataPropertyName = "UserID", FillWeight = 60 });
                dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUserName2", HeaderText = "اسم المستخدم", DataPropertyName = "UserName", FillWeight = 120 });
                dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFullName", HeaderText = "الاسم الكامل", DataPropertyName = "FullName", FillWeight = 160 });
                dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRoleName2", HeaderText = "الدور", DataPropertyName = "RoleName", FillWeight = 100 });

                lblSelectedCollector = new Label
                {
                    Location = new Point(20, 630),
                    Size = new Size(1250, 30),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Font = new Font("Tahoma", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204)
                };

                btnAssign = MakeButton("ربط المستخدم المحدد بالمحصل المحدد", 350, 670, 280);
                btnAssign.Click += BtnAssign_Click;

                btnUnassign = MakeButton("فك الربط عن المحصل المحدد", 640, 670, 220);
                btnUnassign.Click += BtnUnassign_Click;

                lblStatus = new Label
                {
                    Location = new Point(20, 715),
                    Size = new Size(1250, 30),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.DarkGreen
                };

                Controls.AddRange(new Control[]
                {
                lblTitle,
                txtCollectorSearch,
                btnSearchCollectors,
                txtUserSearch,
                btnSearchUsers,
                btnRefresh,
                dgvCollectors,
                dgvUsers,
                lblSelectedCollector,
                btnAssign,
                btnUnassign,
                lblStatus
                });
            }

            private Button MakeButton(string text, int left, int top, int width)
            {
                return new Button
                {
                    Text = text,
                    Location = new Point(left, top),
                    Size = new Size(width, 34),
                    BackColor = Color.FromArgb(0, 122, 204),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
            }

            private void LoadData()
            {
                LoadCollectors();
                LoadUsers();
                UpdateSelectedCollectorInfo();
            }

            private void LoadCollectors()
            {
                try
                {
                    _collectors = _service.GetCollectorsWithUsers(txtCollectorSearch.Text);
                    dgvCollectors.DataSource = null;
                    dgvCollectors.DataSource = _collectors;
                    UpdateSelectedCollectorInfo();
                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = $"عدد المحصلين: {_collectors.Count}";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void LoadUsers()
            {
                try
                {
                    _users = _service.GetAvailableUsers(txtUserSearch.Text, onlyActive: true);
                    dgvUsers.DataSource = null;
                    dgvUsers.DataSource = _users;
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private CollectorUserLinkItem GetSelectedCollector()
            {
                return dgvCollectors.CurrentRow?.DataBoundItem as CollectorUserLinkItem;
            }

            private UserLookupItem GetSelectedUser()
            {
                return dgvUsers.CurrentRow?.DataBoundItem as UserLookupItem;
            }

            private void UpdateSelectedCollectorInfo()
            {
                var collector = GetSelectedCollector();
                if (collector == null)
                {
                    lblSelectedCollector.Text = "اختر محصلًا من القائمة اليسرى.";
                    return;
                }

                if (collector.UserID.HasValue)
                {
                    lblSelectedCollector.Text = $"المحصل المحدد: {collector.CollectorName} — المستخدم الحالي: {collector.UserName} ({collector.RoleName})";
                }
                else
                {
                    lblSelectedCollector.Text = $"المحصل المحدد: {collector.CollectorName} — لا يوجد مستخدم مربوط حاليًا";
                }
            }

            private void BtnAssign_Click(object sender, EventArgs e)
            {
                try
                {
                    var collector = GetSelectedCollector();
                    var user = GetSelectedUser();

                    if (collector == null)
                    {
                        MessageBox.Show("اختر محصلًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (user == null)
                    {
                        MessageBox.Show("اختر مستخدمًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (MessageBox.Show(
                        $"هل تريد ربط المستخدم {user.UserName} بالمحصل {collector.CollectorName}؟",
                        "تأكيد الربط",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes)
                        return;

                    _service.AssignUserToCollector(collector.CollectorID, user.UserID);
                    LoadData();
                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = "تم الربط بنجاح.";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void BtnUnassign_Click(object sender, EventArgs e)
            {
                try
                {
                    var collector = GetSelectedCollector();
                    if (collector == null)
                    {
                        MessageBox.Show("اختر محصلًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!collector.UserID.HasValue)
                    {
                        MessageBox.Show("هذا المحصل غير مربوط بأي مستخدم حاليًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    if (MessageBox.Show(
                        $"هل تريد فك ربط المستخدم عن المحصل {collector.CollectorName}؟",
                        "تأكيد فك الربط",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question) != DialogResult.Yes)
                        return;

                    _service.UnassignUserFromCollector(collector.CollectorID);
                    LoadData();
                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = "تم فك الربط بنجاح.";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }
        }
    }