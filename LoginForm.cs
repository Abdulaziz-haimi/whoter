using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using water3.Models;
using water3.Services;

namespace water3
{
    public partial class LoginForm : Form
    {
        private readonly Color primaryColor = Color.FromArgb(24, 74, 111);
        private readonly Color accentColor = Color.FromArgb(47, 128, 237);
        private readonly Color successColor = Color.FromArgb(34, 139, 84);
        private readonly Color errorColor = Color.FromArgb(192, 57, 43);
        private readonly Color mutedTextColor = Color.FromArgb(100, 116, 139);
        private readonly Color pageBackColor = Color.FromArgb(242, 245, 249);
        private readonly Color panelBackColor = Color.White;

        private readonly UserAuthService _authService = new UserAuthService();
        private readonly PermissionService _permissionService = new PermissionService();

        private AppUser _loggedInUser;

        private TextBox txtUsername;
        private TextBox txtPassword;
        private CheckBox chkRemember;
        private CheckBox chkShowPassword;
        private Button btnLogin;
        private Button btnExit;
        private Label lblStatus;
        private Label lblAttempts;
        private Label lblFooter;

        private Panel cardPanel;
        private Panel brandPanel;

        private int failedAttempts = 0;
        private bool isLockedOut = false;

        private const int MAX_ATTEMPTS = 3;

        public LoginForm()
        {
            InitializeForm();
            CreateControls();

            Load += LoginForm_Load;
            AcceptButton = btnLogin;
            CancelButton = btnExit;
        }

        private void InitializeForm()
        {
            Text = "نظام إدارة المياه - تسجيل الدخول";
            ClientSize = new Size(920, 560);
            MinimumSize = new Size(920, 560);
            MaximumSize = new Size(920, 560);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Tahoma", 10F);
            BackColor = pageBackColor;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
        }

        private void CreateControls()
        {
            TableLayoutPanel root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = pageBackColor,
                Padding = new Padding(26)
            };

            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 39F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 61F));

            brandPanel = CreateBrandPanel();
            cardPanel = CreateLoginCard();

            root.Controls.Add(brandPanel, 0, 0);
            root.Controls.Add(cardPanel, 1, 0);

            Controls.Add(root);
        }

        private Panel CreateBrandPanel()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = primaryColor,
                Margin = new Padding(0, 0, 16, 0),
                Padding = new Padding(28)
            };

            Label logo = new Label
            {
                Text = "💧",
                Dock = DockStyle.Top,
                Height = 90,
                ForeColor = Color.White,
                Font = new Font("Segoe UI Emoji", 42F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label title = new Label
            {
                Text = "نظام إدارة المياه",
                Dock = DockStyle.Top,
                Height = 52,
                ForeColor = Color.White,
                Font = new Font("Tahoma", 19F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label subtitle = new Label
            {
                Text = "منصة مكتبية لإدارة المشتركين، الفواتير، السداد، والتقارير المحاسبية",
                Dock = DockStyle.Top,
                Height = 74,
                ForeColor = Color.FromArgb(219, 234, 254),
                Font = new Font("Tahoma", 10F),
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(10)
            };

            Panel separator = new Panel
            {
                Dock = DockStyle.Top,
                Height = 1,
                BackColor = Color.FromArgb(94, 151, 191),
                Margin = new Padding(0, 18, 0, 18)
            };

            Label features = new Label
            {
                Text =
                    "• أمان وصلاحيات للمستخدمين" + Environment.NewLine +
                    "• متابعة السندات والحركات" + Environment.NewLine +
                    "• تقارير مالية وتشغيلية" + Environment.NewLine +
                    "• واجهة كلاسيكية حديثة",
                Dock = DockStyle.Fill,
                ForeColor = Color.FromArgb(226, 232, 240),
                Font = new Font("Tahoma", 10F),
                TextAlign = ContentAlignment.TopRight,
                Padding = new Padding(10, 22, 10, 10)
            };

            Label version = new Label
            {
                Text = "Water Billing System",
                Dock = DockStyle.Bottom,
                Height = 32,
                ForeColor = Color.FromArgb(191, 219, 254),
                Font = new Font("Tahoma", 8.5F),
                TextAlign = ContentAlignment.MiddleCenter
            };

            panel.Controls.Add(features);
            panel.Controls.Add(separator);
            panel.Controls.Add(subtitle);
            panel.Controls.Add(title);
            panel.Controls.Add(logo);
            panel.Controls.Add(version);

            return panel;
        }

        private Panel CreateLoginCard()
        {
            Panel outer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = pageBackColor,
                Padding = new Padding(28, 22, 28, 22)
            };

            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = panelBackColor,
                Padding = new Padding(42, 34, 42, 24)
            };

            Label title = new Label
            {
                Text = "تسجيل الدخول",
                Dock = DockStyle.Top,
                Height = 42,
                ForeColor = primaryColor,
                Font = new Font("Tahoma", 18F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };

            Label subtitle = new Label
            {
                Text = "أدخل بيانات حسابك للوصول إلى النظام",
                Dock = DockStyle.Top,
                Height = 30,
                ForeColor = mutedTextColor,
                Font = new Font("Tahoma", 9.5F),
                TextAlign = ContentAlignment.MiddleRight
            };

            Panel formHost = new Panel
            {
                Dock = DockStyle.Top,
                Height = 275,
                BackColor = panelBackColor,
                Padding = new Padding(0, 18, 0, 0)
            };

            TableLayoutPanel formLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 7,
                BackColor = panelBackColor
            };

            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42));
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
            formLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

            txtUsername = CreateTextBox();
            txtPassword = CreateTextBox();
            txtPassword.PasswordChar = '●';

            chkRemember = new CheckBox
            {
                Text = "تذكر اسم المستخدم",
                Checked = true,
                Dock = DockStyle.Right,
                AutoSize = true,
                ForeColor = Color.FromArgb(51, 65, 85),
                Font = new Font("Tahoma", 9F)
            };

            chkShowPassword = new CheckBox
            {
                Text = "إظهار كلمة المرور",
                Dock = DockStyle.Left,
                AutoSize = true,
                ForeColor = Color.FromArgb(51, 65, 85),
                Font = new Font("Tahoma", 9F)
            };
            chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;

            Panel optionsPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = panelBackColor
            };
            optionsPanel.Controls.Add(chkRemember);
            optionsPanel.Controls.Add(chkShowPassword);

            btnLogin = CreateButton("تسجيل الدخول", accentColor);
            btnLogin.Click += BtnLogin_Click;

            btnExit = CreateButton("خروج", Color.FromArgb(100, 116, 139));
            btnExit.Click += (s, e) => Application.Exit();

            TableLayoutPanel buttonsLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = panelBackColor
            };
            buttonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            buttonsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            buttonsLayout.Controls.Add(btnExit, 0, 0);
            buttonsLayout.Controls.Add(btnLogin, 1, 0);

            lblStatus = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = errorColor,
                Font = new Font("Tahoma", 9F),
                Visible = false
            };

            lblAttempts = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = mutedTextColor,
                Font = new Font("Tahoma", 8.5F),
                Text = "عدد المحاولات المسموح بها: 3"
            };

            formLayout.Controls.Add(CreateInputBlock("اسم المستخدم", txtUsername), 0, 0);
            formLayout.Controls.Add(CreateInputBlock("كلمة المرور", txtPassword), 0, 1);
            formLayout.Controls.Add(optionsPanel, 0, 2);
            formLayout.Controls.Add(lblStatus, 0, 3);
            formLayout.Controls.Add(buttonsLayout, 0, 4);
            formLayout.Controls.Add(lblAttempts, 0, 5);

            formHost.Controls.Add(formLayout);

            lblFooter = new Label
            {
                Text = "لا يتم حفظ كلمة المرور في إعدادات التطبيق لأسباب أمنية.",
                Dock = DockStyle.Bottom,
                Height = 32,
                ForeColor = Color.FromArgb(100, 116, 139),
                Font = new Font("Tahoma", 8.5F),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Panel line = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = Color.FromArgb(226, 232, 240),
                Margin = new Padding(0, 12, 0, 8)
            };

            card.Controls.Add(lblFooter);
            card.Controls.Add(line);
            card.Controls.Add(formHost);
            card.Controls.Add(subtitle);
            card.Controls.Add(title);

            outer.Controls.Add(card);

            return outer;
        }

        private TextBox CreateTextBox()
        {
            TextBox txt = new TextBox
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Tahoma", 10.5F),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42)
            };

            txt.KeyDown += LoginTextBox_KeyDown;

            return txt;
        }

        private Panel CreateInputBlock(string caption, TextBox textBox)
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(0, 0, 0, 8),
                BackColor = panelBackColor
            };

            Label label = new Label
            {
                Text = caption,
                Dock = DockStyle.Top,
                Height = 22,
                ForeColor = Color.FromArgb(51, 65, 85),
                Font = new Font("Tahoma", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };

            textBox.Height = 29;
            textBox.Dock = DockStyle.Top;

            panel.Controls.Add(textBox);
            panel.Controls.Add(label);

            return panel;
        }

        private Button CreateButton(string text, Color backColor)
        {
            Button button = new Button
            {
                Text = text,
                Dock = DockStyle.Fill,
                Height = 38,
                Margin = new Padding(6, 0, 6, 0),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Tahoma", 10F, FontStyle.Bold)
            };

            button.FlatAppearance.BorderSize = 0;

            return button;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            LoadSavedCredentials();

            if (!string.IsNullOrWhiteSpace(txtUsername.Text))
                txtPassword.Focus();
            else
                txtUsername.Focus();
        }

        private void LoadSavedCredentials()
        {
            try
            {
                string savedUsername = Properties.Settings.Default.Username ?? string.Empty;

                if (!string.IsNullOrWhiteSpace(savedUsername))
                {
                    txtUsername.Text = savedUsername;
                    txtPassword.Text = string.Empty;
                    chkRemember.Checked = true;
                }
            }
            catch
            {
                // تجاهل أخطاء قراءة الإعدادات
            }
        }

        private void SaveCredentials()
        {
            if (chkRemember.Checked)
            {
                Properties.Settings.Default.Username = txtUsername.Text.Trim();
                Properties.Settings.Default.Password = string.Empty;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.Username = string.Empty;
                Properties.Settings.Default.Password = string.Empty;
                Properties.Settings.Default.Save();
            }
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (isLockedOut)
                return;

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

            try
            {
                _loggedInUser = await ValidateCredentialsAsync(
                    txtUsername.Text.Trim(),
                    txtPassword.Text);

                if (_loggedInUser != null)
                {
                    CurrentUser.UserID = _loggedInUser.UserID;
                    CurrentUser.UserName = _loggedInUser.UserName;
                    CurrentUser.FullName = _loggedInUser.FullName ?? _loggedInUser.UserName;
                    CurrentUser.RoleID = _loggedInUser.RoleID;
                    CurrentUser.RoleName = _loggedInUser.RoleName ?? string.Empty;
                    CurrentUser.IsLoggedIn = true;
                    CurrentUser.Permissions = _permissionService.GetPermissionsByRole(_loggedInUser.RoleID);

                    ShowStatus("تم تسجيل الدخول بنجاح!", true);
                    SaveCredentials();
                    failedAttempts = 0;

                    AuditLogService audit = new AuditLogService();
                    audit.Log(
                        action: "LOGIN_SUCCESS",
                        tableName: "Users",
                        recordId: _loggedInUser.UserID,
                        details: "تم تسجيل الدخول بنجاح",
                        entityName: _loggedInUser.UserName);

                    await Task.Delay(300);

                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                failedAttempts++;

                AuditLogService auditFailed = new AuditLogService();
                auditFailed.Log(
                    action: "LOGIN_FAILED",
                    tableName: "Users",
                    recordId: null,
                    details: "فشل تسجيل الدخول لاسم المستخدم: " + txtUsername.Text.Trim(),
                    entityName: txtUsername.Text.Trim());

                int remaining = MAX_ATTEMPTS - failedAttempts;

                if (failedAttempts >= MAX_ATTEMPTS)
                {
                    StartLockout();
                }
                else
                {
                    lblAttempts.Text = "المحاولات المتبقية: " + remaining;
                    ShowStatus("بيانات الدخول غير صحيحة. المحاولات المتبقية: " + remaining, false);
                }
            }
            catch (Exception ex)
            {
                ShowStatus("حدث خطأ أثناء تسجيل الدخول: " + ex.Message, false);
            }
            finally
            {
                if (DialogResult != DialogResult.OK && !isLockedOut)
                {
                    SetControlsEnabled(true);
                    btnLogin.Text = "تسجيل الدخول";
                }
            }
        }

        private void StartLockout()
        {
            isLockedOut = true;
            ShowStatus("تم تجاوز الحد الأقصى للمحاولات. حاول بعد 5 دقائق.", false);

            txtUsername.Enabled = false;
            txtPassword.Enabled = false;
            chkRemember.Enabled = false;
            chkShowPassword.Enabled = false;
            btnLogin.Enabled = false;
            btnExit.Enabled = true;

            lblAttempts.Text = "تم إيقاف تسجيل الدخول مؤقتًا لمدة 5 دقائق.";

            Timer enableTimer = new Timer
            {
                Interval = 300000
            };

            enableTimer.Tick += (s, ev) =>
            {
                failedAttempts = 0;
                isLockedOut = false;

                SetControlsEnabled(true);
                btnLogin.Text = "تسجيل الدخول";
                lblAttempts.Text = "عدد المحاولات المسموح بها: 3";

                enableTimer.Stop();
                enableTimer.Dispose();
            };

            enableTimer.Start();
        }

        private async Task<AppUser> ValidateCredentialsAsync(string username, string password)
        {
            return await Task.Run(() =>
            {
                try
                {
                    return _authService.ValidateUser(username, password);
                }
                catch
                {
                    return null;
                }
            });
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();

                foreach (byte b in bytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }

        private void ShowStatus(string message, bool isSuccess)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = isSuccess ? successColor : errorColor;
            lblStatus.Visible = true;

            Timer hideTimer = new Timer
            {
                Interval = isSuccess ? 2500 : 5000
            };

            hideTimer.Tick += (s, e) =>
            {
                if (!isLockedOut)
                    lblStatus.Visible = false;

                hideTimer.Stop();
                hideTimer.Dispose();
            };

            hideTimer.Start();
        }

        private void SetControlsEnabled(bool enabled)
        {
            txtUsername.Enabled = enabled;
            txtPassword.Enabled = enabled;
            chkRemember.Enabled = enabled;
            chkShowPassword.Enabled = enabled;
            btnLogin.Enabled = enabled;
            btnExit.Enabled = true;
        }

        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '●';
        }

        private void LoginTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && btnLogin.Enabled)
            {
                e.SuppressKeyPress = true;
                btnLogin.PerformClick();
            }
        }
    }
}

/*using System;
using System.Data;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using water3.Models;
using water3.Services;
namespace water3
{
    public partial class LoginForm : Form
    {
        private Color primaryColor = Color.FromArgb(52, 152, 219);
        private Color successColor = Color.FromArgb(46, 204, 113);
        private Color errorColor = Color.FromArgb(231, 76, 60);
        private readonly UserAuthService _authService = new UserAuthService();
        private AppUser _loggedInUser;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private CheckBox chkRemember;
        private Button btnLogin;
        private Button btnExit;
        private Label lblStatus;

        private int failedAttempts = 0;
        private const int MAX_ATTEMPTS = 3;
        private readonly PermissionService _permissionService = new PermissionService();
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
        // استبدل دالتي LoadSavedCredentials و SaveCredentials في LoginForm.cs بهذه النسخة.
        // الهدف: عدم حفظ كلمة المرور في إعدادات التطبيق.

        private void LoadSavedCredentials()
        {
            try
            {
                string savedUsername = Properties.Settings.Default.Username ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(savedUsername))
                {
                    txtUsername.Text = savedUsername;
                    txtPassword.Text = string.Empty;
                    chkRemember.Checked = true;
                }
            }
            catch
            {
                // تجاهل أخطاء قراءة الإعدادات
            }
        }

        private void SaveCredentials()
        {
            if (chkRemember.Checked)
            {
                Properties.Settings.Default.Username = txtUsername.Text.Trim();
                Properties.Settings.Default.Password = string.Empty; // لا تحفظ كلمة المرور في الإنتاج
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.Username = string.Empty;
                Properties.Settings.Default.Password = string.Empty;
                Properties.Settings.Default.Save();
            }
        }

        //private void LoadSavedCredentials()
        //{
        //    try
        //    {
        //        string savedUsername = Properties.Settings.Default.Username ?? "";
        //        string savedPassword = Properties.Settings.Default.Password ?? "";
        //        if (!string.IsNullOrEmpty(savedUsername))
        //        {
        //            txtUsername.Text = savedUsername;
        //            txtPassword.Text = savedPassword;
        //            chkRemember.Checked = true;
        //        }
        //    }
        //    catch { }
        //}

        //private void SaveCredentials()
        //{
        //    if (chkRemember.Checked)
        //    {
        //        Properties.Settings.Default.Username = txtUsername.Text;
        //        Properties.Settings.Default.Password = txtPassword.Text;
        //        Properties.Settings.Default.Save();
        //    }
        //    else
        //    {
        //        Properties.Settings.Default.Username = "";
        //        Properties.Settings.Default.Password = "";
        //        Properties.Settings.Default.Save();
        //    }
        //}
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

            try
            {
                _loggedInUser = await ValidateCredentialsAsync(txtUsername.Text, txtPassword.Text);

                if (_loggedInUser != null)
                {
                    CurrentUser.UserID = _loggedInUser.UserID;
                    CurrentUser.UserName = _loggedInUser.UserName;
                    CurrentUser.FullName = _loggedInUser.FullName ?? _loggedInUser.UserName;
                    CurrentUser.RoleID = _loggedInUser.RoleID;
                    CurrentUser.RoleName = _loggedInUser.RoleName ?? string.Empty;
                    CurrentUser.IsLoggedIn = true;
                    CurrentUser.Permissions = _permissionService.GetPermissionsByRole(_loggedInUser.RoleID);

                    ShowStatus("تم تسجيل الدخول بنجاح!", true);
                    SaveCredentials();
                    failedAttempts = 0;

                    var audit = new AuditLogService();
                    audit.Log(
                        action: "LOGIN_SUCCESS",
                        tableName: "Users",
                        recordId: _loggedInUser.UserID,
                        details: "تم تسجيل الدخول بنجاح",
                        entityName: _loggedInUser.UserName);

                    await System.Threading.Tasks.Task.Delay(300);

                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                failedAttempts++;

                var auditFailed = new AuditLogService();
                auditFailed.Log(
                    action: "LOGIN_FAILED",
                    tableName: "Users",
                    recordId: null,
                    details: $"فشل تسجيل الدخول لاسم المستخدم: {txtUsername.Text}",
                    entityName: txtUsername.Text);

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
                        enableTimer.Dispose();
                    };
                    enableTimer.Start();
                }
                else
                {
                    ShowStatus($"بيانات الدخول غير صحيحة. المحاولات المتبقية: {MAX_ATTEMPTS - failedAttempts}", false);
                }
            }
            catch (Exception ex)
            {
                ShowStatus("حدث خطأ أثناء تسجيل الدخول: " + ex.Message, false);
            }
            finally
            {
                if (DialogResult != DialogResult.OK)
                {
                    SetControlsEnabled(true);
                    btnLogin.Text = "تسجيل الدخول";
                }
            }
        }
  
        private async System.Threading.Tasks.Task<AppUser> ValidateCredentialsAsync(string username, string password)
        {
            return await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    return _authService.ValidateUser(username, password);
                }
                catch
                {
                    return null;
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
*/