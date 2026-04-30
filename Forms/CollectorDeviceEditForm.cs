using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace water3.Forms
{
    public partial class CollectorDeviceEditForm : Form
    {


 
            private TextBox txtDeviceName;
            private TextBox txtPhoneNumber;
            private TextBox txtDeviceCode;
            private TextBox txtDeviceModel;
            private TextBox txtAppVersion;
            private CheckBox chkApproved;
            private CheckBox chkActive;
            private Button btnGenerateCode;
            private Button btnSave;
            private Button btnCancel;

            public string DeviceNameValue { get; private set; }
            public string PhoneNumberValue { get; private set; }
            public string DeviceCodeValue { get; private set; }
            public string DeviceModelValue { get; private set; }
            public string AppVersionValue { get; private set; }
            public bool IsApprovedValue { get; private set; }
            public bool IsActiveValue { get; private set; }

            public CollectorDeviceEditForm()
                : this("", "", "", "", "1.0.0", true, true)
            {
            }

            public CollectorDeviceEditForm(
                string deviceName,
                string phoneNumber,
                string deviceCode,
                string deviceModel,
                string appVersion,
                bool isApproved,
                bool isActive)
            {
                InitializeComponent();

                txtDeviceName.Text = deviceName ?? "";
                txtPhoneNumber.Text = phoneNumber ?? "";
                txtDeviceCode.Text = deviceCode ?? "";
                txtDeviceModel.Text = deviceModel ?? "";
                txtAppVersion.Text = string.IsNullOrWhiteSpace(appVersion) ? "1.0.0" : appVersion;
                chkApproved.Checked = isApproved;
                chkActive.Checked = isActive;
            }

            private void InitializeComponent()
            {
                Text = "بيانات جهاز المحصل";
                StartPosition = FormStartPosition.CenterParent;
                RightToLeft = RightToLeft.Yes;
                RightToLeftLayout = true;
                Font = new Font("Tahoma", 10F);
                BackColor = Color.FromArgb(245, 247, 250);
                Size = new Size(540, 430);
                MinimumSize = new Size(540, 430);

                var root = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(18),
                    ColumnCount = 2,
                    RowCount = 7
                };

                root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
                root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                for (int i = 0; i < 6; i++)
                    root.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));

                root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

                Controls.Add(root);

                txtDeviceName = CreateTextBox();
                txtPhoneNumber = CreateTextBox();
                txtDeviceCode = CreateTextBox();
                txtDeviceModel = CreateTextBox();
                txtAppVersion = CreateTextBox();

                btnGenerateCode = new Button
                {
                    Text = "توليد",
                    Dock = DockStyle.Left,
                    Width = 80,
                    BackColor = Color.FromArgb(0, 87, 183),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnGenerateCode.Click += BtnGenerateCode_Click;

                var codePanel = new Panel { Dock = DockStyle.Fill };
                txtDeviceCode.Dock = DockStyle.Fill;
                codePanel.Controls.Add(txtDeviceCode);
                codePanel.Controls.Add(btnGenerateCode);

                chkApproved = new CheckBox
                {
                    Text = "معتمد",
                    Checked = true,
                    AutoSize = true
                };

                chkActive = new CheckBox
                {
                    Text = "نشط",
                    Checked = true,
                    AutoSize = true
                };

                var checksPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.RightToLeft
                };

                checksPanel.Controls.Add(chkApproved);
                checksPanel.Controls.Add(chkActive);

                btnSave = new Button
                {
                    Text = "حفظ",
                    Width = 100,
                    Height = 35,
                    BackColor = Color.FromArgb(0, 87, 183),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnSave.Click += BtnSave_Click;

                btnCancel = new Button
                {
                    Text = "إلغاء",
                    Width = 100,
                    Height = 35,
                    DialogResult = DialogResult.Cancel
                };

                var buttonsPanel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.RightToLeft
                };

                buttonsPanel.Controls.Add(btnSave);
                buttonsPanel.Controls.Add(btnCancel);

                root.Controls.Add(CreateLabel("اسم الجهاز"), 0, 0);
                root.Controls.Add(txtDeviceName, 1, 0);

                root.Controls.Add(CreateLabel("رقم الهاتف"), 0, 1);
                root.Controls.Add(txtPhoneNumber, 1, 1);

                root.Controls.Add(CreateLabel("كود الجهاز"), 0, 2);
                root.Controls.Add(codePanel, 1, 2);

                root.Controls.Add(CreateLabel("موديل الجهاز"), 0, 3);
                root.Controls.Add(txtDeviceModel, 1, 3);

                root.Controls.Add(CreateLabel("إصدار التطبيق"), 0, 4);
                root.Controls.Add(txtAppVersion, 1, 4);

                root.Controls.Add(CreateLabel("الحالة"), 0, 5);
                root.Controls.Add(checksPanel, 1, 5);

                root.Controls.Add(buttonsPanel, 1, 6);

                AcceptButton = btnSave;
                CancelButton = btnCancel;
            }

            private Label CreateLabel(string text)
            {
                return new Label
                {
                    Text = text,
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleRight,
                    Font = new Font("Tahoma", 10F, FontStyle.Bold)
                };
            }

            private TextBox CreateTextBox()
            {
                return new TextBox
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(5)
                };
            }

            private void BtnGenerateCode_Click(object sender, EventArgs e)
            {
                txtDeviceCode.Text = "DEV-" + Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
            }

            private void BtnSave_Click(object sender, EventArgs e)
            {
                if (string.IsNullOrWhiteSpace(txtDeviceCode.Text))
                {
                    MessageBox.Show("كود الجهاز مطلوب.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtDeviceCode.Focus();
                    return;
                }

                DeviceNameValue = txtDeviceName.Text.Trim();
                PhoneNumberValue = txtPhoneNumber.Text.Trim();
                DeviceCodeValue = txtDeviceCode.Text.Trim();
                DeviceModelValue = txtDeviceModel.Text.Trim();
                AppVersionValue = txtAppVersion.Text.Trim();
                IsApprovedValue = chkApproved.Checked;
                IsActiveValue = chkActive.Checked;

                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }