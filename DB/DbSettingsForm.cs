using System;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace water3.DB
{
    public partial class DbSettingsForm : Form
    {
        public DbSettingsForm()
        {
            InitializeComponent();

            // أحداث
            btnScanServers.Click += (s, e) => ScanServers();
            btnLoadDbs.Click += (s, e) => LoadDatabases();
            btnTest.Click += (s, e) => Test();
            btnSave.Click += (s, e) => Save();
            btnRestore.Click += (s, e) => RestoreFromBackup();
            btnClose.Click += (s, e) => Close();

            rbWindows.CheckedChanged += (s, e) => ToggleAuth();
            rbSql.CheckedChanged += (s, e) => ToggleAuth();

            // تهيئة
            LoadFromCurrentConfig();
            ToggleAuth();
        }

        private void ToggleAuth()
        {
            bool sql = rbSql.Checked;
            txtUser.Enabled = sql;
            txtPass.Enabled = sql;
        }

        private void SetStatus(string msg, bool error = false)
        {
            lblStatus.Text = msg;
            lblStatus.ForeColor = error ? Color.Firebrick : Color.DimGray;
        }

        private void LoadFromCurrentConfig()
        {
            try
            {
                var csb = new SqlConnectionStringBuilder(water3.Db.ConnectionString);
                cboServer.Text = csb.DataSource;

                if (csb.IntegratedSecurity)
                {
                    rbWindows.Checked = true;
                }
                else
                {
                    rbSql.Checked = true;
                    txtUser.Text = csb.UserID;
                    txtPass.Text = csb.Password;
                }

                if (!string.IsNullOrWhiteSpace(csb.InitialCatalog))
                    cboDb.Text = csb.InitialCatalog;
            }
            catch { }
        }

        private string BuildConn(string db)
        {
            var csb = new SqlConnectionStringBuilder
            {
                DataSource = (cboServer.Text ?? ".").Trim(),
                InitialCatalog = db ?? "",
                IntegratedSecurity = rbWindows.Checked,
                ConnectTimeout = 5,
                TrustServerCertificate = true
            };

            if (rbSql.Checked)
            {
                csb.IntegratedSecurity = false;
                csb.UserID = (txtUser.Text ?? "").Trim();
                csb.Password = txtPass.Text ?? "";
            }

            return csb.ConnectionString;
        }

        private void ScanServers()
        {
            try
            {
                SetStatus("جارٍ البحث عن السيرفرات...");
                btnScanServers.Enabled = false;
                cboServer.Items.Clear();

                DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources();
                foreach (DataRow r in dt.Rows)
                {
                    string server = (r["ServerName"] + "").Trim();
                    string inst = (r["InstanceName"] + "").Trim();

                    string full = string.IsNullOrEmpty(inst) ? server : server + "\\" + inst;
                    if (!string.IsNullOrWhiteSpace(full))
                        cboServer.Items.Add(full);
                }

                if (!cboServer.Items.Contains(".")) cboServer.Items.Insert(0, ".");
                if (!cboServer.Items.Contains("localhost")) cboServer.Items.Insert(0, "localhost");

                if (cboServer.Items.Count > 0 && string.IsNullOrWhiteSpace(cboServer.Text))
                    cboServer.SelectedIndex = 0;

                SetStatus($"تم العثور على {cboServer.Items.Count} سيرفر/Instance.");
            }
            catch (Exception ex)
            {
                SetStatus("فشل البحث: " + ex.Message, true);
            }
            finally
            {
                btnScanServers.Enabled = true;
            }
        }

        private void LoadDatabases()
        {
            try
            {
                SetStatus("جارٍ تحميل قواعد البيانات...");

                string cs = BuildConn("master");
                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand("SELECT name FROM sys.databases WHERE database_id > 4 ORDER BY name;", con))
                {
                    con.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        cboDb.Items.Clear();
                        while (r.Read())
                            cboDb.Items.Add(r.GetString(0));
                    }
                }

                if (cboDb.Items.Count > 0 && string.IsNullOrWhiteSpace(cboDb.Text))
                    cboDb.SelectedIndex = 0;

                SetStatus("تم تحميل قواعد البيانات.");
            }
            catch (Exception ex)
            {
                SetStatus("تعذر تحميل القواعد: " + ex.Message, true);
            }
        }

        private void Test()
        {
            try
            {
                string db = (cboDb.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(db)) db = "master";

                string cs = BuildConn(db);
                water3.Db.TestConnection(cs);

                SetStatus("✅ الاتصال ناجح.");
                MessageBox.Show("✅ الاتصال ناجح.", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                SetStatus("❌ فشل الاتصال: " + ex.Message, true);
                MessageBox.Show("❌ فشل الاتصال:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Save()
        {
            try
            {
                string db = (cboDb.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(db))
                {
                    MessageBox.Show("اكتب/اختر قاعدة البيانات أولاً (أو اضغط تحميل القواعد).");
                    return;
                }

                string cs = BuildConn(db);
                water3.Db.TestConnection(cs);
                water3.Db.SaveConnectionString(cs);

                SetStatus("✅ تم حفظ الإعدادات.");
                MessageBox.Show("✅ تم حفظ الإعدادات بنجاح.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                SetStatus("تعذر الحفظ: " + ex.Message, true);
                MessageBox.Show("تعذر الحفظ:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RestoreFromBackup()
        {
            try
            {
                var server = (cboServer.Text ?? "").Trim();
                if (string.IsNullOrWhiteSpace(server))
                {
                    MessageBox.Show("اكتب/اختر اسم السيرفر أولاً.");
                    return;
                }

                using (var ofd = new OpenFileDialog())
                {
                    ofd.Filter = "SQL Backup (*.bak)|*.bak|All Files (*.*)|*.*";
                    ofd.Title = "اختر ملف النسخة الاحتياطية (.bak)";
                    if (ofd.ShowDialog() != DialogResult.OK) return;

                    var bakPath = ofd.FileName;
                    if (!File.Exists(bakPath))
                    {
                        MessageBox.Show("ملف النسخة غير موجود.");
                        return;
                    }

                    var newDbName = Prompt("اسم قاعدة البيانات بعد الاستعادة:", "Restore Database", "WaterBillingDB");
                    newDbName = (newDbName ?? "").Trim();

                    if (string.IsNullOrWhiteSpace(newDbName))
                    {
                        MessageBox.Show("اسم قاعدة البيانات مطلوب.");
                        return;
                    }

                    string masterCs = BuildConn("master");
                    SetStatus("جارٍ الاستعادة... قد تستغرق وقتاً");

                    using (var con = new SqlConnection(masterCs))
                    {
                        con.Open();

                        string logicalData = null, logicalLog = null;
                        using (var cmd = new SqlCommand("RESTORE FILELISTONLY FROM DISK = @bak;", con))
                        {
                            cmd.Parameters.AddWithValue("@bak", bakPath);
                            using (var r = cmd.ExecuteReader())
                            {
                                while (r.Read())
                                {
                                    var type = (r["Type"] + "").Trim();
                                    var name = (r["LogicalName"] + "").Trim();

                                    if (type == "D" && logicalData == null) logicalData = name;
                                    if (type == "L" && logicalLog == null) logicalLog = name;
                                }
                            }
                        }

                        if (string.IsNullOrWhiteSpace(logicalData) || string.IsNullOrWhiteSpace(logicalLog))
                            throw new Exception("تعذر قراءة محتويات النسخة الاحتياطية (Logical names).");

                        string dataPath = null, logPath = null;
                        using (var cmd = new SqlCommand(@"
SELECT
  CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS nvarchar(4000)) AS DataPath,
  CAST(SERVERPROPERTY('InstanceDefaultLogPath')  AS nvarchar(4000)) AS LogPath;", con))
                        {
                            using (var r = cmd.ExecuteReader())
                            {
                                if (r.Read())
                                {
                                    dataPath = (r["DataPath"] + "").Trim();
                                    logPath = (r["LogPath"] + "").Trim();
                                }
                            }
                        }

                        if (string.IsNullOrWhiteSpace(dataPath) || string.IsNullOrWhiteSpace(logPath))
                        {
                            using (var cmd = new SqlCommand(@"
SELECT SUBSTRING(physical_name, 1, LEN(physical_name) - CHARINDEX('\', REVERSE(physical_name)) + 1)
FROM master.sys.master_files
WHERE database_id = 1 AND file_id = 1;", con))
                                dataPath = (cmd.ExecuteScalar() + "").Trim();

                            using (var cmd = new SqlCommand(@"
SELECT SUBSTRING(physical_name, 1, LEN(physical_name) - CHARINDEX('\', REVERSE(physical_name)) + 1)
FROM master.sys.master_files
WHERE database_id = 1 AND file_id = 2;", con))
                                logPath = (cmd.ExecuteScalar() + "").Trim();
                        }

                        if (string.IsNullOrWhiteSpace(dataPath) || string.IsNullOrWhiteSpace(logPath))
                            throw new Exception("تعذر تحديد مسارات حفظ ملفات قاعدة البيانات في السيرفر.");

                        var mdf = Path.Combine(dataPath, newDbName + ".mdf");
                        var ldf = Path.Combine(logPath, newDbName + "_log.ldf");

                        var qDb = QuoteName(newDbName);

                        using (var cmd = new SqlCommand($@"
IF DB_ID(N'{EscapeSqlLiteral(newDbName)}') IS NOT NULL
BEGIN
    ALTER DATABASE {qDb} SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
END", con))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new SqlCommand($@"
RESTORE DATABASE {qDb}
FROM DISK = @bak
WITH REPLACE,
     MOVE @logicalData TO @mdf,
     MOVE @logicalLog  TO @ldf,
     RECOVERY;", con))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.AddWithValue("@bak", bakPath);
                            cmd.Parameters.AddWithValue("@logicalData", logicalData);
                            cmd.Parameters.AddWithValue("@logicalLog", logicalLog);
                            cmd.Parameters.AddWithValue("@mdf", mdf);
                            cmd.Parameters.AddWithValue("@ldf", ldf);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new SqlCommand($"ALTER DATABASE {qDb} SET MULTI_USER;", con))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }

                    LoadDatabases();
                    cboDb.Text = newDbName;

                    SetStatus("✅ تمت الاستعادة بنجاح.");
                    MessageBox.Show("✅ تمت الاستعادة بنجاح.\nالآن اختر القاعدة واحفظ الاتصال.",
                        "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                SetStatus("❌ فشل الاستعادة: " + ex.Message, true);
                MessageBox.Show("❌ فشل الاستعادة:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static string QuoteName(string name) => "[" + (name ?? "").Replace("]", "]]") + "]";
        private static string EscapeSqlLiteral(string s) => (s ?? "").Replace("'", "''");

        private static string Prompt(string text, string caption, string defaultValue = "")
        {
            using (var f = new Form())
            {
                f.Text = caption;
                f.StartPosition = FormStartPosition.CenterParent;
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.MaximizeBox = false;
                f.MinimizeBox = false;
                f.Width = 500;
                f.Height = 180;
                f.RightToLeft = RightToLeft.Yes;
                f.RightToLeftLayout = true;

                var lbl = new Label
                {
                    Text = text,
                    Dock = DockStyle.Top,
                    Height = 45,
                    TextAlign = ContentAlignment.MiddleRight
                };

                var txt = new TextBox
                {
                    Dock = DockStyle.Top,
                    Text = defaultValue
                };

                var panel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    FlowDirection = FlowDirection.RightToLeft,
                    Height = 55,
                    Padding = new Padding(8)
                };

                var ok = new Button { Text = "موافق", DialogResult = DialogResult.OK, Width = 100, Height = 34 };
                var cancel = new Button { Text = "إلغاء", DialogResult = DialogResult.Cancel, Width = 100, Height = 34 };

                panel.Controls.Add(ok);
                panel.Controls.Add(cancel);

                f.Controls.Add(panel);
                f.Controls.Add(txt);
                f.Controls.Add(lbl);

                f.AcceptButton = ok;
                f.CancelButton = cancel;

                return f.ShowDialog() == DialogResult.OK ? txt.Text : null;
            }
        }
    }
}



/*using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data.SqlClient;

namespace water3.DB
{
    public partial class DbSettingsForm : Form
    {
       
    //public class DbSettingsForm : Form
    //{
        TextBox txtServer, txtUser, txtPass;
        ComboBox cboDb;
        RadioButton rbWindows, rbSql;
        Button btnLoadDbs, btnTest, btnSave;

        public DbSettingsForm()
        {
            Text = "ربط النظام بقاعدة البيانات";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            Width = 520;
            Height = 320;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            BuildUI();
            LoadFromCurrentConfig();
            ToggleAuth();
        }

        void BuildUI()
        {
            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
                ColumnCount = 2,
                RowCount = 7
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

            for (int i = 0; i < 7; i++)
                grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 38));

            // Server
            grid.Controls.Add(new Label { Text = "SQL Server", Anchor = AnchorStyles.Right, AutoSize = true }, 0, 0);
            txtServer = new TextBox { Dock = DockStyle.Fill, Text = "." };
            grid.Controls.Add(txtServer, 1, 0);

            // Auth
            grid.Controls.Add(new Label { Text = "طريقة الدخول", Anchor = AnchorStyles.Right, AutoSize = true }, 0, 1);
            var authPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft, WrapContents = false };
            rbWindows = new RadioButton { Text = "Windows", AutoSize = true, Checked = true };
            rbSql = new RadioButton { Text = "SQL Login", AutoSize = true };
            rbWindows.CheckedChanged += (s, e) => ToggleAuth();
            rbSql.CheckedChanged += (s, e) => ToggleAuth();
            authPanel.Controls.Add(rbWindows);
            authPanel.Controls.Add(rbSql);
            grid.Controls.Add(authPanel, 1, 1);

            // User
            grid.Controls.Add(new Label { Text = "اسم المستخدم", Anchor = AnchorStyles.Right, AutoSize = true }, 0, 2);
            txtUser = new TextBox { Dock = DockStyle.Fill };
            grid.Controls.Add(txtUser, 1, 2);

            // Pass
            grid.Controls.Add(new Label { Text = "كلمة المرور", Anchor = AnchorStyles.Right, AutoSize = true }, 0, 3);
            txtPass = new TextBox { Dock = DockStyle.Fill, UseSystemPasswordChar = true };
            grid.Controls.Add(txtPass, 1, 3);

            // Database + Load
            grid.Controls.Add(new Label { Text = "قاعدة البيانات", Anchor = AnchorStyles.Right, AutoSize = true }, 0, 4);
            var dbRow = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
            dbRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            dbRow.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            cboDb = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList };
            btnLoadDbs = new Button { Text = "تحميل القواعد", Dock = DockStyle.Fill };
            btnLoadDbs.Click += (s, e) => LoadDatabases();
            dbRow.Controls.Add(cboDb, 0, 0);
            dbRow.Controls.Add(btnLoadDbs, 1, 0);
            grid.Controls.Add(dbRow, 1, 4);

            // Buttons
            btnTest = new Button { Text = "اختبار اتصال", Dock = DockStyle.Fill };
            btnSave = new Button { Text = "حفظ", Dock = DockStyle.Fill };
            btnTest.Click += (s, e) => Test();
            btnSave.Click += (s, e) => Save();

            grid.Controls.Add(btnTest, 0, 5);
            grid.Controls.Add(btnSave, 1, 5);

            var hint = new Label
            {
                Text = "بعد الحفظ سيتم استخدام الإعدادات في كل الواجهات تلقائياً.",
                Dock = DockStyle.Fill,
                AutoSize = true
            };
            grid.Controls.Add(hint, 0, 6);
            grid.SetColumnSpan(hint, 2);

            Controls.Add(grid);
        }

        void ToggleAuth()
        {
            bool sql = rbSql.Checked;
            txtUser.Enabled = sql;
            txtPass.Enabled = sql;
        }

        void LoadFromCurrentConfig()
        {
            // محاولة بسيطة لتعبئة من الإعداد الحالي
            try
            {
                var csb = new SqlConnectionStringBuilder(Db.ConnectionString);
                txtServer.Text = csb.DataSource;
                if (csb.IntegratedSecurity)
                {
                    rbWindows.Checked = true;
                }
                else
                {
                    rbSql.Checked = true;
                    txtUser.Text = csb.UserID;
                    txtPass.Text = csb.Password;
                }

                if (!string.IsNullOrWhiteSpace(csb.InitialCatalog))
                {
                    cboDb.Items.Clear();
                    cboDb.Items.Add(csb.InitialCatalog);
                    cboDb.SelectedIndex = 0;
                }
            }
            catch { }
        }

        string BuildConnectionString(string databaseOrEmpty)
        {
            var csb = new SqlConnectionStringBuilder
            {
                DataSource = (txtServer.Text ?? ".").Trim(),
                InitialCatalog = databaseOrEmpty ?? "",
                IntegratedSecurity = rbWindows.Checked,
                TrustServerCertificate = true,
                ConnectTimeout = 5
            };

            if (rbSql.Checked)
            {
                csb.IntegratedSecurity = false;
                csb.UserID = (txtUser.Text ?? "").Trim();
                csb.Password = (txtPass.Text ?? "");
            }

            return csb.ConnectionString;
        }

        void LoadDatabases()
        {
            try
            {
                // الاتصال بالماستر لجلب قائمة قواعد البيانات
                string cs = BuildConnectionString("master");

                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand(
                    "SELECT name FROM sys.databases WHERE database_id > 4 ORDER BY name;", con))
                {
                    con.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        cboDb.Items.Clear();
                        while (r.Read())
                            cboDb.Items.Add(r.GetString(0));
                    }
                }

                if (cboDb.Items.Count > 0) cboDb.SelectedIndex = 0;
                MessageBox.Show("تم تحميل قواعد البيانات.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("تعذر تحميل قواعد البيانات: " + ex.Message);
            }
        }

        void Test()
        {
            try
            {
                string db = cboDb.SelectedItem?.ToString();
                if (string.IsNullOrWhiteSpace(db)) db = "master";

                string cs = BuildConnectionString(db);
                Db.TestConnection(cs);

                MessageBox.Show("✅ الاتصال ناجح.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("❌ فشل الاتصال: " + ex.Message);
            }
        }

        void Save()
        {
            try
            {
                string db = cboDb.SelectedItem?.ToString();
                if (string.IsNullOrWhiteSpace(db))
                {
                    MessageBox.Show("اختر قاعدة البيانات أولاً (أو اضغط تحميل القواعد).");
                    return;
                }

                string cs = BuildConnectionString(db);
                Db.TestConnection(cs); // تأكد قبل الحفظ
                Db.SaveConnectionString(cs);

                MessageBox.Show("✅ تم حفظ الاتصال بنجاح.");
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("تعذر الحفظ: " + ex.Message);
            }
        }
    }
}
*/