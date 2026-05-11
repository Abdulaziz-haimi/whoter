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
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using water3.Utils;
namespace water3.Forms
{
    public partial class BackupRestoreForm : Form
    {
 

//namespace water3.Forms
//    {
//        public class BackupRestoreForm : Form
//        {
            private Label lblDatabase;
            private Label lblServer;
            private Label lblStatus;
            private TextBox txtBackupFolder;
            private TextBox txtRestoreFile;
            private Button btnBrowseBackupFolder;
            private Button btnOpenBackupFolder;
            private Button btnBackup;
            private Button btnBrowseRestoreFile;
            private Button btnRestore;
            private Button btnTestConnection;
            private ProgressBar progressBar;
            private ListBox lstLog;

            public BackupRestoreForm()
            {
                ProductionUi.PrepareForm(this, "النسخ الاحتياطي والاستعادة");
                BuildUi();
                Load += BackupRestoreForm_Load;
            }

            private void BackupRestoreForm_Load(object sender, EventArgs e)
            {
                try
                {
                    var csb = new SqlConnectionStringBuilder(Db.ConnectionString);
                    lblDatabase.Text = "قاعدة البيانات: " + csb.InitialCatalog;
                    lblServer.Text = "السيرفر: " + csb.DataSource;
                    txtBackupFolder.Text = GetDefaultBackupFolder();
                    AddLog("تم تحميل شاشة النسخ الاحتياطي والاستعادة.");
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "BackupRestoreForm.Load");
                    AddLog("خطأ في قراءة الاتصال: " + ex.Message);
                }
            }

            private void BuildUi()
            {
                var root = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    RowCount = 4,
                    ColumnCount = 1,
                    Padding = new Padding(15),
                    RightToLeft = RightToLeft.Yes
                };

                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 205));
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 205));
                root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                Controls.Add(root);

                root.Controls.Add(CreateHeaderPanel(), 0, 0);
                root.Controls.Add(CreateBackupPanel(), 0, 1);
                root.Controls.Add(CreateRestorePanel(), 0, 2);
                root.Controls.Add(CreateLogPanel(), 0, 3);
            }

            private Control CreateHeaderPanel()
            {
                var card = ProductionUi.CardPanel();

                var title = ProductionUi.Header(
                    "النسخ الاحتياطي والاستعادة",
                    "إنشاء نسخة احتياطية من قاعدة البيانات أو استعادة نسخة سابقة عند الحاجة."
                );
                title.Dock = DockStyle.Top;

                var info = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 3,
                    RightToLeft = RightToLeft.Yes
                };
                info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36));
                info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36));
                info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28));

                lblDatabase = new Label { Text = "قاعدة البيانات: -", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, AutoEllipsis = true, ForeColor = ProductionUi.DarkText };
                lblServer = new Label { Text = "السيرفر: -", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleRight, AutoEllipsis = true, ForeColor = ProductionUi.DarkText };
                btnTestConnection = ProductionUi.Button("فحص الاتصال", ProductionUi.Primary);
                btnTestConnection.Click += async (s, e) => await TestConnectionAsync();

                info.Controls.Add(lblDatabase, 0, 0);
                info.Controls.Add(lblServer, 1, 0);
                info.Controls.Add(btnTestConnection, 2, 0);

                card.Controls.Add(info);
                card.Controls.Add(title);
                return card;
            }

            private Control CreateBackupPanel()
            {
                var card = ProductionUi.CardPanel();
                var header = ProductionUi.Header("إنشاء نسخة احتياطية Backup", "اختر مجلد الحفظ ثم اضغط إنشاء نسخة احتياطية. سيتم إنشاء ملف .bak.");
                header.Dock = DockStyle.Top;

                var layout = new TableLayoutPanel { Dock = DockStyle.Top, Height = 58, ColumnCount = 3, RightToLeft = RightToLeft.Yes };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));

                txtBackupFolder = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(6, 10, 6, 10) };
                btnBrowseBackupFolder = ProductionUi.Button("اختيار مجلد", ProductionUi.Primary);
                btnOpenBackupFolder = ProductionUi.Button("فتح المجلد", ProductionUi.Success);

                btnBrowseBackupFolder.Click += BtnBrowseBackupFolder_Click;
                btnOpenBackupFolder.Click += BtnOpenBackupFolder_Click;

                layout.Controls.Add(txtBackupFolder, 0, 0);
                layout.Controls.Add(btnBrowseBackupFolder, 1, 0);
                layout.Controls.Add(btnOpenBackupFolder, 2, 0);

                btnBackup = ProductionUi.Button("إنشاء نسخة احتياطية الآن", ProductionUi.Success);
                btnBackup.Dock = DockStyle.Top;
                btnBackup.Height = 45;
                btnBackup.Click += async (s, e) => await BackupAsync();

                card.Controls.Add(btnBackup);
                card.Controls.Add(layout);
                card.Controls.Add(header);
                return card;
            }

            private Control CreateRestorePanel()
            {
                var card = ProductionUi.CardPanel();
                var header = ProductionUi.Header("استعادة نسخة احتياطية Restore", "تحذير: الاستعادة ستستبدل بيانات قاعدة البيانات الحالية بالنسخة المختارة.");
                header.Dock = DockStyle.Top;

                var layout = new TableLayoutPanel { Dock = DockStyle.Top, Height = 58, ColumnCount = 2, RightToLeft = RightToLeft.Yes };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));

                txtRestoreFile = new TextBox { Dock = DockStyle.Fill, Margin = new Padding(6, 10, 6, 10) };
                btnBrowseRestoreFile = ProductionUi.Button("اختيار ملف .bak", ProductionUi.Primary);
                btnBrowseRestoreFile.Click += BtnBrowseRestoreFile_Click;

                layout.Controls.Add(txtRestoreFile, 0, 0);
                layout.Controls.Add(btnBrowseRestoreFile, 1, 0);

                btnRestore = ProductionUi.Button("استعادة النسخة المختارة", ProductionUi.Danger);
                btnRestore.Dock = DockStyle.Top;
                btnRestore.Height = 45;
                btnRestore.Click += async (s, e) => await RestoreAsync();

                card.Controls.Add(btnRestore);
                card.Controls.Add(layout);
                card.Controls.Add(header);
                return card;
            }

            private Control CreateLogPanel()
            {
                var card = ProductionUi.CardPanel();
                var title = new Label { Text = "سجل العمليات", Dock = DockStyle.Top, Height = 30, TextAlign = ContentAlignment.MiddleRight, Font = new Font("Tahoma", 10F, FontStyle.Bold) };
                progressBar = new ProgressBar { Dock = DockStyle.Top, Height = 18, Style = ProgressBarStyle.Blocks };
                lblStatus = new Label { Text = "جاهز", Dock = DockStyle.Top, Height = 30, TextAlign = ContentAlignment.MiddleRight, ForeColor = ProductionUi.Success };
                lstLog = new ListBox { Dock = DockStyle.Fill, Font = new Font("Consolas", 9.5F), HorizontalScrollbar = true };

                card.Controls.Add(lstLog);
                card.Controls.Add(lblStatus);
                card.Controls.Add(progressBar);
                card.Controls.Add(title);
                return card;
            }

            private async Task TestConnectionAsync()
            {
                SetBusy(true, "جاري فحص الاتصال...");
                try
                {
                    await Task.Run(() =>
                    {
                        using (var con = new SqlConnection(Db.ConnectionString))
                        {
                            con.Open();
                            AddLog("الاتصال ناجح: " + con.Database + " / " + con.DataSource);
                        }
                    });
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "BackupRestoreForm.TestConnectionAsync");
                    MessageBox.Show("فشل الاتصال.\n\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SetBusy(false, "جاهز");
                }
            }

            private async Task BackupAsync()
            {
                string folder = txtBackupFolder.Text.Trim();
                if (string.IsNullOrWhiteSpace(folder))
                {
                    MessageBox.Show("اختر مجلد حفظ النسخة الاحتياطية أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SetBusy(true, "جاري إنشاء النسخة الاحتياطية...");
                try
                {
                    await Task.Run(() =>
                    {
                        if (!Directory.Exists(folder))
                            Directory.CreateDirectory(folder);

                        var csb = new SqlConnectionStringBuilder(Db.ConnectionString);
                        string dbName = csb.InitialCatalog;
                        string file = Path.Combine(folder, dbName + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak");

                        AddLog("بدء النسخ الاحتياطي: " + file);

                        string sql = "BACKUP DATABASE " + QuoteName(dbName) +
                                     " TO DISK = N'" + EscapeSql(file) +
                                     "' WITH INIT, CHECKSUM, STATS = 10";

                        using (var con = new SqlConnection(Db.ConnectionString))
                        {
                            con.FireInfoMessageEventOnUserErrors = true;
                            con.InfoMessage += (s, e) => AddLog(e.Message);
                            con.Open();
                            using (var cmd = new SqlCommand(sql, con))
                            {
                                cmd.CommandTimeout = 0;
                                cmd.ExecuteNonQuery();
                            }
                        }

                        AddLog("تم إنشاء النسخة بنجاح.");
                        BeginInvoke(new Action(() => MessageBox.Show("تم إنشاء النسخة الاحتياطية بنجاح.\n\n" + file, "تم", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                    });
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "BackupRestoreForm.BackupAsync");
                    MessageBox.Show("فشل إنشاء النسخة الاحتياطية.\n\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SetBusy(false, "جاهز");
                }
            }

            private async Task RestoreAsync()
            {
                string backupFile = txtRestoreFile.Text.Trim();
                if (string.IsNullOrWhiteSpace(backupFile) || !File.Exists(backupFile))
                {
                    MessageBox.Show("اختر ملف نسخة احتياطية صحيح.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var csb = new SqlConnectionStringBuilder(Db.ConnectionString);
                string dbName = csb.InitialCatalog;

                var confirm = MessageBox.Show(
                    "سيتم استبدال بيانات قاعدة البيانات الحالية.\n\nهل تريد المتابعة؟",
                    "تأكيد الاستعادة",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2
                );

                if (confirm != DialogResult.Yes)
                    return;

                SetBusy(true, "جاري الاستعادة...");
                try
                {
                    await Task.Run(() =>
                    {
                        SqlConnection.ClearAllPools();
                        csb.InitialCatalog = "master";
                        string masterCs = csb.ConnectionString;

                        string sql =
                            "ALTER DATABASE " + QuoteName(dbName) + " SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" + Environment.NewLine +
                            "RESTORE DATABASE " + QuoteName(dbName) + " FROM DISK = N'" + EscapeSql(backupFile) + "' WITH REPLACE, RECOVERY, CHECKSUM, STATS = 10;" + Environment.NewLine +
                            "ALTER DATABASE " + QuoteName(dbName) + " SET MULTI_USER;";

                        try
                        {
                            using (var con = new SqlConnection(masterCs))
                            {
                                con.FireInfoMessageEventOnUserErrors = true;
                                con.InfoMessage += (s, e) => AddLog(e.Message);
                                con.Open();
                                using (var cmd = new SqlCommand(sql, con))
                                {
                                    cmd.CommandTimeout = 0;
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                        catch
                        {
                            TrySetMultiUser(masterCs, dbName);
                            throw;
                        }

                        AddLog("تمت الاستعادة بنجاح.");
                        BeginInvoke(new Action(() => MessageBox.Show("تمت استعادة النسخة بنجاح.\nيفضل إغلاق النظام وفتحه من جديد.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information)));
                    });
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "BackupRestoreForm.RestoreAsync");
                    MessageBox.Show("فشلت الاستعادة.\n\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    SetBusy(false, "جاهز");
                }
            }

            private void BtnBrowseBackupFolder_Click(object sender, EventArgs e)
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    if (Directory.Exists(txtBackupFolder.Text))
                        dialog.SelectedPath = txtBackupFolder.Text;

                    if (dialog.ShowDialog(this) == DialogResult.OK)
                        txtBackupFolder.Text = dialog.SelectedPath;
                }
            }

            private void BtnOpenBackupFolder_Click(object sender, EventArgs e)
            {
                string folder = txtBackupFolder.Text.Trim();
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                System.Diagnostics.Process.Start("explorer.exe", folder);
            }

            private void BtnBrowseRestoreFile_Click(object sender, EventArgs e)
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.Filter = "SQL Backup File (*.bak)|*.bak|All Files (*.*)|*.*";
                    dialog.CheckFileExists = true;
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                        txtRestoreFile.Text = dialog.FileName;
                }
            }

            private string GetDefaultBackupFolder()
            {
                string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Water3Backups");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                return folder;
            }

            private void TrySetMultiUser(string masterConnection, string dbName)
            {
                try
                {
                    using (var con = new SqlConnection(masterConnection))
                    {
                        con.Open();
                        using (var cmd = new SqlCommand("ALTER DATABASE " + QuoteName(dbName) + " SET MULTI_USER", con))
                        {
                            cmd.CommandTimeout = 0;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch { }
            }

            private static string QuoteName(string name)
            {
                return "[" + (name ?? "").Replace("]", "]] ").Replace("]] ", "]]") + "]";
            }

            private static string EscapeSql(string value)
            {
                return (value ?? "").Replace("'", "''");
            }

            private void SetBusy(bool busy, string text)
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new Action(() => SetBusy(busy, text)));
                    return;
                }

                btnBackup.Enabled = !busy;
                btnRestore.Enabled = !busy;
                btnBrowseBackupFolder.Enabled = !busy;
                btnBrowseRestoreFile.Enabled = !busy;
                btnOpenBackupFolder.Enabled = !busy;
                btnTestConnection.Enabled = !busy;
                progressBar.Style = busy ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
                progressBar.MarqueeAnimationSpeed = busy ? 35 : 0;
                lblStatus.Text = text;
                Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
            }

            private void AddLog(string message)
            {
                if (IsDisposed)
                    return;

                if (InvokeRequired)
                {
                    BeginInvoke(new Action(() => AddLog(message)));
                    return;
                }

                lstLog.Items.Add(DateTime.Now.ToString("HH:mm:ss") + " | " + message);
                if (lstLog.Items.Count > 0)
                    lstLog.TopIndex = lstLog.Items.Count - 1;
            }
        }
    }
