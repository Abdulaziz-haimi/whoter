using System.Drawing;
using System.Windows.Forms;

namespace water3
{
    partial class FrmMobileReceiptsQueue
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel mainLayout;
        private Panel pnlFilters;
        private Panel pnlActions;
        private Label lblSearch;
        private TextBox txtSearch;
        private Label lblStatus;
        private ComboBox cboStatus;
        private CheckBox chkUseDate;
        private DateTimePicker dtFrom;
        private DateTimePicker dtTo;
        private Button btnSearch;
        private Button btnRefresh;
        private DataGridView dgvQueue;
        private Button btnDetails;
        private Button btnApprove;
        private Button btnReject;
        private Button btnResetToNew;
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
            this.pnlActions = new Panel();
            this.lblSearch = new Label();
            this.txtSearch = new TextBox();
            this.lblStatus = new Label();
            this.cboStatus = new ComboBox();
            this.chkUseDate = new CheckBox();
            this.dtFrom = new DateTimePicker();
            this.dtTo = new DateTimePicker();
            this.btnSearch = new Button();
            this.btnRefresh = new Button();
            this.dgvQueue = new DataGridView();
            this.btnDetails = new Button();
            this.btnApprove = new Button();
            this.btnReject = new Button();
            this.btnResetToNew = new Button();
            this.btnApproveBatch = new Button();
            this.lblCount = new Label();
            this.mainLayout.SuspendLayout();
            this.pnlFilters.SuspendLayout();
            this.pnlActions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQueue)).BeginInit();
            this.SuspendLayout();
            //
            // mainLayout
            //
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.pnlFilters, 0, 0);
            this.mainLayout.Controls.Add(this.dgvQueue, 0, 1);
            this.mainLayout.Controls.Add(this.pnlActions, 0, 2);
            this.mainLayout.Dock = DockStyle.Fill;
            this.mainLayout.RowCount = 3;
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 78F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 62F));
            this.mainLayout.Location = new Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new Padding(10);
            this.mainLayout.Size = new Size(1280, 720);
            //
            // pnlFilters
            //
            this.pnlFilters.Controls.Add(this.lblSearch);
            this.pnlFilters.Controls.Add(this.txtSearch);
            this.pnlFilters.Controls.Add(this.lblStatus);
            this.pnlFilters.Controls.Add(this.cboStatus);
            this.pnlFilters.Controls.Add(this.chkUseDate);
            this.pnlFilters.Controls.Add(this.dtFrom);
            this.pnlFilters.Controls.Add(this.dtTo);
            this.pnlFilters.Controls.Add(this.btnSearch);
            this.pnlFilters.Controls.Add(this.btnRefresh);
            this.pnlFilters.Dock = DockStyle.Fill;
            this.pnlFilters.BackColor = Color.WhiteSmoke;
            this.pnlFilters.Padding = new Padding(10);
            //
            // lblSearch
            //
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new Point(1180, 17);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new Size(43, 17);
            this.lblSearch.Text = "بحث:";
            //
            // txtSearch
            //
            this.txtSearch.Location = new Point(930, 14);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new Size(240, 24);
            //
            // lblStatus
            //
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new Point(860, 17);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new Size(47, 17);
            this.lblStatus.Text = "الحالة:";
            //
            // cboStatus
            //
            this.cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboStatus.Location = new Point(710, 14);
            this.cboStatus.Name = "cboStatus";
            this.cboStatus.Size = new Size(140, 24);
            //
            // chkUseDate
            //
            this.chkUseDate.AutoSize = true;
            this.chkUseDate.Location = new Point(640, 16);
            this.chkUseDate.Name = "chkUseDate";
            this.chkUseDate.Size = new Size(61, 21);
            this.chkUseDate.Text = "تاريخ";
            //
            // dtFrom
            //
            this.dtFrom.Format = DateTimePickerFormat.Short;
            this.dtFrom.Location = new Point(500, 14);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new Size(130, 24);
            //
            // dtTo
            //
            this.dtTo.Format = DateTimePickerFormat.Short;
            this.dtTo.Location = new Point(360, 14);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new Size(130, 24);
            //
            // btnSearch
            //
            this.btnSearch.Location = new Point(250, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new Size(90, 28);
            this.btnSearch.Text = "بحث";
            this.btnSearch.UseVisualStyleBackColor = true;
            //
            // btnRefresh
            //
            this.btnRefresh.Location = new Point(150, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new Size(90, 28);
            this.btnRefresh.Text = "تحديث";
            this.btnRefresh.UseVisualStyleBackColor = true;
            //
            // dgvQueue
            //
            this.dgvQueue.Dock = DockStyle.Fill;
            this.dgvQueue.Location = new Point(13, 91);
            this.dgvQueue.Name = "dgvQueue";
            this.dgvQueue.Size = new Size(1254, 554);
            this.dgvQueue.BackgroundColor = Color.White;
            //
            // pnlActions
            //
            this.pnlActions.Controls.Add(this.btnDetails);
            this.pnlActions.Controls.Add(this.btnApprove);
            this.pnlActions.Controls.Add(this.btnReject);
            this.pnlActions.Controls.Add(this.btnResetToNew);
            this.pnlActions.Controls.Add(this.btnApproveBatch);
            this.pnlActions.Controls.Add(this.lblCount);
            this.pnlActions.Dock = DockStyle.Fill;
            this.pnlActions.BackColor = Color.WhiteSmoke;
            this.pnlActions.Padding = new Padding(10);
            //
            // btnDetails
            //
            this.btnDetails.Location = new Point(1130, 12);
            this.btnDetails.Name = "btnDetails";
            this.btnDetails.Size = new Size(100, 30);
            this.btnDetails.Text = "تفاصيل";
            this.btnDetails.UseVisualStyleBackColor = true;
            //
            // btnApprove
            //
            this.btnApprove.Location = new Point(1020, 12);
            this.btnApprove.Name = "btnApprove";
            this.btnApprove.Size = new Size(100, 30);
            this.btnApprove.Text = "اعتماد";
            this.btnApprove.UseVisualStyleBackColor = true;
            //
            // btnReject
            //
            this.btnReject.Location = new Point(910, 12);
            this.btnReject.Name = "btnReject";
            this.btnReject.Size = new Size(100, 30);
            this.btnReject.Text = "رفض";
            this.btnReject.UseVisualStyleBackColor = true;
            //
            // btnResetToNew
            //
            this.btnResetToNew.Location = new Point(760, 12);
            this.btnResetToNew.Name = "btnResetToNew";
            this.btnResetToNew.Size = new Size(140, 30);
            this.btnResetToNew.Text = "إرجاع إلى New";
            this.btnResetToNew.UseVisualStyleBackColor = true;
            //
            // btnApproveBatch
            //
            this.btnApproveBatch.Location = new Point(590, 12);
            this.btnApproveBatch.Name = "btnApproveBatch";
            this.btnApproveBatch.Size = new Size(160, 30);
            this.btnApproveBatch.Text = "اعتماد جماعي";
            this.btnApproveBatch.UseVisualStyleBackColor = true;
            //
            // lblCount
            //
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new Point(20, 18);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new Size(92, 17);
            this.lblCount.Text = "عدد السجلات: 0";
            //
            // FrmMobileReceiptsQueue
            //
            this.AutoScaleDimensions = new SizeF(8F, 17F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1280, 720);
            this.Controls.Add(this.mainLayout);
            this.Font = new Font("Tahoma", 10F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmMobileReceiptsQueue";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.mainLayout.ResumeLayout(false);
            this.pnlFilters.ResumeLayout(false);
            this.pnlFilters.PerformLayout();
            this.pnlActions.ResumeLayout(false);
            this.pnlActions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvQueue)).EndInit();
            this.ResumeLayout(false);
        }
    }
}