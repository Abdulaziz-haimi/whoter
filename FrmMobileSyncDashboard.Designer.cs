using System.Drawing;
using System.Windows.Forms;
using water3.Controls;

namespace water3
{
    partial class FrmMobileSyncDashboard
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel mainLayout;
        private Panel pnlTop;
        private CheckBox chkUseDate;
        private DateTimePicker dtFrom;
        private DateTimePicker dtTo;
        private Button btnRefresh;
        private Button btnOpenBatches;
        private Button btnOpenQueue;
        private Button btnApproveBatch;
        private FlowLayoutPanel flowCards;
        private SplitContainer splitMain;
        private GroupBox grpLatestBatches;
        private GroupBox grpLatestQueue;
        private DataGridView dgvLatestBatches;
        private DataGridView dgvLatestQueue;

        private StatCardControl cardTotalImports;
        private StatCardControl cardTotalBatches;
        private StatCardControl cardNew;
        private StatCardControl cardApproved;
        private StatCardControl cardRejected;
        private StatCardControl cardErrors;
        private StatCardControl cardTotalAmount;
        private StatCardControl cardApprovedAmount;

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
            this.pnlTop = new Panel();
            this.chkUseDate = new CheckBox();
            this.dtFrom = new DateTimePicker();
            this.dtTo = new DateTimePicker();
            this.btnRefresh = new Button();
            this.btnOpenBatches = new Button();
            this.btnOpenQueue = new Button();
            this.btnApproveBatch = new Button();
            this.flowCards = new FlowLayoutPanel();
            this.splitMain = new SplitContainer();
            this.grpLatestBatches = new GroupBox();
            this.grpLatestQueue = new GroupBox();
            this.dgvLatestBatches = new DataGridView();
            this.dgvLatestQueue = new DataGridView();
            this.cardTotalImports = new StatCardControl();
            this.cardTotalBatches = new StatCardControl();
            this.cardNew = new StatCardControl();
            this.cardApproved = new StatCardControl();
            this.cardRejected = new StatCardControl();
            this.cardErrors = new StatCardControl();
            this.cardTotalAmount = new StatCardControl();
            this.cardApprovedAmount = new StatCardControl();
            this.mainLayout.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.flowCards.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.grpLatestBatches.SuspendLayout();
            this.grpLatestQueue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLatestBatches)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLatestQueue)).BeginInit();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.pnlTop, 0, 0);
            this.mainLayout.Controls.Add(this.flowCards, 0, 1);
            this.mainLayout.Controls.Add(this.splitMain, 0, 2);
            this.mainLayout.Dock = DockStyle.Fill;
            this.mainLayout.Location = new Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new Padding(10);
            this.mainLayout.RowCount = 3;
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 290F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.mainLayout.Size = new Size(1380, 840);
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = Color.WhiteSmoke;
            this.pnlTop.Controls.Add(this.chkUseDate);
            this.pnlTop.Controls.Add(this.dtFrom);
            this.pnlTop.Controls.Add(this.dtTo);
            this.pnlTop.Controls.Add(this.btnRefresh);
            this.pnlTop.Controls.Add(this.btnOpenBatches);
            this.pnlTop.Controls.Add(this.btnOpenQueue);
            this.pnlTop.Controls.Add(this.btnApproveBatch);
            this.pnlTop.Dock = DockStyle.Fill;
            // 
            // chkUseDate
            // 
            this.chkUseDate.AutoSize = true;
            this.chkUseDate.Location = new Point(1260, 20);
            this.chkUseDate.Text = "فترة";
            // 
            // dtFrom
            // 
            this.dtFrom.Format = DateTimePickerFormat.Short;
            this.dtFrom.Location = new Point(1120, 18);
            this.dtFrom.Size = new Size(130, 24);
            // 
            // dtTo
            // 
            this.dtTo.Format = DateTimePickerFormat.Short;
            this.dtTo.Location = new Point(980, 18);
            this.dtTo.Size = new Size(130, 24);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new Point(870, 15);
            this.btnRefresh.Size = new Size(90, 30);
            this.btnRefresh.Text = "تحديث";
            // 
            // btnOpenBatches
            // 
            this.btnOpenBatches.Location = new Point(700, 15);
            this.btnOpenBatches.Size = new Size(160, 30);
            this.btnOpenBatches.Text = "دفعات المزامنة";
            // 
            // btnOpenQueue
            // 
            this.btnOpenQueue.Location = new Point(520, 15);
            this.btnOpenQueue.Size = new Size(170, 30);
            this.btnOpenQueue.Text = "السجلات المستوردة";
            // 
            // btnApproveBatch
            // 
            this.btnApproveBatch.Location = new Point(350, 15);
            this.btnApproveBatch.Size = new Size(160, 30);
            this.btnApproveBatch.Text = "اعتماد جماعي";
            // 
            // flowCards
            // 
            this.flowCards.AutoScroll = true;
            this.flowCards.Controls.Add(this.cardTotalImports);
            this.flowCards.Controls.Add(this.cardTotalBatches);
            this.flowCards.Controls.Add(this.cardNew);
            this.flowCards.Controls.Add(this.cardApproved);
            this.flowCards.Controls.Add(this.cardRejected);
            this.flowCards.Controls.Add(this.cardErrors);
            this.flowCards.Controls.Add(this.cardTotalAmount);
            this.flowCards.Controls.Add(this.cardApprovedAmount);
            this.flowCards.Dock = DockStyle.Fill;
            this.flowCards.FlowDirection = FlowDirection.RightToLeft;
            this.flowCards.WrapContents = true;
            this.flowCards.Padding = new Padding(4);
            // 
            // splitMain
            // 
            this.splitMain.Dock = DockStyle.Fill;
            this.splitMain.Orientation = Orientation.Horizontal;
            this.splitMain.SplitterDistance = 210;
            this.splitMain.Panel1.Controls.Add(this.grpLatestBatches);
            this.splitMain.Panel2.Controls.Add(this.grpLatestQueue);
            // 
            // grpLatestBatches
            // 
            this.grpLatestBatches.Dock = DockStyle.Fill;
            this.grpLatestBatches.Text = "أحدث دفعات المزامنة";
            this.grpLatestBatches.Controls.Add(this.dgvLatestBatches);
            // 
            // grpLatestQueue
            // 
            this.grpLatestQueue.Dock = DockStyle.Fill;
            this.grpLatestQueue.Text = "أحدث السجلات المستوردة";
            this.grpLatestQueue.Controls.Add(this.dgvLatestQueue);
            // 
            // dgvLatestBatches
            // 
            this.dgvLatestBatches.Dock = DockStyle.Fill;
            this.dgvLatestBatches.BackgroundColor = Color.White;
            this.dgvLatestBatches.ReadOnly = true;
            this.dgvLatestBatches.AllowUserToAddRows = false;
            this.dgvLatestBatches.AllowUserToDeleteRows = false;
            this.dgvLatestBatches.RowHeadersVisible = false;
            this.dgvLatestBatches.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // 
            // dgvLatestQueue
            // 
            this.dgvLatestQueue.Dock = DockStyle.Fill;
            this.dgvLatestQueue.BackgroundColor = Color.White;
            this.dgvLatestQueue.ReadOnly = true;
            this.dgvLatestQueue.AllowUserToAddRows = false;
            this.dgvLatestQueue.AllowUserToDeleteRows = false;
            this.dgvLatestQueue.RowHeadersVisible = false;
            this.dgvLatestQueue.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // 
            // FrmMobileSyncDashboard
            // 
            this.AutoScaleDimensions = new SizeF(8F, 17F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1380, 840);
            this.Controls.Add(this.mainLayout);
            this.Font = new Font("Tahoma", 10F);
            this.Name = "FrmMobileSyncDashboard";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "لوحة متابعة التحصيلات الجوالة";
            this.mainLayout.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.flowCards.ResumeLayout(false);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.grpLatestBatches.ResumeLayout(false);
            this.grpLatestQueue.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLatestBatches)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLatestQueue)).EndInit();
            this.ResumeLayout(false);
        }
    }
}