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
using System.Windows.Forms;
using water3.Services;
using water3.Utils;
namespace water3.Forms
{
    public partial class ErrorLogsForm : Form
    {
 

//namespace water3.Forms
//    {
//        public class ErrorLogsForm : Form
//        {
            private DataGridView grid;
            private TextBox txtDetails;
            private CheckBox chkOnlyOpen;

            public ErrorLogsForm()
            {
                ProductionUi.PrepareForm(this, "سجل الأخطاء");
                BuildUi();
                Load += (s, e) => LoadLogs();
            }

            private void BuildUi()
            {
                var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), ColumnCount = 1, RowCount = 3, RightToLeft = RightToLeft.Yes };
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 105));
                root.RowStyles.Add(new RowStyle(SizeType.Percent, 65));
                root.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
                Controls.Add(root);

                var top = ProductionUi.CardPanel();
                root.Controls.Add(top, 0, 0);

                var buttons = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 4, RightToLeft = RightToLeft.Yes };
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                var btnRefresh = ProductionUi.Button("تحديث", ProductionUi.Primary);
                var btnResolve = ProductionUi.Button("تعليم كمحلول", ProductionUi.Success);
                var btnDelete = ProductionUi.Button("حذف المحدد", ProductionUi.Danger);
                chkOnlyOpen = new CheckBox { Text = "عرض الأخطاء غير المحلولة فقط", Dock = DockStyle.Fill, Checked = true };

                btnRefresh.Click += (s, e) => LoadLogs();
                btnResolve.Click += (s, e) => ResolveSelected();
                btnDelete.Click += (s, e) => DeleteSelected();

                buttons.Controls.Add(btnRefresh, 0, 0);
                buttons.Controls.Add(btnResolve, 1, 0);
                buttons.Controls.Add(btnDelete, 2, 0);
                buttons.Controls.Add(chkOnlyOpen, 3, 0);
                top.Controls.Add(buttons);
                top.Controls.Add(ProductionUi.Header("سجل الأخطاء Error Logs", "تسجيل الأخطاء بدل ضياعها أو ظهورها للمستخدم النهائي."));

                grid = ProductionUi.Grid();
                grid.SelectionChanged += (s, e) => ShowSelectedDetails();
                root.Controls.Add(grid, 0, 1);

                txtDetails = ProductionUi.TextBox(true);
                txtDetails.ReadOnly = true;
                txtDetails.ScrollBars = ScrollBars.Both;
                root.Controls.Add(txtDetails, 0, 2);
            }

            private void LoadLogs()
            {
                try
                {
                    AppSchemaService.EnsureProductionTables();
                    var dt = new DataTable();

                    using (var con = Db.GetConnection())
                    using (var da = new SqlDataAdapter(@"
SELECT TOP 500 ErrorID AS [الرقم], CreatedAt AS [التاريخ], Source AS [المصدر],
       Message AS [الرسالة], UserName AS [المستخدم], MachineName AS [الجهاز],
       AppVersion AS [الإصدار], IsResolved AS [محلول], StackTrace
FROM dbo.AppErrorLogs
WHERE (@OnlyOpen = 0 OR IsResolved = 0)
ORDER BY ErrorID DESC;", con))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@OnlyOpen", chkOnlyOpen.Checked ? 1 : 0);
                        da.Fill(dt);
                    }

                    grid.DataSource = dt;
                    if (grid.Columns.Contains("StackTrace")) grid.Columns["StackTrace"].Visible = false;
                    ShowSelectedDetails();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("تعذر تحميل سجل الأخطاء:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private int? SelectedErrorId()
            {
                if (grid.CurrentRow == null) return null;
                object value = grid.CurrentRow.Cells["الرقم"].Value;
                int id;
                if (value != null && int.TryParse(value.ToString(), out id)) return id;
                return null;
            }

            private void ShowSelectedDetails()
            {
                try
                {
                    if (grid.CurrentRow == null) { txtDetails.Clear(); return; }
                    string details = "";
                    if (grid.CurrentRow.Cells["StackTrace"] != null && grid.CurrentRow.Cells["StackTrace"].Value != null)
                        details = grid.CurrentRow.Cells["StackTrace"].Value.ToString();
                    txtDetails.Text = details;
                }
                catch { }
            }

            private void ResolveSelected()
            {
                int? id = SelectedErrorId();
                if (!id.HasValue) return;

                try
                {
                    using (var con = Db.GetConnection())
                    using (var cmd = new SqlCommand(@"
UPDATE dbo.AppErrorLogs
SET IsResolved=1, ResolvedAt=SYSUTCDATETIME(), ResolutionNote=N'تم التعليم من شاشة سجل الأخطاء'
WHERE ErrorID=@ID;", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", id.Value);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    LoadLogs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("تعذر تحديث السجل:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void DeleteSelected()
            {
                int? id = SelectedErrorId();
                if (!id.HasValue) return;

                if (MessageBox.Show("حذف الخطأ المحدد؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

                try
                {
                    using (var con = Db.GetConnection())
                    using (var cmd = new SqlCommand("DELETE FROM dbo.AppErrorLogs WHERE ErrorID=@ID;", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", id.Value);
                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                    LoadLogs();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("تعذر حذف السجل:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
