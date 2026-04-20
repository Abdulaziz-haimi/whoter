using System.Drawing;
using System.Windows.Forms;

namespace water3
{
    partial class FrmSyncBatches
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel mainLayout;
        private Panel pnlFilters;
        private DataGridView dgvBatches;
        private Panel pnlActions;
        private Label lblStatus;
        private ComboBox cboStatus;
        private CheckBox chkUseDate;
        private DateTimePicker dtFrom;
        private DateTimePicker dtTo;
        private Button btnSearch;
        private Button btnRefresh;
        private Button btnDetails;
        private Button btnApproveBatch;
        private Label lblCount;

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
            this.pnlFilters = new Panel();
            this.dgvBatches = new DataGridView();
            this.pnlActions = new Panel();
            this.lblStatus = new Label();
            this.cboStatus = new ComboBox();
            this.chkUseDate = new CheckBox();
            this.dtFrom = new DateTimePicker();
            this.dtTo = new DateTimePicker();
            this.btnSearch = new Button();
            this.btnRefresh = new Button();
            this.btnDetails = new Button();
            this.btnApproveBatch = new Button();
            this.lblCount = new Label();
            this.mainLayout.SuspendLayout();
            this.pnlFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBatches)).BeginInit();
            this.pnlActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.pnlFilters, 0, 0);
            this.mainLayout.Controls.Add(this.dgvBatches, 0, 1);
            this.mainLayout.Controls.Add(this.pnlActions, 0, 2);
            this.mainLayout.Dock = DockStyle.Fill;
            this.mainLayout.Location = new Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new Padding(10);
            this.mainLayout.RowCount = 3;
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            this.mainLayout.Size = new Size(1260, 720);
            // 
            // pnlFilters
            // 
            this.pnlFilters.BackColor = Color.WhiteSmoke;
            this.pnlFilters.Controls.Add(this.lblStatus);
            this.pnlFilters.Controls.Add(this.cboStatus);
            this.pnlFilters.Controls.Add(this.chkUseDate);
            this.pnlFilters.Controls.Add(this.dtFrom);
            this.pnlFilters.Controls.Add(this.dtTo);
            this.pnlFilters.Controls.Add(this.btnSearch);
            this.pnlFilters.Controls.Add(this.btnRefresh);
            this.pnlFilters.Dock = DockStyle.Fill;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new Point(1140, 18);
            this.lblStatus.Text = "الحالة:";
            // 
            // cboStatus
            // 
            this.cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new Point(990, 15);
            this.cboStatus.Size = new Size(140, 24);
            // 
            // chkUseDate
            // 
            this.chkUseDate.AutoSize = true;
            this.chkUseDate.Location = new Point(920, 17);
            this.chkUseDate.Text = "فترة";
            // 
            // dtFrom
            // 
            this.dtFrom.Format = DateTimePickerFormat.Short;
            this.dtFrom.Location = new Point(780, 15);
            this.dtFrom.Size = new Size(130, 24);
            // 
            // dtTo
            // 
            this.dtTo.Format = DateTimePickerFormat.Short;
            this.dtTo.Location = new Point(640, 15);
            this.dtTo.Size = new Size(130, 24);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new Point(540, 12);
            this.btnSearch.Size = new Size(90, 28);
            this.btnSearch.Text = "بحث";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new Point(440, 12);
            this.btnRefresh.Size = new Size(90, 28);
            this.btnRefresh.Text = "تحديث";
            // 
            // dgvBatches
            // 
            this.dgvBatches.Dock = DockStyle.Fill;
            this.dgvBatches.BackgroundColor = Color.White;
            // 
            // pnlActions
            // 
            this.pnlActions.BackColor = Color.WhiteSmoke;
            this.pnlActions.Controls.Add(this.btnDetails);
            this.pnlActions.Controls.Add(this.btnApproveBatch);
            this.pnlActions.Controls.Add(this.lblCount);
            this.pnlActions.Dock = DockStyle.Fill;
            // 
            // btnDetails
            // 
            this.btnDetails.Location = new Point(1120, 12);
            this.btnDetails.Size = new Size(100, 30);
            this.btnDetails.Text = "تفاصيل";
            // 
            // btnApproveBatch
            // 
            this.btnApproveBatch.Location = new Point(960, 12);
            this.btnApproveBatch.Size = new Size(150, 30);
            this.btnApproveBatch.Text = "اعتماد جماعي";
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new Point(20, 18);
            this.lblCount.Text = "عدد الدفعات: 0";
            // 
            // FrmSyncBatches
            // 
            this.AutoScaleDimensions = new SizeF(8F, 17F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1260, 720);
            this.Controls.Add(this.mainLayout);
            this.Font = new Font("Tahoma", 10F);
            this.Name = "FrmSyncBatches";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "دفعات المزامنة";
            this.mainLayout.ResumeLayout(false);
            this.pnlFilters.ResumeLayout(false);
            this.pnlFilters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBatches)).EndInit();
            this.pnlActions.ResumeLayout(false);
            this.pnlActions.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}