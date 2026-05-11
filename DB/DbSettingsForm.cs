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
        private Button btnCreateDatabase;

        public DbSettingsForm()
        {
            InitializeComponent();

            AddCreateDatabaseButton();

            btnScanServers.Click += (s, e) => ScanServers();
            btnLoadDbs.Click += (s, e) => LoadDatabases();
            btnTest.Click += (s, e) => Test();
            btnSave.Click += (s, e) => Save();
            btnRestore.Click += (s, e) => RestoreFromBackup();
            btnCreateDatabase.Click += (s, e) => CreateAndPrepareDatabase();
            btnClose.Click += (s, e) => Close();

            rbWindows.CheckedChanged += (s, e) => ToggleAuth();
            rbSql.CheckedChanged += (s, e) => ToggleAuth();

            LoadFromCurrentConfig();
            ToggleAuth();

            if (string.IsNullOrWhiteSpace(cboServer.Text))
                cboServer.Text = @".\SQLEXPRESS";

            if (string.IsNullOrWhiteSpace(cboDb.Text))
                cboDb.Text = "WaterBillingDB";
        }

        private void AddCreateDatabaseButton()
        {
            btnCreateDatabase = new Button
            {
                Text = "إنشاء وتجهيز قاعدة جديدة",
                Width = 210,
                Height = 34,
                BackColor = Color.FromArgb(34, 139, 230),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            pnlInCardBtns.Controls.Add(btnCreateDatabase);
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
            lblStatus.Refresh();
        }

        private void SetBusy(bool busy)
        {
            btnScanServers.Enabled = !busy;
            btnLoadDbs.Enabled = !busy;
            btnTest.Enabled = !busy;
            btnSave.Enabled = !busy;
            btnRestore.Enabled = !busy;
            btnCreateDatabase.Enabled = !busy;
            btnClose.Enabled = !busy;
            Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        }

        private void LoadFromCurrentConfig()
        {
            try
            {
                var csb = new SqlConnectionStringBuilder(water3.Db.ConnectionString);

                cboServer.Text = csb.DataSource;
                cboDb.Text = csb.InitialCatalog;

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
            }
            catch
            {
                // أول تشغيل: لا توجد إعدادات بعد
            }
        }

        private string BuildConn(string db)
        {
            return water3.Db.BuildConnectionString(
                (cboServer.Text ?? ".").Trim(),
                db,
                rbWindows.Checked,
                (txtUser.Text ?? "").Trim(),
                txtPass.Text ?? "",
                10
            );
        }

        private string BuildMasterConn()
        {
            return water3.Db.BuildMasterConnectionString(
                (cboServer.Text ?? ".").Trim(),
                rbWindows.Checked,
                (txtUser.Text ?? "").Trim(),
                txtPass.Text ?? ""
            );
        }

        private void ScanServers()
        {
            try
            {
                SetBusy(true);
                SetStatus("جارٍ البحث عن السيرفرات...");

                cboServer.Items.Clear();

                DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources();

                foreach (DataRow r in dt.Rows)
                {
                    string server = (r["ServerName"] + "").Trim();
                    string inst = (r["InstanceName"] + "").Trim();

                    string full = string.IsNullOrEmpty(inst)
                        ? server
                        : server + "\\" + inst;

                    if (!string.IsNullOrWhiteSpace(full) && !cboServer.Items.Contains(full))
                        cboServer.Items.Add(full);
                }

                AddCommonServers();

                if (cboServer.Items.Count > 0 && string.IsNullOrWhiteSpace(cboServer.Text))
                    cboServer.SelectedIndex = 0;

                SetStatus("تم العثور على " + cboServer.Items.Count + " سيرفر/Instance.");
            }
            catch (Exception ex)
            {
                SetStatus("فشل البحث: " + ex.Message, true);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void AddCommonServers()
        {
            string[] defaults =
            {
                @".\SQLEXPRESS",
                ".",
                "localhost",
                @"localhost\SQLEXPRESS",
                Environment.MachineName,
                Environment.MachineName + @"\SQLEXPRESS"
            };

            for (int i = defaults.Length - 1; i >= 0; i--)
            {
                if (!cboServer.Items.Contains(defaults[i]))
                    cboServer.Items.Insert(0, defaults[i]);
            }
        }

        private void LoadDatabases()
        {
            try
            {
                SetBusy(true);
                SetStatus("جارٍ تحميل قواعد البيانات...");

                string cs = BuildConn("master");

                using (var con = new SqlConnection(cs))
                using (var cmd = new SqlCommand(@"
SELECT name
FROM sys.databases
WHERE database_id > 4
ORDER BY name;", con))
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
            finally
            {
                SetBusy(false);
            }
        }

        private void Test()
        {
            try
            {
                string db = (cboDb.Text ?? "").Trim();

                if (string.IsNullOrWhiteSpace(db))
                    db = "master";

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
                    MessageBox.Show("اكتب أو اختر اسم قاعدة البيانات أولاً.");
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

        private void CreateAndPrepareDatabase()
        {
            try
            {
                string dbName = (cboDb.Text ?? "").Trim();

                if (string.IsNullOrWhiteSpace(cboServer.Text))
                {
                    MessageBox.Show("اكتب اسم السيرفر أولاً.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(dbName))
                {
                    MessageBox.Show("اكتب اسم قاعدة البيانات أولاً.");
                    return;
                }

                string adminUser = Prompt("اسم مستخدم المدير:", "بيانات المدير", "admin");
                if (adminUser == null) return;

                string adminFullName = Prompt("الاسم الكامل للمدير:", "بيانات المدير", "مدير النظام");
                if (adminFullName == null) return;

                string adminPassword = PromptPassword("كلمة مرور المدير:", "بيانات المدير", "123");
                if (adminPassword == null) return;

                string unitPriceText = Prompt("سعر وحدة المياه:", "التعرفة الأساسية", "0");
                if (unitPriceText == null) return;

                string serviceFeesText = Prompt("رسوم الخدمة:", "التعرفة الأساسية", "0");
                if (serviceFeesText == null) return;

                decimal unitPrice;
                decimal serviceFees;

                if (!decimal.TryParse(unitPriceText, out unitPrice))
                    throw new Exception("سعر وحدة المياه غير صحيح.");

                if (!decimal.TryParse(serviceFeesText, out serviceFees))
                    throw new Exception("رسوم الخدمة غير صحيحة.");

                string scriptPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "DB",
                    "Scripts",
                    "01_Schema.sql"
                );

                if (!File.Exists(scriptPath))
                {
                    MessageBox.Show(
                        "لم يتم العثور على سكربت إنشاء قاعدة البيانات:\n" + scriptPath +
                        "\n\nضع ملف 01_Schema.sql داخل DB\\Scripts واجعل Copy to Output Directory = Copy if newer.",
                        "ملف السكربت غير موجود",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return;
                }

                DialogResult confirm = MessageBox.Show(
                    "سيتم إنشاء قاعدة البيانات وتجهيز الجداول والبيانات الأساسية.\n\n" +
                    "السيرفر: " + cboServer.Text + "\n" +
                    "قاعدة البيانات: " + dbName + "\n\n" +
                    "هل تريد المتابعة؟",
                    "تأكيد إنشاء القاعدة",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirm != DialogResult.Yes)
                    return;

                SetBusy(true);

                string masterCs = BuildMasterConn();
                string appCs = BuildConn(dbName);

                SetStatus("جاري اختبار الاتصال بالسيرفر...");
                water3.Db.TestConnection(masterCs);

                SetStatus("جاري إنشاء قاعدة البيانات إن لم تكن موجودة...");
                water3.Db.CreateDatabaseIfNotExists(masterCs, dbName);

                SetStatus("جاري اختبار الاتصال بقاعدة البيانات...");
                water3.Db.TestConnection(appCs);

                if (!water3.Db.IsSchemaInstalled(appCs))
                {
                    SetStatus("جاري إنشاء الجداول والإجراءات...");
                    water3.Db.InstallSchemaFromFile(appCs, dbName, scriptPath);
                }
                else
                {
                    DialogResult schemaExists = MessageBox.Show(
                        "يبدو أن الجداول موجودة مسبقًا في هذه القاعدة.\n\n" +
                        "سيتم تجاوز إنشاء الجداول وتنفيذ البيانات الأساسية فقط.",
                        "تنبيه",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning
                    );

                    if (schemaExists != DialogResult.OK)
                        return;
                }

                SetStatus("جاري إدخال البيانات الأساسية...");
                water3.Db.SeedBasicData(
                    appCs,
                    adminUser,
                    adminFullName,
                    adminPassword,
                    unitPrice,
                    serviceFees
                );
                string sqlLogin = "water_api";
                string sqlPassword = "a123";

                water3.Db.CreateSqlLoginAndUser(
                    masterCs,
                    dbName,
                    sqlLogin,
                    sqlPassword
                );

                string finalAppCs = water3.Db.BuildConnectionString(
                    (cboServer.Text ?? ".").Trim(),
                    dbName,
                    false,
                    sqlLogin,
                    sqlPassword,
                    10
                );

                water3.Db.TestConnection(finalAppCs);
                water3.Db.SaveConnectionString(finalAppCs);
                //water3.Db.SaveConnectionString(appCs);

                cboDb.Text = dbName;

                SetStatus("✅ تم إنشاء وتجهيز قاعدة البيانات بنجاح.");

                MessageBox.Show(
                    "✅ تم تجهيز قاعدة البيانات بنجاح.\n\n" +
                    "بيانات الدخول:\n" +
                    "اسم المستخدم: " + adminUser + "\n" +
                    "كلمة المرور: " + adminPassword,
                    "تم",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                SetStatus("❌ فشل إنشاء القاعدة: " + ex.Message, true);

                MessageBox.Show(
                    "❌ فشل إنشاء وتجهيز قاعدة البيانات:\n\n" + ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void RestoreFromBackup()
        {
            try
            {
                string server = (cboServer.Text ?? "").Trim();

                if (string.IsNullOrWhiteSpace(server))
                {
                    MessageBox.Show("اكتب أو اختر اسم السيرفر أولاً.");
                    return;
                }

                using (var ofd = new OpenFileDialog())
                {
                    ofd.Filter = "SQL Backup (*.bak)|*.bak|All Files (*.*)|*.*";
                    ofd.Title = "اختر ملف النسخة الاحتياطية (.bak)";

                    if (ofd.ShowDialog() != DialogResult.OK)
                        return;

                    string bakPath = ofd.FileName;

                    if (!File.Exists(bakPath))
                    {
                        MessageBox.Show("ملف النسخة غير موجود.");
                        return;
                    }

                    string newDbName = Prompt("اسم قاعدة البيانات بعد الاستعادة:", "Restore Database", "WaterBillingDB");

                    if (newDbName == null)
                        return;

                    newDbName = newDbName.Trim();

                    if (string.IsNullOrWhiteSpace(newDbName))
                    {
                        MessageBox.Show("اسم قاعدة البيانات مطلوب.");
                        return;
                    }

                    SetBusy(true);
                    SetStatus("جارٍ الاستعادة...");

                    string masterCs = BuildConn("master");

                    using (var con = new SqlConnection(masterCs))
                    {
                        con.Open();

                        string logicalData = null;
                        string logicalLog = null;

                        using (var cmd = new SqlCommand("RESTORE FILELISTONLY FROM DISK = @bak;", con))
                        {
                            cmd.Parameters.AddWithValue("@bak", bakPath);

                            using (var r = cmd.ExecuteReader())
                            {
                                while (r.Read())
                                {
                                    string type = (r["Type"] + "").Trim();
                                    string name = (r["LogicalName"] + "").Trim();

                                    if (type == "D" && logicalData == null)
                                        logicalData = name;

                                    if (type == "L" && logicalLog == null)
                                        logicalLog = name;
                                }
                            }
                        }

                        if (string.IsNullOrWhiteSpace(logicalData) || string.IsNullOrWhiteSpace(logicalLog))
                            throw new Exception("تعذر قراءة محتويات النسخة الاحتياطية.");

                        string dataPath;
                        string logPath;

                        GetSqlDefaultPaths(con, out dataPath, out logPath);

                        if (string.IsNullOrWhiteSpace(dataPath) || string.IsNullOrWhiteSpace(logPath))
                            throw new Exception("تعذر تحديد مسارات حفظ ملفات قاعدة البيانات في السيرفر.");

                        string mdf = Path.Combine(dataPath, newDbName + ".mdf");
                        string ldf = Path.Combine(logPath, newDbName + "_log.ldf");

                        string qDb = QuoteName(newDbName);

                        using (var cmd = new SqlCommand(@"
IF DB_ID(@db) IS NOT NULL
BEGIN
    DECLARE @sql nvarchar(max);
    SET @sql = N'ALTER DATABASE ' + QUOTENAME(@db) + N' SET SINGLE_USER WITH ROLLBACK IMMEDIATE';
    EXEC(@sql);
END", con))
                        {
                            cmd.Parameters.AddWithValue("@db", newDbName);
                            cmd.ExecuteNonQuery();
                        }

                        using (var cmd = new SqlCommand(@"
RESTORE DATABASE " + qDb + @"
FROM DISK = @bak
WITH REPLACE,
     MOVE @logicalData TO @mdf,
     MOVE @logicalLog TO @ldf,
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

                        using (var cmd = new SqlCommand("ALTER DATABASE " + qDb + " SET MULTI_USER;", con))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }

                    string appCs = BuildConn(newDbName);
                    water3.Db.TestConnection(appCs);
                    water3.Db.SaveConnectionString(appCs);

                    LoadDatabases();
                    cboDb.Text = newDbName;

                    SetStatus("✅ تمت الاستعادة وحفظ الاتصال بنجاح.");

                    MessageBox.Show(
                        "✅ تمت الاستعادة وحفظ الاتصال بنجاح.",
                        "تم",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            catch (Exception ex)
            {
                SetStatus("❌ فشل الاستعادة: " + ex.Message, true);

                MessageBox.Show(
                    "❌ فشل الاستعادة:\n" + ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                SetBusy(false);
            }
        }

        private static void GetSqlDefaultPaths(SqlConnection con, out string dataPath, out string logPath)
        {
            dataPath = null;
            logPath = null;

            using (var cmd = new SqlCommand(@"
SELECT
  CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS nvarchar(4000)) AS DataPath,
  CAST(SERVERPROPERTY('InstanceDefaultLogPath') AS nvarchar(4000)) AS LogPath;", con))
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

            if (string.IsNullOrWhiteSpace(dataPath))
            {
                using (var cmd = new SqlCommand(@"
SELECT SUBSTRING(physical_name, 1, LEN(physical_name) - CHARINDEX('\', REVERSE(physical_name)) + 1)
FROM master.sys.master_files
WHERE database_id = 1 AND file_id = 1;", con))
                {
                    dataPath = (cmd.ExecuteScalar() + "").Trim();
                }
            }

            if (string.IsNullOrWhiteSpace(logPath))
            {
                using (var cmd = new SqlCommand(@"
SELECT SUBSTRING(physical_name, 1, LEN(physical_name) - CHARINDEX('\', REVERSE(physical_name)) + 1)
FROM master.sys.master_files
WHERE database_id = 1 AND file_id = 2;", con))
                {
                    logPath = (cmd.ExecuteScalar() + "").Trim();
                }
            }
        }

        private static string QuoteName(string name)
        {
            return "[" + (name ?? "").Replace("]", "]]") + "]";
        }

        private static string Prompt(string text, string caption, string defaultValue)
        {
            return PromptInternal(text, caption, defaultValue, false);
        }

        private static string PromptPassword(string text, string caption, string defaultValue)
        {
            return PromptInternal(text, caption, defaultValue, true);
        }

        private static string PromptInternal(string text, string caption, string defaultValue, bool password)
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

                if (password)
                    txt.UseSystemPasswordChar = true;

                var panel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Bottom,
                    FlowDirection = FlowDirection.RightToLeft,
                    Height = 55,
                    Padding = new Padding(8)
                };

                var ok = new Button
                {
                    Text = "موافق",
                    DialogResult = DialogResult.OK,
                    Width = 100,
                    Height = 34
                };

                var cancel = new Button
                {
                    Text = "إلغاء",
                    DialogResult = DialogResult.Cancel,
                    Width = 100,
                    Height = 34
                };

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


*/