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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using water3.Services;
using water3.Utils;
namespace water3.Forms
{
    public partial class CompanySettingsForm : Form
    {


//namespace water3.Forms
//    {
//        public class CompanySettingsForm : Form
//        {
            private TextBox txtName, txtPhone, txtAddress, txtLogo, txtInvoiceTitle, txtInvoiceFooter, txtCurrency;
            private PictureBox picLogo;

            public CompanySettingsForm()
            {
                ProductionUi.PrepareForm(this, "إعداد بيانات الشركة");
                BuildUi();
                Load += (s, e) => LoadSettings();
            }

            private void BuildUi()
            {
                var root = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(16), ColumnCount = 2, RowCount = 1, RightToLeft = RightToLeft.Yes };
                root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65));
                root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
                Controls.Add(root);

                var card = ProductionUi.CardPanel();
                root.Controls.Add(card, 0, 0);
                var panel = new Panel { Dock = DockStyle.Fill };
                card.Controls.Add(panel);
                panel.Controls.Add(BuildButtons());
                panel.Controls.Add(BuildFields());
                panel.Controls.Add(ProductionUi.Header("إعداد بيانات الشركة", "اسم الشركة، الشعار، الهاتف، العنوان، وبيانات الفاتورة."));

                var logoCard = ProductionUi.CardPanel();
                root.Controls.Add(logoCard, 1, 0);
                picLogo = new PictureBox { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.Zoom, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White };
                logoCard.Controls.Add(picLogo);
                logoCard.Controls.Add(new Label { Text = "معاينة الشعار", Dock = DockStyle.Top, Height = 35, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Tahoma", 11F, FontStyle.Bold), ForeColor = ProductionUi.Primary });
            }

            private Control BuildFields()
            {
                var grid = new TableLayoutPanel { Dock = DockStyle.Top, Height = 390, ColumnCount = 2, RowCount = 7, Padding = new Padding(6), RightToLeft = RightToLeft.Yes };
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
                grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                txtName = ProductionUi.TextBox();
                txtPhone = ProductionUi.TextBox();
                txtAddress = ProductionUi.TextBox(true);
                txtLogo = ProductionUi.TextBox();
                txtInvoiceTitle = ProductionUi.TextBox();
                txtInvoiceFooter = ProductionUi.TextBox(true);
                txtCurrency = ProductionUi.TextBox();

                AddRow(grid, 0, "اسم الشركة", txtName, 48);
                AddRow(grid, 1, "الهاتف", txtPhone, 48);
                AddRow(grid, 2, "العنوان", txtAddress, 78);
                AddLogoRow(grid, 3);
                AddRow(grid, 4, "عنوان الفاتورة", txtInvoiceTitle, 48);
                AddRow(grid, 5, "تذييل الفاتورة", txtInvoiceFooter, 78);
                AddRow(grid, 6, "العملة", txtCurrency, 48);
                return grid;
            }

            private void AddRow(TableLayoutPanel grid, int row, string label, Control input, int height)
            {
                grid.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
                grid.Controls.Add(ProductionUi.Label(label), 0, row);
                grid.Controls.Add(input, 1, row);
            }

            private void AddLogoRow(TableLayoutPanel grid, int row)
            {
                grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
                grid.Controls.Add(ProductionUi.Label("مسار الشعار"), 0, row);
                var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RightToLeft = RightToLeft.Yes };
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
                var btn = ProductionUi.Button("اختيار", ProductionUi.Primary);
                btn.Click += BtnBrowseLogo_Click;
                layout.Controls.Add(txtLogo, 0, 0);
                layout.Controls.Add(btn, 1, 0);
                grid.Controls.Add(layout, 1, row);
            }

            private Control BuildButtons()
            {
                var buttons = new TableLayoutPanel { Dock = DockStyle.Top, Height = 55, ColumnCount = 3, RightToLeft = RightToLeft.Yes };
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
                buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
                var btnSave = ProductionUi.Button("حفظ", ProductionUi.Success);
                var btnReload = ProductionUi.Button("تحديث", ProductionUi.Primary);
                var btnClearLogo = ProductionUi.Button("حذف الشعار", ProductionUi.Danger);
                btnSave.Click += (s, e) => SaveSettings();
                btnReload.Click += (s, e) => LoadSettings();
                btnClearLogo.Click += (s, e) => { txtLogo.Clear(); if (picLogo.Image != null) { picLogo.Image.Dispose(); picLogo.Image = null; } };
                buttons.Controls.Add(btnSave, 0, 0);
                buttons.Controls.Add(btnReload, 1, 0);
                buttons.Controls.Add(btnClearLogo, 2, 0);
                return buttons;
            }

            private void BtnBrowseLogo_Click(object sender, EventArgs e)
            {
                using (var dialog = new OpenFileDialog())
                {
                    dialog.Title = "اختر شعار الشركة";
                    dialog.Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp|All Files|*.*";
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        txtLogo.Text = dialog.FileName;
                        LoadLogoPreview(dialog.FileName);
                    }
                }
            }

            private void LoadSettings()
            {
                try
                {
                    AppSchemaService.EnsureProductionTables();
                    txtName.Text = AppSettingsService.Get("Company.Name", "مؤسسة المياه");
                    txtPhone.Text = AppSettingsService.Get("Company.Phone", "");
                    txtAddress.Text = AppSettingsService.Get("Company.Address", "");
                    txtLogo.Text = AppSettingsService.Get("Company.LogoPath", "");
                    txtInvoiceTitle.Text = AppSettingsService.Get("Invoice.Title", "فاتورة مياه");
                    txtInvoiceFooter.Text = AppSettingsService.Get("Invoice.Footer", "شكراً لتعاملكم معنا");
                    txtCurrency.Text = AppSettingsService.Get("Invoice.Currency", "ريال");
                    LoadLogoPreview(txtLogo.Text);
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "CompanySettingsForm.LoadSettings");
                    MessageBox.Show("تعذر تحميل الإعدادات:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void SaveSettings()
            {
                try
                {
                    AppSettingsService.SetMany(
                        Tuple.Create("Company.Name", txtName.Text.Trim()),
                        Tuple.Create("Company.Phone", txtPhone.Text.Trim()),
                        Tuple.Create("Company.Address", txtAddress.Text.Trim()),
                        Tuple.Create("Company.LogoPath", txtLogo.Text.Trim()),
                        Tuple.Create("Invoice.Title", txtInvoiceTitle.Text.Trim()),
                        Tuple.Create("Invoice.Footer", txtInvoiceFooter.Text.Trim()),
                        Tuple.Create("Invoice.Currency", txtCurrency.Text.Trim())
                    );
                    MessageBox.Show("تم حفظ بيانات الشركة بنجاح.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    AppErrorLogger.Log(ex, "CompanySettingsForm.SaveSettings");
                    MessageBox.Show("تعذر حفظ الإعدادات:\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void LoadLogoPreview(string path)
            {
                try
                {
                    if (picLogo.Image != null) { picLogo.Image.Dispose(); picLogo.Image = null; }
                    if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                    {
                        using (var img = Image.FromFile(path)) picLogo.Image = new Bitmap(img);
                    }
                }
                catch { if (picLogo != null) picLogo.Image = null; }
            }
        }
    }
