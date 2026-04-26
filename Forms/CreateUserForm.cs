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
namespace water3.Forms
{
    public partial class CreateUserForm : Form
    {

       //public class CreateUserForm : Form
       //{
              private readonly UserAuthService _service = new UserAuthService();

            private Label lblTitle;
            private Label lblUserName;
            private Label lblFullName;
            private Label lblPassword;
            private Label lblConfirmPassword;
            private Label lblRole;
            private Label lblPhone;

            private TextBox txtUserName;
            private TextBox txtFullName;
            private TextBox txtPassword;
            private TextBox txtConfirmPassword;
            private TextBox txtPhone;

            private ComboBox cboRoles;
            private CheckBox chkIsActive;
            private Button btnSave;
            private Button btnCancel;
            private Label lblStatus;

            public CreateUserForm()
            {
                InitializeComponent();
                LoadRoles();
            }

            private void InitializeComponent()
            {
                Text = "إنشاء مستخدم جديد";
                StartPosition = FormStartPosition.CenterScreen;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.White;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;
                ClientSize = new Size(520, 430);

                lblTitle = new Label
                {
                    Text = "إنشاء مستخدم جديد",
                    Font = new Font("Tahoma", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(0, 102, 204),
                    AutoSize = true,
                    Location = new Point(170, 20)
                };

                lblUserName = MakeLabel("اسم المستخدم", 60);
                lblFullName = MakeLabel("الاسم الكامل", 110);
                lblPassword = MakeLabel("كلمة المرور", 160);
                lblConfirmPassword = MakeLabel("تأكيد كلمة المرور", 210);
                lblRole = MakeLabel("الصلاحية", 260);
                lblPhone = MakeLabel("رقم الهاتف", 310);

                txtUserName = MakeTextBox(60);
                txtFullName = MakeTextBox(110);
                txtPassword = MakeTextBox(160, true);
                txtConfirmPassword = MakeTextBox(210, true);
                txtPhone = MakeTextBox(310);

                cboRoles = new ComboBox
                {
                    Location = new Point(150, 260),
                    Size = new Size(250, 28),
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                chkIsActive = new CheckBox
                {
                    Text = "المستخدم نشط",
                    Checked = true,
                    AutoSize = true,
                    Location = new Point(150, 350)
                };

                btnSave = new Button
                {
                    Text = "حفظ",
                    Location = new Point(150, 380),
                    Size = new Size(120, 35),
                    BackColor = Color.FromArgb(0, 122, 204),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnSave.Click += BtnSave_Click;

                btnCancel = new Button
                {
                    Text = "إلغاء",
                    Location = new Point(280, 380),
                    Size = new Size(120, 35),
                    BackColor = Color.Gray,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnCancel.Click += (s, e) => Close();

                lblStatus = new Label
                {
                    AutoSize = false,
                    Size = new Size(360, 30),
                    Location = new Point(80, 345),
                    ForeColor = Color.DarkRed,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                Controls.AddRange(new Control[]
                {
                lblTitle,
                lblUserName, txtUserName,
                lblFullName, txtFullName,
                lblPassword, txtPassword,
                lblConfirmPassword, txtConfirmPassword,
                lblRole, cboRoles,
                lblPhone, txtPhone,
                chkIsActive,
                lblStatus,
                btnSave, btnCancel
                });
            }

            private Label MakeLabel(string text, int top)
            {
                return new Label
                {
                    Text = text,
                    AutoSize = true,
                    Location = new Point(410, top + 5)
                };
            }

            private TextBox MakeTextBox(int top, bool isPassword = false)
            {
                return new TextBox
                {
                    Location = new Point(150, top),
                    Size = new Size(250, 27),
                    UseSystemPasswordChar = isPassword
                };
            }

            private void LoadRoles()
            {
                try
                {
                    List<AppRole> roles = _service.GetRoles();
                    cboRoles.DataSource = roles;
                    cboRoles.DisplayMember = nameof(AppRole.RoleName);
                    cboRoles.ValueMember = nameof(AppRole.RoleID);
                }
                catch (Exception ex)
                {
                    ShowStatus("فشل تحميل الصلاحيات: " + ex.Message, false);
                }
            }

            private void BtnSave_Click(object sender, EventArgs e)
            {
                try
                {
                    lblStatus.Text = string.Empty;

                    if (string.IsNullOrWhiteSpace(txtUserName.Text))
                    {
                        ShowStatus("اسم المستخدم مطلوب.", false);
                        txtUserName.Focus();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        ShowStatus("كلمة المرور مطلوبة.", false);
                        txtPassword.Focus();
                        return;
                    }

                    if (txtPassword.Text != txtConfirmPassword.Text)
                    {
                        ShowStatus("تأكيد كلمة المرور غير مطابق.", false);
                        txtConfirmPassword.Focus();
                        return;
                    }

                    if (cboRoles.SelectedValue == null)
                    {
                        ShowStatus("اختر الصلاحية.", false);
                        return;
                    }

                    int newId = _service.CreateUser(
                        txtUserName.Text,
                        txtFullName.Text,
                        txtPassword.Text,
                        Convert.ToInt32(cboRoles.SelectedValue),
                        txtPhone.Text,
                        chkIsActive.Checked);

                    ShowStatus("تم إنشاء المستخدم بنجاح. رقم المستخدم: " + newId, true);
                    ClearInputs();
                }
                catch (Exception ex)
                {
                    ShowStatus(ex.Message, false);
                }
            }

            private void ClearInputs()
            {
                txtUserName.Clear();
                txtFullName.Clear();
                txtPassword.Clear();
                txtConfirmPassword.Clear();
                txtPhone.Clear();
                chkIsActive.Checked = true;
                txtUserName.Focus();
            }

            private void ShowStatus(string message, bool success)
            {
                lblStatus.ForeColor = success ? Color.DarkGreen : Color.DarkRed;
                lblStatus.Text = message;
            }
        }
    }