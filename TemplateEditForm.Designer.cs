namespace water3
{
    partial class TemplateEditForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.TableLayoutPanel layout;

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;

        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cmbType;

        private System.Windows.Forms.Label lblLang;
        private System.Windows.Forms.ComboBox cmbLang;

        private System.Windows.Forms.Label lblVars;
        private System.Windows.Forms.ComboBox cmbVars;
        private System.Windows.Forms.Button btnInsertVar;

        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.TextBox txtText;

        private System.Windows.Forms.CheckBox chkActive;

        private System.Windows.Forms.Label lblChars;
        private System.Windows.Forms.Label lblSmsHint;

        private System.Windows.Forms.FlowLayoutPanel pnlButtons;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSave;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlMain = new System.Windows.Forms.Panel();
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblType = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.lblLang = new System.Windows.Forms.Label();
            this.cmbLang = new System.Windows.Forms.ComboBox();
            this.lblVars = new System.Windows.Forms.Label();
            this.pnlVars = new System.Windows.Forms.FlowLayoutPanel();
            this.btnInsertVar = new System.Windows.Forms.Button();
            this.cmbVars = new System.Windows.Forms.ComboBox();
            this.lblText = new System.Windows.Forms.Label();
            this.txtText = new System.Windows.Forms.TextBox();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.lblChars = new System.Windows.Forms.Label();
            this.lblSmsHint = new System.Windows.Forms.Label();
            this.pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.pnlMain.SuspendLayout();
            this.layout.SuspendLayout();
            this.pnlVars.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.layout);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(18);
            this.pnlMain.Size = new System.Drawing.Size(720, 600);
            this.pnlMain.TabIndex = 0;
            // 
            // layout
            // 
            this.layout.ColumnCount = 2;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.lblName, 0, 0);
            this.layout.Controls.Add(this.txtName, 1, 0);
            this.layout.Controls.Add(this.lblType, 0, 1);
            this.layout.Controls.Add(this.cmbType, 1, 1);
            this.layout.Controls.Add(this.lblLang, 0, 2);
            this.layout.Controls.Add(this.cmbLang, 1, 2);
            this.layout.Controls.Add(this.lblVars, 0, 3);
            this.layout.Controls.Add(this.pnlVars, 1, 3);
            this.layout.Controls.Add(this.lblText, 0, 4);
            this.layout.Controls.Add(this.txtText, 1, 4);
            this.layout.Controls.Add(this.chkActive, 1, 5);
            this.layout.Controls.Add(this.lblChars, 1, 6);
            this.layout.Controls.Add(this.lblSmsHint, 1, 7);
            this.layout.Controls.Add(this.pnlButtons, 1, 8);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(18, 18);
            this.layout.Name = "layout";
            this.layout.RowCount = 9;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.layout.Size = new System.Drawing.Size(684, 564);
            this.layout.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(557, 11);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(56, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "اسم القالب:";
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(3, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(548, 20);
            this.txtName.TabIndex = 1;
            // 
            // lblType
            // 
            this.lblType.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(557, 47);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(56, 13);
            this.lblType.TabIndex = 2;
            this.lblType.Text = "نوع القالب:";
            // 
            // cmbType
            // 
            this.cmbType.Dock = System.Windows.Forms.DockStyle.Left;
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.Location = new System.Drawing.Point(331, 39);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(220, 21);
            this.cmbType.TabIndex = 3;
            // 
            // lblLang
            // 
            this.lblLang.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblLang.AutoSize = true;
            this.lblLang.Location = new System.Drawing.Point(557, 83);
            this.lblLang.Name = "lblLang";
            this.lblLang.Size = new System.Drawing.Size(30, 13);
            this.lblLang.TabIndex = 4;
            this.lblLang.Text = "اللغة:";
            // 
            // cmbLang
            // 
            this.cmbLang.Dock = System.Windows.Forms.DockStyle.Left;
            this.cmbLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLang.Location = new System.Drawing.Point(431, 75);
            this.cmbLang.Name = "cmbLang";
            this.cmbLang.Size = new System.Drawing.Size(120, 21);
            this.cmbLang.TabIndex = 5;
            // 
            // lblVars
            // 
            this.lblVars.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblVars.AutoSize = true;
            this.lblVars.Location = new System.Drawing.Point(557, 121);
            this.lblVars.Name = "lblVars";
            this.lblVars.Size = new System.Drawing.Size(51, 13);
            this.lblVars.TabIndex = 6;
            this.lblVars.Text = "المتغيرات:";
            // 
            // pnlVars
            // 
            this.pnlVars.Controls.Add(this.btnInsertVar);
            this.pnlVars.Controls.Add(this.cmbVars);
            this.pnlVars.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlVars.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlVars.Location = new System.Drawing.Point(3, 111);
            this.pnlVars.Name = "pnlVars";
            this.pnlVars.Size = new System.Drawing.Size(548, 34);
            this.pnlVars.TabIndex = 7;
            this.pnlVars.WrapContents = false;
            // 
            // btnInsertVar
            // 
            this.btnInsertVar.Location = new System.Drawing.Point(0, 6);
            this.btnInsertVar.Margin = new System.Windows.Forms.Padding(8, 6, 0, 0);
            this.btnInsertVar.Name = "btnInsertVar";
            this.btnInsertVar.Size = new System.Drawing.Size(80, 26);
            this.btnInsertVar.TabIndex = 0;
            this.btnInsertVar.Text = "إدراج";
            // 
            // cmbVars
            // 
            this.cmbVars.Dock = System.Windows.Forms.DockStyle.Left;
            this.cmbVars.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbVars.Location = new System.Drawing.Point(91, 3);
            this.cmbVars.Name = "cmbVars";
            this.cmbVars.Size = new System.Drawing.Size(220, 21);
            this.cmbVars.TabIndex = 1;
            // 
            // lblText
            // 
            this.lblText.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(557, 281);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(54, 13);
            this.lblText.TabIndex = 8;
            this.lblText.Text = "نص القالب:";
            // 
            // txtText
            // 
            this.txtText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtText.Location = new System.Drawing.Point(3, 151);
            this.txtText.Multiline = true;
            this.txtText.Name = "txtText";
            this.txtText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtText.Size = new System.Drawing.Size(548, 273);
            this.txtText.TabIndex = 9;
            // 
            // chkActive
            // 
            this.chkActive.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.chkActive.Location = new System.Drawing.Point(3, 430);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(104, 24);
            this.chkActive.TabIndex = 10;
            this.chkActive.Text = "مفعل";
            // 
            // lblChars
            // 
            this.lblChars.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblChars.Location = new System.Drawing.Point(3, 457);
            this.lblChars.Name = "lblChars";
            this.lblChars.Size = new System.Drawing.Size(548, 26);
            this.lblChars.TabIndex = 11;
            this.lblChars.Text = "الأحرف: 0";
            this.lblChars.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblSmsHint
            // 
            this.lblSmsHint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSmsHint.Location = new System.Drawing.Point(3, 483);
            this.lblSmsHint.Name = "lblSmsHint";
            this.lblSmsHint.Size = new System.Drawing.Size(548, 26);
            this.lblSmsHint.TabIndex = 12;
            this.lblSmsHint.Text = "SMS: ---";
            this.lblSmsHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnPreview);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlButtons.Location = new System.Drawing.Point(3, 512);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.pnlButtons.Size = new System.Drawing.Size(548, 49);
            this.pnlButtons.TabIndex = 13;
            this.pnlButtons.WrapContents = false;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.SeaGreen;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(3, 13);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 34);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "حفظ";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.IndianRed;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(119, 13);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 34);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "إلغاء";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(235, 13);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(110, 34);
            this.btnPreview.TabIndex = 2;
            this.btnPreview.Text = "معاينة";
            // 
            // TemplateEditForm
            // 
            this.AcceptButton = this.btnSave;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(720, 600);
            this.Controls.Add(this.pnlMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TemplateEditForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TemplateEditForm";
            this.pnlMain.ResumeLayout(false);
            this.layout.ResumeLayout(false);
            this.layout.PerformLayout();
            this.pnlVars.ResumeLayout(false);
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.FlowLayoutPanel pnlVars;
    }
}
/*namespace water3
{
    partial class TemplateEditForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.TableLayoutPanel layout;

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;

        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cmbType;

        private System.Windows.Forms.Label lblLang;
        private System.Windows.Forms.ComboBox cmbLang;

        private System.Windows.Forms.Label lblText;
        private System.Windows.Forms.TextBox txtText;

        private System.Windows.Forms.CheckBox chkActive;

        private System.Windows.Forms.FlowLayoutPanel pnlButtons;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlMain = new System.Windows.Forms.Panel();
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblType = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.lblLang = new System.Windows.Forms.Label();
            this.cmbLang = new System.Windows.Forms.ComboBox();
            this.lblText = new System.Windows.Forms.Label();
            this.txtText = new System.Windows.Forms.TextBox();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.pnlMain.SuspendLayout();
            this.layout.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.layout);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(20);
            this.pnlMain.Size = new System.Drawing.Size(600, 520);
            this.pnlMain.TabIndex = 0;
            // 
            // layout
            // 
            this.layout.ColumnCount = 2;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.lblName, 0, 0);
            this.layout.Controls.Add(this.txtName, 1, 0);
            this.layout.Controls.Add(this.lblType, 0, 1);
            this.layout.Controls.Add(this.cmbType, 1, 1);
            this.layout.Controls.Add(this.lblLang, 0, 2);
            this.layout.Controls.Add(this.cmbLang, 1, 2);
            this.layout.Controls.Add(this.lblText, 0, 3);
            this.layout.Controls.Add(this.txtText, 1, 3);
            this.layout.Controls.Add(this.chkActive, 1, 4);
            this.layout.Controls.Add(this.pnlButtons, 1, 5);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(20, 20);
            this.layout.Name = "layout";
            this.layout.RowCount = 6;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.layout.Size = new System.Drawing.Size(560, 480);
            this.layout.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(450, 15);
            this.lblName.Margin = new System.Windows.Forms.Padding(0, 8, 10, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(64, 14);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "اسم القالب";
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(0, 6);
            this.txtName.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(440, 22);
            this.txtName.TabIndex = 1;
            // 
            // lblType
            // 
            this.lblType.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblType.AutoSize = true;
            this.lblType.Location = new System.Drawing.Point(450, 51);
            this.lblType.Margin = new System.Windows.Forms.Padding(0, 8, 10, 0);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(56, 14);
            this.lblType.TabIndex = 2;
            this.lblType.Text = "نوع القالب";
            // 
            // cmbType
            // 
            this.cmbType.Dock = System.Windows.Forms.DockStyle.Left;
            this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbType.Location = new System.Drawing.Point(180, 42);
            this.cmbType.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(260, 22);
            this.cmbType.TabIndex = 3;
            // 
            // lblLang
            // 
            this.lblLang.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblLang.AutoSize = true;
            this.lblLang.Location = new System.Drawing.Point(450, 87);
            this.lblLang.Margin = new System.Windows.Forms.Padding(0, 8, 10, 0);
            this.lblLang.Name = "lblLang";
            this.lblLang.Size = new System.Drawing.Size(30, 14);
            this.lblLang.TabIndex = 4;
            this.lblLang.Text = "اللغة";
            // 
            // cmbLang
            // 
            this.cmbLang.Dock = System.Windows.Forms.DockStyle.Left;
            this.cmbLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLang.Location = new System.Drawing.Point(320, 78);
            this.cmbLang.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.cmbLang.Name = "cmbLang";
            this.cmbLang.Size = new System.Drawing.Size(120, 22);
            this.cmbLang.TabIndex = 5;
            // 
            // lblText
            // 
            this.lblText.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblText.AutoSize = true;
            this.lblText.Location = new System.Drawing.Point(450, 246);
            this.lblText.Margin = new System.Windows.Forms.Padding(0, 8, 10, 0);
            this.lblText.Name = "lblText";
            this.lblText.Size = new System.Drawing.Size(58, 14);
            this.lblText.TabIndex = 6;
            this.lblText.Text = "نص القالب";
            // 
            // txtText
            // 
            this.txtText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtText.Location = new System.Drawing.Point(0, 114);
            this.txtText.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.txtText.Multiline = true;
            this.txtText.Name = "txtText";
            this.txtText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtText.Size = new System.Drawing.Size(440, 277);
            this.txtText.TabIndex = 7;
            // 
            // chkActive
            // 
            this.chkActive.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.chkActive.Location = new System.Drawing.Point(0, 398);
            this.chkActive.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(104, 24);
            this.chkActive.TabIndex = 8;
            this.chkActive.Text = "مفعل";
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlButtons.Location = new System.Drawing.Point(3, 428);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.pnlButtons.Size = new System.Drawing.Size(434, 49);
            this.pnlButtons.TabIndex = 9;
            this.pnlButtons.WrapContents = false;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.IndianRed;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(3, 11);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 35);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "إلغاء";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.SeaGreen;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(126, 8);
            this.btnSave.Margin = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 35);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "حفظ";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // TemplateEditForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(600, 520);
            this.Controls.Add(this.pnlMain);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TemplateEditForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TemplateEditForm";
            this.pnlMain.ResumeLayout(false);
            this.layout.ResumeLayout(false);
            this.layout.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
*/