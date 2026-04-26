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
    public partial class RolePermissionsForm : Form
    {
     
        //public class RolePermissionsForm : Form
        //{
            private readonly RolePermissionService _service = new RolePermissionService();
            private readonly int _roleId;
            private readonly string _roleName;
            private List<RolePermissionItem> _items = new List<RolePermissionItem>();

            private Label lblTitle;
            private CheckedListBox clbPermissions;
            private Button btnCheckAll;
            private Button btnUncheckAll;
            private Button btnSave;
            private Button btnCancel;
            private Label lblStatus;

            public RolePermissionsForm(int roleId, string roleName)
            {
                _roleId = roleId;
                _roleName = roleName;
                InitializeComponent();
                PermissionHelper.EnforceFormPermission(this, "USERS_MANAGE");
                LoadPermissions();
            }

            private void InitializeComponent()
            {
                Text = "صلاحيات الدور";
                StartPosition = FormStartPosition.CenterParent;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                ClientSize = new Size(700, 520);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;

                lblTitle = new Label
                {
                    Text = "صلاحيات الدور: " + _roleName,
                    Font = new Font("Tahoma", 14F, FontStyle.Bold),
                    AutoSize = true,
                    Location = new Point(20, 20),
                    ForeColor = Color.FromArgb(0, 102, 204)
                };

                clbPermissions = new CheckedListBox
                {
                    Location = new Point(20, 60),
                    Size = new Size(650, 360),
                    CheckOnClick = true,
                    Font = new Font("Tahoma", 10F)
                };

                btnCheckAll = new Button { Text = "تحديد الكل", Location = new Point(20, 430), Size = new Size(110, 35), BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                btnUncheckAll = new Button { Text = "إلغاء الكل", Location = new Point(140, 430), Size = new Size(110, 35), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                btnSave = new Button { Text = "حفظ", Location = new Point(460, 430), Size = new Size(100, 35), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                btnCancel = new Button { Text = "إلغاء", Location = new Point(570, 430), Size = new Size(100, 35), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                lblStatus = new Label { Location = new Point(20, 470), Size = new Size(650, 24), TextAlign = ContentAlignment.MiddleCenter };

                btnCheckAll.Click += (s, e) => SetAllChecked(true);
                btnUncheckAll.Click += (s, e) => SetAllChecked(false);
                btnSave.Click += BtnSave_Click;
                btnCancel.Click += (s, e) => Close();

                Controls.AddRange(new Control[] { lblTitle, clbPermissions, btnCheckAll, btnUncheckAll, btnSave, btnCancel, lblStatus });
            }

            private void LoadPermissions()
            {
                try
                {
                    _items = _service.GetRolePermissions(_roleId);
                    clbPermissions.Items.Clear();

                    foreach (var item in _items)
                    {
                        int index = clbPermissions.Items.Add($"[{item.Category}] {item.PermissionName} - {item.PermissionKey}");
                        clbPermissions.SetItemChecked(index, item.IsAllowed);
                    }
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }

            private void SetAllChecked(bool value)
            {
                for (int i = 0; i < clbPermissions.Items.Count; i++)
                    clbPermissions.SetItemChecked(i, value);
            }

            private void BtnSave_Click(object sender, EventArgs e)
            {
                try
                {
                    for (int i = 0; i < _items.Count; i++)
                        _items[i].IsAllowed = clbPermissions.GetItemChecked(i);

                    _service.SaveRolePermissions(_roleId, _items);
                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = "تم حفظ صلاحيات الدور بنجاح.";
                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    lblStatus.ForeColor = Color.DarkRed;
                    lblStatus.Text = ex.Message;
                }
            }
        }
    }