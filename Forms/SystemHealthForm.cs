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
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using water3.Services;
using water3.Utils;
namespace water3.Forms
{
    public partial class SystemHealthForm : Form
    {
  

//namespace water3.Forms
//    {
//        public class SystemHealthForm : Form
//        {
            private DataGridView grid;
            private Label lblSummary;

            public SystemHealthForm()
            {
                ProductionUi.PrepareForm(this, "فحص النظام");
                BuildUi();
                Load += (s, e) => RunChecks();
            }

            private void BuildUi()
            {
                var root = ProductionUi.CardPanel();
                root.Dock = DockStyle.Fill;
                Controls.Add(root);

                var panel = new Panel { Dock = DockStyle.Fill };
                root.Controls.Add(panel);

                grid = ProductionUi.Grid();
                lblSummary = new Label { Dock = DockStyle.Bottom, Height = 35, TextAlign = ContentAlignment.MiddleRight, ForeColor = ProductionUi.Primary };

                var btnRun = ProductionUi.Button("إعادة الفحص", ProductionUi.Primary);
                btnRun.Dock = DockStyle.Top;
                btnRun.Height = 44;
                btnRun.Click += (s, e) => RunChecks();

                panel.Controls.Add(grid);
                panel.Controls.Add(lblSummary);
                panel.Controls.Add(btnRun);
                panel.Controls.Add(ProductionUi.Header("فحص النظام System Health", "فحص الاتصال، الجداول، الصلاحيات، آخر نسخة احتياطية، وإصدار قاعدة البيانات."));
            }

            private void RunChecks()
            {
                var dt = new DataTable();
                dt.Columns.Add("الفحص");
                dt.Columns.Add("الحالة");
                dt.Columns.Add("التفاصيل");
                int ok = 0, fail = 0;

                Action<string, bool, string> add = delegate (string name, bool success, string details)
                {
                    dt.Rows.Add(name, success ? "ناجح" : "فشل", details);
                    if (success) ok++; else fail++;
                };

                try
                {
                    using (var con = Db.GetConnection())
                    {
                        con.Open();
                        add("الاتصال بقاعدة البيانات", true, con.Database + " / " + con.DataSource);
                        AddScalarCheck(con, dt, "إصدار SQL Server", "SELECT CAST(SERVERPROPERTY('ProductVersion') AS nvarchar(100))", ref ok, ref fail);
                        AddTablesCheck(con, dt, ref ok, ref fail);
                        AddPermissionCheck(con, dt, ref ok, ref fail);
                        AddLastBackupCheck(con, dt, ref ok, ref fail);
                        AddVersionCheck(con, dt, ref ok, ref fail);
                    }
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "SystemHealthForm.RunChecks");
                    add("الاتصال بقاعدة البيانات", false, ex.Message);
                }

                grid.DataSource = dt;
                lblSummary.Text = "الفحوصات الناجحة: " + ok + " | الفاشلة: " + fail;
            }

            private void AddScalarCheck(SqlConnection con, DataTable dt, string name, string sql, ref int ok, ref int fail)
            {
                try
                {
                    using (var cmd = new SqlCommand(sql, con))
                    {
                        object value = cmd.ExecuteScalar();
                        dt.Rows.Add(name, "ناجح", Convert.ToString(value));
                        ok++;
                    }
                }
                catch (Exception ex) { dt.Rows.Add(name, "فشل", ex.Message); fail++; }
            }

            private void AddTablesCheck(SqlConnection con, DataTable dt, ref int ok, ref int fail)
            {
                string[] tables = { "Users", "Roles", "Subscribers", "Meters", "Invoices", "Payments", "Receipts", "Accounts", "BillingConstants" };
                foreach (string table in tables)
                {
                    try
                    {
                        using (var cmd = new SqlCommand("SELECT COUNT(1) FROM sys.tables WHERE name=@T;", con))
                        {
                            cmd.Parameters.AddWithValue("@T", table);
                            bool exists = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                            dt.Rows.Add("جدول " + table, exists ? "ناجح" : "فشل", exists ? "موجود" : "غير موجود");
                            if (exists) ok++; else fail++;
                        }
                    }
                    catch (Exception ex) { dt.Rows.Add("جدول " + table, "فشل", ex.Message); fail++; }
                }
            }

            private void AddPermissionCheck(SqlConnection con, DataTable dt, ref int ok, ref int fail)
            {
                string sql = @"SELECT HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'SELECT') AS CanSelect,
                                  HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'INSERT') AS CanInsert,
                                  HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'UPDATE') AS CanUpdate,
                                  HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'DELETE') AS CanDelete;";
                try
                {
                    using (var cmd = new SqlCommand(sql, con))
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            dt.Rows.Add("صلاحيات المستخدم", "ناجح", "SELECT=" + r["CanSelect"] + ", INSERT=" + r["CanInsert"] + ", UPDATE=" + r["CanUpdate"] + ", DELETE=" + r["CanDelete"]);
                            ok++;
                        }
                    }
                }
                catch (Exception ex) { dt.Rows.Add("صلاحيات المستخدم", "فشل", ex.Message); fail++; }
            }

            private void AddLastBackupCheck(SqlConnection con, DataTable dt, ref int ok, ref int fail)
            {
                try
                {
                    AppSchemaService.EnsureProductionTables();
                    using (var cmd = new SqlCommand("SELECT MAX(CreatedAt) FROM dbo.AppBackupHistory;", con))
                    {
                        object value = cmd.ExecuteScalar();
                        if (value == null || value == DBNull.Value) { dt.Rows.Add("آخر نسخة احتياطية", "فشل", "لا توجد نسخة مسجلة داخل النظام."); fail++; }
                        else { dt.Rows.Add("آخر نسخة احتياطية", "ناجح", Convert.ToDateTime(value).ToString("yyyy/MM/dd HH:mm")); ok++; }
                    }
                }
                catch (Exception ex) { dt.Rows.Add("آخر نسخة احتياطية", "فشل", ex.Message); fail++; }
            }

            private void AddVersionCheck(SqlConnection con, DataTable dt, ref int ok, ref int fail)
            {
                try
                {
                    AppSchemaService.EnsureProductionTables();
                    using (var cmd = new SqlCommand("SELECT AppVersion, DatabaseVersion, LastUpdateAt FROM dbo.AppVersionInfo WHERE ID=1;", con))
                    using (var r = cmd.ExecuteReader())
                    {
                        if (r.Read()) { dt.Rows.Add("إصدار النظام وقاعدة البيانات", "ناجح", "App=" + r["AppVersion"] + ", DB=" + r["DatabaseVersion"] + ", Last=" + r["LastUpdateAt"]); ok++; }
                        else { dt.Rows.Add("إصدار النظام وقاعدة البيانات", "فشل", "لا يوجد سجل إصدار."); fail++; }
                    }
                }
                catch (Exception ex) { dt.Rows.Add("إصدار النظام وقاعدة البيانات", "فشل", ex.Message); fail++; }
            }
        }
    }
