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
        private TextBox txtName;
        private TextBox txtSystemName;
        private TextBox txtPhone;
        private TextBox txtEmail;
        private TextBox txtAddress;
        private TextBox txtLogo;
        private TextBox txtInvoiceTitle;
        private TextBox txtInvoiceFooter;
        private TextBox txtCurrency;

        private PictureBox picLogo;

        public CompanySettingsForm()
        {
            ProductionUi.PrepareForm(this, "إعداد بيانات الشركة");

            StartPosition = FormStartPosition.CenterScreen;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Tahoma", 9.5F);
            MinimumSize = new Size(1050, 650);

            BuildUi();

            Load += (s, e) => LoadSettings();
        }

        private void BuildUi()
        {
            SuspendLayout();
            Controls.Clear();

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16),
                ColumnCount = 2,
                RowCount = 1,
                RightToLeft = RightToLeft.Yes,
                BackColor = Color.FromArgb(245, 247, 250),
                Margin = new Padding(0)
            };

            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));

            Controls.Add(root);

            root.Controls.Add(BuildDataCard(), 0, 0);
            root.Controls.Add(BuildLogoPreviewCard(), 1, 0);

            ResumeLayout(false);
        }

        private Control BuildDataCard()
        {
            var card = ProductionUi.CardPanel();
            card.Dock = DockStyle.Fill;
            card.Padding = new Padding(16);
            card.Margin = new Padding(0, 0, 10, 0);
            card.BackColor = Color.White;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                RightToLeft = RightToLeft.Yes,
                BackColor = Color.White,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 74F));   // العنوان
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));   // الأزرار
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));   // الحقول

            var header = ProductionUi.Header(
                "إعداد بيانات الشركة",
                "اسم الشركة، اسم النظام، الشعار، الهاتف، البريد، العنوان، وبيانات الفاتورة والطباعة."
            );

            header.Dock = DockStyle.Fill;
            header.Margin = new Padding(0, 0, 0, 8);

            layout.Controls.Add(header, 0, 0);
            layout.Controls.Add(BuildButtons(), 0, 1);
            layout.Controls.Add(BuildFieldsContainer(), 0, 2);

            card.Controls.Add(layout);

            return card;
        }

        private Control BuildLogoPreviewCard()
        {
            var card = ProductionUi.CardPanel();
            card.Dock = DockStyle.Fill;
            card.Padding = new Padding(14);
            card.Margin = new Padding(10, 0, 0, 0);
            card.BackColor = Color.White;

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            var lblTitle = new Label
            {
                Text = "معاينة الشعار",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Tahoma", 12F, FontStyle.Bold),
                ForeColor = ProductionUi.Primary,
                BackColor = Color.White
            };

            picLogo = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 8, 0, 0)
            };

            layout.Controls.Add(lblTitle, 0, 0);
            layout.Controls.Add(picLogo, 0, 1);

            card.Controls.Add(layout);

            return card;
        }

        private Control BuildButtons()
        {
            var buttons = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                RightToLeft = RightToLeft.Yes,
                BackColor = Color.White,
                Padding = new Padding(0, 8, 0, 8),
                Margin = new Padding(0)
            };

            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145F));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var btnSave = ProductionUi.Button("حفظ", ProductionUi.Success);
            var btnReload = ProductionUi.Button("تحديث", ProductionUi.Primary);
            var btnClearLogo = ProductionUi.Button("حذف الشعار", ProductionUi.Danger);

            PrepareButton(btnSave);
            PrepareButton(btnReload);
            PrepareButton(btnClearLogo);

            btnSave.Click += (s, e) => SaveSettings();
            btnReload.Click += (s, e) => LoadSettings();

            btnClearLogo.Click += (s, e) =>
            {
                if (txtLogo != null)
                    txtLogo.Clear();

                ClearLogoPreview();
            };

            buttons.Controls.Add(btnSave, 0, 0);
            buttons.Controls.Add(btnReload, 1, 0);
            buttons.Controls.Add(btnClearLogo, 2, 0);

            return buttons;
        }

        private Control BuildFieldsContainer()
        {
            var container = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.White,
                Margin = new Padding(0),
                Padding = new Padding(0, 4, 0, 0)
            };

            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RowCount = 9,
                RightToLeft = RightToLeft.Yes,
                Padding = new Padding(4),
                Margin = new Padding(0),
                BackColor = Color.White
            };

            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            txtName = CreateTextBox();
            txtSystemName = CreateTextBox();
            txtPhone = CreateTextBox();
            txtEmail = CreateTextBox();
            txtAddress = CreateTextBox(true);
            txtLogo = CreateTextBox();
            txtInvoiceTitle = CreateTextBox();
            txtInvoiceFooter = CreateTextBox(true);
            txtCurrency = CreateTextBox();

            AddRow(grid, 0, "اسم الشركة", txtName, 48);
            AddRow(grid, 1, "اسم النظام", txtSystemName, 48);
            AddRow(grid, 2, "الهاتف", txtPhone, 48);
            AddRow(grid, 3, "البريد", txtEmail, 48);
            AddRow(grid, 4, "العنوان", txtAddress, 82);
            AddLogoRow(grid, 5);
            AddRow(grid, 6, "عنوان الفاتورة", txtInvoiceTitle, 48);
            AddRow(grid, 7, "تذييل الفاتورة", txtInvoiceFooter, 86);
            AddRow(grid, 8, "العملة", txtCurrency, 48);

            container.Controls.Add(grid);

            return container;
        }

        private TextBox CreateTextBox(bool multiline = false)
        {
            TextBox txt = ProductionUi.TextBox(multiline);

            txt.Dock = DockStyle.Fill;
            txt.Margin = new Padding(6, 4, 6, 4);
            txt.TextAlign = HorizontalAlignment.Right;
            txt.Font = new Font("Tahoma", 9.5F);

            if (multiline)
            {
                txt.Multiline = true;
                txt.ScrollBars = ScrollBars.Vertical;
            }

            return txt;
        }

        private void AddRow(TableLayoutPanel grid, int row, string label, Control input, int height)
        {
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, height));

            var lbl = ProductionUi.Label(label);
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleRight;
            lbl.Margin = new Padding(4);
            lbl.Font = new Font("Tahoma", 9.3F, FontStyle.Bold);

            input.Dock = DockStyle.Fill;
            input.Margin = new Padding(6, 4, 6, 4);

            grid.Controls.Add(lbl, 0, row);
            grid.Controls.Add(input, 1, row);
        }

        private void AddLogoRow(TableLayoutPanel grid, int row)
        {
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));

            var lbl = ProductionUi.Label("مسار الشعار");
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleRight;
            lbl.Margin = new Padding(4);
            lbl.Font = new Font("Tahoma", 9.3F, FontStyle.Bold);

            grid.Controls.Add(lbl, 0, row);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                RightToLeft = RightToLeft.Yes,
                Margin = new Padding(0),
                Padding = new Padding(0),
                BackColor = Color.White
            };

            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115F));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var btnBrowse = ProductionUi.Button("اختيار", ProductionUi.Primary);
            PrepareButton(btnBrowse);
            btnBrowse.Width = 105;
            btnBrowse.Click += BtnBrowseLogo_Click;

            txtLogo.Dock = DockStyle.Fill;
            txtLogo.Margin = new Padding(6, 4, 6, 4);

            layout.Controls.Add(btnBrowse, 0, 0);
            layout.Controls.Add(txtLogo, 1, 0);

            grid.Controls.Add(layout, 1, row);
        }

        private void PrepareButton(Button btn)
        {
            btn.Dock = DockStyle.Fill;
            btn.Height = 34;
            btn.Margin = new Padding(4, 0, 4, 0);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Tahoma", 9.3F, FontStyle.Bold);
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.Cursor = Cursors.Hand;
        }

        private void BtnBrowseLogo_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Title = "اختر شعار الشركة";
                dialog.Filter = "Images|*.png;*.jpg;*.jpeg;*.bmp|All Files|*.*";

                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    string savedLogoPath = CopyLogoToAppFolder(dialog.FileName);

                    txtLogo.Text = savedLogoPath;
                    LoadLogoPreview(savedLogoPath);
                }
            }
        }

        private string CopyLogoToAppFolder(string sourcePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                    return sourcePath;

                string assetsDir = Path.Combine(Application.StartupPath, "Assets");

                if (!Directory.Exists(assetsDir))
                    Directory.CreateDirectory(assetsDir);

                string extension = Path.GetExtension(sourcePath);

                if (string.IsNullOrWhiteSpace(extension))
                    extension = ".png";

                string targetPath = Path.Combine(assetsDir, "company_logo" + extension);

                File.Copy(sourcePath, targetPath, true);

                return targetPath;
            }
            catch
            {
                return sourcePath;
            }
        }

        private void LoadSettings()
        {
            try
            {
                AppSchemaService.EnsureProductionTables();

                txtName.Text = AppSettingsService.Get("Company.Name", "مؤسسة المياه");
                txtSystemName.Text = AppSettingsService.Get("System.Name", "نظام إدارة المياه");
                txtPhone.Text = AppSettingsService.Get("Company.Phone", "");
                txtEmail.Text = AppSettingsService.Get("Company.Email", "");
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

                MessageBox.Show(
                    "تعذر تحميل الإعدادات:\n" + ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void SaveSettings()
        {
            try
            {
                AppSettingsService.SetMany(
                    Tuple.Create("Company.Name", SafeText(txtName)),
                    Tuple.Create("System.Name", SafeText(txtSystemName)),
                    Tuple.Create("Company.Phone", SafeText(txtPhone)),
                    Tuple.Create("Company.Email", SafeText(txtEmail)),
                    Tuple.Create("Company.Address", SafeText(txtAddress)),
                    Tuple.Create("Company.LogoPath", SafeText(txtLogo)),
                    Tuple.Create("Invoice.Title", SafeText(txtInvoiceTitle)),
                    Tuple.Create("Invoice.Footer", SafeText(txtInvoiceFooter)),
                    Tuple.Create("Invoice.Currency", SafeText(txtCurrency))
                );

                MessageBox.Show(
                    "تم حفظ بيانات الشركة بنجاح.",
                    "تم",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                AppErrorLogger.Log(ex, "CompanySettingsForm.SaveSettings");

                MessageBox.Show(
                    "تعذر حفظ الإعدادات:\n" + ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private string SafeText(TextBox txt)
        {
            return txt == null ? string.Empty : txt.Text.Trim();
        }

        private void LoadLogoPreview(string path)
        {
            try
            {
                ClearLogoPreview();

                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    using (Image img = Image.FromFile(path))
                    {
                        picLogo.Image = new Bitmap(img);
                    }
                }
            }
            catch
            {
                ClearLogoPreview();
            }
        }

        private void ClearLogoPreview()
        {
            try
            {
                if (picLogo != null && picLogo.Image != null)
                {
                    Image oldImage = picLogo.Image;
                    picLogo.Image = null;
                    oldImage.Dispose();
                }
            }
            catch
            {
                if (picLogo != null)
                    picLogo.Image = null;
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
*/