using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace water3
{
    public partial class RegistrationForm : Form
    {
        public RegistrationForm()
        {
            InitializeComponent();

            // Events
            btnRegister.Click += BtnRegister_Click;
            chkShowPassword.CheckedChanged += (s, e) => TogglePasswordVisibility(chkShowPassword.Checked);

            txtFullName.TextChanged += (s, e) => ValidateInputsLive();
            txtUsername.TextChanged += (s, e) => ValidateInputsLive();
            txtPassword.TextChanged += (s, e) => ValidateInputsLive();
            txtConfirmPassword.TextChanged += (s, e) => ValidateInputsLive();
            txtPhone.TextChanged += (s, e) => ValidateInputsLive();

            txtPhone.KeyPress += TxtPhone_KeyPress;

            Shown += (s, e) =>
            {
                txtFullName.Focus();
                ValidateInputsLive();
            };
        }

        private void TogglePasswordVisibility(bool show)
        {
            // UseSystemPasswordChar = true => مخفي
            txtPassword.UseSystemPasswordChar = !show;
            txtConfirmPassword.UseSystemPasswordChar = !show;
        }

        private void TxtPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            // يسمح بالأرقام + Backspace فقط
            if (char.IsControl(e.KeyChar)) return;
            if (!char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void ValidateInputsLive()
        {
            lblStatus.Text = "";

            bool ok =
                !string.IsNullOrWhiteSpace(txtFullName.Text) &&
                !string.IsNullOrWhiteSpace(txtUsername.Text) &&
                !string.IsNullOrWhiteSpace(txtPassword.Text) &&
                !string.IsNullOrWhiteSpace(txtConfirmPassword.Text) &&
                txtPassword.Text == txtConfirmPassword.Text &&
                txtPassword.Text.Length >= 6;

            // phone optional: إذا مكتوب لازم يكون أرقام
            if (!string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                if (!long.TryParse(txtPhone.Text.Trim(), out _))
                    ok = false;
            }

            btnRegister.Enabled = ok;
            btnRegister.BackColor = ok ? Color.FromArgb(46, 204, 113) : Color.FromArgb(170, 170, 170);
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            lblStatus.ForeColor = Color.FromArgb(220, 53, 69);
            lblStatus.Text = "";

            string fullName = (txtFullName.Text ?? "").Trim();
            string userName = (txtUsername.Text ?? "").Trim();
            string pass = txtPassword.Text ?? "";
            string pass2 = txtConfirmPassword.Text ?? "";
            string phoneText = (txtPhone.Text ?? "").Trim();

            if (fullName.Length == 0 || userName.Length == 0 || pass.Length == 0 || pass2.Length == 0)
            {
                lblStatus.Text = "يرجى ملء جميع الحقول المطلوبة.";
                return;
            }

            if (userName.Length < 3)
            {
                lblStatus.Text = "اسم المستخدم يجب أن يكون 3 أحرف على الأقل.";
                return;
            }

            if (pass.Length < 6)
            {
                lblStatus.Text = "كلمة المرور يجب أن تكون 6 أحرف/أرقام على الأقل.";
                return;
            }

            if (pass != pass2)
            {
                lblStatus.Text = "كلمة المرور وتأكيدها غير متطابقين.";
                return;
            }

            long phoneValue = 0;
            bool hasPhone = !string.IsNullOrWhiteSpace(phoneText);
            if (hasPhone && !long.TryParse(phoneText, out phoneValue))
            {
                lblStatus.Text = "رقم الهاتف غير صحيح.";
                return;
            }

            try
            {
                using (SqlConnection con = Db.GetConnection())
                {
                    con.Open();

                    // تحقق من تكرار اسم المستخدم
                    using (var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE UserName = @UserName", con))
                    {
                        checkCmd.Parameters.AddWithValue("@UserName", userName);
                        int exists = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (exists > 0)
                        {
                            lblStatus.Text = "اسم المستخدم موجود مسبقًا.";
                            return;
                        }
                    }

                    // إدخال المستخدم الجديد
                    string insertQuery = @"
INSERT INTO Users
(UserName, FullName, PasswordHash, RoleID, IsActive, Phone, CreatedDate)
VALUES
(@UserName, @FullName, @PasswordHash, @RoleID, @IsActive, @Phone, @CreatedDate)";

                    using (var insertCmd = new SqlCommand(insertQuery, con))
                    {
                        insertCmd.Parameters.AddWithValue("@UserName", userName);
                        insertCmd.Parameters.AddWithValue("@FullName", (object)fullName ?? DBNull.Value);

                        // ملاحظة: الأفضل أمنياً استخدام salt + PBKDF2/BCrypt.
                        // لكن طبقاً لكودك الحالي سنبقى SHA256 لتوافق قاعدة البيانات.
                        insertCmd.Parameters.AddWithValue("@PasswordHash", HashPassword(pass));

                        insertCmd.Parameters.AddWithValue("@RoleID", 2);     // افتراضي
                        insertCmd.Parameters.AddWithValue("@IsActive", true);

                        if (hasPhone)
                            insertCmd.Parameters.AddWithValue("@Phone", phoneValue);
                        else
                            insertCmd.Parameters.AddWithValue("@Phone", DBNull.Value);

                        insertCmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now.ToString("yyyy-MM-dd"));

                        insertCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("تم إنشاء الحساب بنجاح!", "تم التسجيل",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Close();
                }
            }
            catch (SqlException ex)
            {
                lblStatus.Text = "خطأ قاعدة بيانات: " + ex.Message;
            }
            catch (Exception ex)
            {
                lblStatus.Text = "حدث خطأ أثناء التسجيل: " + ex.Message;
            }
            finally
            {
                ValidateInputsLive();
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
