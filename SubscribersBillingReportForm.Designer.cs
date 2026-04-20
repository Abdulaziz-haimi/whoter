namespace water3
{
    partial class SubscribersBillingReportForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel mainLayout;

        // Top tools
        private System.Windows.Forms.FlowLayoutPanel topPanel;
        private System.Windows.Forms.Label lblSearchTitle;
        private System.Windows.Forms.TextBox txtSearch;

        private System.Windows.Forms.Label lblFromTitle;
        private System.Windows.Forms.DateTimePicker dtFrom;

        private System.Windows.Forms.Label lblToTitle;
        private System.Windows.Forms.DateTimePicker dtTo;

        private System.Windows.Forms.Label lblReportTypeTitle;
        private System.Windows.Forms.ComboBox ddlReportType;

        private System.Windows.Forms.Label lblFilterModeTitle;
        private System.Windows.Forms.ComboBox ddlFilterMode;

        private System.Windows.Forms.ComboBox ddlCollectors;
        private System.Windows.Forms.TextBox txtMeterFilter;

        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Button btnPrint;

        // UserControl
        private water3.ReportOptionsPanel reportOptions;

        // Grid
        private System.Windows.Forms.DataGridView dgv;

        // Totals
        private System.Windows.Forms.TableLayoutPanel totalsPanel;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Label lblTotalConsumption;
        private System.Windows.Forms.Label lblTotalInvoices;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.topPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSearchTitle = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.lblFromTitle = new System.Windows.Forms.Label();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.lblToTitle = new System.Windows.Forms.Label();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.lblReportTypeTitle = new System.Windows.Forms.Label();
            this.ddlReportType = new System.Windows.Forms.ComboBox();
            this.lblFilterModeTitle = new System.Windows.Forms.Label();
            this.ddlFilterMode = new System.Windows.Forms.ComboBox();
            this.ddlCollectors = new System.Windows.Forms.ComboBox();
            this.txtMeterFilter = new System.Windows.Forms.TextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnExportExcel = new System.Windows.Forms.Button();
            this.btnPrint = new System.Windows.Forms.Button();
            this.reportOptions = new water3.ReportOptionsPanel();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.totalsPanel = new System.Windows.Forms.TableLayoutPanel();
            this.lblCount = new System.Windows.Forms.Label();
            this.lblTotalConsumption = new System.Windows.Forms.Label();
            this.lblTotalInvoices = new System.Windows.Forms.Label();
            this.mainLayout.SuspendLayout();
            this.topPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.totalsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.BackColor = System.Drawing.Color.White;
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainLayout.Controls.Add(this.topPanel, 0, 0);
            this.mainLayout.Controls.Add(this.reportOptions, 0, 1);
            this.mainLayout.Controls.Add(this.dgv, 0, 2);
            this.mainLayout.Controls.Add(this.totalsPanel, 0, 3);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new System.Windows.Forms.Padding(15);
            this.mainLayout.RowCount = 4;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 65F));
            this.mainLayout.Size = new System.Drawing.Size(1093, 553);
            this.mainLayout.TabIndex = 0;
            // 
            // topPanel
            // 
            this.topPanel.AutoScroll = true;
            this.topPanel.Controls.Add(this.lblSearchTitle);
            this.topPanel.Controls.Add(this.txtSearch);
            this.topPanel.Controls.Add(this.lblFromTitle);
            this.topPanel.Controls.Add(this.dtFrom);
            this.topPanel.Controls.Add(this.lblToTitle);
            this.topPanel.Controls.Add(this.dtTo);
            this.topPanel.Controls.Add(this.lblReportTypeTitle);
            this.topPanel.Controls.Add(this.ddlReportType);
            this.topPanel.Controls.Add(this.lblFilterModeTitle);
            this.topPanel.Controls.Add(this.ddlFilterMode);
            this.topPanel.Controls.Add(this.ddlCollectors);
            this.topPanel.Controls.Add(this.txtMeterFilter);
            this.topPanel.Controls.Add(this.btnApply);
            this.topPanel.Controls.Add(this.btnRefresh);
            this.topPanel.Controls.Add(this.btnExportExcel);
            this.topPanel.Controls.Add(this.btnPrint);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.topPanel.Location = new System.Drawing.Point(18, 18);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.topPanel.Size = new System.Drawing.Size(1057, 89);
            this.topPanel.TabIndex = 0;
            this.topPanel.WrapContents = false;
            // 
            // lblSearchTitle
            // 
            this.lblSearchTitle.AutoSize = true;
            this.lblSearchTitle.Location = new System.Drawing.Point(8, 16);
            this.lblSearchTitle.Margin = new System.Windows.Forms.Padding(0, 6, 8, 0);
            this.lblSearchTitle.Name = "lblSearchTitle";
            this.lblSearchTitle.Size = new System.Drawing.Size(29, 13);
            this.lblSearchTitle.TabIndex = 0;
            this.lblSearchTitle.Text = "بحث:";
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(40, 13);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(220, 20);
            this.txtSearch.TabIndex = 1;
            // 
            // lblFromTitle
            // 
            this.lblFromTitle.AutoSize = true;
            this.lblFromTitle.Location = new System.Drawing.Point(271, 16);
            this.lblFromTitle.Margin = new System.Windows.Forms.Padding(18, 6, 8, 0);
            this.lblFromTitle.Name = "lblFromTitle";
            this.lblFromTitle.Size = new System.Drawing.Size(22, 13);
            this.lblFromTitle.TabIndex = 2;
            this.lblFromTitle.Text = "من:";
            // 
            // dtFrom
            // 
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFrom.Location = new System.Drawing.Point(314, 13);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(125, 20);
            this.dtFrom.TabIndex = 3;
            // 
            // lblToTitle
            // 
            this.lblToTitle.AutoSize = true;
            this.lblToTitle.Location = new System.Drawing.Point(450, 16);
            this.lblToTitle.Margin = new System.Windows.Forms.Padding(10, 6, 8, 0);
            this.lblToTitle.Name = "lblToTitle";
            this.lblToTitle.Size = new System.Drawing.Size(27, 13);
            this.lblToTitle.TabIndex = 4;
            this.lblToTitle.Text = "إلى:";
            // 
            // dtTo
            // 
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTo.Location = new System.Drawing.Point(490, 13);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(125, 20);
            this.dtTo.TabIndex = 5;
            // 
            // lblReportTypeTitle
            // 
            this.lblReportTypeTitle.AutoSize = true;
            this.lblReportTypeTitle.Location = new System.Drawing.Point(626, 16);
            this.lblReportTypeTitle.Margin = new System.Windows.Forms.Padding(18, 6, 8, 0);
            this.lblReportTypeTitle.Name = "lblReportTypeTitle";
            this.lblReportTypeTitle.Size = new System.Drawing.Size(58, 13);
            this.lblReportTypeTitle.TabIndex = 6;
            this.lblReportTypeTitle.Text = "نوع العرض:";
            // 
            // ddlReportType
            // 
            this.ddlReportType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlReportType.Items.AddRange(new object[] {
            "آخر فاتورة لكل مشترك (داخل الفترة)",
            "كل الفواتير داخل الفترة (تفصيلي)"});
            this.ddlReportType.Location = new System.Drawing.Point(705, 13);
            this.ddlReportType.Name = "ddlReportType";
            this.ddlReportType.Size = new System.Drawing.Size(245, 21);
            this.ddlReportType.TabIndex = 7;
            // 
            // lblFilterModeTitle
            // 
            this.lblFilterModeTitle.AutoSize = true;
            this.lblFilterModeTitle.Location = new System.Drawing.Point(961, 16);
            this.lblFilterModeTitle.Margin = new System.Windows.Forms.Padding(18, 6, 8, 0);
            this.lblFilterModeTitle.Name = "lblFilterModeTitle";
            this.lblFilterModeTitle.Size = new System.Drawing.Size(34, 13);
            this.lblFilterModeTitle.TabIndex = 8;
            this.lblFilterModeTitle.Text = "فلترة:";
            // 
            // ddlFilterMode
            // 
            this.ddlFilterMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlFilterMode.Items.AddRange(new object[] {
            "بدون",
            "حسب المحصل",
            "حسب العداد"});
            this.ddlFilterMode.Location = new System.Drawing.Point(1016, 13);
            this.ddlFilterMode.Name = "ddlFilterMode";
            this.ddlFilterMode.Size = new System.Drawing.Size(150, 21);
            this.ddlFilterMode.TabIndex = 9;
            // 
            // ddlCollectors
            // 
            this.ddlCollectors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlCollectors.Location = new System.Drawing.Point(1172, 13);
            this.ddlCollectors.Name = "ddlCollectors";
            this.ddlCollectors.Size = new System.Drawing.Size(220, 21);
            this.ddlCollectors.TabIndex = 10;
            this.ddlCollectors.Visible = false;
            // 
            // txtMeterFilter
            // 
            this.txtMeterFilter.Location = new System.Drawing.Point(1398, 13);
            this.txtMeterFilter.Name = "txtMeterFilter";
            this.txtMeterFilter.Size = new System.Drawing.Size(170, 20);
            this.txtMeterFilter.TabIndex = 11;
            this.txtMeterFilter.Visible = false;
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(1571, 10);
            this.btnApply.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(90, 34);
            this.btnApply.TabIndex = 12;
            this.btnApply.Text = "تطبيق";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(1671, 10);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(90, 34);
            this.btnRefresh.TabIndex = 13;
            this.btnRefresh.Text = "تحديث";
            // 
            // btnExportExcel
            // 
            this.btnExportExcel.Location = new System.Drawing.Point(1771, 10);
            this.btnExportExcel.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnExportExcel.Name = "btnExportExcel";
            this.btnExportExcel.Size = new System.Drawing.Size(120, 34);
            this.btnExportExcel.TabIndex = 14;
            this.btnExportExcel.Text = "تصدير CSV";
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(1901, 10);
            this.btnPrint.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(90, 34);
            this.btnPrint.TabIndex = 15;
            this.btnPrint.Text = "طباعة";
            // 
            // reportOptions
            // 
            this.reportOptions.BackColor = System.Drawing.Color.White;
            this.reportOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportOptions.Location = new System.Drawing.Point(18, 113);
            this.reportOptions.Name = "reportOptions";
            this.reportOptions.Size = new System.Drawing.Size(1057, 114);
            this.reportOptions.TabIndex = 1;
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.BackgroundColor = System.Drawing.Color.White;
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(18, 233);
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowTemplate.Height = 36;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(1057, 237);
            this.dgv.TabIndex = 2;
            // 
            // totalsPanel
            // 
            this.totalsPanel.ColumnCount = 3;
            this.totalsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.totalsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.totalsPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.totalsPanel.Controls.Add(this.lblCount, 0, 0);
            this.totalsPanel.Controls.Add(this.lblTotalConsumption, 1, 0);
            this.totalsPanel.Controls.Add(this.lblTotalInvoices, 2, 0);
            this.totalsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.totalsPanel.Location = new System.Drawing.Point(18, 476);
            this.totalsPanel.Name = "totalsPanel";
            this.totalsPanel.Padding = new System.Windows.Forms.Padding(10);
            this.totalsPanel.RowCount = 1;
            this.totalsPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.totalsPanel.Size = new System.Drawing.Size(1057, 59);
            this.totalsPanel.TabIndex = 3;
            // 
            // lblCount
            // 
            this.lblCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCount.Location = new System.Drawing.Point(705, 10);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(339, 39);
            this.lblCount.TabIndex = 0;
            this.lblCount.Text = "عدد السجلات: 0";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalConsumption
            // 
            this.lblTotalConsumption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotalConsumption.Location = new System.Drawing.Point(360, 10);
            this.lblTotalConsumption.Name = "lblTotalConsumption";
            this.lblTotalConsumption.Size = new System.Drawing.Size(339, 39);
            this.lblTotalConsumption.TabIndex = 1;
            this.lblTotalConsumption.Text = "إجمالي الاستهلاك: 0.00";
            this.lblTotalConsumption.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotalInvoices
            // 
            this.lblTotalInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTotalInvoices.Location = new System.Drawing.Point(13, 10);
            this.lblTotalInvoices.Name = "lblTotalInvoices";
            this.lblTotalInvoices.Size = new System.Drawing.Size(341, 39);
            this.lblTotalInvoices.TabIndex = 2;
            this.lblTotalInvoices.Text = "إجمالي الفواتير: 0.00";
            this.lblTotalInvoices.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SubscribersBillingReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1093, 553);
            this.Controls.Add(this.mainLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SubscribersBillingReportForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "SubscribersBillingReportForm";
            this.mainLayout.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.totalsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
