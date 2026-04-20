using System.Windows.Forms;
namespace water3.Controls
{

        partial class SubscriberEditorControl
        {
            private System.ComponentModel.IContainer components = null;

            private Panel pnlCard;
            private TableLayoutPanel layoutRoot;
            private TableLayoutPanel tblSubscriberFields;
            private TableLayoutPanel tblMeters;

            private Label lblName;
            private Label lblPhone;
            private Label lblAddress;
            private Label lblAccount;
            private Label lblMeterReading;
            private Label lblMeterNo;
            private Label lblMeterLocation;

            private TextBox txtName;
            private TextBox txtPhone;
            private TextBox txtAddress;
            private ComboBox cboAccount;
            private TextBox txtMeterReading;

            private TextBox txtNewMeterNumber;
            private TextBox txtNewMeterLocation;
            private CheckBox chkIsPrimary;

            private FlowLayoutPanel pnlButtons;
            private FlowLayoutPanel pnlMeterButtons;

            private Button btnAdd;
            private Button btnUpdate;
            private Button btnClear;
            private Button btnCreateAccount;

            private Button btnAddMeter;
            private Button btnSetPrimary;
            private Button btnUnlinkMeter;
            private Button btnImportData;
            private Button btnSaveImported;

            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                    components.Dispose();

                base.Dispose(disposing);
            }

            private void InitializeComponent()
            {
            this.pnlCard = new System.Windows.Forms.Panel();
            this.layoutRoot = new System.Windows.Forms.TableLayoutPanel();
            this.tblSubscriberFields = new System.Windows.Forms.TableLayoutPanel();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblAccount = new System.Windows.Forms.Label();
            this.cboAccount = new System.Windows.Forms.ComboBox();
            this.lblMeterReading = new System.Windows.Forms.Label();
            this.txtMeterReading = new System.Windows.Forms.TextBox();
            this.pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnCreateAccount = new System.Windows.Forms.Button();
            this.tblMeters = new System.Windows.Forms.TableLayoutPanel();
            this.lblMeterNo = new System.Windows.Forms.Label();
            this.txtNewMeterNumber = new System.Windows.Forms.TextBox();
            this.lblMeterLocation = new System.Windows.Forms.Label();
            this.txtNewMeterLocation = new System.Windows.Forms.TextBox();
            this.chkIsPrimary = new System.Windows.Forms.CheckBox();
            this.pnlMeterButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddMeter = new System.Windows.Forms.Button();
            this.btnSetPrimary = new System.Windows.Forms.Button();
            this.btnUnlinkMeter = new System.Windows.Forms.Button();
            this.btnImportData = new System.Windows.Forms.Button();
            this.btnSaveImported = new System.Windows.Forms.Button();
            this.pnlCard.SuspendLayout();
            this.layoutRoot.SuspendLayout();
            this.tblSubscriberFields.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.tblMeters.SuspendLayout();
            this.pnlMeterButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.Controls.Add(this.layoutRoot);
            this.pnlCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCard.Location = new System.Drawing.Point(0, 0);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(1180, 300);
            this.pnlCard.TabIndex = 0;
            // 
            // layoutRoot
            // 
            this.layoutRoot.ColumnCount = 1;
            this.layoutRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layoutRoot.Controls.Add(this.tblSubscriberFields, 0, 0);
            this.layoutRoot.Controls.Add(this.tblMeters, 0, 1);
            this.layoutRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layoutRoot.Location = new System.Drawing.Point(0, 0);
            this.layoutRoot.Name = "layoutRoot";
            this.layoutRoot.Padding = new System.Windows.Forms.Padding(0, 52, 0, 0);
            this.layoutRoot.RowCount = 2;
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.layoutRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this.layoutRoot.Size = new System.Drawing.Size(1180, 300);
            this.layoutRoot.TabIndex = 0;
            // 
            // tblSubscriberFields
            // 
            this.tblSubscriberFields.ColumnCount = 6;
            this.tblSubscriberFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tblSubscriberFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36F));
            this.tblSubscriberFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tblSubscriberFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24F));
            this.tblSubscriberFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tblSubscriberFields.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tblSubscriberFields.Controls.Add(this.lblName, 0, 0);
            this.tblSubscriberFields.Controls.Add(this.txtName, 1, 0);
            this.tblSubscriberFields.Controls.Add(this.lblPhone, 2, 0);
            this.tblSubscriberFields.Controls.Add(this.txtPhone, 3, 0);
            this.tblSubscriberFields.Controls.Add(this.lblAddress, 4, 0);
            this.tblSubscriberFields.Controls.Add(this.txtAddress, 5, 0);
            this.tblSubscriberFields.Controls.Add(this.lblAccount, 0, 1);
            this.tblSubscriberFields.Controls.Add(this.cboAccount, 1, 1);
            this.tblSubscriberFields.Controls.Add(this.lblMeterReading, 2, 1);
            this.tblSubscriberFields.Controls.Add(this.txtMeterReading, 3, 1);
            this.tblSubscriberFields.Controls.Add(this.pnlButtons, 4, 1);
            this.tblSubscriberFields.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblSubscriberFields.Location = new System.Drawing.Point(3, 55);
            this.tblSubscriberFields.Name = "tblSubscriberFields";
            this.tblSubscriberFields.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.tblSubscriberFields.RowCount = 2;
            this.tblSubscriberFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 48F));
            this.tblSubscriberFields.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tblSubscriberFields.Size = new System.Drawing.Size(1174, 106);
            this.tblSubscriberFields.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblName.Location = new System.Drawing.Point(29, 19);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(63, 18);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "الاسم";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(98, 7);
            this.txtName.Multiline = true;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(314, 42);
            this.txtName.TabIndex = 1;
            // 
            // lblPhone
            // 
            this.lblPhone.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPhone.Location = new System.Drawing.Point(444, 19);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(63, 18);
            this.lblPhone.TabIndex = 2;
            this.lblPhone.Text = "الهاتف";
            this.lblPhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPhone
            // 
            this.txtPhone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPhone.Location = new System.Drawing.Point(513, 7);
            this.txtPhone.Multiline = true;
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(207, 42);
            this.txtPhone.TabIndex = 3;
            // 
            // lblAddress
            // 
            this.lblAddress.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAddress.Location = new System.Drawing.Point(752, 19);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(63, 18);
            this.lblAddress.TabIndex = 4;
            this.lblAddress.Text = "العنوان";
            this.lblAddress.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAddress
            // 
            this.txtAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAddress.Location = new System.Drawing.Point(821, 7);
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(350, 42);
            this.txtAddress.TabIndex = 5;
            // 
            // lblAccount
            // 
            this.lblAccount.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblAccount.Location = new System.Drawing.Point(3, 71);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(89, 18);
            this.lblAccount.TabIndex = 6;
            this.lblAccount.Text = "حساب الذمة";
            this.lblAccount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboAccount
            // 
            this.cboAccount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboAccount.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAccount.FormattingEnabled = true;
            this.cboAccount.Location = new System.Drawing.Point(98, 55);
            this.cboAccount.Name = "cboAccount";
            this.cboAccount.Size = new System.Drawing.Size(314, 21);
            this.cboAccount.TabIndex = 7;
            // 
            // lblMeterReading
            // 
            this.lblMeterReading.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblMeterReading.Location = new System.Drawing.Point(419, 71);
            this.lblMeterReading.Name = "lblMeterReading";
            this.lblMeterReading.Size = new System.Drawing.Size(88, 18);
            this.lblMeterReading.TabIndex = 8;
            this.lblMeterReading.Text = "قراءة العداد";
            this.lblMeterReading.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtMeterReading
            // 
            this.txtMeterReading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMeterReading.Location = new System.Drawing.Point(513, 55);
            this.txtMeterReading.Multiline = true;
            this.txtMeterReading.Name = "txtMeterReading";
            this.txtMeterReading.Size = new System.Drawing.Size(207, 50);
            this.txtMeterReading.TabIndex = 9;
            // 
            // pnlButtons
            // 
            this.tblSubscriberFields.SetColumnSpan(this.pnlButtons, 2);
            this.pnlButtons.Controls.Add(this.btnAdd);
            this.pnlButtons.Controls.Add(this.btnUpdate);
            this.pnlButtons.Controls.Add(this.btnClear);
            this.pnlButtons.Controls.Add(this.btnCreateAccount);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlButtons.Location = new System.Drawing.Point(726, 55);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(445, 50);
            this.pnlButtons.TabIndex = 10;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(3, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(42, 42);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(51, 3);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(42, 42);
            this.btnUpdate.TabIndex = 1;
            this.btnUpdate.UseVisualStyleBackColor = true;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(99, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(42, 42);
            this.btnClear.TabIndex = 2;
            this.btnClear.UseVisualStyleBackColor = true;
            // 
            // btnCreateAccount
            // 
            this.btnCreateAccount.Location = new System.Drawing.Point(147, 3);
            this.btnCreateAccount.Name = "btnCreateAccount";
            this.btnCreateAccount.Size = new System.Drawing.Size(42, 42);
            this.btnCreateAccount.TabIndex = 3;
            this.btnCreateAccount.UseVisualStyleBackColor = true;
            // 
            // tblMeters
            // 
            this.tblMeters.ColumnCount = 6;
            this.tblMeters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tblMeters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 24F));
            this.tblMeters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 98F));
            this.tblMeters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32F));
            this.tblMeters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.tblMeters.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44F));
            this.tblMeters.Controls.Add(this.lblMeterNo, 0, 0);
            this.tblMeters.Controls.Add(this.txtNewMeterNumber, 1, 0);
            this.tblMeters.Controls.Add(this.lblMeterLocation, 2, 0);
            this.tblMeters.Controls.Add(this.txtNewMeterLocation, 3, 0);
            this.tblMeters.Controls.Add(this.chkIsPrimary, 4, 0);
            this.tblMeters.Controls.Add(this.pnlMeterButtons, 5, 0);
            this.tblMeters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMeters.Location = new System.Drawing.Point(3, 167);
            this.tblMeters.Name = "tblMeters";
            this.tblMeters.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.tblMeters.RowCount = 1;
            this.tblMeters.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            this.tblMeters.Size = new System.Drawing.Size(1174, 130);
            this.tblMeters.TabIndex = 1;
            // 
            // lblMeterNo
            // 
            this.lblMeterNo.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblMeterNo.Location = new System.Drawing.Point(3, 60);
            this.lblMeterNo.Name = "lblMeterNo";
            this.lblMeterNo.Size = new System.Drawing.Size(89, 18);
            this.lblMeterNo.TabIndex = 0;
            this.lblMeterNo.Text = "رقم العداد";
            this.lblMeterNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNewMeterNumber
            // 
            this.txtNewMeterNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNewMeterNumber.Location = new System.Drawing.Point(98, 11);
            this.txtNewMeterNumber.Name = "txtNewMeterNumber";
            this.txtNewMeterNumber.Size = new System.Drawing.Size(207, 20);
            this.txtNewMeterNumber.TabIndex = 1;
            // 
            // lblMeterLocation
            // 
            this.lblMeterLocation.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblMeterLocation.Location = new System.Drawing.Point(324, 60);
            this.lblMeterLocation.Name = "lblMeterLocation";
            this.lblMeterLocation.Size = new System.Drawing.Size(79, 18);
            this.lblMeterLocation.TabIndex = 2;
            this.lblMeterLocation.Text = "الموقع";
            this.lblMeterLocation.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNewMeterLocation
            // 
            this.txtNewMeterLocation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNewMeterLocation.Location = new System.Drawing.Point(409, 11);
            this.txtNewMeterLocation.Name = "txtNewMeterLocation";
            this.txtNewMeterLocation.Size = new System.Drawing.Size(279, 20);
            this.txtNewMeterLocation.TabIndex = 3;
            // 
            // chkIsPrimary
            // 
            this.chkIsPrimary.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.chkIsPrimary.AutoSize = true;
            this.chkIsPrimary.Location = new System.Drawing.Point(704, 60);
            this.chkIsPrimary.Name = "chkIsPrimary";
            this.chkIsPrimary.Size = new System.Drawing.Size(63, 17);
            this.chkIsPrimary.TabIndex = 4;
            this.chkIsPrimary.Text = "أساسي";
            this.chkIsPrimary.UseVisualStyleBackColor = true;
            // 
            // pnlMeterButtons
            // 
            this.pnlMeterButtons.Controls.Add(this.btnAddMeter);
            this.pnlMeterButtons.Controls.Add(this.btnSetPrimary);
            this.pnlMeterButtons.Controls.Add(this.btnUnlinkMeter);
            this.pnlMeterButtons.Controls.Add(this.btnImportData);
            this.pnlMeterButtons.Controls.Add(this.btnSaveImported);
            this.pnlMeterButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMeterButtons.Location = new System.Drawing.Point(784, 11);
            this.pnlMeterButtons.Name = "pnlMeterButtons";
            this.pnlMeterButtons.Size = new System.Drawing.Size(387, 116);
            this.pnlMeterButtons.TabIndex = 5;
            // 
            // btnAddMeter
            // 
            this.btnAddMeter.Location = new System.Drawing.Point(3, 3);
            this.btnAddMeter.Name = "btnAddMeter";
            this.btnAddMeter.Size = new System.Drawing.Size(42, 42);
            this.btnAddMeter.TabIndex = 0;
            this.btnAddMeter.UseVisualStyleBackColor = true;
            // 
            // btnSetPrimary
            // 
            this.btnSetPrimary.Location = new System.Drawing.Point(51, 3);
            this.btnSetPrimary.Name = "btnSetPrimary";
            this.btnSetPrimary.Size = new System.Drawing.Size(42, 42);
            this.btnSetPrimary.TabIndex = 1;
            this.btnSetPrimary.UseVisualStyleBackColor = true;
            // 
            // btnUnlinkMeter
            // 
            this.btnUnlinkMeter.Location = new System.Drawing.Point(99, 3);
            this.btnUnlinkMeter.Name = "btnUnlinkMeter";
            this.btnUnlinkMeter.Size = new System.Drawing.Size(42, 42);
            this.btnUnlinkMeter.TabIndex = 2;
            this.btnUnlinkMeter.UseVisualStyleBackColor = true;
            // 
            // btnImportData
            // 
            this.btnImportData.Location = new System.Drawing.Point(147, 3);
            this.btnImportData.Name = "btnImportData";
            this.btnImportData.Size = new System.Drawing.Size(42, 42);
            this.btnImportData.TabIndex = 3;
            this.btnImportData.UseVisualStyleBackColor = true;
            // 
            // btnSaveImported
            // 
            this.btnSaveImported.Location = new System.Drawing.Point(195, 3);
            this.btnSaveImported.Name = "btnSaveImported";
            this.btnSaveImported.Size = new System.Drawing.Size(42, 42);
            this.btnSaveImported.TabIndex = 4;
            this.btnSaveImported.UseVisualStyleBackColor = true;
            // 
            // SubscriberEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.pnlCard);
            this.Name = "SubscriberEditorControl";
            this.Size = new System.Drawing.Size(1180, 300);
            this.pnlCard.ResumeLayout(false);
            this.layoutRoot.ResumeLayout(false);
            this.tblSubscriberFields.ResumeLayout(false);
            this.tblSubscriberFields.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.tblMeters.ResumeLayout(false);
            this.tblMeters.PerformLayout();
            this.pnlMeterButtons.ResumeLayout(false);
            this.ResumeLayout(false);

            }
        }
    }