using System.Drawing;
using System.Windows.Forms;

namespace water3
{
    partial class FrmSyncBatchDetails
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel mainLayout;
        private TextBox txtHeader;
        private SplitContainer splitMain;
        private GroupBox grpImports;
        private GroupBox grpErrors;
        private DataGridView dgvImports;
        private DataGridView dgvErrors;
        private Panel pnlActions;
        private Button btnRefresh;
        private Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainLayout = new TableLayoutPanel();
            this.txtHeader = new TextBox();
            this.splitMain = new SplitContainer();
            this.grpImports = new GroupBox();
            this.grpErrors = new GroupBox();
            this.dgvImports = new DataGridView();
            this.dgvErrors = new DataGridView();
            this.pnlActions = new Panel();
            this.btnRefresh = new Button();
            this.btnClose = new Button();
            this.mainLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.grpImports.SuspendLayout();
            this.grpErrors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvImports)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).BeginInit();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.txtHeader, 0, 0);
            this.mainLayout.Controls.Add(this.splitMain, 0, 1);
            this.mainLayout.Controls.Add(this.pnlActions, 0, 2);
            this.mainLayout.Dock = DockStyle.Fill;
            this.mainLayout.Location = new Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new Padding(10);
            this.mainLayout.RowCount = 3;
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            this.mainLayout.Size = new Size(1180, 720);
            // 
            // txtHeader
            // 
            this.txtHeader.Dock = DockStyle.Fill;
            this.txtHeader.Multiline = true;
            this.txtHeader.ReadOnly = true;
            this.txtHeader.BackColor = Color.White;
            this.txtHeader.ScrollBars = ScrollBars.Vertical;
            this.txtHeader.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            // 
            // splitMain
            // 
            this.splitMain.Dock = DockStyle.Fill;
            this.splitMain.Orientation = Orientation.Horizontal;
            this.splitMain.SplitterDistance = 360;
            this.splitMain.Panel1.Controls.Add(this.grpImports);
            this.splitMain.Panel2.Controls.Add(this.grpErrors);
            // 
            // grpImports
            // 
            this.grpImports.Dock = DockStyle.Fill;
            this.grpImports.Text = "السجلات داخل الدفعة";
            this.grpImports.Controls.Add(this.dgvImports);
            // 
            // grpErrors
            // 
            this.grpErrors.Dock = DockStyle.Fill;
            this.grpErrors.Text = "أخطاء الدفعة";
            this.grpErrors.Controls.Add(this.dgvErrors);
            // 
            // dgvImports
            // 
            this.dgvImports.Dock = DockStyle.Fill;
            this.dgvImports.BackgroundColor = Color.White;
            this.dgvImports.ReadOnly = true;
            this.dgvImports.AllowUserToAddRows = false;
            this.dgvImports.AllowUserToDeleteRows = false;
            this.dgvImports.RowHeadersVisible = false;
            this.dgvImports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // 
            // dgvErrors
            // 
            this.dgvErrors.Dock = DockStyle.Fill;
            this.dgvErrors.BackgroundColor = Color.White;
            this.dgvErrors.ReadOnly = true;
            this.dgvErrors.AllowUserToAddRows = false;
            this.dgvErrors.AllowUserToDeleteRows = false;
            this.dgvErrors.RowHeadersVisible = false;
            this.dgvErrors.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // 
            // pnlActions
            // 
            this.pnlActions.Controls.Add(this.btnRefresh);
            this.pnlActions.Controls.Add(this.btnClose);
            this.pnlActions.Dock = DockStyle.Fill;
            this.pnlActions.BackColor = Color.WhiteSmoke;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new Point(1060, 10);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new Size(90, 30);
            this.btnRefresh.Text = "تحديث";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new Point(20, 10);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(90, 30);
            this.btnClose.Text = "إغلاق";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FrmSyncBatchDetails
            // 
            this.AutoScaleDimensions = new SizeF(8F, 17F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1180, 720);
            this.Controls.Add(this.mainLayout);
            this.Font = new Font("Tahoma", 10F);
            this.Name = "FrmSyncBatchDetails";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "تفاصيل دفعة المزامنة";
            this.mainLayout.ResumeLayout(false);
            this.mainLayout.PerformLayout();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.grpImports.ResumeLayout(false);
            this.grpErrors.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvImports)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvErrors)).EndInit();
            this.pnlActions.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}