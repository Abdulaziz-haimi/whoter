using System;
using System.Data;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace water3
{
    public partial class LoginForm : Form
    {
        private Color primaryColor = Color.FromArgb(52, 152, 219);
        private Color successColor = Color.FromArgb(46, 204, 113);
        private Color errorColor = Color.FromArgb(231, 76, 60);

        private TextBox txtUsername;
        private TextBox txtPassword;
        private CheckBox chkRemember;
        private Button btnLogin;
        private Button btnExit;
        private Label lblStatus;

        private int failedAttempts = 0;
        private const int MAX_ATTEMPTS = 3;

        // سلسلة الاتصال بقاعدة البيانات
      
        public LoginForm()
        {
            InitializeForm();
            CreateControls();
            this.Load += LoginForm_Load;
        }

        private void InitializeForm()
        {
            this.Text = "نظام إدارة المياه - تسجيل الدخول";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Tahoma", 10);
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void CreateControls()
        {
            Label lblUsername = new Label { Text = "اسم المستخدم:", Location = new Point(50, 50), Size = new Size(120, 25) };
            txtUsername = new TextBox { Location = new Point(180, 50), Size = new Size(250, 30) };

            Label lblPassword = new Label { Text = "كلمة المرور:", Location = new Point(50, 100), Size = new Size(120, 25) };
            txtPassword = new TextBox { Location = new Point(180, 100), Size = new Size(250, 30), PasswordChar = '●' };

            chkRemember = new CheckBox { Text = "تذكر بيانات الدخول", Location = new Point(180, 140), Checked = true };

            lblStatus = new Label { Location = new Point(50, 180), Size = new Size(380, 25), ForeColor = errorColor, Visible = false };

            btnLogin = new Button
            {
                Text = "تسجيل الدخول",
                Location = new Point(180, 220),
                Size = new Size(120, 35),
                BackColor = primaryColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.Click += BtnLogin_Click;

            btnExit = new Button
            {
                Text = "خروج",
                Location = new Point(310, 220),
                Size = new Size(120, 35),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnExit.Click += (s, e) => Application.Exit();

            this.Controls.AddRange(new Control[] { lblUsername, txtUsername, lblPassword, txtPassword, chkRemember, lblStatus, btnLogin, btnExit });
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            LoadSavedCredentials();
            txtUsername.Focus();
        }

        private void LoadSavedCredentials()
        {
            try
            {
                string savedUsername = Properties.Settings.Default.Username ?? "";
                string savedPassword = Properties.Settings.Default.Password ?? "";
                if (!string.IsNullOrEmpty(savedUsername))
                {
                    txtUsername.Text = savedUsername;
                    txtPassword.Text = savedPassword;
                    chkRemember.Checked = true;
                }
            }
            catch { }
        }

        private void SaveCredentials()
        {
            if (chkRemember.Checked)
            {
                Properties.Settings.Default.Username = txtUsername.Text;
                Properties.Settings.Default.Password = txtPassword.Text;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.Username = "";
                Properties.Settings.Default.Password = "";
                Properties.Settings.Default.Save();
            }
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                ShowStatus("يرجى إدخال اسم المستخدم", false);
                txtUsername.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                ShowStatus("يرجى إدخال كلمة المرور", false);
                txtPassword.Focus();
                return;
            }

            SetControlsEnabled(false);
            btnLogin.Text = "جاري التحقق...";

            bool isValid = await ValidateCredentialsAsync(txtUsername.Text, txtPassword.Text);

            if (isValid)
            {
                ShowStatus("تم تسجيل الدخول بنجاح!", true);
                SaveCredentials();
                failedAttempts = 0;
                await Task.Delay(500);

                MainForm mainForm = new MainForm();
                mainForm.Show();
                this.Hide();
            }
            else
            {
                failedAttempts++;
                if (failedAttempts >= MAX_ATTEMPTS)
                {
                    ShowStatus("تم تجاوز الحد الأقصى للمحاولات. حاول لاحقاً", false);
                    btnLogin.Enabled = false;
                    Timer enableTimer = new Timer();
                    enableTimer.Interval = 300000; // 5 دقائق
                    enableTimer.Tick += (s, ev) =>
                    {
                        btnLogin.Enabled = true;
                        failedAttempts = 0;
                        enableTimer.Stop();
                    };
                    enableTimer.Start();
                }
                else
                {
                    ShowStatus($"بيانات الدخول غير صحيحة. المحاولات المتبقية: {MAX_ATTEMPTS - failedAttempts}", false);
                }
            }

            SetControlsEnabled(true);
            btnLogin.Text = "تسجيل الدخول";
        }

        private async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            return await Task.Run(() =>
            {
                try
                {
                    using (SqlConnection con = Db.GetConnection())
                    {
                        string query = "SELECT PasswordHash FROM Users WHERE Username = @Username AND IsActive = 1";
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@Username", username);

                        con.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            string storedHash = result.ToString();
                            string enteredHash = HashPassword(password);
                            return storedHash == enteredHash;
                        }
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            });
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

        private void ShowStatus(string message, bool isSuccess)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = isSuccess ? successColor : errorColor;
            lblStatus.Visible = true;

            Timer hideTimer = new Timer { Interval = 5000 };
            hideTimer.Tick += (s, e) =>
            {
                lblStatus.Visible = false;
                hideTimer.Stop();
            };
            hideTimer.Start();
        }

        private void SetControlsEnabled(bool enabled)
        {
            txtUsername.Enabled = enabled;
            txtPassword.Enabled = enabled;
            chkRemember.Enabled = enabled;
            btnLogin.Enabled = enabled;
            btnExit.Enabled = enabled;
        }
    }
}
