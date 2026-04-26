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
    public partial class RolesManagementForm : Form
    {

        //public class RolesManagementForm : Form
        //{
            private readonly RolePermissionService _service = new RolePermissionService();
            private List<AppRole> _roles = new List<AppRole>();

            private Label lblTitle;
            private DataGridView dgvRoles;
            private Button btnNew;
            private Button btnEdit;
            private Button btnDelete;
            private Button btnPermissions;
            private Label lblStatus;

            public RolesManagementForm()
            {
                InitializeComponent();
                PermissionHelper.EnforceFormPermission(this, "USERS_MANAGE");
                LoadRoles();
            }

            private void InitializeComponent()
            {
                Text = "إدارة الأدوار";
                StartPosition = FormStartPosition.CenterScreen;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;
                ClientSize = new Size(800, 500);

                lblTitle = new Label
                {
                    Text = "إدارة الأدوار",
                    Font = new Font("Tahoma", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204),
                    AutoSize = true,
                    Location = new Point(20, 20)
                };

                btnNew = MakeButton("دور جديد", 20, 60, 120);
                btnEdit = MakeButton("تعديل", 150, 60, 100);
                btnDelete = MakeButton("حذف", 260, 60, 100);
                btnPermissions = MakeButton("صلاحيات الدور", 370, 60, 130);

                btnNew.Click += BtnNew_Click;
                btnEdit.Click += BtnEdit_Click;
                btnDelete.Click += BtnDelete_Click;
                btnPermissions.Click += BtnPermissions_Click;

                dgvRoles = new DataGridView
                {
                    Location = new Point(20, 100),
                    Size = new Size(740, 330),
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
                dgvRoles.DoubleClick += (s, e) => EditSelectedRole();

                dgvRoles.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRoleID", HeaderText = "الرقم", DataPropertyName = "RoleID", FillWeight = 70 });
                dgvRoles.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRoleName", HeaderText = "اسم الدور", DataPropertyName = "RoleName", FillWeight = 180 });

                lblStatus = new Label
                {
                    Location = new Point(20, 440),
                    Size = new Size(740, 30),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = Color.DarkGreen
                };

                Controls.AddRange(new Control[]
                {
                lblTitle,
                btnNew,
                btnEdit,
                btnDelete,
                btnPermissions,
                dgvRoles,
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

            private void LoadRoles()
            {
                try
                {
                    _roles = _service.GetRoles();
                    dgvRoles.DataSource = null;
                    dgvRoles.DataSource = _roles;
                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = $"عدد الأدوار: {_roles.Count}";
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private AppRole GetSelectedRole()
            {
                return dgvRoles.CurrentRow?.DataBoundItem as AppRole;
            }

            private void BtnNew_Click(object sender, EventArgs e)
            {
                using (var frm = new RoleEditForm())
                {
                    frm.ShowDialog();
                }
                LoadRoles();
            }

            private void BtnEdit_Click(object sender, EventArgs e)
            {
                EditSelectedRole();
            }

            private void EditSelectedRole()
            {
                var role = GetSelectedRole();
                if (role == null)
                {
                    MessageBox.Show("اختر دورًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var frm = new RoleEditForm(role.RoleID, role.RoleName))
                {
                    frm.ShowDialog();
                }
                LoadRoles();
            }

            private void BtnDelete_Click(object sender, EventArgs e)
            {
                try
                {
                    var role = GetSelectedRole();
                    if (role == null)
                    {
                        MessageBox.Show("اختر دورًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (MessageBox.Show("هل تريد حذف الدور؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;

                    _service.DeleteRole(role.RoleID);
                    LoadRoles();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void BtnPermissions_Click(object sender, EventArgs e)
            {
                var role = GetSelectedRole();
                if (role == null)
                {
                    MessageBox.Show("اختر دورًا أولًا.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var frm = new RolePermissionsForm(role.RoleID, role.RoleName))
                {
                    frm.ShowDialog();
                }
            }
        }
    }