namespace water3
{
    partial class CollectorsForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.StatusStrip status;
        private System.Windows.Forms.ToolStripStatusLabel stCount;
        private System.Windows.Forms.ToolStripStatusLabel stSpring;
        private System.Windows.Forms.ToolStripStatusLabel stSelected;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblSearchHint;

        private System.Windows.Forms.Panel pnlForm;
        private System.Windows.Forms.TableLayoutPanel formLayout;
        private System.Windows.Forms.Label lblId;
        private System.Windows.Forms.Label lblIdValue;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.FlowLayoutPanel pnlButtons;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnNew;
        private System.Windows.Forms.Button btnRefresh;

        private System.Windows.Forms.DataGridView grid;

        private void InitializeComponent()
        {
            this.status = new System.Windows.Forms.StatusStrip();
            this.stCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.stSpring = new System.Windows.Forms.ToolStripStatusLabel();
            this.stSelected = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblSearchHint = new System.Windows.Forms.Label();
            this.pnlForm = new System.Windows.Forms.Panel();
            this.formLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblId = new System.Windows.Forms.Label();
            this.lblIdValue = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnNew = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.grid = new System.Windows.Forms.DataGridView();
            this.status.SuspendLayout();
            this.mainLayout.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.pnlForm.SuspendLayout();
            this.formLayout.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            this.SuspendLayout();
            // 
            // status
            // 
            this.status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stCount,
            this.stSpring,
            this.stSelected});
            this.status.Location = new System.Drawing.Point(0, 598);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(980, 22);
            this.status.TabIndex = 1;
            // 
            // stCount
            // 
            this.stCount.Name = "stCount";
            this.stCount.Size = new System.Drawing.Size(45, 17);
            this.stCount.Text = "العدد: 0";
            // 
            // stSpring
            // 
            this.stSpring.Name = "stSpring";
            this.stSpring.Size = new System.Drawing.Size(869, 17);
            this.stSpring.Spring = true;
            // 
            // stSelected
            // 
            this.stSelected.Name = "stSelected";
            this.stSelected.Size = new System.Drawing.Size(51, 17);
            this.stSelected.Text = "المحدد: -";
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.pnlTop, 0, 0);
            this.mainLayout.Controls.Add(this.pnlForm, 0, 1);
            this.mainLayout.Controls.Add(this.grid, 0, 2);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new System.Windows.Forms.Padding(10);
            this.mainLayout.RowCount = 3;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Size = new System.Drawing.Size(980, 598);
            this.mainLayout.TabIndex = 0;
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.txtSearch);
            this.pnlTop.Controls.Add(this.lblSearchHint);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTop.Location = new System.Drawing.Point(13, 13);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(12);
            this.pnlTop.Size = new System.Drawing.Size(954, 49);
            this.pnlTop.TabIndex = 0;
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Right;
            this.txtSearch.Location = new System.Drawing.Point(510, 12);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(320, 20);
            this.txtSearch.TabIndex = 0;
            // 
            // lblSearchHint
            // 
            this.lblSearchHint.AutoSize = true;
            this.lblSearchHint.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblSearchHint.Location = new System.Drawing.Point(830, 12);
            this.lblSearchHint.Name = "lblSearchHint";
            this.lblSearchHint.Size = new System.Drawing.Size(112, 13);
            this.lblSearchHint.TabIndex = 1;
            this.lblSearchHint.Text = "بحث بالاسم أو الهاتف...";
            this.lblSearchHint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlForm
            // 
            this.pnlForm.Controls.Add(this.formLayout);
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlForm.Location = new System.Drawing.Point(13, 68);
            this.pnlForm.Name = "pnlForm";
            this.pnlForm.Padding = new System.Windows.Forms.Padding(12);
            this.pnlForm.Size = new System.Drawing.Size(954, 114);
            this.pnlForm.TabIndex = 1;
            // 
            // formLayout
            // 
            this.formLayout.ColumnCount = 6;
            this.formLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.formLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.formLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.formLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.formLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.formLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.formLayout.Controls.Add(this.lblId, 0, 0);
            this.formLayout.Controls.Add(this.lblIdValue, 1, 0);
            this.formLayout.Controls.Add(this.lblName, 2, 0);
            this.formLayout.Controls.Add(this.txtName, 3, 0);
            this.formLayout.Controls.Add(this.lblPhone, 4, 0);
            this.formLayout.Controls.Add(this.txtPhone, 5, 0);
            this.formLayout.Controls.Add(this.pnlButtons, 0, 1);
            this.formLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formLayout.Location = new System.Drawing.Point(12, 12);
            this.formLayout.Name = "formLayout";
            this.formLayout.RowCount = 2;
            this.formLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.formLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.formLayout.Size = new System.Drawing.Size(930, 90);
            this.formLayout.TabIndex = 0;
            // 
            // lblId
            // 
            this.lblId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblId.Location = new System.Drawing.Point(863, 0);
            this.lblId.Name = "lblId";
            this.lblId.Size = new System.Drawing.Size(64, 35);
            this.lblId.TabIndex = 0;
            this.lblId.Text = "ID:";
            this.lblId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblIdValue
            // 
            this.lblIdValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblIdValue.Location = new System.Drawing.Point(783, 0);
            this.lblIdValue.Name = "lblIdValue";
            this.lblIdValue.Size = new System.Drawing.Size(74, 35);
            this.lblIdValue.TabIndex = 1;
            this.lblIdValue.Text = "-";
            this.lblIdValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblName
            // 
            this.lblName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblName.Location = new System.Drawing.Point(693, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(84, 35);
            this.lblName.TabIndex = 2;
            this.lblName.Text = "اسم المتحصل:";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(383, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(304, 20);
            this.txtName.TabIndex = 3;
            // 
            // lblPhone
            // 
            this.lblPhone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPhone.Location = new System.Drawing.Point(313, 0);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(64, 35);
            this.lblPhone.TabIndex = 4;
            this.lblPhone.Text = "الهاتف:";
            this.lblPhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtPhone
            // 
            this.txtPhone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPhone.Location = new System.Drawing.Point(3, 3);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(304, 20);
            this.txtPhone.TabIndex = 5;
            // 
            // pnlButtons
            // 
            this.formLayout.SetColumnSpan(this.pnlButtons, 6);
            this.pnlButtons.Controls.Add(this.btnSave);
            this.pnlButtons.Controls.Add(this.btnDelete);
            this.pnlButtons.Controls.Add(this.btnNew);
            this.pnlButtons.Controls.Add(this.btnRefresh);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlButtons.Location = new System.Drawing.Point(3, 38);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(924, 49);
            this.pnlButtons.TabIndex = 6;
            this.pnlButtons.WrapContents = false;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(3, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "حفظ";
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(84, 3);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(75, 23);
            this.btnDelete.TabIndex = 1;
            this.btnDelete.Text = "حذف";
            // 
            // btnNew
            // 
            this.btnNew.Location = new System.Drawing.Point(165, 3);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(75, 23);
            this.btnNew.TabIndex = 2;
            this.btnNew.Text = "جديد";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(246, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "تحديث";
            // 
            // grid
            // 
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Location = new System.Drawing.Point(13, 188);
            this.grid.Name = "grid";
            this.grid.Size = new System.Drawing.Size(954, 397);
            this.grid.TabIndex = 2;
            // 
            // CollectorsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(980, 620);
            this.Controls.Add(this.mainLayout);
            this.Controls.Add(this.status);
            this.Name = "CollectorsForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "إدارة المتحصلين";
            this.status.ResumeLayout(false);
            this.status.PerformLayout();
            this.mainLayout.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlForm.ResumeLayout(false);
            this.formLayout.ResumeLayout(false);
            this.formLayout.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
