namespace water3
{
    partial class DashboardForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Timer autoRefreshTimer;
        private System.Windows.Forms.ToolTip toolTip1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.autoRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.root = new System.Windows.Forms.TableLayoutPanel();
            this.filterCard = new System.Windows.Forms.Panel();
            this.filterLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblPeriod = new System.Windows.Forms.Label();
            this.cbPeriodPreset = new System.Windows.Forms.ComboBox();
            this.lblPeriodText = new System.Windows.Forms.Label();
            this.pnlToWrap = new System.Windows.Forms.Panel();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.pnlFromWrap = new System.Windows.Forms.Panel();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.lblFrom = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnQuickReading = new System.Windows.Forms.Button();
            this.btnQuickPay = new System.Windows.Forms.Button();
            this.btnStatement = new System.Windows.Forms.Button();
            this.kpiPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tabsCard = new System.Windows.Forms.Panel();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabInvoices = new System.Windows.Forms.TabPage();
            this.bodyInvoices = new System.Windows.Forms.Panel();
            this.gridCardInvoices = new System.Windows.Forms.Panel();
            this.gridInvoices = new System.Windows.Forms.DataGridView();
            this.hdrInvoices = new System.Windows.Forms.Panel();
            this.lblHdrInvoices = new System.Windows.Forms.Label();
            this.btnAllInvoices = new System.Windows.Forms.Button();
            this.tabPayments = new System.Windows.Forms.TabPage();
            this.bodyPayments = new System.Windows.Forms.Panel();
            this.gridCardPayments = new System.Windows.Forms.Panel();
            this.gridPayments = new System.Windows.Forms.DataGridView();
            this.hdrPayments = new System.Windows.Forms.Panel();
            this.lblHdrPayments = new System.Windows.Forms.Label();
            this.btnAllPayments = new System.Windows.Forms.Button();
            this.tabOutstanding = new System.Windows.Forms.TabPage();
            this.bodyOutstanding = new System.Windows.Forms.Panel();
            this.gridCardOutstanding = new System.Windows.Forms.Panel();
            this.gridOutstanding = new System.Windows.Forms.DataGridView();
            this.hdrOutstanding = new System.Windows.Forms.Panel();
            this.lblHdrOutstanding = new System.Windows.Forms.Label();
            this.btnAllOutstanding = new System.Windows.Forms.Button();
            this.root.SuspendLayout();
            this.filterCard.SuspendLayout();
            this.filterLayout.SuspendLayout();
            this.pnlToWrap.SuspendLayout();
            this.pnlFromWrap.SuspendLayout();
            this.tabsCard.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabInvoices.SuspendLayout();
            this.bodyInvoices.SuspendLayout();
            this.gridCardInvoices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridInvoices)).BeginInit();
            this.hdrInvoices.SuspendLayout();
            this.tabPayments.SuspendLayout();
            this.bodyPayments.SuspendLayout();
            this.gridCardPayments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPayments)).BeginInit();
            this.hdrPayments.SuspendLayout();
            this.tabOutstanding.SuspendLayout();
            this.bodyOutstanding.SuspendLayout();
            this.gridCardOutstanding.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridOutstanding)).BeginInit();
            this.hdrOutstanding.SuspendLayout();
            this.SuspendLayout();
            // 
            // autoRefreshTimer
            // 
            this.autoRefreshTimer.Interval = 300000;
            this.autoRefreshTimer.Tick += new System.EventHandler(this.autoRefreshTimer_Tick);
            // 
            // root
            // 
            this.root.ColumnCount = 1;
            this.root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.root.Controls.Add(this.filterCard, 0, 0);
            this.root.Controls.Add(this.kpiPanel, 0, 1);
            this.root.Controls.Add(this.tabsCard, 0, 2);
            this.root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.root.Location = new System.Drawing.Point(0, 0);
            this.root.Margin = new System.Windows.Forms.Padding(0);
            this.root.Name = "root";
            this.root.Padding = new System.Windows.Forms.Padding(12);
            this.root.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.root.RowCount = 3;
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 122F));
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.root.Size = new System.Drawing.Size(1200, 749);
            this.root.TabIndex = 0;
            // 
            // filterCard
            // 
            this.filterCard.Controls.Add(this.filterLayout);
            this.filterCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterCard.Location = new System.Drawing.Point(15, 15);
            this.filterCard.Name = "filterCard";
            this.filterCard.Padding = new System.Windows.Forms.Padding(12);
            this.filterCard.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.filterCard.Size = new System.Drawing.Size(1170, 82);
            this.filterCard.TabIndex = 0;
            this.filterCard.Paint += new System.Windows.Forms.PaintEventHandler(this.card_Paint);
            // 
            // filterLayout
            // 
            this.filterLayout.ColumnCount = 12;
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 180F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.filterLayout.Controls.Add(this.lblPeriod, 0, 0);
            this.filterLayout.Controls.Add(this.cbPeriodPreset, 1, 0);
            this.filterLayout.Controls.Add(this.lblPeriodText, 2, 0);
            this.filterLayout.Controls.Add(this.pnlToWrap, 3, 0);
            this.filterLayout.Controls.Add(this.lblTo, 4, 0);
            this.filterLayout.Controls.Add(this.pnlFromWrap, 5, 0);
            this.filterLayout.Controls.Add(this.lblFrom, 6, 0);
            this.filterLayout.Controls.Add(this.btnRefresh, 8, 0);
            this.filterLayout.Controls.Add(this.btnQuickReading, 9, 0);
            this.filterLayout.Controls.Add(this.btnQuickPay, 10, 0);
            this.filterLayout.Controls.Add(this.btnStatement, 11, 0);
            this.filterLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterLayout.Location = new System.Drawing.Point(12, 12);
            this.filterLayout.Margin = new System.Windows.Forms.Padding(0);
            this.filterLayout.Name = "filterLayout";
            this.filterLayout.Padding = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.filterLayout.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.filterLayout.RowCount = 1;
            this.filterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filterLayout.Size = new System.Drawing.Size(1146, 58);
            this.filterLayout.TabIndex = 0;
            // 
            // lblPeriod
            // 
            this.lblPeriod.AutoEllipsis = true;
            this.lblPeriod.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPeriod.Location = new System.Drawing.Point(3, 15);
            this.lblPeriod.Margin = new System.Windows.Forms.Padding(3, 9, 8, 3);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblPeriod.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblPeriod.Size = new System.Drawing.Size(235, 34);
            this.lblPeriod.TabIndex = 10;
            this.lblPeriod.Text = "📅 الفترة";
            this.lblPeriod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblPeriod.Paint += new System.Windows.Forms.PaintEventHandler(this.lblPeriod_Paint);
            // 
            // cbPeriodPreset
            // 
            this.cbPeriodPreset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbPeriodPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPeriodPreset.FormattingEnabled = true;
            this.cbPeriodPreset.Location = new System.Drawing.Point(249, 17);
            this.cbPeriodPreset.Margin = new System.Windows.Forms.Padding(3, 11, 3, 3);
            this.cbPeriodPreset.Name = "cbPeriodPreset";
            this.cbPeriodPreset.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.cbPeriodPreset.Size = new System.Drawing.Size(174, 21);
            this.cbPeriodPreset.TabIndex = 0;
            this.cbPeriodPreset.SelectedIndexChanged += new System.EventHandler(this.cbPeriodPreset_SelectedIndexChanged);
            // 
            // lblPeriodText
            // 
            this.lblPeriodText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPeriodText.Location = new System.Drawing.Point(429, 6);
            this.lblPeriodText.Name = "lblPeriodText";
            this.lblPeriodText.Size = new System.Drawing.Size(64, 46);
            this.lblPeriodText.TabIndex = 1;
            this.lblPeriodText.Text = "الفترة:";
            this.lblPeriodText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlToWrap
            // 
            this.pnlToWrap.Controls.Add(this.dtTo);
            this.pnlToWrap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlToWrap.Location = new System.Drawing.Point(499, 9);
            this.pnlToWrap.Name = "pnlToWrap";
            this.pnlToWrap.Padding = new System.Windows.Forms.Padding(10, 8, 10, 7);
            this.pnlToWrap.Size = new System.Drawing.Size(164, 40);
            this.pnlToWrap.TabIndex = 2;
            this.pnlToWrap.Paint += new System.Windows.Forms.PaintEventHandler(this.inputWrap_Paint);
            // 
            // dtTo
            // 
            this.dtTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTo.Location = new System.Drawing.Point(10, 8);
            this.dtTo.Name = "dtTo";
            this.dtTo.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dtTo.RightToLeftLayout = true;
            this.dtTo.Size = new System.Drawing.Size(144, 20);
            this.dtTo.TabIndex = 0;
            // 
            // lblTo
            // 
            this.lblTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTo.Location = new System.Drawing.Point(669, 6);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(49, 46);
            this.lblTo.TabIndex = 3;
            this.lblTo.Text = "إلى:";
            this.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlFromWrap
            // 
            this.pnlFromWrap.Controls.Add(this.dtFrom);
            this.pnlFromWrap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFromWrap.Location = new System.Drawing.Point(724, 9);
            this.pnlFromWrap.Name = "pnlFromWrap";
            this.pnlFromWrap.Padding = new System.Windows.Forms.Padding(10, 8, 10, 7);
            this.pnlFromWrap.Size = new System.Drawing.Size(164, 40);
            this.pnlFromWrap.TabIndex = 4;
            this.pnlFromWrap.Paint += new System.Windows.Forms.PaintEventHandler(this.inputWrap_Paint);
            // 
            // dtFrom
            // 
            this.dtFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFrom.Location = new System.Drawing.Point(10, 8);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dtFrom.RightToLeftLayout = true;
            this.dtFrom.Size = new System.Drawing.Size(144, 20);
            this.dtFrom.TabIndex = 0;
            // 
            // lblFrom
            // 
            this.lblFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFrom.Location = new System.Drawing.Point(894, 6);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(49, 46);
            this.lblFrom.TabIndex = 5;
            this.lblFrom.Text = "من:";
            this.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Location = new System.Drawing.Point(965, 9);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(40, 40);
            this.btnRefresh.TabIndex = 6;
            this.btnRefresh.Text = "🔄";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnQuickReading
            // 
            this.btnQuickReading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnQuickReading.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuickReading.Location = new System.Drawing.Point(1011, 9);
            this.btnQuickReading.Name = "btnQuickReading";
            this.btnQuickReading.Size = new System.Drawing.Size(40, 40);
            this.btnQuickReading.TabIndex = 7;
            this.btnQuickReading.Text = "📝";
            this.btnQuickReading.UseVisualStyleBackColor = false;
            this.btnQuickReading.Click += new System.EventHandler(this.btnQuickReading_Click);
            // 
            // btnQuickPay
            // 
            this.btnQuickPay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnQuickPay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuickPay.Location = new System.Drawing.Point(1057, 9);
            this.btnQuickPay.Name = "btnQuickPay";
            this.btnQuickPay.Size = new System.Drawing.Size(40, 40);
            this.btnQuickPay.TabIndex = 8;
            this.btnQuickPay.Text = "💰";
            this.btnQuickPay.UseVisualStyleBackColor = false;
            this.btnQuickPay.Click += new System.EventHandler(this.btnQuickPay_Click);
            // 
            // btnStatement
            // 
            this.btnStatement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStatement.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStatement.Location = new System.Drawing.Point(1103, 9);
            this.btnStatement.Name = "btnStatement";
            this.btnStatement.Size = new System.Drawing.Size(40, 40);
            this.btnStatement.TabIndex = 9;
            this.btnStatement.Text = "📊";
            this.btnStatement.UseVisualStyleBackColor = false;
            this.btnStatement.Click += new System.EventHandler(this.btnStatement_Click);
            // 
            // kpiPanel
            // 
            this.kpiPanel.AutoScroll = true;
            this.kpiPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.kpiPanel.Location = new System.Drawing.Point(15, 103);
            this.kpiPanel.Name = "kpiPanel";
            this.kpiPanel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.kpiPanel.Size = new System.Drawing.Size(1170, 93);
            this.kpiPanel.TabIndex = 2;
            this.kpiPanel.WrapContents = false;
            // 
            // tabsCard
            // 
            this.tabsCard.Controls.Add(this.tabs);
            this.tabsCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabsCard.Location = new System.Drawing.Point(15, 225);
            this.tabsCard.Name = "tabsCard";
            this.tabsCard.Padding = new System.Windows.Forms.Padding(6);
            this.tabsCard.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabsCard.Size = new System.Drawing.Size(1170, 509);
            this.tabsCard.TabIndex = 3;
            this.tabsCard.Paint += new System.Windows.Forms.PaintEventHandler(this.card_Paint);
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabInvoices);
            this.tabs.Controls.Add(this.tabPayments);
            this.tabs.Controls.Add(this.tabOutstanding);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabs.ItemSize = new System.Drawing.Size(185, 38);
            this.tabs.Location = new System.Drawing.Point(6, 6);
            this.tabs.Name = "tabs";
            this.tabs.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(1158, 497);
            this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabs.TabIndex = 0;
            this.tabs.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabs_DrawItem);
            // 
            // tabInvoices
            // 
            this.tabInvoices.Controls.Add(this.bodyInvoices);
            this.tabInvoices.Controls.Add(this.hdrInvoices);
            this.tabInvoices.Location = new System.Drawing.Point(4, 42);
            this.tabInvoices.Name = "tabInvoices";
            this.tabInvoices.Padding = new System.Windows.Forms.Padding(8);
            this.tabInvoices.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tabInvoices.Size = new System.Drawing.Size(1150, 451);
            this.tabInvoices.TabIndex = 0;
            this.tabInvoices.Text = "📄 آخر الفواتير";
            // 
            // bodyInvoices
            // 
            this.bodyInvoices.Controls.Add(this.gridCardInvoices);
            this.bodyInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bodyInvoices.Location = new System.Drawing.Point(8, 50);
            this.bodyInvoices.Name = "bodyInvoices";
            this.bodyInvoices.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.bodyInvoices.Size = new System.Drawing.Size(1134, 393);
            this.bodyInvoices.TabIndex = 1;
            // 
            // gridCardInvoices
            // 
            this.gridCardInvoices.Controls.Add(this.gridInvoices);
            this.gridCardInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCardInvoices.Location = new System.Drawing.Point(0, 8);
            this.gridCardInvoices.Name = "gridCardInvoices";
            this.gridCardInvoices.Padding = new System.Windows.Forms.Padding(6);
            this.gridCardInvoices.Size = new System.Drawing.Size(1134, 385);
            this.gridCardInvoices.TabIndex = 0;
            this.gridCardInvoices.Paint += new System.Windows.Forms.PaintEventHandler(this.card_Paint);
            // 
            // gridInvoices
            // 
            this.gridInvoices.AllowUserToAddRows = false;
            this.gridInvoices.AllowUserToDeleteRows = false;
            this.gridInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridInvoices.Location = new System.Drawing.Point(6, 6);
            this.gridInvoices.Name = "gridInvoices";
            this.gridInvoices.ReadOnly = true;
            this.gridInvoices.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.gridInvoices.RowHeadersVisible = false;
            this.gridInvoices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridInvoices.Size = new System.Drawing.Size(1122, 373);
            this.gridInvoices.TabIndex = 0;
            this.gridInvoices.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridInvoices_CellDoubleClick);
            // 
            // hdrInvoices
            // 
            this.hdrInvoices.Controls.Add(this.lblHdrInvoices);
            this.hdrInvoices.Controls.Add(this.btnAllInvoices);
            this.hdrInvoices.Dock = System.Windows.Forms.DockStyle.Top;
            this.hdrInvoices.Location = new System.Drawing.Point(8, 8);
            this.hdrInvoices.Name = "hdrInvoices";
            this.hdrInvoices.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.hdrInvoices.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.hdrInvoices.Size = new System.Drawing.Size(1134, 42);
            this.hdrInvoices.TabIndex = 0;
            // 
            // lblHdrInvoices
            // 
            this.lblHdrInvoices.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblHdrInvoices.Location = new System.Drawing.Point(864, 6);
            this.lblHdrInvoices.Name = "lblHdrInvoices";
            this.lblHdrInvoices.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblHdrInvoices.Size = new System.Drawing.Size(260, 30);
            this.lblHdrInvoices.TabIndex = 0;
            this.lblHdrInvoices.Text = "آخر الفواتير";
            this.lblHdrInvoices.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnAllInvoices
            // 
            this.btnAllInvoices.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnAllInvoices.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAllInvoices.Location = new System.Drawing.Point(10, 6);
            this.btnAllInvoices.Name = "btnAllInvoices";
            this.btnAllInvoices.Size = new System.Drawing.Size(125, 30);
            this.btnAllInvoices.TabIndex = 1;
            this.btnAllInvoices.Text = "عرض الكل ↗";
            this.btnAllInvoices.UseVisualStyleBackColor = false;
            this.btnAllInvoices.Click += new System.EventHandler(this.btnAllInvoices_Click);
            // 
            // tabPayments
            // 
            this.tabPayments.Controls.Add(this.bodyPayments);
            this.tabPayments.Controls.Add(this.hdrPayments);
            this.tabPayments.Location = new System.Drawing.Point(4, 42);
            this.tabPayments.Name = "tabPayments";
            this.tabPayments.Padding = new System.Windows.Forms.Padding(8);
            this.tabPayments.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tabPayments.Size = new System.Drawing.Size(1150, 486);
            this.tabPayments.TabIndex = 1;
            this.tabPayments.Text = "💰 آخر المدفوعات";
            // 
            // bodyPayments
            // 
            this.bodyPayments.Controls.Add(this.gridCardPayments);
            this.bodyPayments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bodyPayments.Location = new System.Drawing.Point(8, 50);
            this.bodyPayments.Name = "bodyPayments";
            this.bodyPayments.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.bodyPayments.Size = new System.Drawing.Size(1134, 428);
            this.bodyPayments.TabIndex = 1;
            // 
            // gridCardPayments
            // 
            this.gridCardPayments.Controls.Add(this.gridPayments);
            this.gridCardPayments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCardPayments.Location = new System.Drawing.Point(0, 8);
            this.gridCardPayments.Name = "gridCardPayments";
            this.gridCardPayments.Padding = new System.Windows.Forms.Padding(6);
            this.gridCardPayments.Size = new System.Drawing.Size(1134, 420);
            this.gridCardPayments.TabIndex = 0;
            this.gridCardPayments.Paint += new System.Windows.Forms.PaintEventHandler(this.card_Paint);
            // 
            // gridPayments
            // 
            this.gridPayments.AllowUserToAddRows = false;
            this.gridPayments.AllowUserToDeleteRows = false;
            this.gridPayments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPayments.Location = new System.Drawing.Point(6, 6);
            this.gridPayments.Name = "gridPayments";
            this.gridPayments.ReadOnly = true;
            this.gridPayments.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.gridPayments.RowHeadersVisible = false;
            this.gridPayments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridPayments.Size = new System.Drawing.Size(1122, 408);
            this.gridPayments.TabIndex = 0;
            this.gridPayments.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridPayments_CellDoubleClick);
            // 
            // hdrPayments
            // 
            this.hdrPayments.Controls.Add(this.lblHdrPayments);
            this.hdrPayments.Controls.Add(this.btnAllPayments);
            this.hdrPayments.Dock = System.Windows.Forms.DockStyle.Top;
            this.hdrPayments.Location = new System.Drawing.Point(8, 8);
            this.hdrPayments.Name = "hdrPayments";
            this.hdrPayments.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.hdrPayments.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.hdrPayments.Size = new System.Drawing.Size(1134, 42);
            this.hdrPayments.TabIndex = 0;
            // 
            // lblHdrPayments
            // 
            this.lblHdrPayments.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblHdrPayments.Location = new System.Drawing.Point(864, 6);
            this.lblHdrPayments.Name = "lblHdrPayments";
            this.lblHdrPayments.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblHdrPayments.Size = new System.Drawing.Size(260, 30);
            this.lblHdrPayments.TabIndex = 0;
            this.lblHdrPayments.Text = "آخر المدفوعات";
            this.lblHdrPayments.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnAllPayments
            // 
            this.btnAllPayments.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnAllPayments.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAllPayments.Location = new System.Drawing.Point(10, 6);
            this.btnAllPayments.Name = "btnAllPayments";
            this.btnAllPayments.Size = new System.Drawing.Size(125, 30);
            this.btnAllPayments.TabIndex = 1;
            this.btnAllPayments.Text = "عرض الكل ↗";
            this.btnAllPayments.UseVisualStyleBackColor = false;
            this.btnAllPayments.Click += new System.EventHandler(this.btnAllPayments_Click);
            // 
            // tabOutstanding
            // 
            this.tabOutstanding.Controls.Add(this.bodyOutstanding);
            this.tabOutstanding.Controls.Add(this.hdrOutstanding);
            this.tabOutstanding.Location = new System.Drawing.Point(4, 42);
            this.tabOutstanding.Name = "tabOutstanding";
            this.tabOutstanding.Padding = new System.Windows.Forms.Padding(8);
            this.tabOutstanding.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tabOutstanding.Size = new System.Drawing.Size(1150, 486);
            this.tabOutstanding.TabIndex = 2;
            this.tabOutstanding.Text = "⚠️ أعلى المتأخرات";
            // 
            // bodyOutstanding
            // 
            this.bodyOutstanding.Controls.Add(this.gridCardOutstanding);
            this.bodyOutstanding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bodyOutstanding.Location = new System.Drawing.Point(8, 50);
            this.bodyOutstanding.Name = "bodyOutstanding";
            this.bodyOutstanding.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.bodyOutstanding.Size = new System.Drawing.Size(1134, 428);
            this.bodyOutstanding.TabIndex = 1;
            // 
            // gridCardOutstanding
            // 
            this.gridCardOutstanding.Controls.Add(this.gridOutstanding);
            this.gridCardOutstanding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCardOutstanding.Location = new System.Drawing.Point(0, 8);
            this.gridCardOutstanding.Name = "gridCardOutstanding";
            this.gridCardOutstanding.Padding = new System.Windows.Forms.Padding(6);
            this.gridCardOutstanding.Size = new System.Drawing.Size(1134, 420);
            this.gridCardOutstanding.TabIndex = 0;
            this.gridCardOutstanding.Paint += new System.Windows.Forms.PaintEventHandler(this.card_Paint);
            // 
            // gridOutstanding
            // 
            this.gridOutstanding.AllowUserToAddRows = false;
            this.gridOutstanding.AllowUserToDeleteRows = false;
            this.gridOutstanding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridOutstanding.Location = new System.Drawing.Point(6, 6);
            this.gridOutstanding.Name = "gridOutstanding";
            this.gridOutstanding.ReadOnly = true;
            this.gridOutstanding.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.gridOutstanding.RowHeadersVisible = false;
            this.gridOutstanding.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridOutstanding.Size = new System.Drawing.Size(1122, 408);
            this.gridOutstanding.TabIndex = 0;
            this.gridOutstanding.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridOutstanding_CellDoubleClick);
            // 
            // hdrOutstanding
            // 
            this.hdrOutstanding.Controls.Add(this.lblHdrOutstanding);
            this.hdrOutstanding.Controls.Add(this.btnAllOutstanding);
            this.hdrOutstanding.Dock = System.Windows.Forms.DockStyle.Top;
            this.hdrOutstanding.Location = new System.Drawing.Point(8, 8);
            this.hdrOutstanding.Name = "hdrOutstanding";
            this.hdrOutstanding.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.hdrOutstanding.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.hdrOutstanding.Size = new System.Drawing.Size(1134, 42);
            this.hdrOutstanding.TabIndex = 0;
            // 
            // lblHdrOutstanding
            // 
            this.lblHdrOutstanding.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblHdrOutstanding.Location = new System.Drawing.Point(864, 6);
            this.lblHdrOutstanding.Name = "lblHdrOutstanding";
            this.lblHdrOutstanding.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.lblHdrOutstanding.Size = new System.Drawing.Size(260, 30);
            this.lblHdrOutstanding.TabIndex = 0;
            this.lblHdrOutstanding.Text = "أعلى المتأخرات";
            this.lblHdrOutstanding.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnAllOutstanding
            // 
            this.btnAllOutstanding.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnAllOutstanding.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAllOutstanding.Location = new System.Drawing.Point(10, 6);
            this.btnAllOutstanding.Name = "btnAllOutstanding";
            this.btnAllOutstanding.Size = new System.Drawing.Size(125, 30);
            this.btnAllOutstanding.TabIndex = 1;
            this.btnAllOutstanding.Text = "عرض الكل ↗";
            this.btnAllOutstanding.UseVisualStyleBackColor = false;
            this.btnAllOutstanding.Click += new System.EventHandler(this.btnAllOutstanding_Click);
            // 
            // DashboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 749);
            this.Controls.Add(this.root);
            this.MinimumSize = new System.Drawing.Size(1020, 660);
            this.Name = "DashboardForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Text = "لوحة التحكم";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DashboardForm_FormClosing);
            this.Load += new System.EventHandler(this.DashboardForm_Load);
            this.root.ResumeLayout(false);
            this.filterCard.ResumeLayout(false);
            this.filterLayout.ResumeLayout(false);
            this.pnlToWrap.ResumeLayout(false);
            this.pnlFromWrap.ResumeLayout(false);
            this.tabsCard.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.tabInvoices.ResumeLayout(false);
            this.bodyInvoices.ResumeLayout(false);
            this.gridCardInvoices.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridInvoices)).EndInit();
            this.hdrInvoices.ResumeLayout(false);
            this.tabPayments.ResumeLayout(false);
            this.bodyPayments.ResumeLayout(false);
            this.gridCardPayments.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridPayments)).EndInit();
            this.hdrPayments.ResumeLayout(false);
            this.tabOutstanding.ResumeLayout(false);
            this.bodyOutstanding.ResumeLayout(false);
            this.gridCardOutstanding.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridOutstanding)).EndInit();
            this.hdrOutstanding.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel tabsCard;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabInvoices;
        private System.Windows.Forms.Panel bodyInvoices;
        private System.Windows.Forms.Panel gridCardInvoices;
        private System.Windows.Forms.DataGridView gridInvoices;
        private System.Windows.Forms.Panel hdrInvoices;
        private System.Windows.Forms.Label lblHdrInvoices;
        private System.Windows.Forms.Button btnAllInvoices;
        private System.Windows.Forms.TabPage tabPayments;
        private System.Windows.Forms.Panel bodyPayments;
        private System.Windows.Forms.Panel gridCardPayments;
        private System.Windows.Forms.DataGridView gridPayments;
        private System.Windows.Forms.Panel hdrPayments;
        private System.Windows.Forms.Label lblHdrPayments;
        private System.Windows.Forms.Button btnAllPayments;
        private System.Windows.Forms.TabPage tabOutstanding;
        private System.Windows.Forms.Panel bodyOutstanding;
        private System.Windows.Forms.Panel gridCardOutstanding;
        private System.Windows.Forms.DataGridView gridOutstanding;
        private System.Windows.Forms.Panel hdrOutstanding;
        private System.Windows.Forms.Label lblHdrOutstanding;
        private System.Windows.Forms.Button btnAllOutstanding;
        private System.Windows.Forms.FlowLayoutPanel kpiPanel;
        private System.Windows.Forms.Label lblPeriod;
        private System.Windows.Forms.Panel filterCard;
        private System.Windows.Forms.TableLayoutPanel filterLayout;
        private System.Windows.Forms.Button btnStatement;
        private System.Windows.Forms.Button btnQuickPay;
        private System.Windows.Forms.Button btnQuickReading;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Panel pnlFromWrap;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.Panel pnlToWrap;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Label lblPeriodText;
        private System.Windows.Forms.ComboBox cbPeriodPreset;
        private System.Windows.Forms.TableLayoutPanel root;
    }
}


/*namespace water3
{
    partial class DashboardForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Timer autoRefreshTimer;
        private System.Windows.Forms.ToolTip toolTip1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.autoRefreshTimer = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tabsCard = new System.Windows.Forms.Panel();
            this.tabs = new System.Windows.Forms.TabControl();
            this.tabInvoices = new System.Windows.Forms.TabPage();
            this.bodyInvoices = new System.Windows.Forms.Panel();
            this.gridCardInvoices = new System.Windows.Forms.Panel();
            this.gridInvoices = new System.Windows.Forms.DataGridView();
            this.hdrInvoices = new System.Windows.Forms.Panel();
            this.lblHdrInvoices = new System.Windows.Forms.Label();
            this.btnAllInvoices = new System.Windows.Forms.Button();
            this.tabPayments = new System.Windows.Forms.TabPage();
            this.bodyPayments = new System.Windows.Forms.Panel();
            this.gridCardPayments = new System.Windows.Forms.Panel();
            this.gridPayments = new System.Windows.Forms.DataGridView();
            this.hdrPayments = new System.Windows.Forms.Panel();
            this.lblHdrPayments = new System.Windows.Forms.Label();
            this.btnAllPayments = new System.Windows.Forms.Button();
            this.tabOutstanding = new System.Windows.Forms.TabPage();
            this.bodyOutstanding = new System.Windows.Forms.Panel();
            this.gridCardOutstanding = new System.Windows.Forms.Panel();
            this.gridOutstanding = new System.Windows.Forms.DataGridView();
            this.hdrOutstanding = new System.Windows.Forms.Panel();
            this.lblHdrOutstanding = new System.Windows.Forms.Label();
            this.btnAllOutstanding = new System.Windows.Forms.Button();
            this.kpiPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblPeriod = new System.Windows.Forms.Label();
            this.filterCard = new System.Windows.Forms.Panel();
            this.filterLayout = new System.Windows.Forms.TableLayoutPanel();
            this.btnStatement = new System.Windows.Forms.Button();
            this.btnQuickPay = new System.Windows.Forms.Button();
            this.btnQuickReading = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblFrom = new System.Windows.Forms.Label();
            this.pnlFromWrap = new System.Windows.Forms.Panel();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.pnlToWrap = new System.Windows.Forms.Panel();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.lblPeriodText = new System.Windows.Forms.Label();
            this.cbPeriodPreset = new System.Windows.Forms.ComboBox();
            this.root = new System.Windows.Forms.TableLayoutPanel();
            this.tabsCard.SuspendLayout();
            this.tabs.SuspendLayout();
            this.tabInvoices.SuspendLayout();
            this.bodyInvoices.SuspendLayout();
            this.gridCardInvoices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridInvoices)).BeginInit();
            this.hdrInvoices.SuspendLayout();
            this.tabPayments.SuspendLayout();
            this.bodyPayments.SuspendLayout();
            this.gridCardPayments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridPayments)).BeginInit();
            this.hdrPayments.SuspendLayout();
            this.tabOutstanding.SuspendLayout();
            this.bodyOutstanding.SuspendLayout();
            this.gridCardOutstanding.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridOutstanding)).BeginInit();
            this.hdrOutstanding.SuspendLayout();
            this.filterCard.SuspendLayout();
            this.filterLayout.SuspendLayout();
            this.pnlFromWrap.SuspendLayout();
            this.pnlToWrap.SuspendLayout();
            this.root.SuspendLayout();
            this.SuspendLayout();
            // 
            // autoRefreshTimer
            // 
            this.autoRefreshTimer.Interval = 300000;
            this.autoRefreshTimer.Tick += new System.EventHandler(this.autoRefreshTimer_Tick);
            // 
            // tabsCard
            // 
            this.tabsCard.Controls.Add(this.tabs);
            this.tabsCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabsCard.Location = new System.Drawing.Point(13, 245);
            this.tabsCard.Name = "tabsCard";
            this.tabsCard.Padding = new System.Windows.Forms.Padding(6);
            this.tabsCard.Size = new System.Drawing.Size(1174, 491);
            this.tabsCard.TabIndex = 3;
            this.tabsCard.Paint += new System.Windows.Forms.PaintEventHandler(this.card_Paint);
            // 
            // tabs
            // 
            this.tabs.Controls.Add(this.tabInvoices);
            this.tabs.Controls.Add(this.tabPayments);
            this.tabs.Controls.Add(this.tabOutstanding);
            this.tabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabs.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabs.ItemSize = new System.Drawing.Size(180, 38);
            this.tabs.Location = new System.Drawing.Point(6, 6);
            this.tabs.Name = "tabs";
            this.tabs.SelectedIndex = 0;
            this.tabs.Size = new System.Drawing.Size(1162, 479);
            this.tabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabs.TabIndex = 0;
            this.tabs.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabs_DrawItem);
            // 
            // tabInvoices
            // 
            this.tabInvoices.Controls.Add(this.bodyInvoices);
            this.tabInvoices.Controls.Add(this.hdrInvoices);
            this.tabInvoices.Location = new System.Drawing.Point(4, 42);
            this.tabInvoices.Name = "tabInvoices";
            this.tabInvoices.Padding = new System.Windows.Forms.Padding(8);
            this.tabInvoices.Size = new System.Drawing.Size(1154, 433);
            this.tabInvoices.TabIndex = 0;
            this.tabInvoices.Text = "📄 آخر الفواتير";
            this.tabInvoices.UseVisualStyleBackColor = true;
            // 
            // bodyInvoices
            // 
            this.bodyInvoices.Controls.Add(this.gridCardInvoices);
            this.bodyInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bodyInvoices.Location = new System.Drawing.Point(8, 48);
            this.bodyInvoices.Name = "bodyInvoices";
            this.bodyInvoices.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.bodyInvoices.Size = new System.Drawing.Size(1138, 377);
            this.bodyInvoices.TabIndex = 0;
            // 
            // gridCardInvoices
            // 
            this.gridCardInvoices.Controls.Add(this.gridInvoices);
            this.gridCardInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCardInvoices.Location = new System.Drawing.Point(0, 8);
            this.gridCardInvoices.Name = "gridCardInvoices";
            this.gridCardInvoices.Padding = new System.Windows.Forms.Padding(6);
            this.gridCardInvoices.Size = new System.Drawing.Size(1138, 369);
            this.gridCardInvoices.TabIndex = 0;
            this.gridCardInvoices.Paint += new System.Windows.Forms.PaintEventHandler(this.card_Paint);
            // 
            // gridInvoices
            // 
            this.gridInvoices.AllowUserToAddRows = false;
            this.gridInvoices.AllowUserToDeleteRows = false;
            this.gridInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridInvoices.Location = new System.Drawing.Point(6, 6);
            this.gridInvoices.Name = "gridInvoices";
            this.gridInvoices.ReadOnly = true;
            this.gridInvoices.RowHeadersVisible = false;
            this.gridInvoices.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridInvoices.Size = new System.Drawing.Size(1126, 357);
            this.gridInvoices.TabIndex = 0;
            this.gridInvoices.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridInvoices_CellDoubleClick);
            // 
            // hdrInvoices
            // 
            this.hdrInvoices.Controls.Add(this.lblHdrInvoices);
            this.hdrInvoices.Controls.Add(this.btnAllInvoices);
            this.hdrInvoices.Dock = System.Windows.Forms.DockStyle.Top;
            this.hdrInvoices.Location = new System.Drawing.Point(8, 8);
            this.hdrInvoices.Name = "hdrInvoices";
            this.hdrInvoices.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.hdrInvoices.Size = new System.Drawing.Size(1138, 40);
            this.hdrInvoices.TabIndex = 1;
            // 
            // lblHdrInvoices
            // 
            this.lblHdrInvoices.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblHdrInvoices.Location = new System.Drawing.Point(868, 6);
            this.lblHdrInvoices.Name = "lblHdrInvoices";
            this.lblHdrInvoices.Size = new System.Drawing.Size(260, 28);
            this.lblHdrInvoices.TabIndex = 0;
            this.lblHdrInvoices.Text = "آخر الفواتير";
            this.lblHdrInvoices.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnAllInvoices
            // 
            this.btnAllInvoices.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnAllInvoices.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAllInvoices.Location = new System.Drawing.Point(10, 6);
            this.btnAllInvoices.Name = "btnAllInvoices";
            this.btnAllInvoices.Size = new System.Drawing.Size(120, 28);
            this.btnAllInvoices.TabIndex = 1;
            this.btnAllInvoices.Text = "عرض الكل ↗";
            this.btnAllInvoices.UseVisualStyleBackColor = true;
            this.btnAllInvoices.Click += new System.EventHandler(this.btnAllInvoices_Click);
            // 
            // tabPayments
            // 
            this.tabPayments.Controls.Add(this.bodyPayments);
            this.tabPayments.Controls.Add(this.hdrPayments);
            this.tabPayments.Location = new System.Drawing.Point(4, 42);
            this.tabPayments.Name = "tabPayments";
            this.tabPayments.Padding = new System.Windows.Forms.Padding(8);
            this.tabPayments.Size = new System.Drawing.Size(1154, 504);
            this.tabPayments.TabIndex = 1;
            this.tabPayments.Text = "💰 آخر المدفوعات";
            this.tabPayments.UseVisualStyleBackColor = true;
            // 
            // bodyPayments
            // 
            this.bodyPayments.Controls.Add(this.gridCardPayments);
            this.bodyPayments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bodyPayments.Location = new System.Drawing.Point(8, 48);
            this.bodyPayments.Name = "bodyPayments";
            this.bodyPayments.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.bodyPayments.Size = new System.Drawing.Size(1138, 448);
            this.bodyPayments.TabIndex = 0;
            // 
            // gridCardPayments
            // 
            this.gridCardPayments.Controls.Add(this.gridPayments);
            this.gridCardPayments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCardPayments.Location = new System.Drawing.Point(0, 8);
            this.gridCardPayments.Name = "gridCardPayments";
            this.gridCardPayments.Padding = new System.Windows.Forms.Padding(6);
            this.gridCardPayments.Size = new System.Drawing.Size(1138, 440);
            this.gridCardPayments.TabIndex = 0;
            this.gridCardPayments.Paint += new System.Windows.Forms.PaintEventHandler(this.card_Paint);
            // 
            // gridPayments
            // 
            this.gridPayments.AllowUserToAddRows = false;
            this.gridPayments.AllowUserToDeleteRows = false;
            this.gridPayments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPayments.Location = new System.Drawing.Point(6, 6);
            this.gridPayments.Name = "gridPayments";
            this.gridPayments.ReadOnly = true;
            this.gridPayments.RowHeadersVisible = false;
            this.gridPayments.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridPayments.Size = new System.Drawing.Size(1126, 428);
            this.gridPayments.TabIndex = 0;
            this.gridPayments.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridPayments_CellDoubleClick);
            // 
            // hdrPayments
            // 
            this.hdrPayments.Controls.Add(this.lblHdrPayments);
            this.hdrPayments.Controls.Add(this.btnAllPayments);
            this.hdrPayments.Dock = System.Windows.Forms.DockStyle.Top;
            this.hdrPayments.Location = new System.Drawing.Point(8, 8);
            this.hdrPayments.Name = "hdrPayments";
            this.hdrPayments.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.hdrPayments.Size = new System.Drawing.Size(1138, 40);
            this.hdrPayments.TabIndex = 1;
            // 
            // lblHdrPayments
            // 
            this.lblHdrPayments.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblHdrPayments.Location = new System.Drawing.Point(868, 6);
            this.lblHdrPayments.Name = "lblHdrPayments";
            this.lblHdrPayments.Size = new System.Drawing.Size(260, 28);
            this.lblHdrPayments.TabIndex = 0;
            this.lblHdrPayments.Text = "آخر المدفوعات";
            this.lblHdrPayments.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnAllPayments
            // 
            this.btnAllPayments.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnAllPayments.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAllPayments.Location = new System.Drawing.Point(10, 6);
            this.btnAllPayments.Name = "btnAllPayments";
            this.btnAllPayments.Size = new System.Drawing.Size(120, 28);
            this.btnAllPayments.TabIndex = 1;
            this.btnAllPayments.Text = "عرض الكل ↗";
            this.btnAllPayments.UseVisualStyleBackColor = true;
            this.btnAllPayments.Click += new System.EventHandler(this.btnAllPayments_Click);
            // 
            // tabOutstanding
            // 
            this.tabOutstanding.Controls.Add(this.bodyOutstanding);
            this.tabOutstanding.Controls.Add(this.hdrOutstanding);
            this.tabOutstanding.Location = new System.Drawing.Point(4, 42);
            this.tabOutstanding.Name = "tabOutstanding";
            this.tabOutstanding.Padding = new System.Windows.Forms.Padding(8);
            this.tabOutstanding.Size = new System.Drawing.Size(1154, 504);
            this.tabOutstanding.TabIndex = 2;
            this.tabOutstanding.Text = "⚠️ أعلى المتأخرات";
            this.tabOutstanding.UseVisualStyleBackColor = true;
            // 
            // bodyOutstanding
            // 
            this.bodyOutstanding.Controls.Add(this.gridCardOutstanding);
            this.bodyOutstanding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bodyOutstanding.Location = new System.Drawing.Point(8, 48);
            this.bodyOutstanding.Name = "bodyOutstanding";
            this.bodyOutstanding.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.bodyOutstanding.Size = new System.Drawing.Size(1138, 448);
            this.bodyOutstanding.TabIndex = 0;
            // 
            // gridCardOutstanding
            // 
            this.gridCardOutstanding.Controls.Add(this.gridOutstanding);
            this.gridCardOutstanding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridCardOutstanding.Location = new System.Drawing.Point(0, 8);
            this.gridCardOutstanding.Name = "gridCardOutstanding";
            this.gridCardOutstanding.Padding = new System.Windows.Forms.Padding(6);
            this.gridCardOutstanding.Size = new System.Drawing.Size(1138, 440);
            this.gridCardOutstanding.TabIndex = 0;
            this.gridCardOutstanding.Paint += new System.Windows.Forms.PaintEventHandler(this.card_Paint);
            // 
            // gridOutstanding
            // 
            this.gridOutstanding.AllowUserToAddRows = false;
            this.gridOutstanding.AllowUserToDeleteRows = false;
            this.gridOutstanding.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridOutstanding.Location = new System.Drawing.Point(6, 6);
            this.gridOutstanding.Name = "gridOutstanding";
            this.gridOutstanding.ReadOnly = true;
            this.gridOutstanding.RowHeadersVisible = false;
            this.gridOutstanding.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridOutstanding.Size = new System.Drawing.Size(1126, 428);
            this.gridOutstanding.TabIndex = 0;
            this.gridOutstanding.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridOutstanding_CellDoubleClick);
            // 
            // hdrOutstanding
            // 
            this.hdrOutstanding.Controls.Add(this.lblHdrOutstanding);
            this.hdrOutstanding.Controls.Add(this.btnAllOutstanding);
            this.hdrOutstanding.Dock = System.Windows.Forms.DockStyle.Top;
            this.hdrOutstanding.Location = new System.Drawing.Point(8, 8);
            this.hdrOutstanding.Name = "hdrOutstanding";
            this.hdrOutstanding.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.hdrOutstanding.Size = new System.Drawing.Size(1138, 40);
            this.hdrOutstanding.TabIndex = 1;
            // 
            // lblHdrOutstanding
            // 
            this.lblHdrOutstanding.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblHdrOutstanding.Location = new System.Drawing.Point(868, 6);
            this.lblHdrOutstanding.Name = "lblHdrOutstanding";
            this.lblHdrOutstanding.Size = new System.Drawing.Size(260, 28);
            this.lblHdrOutstanding.TabIndex = 0;
            this.lblHdrOutstanding.Text = "أعلى المتأخرات";
            this.lblHdrOutstanding.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnAllOutstanding
            // 
            this.btnAllOutstanding.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnAllOutstanding.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAllOutstanding.Location = new System.Drawing.Point(10, 6);
            this.btnAllOutstanding.Name = "btnAllOutstanding";
            this.btnAllOutstanding.Size = new System.Drawing.Size(120, 28);
            this.btnAllOutstanding.TabIndex = 1;
            this.btnAllOutstanding.Text = "عرض الكل ↗";
            this.btnAllOutstanding.UseVisualStyleBackColor = true;
            this.btnAllOutstanding.Click += new System.EventHandler(this.btnAllOutstanding_Click);
            // 
            // kpiPanel
            // 
            this.kpiPanel.AutoScroll = true;
            this.kpiPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.kpiPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.kpiPanel.Location = new System.Drawing.Point(13, 129);
            this.kpiPanel.Name = "kpiPanel";
            this.kpiPanel.Size = new System.Drawing.Size(1174, 110);
            this.kpiPanel.TabIndex = 2;
            this.kpiPanel.WrapContents = false;
            // 
            // lblPeriod
            // 
            this.lblPeriod.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPeriod.Location = new System.Drawing.Point(13, 92);
            this.lblPeriod.Name = "lblPeriod";
            this.lblPeriod.Padding = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.lblPeriod.Size = new System.Drawing.Size(1174, 34);
            this.lblPeriod.TabIndex = 1;
            this.lblPeriod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblPeriod.Paint += new System.Windows.Forms.PaintEventHandler(this.lblPeriod_Paint);
            // 
            // filterCard
            // 
            this.filterCard.Controls.Add(this.filterLayout);
            this.filterCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterCard.Location = new System.Drawing.Point(13, 13);
            this.filterCard.Name = "filterCard";
            this.filterCard.Padding = new System.Windows.Forms.Padding(12);
            this.filterCard.Size = new System.Drawing.Size(1174, 76);
            this.filterCard.TabIndex = 0;
            this.filterCard.Paint += new System.Windows.Forms.PaintEventHandler(this.card_Paint);
            // 
            // filterLayout
            // 
            this.filterLayout.ColumnCount = 11;
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 170F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.filterLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filterLayout.Controls.Add(this.btnStatement, 0, 0);
            this.filterLayout.Controls.Add(this.btnQuickPay, 1, 0);
            this.filterLayout.Controls.Add(this.btnQuickReading, 2, 0);
            this.filterLayout.Controls.Add(this.btnRefresh, 3, 0);
            this.filterLayout.Controls.Add(this.lblFrom, 5, 0);
            this.filterLayout.Controls.Add(this.pnlFromWrap, 6, 0);
            this.filterLayout.Controls.Add(this.lblTo, 7, 0);
            this.filterLayout.Controls.Add(this.pnlToWrap, 8, 0);
            this.filterLayout.Controls.Add(this.lblPeriodText, 9, 0);
            this.filterLayout.Controls.Add(this.cbPeriodPreset, 10, 0);
            this.filterLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterLayout.Location = new System.Drawing.Point(12, 12);
            this.filterLayout.Name = "filterLayout";
            this.filterLayout.Padding = new System.Windows.Forms.Padding(0, 6, 0, 6);
            this.filterLayout.RowCount = 1;
            this.filterLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filterLayout.Size = new System.Drawing.Size(1150, 52);
            this.filterLayout.TabIndex = 0;
            // 
            // btnStatement
            // 
            this.btnStatement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnStatement.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStatement.Location = new System.Drawing.Point(1111, 9);
            this.btnStatement.Name = "btnStatement";
            this.btnStatement.Size = new System.Drawing.Size(36, 34);
            this.btnStatement.TabIndex = 0;
            this.btnStatement.Text = "📊";
            this.btnStatement.UseVisualStyleBackColor = true;
            this.btnStatement.Click += new System.EventHandler(this.btnStatement_Click);
            // 
            // btnQuickPay
            // 
            this.btnQuickPay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnQuickPay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuickPay.Location = new System.Drawing.Point(1069, 9);
            this.btnQuickPay.Name = "btnQuickPay";
            this.btnQuickPay.Size = new System.Drawing.Size(36, 34);
            this.btnQuickPay.TabIndex = 1;
            this.btnQuickPay.Text = "💰";
            this.btnQuickPay.UseVisualStyleBackColor = true;
            this.btnQuickPay.Click += new System.EventHandler(this.btnQuickPay_Click);
            // 
            // btnQuickReading
            // 
            this.btnQuickReading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnQuickReading.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnQuickReading.Location = new System.Drawing.Point(1027, 9);
            this.btnQuickReading.Name = "btnQuickReading";
            this.btnQuickReading.Size = new System.Drawing.Size(36, 34);
            this.btnQuickReading.TabIndex = 2;
            this.btnQuickReading.Text = "📝";
            this.btnQuickReading.UseVisualStyleBackColor = true;
            this.btnQuickReading.Click += new System.EventHandler(this.btnQuickReading_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Location = new System.Drawing.Point(985, 9);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(36, 34);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "🔄";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblFrom
            // 
            this.lblFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFrom.Location = new System.Drawing.Point(889, 6);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(74, 40);
            this.lblFrom.TabIndex = 4;
            this.lblFrom.Text = "من:";
            this.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlFromWrap
            // 
            this.pnlFromWrap.Controls.Add(this.dtFrom);
            this.pnlFromWrap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFromWrap.Location = new System.Drawing.Point(719, 9);
            this.pnlFromWrap.Name = "pnlFromWrap";
            this.pnlFromWrap.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.pnlFromWrap.Size = new System.Drawing.Size(164, 34);
            this.pnlFromWrap.TabIndex = 5;
            this.pnlFromWrap.Paint += new System.Windows.Forms.PaintEventHandler(this.inputWrap_Paint);
            // 
            // dtFrom
            // 
            this.dtFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFrom.Location = new System.Drawing.Point(10, 7);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.RightToLeftLayout = true;
            this.dtFrom.Size = new System.Drawing.Size(144, 20);
            this.dtFrom.TabIndex = 0;
            // 
            // lblTo
            // 
            this.lblTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTo.Location = new System.Drawing.Point(659, 6);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(54, 40);
            this.lblTo.TabIndex = 6;
            this.lblTo.Text = "إلى:";
            this.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlToWrap
            // 
            this.pnlToWrap.Controls.Add(this.dtTo);
            this.pnlToWrap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlToWrap.Location = new System.Drawing.Point(489, 9);
            this.pnlToWrap.Name = "pnlToWrap";
            this.pnlToWrap.Padding = new System.Windows.Forms.Padding(10, 7, 10, 7);
            this.pnlToWrap.Size = new System.Drawing.Size(164, 34);
            this.pnlToWrap.TabIndex = 7;
            this.pnlToWrap.Paint += new System.Windows.Forms.PaintEventHandler(this.inputWrap_Paint);
            // 
            // dtTo
            // 
            this.dtTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTo.Location = new System.Drawing.Point(10, 7);
            this.dtTo.Name = "dtTo";
            this.dtTo.RightToLeftLayout = true;
            this.dtTo.Size = new System.Drawing.Size(144, 20);
            this.dtTo.TabIndex = 0;
            // 
            // lblPeriodText
            // 
            this.lblPeriodText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPeriodText.Location = new System.Drawing.Point(419, 6);
            this.lblPeriodText.Name = "lblPeriodText";
            this.lblPeriodText.Size = new System.Drawing.Size(64, 40);
            this.lblPeriodText.TabIndex = 8;
            this.lblPeriodText.Text = "الفترة:";
            this.lblPeriodText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbPeriodPreset
            // 
            this.cbPeriodPreset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbPeriodPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPeriodPreset.FormattingEnabled = true;
            this.cbPeriodPreset.Location = new System.Drawing.Point(3, 14);
            this.cbPeriodPreset.Margin = new System.Windows.Forms.Padding(3, 8, 3, 3);
            this.cbPeriodPreset.Name = "cbPeriodPreset";
            this.cbPeriodPreset.Size = new System.Drawing.Size(410, 21);
            this.cbPeriodPreset.TabIndex = 9;
            this.cbPeriodPreset.SelectedIndexChanged += new System.EventHandler(this.cbPeriodPreset_SelectedIndexChanged);
            // 
            // root
            // 
            this.root.ColumnCount = 1;
            this.root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.root.Controls.Add(this.filterCard, 0, 0);
            this.root.Controls.Add(this.lblPeriod, 0, 1);
            this.root.Controls.Add(this.kpiPanel, 0, 2);
            this.root.Controls.Add(this.tabsCard, 0, 3);
            this.root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.root.Location = new System.Drawing.Point(0, 0);
            this.root.Name = "root";
            this.root.Padding = new System.Windows.Forms.Padding(10);
            this.root.RowCount = 4;
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 116F));
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.root.Size = new System.Drawing.Size(1200, 749);
            this.root.TabIndex = 0;
            // 
            // DashboardForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 749);
            this.Controls.Add(this.root);
            this.Name = "DashboardForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "لوحة التحكم";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DashboardForm_FormClosing);
            this.Load += new System.EventHandler(this.DashboardForm_Load);
            this.tabsCard.ResumeLayout(false);
            this.tabs.ResumeLayout(false);
            this.tabInvoices.ResumeLayout(false);
            this.bodyInvoices.ResumeLayout(false);
            this.gridCardInvoices.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridInvoices)).EndInit();
            this.hdrInvoices.ResumeLayout(false);
            this.tabPayments.ResumeLayout(false);
            this.bodyPayments.ResumeLayout(false);
            this.gridCardPayments.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridPayments)).EndInit();
            this.hdrPayments.ResumeLayout(false);
            this.tabOutstanding.ResumeLayout(false);
            this.bodyOutstanding.ResumeLayout(false);
            this.gridCardOutstanding.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gridOutstanding)).EndInit();
            this.hdrOutstanding.ResumeLayout(false);
            this.filterCard.ResumeLayout(false);
            this.filterLayout.ResumeLayout(false);
            this.pnlFromWrap.ResumeLayout(false);
            this.pnlToWrap.ResumeLayout(false);
            this.root.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel tabsCard;
        private System.Windows.Forms.TabControl tabs;
        private System.Windows.Forms.TabPage tabInvoices;
        private System.Windows.Forms.Panel bodyInvoices;
        private System.Windows.Forms.Panel gridCardInvoices;
        private System.Windows.Forms.DataGridView gridInvoices;
        private System.Windows.Forms.Panel hdrInvoices;
        private System.Windows.Forms.Label lblHdrInvoices;
        private System.Windows.Forms.Button btnAllInvoices;
        private System.Windows.Forms.TabPage tabPayments;
        private System.Windows.Forms.Panel bodyPayments;
        private System.Windows.Forms.Panel gridCardPayments;
        private System.Windows.Forms.DataGridView gridPayments;
        private System.Windows.Forms.Panel hdrPayments;
        private System.Windows.Forms.Label lblHdrPayments;
        private System.Windows.Forms.Button btnAllPayments;
        private System.Windows.Forms.TabPage tabOutstanding;
        private System.Windows.Forms.Panel bodyOutstanding;
        private System.Windows.Forms.Panel gridCardOutstanding;
        private System.Windows.Forms.DataGridView gridOutstanding;
        private System.Windows.Forms.Panel hdrOutstanding;
        private System.Windows.Forms.Label lblHdrOutstanding;
        private System.Windows.Forms.Button btnAllOutstanding;
        private System.Windows.Forms.FlowLayoutPanel kpiPanel;
        private System.Windows.Forms.Label lblPeriod;
        private System.Windows.Forms.Panel filterCard;
        private System.Windows.Forms.TableLayoutPanel filterLayout;
        private System.Windows.Forms.Button btnStatement;
        private System.Windows.Forms.Button btnQuickPay;
        private System.Windows.Forms.Button btnQuickReading;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Panel pnlFromWrap;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.Panel pnlToWrap;
        private System.Windows.Forms.DateTimePicker dtTo;
        private System.Windows.Forms.Label lblPeriodText;
        private System.Windows.Forms.ComboBox cbPeriodPreset;
        private System.Windows.Forms.TableLayoutPanel root;
    }
}
*/