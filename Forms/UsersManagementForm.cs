using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using water3.Utils;
using water3.Models;
using water3.Services;

namespace water3.Forms
{
    public partial class UsersManagementForm : Form
    {

        //public class UsersManagementForm : Form
        //{
        private readonly UserAuthService _service = new UserAuthService();
            private List<AppUser> _users = new List<AppUser>();

            private Label lblTitle;
            private TextBox txtSearch;
            private Button btnSearch;
            private Button btnNew;
            private Button btnEdit;
            private Button btnToggleActive;
            private Button btnResetPassword;
            private DataGridView dgvUsers;
            private Label lblStatus;
        private Button btnRoles;
        public UsersManagementForm()
            {
                InitializeComponent();
            PermissionHelper.EnforceFormPermission(this, "USERS_VIEW");

            PermissionHelper.ApplyControlPermission(btnNew, "USERS_MANAGE");
            PermissionHelper.ApplyControlPermission(btnEdit, "USERS_MANAGE");
            PermissionHelper.ApplyControlPermission(btnToggleActive, "USERS_MANAGE");
            PermissionHelper.ApplyControlPermission(btnResetPassword, "USERS_RESET_PASSWORD");

            LoadUsers();
            }

            private void InitializeComponent()
            {
            Text = "إدارة المستخدمين";
                StartPosition = FormStartPosition.CenterScreen;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;
                WindowState = FormWindowState.Maximized;
            btnRoles = MakeButton("إدارة الأدوار", 970, 58, 140);
            btnRoles.Click += BtnRoles_Click;

            lblTitle = new Label
                {
                    Text = "إدارة المستخدمين",
                    Font = new Font("Tahoma", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };

                txtSearch = new TextBox
                {
                    Location = new Point(20, 60),
                    Size = new Size(250, 27)
                };
                txtSearch.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                        LoadUsers();
                };

                btnSearch = MakeButton("بحث", 280, 58, 100);
                btnSearch.Click += (s, e) => LoadUsers();

                btnNew = MakeButton("مستخدم جديد", 390, 58, 120);
                btnNew.Click += BtnNew_Click;

                btnEdit = MakeButton("تعديل", 520, 58, 100);
                btnEdit.Click += BtnEdit_Click;

                btnToggleActive = MakeButton("تفعيل/تعطيل", 630, 58, 130);
                btnToggleActive.Click += BtnToggleActive_Click;

                btnResetPassword = MakeButton("إعادة تعيين كلمة المرور", 770, 58, 190);
                btnResetPassword.Click += BtnResetPassword_Click;

                dgvUsers = new DataGridView
                {
                    Location = new Point(20, 100),
                    Size = new Size(1100, 520),
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    AllowUserToDeleteRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    AutoGenerateColumns = false,
                    BackgroundColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
                };
                dgvUsers.DoubleClick += (s, e) => EditSelectedUser();

                dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUserID", HeaderText = "الرقم", DataPropertyName = "UserID", FillWeight = 60 });
                dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUserName", HeaderText = "اسم المستخدم", DataPropertyName = "UserName", FillWeight = 120 });
                dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFullName", HeaderText = "الاسم الكامل", DataPropertyName = "FullName", FillWeight = 150 });
                dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRoleName", HeaderText = "الصلاحية", DataPropertyName = "RoleName", FillWeight = 120 });
                dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPhone", HeaderText = "الهاتف", DataPropertyName = "Phone", FillWeight = 120 });
                dgvUsers.Columns.Add(new DataGridViewCheckBoxColumn { Name = "colIsActive", HeaderText = "نشط", DataPropertyName = "IsActive", FillWeight = 70 });

                lblStatus = new Label
                {
                    Location = new Point(20, 630),
                    Size = new Size(1100, 30),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.DarkGreen
                };

                Controls.AddRange(new Control[]
                {
                lblTitle,
                txtSearch,
                btnSearch,
                btnNew,
                btnEdit,
                btnToggleActive,
                btnResetPassword,
                dgvUsers,
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

            private void LoadUsers()
            {
                try
                {
                    _users = _service.GetUsers(txtSearch.Text);
                    dgvUsers.DataSource = null;
                    dgvUsers.DataSource = _users;
                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = $"عدد المستخدمين: {_users.Count}";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private AppUser GetSelectedUser()
            {
                if (dgvUsers.CurrentRow == null)
                    return null;

                return dgvUsers.CurrentRow.DataBoundItem as AppUser;
            }

            private void BtnNew_Click(object sender, EventArgs e)
            {
                using (var frm = new CreateUserForm())
                {
                    frm.ShowDialog();
                }
                LoadUsers();
            }
        private void BtnRoles_Click(object sender, EventArgs e)
        {
            using (var frm = new RolesManagementForm())
            {
                frm.ShowDialog();
            }
        }
        private void btnCollectorUserLink_Click(object sender, EventArgs e)
        {
            using (var frm = new CollectorUserLinkForm())
            {
                frm.ShowDialog();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
            {
                EditSelectedUser();
            }

            private void EditSelectedUser()
            {
                var user = GetSelectedUser();
                if (user == null)
                {
                    MessageBox.Show("اختر مستخدمًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var frm = new EditUserForm(user.UserID))
                {
                    frm.ShowDialog();
                }
                LoadUsers();
            }

            private void BtnToggleActive_Click(object sender, EventArgs e)
            {
                try
                {
                    var user = GetSelectedUser();
                    if (user == null)
                    {
                        MessageBox.Show("اختر مستخدمًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    bool newState = !user.IsActive;
                    string actionText = newState ? "تفعيل" : "تعطيل";

                    if (MessageBox.Show($"هل تريد {actionText} المستخدم؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;

                    _service.SetUserActive(user.UserID, newState);
                    LoadUsers();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void BtnResetPassword_Click(object sender, EventArgs e)
            {
                try
                {
                    var user = GetSelectedUser();
                    if (user == null)
                    {
                        MessageBox.Show("اختر مستخدمًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    using (var frm = new ResetPasswordForm(user.UserID, user.UserName))
                    {
                        frm.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }