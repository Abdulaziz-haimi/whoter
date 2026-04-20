namespace water3.Forms
{
  
        partial class AccountStatementForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel root;

        private System.Windows.Forms.Panel pnlFiltersCard;
        private System.Windows.Forms.TableLayoutPanel filtersLayout;
        private System.Windows.Forms.FlowLayoutPanel flpTop;
        private System.Windows.Forms.FlowLayoutPanel flpOptions;

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Button btnPrint;

        private System.Windows.Forms.Label lblReportType;
        private System.Windows.Forms.ComboBox ddlReportType;

        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.DateTimePicker dtFrom;

        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.DateTimePicker dtTo;

        private System.Windows.Forms.Label lblSubscriber;
        private System.Windows.Forms.ComboBox ddlSubscribers;

        private System.Windows.Forms.Label lblColumns;
        private System.Windows.Forms.CheckedListBox clbColumns;

        private System.Windows.Forms.Label lblSortBy;
        private System.Windows.Forms.ComboBox ddlSortBy;

        private System.Windows.Forms.CheckBox chkSortDesc;

        private System.Windows.Forms.Label lblGroupBy;
        private System.Windows.Forms.ComboBox ddlGroupBy;

        private System.Windows.Forms.CheckBox chkOnlyInvoices;
        private System.Windows.Forms.CheckBox chkOnlyPayments;

        private System.Windows.Forms.Button btnSavePreset;
        private System.Windows.Forms.Button btnLoadPreset;

        private System.Windows.Forms.Panel pnlInfoCard;
        private System.Windows.Forms.Label lblSubscriberInfo;

        private System.Windows.Forms.Panel pnlGridCard;
        private System.Windows.Forms.DataGridView dgv;

        private System.Windows.Forms.Panel pnlTotalsCard;
        private System.Windows.Forms.TableLayoutPanel totalsLayout;
        private System.Windows.Forms.Label lblOpen;
        private System.Windows.Forms.Label lblDebitTotal;
        private System.Windows.Forms.Label lblCreditTotal;
        private System.Windows.Forms.Label lblNet;
        private System.Windows.Forms.Label lblClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.root = new System.Windows.Forms.TableLayoutPanel();
            this.pnlFiltersCard = new System.Windows.Forms.Panel();
            this.filtersLayout = new System.Windows.Forms.TableLayoutPanel();
            this.flpTop = new System.Windows.Forms.FlowLayoutPanel();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.lblReportType = new System.Windows.Forms.Label();
            this.ddlReportType = new System.Windows.Forms.ComboBox();
            this.lblFrom = new System.Windows.Forms.Label();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.lblSubscriber = new System.Windows.Forms.Label();
            this.ddlSubscribers = new System.Windows.Forms.ComboBox();
            this.flpOptions = new System.Windows.Forms.FlowLayoutPanel();
            this.lblColumns = new System.Windows.Forms.Label();
            this.clbColumns = new System.Windows.Forms.CheckedListBox();
            this.lblSortBy = new System.Windows.Forms.Label();
            this.ddlSortBy = new System.Windows.Forms.ComboBox();
            this.chkSortDesc = new System.Windows.Forms.CheckBox();
            this.lblGroupBy = new System.Windows.Forms.Label();
            this.ddlGroupBy = new System.Windows.Forms.ComboBox();
            this.chkOnlyInvoices = new System.Windows.Forms.CheckBox();
            this.chkOnlyPayments = new System.Windows.Forms.CheckBox();
            this.btnSavePreset = new System.Windows.Forms.Button();
            this.btnLoadPreset = new System.Windows.Forms.Button();
            this.pnlInfoCard = new System.Windows.Forms.Panel();
            this.lblSubscriberInfo = new System.Windows.Forms.Label();
            this.pnlGridCard = new System.Windows.Forms.Panel();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.pnlTotalsCard = new System.Windows.Forms.Panel();
            this.totalsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblOpen = new System.Windows.Forms.Label();
            this.lblDebitTotal = new System.Windows.Forms.Label();
            this.lblCreditTotal = new System.Windows.Forms.Label();
            this.lblNet = new System.Windows.Forms.Label();
            this.lblClose = new System.Windows.Forms.Label();
            this.root.SuspendLayout();
            this.pnlFiltersCard.SuspendLayout();
            this.filtersLayout.SuspendLayout();
            this.flpTop.SuspendLayout();
            this.flpOptions.SuspendLayout();
            this.pnlInfoCard.SuspendLayout();
            this.pnlGridCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.pnlTotalsCard.SuspendLayout();
            this.totalsLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // root
            // 
            this.root.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.root.ColumnCount = 1;
            this.root.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.root.Controls.Add(this.pnlFiltersCard, 0, 0);
            this.root.Controls.Add(this.pnlInfoCard, 0, 1);
            this.root.Controls.Add(this.pnlGridCard, 0, 2);
            this.root.Controls.Add(this.pnlTotalsCard, 0, 3);
            this.root.Dock = System.Windows.Forms.DockStyle.Fill;
            this.root.Location = new System.Drawing.Point(0, 0);
            this.root.Name = "root";
            this.root.Padding = new System.Windows.Forms.Padding(12);
            this.root.RowCount = 4;
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 158F));
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 33F));
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.root.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 78F));
            this.root.Size = new System.Drawing.Size(1093, 553);
            this.root.TabIndex = 0;

            // 
            // pnlFiltersCard
            // 
            this.pnlFiltersCard.BackColor = System.Drawing.Color.White;
            this.pnlFiltersCard.Controls.Add(this.filtersLayout);
            this.pnlFiltersCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFiltersCard.Location = new System.Drawing.Point(15, 15);
            this.pnlFiltersCard.Name = "pnlFiltersCard";
            this.pnlFiltersCard.Padding = new System.Windows.Forms.Padding(10);
            this.pnlFiltersCard.Size = new System.Drawing.Size(1063, 152);
            this.pnlFiltersCard.TabIndex = 0;
            // 
            // filtersLayout
            // 
            this.filtersLayout.ColumnCount = 1;
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1043F));
            this.filtersLayout.Controls.Add(this.flpTop, 0, 0);
            this.filtersLayout.Controls.Add(this.flpOptions, 0, 1);
            this.filtersLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filtersLayout.Location = new System.Drawing.Point(10, 10);
            this.filtersLayout.Name = "filtersLayout";
            this.filtersLayout.RowCount = 2;
            this.filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 49F));
            this.filtersLayout.Size = new System.Drawing.Size(1043, 132);
            this.filtersLayout.TabIndex = 0;
            // 
            // flpTop
            // 
            this.flpTop.AutoScroll = true;
            this.flpTop.BackColor = System.Drawing.Color.White;
            this.flpTop.Controls.Add(this.btnApply);
            this.flpTop.Controls.Add(this.btnRefresh);
            this.flpTop.Controls.Add(this.btnExportExcel);
            this.flpTop.Controls.Add(this.btnPrint);
            this.flpTop.Controls.Add(this.lblReportType);
            this.flpTop.Controls.Add(this.ddlReportType);
            this.flpTop.Controls.Add(this.lblFrom);
            this.flpTop.Controls.Add(this.dtFrom);
            this.flpTop.Controls.Add(this.lblTo);
            this.flpTop.Controls.Add(this.dtTo);
            this.flpTop.Controls.Add(this.lblSubscriber);
            this.flpTop.Controls.Add(this.ddlSubscribers);
            this.flpTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpTop.Location = new System.Drawing.Point(3, 3);
            this.flpTop.Name = "flpTop";
            this.flpTop.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.flpTop.Size = new System.Drawing.Size(1037, 59);
            this.flpTop.TabIndex = 0;
            this.flpTop.WrapContents = false;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(992, 11);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(42, 36);
            this.btnApply.TabIndex = 0;
            this.btnApply.Text = "✔";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(944, 11);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(42, 36);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "🔄";
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Location = new System.Drawing.Point(896, 11);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(42, 36);
            this.btnExportExcel.TabIndex = 2;
            this.btnExportExcel.Text = "⬇";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(848, 11);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(42, 36);
            this.btnPrint.TabIndex = 3;
            this.btnPrint.Text = "🖨";
            // 
            // lblReportType
            // 
            this.lblReportType.AutoSize = true;
            this.lblReportType.Location = new System.Drawing.Point(776, 15);
            this.lblReportType.Margin = new System.Windows.Forms.Padding(10, 7, 6, 0);
            this.lblReportType.Name = "lblReportType";
            this.lblReportType.Size = new System.Drawing.Size(59, 13);
            this.lblReportType.TabIndex = 4;
            this.lblReportType.Text = "نوع التقرير:";
            // 
            // ddlReportType
            // 
            this.ddlReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlReportType.Location = new System.Drawing.Point(627, 11);
            this.ddlReportType.Name = "ddlReportType";
            this.ddlReportType.Size = new System.Drawing.Size(140, 21);
            this.ddlReportType.TabIndex = 5;
            // 
            // lblFrom
            // 
            this.lblFrom.AutoSize = true;
            this.lblFrom.Location = new System.Drawing.Point(592, 15);
            this.lblFrom.Margin = new System.Windows.Forms.Padding(10, 7, 6, 0);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(22, 13);
            this.lblFrom.TabIndex = 6;
            this.lblFrom.Text = "من:";
            // 
            // dtFrom
            // 
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFrom.Location = new System.Drawing.Point(453, 11);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(130, 20);
            this.dtFrom.TabIndex = 7;
            // 
            // lblTo
            // 
            this.lblTo.AutoSize = true;
            this.lblTo.Location = new System.Drawing.Point(413, 15);
            this.lblTo.Margin = new System.Windows.Forms.Padding(10, 7, 6, 0);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(27, 13);
            this.lblTo.TabIndex = 8;
            this.lblTo.Text = "إلى:";
            // 
            // dtTo
            // 
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTo.Location = new System.Drawing.Point(274, 11);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(130, 20);
            this.dtTo.TabIndex = 9;
            // 
            // lblSubscriber
            // 
            this.lblSubscriber.AutoSize = true;
            this.lblSubscriber.Location = new System.Drawing.Point(190, 15);
            this.lblSubscriber.Margin = new System.Windows.Forms.Padding(10, 7, 6, 0);
            this.lblSubscriber.Name = "lblSubscriber";
            this.lblSubscriber.Size = new System.Drawing.Size(71, 13);
            this.lblSubscriber.TabIndex = 10;
            this.lblSubscriber.Text = "بحث/المشترك:";
            // 
            // ddlSubscribers
            // 
            this.ddlSubscribers.Location = new System.Drawing.Point(13, 11);
            this.ddlSubscribers.Name = "ddlSubscribers";
            this.ddlSubscribers.Size = new System.Drawing.Size(168, 21);
            this.ddlSubscribers.TabIndex = 11;
            // 
            // flpOptions
            // 
            this.flpOptions.AutoScroll = true;
            this.flpOptions.BackColor = System.Drawing.Color.White;
            this.flpOptions.Controls.Add(this.lblColumns);
            this.flpOptions.Controls.Add(this.clbColumns);
            this.flpOptions.Controls.Add(this.lblSortBy);
            this.flpOptions.Controls.Add(this.ddlSortBy);
            this.flpOptions.Controls.Add(this.chkSortDesc);
            this.flpOptions.Controls.Add(this.lblGroupBy);
            this.flpOptions.Controls.Add(this.ddlGroupBy);
            this.flpOptions.Controls.Add(this.chkOnlyInvoices);
            this.flpOptions.Controls.Add(this.chkOnlyPayments);
            this.flpOptions.Controls.Add(this.btnSavePreset);
            this.flpOptions.Controls.Add(this.btnLoadPreset);
            this.flpOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flpOptions.Location = new System.Drawing.Point(3, 68);
            this.flpOptions.Name = "flpOptions";
            this.flpOptions.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.flpOptions.Size = new System.Drawing.Size(1037, 61);
            this.flpOptions.TabIndex = 1;
            this.flpOptions.WrapContents = false;
            // 
            // lblColumns
            // 
            this.lblColumns.AutoSize = true;
            this.lblColumns.Location = new System.Drawing.Point(985, 13);
            this.lblColumns.Margin = new System.Windows.Forms.Padding(10, 7, 6, 0);
            this.lblColumns.Name = "lblColumns";
            this.lblColumns.Size = new System.Drawing.Size(42, 13);
            this.lblColumns.TabIndex = 0;
            this.lblColumns.Text = "الأعمدة:";
            // 
            // clbColumns
            // 
            this.clbColumns.CheckOnClick = true;
            this.clbColumns.Location = new System.Drawing.Point(793, 9);
            this.clbColumns.Name = "clbColumns";
            this.clbColumns.Size = new System.Drawing.Size(183, 49);
            this.clbColumns.TabIndex = 1;
            // 
            // lblSortBy
            // 
            this.lblSortBy.AutoSize = true;
            this.lblSortBy.Location = new System.Drawing.Point(718, 13);
            this.lblSortBy.Margin = new System.Windows.Forms.Padding(10, 7, 6, 0);
            this.lblSortBy.Name = "lblSortBy";
            this.lblSortBy.Size = new System.Drawing.Size(62, 13);
            this.lblSortBy.TabIndex = 2;
            this.lblSortBy.Text = "ترتيب حسب:";
            // 
            // ddlSortBy
            // 
            this.ddlSortBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlSortBy.Location = new System.Drawing.Point(559, 9);
            this.ddlSortBy.Name = "ddlSortBy";
            this.ddlSortBy.Size = new System.Drawing.Size(150, 21);
            this.ddlSortBy.TabIndex = 3;
            // 
            // chkSortDesc
            // 
            this.chkSortDesc.AutoSize = true;
            this.chkSortDesc.Location = new System.Drawing.Point(492, 14);
            this.chkSortDesc.Margin = new System.Windows.Forms.Padding(8, 8, 0, 0);
            this.chkSortDesc.Name = "chkSortDesc";
            this.chkSortDesc.Size = new System.Drawing.Size(56, 17);
            this.chkSortDesc.TabIndex = 4;
            this.chkSortDesc.Text = "تنازلي";
            // 
            // lblGroupBy
            // 
            this.lblGroupBy.AutoSize = true;
            this.lblGroupBy.Location = new System.Drawing.Point(445, 13);
            this.lblGroupBy.Margin = new System.Windows.Forms.Padding(10, 7, 6, 0);
            this.lblGroupBy.Name = "lblGroupBy";
            this.lblGroupBy.Size = new System.Drawing.Size(37, 13);
            this.lblGroupBy.TabIndex = 5;
            this.lblGroupBy.Text = "تجميع:";
            // 
            // ddlGroupBy
            // 
            this.ddlGroupBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlGroupBy.Location = new System.Drawing.Point(286, 9);
            this.ddlGroupBy.Name = "ddlGroupBy";
            this.ddlGroupBy.Size = new System.Drawing.Size(150, 21);
            this.ddlGroupBy.TabIndex = 6;
            // 
            // chkOnlyInvoices
            // 
            this.chkOnlyInvoices.AutoSize = true;
            this.chkOnlyInvoices.Location = new System.Drawing.Point(200, 14);
            this.chkOnlyInvoices.Margin = new System.Windows.Forms.Padding(8, 8, 0, 0);
            this.chkOnlyInvoices.Name = "chkOnlyInvoices";
            this.chkOnlyInvoices.Size = new System.Drawing.Size(75, 17);
            this.chkOnlyInvoices.TabIndex = 7;
            this.chkOnlyInvoices.Text = "فواتير فقط";
            // 
            // chkOnlyPayments
            // 
            this.chkOnlyPayments.AutoSize = true;
            this.chkOnlyPayments.Location = new System.Drawing.Point(104, 14);
            this.chkOnlyPayments.Margin = new System.Windows.Forms.Padding(8, 8, 0, 0);
            this.chkOnlyPayments.Name = "chkOnlyPayments";
            this.chkOnlyPayments.Size = new System.Drawing.Size(88, 17);
            this.chkOnlyPayments.TabIndex = 8;
            this.chkOnlyPayments.Text = "مدفوعات فقط";
            // 
            // btnSavePreset
            // 
            this.btnSavePreset.Location = new System.Drawing.Point(59, 9);
            this.btnSavePreset.Name = "btnSavePreset";
            this.btnSavePreset.Size = new System.Drawing.Size(42, 36);
            this.btnSavePreset.TabIndex = 9;
            this.btnSavePreset.Text = "💾";
            // 
            // btnLoadPreset
            // 
            this.btnLoadPreset.Location = new System.Drawing.Point(11, 9);
            this.btnLoadPreset.Name = "btnLoadPreset";
            this.btnLoadPreset.Size = new System.Drawing.Size(42, 36);
            this.btnLoadPreset.TabIndex = 10;
            this.btnLoadPreset.Text = "📂";
            // 
            // pnlInfoCard
            // 
            this.pnlInfoCard.BackColor = System.Drawing.Color.White;
            this.pnlInfoCard.Controls.Add(this.lblSubscriberInfo);
            this.pnlInfoCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlInfoCard.Location = new System.Drawing.Point(15, 173);
            this.pnlInfoCard.Name = "pnlInfoCard";
            this.pnlInfoCard.Padding = new System.Windows.Forms.Padding(8);
            this.pnlInfoCard.Size = new System.Drawing.Size(1063, 27);
            this.pnlInfoCard.TabIndex = 1;
            // 
            // lblSubscriberInfo
            // 
            this.lblSubscriberInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSubscriberInfo.Location = new System.Drawing.Point(8, 8);
            this.lblSubscriberInfo.Name = "lblSubscriberInfo";
            this.lblSubscriberInfo.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblSubscriberInfo.Size = new System.Drawing.Size(1047, 11);
            this.lblSubscriberInfo.TabIndex = 0;
            this.lblSubscriberInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlGridCard
            // 
            this.pnlGridCard.BackColor = System.Drawing.Color.White;
            this.pnlGridCard.Controls.Add(this.dgv);
            this.pnlGridCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGridCard.Location = new System.Drawing.Point(15, 206);
            this.pnlGridCard.Name = "pnlGridCard";
            this.pnlGridCard.Padding = new System.Windows.Forms.Padding(8);
            this.pnlGridCard.Size = new System.Drawing.Size(1063, 254);
            this.pnlGridCard.TabIndex = 2;
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.BackgroundColor = System.Drawing.Color.White;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(8, 8);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersVisible = false;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(1047, 238);
            this.dgv.TabIndex = 0;
            // 
            // pnlTotalsCard
            // 
            this.pnlTotalsCard.BackColor = System.Drawing.Color.White;
            this.pnlTotalsCard.Controls.Add(this.totalsLayout);
            this.pnlTotalsCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTotalsCard.Location = new System.Drawing.Point(15, 466);
            this.pnlTotalsCard.Name = "pnlTotalsCard";
            this.pnlTotalsCard.Padding = new System.Windows.Forms.Padding(10);
            this.pnlTotalsCard.Size = new System.Drawing.Size(1063, 72);
            this.pnlTotalsCard.TabIndex = 3;
            // 
            // totalsLayout
            // 
            this.totalsLayout.ColumnCount = 5;
            this.totalsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.totalsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.totalsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.totalsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.totalsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.totalsLayout.Controls.Add(this.lblOpen, 0, 0);
            this.totalsLayout.Controls.Add(this.lblDebitTotal, 1, 0);
            this.totalsLayout.Controls.Add(this.lblCreditTotal, 2, 0);
            this.totalsLayout.Controls.Add(this.lblNet, 3, 0);
            this.totalsLayout.Controls.Add(this.lblClose, 4, 0);
            this.totalsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.totalsLayout.Location = new System.Drawing.Point(10, 10);
            this.totalsLayout.Name = "totalsLayout";
            this.totalsLayout.RowCount = 1;
            this.totalsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.totalsLayout.Size = new System.Drawing.Size(1043, 52);
            this.totalsLayout.TabIndex = 0;
            // 
            // lblOpen
            // 
            this.lblOpen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblOpen.Location = new System.Drawing.Point(838, 0);
            this.lblOpen.Name = "lblOpen";
            this.lblOpen.Size = new System.Drawing.Size(202, 52);
            this.lblOpen.TabIndex = 0;
            this.lblOpen.Text = "افتتاحي: 0.00";
            this.lblOpen.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDebitTotal
            // 
            this.lblDebitTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDebitTotal.Location = new System.Drawing.Point(630, 0);
            this.lblDebitTotal.Name = "lblDebitTotal";
            this.lblDebitTotal.Size = new System.Drawing.Size(202, 52);
            this.lblDebitTotal.TabIndex = 1;
            this.lblDebitTotal.Text = "إجمالي مدين: 0.00";
            this.lblDebitTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCreditTotal
            // 
            this.lblCreditTotal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCreditTotal.Location = new System.Drawing.Point(422, 0);
            this.lblCreditTotal.Name = "lblCreditTotal";
            this.lblCreditTotal.Size = new System.Drawing.Size(202, 52);
            this.lblCreditTotal.TabIndex = 2;
            this.lblCreditTotal.Text = "إجمالي دائن: 0.00";
            this.lblCreditTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNet
            // 
            this.lblNet.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNet.Location = new System.Drawing.Point(214, 0);
            this.lblNet.Name = "lblNet";
            this.lblNet.Size = new System.Drawing.Size(202, 52);
            this.lblNet.TabIndex = 3;
            this.lblNet.Text = "الصافي: 0.00";
            this.lblNet.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblClose
            // 
            this.lblClose.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblClose.Location = new System.Drawing.Point(3, 0);
            this.lblClose.Name = "lblClose";
            this.lblClose.Size = new System.Drawing.Size(205, 52);
            this.lblClose.TabIndex = 4;
            this.lblClose.Text = "ختامي: 0.00";
            this.lblClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AccountStatementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1093, 553);
            this.Controls.Add(this.root);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "AccountStatementForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "كشف حساب المشترك";
            this.root.ResumeLayout(false);
            this.pnlFiltersCard.ResumeLayout(false);
            this.filtersLayout.ResumeLayout(false);
            this.flpTop.ResumeLayout(false);
            this.flpTop.PerformLayout();
            this.flpOptions.ResumeLayout(false);
            this.flpOptions.PerformLayout();
            this.pnlInfoCard.ResumeLayout(false);
            this.pnlGridCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.pnlTotalsCard.ResumeLayout(false);
            this.totalsLayout.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
