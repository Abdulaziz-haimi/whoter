using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using water3.Services;
namespace water3.Forms
{
    public partial class ResetPasswordForm : Form
    {
   
       
            private readonly UserAuthService _service = new UserAuthService();
            private readonly int _userId;

            private TextBox txtPassword;
            private TextBox txtConfirmPassword;
            private Label lblInfo;
            private Label lblStatus;
            private Button btnSave;
            private Button btnCancel;

            public ResetPasswordForm(int userId, string userName)
            {
                _userId = userId;
                InitializeComponent(userName);
            }

            private void InitializeComponent(string userName)
            {
                Text = "إعادة تعيين كلمة المرور";
                StartPosition = FormStartPosition.CenterParent;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                ClientSize = new Size(450, 230);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MaximizeBox = false;
                MinimizeBox = false;

                lblInfo = new Label
                {
                    Text = "المستخدم: " + userName,
                    AutoSize = true,
                    Location = new Point(160, 25),
                    Font = new Font("Tahoma", 11F, FontStyle.Bold)
                };

                Controls.Add(new Label { Text = "كلمة المرور الجديدة", AutoSize = true, Location = new Point(330, 70) });
                Controls.Add(new Label { Text = "تأكيد كلمة المرور", AutoSize = true, Location = new Point(330, 110) });

                txtPassword = new TextBox { Location = new Point(80, 65), Size = new Size(220, 27), UseSystemPasswordChar = true };
                txtConfirmPassword = new TextBox { Location = new Point(80, 105), Size = new Size(220, 27), UseSystemPasswordChar = true };

                btnSave = new Button { Text = "حفظ", Location = new Point(80, 150), Size = new Size(110, 35), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                btnCancel = new Button { Text = "إلغاء", Location = new Point(200, 150), Size = new Size(110, 35), BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                lblStatus = new Label { Location = new Point(20, 190), Size = new Size(400, 24), TextAlign = ContentAlignment.MiddleCenter };

                btnSave.Click += BtnSave_Click;
                btnCancel.Click += (s, e) => Close();

                Controls.AddRange(new Control[] { lblInfo, txtPassword, txtConfirmPassword, btnSave, btnCancel, lblStatus });
            }

            private void BtnSave_Click(object sender, EventArgs e)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        lblStatus.ForeColor = Color.DarkRed;
                        lblStatus.Text = "أدخل كلمة المرور الجديدة.";
                        return;
                    }

                    if (txtPassword.Text != txtConfirmPassword.Text)
                    {
                        lblStatus.ForeColor = Color.DarkRed;
                        lblStatus.Text = "تأكيد كلمة المرور غير مطابق.";
                        return;
                    }

                    _service.ResetPassword(_userId, txtPassword.Text);
                    lblStatus.ForeColor = Color.DarkGreen;
                    lblStatus.Text = "تم تغيير كلمة المرور بنجاح.";
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