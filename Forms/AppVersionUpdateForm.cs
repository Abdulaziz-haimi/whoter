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
using System.Reflection;
using System.Windows.Forms;
using water3.Services;
using water3.Utils;
namespace water3.Forms
{
    public partial class AppVersionUpdateForm : Form
    {


//namespace water3.Forms
//    {
//        public class AppVersionUpdateForm : Form
//        {
            private TextBox txtAppVersion, txtDbVersion, txtNotes;
            private Label lblAssemblyVersion, lblLastUpdate;

            public AppVersionUpdateForm()
            {
                ProductionUi.PrepareForm(this, "إدارة النسخة والتحديثات");
                BuildUi();
                Load += (s, e) => LoadVersionInfo();
            }

            private void BuildUi()
            {
                var root = ProductionUi.CardPanel();
                root.Dock = DockStyle.Fill;
                Controls.Add(root);

                var panel = new Panel { Dock = DockStyle.Fill };
                root.Controls.Add(panel);

                var fields = new TableLayoutPanel { Dock = DockStyle.Top, Height = 280, ColumnCount = 2, RowCount = 5, Padding = new Padding(10), RightToLeft = RightToLeft.Yes };
                fields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
                fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                lblAssemblyVersion = ProductionUi.Label("");
                lblLastUpdate = ProductionUi.Label("");
                txtAppVersion = ProductionUi.TextBox();
                txtDbVersion = ProductionUi.TextBox();
                txtNotes = ProductionUi.TextBox(true);

                AddRow(fields, 0, "إصدار ملف البرنامج", lblAssemblyVersion, 48);
                AddRow(fields, 1, "رقم إصدار النظام", txtAppVersion, 48);
                AddRow(fields, 2, "رقم إصدار قاعدة البيانات", txtDbVersion, 48);
                AddRow(fields, 3, "آخر تحديث", lblLastUpdate, 48);
                AddRow(fields, 4, "ملاحظات التحديث", txtNotes, 82);

                var buttons = new TableLayoutPanel { Dock = DockStyle.Top, Height = 55, ColumnCount = 4, RightToLeft = RightToLeft.Yes };
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                var btnSave = ProductionUi.Button("حفظ الإصدار", ProductionUi.Success);
                var btnReload = ProductionUi.Button("تحديث", ProductionUi.Primary);
                var btnMigrations = ProductionUi.Button("فتح ترقيات قاعدة البيانات", ProductionUi.Warning);
                var btnHealth = ProductionUi.Button("فحص النظام", ProductionUi.Danger);
                btnSave.Click += (s, e) => SaveVersionInfo();
                btnReload.Click += (s, e) => LoadVersionInfo();
                btnMigrations.Click += (s, e) => new DatabaseMigrationForm().ShowDialog(this);
                btnHealth.Click += (s, e) => new SystemHealthForm().ShowDialog(this);

                buttons.Controls.Add(btnSave, 0, 0);
                buttons.Controls.Add(btnReload, 1, 0);
                buttons.Controls.Add(btnMigrations, 2, 0);
                buttons.Controls.Add(btnHealth, 3, 0);

                panel.Controls.Add(buttons);
                panel.Controls.Add(fields);
                panel.Controls.Add(ProductionUi.Header("إدارة النسخة والتحديثات", "رقم الإصدار، آخر تحديث، وتحديث قاعدة البيانات عبر شاشة Migration."));
            }

            private void AddRow(TableLayoutPanel grid, int row, string label, Control input, int height)
            {
                grid.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
                grid.Controls.Add(ProductionUi.Label(label), 0, row);
                grid.Controls.Add(input, 1, row);
            }

            private void LoadVersionInfo()
            {
                try
                {
                    AppSchemaService.EnsureProductionTables();
                    lblAssemblyVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();

                    using (var con = Db.GetConnection())
                    using (var cmd = new SqlCommand("SELECT AppVersion, DatabaseVersion, LastUpdateAt, UpdateNotes FROM dbo.AppVersionInfo WHERE ID=1;", con))
                    {
                        con.Open();
                        using (var r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {
                                txtAppVersion.Text = Convert.ToString(r["AppVersion"]);
                                txtDbVersion.Text = Convert.ToString(r["DatabaseVersion"]);
                                lblLastUpdate.Text = Convert.ToString(r["LastUpdateAt"]);
                                txtNotes.Text = Convert.ToString(r["UpdateNotes"]);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "AppVersionUpdateForm.LoadVersionInfo");
                    MessageBox.Show("تعذر تحميل بيانات الإصدار:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void SaveVersionInfo()
            {
                try
                {
                    AppSchemaService.EnsureProductionTables();
                    using (var con = Db.GetConnection())
                    using (var cmd = new SqlCommand(@"
MERGE dbo.AppVersionInfo AS T
USING (SELECT 1 AS ID) AS S
ON T.ID = S.ID
WHEN MATCHED THEN
    UPDATE SET AppVersion=@AppVersion, DatabaseVersion=@DbVersion, LastUpdateAt=SYSUTCDATETIME(), UpdateNotes=@Notes
WHEN NOT MATCHED THEN
    INSERT(ID, AppVersion, DatabaseVersion, LastUpdateAt, UpdateNotes)
    VALUES(1, @AppVersion, @DbVersion, SYSUTCDATETIME(), @Notes);", con))
                    {
                        cmd.Parameters.AddWithValue("@AppVersion", txtAppVersion.Text.Trim());
                        cmd.Parameters.AddWithValue("@DbVersion", txtDbVersion.Text.Trim());
                        cmd.Parameters.AddWithValue("@Notes", txtNotes.Text.Trim());
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("تم حفظ بيانات الإصدار.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadVersionInfo();
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "AppVersionUpdateForm.SaveVersionInfo");
                    MessageBox.Show("تعذر حفظ الإصدار:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
