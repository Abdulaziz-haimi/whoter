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
namespace water3.Forms
{
    public partial class EditUserForm : Form
    {
      
      
            private readonly UserAuthService _service = new UserAuthService();
            private readonly int _userId;

            private TextBox txtUserName;
            private TextBox txtFullName;
            private TextBox txtPhone;
            private ComboBox cboRoles;
            private CheckBox chkIsActive;
            private Button btnSave;
            private Button btnCancel;
            private Label lblStatus;

            public EditUserForm(int userId)
            {
                _userId = userId;
                InitializeComponent();
                LoadRoles();
                LoadUser();
            }

            private void InitializeComponent()
            {
                Text = "تعديل المستخدم";
                StartPosition = FormStartPosition.CenterParent;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                ClientSize = new Size(500, 300);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;

                Controls.Add(new Label { Text = "اسم المستخدم", AutoSize = true, Location = new Point(390, 40) });
                Controls.Add(new Label { Text = "الاسم الكامل", AutoSize = true, Location = new Point(390, 80) });
                Controls.Add(new Label { Text = "رقم الهاتف", AutoSize = true, Location = new Point(390, 120) });
                Controls.Add(new Label { Text = "الصلاحية", AutoSize = true, Location = new Point(390, 160) });

                txtUserName = new TextBox { Location = new Point(120, 35), Size = new Size(250, 27) };
                txtFullName = new TextBox { Location = new Point(120, 75), Size = new Size(250, 27) };
                txtPhone = new TextBox { Location = new Point(120, 115), Size = new Size(250, 27) };
                cboRoles = new ComboBox { Location = new Point(120, 155), Size = new Size(250, 28), DropDownStyle = ComboBoxStyle.DropDownList };
                chkIsActive = new CheckBox { Text = "نشط", Location = new Point(120, 195), AutoSize = true };

                btnSave = new Button { Text = "حفظ", Location = new Point(120, 230), Size = new Size(120, 35), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                btnCancel = new Button { Text = "إلغاء", Location = new Point(250, 230), Size = new Size(120, 35), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                lblStatus = new Label { Location = new Point(40, 270), Size = new Size(420, 24), TextAlign = ContentAlignment.MiddleCenter };

                btnSave.Click += BtnSave_Click;
                btnCancel.Click += (s, e) => Close();

                Controls.AddRange(new Control[] { txtUserName, txtFullName, txtPhone, cboRoles, chkIsActive, btnSave, btnCancel, lblStatus });
            }

            private void LoadRoles()
            {
                cboRoles.DataSource = _service.GetRoles();
                cboRoles.DisplayMember = nameof(AppRole.RoleName);
                cboRoles.ValueMember = nameof(AppRole.RoleID);
            }

            private void LoadUser()
            {
                var user = _service.GetUserById(_userId);
                if (user == null)
                {
                    MessageBox.Show("المستخدم غير موجود.", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Close();
                    return;
                }

                txtUserName.Text = user.UserName;
                txtFullName.Text = user.FullName;
                txtPhone.Text = user.Phone;
                cboRoles.SelectedValue = user.RoleID;
                chkIsActive.Checked = user.IsActive;
            }

            private void BtnSave_Click(object sender, EventArgs e)
            {
                try
                {
                    _service.UpdateUser(
                        _userId,
                        txtUserName.Text,
                        txtFullName.Text,
                        Convert.ToInt32(cboRoles.SelectedValue),
                        txtPhone.Text,
                        chkIsActive.Checked);

                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = "تم حفظ التعديلات بنجاح.";
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