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
using System.IO;
using System.Text;
using System.Windows.Forms;
using water3.Services;
using water3.Utils;
namespace water3.Forms
{
    public partial class DatabaseMigrationForm : Form
    {


//namespace water3.Forms
//    {
//        public class DatabaseMigrationForm : Form
//        {
            private ListBox lstFiles;
            private TextBox txtSql;
            private DataGridView gridHistory;

            public DatabaseMigrationForm()
            {
                ProductionUi.PrepareForm(this, "ترقية قاعدة البيانات");
                BuildUi();
                Load += (s, e) => { LoadMigrationFiles(); LoadHistory(); };
            }

            private void BuildUi()
            {
                var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), ColumnCount = 2, RowCount = 2, RightToLeft = RightToLeft.Yes };
                root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
                root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
                root.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
                root.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
                Controls.Add(root);

                var left = ProductionUi.CardPanel();
                root.Controls.Add(left, 0, 0);
                var leftPanel = new Panel { Dock = DockStyle.Fill };
                left.Controls.Add(leftPanel);

                lstFiles = new ListBox { Dock = DockStyle.Fill };
                lstFiles.SelectedIndexChanged += (s, e) => LoadSelectedFile();

                var btns = new TableLayoutPanel { Dock = DockStyle.Bottom, Height = 120, ColumnCount = 1 };
                var btnRefresh = ProductionUi.Button("تحديث الملفات", ProductionUi.Primary);
                var btnApply = ProductionUi.Button("تطبيق الترقية", ProductionUi.Success);
                var btnCore = ProductionUi.Button("إنشاء جداول النظام", ProductionUi.Warning);
                btnRefresh.Click += (s, e) => LoadMigrationFiles();
                btnApply.Click += (s, e) => ApplySelectedMigration();
                btnCore.Click += (s, e) => EnsureCoreTables();
                btns.Controls.Add(btnRefresh, 0, 0);
                btns.Controls.Add(btnApply, 0, 1);
                btns.Controls.Add(btnCore, 0, 2);

                leftPanel.Controls.Add(lstFiles);
                leftPanel.Controls.Add(btns);
                leftPanel.Controls.Add(ProductionUi.Header("ملفات الترقية", "ضع ملفات .sql داخل مجلد Migrations بجانب البرنامج."));

                txtSql = ProductionUi.TextBox(true);
                txtSql.Font = new System.Drawing.Font("Consolas", 10F);
                txtSql.ScrollBars = ScrollBars.Both;
                root.Controls.Add(txtSql, 1, 0);

                gridHistory = ProductionUi.Grid();
                root.SetColumnSpan(gridHistory, 2);
                root.Controls.Add(gridHistory, 0, 1);
            }

            private string MigrationsFolder
            {
                get
                {
                    string dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Migrations");
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    return dir;
                }
            }

            private void LoadMigrationFiles()
            {
                try
                {
                    lstFiles.Items.Clear();
                    foreach (string file in Directory.GetFiles(MigrationsFolder, "*.sql"))
                        lstFiles.Items.Add(file);
                    if (lstFiles.Items.Count > 0) lstFiles.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "DatabaseMigrationForm.LoadMigrationFiles");
                    MessageBox.Show("تعذر تحميل ملفات الترقية:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void LoadSelectedFile()
            {
                try
                {
                    if (lstFiles.SelectedItem == null) { txtSql.Clear(); return; }
                    txtSql.Text = File.ReadAllText(lstFiles.SelectedItem.ToString(), Encoding.UTF8);
                }
                catch (Exception ex) { txtSql.Text = ex.Message; }
            }

            private void ApplySelectedMigration()
            {
                if (lstFiles.SelectedItem == null)
                {
                    MessageBox.Show("اختر ملف ترقية أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string file = lstFiles.SelectedItem.ToString();
                if (MessageBox.Show("سيتم تطبيق ملف الترقية:\n" + Path.GetFileName(file), "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

                try
                {
                    AppSchemaService.ApplyMigrationFile(file);
                    MessageBox.Show("تم تطبيق الترقية بنجاح.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadHistory();
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "DatabaseMigrationForm.ApplySelectedMigration");
                    MessageBox.Show("فشل تطبيق الترقية:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    LoadHistory();
                }
            }

            private void EnsureCoreTables()
            {
                try
                {
                    AppSchemaService.EnsureProductionTables();
                    MessageBox.Show("تم إنشاء/تحديث جداول إدارة النظام بنجاح.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadHistory();
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "DatabaseMigrationForm.EnsureCoreTables");
                    MessageBox.Show("تعذر إنشاء الجداول:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void LoadHistory()
            {
                try { gridHistory.DataSource = AppSchemaService.GetMigrationHistory(); }
                catch (Exception ex) { AppErrorLogger.Log(ex, "DatabaseMigrationForm.LoadHistory"); }
            }
        }
    }
