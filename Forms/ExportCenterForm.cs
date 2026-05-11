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
using water3.Utils;
namespace water3.Forms
{
    public partial class ExportCenterForm : Form
    {


            private ComboBox cmbTables;
            private NumericUpDown numTop;
            private DataGridView grid;

            public ExportCenterForm()
            {
                ProductionUi.PrepareForm(this, "مركز التصدير");
                BuildUi();
                Load += (s, e) => LoadTables();
            }

            private void BuildUi()
            {
                var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(14), ColumnCount = 1, RowCount = 2, RightToLeft = RightToLeft.Yes };
                root.RowStyles.Add(new RowStyle(SizeType.Absolute, 150));
                root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
                Controls.Add(root);

                var top = ProductionUi.CardPanel();
                root.Controls.Add(top, 0, 0);
                var p = new Panel { Dock = DockStyle.Fill };
                top.Controls.Add(p);

                var controls = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 8, RightToLeft = RightToLeft.Yes };
                controls.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100));
                controls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
                controls.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80));
                controls.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
                controls.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
                controls.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
                controls.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
                controls.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));

                cmbTables = ProductionUi.Combo();
                numTop = new NumericUpDown { Dock = DockStyle.Fill, Minimum = 10, Maximum = 100000, Value = 1000, Increment = 100 };

                var btnLoad = ProductionUi.Button("عرض", ProductionUi.Primary);
                var btnExcel = ProductionUi.Button("Excel", ProductionUi.Success);
                var btnCsv = ProductionUi.Button("CSV", ProductionUi.Warning);
                var btnPdf = ProductionUi.Button("PDF", ProductionUi.Danger);
                btnLoad.Click += (s, e) => LoadData();
                btnExcel.Click += (s, e) => Export("xls");
                btnCsv.Click += (s, e) => Export("csv");
                btnPdf.Click += (s, e) => Export("pdf");

                controls.Controls.Add(ProductionUi.Label("الجدول"), 0, 0);
                controls.Controls.Add(cmbTables, 1, 0);
                controls.Controls.Add(ProductionUi.Label("عدد"), 2, 0);
                controls.Controls.Add(numTop, 3, 0);
                controls.Controls.Add(btnLoad, 4, 0);
                controls.Controls.Add(btnExcel, 5, 0);
                controls.Controls.Add(btnCsv, 6, 0);
                controls.Controls.Add(btnPdf, 7, 0);

                p.Controls.Add(controls);
                p.Controls.Add(ProductionUi.Header("مركز التصدير Excel / PDF / CSV", "تصدير أي جدول من قاعدة البيانات أو ربط الخدمة بأي DataGridView داخل الشاشات."));

                grid = ProductionUi.Grid();
                root.Controls.Add(grid, 0, 1);
            }

            private void LoadTables()
            {
                try
                {
                    cmbTables.Items.Clear();
                    using (var con = Db.GetConnection())
                    using (var cmd = new SqlCommand("SELECT name FROM sys.tables WHERE is_ms_shipped=0 ORDER BY name;", con))
                    {
                        con.Open();
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read()) cmbTables.Items.Add(r.GetString(0));
                        }
                    }
                    if (cmbTables.Items.Count > 0) cmbTables.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "ExportCenterForm.LoadTables");
                    MessageBox.Show("تعذر تحميل الجداول:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void LoadData()
            {
                try
                {
                    if (cmbTables.SelectedItem == null) return;
                    string table = cmbTables.SelectedItem.ToString();
                    var dt = new DataTable();

                    using (var con = Db.GetConnection())
                    using (var cmd = new SqlCommand("SELECT TOP (" + Convert.ToInt32(numTop.Value) + ") * FROM " + QuoteName(table) + ";", con))
                    using (var da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                    grid.DataSource = dt;
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "ExportCenterForm.LoadData");
                    MessageBox.Show("تعذر عرض البيانات:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void Export(string type)
            {
                try
                {
                    if (grid.DataSource == null || grid.Rows.Count == 0)
                    {
                        MessageBox.Show("اعرض البيانات أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    string table = cmbTables.Text;
                    using (var dialog = new SaveFileDialog())
                    {
                        dialog.FileName = table + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                        if (type == "csv") dialog.Filter = "CSV File (*.csv)|*.csv";
                        else if (type == "pdf") dialog.Filter = "PDF File (*.pdf)|*.pdf";
                        else dialog.Filter = "Excel File (*.xls)|*.xls";

                        if (dialog.ShowDialog() != DialogResult.OK) return;

                        DataTable dt = DataExportService.DataTableFromGrid(grid);
                        if (type == "csv") DataExportService.ExportCsv(dt, dialog.FileName);
                        else if (type == "pdf") DataExportService.ExportPdfImage(dt, dialog.FileName, table);
                        else DataExportService.ExportExcelHtml(dt, dialog.FileName, table);

                        MessageBox.Show("تم التصدير بنجاح.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "ExportCenterForm.Export");
                    MessageBox.Show("تعذر التصدير:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private string QuoteName(string name)
            {
                return "[" + (name ?? "").Replace("]", "]]") + "]";
            }
        }
    }
