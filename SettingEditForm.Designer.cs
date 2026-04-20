namespace water3
{
    partial class SettingEditForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TableLayoutPanel layout;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.TextBox txtValue;
        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.TextBox txtDesc;
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
            this.layout = new System.Windows.Forms.TableLayoutPanel();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.txtValue = new System.Windows.Forms.TextBox();
            this.lblDesc = new System.Windows.Forms.Label();
            this.txtDesc = new System.Windows.Forms.TextBox();
            this.pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.layout.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // layout
            // 
            this.layout.ColumnCount = 2;
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.layout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.Controls.Add(this.lblName, 0, 0);
            this.layout.Controls.Add(this.txtName, 1, 0);
            this.layout.Controls.Add(this.lblValue, 0, 1);
            this.layout.Controls.Add(this.txtValue, 1, 1);
            this.layout.Controls.Add(this.lblDesc, 0, 2);
            this.layout.Controls.Add(this.txtDesc, 1, 2);
            this.layout.Controls.Add(this.pnlButtons, 1, 3);
            this.layout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.layout.Location = new System.Drawing.Point(0, 0);
            this.layout.Name = "layout";
            this.layout.Padding = new System.Windows.Forms.Padding(16);
            this.layout.RowCount = 4;
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.layout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.layout.Size = new System.Drawing.Size(560, 300);
            this.layout.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(427, 27);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(59, 13);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "اسم الإعداد:";
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(19, 19);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(402, 20);
            this.txtName.TabIndex = 1;
            // 
            // lblValue
            // 
            this.lblValue.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(427, 63);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(35, 13);
            this.lblValue.TabIndex = 2;
            this.lblValue.Text = "القيمة:";
            // 
            // txtValue
            // 
            this.txtValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtValue.Location = new System.Drawing.Point(19, 55);
            this.txtValue.Name = "txtValue";
            this.txtValue.Size = new System.Drawing.Size(402, 20);
            this.txtValue.TabIndex = 3;
            // 
            // lblDesc
            // 
            this.lblDesc.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDesc.AutoSize = true;
            this.lblDesc.Location = new System.Drawing.Point(427, 152);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(40, 13);
            this.lblDesc.TabIndex = 4;
            this.lblDesc.Text = "الوصف:";
            // 
            // txtDesc
            // 
            this.txtDesc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtDesc.Location = new System.Drawing.Point(19, 91);
            this.txtDesc.Multiline = true;
            this.txtDesc.Name = "txtDesc";
            this.txtDesc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDesc.Size = new System.Drawing.Size(402, 135);
            this.txtDesc.TabIndex = 5;
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnCancel);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlButtons.Location = new System.Drawing.Point(19, 232);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.pnlButtons.Size = new System.Drawing.Size(402, 49);
            this.pnlButtons.TabIndex = 6;
            this.pnlButtons.WrapContents = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(3, 13);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(110, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "حفظ";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(119, 13);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(110, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "إلغاء";
            // 
            // SettingEditForm
            // 
            this.ClientSize = new System.Drawing.Size(560, 300);
            this.Controls.Add(this.layout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingEditForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SettingEditForm";
            this.layout.ResumeLayout(false);
            this.layout.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
/*namespace water3
{
    partial class SettingEditForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Label lblSettingName;
        private System.Windows.Forms.Label lblSettingValue;
        private System.Windows.Forms.Label lblDescription;

        private System.Windows.Forms.TextBox txtSettingName;
        private System.Windows.Forms.TextBox txtSettingValue;
        private System.Windows.Forms.TextBox txtDescription;

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.panelMain = new System.Windows.Forms.Panel();
            this.lblSettingName = new System.Windows.Forms.Label();
            this.lblSettingValue = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();

            this.txtSettingName = new System.Windows.Forms.TextBox();
            this.txtSettingValue = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();

            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            this.panelMain.SuspendLayout();
            this.SuspendLayout();

            // ===== Form =====
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 300);
            this.Name = "SettingEditForm";
            this.Text = "SettingEditForm";

            // ===== panelMain =====
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Padding = new System.Windows.Forms.Padding(20);

            // ===== lblSettingName =====
            this.lblSettingName.Text = "اسم الإعداد:";
            this.lblSettingName.Location = new System.Drawing.Point(20, 20);
            this.lblSettingName.Size = new System.Drawing.Size(100, 25);
            this.lblSettingName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // ===== txtSettingName =====
            this.txtSettingName.Location = new System.Drawing.Point(130, 20);
            this.txtSettingName.Size = new System.Drawing.Size(300, 25);

            // ===== lblSettingValue =====
            this.lblSettingValue.Text = "القيمة:";
            this.lblSettingValue.Location = new System.Drawing.Point(20, 60);
            this.lblSettingValue.Size = new System.Drawing.Size(100, 25);
            this.lblSettingValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // ===== txtSettingValue =====
            this.txtSettingValue.Location = new System.Drawing.Point(130, 60);
            this.txtSettingValue.Size = new System.Drawing.Size(300, 25);

            // ===== lblDescription =====
            this.lblDescription.Text = "الوصف:";
            this.lblDescription.Location = new System.Drawing.Point(20, 100);
            this.lblDescription.Size = new System.Drawing.Size(100, 25);
            this.lblDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            // ===== txtDescription =====
            this.txtDescription.Location = new System.Drawing.Point(130, 100);
            this.txtDescription.Size = new System.Drawing.Size(300, 60);
            this.txtDescription.Multiline = true;

            // ===== btnSave =====
            this.btnSave.Text = "حفظ";
            this.btnSave.Location = new System.Drawing.Point(200, 190);
            this.btnSave.Size = new System.Drawing.Size(100, 35);
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // ===== btnCancel =====
            this.btnCancel.Text = "إلغاء";
            this.btnCancel.Location = new System.Drawing.Point(310, 190);
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // ===== add to panel =====
            this.panelMain.Controls.Add(this.lblSettingName);
            this.panelMain.Controls.Add(this.txtSettingName);

            this.panelMain.Controls.Add(this.lblSettingValue);
            this.panelMain.Controls.Add(this.txtSettingValue);

            this.panelMain.Controls.Add(this.lblDescription);
            this.panelMain.Controls.Add(this.txtDescription);

            this.panelMain.Controls.Add(this.btnSave);
            this.panelMain.Controls.Add(this.btnCancel);

            // ===== add to form =====
            this.Controls.Add(this.panelMain);

            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}
*/