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
using System.Windows.Forms;
using water3.Theming;

namespace water3.Controls
{
    public partial class SubscriberEditorControl : UserControl
    {
 


            private readonly ToolTip _toolTip = new ToolTip
            {
                ShowAlways = true,
                InitialDelay = 150,
                ReshowDelay = 100,
                AutoPopDelay = 5000
            };

            public SubscriberEditorControl()
            {
                InitializeComponent();
                ApplyUi();
            }

            // ===== Exposed Controls =====
            public TextBox NameTextBox => txtName;
            public TextBox PhoneTextBox => txtPhone;
            public TextBox AddressTextBox => txtAddress;
            public ComboBox AccountComboBox => cboAccount;
            public TextBox MeterReadingTextBox => txtMeterReading;

            public TextBox NewMeterNumberTextBox => txtNewMeterNumber;
            public TextBox NewMeterLocationTextBox => txtNewMeterLocation;
            public CheckBox IsPrimaryCheckBox => chkIsPrimary;

            public Button AddButton => btnAdd;
            public Button UpdateButton => btnUpdate;
            public Button ClearButton => btnClear;
            public Button CreateAccountButton => btnCreateAccount;

            public Button AddMeterButton => btnAddMeter;
            public Button SetPrimaryButton => btnSetPrimary;
            public Button UnlinkMeterButton => btnUnlinkMeter;
            public Button ImportDataButton => btnImportData;
            public Button SaveImportedButton => btnSaveImported;

            private void ApplyUi()
            {
                ControlStyler.StyleCard(pnlCard);

                ControlStyler.StyleLabel(lblName, bold: true);
                ControlStyler.StyleLabel(lblPhone, bold: true);
                ControlStyler.StyleLabel(lblAddress, bold: true);
                ControlStyler.StyleLabel(lblAccount, bold: true);
                ControlStyler.StyleLabel(lblMeterReading, bold: true);
                ControlStyler.StyleLabel(lblMeterNo, bold: true);
                ControlStyler.StyleLabel(lblMeterLocation, bold: true);
                ControlStyler.StyleCheckBox(chkIsPrimary);

                ControlStyler.StyleTextBox(txtName);
                ControlStyler.StyleTextBox(txtPhone);
                ControlStyler.StyleTextBox(txtAddress);
                ControlStyler.StyleTextBox(txtMeterReading, center: true);
                ControlStyler.StyleTextBox(txtNewMeterNumber);
                ControlStyler.StyleTextBox(txtNewMeterLocation);
                ControlStyler.StyleComboBox(cboAccount);

                txtPhone.RightToLeft = RightToLeft.No;
                txtMeterReading.Text = "0";

                ControlStyler.StyleActionButton(btnAdd, AppTheme.Success);
                ControlStyler.StyleActionButton(btnUpdate, AppTheme.Primary);
                ControlStyler.StyleActionButton(btnClear, AppTheme.Gray);
                ControlStyler.StyleActionButton(btnCreateAccount, AppTheme.Warning);

                ControlStyler.StyleActionButton(btnAddMeter, AppTheme.Success);
                ControlStyler.StyleActionButton(btnSetPrimary, AppTheme.Purple);
                ControlStyler.StyleActionButton(btnUnlinkMeter, AppTheme.Danger);
                ControlStyler.StyleActionButton(btnImportData, AppTheme.DarkGray);
                ControlStyler.StyleActionButton(btnSaveImported, AppTheme.Primary);

                ControlStyler.SetButtonGlyph(btnAdd, "\uE710", "إضافة مشترك", _toolTip);
                ControlStyler.SetButtonGlyph(btnUpdate, "\uE70F", "تعديل بيانات المشترك", _toolTip);
                ControlStyler.SetButtonGlyph(btnClear, "\uE894", "تفريغ الحقول", _toolTip);
                ControlStyler.SetButtonGlyph(btnCreateAccount, "\uE77B", "إنشاء وربط حساب ذمة", _toolTip);

                ControlStyler.SetButtonGlyph(btnAddMeter, "\uE710", "إضافة عداد", _toolTip);
                ControlStyler.SetButtonGlyph(btnSetPrimary, "\uE734", "تعيين كعداد أساسي", _toolTip);
                ControlStyler.SetButtonGlyph(btnUnlinkMeter, "\uE711", "فك ربط العداد", _toolTip);
                ControlStyler.SetButtonGlyph(btnImportData, "\uE8E5", "استيراد بيانات من Excel", _toolTip);
                ControlStyler.SetButtonGlyph(btnSaveImported, "\uE74E", "حفظ البيانات المستوردة", _toolTip);

                ConfigureLayoutTweaks();
            }

            private void ConfigureLayoutTweaks()
            {
                var header = ControlStyler.BuildCardHeader(
                    "بيانات المشترك والعداد",
                    "إضافة وتعديل وربط العدادات والحسابات");

                pnlCard.Controls.Add(header);
                header.BringToFront();

                pnlButtons.FlowDirection = FlowDirection.RightToLeft;
                pnlButtons.WrapContents = false;
                pnlButtons.AutoScroll = false;
                pnlButtons.Padding = new Padding(0, 6, 0, 0);

                pnlMeterButtons.FlowDirection = FlowDirection.RightToLeft;
                pnlMeterButtons.WrapContents = false;
                pnlMeterButtons.AutoScroll = true;
                pnlMeterButtons.Padding = new Padding(0, 2, 0, 0);

                MinimumSize = new Size(1100, 290);
            }

            public void ToggleMeterButtons(bool enabled)
            {
                btnAddMeter.Enabled = enabled;
                btnSetPrimary.Enabled = enabled;
                btnUnlinkMeter.Enabled = enabled;
            }

            public void SetImportSaveEnabled(bool enabled)
            {
                btnSaveImported.Enabled = enabled;
            }

            public void ClearInputs(bool clearMeterReading = false)
            {
                txtName.Clear();
                txtPhone.Clear();
                txtAddress.Clear();
                txtNewMeterNumber.Clear();
                txtNewMeterLocation.Clear();
                chkIsPrimary.Checked = false;

                if (clearMeterReading)
                    txtMeterReading.Clear();

                if (cboAccount.Items.Count > 0)
                    cboAccount.SelectedIndex = 0;
            }
        }
    }