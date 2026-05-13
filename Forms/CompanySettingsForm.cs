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
        private TextBox txtNameEn;
        private TextBox txtSystemName;

        private TextBox txtPhone;
        private TextBox txtMobile;
        private TextBox txtWhatsApp;
        private TextBox txtEmail;
        private TextBox txtWebsite;
        private TextBox txtAddress;

        private TextBox txtLogo;

        private TextBox txtCommercialRegister;
        private TextBox txtTaxNumber;
        private TextBox txtBankName;
        private TextBox txtBankAccount;

        private TextBox txtInvoiceTitle;
        private TextBox txtInvoiceFooter;
        private TextBox txtInvoiceNote1;
        private TextBox txtInvoiceNote2;
        private TextBox txtCurrency;

        private PictureBox picLogo;

        private Label lblPreviewName;
        private Label lblPreviewSystem;
        private Label lblPreviewContact;
        private Label lblPreviewAddress;
        private Label lblStatus;

        private readonly Color PageBack = Color.FromArgb(245, 247, 250);
        private readonly Color CardBack = Color.White;
        private readonly Color Primary = Color.FromArgb(0, 102, 204);
        private readonly Color Muted = Color.FromArgb(100, 116, 139);
        private readonly Color Border = Color.FromArgb(226, 232, 240);

        public CompanySettingsForm()
        {
            ProductionUi.PrepareForm(this, "إعداد بيانات الشركة");

            StartPosition = FormStartPosition.CenterScreen;
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            BackColor = PageBack;
            Font = new Font("Tahoma", 9.5F);
            MinimumSize = new Size(1180, 720);
            AutoScaleMode = AutoScaleMode.Dpi;

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
                BackColor = PageBack,
                Margin = new Padding(0)
            };

            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68F));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32F));

            root.Controls.Add(BuildDataCard(), 0, 0);
            root.Controls.Add(BuildPreviewCard(), 1, 0);

            Controls.Add(root);
            ResumeLayout(false);
        }

        private Control BuildDataCard()
        {
            var card = CreateCard();
            card.Padding = new Padding(16);
            card.Margin = new Padding(0, 0, 10, 0);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                BackColor = CardBack,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 76F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));

            var header = ProductionUi.Header(
                "إعداد بيانات الشركة",
                "إدارة بيانات الشركة التي ستظهر في التقارير، الفواتير، السندات، والطباعة."
            );

            header.Dock = DockStyle.Fill;
            header.Margin = new Padding(0, 0, 0, 8);

            lblStatus = new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Muted,
                Font = new Font("Tahoma", 9F, FontStyle.Bold),
                BackColor = CardBack
            };

            layout.Controls.Add(header, 0, 0);
            layout.Controls.Add(BuildButtons(), 0, 1);
            layout.Controls.Add(BuildFieldsContainer(), 0, 2);
            layout.Controls.Add(lblStatus, 0, 3);

            card.Controls.Add(layout);
            return card;
        }

        private Control BuildPreviewCard()
        {
            var card = CreateCard();
            card.Padding = new Padding(14);
            card.Margin = new Padding(10, 0, 0, 0);

            var layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                BackColor = CardBack,
                Margin = new Padding(0)
            };

            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 68F));

            var lblTitle = new Label
            {
                Text = "معاينة بيانات الطباعة",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Tahoma", 12F, FontStyle.Bold),
                ForeColor = Primary,
                BackColor = CardBack
            };

            picLogo = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 8, 0, 8)
            };

            lblPreviewName = PreviewLabel(13F, FontStyle.Bold, Primary);
            lblPreviewSystem = PreviewLabel(9.5F, FontStyle.Bold, Color.FromArgb(30, 41, 59));
            lblPreviewContact = PreviewLabel(8.8F, FontStyle.Regular, Muted);
            lblPreviewAddress = PreviewLabel(8.8F, FontStyle.Regular, Muted);

            layout.Controls.Add(lblTitle, 0, 0);
            layout.Controls.Add(picLogo, 0, 1);
            layout.Controls.Add(lblPreviewName, 0, 2);
            layout.Controls.Add(lblPreviewSystem, 0, 3);
            layout.Controls.Add(lblPreviewContact, 0, 4);
            layout.Controls.Add(lblPreviewAddress, 0, 5);

            card.Controls.Add(layout);
            return card;
        }

        private Label PreviewLabel(float size, FontStyle style, Color color)
        {
            return new Label
            {
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Tahoma", size, style),
                ForeColor = color,
                BackColor = CardBack,
                AutoEllipsis = true
            };
        }

        private Control BuildButtons()
        {
            var buttons = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 5,
                RowCount = 1,
                RightToLeft = RightToLeft.Yes,
                BackColor = CardBack,
                Padding = new Padding(0, 8, 0, 8),
                Margin = new Padding(0)
            };

            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 125F));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145F));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145F));
            buttons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var btnSave = ProductionUi.Button("حفظ", ProductionUi.Success);
            var btnReload = ProductionUi.Button("تحديث", ProductionUi.Primary);
            var btnPreview = ProductionUi.Button("معاينة", Color.FromArgb(14, 165, 233));
            var btnClearLogo = ProductionUi.Button("حذف الشعار", ProductionUi.Danger);

            PrepareButton(btnSave);
            PrepareButton(btnReload);
            PrepareButton(btnPreview);
            PrepareButton(btnClearLogo);

            btnSave.Click += (s, e) => SaveSettings();
            btnReload.Click += (s, e) => LoadSettings();
            btnPreview.Click += (s, e) => UpdatePreview();

            btnClearLogo.Click += (s, e) =>
            {
                if (txtLogo != null)
                    txtLogo.Clear();

                ClearLogoPreview();
                UpdatePreview();
            };

            buttons.Controls.Add(btnSave, 0, 0);
            buttons.Controls.Add(btnReload, 1, 0);
            buttons.Controls.Add(btnPreview, 2, 0);
            buttons.Controls.Add(btnClearLogo, 3, 0);

            return buttons;
        }

        private Control BuildFieldsContainer()
        {
            var container = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = CardBack,
                Margin = new Padding(0),
                Padding = new Padding(0, 4, 0, 0)
            };

            var grid = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                RightToLeft = RightToLeft.Yes,
                Padding = new Padding(4),
                Margin = new Padding(0),
                BackColor = CardBack
            };

            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 155F));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            int row = 0;

            txtName = CreateTextBox();
            txtNameEn = CreateTextBox();
            txtSystemName = CreateTextBox();

            txtPhone = CreateTextBox();
            txtMobile = CreateTextBox();
            txtWhatsApp = CreateTextBox();
            txtEmail = CreateTextBox();
            txtWebsite = CreateTextBox();
            txtAddress = CreateTextBox(true);

            txtLogo = CreateTextBox();

            txtCommercialRegister = CreateTextBox();
            txtTaxNumber = CreateTextBox();
            txtBankName = CreateTextBox();
            txtBankAccount = CreateTextBox();

            txtInvoiceTitle = CreateTextBox();
            txtInvoiceFooter = CreateTextBox(true);
            txtInvoiceNote1 = CreateTextBox(true);
            txtInvoiceNote2 = CreateTextBox(true);
            txtCurrency = CreateTextBox();

            AddSection(grid, ref row, "بيانات الشركة الأساسية");
            AddRow(grid, ref row, "اسم الشركة", txtName, 48);
            AddRow(grid, ref row, "اسم الشركة بالإنجليزية", txtNameEn, 48);
            AddRow(grid, ref row, "اسم النظام", txtSystemName, 48);

            AddSection(grid, ref row, "بيانات التواصل والعنوان");
            AddRow(grid, ref row, "الهاتف", txtPhone, 48);
            AddRow(grid, ref row, "الجوال", txtMobile, 48);
            AddRow(grid, ref row, "واتساب", txtWhatsApp, 48);
            AddRow(grid, ref row, "البريد الإلكتروني", txtEmail, 48);
            AddRow(grid, ref row, "الموقع الإلكتروني", txtWebsite, 48);
            AddRow(grid, ref row, "العنوان", txtAddress, 86);

            AddSection(grid, ref row, "الشعار");
            AddLogoRow(grid, ref row);

            AddSection(grid, ref row, "بيانات رسمية ومالية اختيارية");
            AddRow(grid, ref row, "السجل التجاري", txtCommercialRegister, 48);
            AddRow(grid, ref row, "الرقم الضريبي", txtTaxNumber, 48);
            AddRow(grid, ref row, "اسم البنك", txtBankName, 48);
            AddRow(grid, ref row, "رقم الحساب", txtBankAccount, 48);

            AddSection(grid, ref row, "بيانات الفاتورة والطباعة");
            AddRow(grid, ref row, "عنوان الفاتورة", txtInvoiceTitle, 48);
            AddRow(grid, ref row, "تذييل الفاتورة", txtInvoiceFooter, 82);
            AddRow(grid, ref row, "ملاحظة 1", txtInvoiceNote1, 82);
            AddRow(grid, ref row, "ملاحظة 2", txtInvoiceNote2, 82);
            AddRow(grid, ref row, "العملة", txtCurrency, 48);

            container.Controls.Add(grid);
            return container;
        }

        private void AddSection(TableLayoutPanel grid, ref int row, string title)
        {
            grid.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));

            var lbl = new Label
            {
                Text = title,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Tahoma", 10F, FontStyle.Bold),
                ForeColor = Primary,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(8, 0, 8, 0),
                Margin = new Padding(4, 8, 4, 4)
            };

            grid.Controls.Add(lbl, 0, row);
            grid.SetColumnSpan(lbl, 2);

            row++;
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

            txt.TextChanged += (s, e) => UpdatePreview();

            return txt;
        }

        private void AddRow(TableLayoutPanel grid, ref int row, string label, Control input, int height)
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

            row++;
        }

        private void AddLogoRow(TableLayoutPanel grid, ref int row)
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
                BackColor = CardBack
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

            row++;
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

        private Panel CreateCard()
        {
            Panel panel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = CardBack
            };

            panel.Paint += (s, e) =>
            {
                Control c = s as Control;
                if (c == null) return;

                using (Pen pen = new Pen(Border))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, c.Width - 1, c.Height - 1);
                }
            };

            return panel;
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
                    UpdatePreview();
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
                txtNameEn.Text = AppSettingsService.Get("Company.NameEn", "");
                txtSystemName.Text = AppSettingsService.Get("System.Name", "نظام إدارة المياه");

                txtPhone.Text = AppSettingsService.Get("Company.Phone", "");
                txtMobile.Text = AppSettingsService.Get("Company.Mobile", "");
                txtWhatsApp.Text = AppSettingsService.Get("Company.WhatsApp", "");
                txtEmail.Text = AppSettingsService.Get("Company.Email", "");
                txtWebsite.Text = AppSettingsService.Get("Company.Website", "");
                txtAddress.Text = AppSettingsService.Get("Company.Address", "");

                txtLogo.Text = AppSettingsService.Get("Company.LogoPath", "");

                txtCommercialRegister.Text = AppSettingsService.Get("Company.CommercialRegister", "");
                txtTaxNumber.Text = AppSettingsService.Get("Company.TaxNumber", "");
                txtBankName.Text = AppSettingsService.Get("Company.BankName", "");
                txtBankAccount.Text = AppSettingsService.Get("Company.BankAccount", "");

                txtInvoiceTitle.Text = AppSettingsService.Get("Invoice.Title", "فاتورة مياه");
                txtInvoiceFooter.Text = AppSettingsService.Get("Invoice.Footer", "شكراً لتعاملكم معنا");
                txtInvoiceNote1.Text = AppSettingsService.Get("Invoice.Note1", "");
                txtInvoiceNote2.Text = AppSettingsService.Get("Invoice.Note2", "");
                txtCurrency.Text = AppSettingsService.Get("Invoice.Currency", "ريال");

                LoadLogoPreview(txtLogo.Text);
                UpdatePreview();

                lblStatus.ForeColor = Muted;
                lblStatus.Text = "تم تحميل بيانات الشركة.";
            }
            catch (Exception ex)
            {
                AppErrorLogger.Log(ex, "CompanySettingsForm.LoadSettings");

                lblStatus.ForeColor = Color.Firebrick;
                lblStatus.Text = "تعذر تحميل بيانات الشركة.";

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
                    Tuple.Create("Company.NameEn", SafeText(txtNameEn)),
                    Tuple.Create("System.Name", SafeText(txtSystemName)),

                    Tuple.Create("Company.Phone", SafeText(txtPhone)),
                    Tuple.Create("Company.Mobile", SafeText(txtMobile)),
                    Tuple.Create("Company.WhatsApp", SafeText(txtWhatsApp)),
                    Tuple.Create("Company.Email", SafeText(txtEmail)),
                    Tuple.Create("Company.Website", SafeText(txtWebsite)),
                    Tuple.Create("Company.Address", SafeText(txtAddress)),

                    Tuple.Create("Company.LogoPath", SafeText(txtLogo)),

                    Tuple.Create("Company.CommercialRegister", SafeText(txtCommercialRegister)),
                    Tuple.Create("Company.TaxNumber", SafeText(txtTaxNumber)),
                    Tuple.Create("Company.BankName", SafeText(txtBankName)),
                    Tuple.Create("Company.BankAccount", SafeText(txtBankAccount)),

                    Tuple.Create("Invoice.Title", SafeText(txtInvoiceTitle)),
                    Tuple.Create("Invoice.Footer", SafeText(txtInvoiceFooter)),
                    Tuple.Create("Invoice.Note1", SafeText(txtInvoiceNote1)),
                    Tuple.Create("Invoice.Note2", SafeText(txtInvoiceNote2)),
                    Tuple.Create("Invoice.Currency", SafeText(txtCurrency))
                );

                UpdatePreview();

                lblStatus.ForeColor = Color.DarkGreen;
                lblStatus.Text = "تم حفظ بيانات الشركة بنجاح.";

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

                lblStatus.ForeColor = Color.Firebrick;
                lblStatus.Text = "تعذر حفظ بيانات الشركة.";

                MessageBox.Show(
                    "تعذر حفظ الإعدادات:\n" + ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void UpdatePreview()
        {
            if (lblPreviewName == null)
                return;

            string companyName = SafeText(txtName);
            string systemName = SafeText(txtSystemName);
            string phone = SafeText(txtPhone);
            string mobile = SafeText(txtMobile);
            string email = SafeText(txtEmail);
            string address = SafeText(txtAddress);

            lblPreviewName.Text = string.IsNullOrWhiteSpace(companyName)
                ? "اسم الشركة"
                : companyName;

            lblPreviewSystem.Text = string.IsNullOrWhiteSpace(systemName)
                ? "اسم النظام"
                : systemName;

            string contact = "";

            if (!string.IsNullOrWhiteSpace(phone))
                contact += "الهاتف: " + phone;

            if (!string.IsNullOrWhiteSpace(mobile))
                contact += (contact.Length > 0 ? " | " : "") + "الجوال: " + mobile;

            if (!string.IsNullOrWhiteSpace(email))
                contact += (contact.Length > 0 ? " | " : "") + "البريد: " + email;

            lblPreviewContact.Text = string.IsNullOrWhiteSpace(contact)
                ? "بيانات التواصل"
                : contact;

            lblPreviewAddress.Text = string.IsNullOrWhiteSpace(address)
                ? "العنوان"
                : "العنوان: " + address;
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

*/