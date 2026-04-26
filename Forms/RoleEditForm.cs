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
using System.Drawing;
using System.Windows.Forms;
using water3.Services;
namespace water3.Forms
{
    public partial class RoleEditForm : Form
    {

        //public class RoleEditForm : Form
        //{
            private readonly RolePermissionService _service = new RolePermissionService();
            private readonly int? _roleId;

            private TextBox txtRoleName;
            private Button btnSave;
            private Button btnCancel;
            private Label lblStatus;

            public RoleEditForm(int? roleId = null, string roleName = null)
            {
                _roleId = roleId;
                InitializeComponent();
                txtRoleName.Text = roleName ?? string.Empty;
            }

            private void InitializeComponent()
            {
                Text = _roleId.HasValue ? "تعديل الدور" : "دور جديد";
                StartPosition = FormStartPosition.CenterParent;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                ClientSize = new Size(420, 180);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;

                Controls.Add(new Label { Text = "اسم الدور", AutoSize = true, Location = new Point(320, 35) });
                txtRoleName = new TextBox { Location = new Point(60, 30), Size = new Size(240, 27) };

                btnSave = new Button { Text = "حفظ", Location = new Point(60, 80), Size = new Size(100, 35), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                btnCancel = new Button { Text = "إلغاء", Location = new Point(170, 80), Size = new Size(100, 35), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                lblStatus = new Label { Location = new Point(20, 125), Size = new Size(360, 24), TextAlign = ContentAlignment.MiddleCenter };

                btnSave.Click += BtnSave_Click;
                btnCancel.Click += (s, e) => Close();

                Controls.AddRange(new Control[] { txtRoleName, btnSave, btnCancel, lblStatus });
            }

            private void BtnSave_Click(object sender, EventArgs e)
            {
                try
                {
                    if (_roleId.HasValue)
                        _service.UpdateRole(_roleId.Value, txtRoleName.Text);
                    else
                        _service.CreateRole(txtRoleName.Text);

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