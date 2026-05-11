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
using System.Drawing.Printing;
using System.Windows.Forms;
using water3.Services;
using water3.Utils;
namespace water3.Forms
{
    public partial class PrinterInvoiceSettingsForm : Form
    {
 

//namespace water3.Forms
//    {
//        public class PrinterInvoiceSettingsForm : Form
//        {
            private ComboBox cmbPrinters, cmbPaper, cmbTemplate;
            private TextBox txtCopies;
            private CheckBox chkPreviewBeforePrint;

            public PrinterInvoiceSettingsForm()
            {
                ProductionUi.PrepareForm(this, "إعداد الطابعة والفواتير");
                BuildUi();
                Load += (s, e) => LoadSettings();
            }

            private void BuildUi()
            {
                var root = ProductionUi.CardPanel();
                root.Dock = DockStyle.Fill;
                Controls.Add(root);

                var panel = new Panel { Dock = DockStyle.Fill };
                root.Controls.Add(panel);
                panel.Controls.Add(BuildButtons());
                panel.Controls.Add(BuildFields());
                panel.Controls.Add(ProductionUi.Header("إعداد الطابعة والفواتير", "اختيار الطابعة الافتراضية، حجم الورق، نموذج الفاتورة، وعدد النسخ."));
            }

            private Control BuildFields()
            {
                var grid = new TableLayoutPanel { Dock = DockStyle.Top, Height = 260, ColumnCount = 2, RowCount = 5, Padding = new Padding(10), RightToLeft = RightToLeft.Yes };
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                cmbPrinters = ProductionUi.Combo();
                cmbPaper = ProductionUi.Combo();
                cmbTemplate = ProductionUi.Combo();
                txtCopies = ProductionUi.TextBox();
                chkPreviewBeforePrint = new CheckBox { Text = "عرض معاينة قبل الطباعة", Dock = DockStyle.Fill, Checked = true };

                cmbPaper.Items.AddRange(new object[] { "A4", "A5", "Receipt 80mm", "Receipt 58mm" });
                cmbTemplate.Items.AddRange(new object[] { "Classic", "Modern", "Thermal Receipt" });

                AddRow(grid, 0, "الطابعة الافتراضية", cmbPrinters);
                AddRow(grid, 1, "حجم الورق", cmbPaper);
                AddRow(grid, 2, "نموذج الفاتورة", cmbTemplate);
                AddRow(grid, 3, "عدد النسخ", txtCopies);
                AddRow(grid, 4, "المعاينة", chkPreviewBeforePrint);
                return grid;
            }

            private void AddRow(TableLayoutPanel grid, int row, string label, Control input)
            {
                grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
                grid.Controls.Add(ProductionUi.Label(label), 0, row);
                grid.Controls.Add(input, 1, row);
            }

            private Control BuildButtons()
            {
                var buttons = new TableLayoutPanel { Dock = DockStyle.Top, Height = 55, ColumnCount = 3, RightToLeft = RightToLeft.Yes };
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                var btnSave = ProductionUi.Button("حفظ", ProductionUi.Success);
                var btnReload = ProductionUi.Button("تحديث", ProductionUi.Primary);
                var btnTest = ProductionUi.Button("اختبار الطابعة", ProductionUi.Warning);
                btnSave.Click += (s, e) => SaveSettings();
                btnReload.Click += (s, e) => LoadSettings();
                btnTest.Click += (s, e) => TestPrinter();

                buttons.Controls.Add(btnSave, 0, 0);
                buttons.Controls.Add(btnReload, 1, 0);
                buttons.Controls.Add(btnTest, 2, 0);
                return buttons;
            }

            private void LoadSettings()
            {
                try
                {
                    AppSchemaService.EnsureProductionTables();
                    cmbPrinters.Items.Clear();
                    foreach (string printer in PrinterSettings.InstalledPrinters) cmbPrinters.Items.Add(printer);

                    string savedPrinter = AppSettingsService.Get("Print.DefaultPrinter", "");
                    if (!string.IsNullOrWhiteSpace(savedPrinter) && cmbPrinters.Items.Contains(savedPrinter)) cmbPrinters.SelectedItem = savedPrinter;
                    else if (cmbPrinters.Items.Count > 0) cmbPrinters.SelectedIndex = 0;

                    cmbPaper.SelectedItem = AppSettingsService.Get("Print.PaperSize", "A4");
                    if (cmbPaper.SelectedIndex < 0) cmbPaper.SelectedIndex = 0;

                    cmbTemplate.SelectedItem = AppSettingsService.Get("Print.InvoiceTemplate", "Classic");
                    if (cmbTemplate.SelectedIndex < 0) cmbTemplate.SelectedIndex = 0;

                    txtCopies.Text = AppSettingsService.Get("Print.Copies", "1");
                    chkPreviewBeforePrint.Checked = AppSettingsService.Get("Print.PreviewBeforePrint", "1") == "1";
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "PrinterInvoiceSettingsForm.LoadSettings");
                    MessageBox.Show("تعذر تحميل إعدادات الطباعة:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void SaveSettings()
            {
                try
                {
                    int copies;
                    if (!int.TryParse(txtCopies.Text, out copies) || copies <= 0) copies = 1;

                    AppSettingsService.SetMany(
                        Tuple.Create("Print.DefaultPrinter", cmbPrinters.Text),
                        Tuple.Create("Print.PaperSize", cmbPaper.Text),
                        Tuple.Create("Print.InvoiceTemplate", cmbTemplate.Text),
                        Tuple.Create("Print.Copies", copies.ToString()),
                        Tuple.Create("Print.PreviewBeforePrint", chkPreviewBeforePrint.Checked ? "1" : "0")
                    );
                    MessageBox.Show("تم حفظ إعدادات الطباعة بنجاح.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "PrinterInvoiceSettingsForm.SaveSettings");
                    MessageBox.Show("تعذر حفظ إعدادات الطباعة:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void TestPrinter()
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(cmbPrinters.Text))
                    {
                        MessageBox.Show("اختر طابعة أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var ps = new PrinterSettings { PrinterName = cmbPrinters.Text };
                    if (!ps.IsValid) throw new Exception("الطابعة غير صالحة أو غير متصلة.");
                    MessageBox.Show("الطابعة صالحة:\n" + cmbPrinters.Text, "اختبار الطابعة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "PrinterInvoiceSettingsForm.TestPrinter");
                    MessageBox.Show("فشل اختبار الطابعة:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
