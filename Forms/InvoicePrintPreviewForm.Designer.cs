using System.Windows.Forms;
using water3.Services;

namespace water3.Forms
{
    partial class InvoicePrintPreviewForm
    {
        private System.ComponentModel.IContainer components = null;

        // ====== Controls (Designer) ======
        private System.Windows.Forms.TableLayoutPanel mainContainer;

        private System.Windows.Forms.Panel cardFilterStats;
        private System.Windows.Forms.TableLayoutPanel rootFilterStats;
        private System.Windows.Forms.TableLayoutPanel filtersLayout;
        private System.Windows.Forms.FlowLayoutPanel statsFlow;

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TableLayoutPanel dateLayout;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.DateTimePicker dtTo;

        private System.Windows.Forms.ComboBox cbSubscriberFilter;

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cbStatusFilter;

        private System.Windows.Forms.Label lblPaymentType;
        private System.Windows.Forms.ComboBox cbPaymentTypeFilter;

        private System.Windows.Forms.TextBox txtSearch;

        private System.Windows.Forms.Label lblTotalInvoices;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.Label lblPaidAmount;
        private System.Windows.Forms.Label lblRemainingAmount;

        private System.Windows.Forms.Panel cardToolbar;
        private System.Windows.Forms.FlowLayoutPanel toolbarFlow;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnPrintAll;
        private System.Windows.Forms.Button btnSMS;
        private System.Windows.Forms.Button btnSMSAll;
        private System.Windows.Forms.Button btnExport;

        private System.Windows.Forms.Panel cardContent;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabInvoices;
        private System.Windows.Forms.TabPage tabPayments;
        private System.Windows.Forms.TabPage tabSms;

        private System.Windows.Forms.DataGridView dgvInvoices;
        private System.Windows.Forms.DataGridView dgvPayments;
        private System.Windows.Forms.DataGridView dgvSmsLogs;

        private System.Windows.Forms.Panel loadingPanel;
        private System.Windows.Forms.Panel loadingContainer;
        private System.Windows.Forms.Label lblLoading;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.mainContainer = new System.Windows.Forms.TableLayoutPanel();
            this.cardFilterStats = new System.Windows.Forms.Panel();
            this.rootFilterStats = new System.Windows.Forms.TableLayoutPanel();
            this.filtersLayout = new System.Windows.Forms.TableLayoutPanel();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dateLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblFrom = new System.Windows.Forms.Label();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.cbSubscriberFilter = new System.Windows.Forms.ComboBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cbStatusFilter = new System.Windows.Forms.ComboBox();
            this.lblPaymentType = new System.Windows.Forms.Label();
            this.cbPaymentTypeFilter = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.statsFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTotalInvoices = new System.Windows.Forms.Label();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.lblPaidAmount = new System.Windows.Forms.Label();
            this.lblRemainingAmount = new System.Windows.Forms.Label();
            this.cardToolbar = new System.Windows.Forms.Panel();
            this.toolbarFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnPrintAll = new System.Windows.Forms.Button();
            this.btnSMS = new System.Windows.Forms.Button();
            this.btnSMSAll = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.cardContent = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabInvoices = new System.Windows.Forms.TabPage();
            this.dgvInvoices = new System.Windows.Forms.DataGridView();
            this.tabPayments = new System.Windows.Forms.TabPage();
            this.dgvPayments = new System.Windows.Forms.DataGridView();
            this.tabSms = new System.Windows.Forms.TabPage();
            this.dgvSmsLogs = new System.Windows.Forms.DataGridView();
            this.loadingPanel = new System.Windows.Forms.Panel();
            this.loadingContainer = new System.Windows.Forms.Panel();
            this.lblLoading = new System.Windows.Forms.Label();
            this.mainContainer.SuspendLayout();
            this.cardFilterStats.SuspendLayout();
            this.rootFilterStats.SuspendLayout();
            this.filtersLayout.SuspendLayout();
            this.dateLayout.SuspendLayout();
            this.statsFlow.SuspendLayout();
            this.cardToolbar.SuspendLayout();
            this.toolbarFlow.SuspendLayout();
            this.cardContent.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabInvoices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).BeginInit();
            this.tabPayments.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayments)).BeginInit();
            this.tabSms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSmsLogs)).BeginInit();
            this.loadingPanel.SuspendLayout();
            this.loadingContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainContainer
            // 
            this.mainContainer.ColumnCount = 1;
            this.mainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1180F));
            this.mainContainer.Controls.Add(this.cardFilterStats, 0, 0);
            this.mainContainer.Controls.Add(this.cardToolbar, 0, 1);
            this.mainContainer.Controls.Add(this.cardContent, 0, 2);
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContainer.Location = new System.Drawing.Point(0, 0);
            this.mainContainer.Margin = new System.Windows.Forms.Padding(0);
            this.mainContainer.Name = "mainContainer";
            this.mainContainer.Padding = new System.Windows.Forms.Padding(10);
            this.mainContainer.RowCount = 3;
            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 121F));
            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainContainer.Size = new System.Drawing.Size(1200, 720);
            this.mainContainer.TabIndex = 0;
            // 
            // cardFilterStats
            // 
            this.cardFilterStats.Controls.Add(this.rootFilterStats);
            this.cardFilterStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardFilterStats.Location = new System.Drawing.Point(13, 13);
            this.cardFilterStats.Name = "cardFilterStats";
            this.cardFilterStats.Padding = new System.Windows.Forms.Padding(12);
            this.cardFilterStats.Size = new System.Drawing.Size(1174, 115);
            this.cardFilterStats.TabIndex = 0;
            // 
            // rootFilterStats
            // 
            this.rootFilterStats.ColumnCount = 1;
            this.rootFilterStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1150F));
            this.rootFilterStats.Controls.Add(this.filtersLayout, 0, 0);
            this.rootFilterStats.Controls.Add(this.statsFlow, 0, 1);
            this.rootFilterStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootFilterStats.Location = new System.Drawing.Point(12, 12);
            this.rootFilterStats.Name = "rootFilterStats";
            this.rootFilterStats.RowCount = 2;
            this.rootFilterStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.rootFilterStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            this.rootFilterStats.Size = new System.Drawing.Size(1150, 91);
            this.rootFilterStats.TabIndex = 0;
            // 
            // filtersLayout
            // 
            this.filtersLayout.ColumnCount = 8;
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.filtersLayout.Controls.Add(this.btnSearch, 0, 0);
            this.filtersLayout.Controls.Add(this.dateLayout, 1, 0);
            this.filtersLayout.Controls.Add(this.cbSubscriberFilter, 2, 0);
            this.filtersLayout.Controls.Add(this.lblStatus, 3, 0);
            this.filtersLayout.Controls.Add(this.cbStatusFilter, 4, 0);
            this.filtersLayout.Controls.Add(this.lblPaymentType, 3, 0);
            this.filtersLayout.Controls.Add(this.cbPaymentTypeFilter, 4, 0);
            this.filtersLayout.Controls.Add(this.txtSearch, 6, 0);
            this.filtersLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filtersLayout.Location = new System.Drawing.Point(3, 3);
            this.filtersLayout.Name = "filtersLayout";
            this.filtersLayout.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.filtersLayout.Size = new System.Drawing.Size(1144, 32);
            this.filtersLayout.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSearch.Location = new System.Drawing.Point(1066, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 26);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "🔍 بحث";
            // 
            // dateLayout
            // 
            this.dateLayout.ColumnCount = 4;
            this.dateLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.dateLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dateLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.dateLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dateLayout.Controls.Add(this.lblFrom, 0, 0);
            this.dateLayout.Controls.Add(this.dtFrom, 1, 0);
            this.dateLayout.Controls.Add(this.lblTo, 2, 0);
            this.dateLayout.Controls.Add(this.dtTo, 3, 0);
            this.dateLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dateLayout.Location = new System.Drawing.Point(795, 3);
            this.dateLayout.Name = "dateLayout";
            this.dateLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.dateLayout.Size = new System.Drawing.Size(265, 26);
            this.dateLayout.TabIndex = 1;
            // 
            // lblFrom
            // 
            this.lblFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFrom.Location = new System.Drawing.Point(232, 0);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(30, 26);
            this.lblFrom.TabIndex = 0;
            this.lblFrom.Text = "من:";
            this.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtFrom
            // 
            this.dtFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFrom.Location = new System.Drawing.Point(136, 3);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(90, 20);
            this.dtFrom.TabIndex = 1;
            // 
            // lblTo
            // 
            this.lblTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTo.Location = new System.Drawing.Point(100, 0);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(30, 26);
            this.lblTo.TabIndex = 2;
            this.lblTo.Text = "إلى:";
            this.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtTo
            // 
            this.dtTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTo.Location = new System.Drawing.Point(3, 3);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(91, 20);
            this.dtTo.TabIndex = 3;
            // 
            // cbSubscriberFilter
            // 
            this.cbSubscriberFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbSubscriberFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSubscriberFilter.Location = new System.Drawing.Point(621, 3);
            this.cbSubscriberFilter.Name = "cbSubscriberFilter";
            this.cbSubscriberFilter.Size = new System.Drawing.Size(168, 21);
            this.cbSubscriberFilter.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Location = new System.Drawing.Point(426, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(129, 32);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "الحالة:";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbStatusFilter
            // 
            this.cbStatusFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStatusFilter.Location = new System.Drawing.Point(28, 3);
            this.cbStatusFilter.Name = "cbStatusFilter";
            this.cbStatusFilter.Size = new System.Drawing.Size(382, 21);
            this.cbStatusFilter.TabIndex = 4;
            // 
            // lblPaymentType
            // 
            this.lblPaymentType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPaymentType.Location = new System.Drawing.Point(561, 0);
            this.lblPaymentType.Name = "lblPaymentType";
            this.lblPaymentType.Size = new System.Drawing.Size(54, 32);
            this.lblPaymentType.TabIndex = 5;
            this.lblPaymentType.Text = "الطريقة:";
            this.lblPaymentType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbPaymentTypeFilter
            // 
            this.cbPaymentTypeFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbPaymentTypeFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbPaymentTypeFilter.Location = new System.Drawing.Point(416, 3);
            this.cbPaymentTypeFilter.Name = "cbPaymentTypeFilter";
            this.cbPaymentTypeFilter.Size = new System.Drawing.Size(4, 21);
            this.cbPaymentTypeFilter.TabIndex = 6;
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(3, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(19, 20);
            this.txtSearch.TabIndex = 7;
            // 
            // statsFlow
            // 
            this.statsFlow.Controls.Add(this.lblTotalInvoices);
            this.statsFlow.Controls.Add(this.lblTotalAmount);
            this.statsFlow.Controls.Add(this.lblPaidAmount);
            this.statsFlow.Controls.Add(this.lblRemainingAmount);
            this.statsFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statsFlow.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.statsFlow.Location = new System.Drawing.Point(3, 41);
            this.statsFlow.Name = "statsFlow";
            this.statsFlow.Size = new System.Drawing.Size(1144, 82);
            this.statsFlow.TabIndex = 1;
            this.statsFlow.WrapContents = false;
            // 
            // lblTotalInvoices
            // 
            this.lblTotalInvoices.Location = new System.Drawing.Point(921, 0);
            this.lblTotalInvoices.Name = "lblTotalInvoices";
            this.lblTotalInvoices.Size = new System.Drawing.Size(220, 62);
            this.lblTotalInvoices.TabIndex = 0;
            this.lblTotalInvoices.Text = "📄 إجمالي الفواتير\n0";
            this.lblTotalInvoices.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.Location = new System.Drawing.Point(695, 0);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(220, 62);
            this.lblTotalAmount.TabIndex = 1;
            this.lblTotalAmount.Text = "💰 إجمالي المبالغ\n0";
            this.lblTotalAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPaidAmount
            // 
            this.lblPaidAmount.Location = new System.Drawing.Point(469, 0);
            this.lblPaidAmount.Name = "lblPaidAmount";
            this.lblPaidAmount.Size = new System.Drawing.Size(220, 62);
            this.lblPaidAmount.TabIndex = 2;
            this.lblPaidAmount.Text = "💳 المدفوع\n0";
            this.lblPaidAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblRemainingAmount
            // 
            this.lblRemainingAmount.Location = new System.Drawing.Point(243, 0);
            this.lblRemainingAmount.Name = "lblRemainingAmount";
            this.lblRemainingAmount.Size = new System.Drawing.Size(220, 62);
            this.lblRemainingAmount.TabIndex = 3;
            this.lblRemainingAmount.Text = "⚠️ المتبقي\n0";
            this.lblRemainingAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cardToolbar
            // 
            this.cardToolbar.Controls.Add(this.toolbarFlow);
            this.cardToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardToolbar.Location = new System.Drawing.Point(13, 134);
            this.cardToolbar.Name = "cardToolbar";
            this.cardToolbar.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.cardToolbar.Size = new System.Drawing.Size(1174, 44);
            this.cardToolbar.TabIndex = 1;
            // 
            // toolbarFlow
            // 
            this.toolbarFlow.Controls.Add(this.btnPrint);
            this.toolbarFlow.Controls.Add(this.btnPrintAll);
            this.toolbarFlow.Controls.Add(this.btnSMS);
            this.toolbarFlow.Controls.Add(this.btnSMSAll);
            this.toolbarFlow.Controls.Add(this.btnExport);
            this.toolbarFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolbarFlow.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.toolbarFlow.Location = new System.Drawing.Point(10, 6);
            this.toolbarFlow.Name = "toolbarFlow";
            this.toolbarFlow.Size = new System.Drawing.Size(1154, 32);
            this.toolbarFlow.TabIndex = 0;
            this.toolbarFlow.WrapContents = false;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(1076, 3);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "🖨️ طباعة مختارة";
            // 
            // btnPrintAll
            // 
            this.btnPrintAll.Location = new System.Drawing.Point(995, 3);
            this.btnPrintAll.Name = "btnPrintAll";
            this.btnPrintAll.Size = new System.Drawing.Size(75, 23);
            this.btnPrintAll.TabIndex = 1;
            this.btnPrintAll.Text = "🖨️ طباعة الكل";
            // 
            // btnSMS
            // 
            this.btnSMS.Location = new System.Drawing.Point(914, 3);
            this.btnSMS.Name = "btnSMS";
            this.btnSMS.Size = new System.Drawing.Size(75, 23);
            this.btnSMS.TabIndex = 2;
            this.btnSMS.Text = "📱 SMS مختارة";
            // 
            // btnSMSAll
            // 
            this.btnSMSAll.Location = new System.Drawing.Point(833, 3);
            this.btnSMSAll.Name = "btnSMSAll";
            this.btnSMSAll.Size = new System.Drawing.Size(75, 23);
            this.btnSMSAll.TabIndex = 3;
            this.btnSMSAll.Text = "📱 SMS للكل";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(752, 3);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 4;
            this.btnExport.Text = "📊 تصدير Excel";
            // 
            // cardContent
            // 
            this.cardContent.Controls.Add(this.tabControl);
            this.cardContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardContent.Location = new System.Drawing.Point(13, 184);
            this.cardContent.Name = "cardContent";
            this.cardContent.Padding = new System.Windows.Forms.Padding(6);
            this.cardContent.Size = new System.Drawing.Size(1174, 523);
            this.cardContent.TabIndex = 2;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabInvoices);
            this.tabControl.Controls.Add(this.tabPayments);
            this.tabControl.Controls.Add(this.tabSms);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.ItemSize = new System.Drawing.Size(160, 36);
            this.tabControl.Location = new System.Drawing.Point(6, 6);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1162, 511);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabIndex = 0;
            // 
            // tabInvoices
            // 
            this.tabInvoices.Controls.Add(this.dgvInvoices);
            this.tabInvoices.Location = new System.Drawing.Point(4, 40);
            this.tabInvoices.Name = "tabInvoices";
            this.tabInvoices.Padding = new System.Windows.Forms.Padding(8);
            this.tabInvoices.Size = new System.Drawing.Size(1154, 467);
            this.tabInvoices.TabIndex = 0;
            this.tabInvoices.Text = "📄 الفواتير";
            // 
            // dgvInvoices
            // 
            this.dgvInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInvoices.Location = new System.Drawing.Point(8, 8);
            this.dgvInvoices.Name = "dgvInvoices";
            this.dgvInvoices.Size = new System.Drawing.Size(1138, 451);
            this.dgvInvoices.TabIndex = 0;
            // 
            // tabPayments
            // 
            this.tabPayments.Controls.Add(this.dgvPayments);
            this.tabPayments.Location = new System.Drawing.Point(4, 40);
            this.tabPayments.Name = "tabPayments";
            this.tabPayments.Padding = new System.Windows.Forms.Padding(8);
            this.tabPayments.Size = new System.Drawing.Size(1154, 467);
            this.tabPayments.TabIndex = 1;
            this.tabPayments.Text = "🧾 السداد";
            // 
            // dgvPayments
            // 
            this.dgvPayments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPayments.Location = new System.Drawing.Point(8, 8);
            this.dgvPayments.Name = "dgvPayments";
            this.dgvPayments.Size = new System.Drawing.Size(1138, 451);
            this.dgvPayments.TabIndex = 0;
            // 
            // tabSms
            // 
            this.tabSms.Controls.Add(this.dgvSmsLogs);
            this.tabSms.Location = new System.Drawing.Point(4, 40);
            this.tabSms.Name = "tabSms";
            this.tabSms.Padding = new System.Windows.Forms.Padding(8);
            this.tabSms.Size = new System.Drawing.Size(1154, 467);
            this.tabSms.TabIndex = 2;
            this.tabSms.Text = "💬 سجل الرسائل";
            // 
            // dgvSmsLogs
            // 
            this.dgvSmsLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSmsLogs.Location = new System.Drawing.Point(8, 8);
            this.dgvSmsLogs.Name = "dgvSmsLogs";
            this.dgvSmsLogs.Size = new System.Drawing.Size(1138, 451);
            this.dgvSmsLogs.TabIndex = 0;
            // 
            // loadingPanel
            // 
            this.loadingPanel.Controls.Add(this.loadingContainer);
            this.loadingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadingPanel.Location = new System.Drawing.Point(0, 0);
            this.loadingPanel.Name = "loadingPanel";
            this.loadingPanel.Size = new System.Drawing.Size(1200, 720);
            this.loadingPanel.TabIndex = 1;
            this.loadingPanel.Visible = false;
            // 
            // loadingContainer
            // 
            this.loadingContainer.Controls.Add(this.lblLoading);
            this.loadingContainer.Location = new System.Drawing.Point(0, 0);
            this.loadingContainer.Name = "loadingContainer";
            this.loadingContainer.Size = new System.Drawing.Size(240, 110);
            this.loadingContainer.TabIndex = 0;
            // 
            // lblLoading
            // 
            this.lblLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLoading.Location = new System.Drawing.Point(0, 0);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(240, 110);
            this.lblLoading.TabIndex = 0;
            this.lblLoading.Text = "جاري التحميل...";
            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InvoicePrintPreviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 720);
            this.Controls.Add(this.mainContainer);
            this.Controls.Add(this.loadingPanel);
            this.Name = "InvoicePrintPreviewForm";
            this.Text = "إرسال الفواتير والسداد";
            this.mainContainer.ResumeLayout(false);
            this.cardFilterStats.ResumeLayout(false);
            this.rootFilterStats.ResumeLayout(false);
            this.filtersLayout.ResumeLayout(false);
            this.filtersLayout.PerformLayout();
            this.dateLayout.ResumeLayout(false);
            this.statsFlow.ResumeLayout(false);
            this.cardToolbar.ResumeLayout(false);
            this.toolbarFlow.ResumeLayout(false);
            this.cardContent.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabInvoices.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).EndInit();
            this.tabPayments.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPayments)).EndInit();
            this.tabSms.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSmsLogs)).EndInit();
            this.loadingPanel.ResumeLayout(false);
            this.loadingContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
//using System.Windows.Forms;
//using water3.Services;

//namespace water3.Forms
//{
//    partial class InvoicePrintPreviewForm
//    {
//        private System.ComponentModel.IContainer components = null;

//        // ====== Controls (Designer) ======
//        private System.Windows.Forms.TableLayoutPanel mainContainer;

//        private System.Windows.Forms.Panel cardFilterStats;
//        private System.Windows.Forms.TableLayoutPanel rootFilterStats;
//        private System.Windows.Forms.TableLayoutPanel filtersLayout;
//        private System.Windows.Forms.FlowLayoutPanel statsFlow;

//        private System.Windows.Forms.Button btnSearch;
//        private System.Windows.Forms.TableLayoutPanel dateLayout;
//        private System.Windows.Forms.Label lblFrom;
//        private System.Windows.Forms.Label lblTo;
//        private System.Windows.Forms.DateTimePicker dtFrom;
//        private System.Windows.Forms.DateTimePicker dtTo;

//        private System.Windows.Forms.ComboBox cbSubscriberFilter;

//        private System.Windows.Forms.Label lblStatus;
//        private System.Windows.Forms.ComboBox cbStatusFilter;

//        private System.Windows.Forms.Label lblPaymentType;
//        private System.Windows.Forms.ComboBox cbPaymentTypeFilter;

//        private System.Windows.Forms.TextBox txtSearch;

//        private System.Windows.Forms.Label lblTotalInvoices;
//        private System.Windows.Forms.Label lblTotalAmount;
//        private System.Windows.Forms.Label lblPaidAmount;
//        private System.Windows.Forms.Label lblRemainingAmount;

//        private System.Windows.Forms.Panel cardToolbar;
//        private System.Windows.Forms.FlowLayoutPanel toolbarFlow;
//        private System.Windows.Forms.Button btnPrint;
//        private System.Windows.Forms.Button btnPrintAll;
//        private System.Windows.Forms.Button btnSMS;
//        private System.Windows.Forms.Button btnSMSAll;
//        private System.Windows.Forms.Button btnExport;

//        private System.Windows.Forms.Panel cardContent;
//        private System.Windows.Forms.TabControl tabControl;
//        private System.Windows.Forms.TabPage tabInvoices;
//        private System.Windows.Forms.TabPage tabPayments;
//        private System.Windows.Forms.TabPage tabSms;

//        private System.Windows.Forms.DataGridView dgvInvoices;
//        private System.Windows.Forms.DataGridView dgvPayments;
//        private System.Windows.Forms.DataGridView dgvSmsLogs;

//        private System.Windows.Forms.Panel loadingPanel;
//        private System.Windows.Forms.Panel loadingContainer;
//        private System.Windows.Forms.Label lblLoading;

//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && (components != null)) components.Dispose();
//            base.Dispose(disposing);
//        }

//        private void InitializeComponent()
//        {
//            this.mainContainer = new System.Windows.Forms.TableLayoutPanel();
//            this.cardFilterStats = new System.Windows.Forms.Panel();
//            this.rootFilterStats = new System.Windows.Forms.TableLayoutPanel();
//            this.filtersLayout = new System.Windows.Forms.TableLayoutPanel();
//            this.btnSearch = new System.Windows.Forms.Button();
//            this.dateLayout = new System.Windows.Forms.TableLayoutPanel();
//            this.lblFrom = new System.Windows.Forms.Label();
//            this.dtFrom = new System.Windows.Forms.DateTimePicker();
//            this.lblTo = new System.Windows.Forms.Label();
//            this.dtTo = new System.Windows.Forms.DateTimePicker();
//            this.cbSubscriberFilter = new System.Windows.Forms.ComboBox();

//            this.lblStatus = new System.Windows.Forms.Label();
//            this.cbStatusFilter = new System.Windows.Forms.ComboBox();

//            this.lblPaymentType = new System.Windows.Forms.Label();
//            this.cbPaymentTypeFilter = new System.Windows.Forms.ComboBox();

//            this.txtSearch = new System.Windows.Forms.TextBox();

//            this.statsFlow = new System.Windows.Forms.FlowLayoutPanel();
//            this.lblTotalInvoices = new System.Windows.Forms.Label();
//            this.lblTotalAmount = new System.Windows.Forms.Label();
//            this.lblPaidAmount = new System.Windows.Forms.Label();
//            this.lblRemainingAmount = new System.Windows.Forms.Label();

//            this.cardToolbar = new System.Windows.Forms.Panel();
//            this.toolbarFlow = new System.Windows.Forms.FlowLayoutPanel();
//            this.btnPrint = new System.Windows.Forms.Button();
//            this.btnPrintAll = new System.Windows.Forms.Button();
//            this.btnSMS = new System.Windows.Forms.Button();
//            this.btnSMSAll = new System.Windows.Forms.Button();
//            this.btnExport = new System.Windows.Forms.Button();

//            this.cardContent = new System.Windows.Forms.Panel();
//            this.tabControl = new System.Windows.Forms.TabControl();
//            this.tabInvoices = new System.Windows.Forms.TabPage();
//            this.dgvInvoices = new System.Windows.Forms.DataGridView();

//            this.tabPayments = new System.Windows.Forms.TabPage();
//            this.dgvPayments = new System.Windows.Forms.DataGridView();

//            this.tabSms = new System.Windows.Forms.TabPage();
//            this.dgvSmsLogs = new System.Windows.Forms.DataGridView();

//            this.loadingPanel = new System.Windows.Forms.Panel();
//            this.loadingContainer = new System.Windows.Forms.Panel();
//            this.lblLoading = new System.Windows.Forms.Label();

//            this.mainContainer.SuspendLayout();
//            this.cardFilterStats.SuspendLayout();
//            this.rootFilterStats.SuspendLayout();
//            this.filtersLayout.SuspendLayout();
//            this.dateLayout.SuspendLayout();
//            this.statsFlow.SuspendLayout();
//            this.cardToolbar.SuspendLayout();
//            this.toolbarFlow.SuspendLayout();
//            this.cardContent.SuspendLayout();
//            this.tabControl.SuspendLayout();
//            this.tabInvoices.SuspendLayout();
//            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).BeginInit();
//            this.tabPayments.SuspendLayout();
//            ((System.ComponentModel.ISupportInitialize)(this.dgvPayments)).BeginInit();
//            this.tabSms.SuspendLayout();
//            ((System.ComponentModel.ISupportInitialize)(this.dgvSmsLogs)).BeginInit();
//            this.loadingPanel.SuspendLayout();
//            this.loadingContainer.SuspendLayout();
//            this.SuspendLayout();

//            // mainContainer
//            this.mainContainer.ColumnCount = 1;
//            this.mainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1180F));
//            this.mainContainer.Controls.Add(this.cardFilterStats, 0, 0);
//            this.mainContainer.Controls.Add(this.cardToolbar, 0, 1);
//            this.mainContainer.Controls.Add(this.cardContent, 0, 2);
//            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.mainContainer.Location = new System.Drawing.Point(0, 0);
//            this.mainContainer.Margin = new System.Windows.Forms.Padding(0);
//            this.mainContainer.Name = "mainContainer";
//            this.mainContainer.Padding = new System.Windows.Forms.Padding(10);
//            this.mainContainer.RowCount = 3;
//            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 121F));
//            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
//            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
//            this.mainContainer.Size = new System.Drawing.Size(1200, 720);

//            // cardFilterStats
//            this.cardFilterStats.Controls.Add(this.rootFilterStats);
//            this.cardFilterStats.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.cardFilterStats.Location = new System.Drawing.Point(13, 13);
//            this.cardFilterStats.Name = "cardFilterStats";
//            this.cardFilterStats.Padding = new System.Windows.Forms.Padding(12);
//            this.cardFilterStats.Size = new System.Drawing.Size(1174, 115);

//            // rootFilterStats
//            this.rootFilterStats.ColumnCount = 1;
//            this.rootFilterStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1150F));
//            this.rootFilterStats.Controls.Add(this.filtersLayout, 0, 0);
//            this.rootFilterStats.Controls.Add(this.statsFlow, 0, 1);
//            this.rootFilterStats.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.rootFilterStats.Location = new System.Drawing.Point(12, 12);
//            this.rootFilterStats.Name = "rootFilterStats";
//            this.rootFilterStats.RowCount = 2;
//            this.rootFilterStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
//            this.rootFilterStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 88F));
//            this.rootFilterStats.Size = new System.Drawing.Size(1150, 91);

//            // filtersLayout  (تم توسيعها لدعم Status/PaymentType)
//            this.filtersLayout.ColumnCount = 8;
//            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
//            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28F)); // date
//            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 18F)); // subscriber
//            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F)); // lblStatus/lblPay
//            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F)); // cbStatus/cbPay
//            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F)); // spacer
//            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F)); // search
//            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 13F));
//            this.filtersLayout.Controls.Add(this.btnSearch, 0, 0);
//            this.filtersLayout.Controls.Add(this.dateLayout, 1, 0);
//            this.filtersLayout.Controls.Add(this.cbSubscriberFilter, 2, 0);
//            this.filtersLayout.Controls.Add(this.lblStatus, 3, 0);
//            this.filtersLayout.Controls.Add(this.cbStatusFilter, 4, 0);
//            this.filtersLayout.Controls.Add(this.lblPaymentType, 3, 0);
//            this.filtersLayout.Controls.Add(this.cbPaymentTypeFilter, 4, 0);
//            this.filtersLayout.Controls.Add(this.txtSearch, 6, 0);
//            this.filtersLayout.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.filtersLayout.Location = new System.Drawing.Point(3, 3);
//            this.filtersLayout.Name = "filtersLayout";
//            this.filtersLayout.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
//            this.filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
//            this.filtersLayout.Size = new System.Drawing.Size(1144, 32);

//            // btnSearch
//            this.btnSearch.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.btnSearch.Location = new System.Drawing.Point(1066, 3);
//            this.btnSearch.Name = "btnSearch";
//            this.btnSearch.Size = new System.Drawing.Size(75, 26);
//            this.btnSearch.Text = "🔍 بحث";

//            // dateLayout
//            this.dateLayout.ColumnCount = 4;
//            this.dateLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
//            this.dateLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
//            this.dateLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
//            this.dateLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
//            this.dateLayout.Controls.Add(this.lblFrom, 0, 0);
//            this.dateLayout.Controls.Add(this.dtFrom, 1, 0);
//            this.dateLayout.Controls.Add(this.lblTo, 2, 0);
//            this.dateLayout.Controls.Add(this.dtTo, 3, 0);
//            this.dateLayout.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.dateLayout.Location = new System.Drawing.Point(750, 3);
//            this.dateLayout.Name = "dateLayout";
//            this.dateLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
//            this.dateLayout.Size = new System.Drawing.Size(310, 26);

//            this.lblFrom.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.lblFrom.Text = "من:";
//            this.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

//            this.dtFrom.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;

//            this.lblTo.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.lblTo.Text = "إلى:";
//            this.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

//            this.dtTo.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;

//            // cbSubscriberFilter
//            this.cbSubscriberFilter.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.cbSubscriberFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
//            this.cbSubscriberFilter.Location = new System.Drawing.Point(555, 3);
//            this.cbSubscriberFilter.Name = "cbSubscriberFilter";
//            this.cbSubscriberFilter.Size = new System.Drawing.Size(189, 21);

//            // lblStatus
//            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.lblStatus.Text = "الحالة:";
//            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

//            // cbStatusFilter
//            this.cbStatusFilter.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.cbStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

//            // lblPaymentType
//            this.lblPaymentType.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.lblPaymentType.Text = "الطريقة:";
//            this.lblPaymentType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

//            // cbPaymentTypeFilter
//            this.cbPaymentTypeFilter.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.cbPaymentTypeFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

//            // txtSearch
//            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.txtSearch.Location = new System.Drawing.Point(16, 3);
//            this.txtSearch.Name = "txtSearch";
//            this.txtSearch.Size = new System.Drawing.Size(350, 20);

//            // statsFlow
//            this.statsFlow.Controls.Add(this.lblTotalInvoices);
//            this.statsFlow.Controls.Add(this.lblTotalAmount);
//            this.statsFlow.Controls.Add(this.lblPaidAmount);
//            this.statsFlow.Controls.Add(this.lblRemainingAmount);
//            this.statsFlow.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.statsFlow.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
//            this.statsFlow.WrapContents = false;

//            this.lblTotalInvoices.Size = new System.Drawing.Size(220, 62);
//            this.lblTotalInvoices.Text = "📄 إجمالي الفواتير\n0";
//            this.lblTotalInvoices.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

//            this.lblTotalAmount.Size = new System.Drawing.Size(220, 62);
//            this.lblTotalAmount.Text = "💰 إجمالي المبالغ\n0";
//            this.lblTotalAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

//            this.lblPaidAmount.Size = new System.Drawing.Size(220, 62);
//            this.lblPaidAmount.Text = "💳 المدفوع\n0";
//            this.lblPaidAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

//            this.lblRemainingAmount.Size = new System.Drawing.Size(220, 62);
//            this.lblRemainingAmount.Text = "⚠️ المتبقي\n0";
//            this.lblRemainingAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

//            // cardToolbar
//            this.cardToolbar.Controls.Add(this.toolbarFlow);
//            this.cardToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.cardToolbar.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);

//            // toolbarFlow
//            this.toolbarFlow.Controls.Add(this.btnPrint);
//            this.toolbarFlow.Controls.Add(this.btnPrintAll);
//            this.toolbarFlow.Controls.Add(this.btnSMS);
//            this.toolbarFlow.Controls.Add(this.btnSMSAll);
//            this.toolbarFlow.Controls.Add(this.btnExport);
//            this.toolbarFlow.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.toolbarFlow.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
//            this.toolbarFlow.WrapContents = false;

//            this.btnPrint.Text = "🖨️ طباعة مختارة";
//            this.btnPrintAll.Text = "🖨️ طباعة الكل";
//            this.btnSMS.Text = "📱 SMS مختارة";
//            this.btnSMSAll.Text = "📱 SMS للكل";
//            this.btnExport.Text = "📊 تصدير Excel";

//            // cardContent
//            this.cardContent.Controls.Add(this.tabControl);
//            this.cardContent.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.cardContent.Padding = new System.Windows.Forms.Padding(6);

//            // tabControl
//            this.tabControl.Controls.Add(this.tabInvoices);
//            this.tabControl.Controls.Add(this.tabPayments);
//            this.tabControl.Controls.Add(this.tabSms);
//            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.tabControl.ItemSize = new System.Drawing.Size(160, 36);
//            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;

//            // tabInvoices
//            this.tabInvoices.Controls.Add(this.dgvInvoices);
//            this.tabInvoices.Padding = new System.Windows.Forms.Padding(8);
//            this.tabInvoices.Text = "📄 الفواتير";

//            this.dgvInvoices.Dock = System.Windows.Forms.DockStyle.Fill;

//            // tabPayments
//            this.tabPayments.Controls.Add(this.dgvPayments);
//            this.tabPayments.Padding = new System.Windows.Forms.Padding(8);
//            this.tabPayments.Text = "🧾 السداد";

//            this.dgvPayments.Dock = System.Windows.Forms.DockStyle.Fill;

//            // tabSms
//            this.tabSms.Controls.Add(this.dgvSmsLogs);
//            this.tabSms.Padding = new System.Windows.Forms.Padding(8);
//            this.tabSms.Text = "💬 سجل الرسائل";

//            this.dgvSmsLogs.Dock = System.Windows.Forms.DockStyle.Fill;

//            // loadingPanel
//            this.loadingPanel.Controls.Add(this.loadingContainer);
//            this.loadingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.loadingPanel.Visible = false;

//            this.loadingContainer.Controls.Add(this.lblLoading);
//            this.loadingContainer.Size = new System.Drawing.Size(240, 110);

//            this.lblLoading.Dock = System.Windows.Forms.DockStyle.Fill;
//            this.lblLoading.Text = "جاري التحميل...";
//            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

//            // Form
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.ClientSize = new System.Drawing.Size(1200, 720);
//            this.Controls.Add(this.mainContainer);
//            this.Controls.Add(this.loadingPanel);
//            this.Name = "InvoicePrintPreviewForm";
//            this.Text = "إرسال الفواتير والسداد";

//            this.mainContainer.ResumeLayout(false);
//            this.cardFilterStats.ResumeLayout(false);
//            this.rootFilterStats.ResumeLayout(false);
//            this.filtersLayout.ResumeLayout(false);
//            this.filtersLayout.PerformLayout();
//            this.dateLayout.ResumeLayout(false);
//            this.statsFlow.ResumeLayout(false);
//            this.cardToolbar.ResumeLayout(false);
//            this.toolbarFlow.ResumeLayout(false);
//            this.cardContent.ResumeLayout(false);
//            this.tabControl.ResumeLayout(false);
//            this.tabInvoices.ResumeLayout(false);
//            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).EndInit();
//            this.tabPayments.ResumeLayout(false);
//            ((System.ComponentModel.ISupportInitialize)(this.dgvPayments)).EndInit();
//            this.tabSms.ResumeLayout(false);
//            ((System.ComponentModel.ISupportInitialize)(this.dgvSmsLogs)).EndInit();
//            this.loadingPanel.ResumeLayout(false);
//            this.loadingContainer.ResumeLayout(false);
//            this.ResumeLayout(false);
//        }
//    }
//}
/*using System.Windows.Forms;
using water3.Services;

namespace water3.Forms
{
    partial class InvoicePrintPreviewForm
    {
        private System.ComponentModel.IContainer components = null;

        // ====== Controls (Designer) ======
        private System.Windows.Forms.TableLayoutPanel mainContainer;

        private System.Windows.Forms.Panel cardFilterStats;
        private System.Windows.Forms.TableLayoutPanel rootFilterStats;
        private System.Windows.Forms.TableLayoutPanel filtersLayout;
        private System.Windows.Forms.FlowLayoutPanel statsFlow;

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TableLayoutPanel dateLayout;
        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.DateTimePicker dtTo;

        private System.Windows.Forms.ComboBox cbSubscriberFilter;
        private System.Windows.Forms.ComboBox cbStatusFilter;
        private System.Windows.Forms.TextBox txtSearch;

        private System.Windows.Forms.Label lblTotalInvoices;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.Label lblPaidAmount;
        private System.Windows.Forms.Label lblRemainingAmount;

        private System.Windows.Forms.Panel cardToolbar;
        private System.Windows.Forms.FlowLayoutPanel toolbarFlow;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnPrintAll;
        private System.Windows.Forms.Button btnSMS;
        private System.Windows.Forms.Button btnSMSAll;
        private System.Windows.Forms.Button btnExport;

        private System.Windows.Forms.Panel cardContent;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabInvoices;
        private System.Windows.Forms.TabPage tabSms;
        private System.Windows.Forms.TabPage tabPreview;

        private System.Windows.Forms.DataGridView dgvInvoices;
        private System.Windows.Forms.DataGridView dgvSmsLogs;
        private System.Windows.Forms.WebBrowser wbPreview;

        private System.Windows.Forms.Panel loadingPanel;
        private System.Windows.Forms.Panel loadingContainer;
        private System.Windows.Forms.Label lblLoading;
        // ===== SMS Gateway (Android) =====
        private AndroidSmsGatewayClient _gatewayClient;
        private SmsGatewaySettings _gatewaySettings;

        // UI controls (داخل نفس الفورم بدون Designer)
        private TextBox txtGatewayIp;
        private TextBox txtGatewayKey;
        private Button btnGatewayTest;
        private Button btnGatewaySave;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.mainContainer = new System.Windows.Forms.TableLayoutPanel();
            this.cardFilterStats = new System.Windows.Forms.Panel();
            this.rootFilterStats = new System.Windows.Forms.TableLayoutPanel();
            this.filtersLayout = new System.Windows.Forms.TableLayoutPanel();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dateLayout = new System.Windows.Forms.TableLayoutPanel();
            this.lblFrom = new System.Windows.Forms.Label();
            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.lblTo = new System.Windows.Forms.Label();
            this.dtTo = new System.Windows.Forms.DateTimePicker();
            this.cbSubscriberFilter = new System.Windows.Forms.ComboBox();
            this.cbStatusFilter = new System.Windows.Forms.ComboBox();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.statsFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTotalInvoices = new System.Windows.Forms.Label();
            this.lblTotalAmount = new System.Windows.Forms.Label();
            this.lblPaidAmount = new System.Windows.Forms.Label();
            this.lblRemainingAmount = new System.Windows.Forms.Label();
            this.cardToolbar = new System.Windows.Forms.Panel();
            this.toolbarFlow = new System.Windows.Forms.FlowLayoutPanel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.btnPrintAll = new System.Windows.Forms.Button();
            this.btnSMS = new System.Windows.Forms.Button();
            this.btnSMSAll = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.cardContent = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabInvoices = new System.Windows.Forms.TabPage();
            this.dgvInvoices = new System.Windows.Forms.DataGridView();
            this.tabSms = new System.Windows.Forms.TabPage();
            this.dgvSmsLogs = new System.Windows.Forms.DataGridView();
            this.tabPreview = new System.Windows.Forms.TabPage();
            this.wbPreview = new System.Windows.Forms.WebBrowser();
            this.loadingPanel = new System.Windows.Forms.Panel();
            this.loadingContainer = new System.Windows.Forms.Panel();
            this.lblLoading = new System.Windows.Forms.Label();
            this.mainContainer.SuspendLayout();
            this.cardFilterStats.SuspendLayout();
            this.rootFilterStats.SuspendLayout();
            this.filtersLayout.SuspendLayout();
            this.dateLayout.SuspendLayout();
            this.statsFlow.SuspendLayout();
            this.cardToolbar.SuspendLayout();
            this.toolbarFlow.SuspendLayout();
            this.cardContent.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.tabInvoices.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).BeginInit();
            this.tabSms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSmsLogs)).BeginInit();
            this.tabPreview.SuspendLayout();
            this.loadingPanel.SuspendLayout();
            this.loadingContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainContainer
            // 
            this.mainContainer.ColumnCount = 1;
            this.mainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1180F));
            this.mainContainer.Controls.Add(this.cardFilterStats, 0, 0);
            this.mainContainer.Controls.Add(this.cardToolbar, 0, 1);
            this.mainContainer.Controls.Add(this.cardContent, 0, 2);
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContainer.Location = new System.Drawing.Point(0, 0);
            this.mainContainer.Margin = new System.Windows.Forms.Padding(0);
            this.mainContainer.Name = "mainContainer";
            this.mainContainer.Padding = new System.Windows.Forms.Padding(10);
            this.mainContainer.RowCount = 3;
            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 121F));
            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.mainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainContainer.Size = new System.Drawing.Size(1200, 720);
            this.mainContainer.TabIndex = 0;
            // 
            // cardFilterStats
            // 
            this.cardFilterStats.Controls.Add(this.rootFilterStats);
            this.cardFilterStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardFilterStats.Location = new System.Drawing.Point(13, 13);
            this.cardFilterStats.Name = "cardFilterStats";
            this.cardFilterStats.Padding = new System.Windows.Forms.Padding(12);
            this.cardFilterStats.Size = new System.Drawing.Size(1174, 115);
            this.cardFilterStats.TabIndex = 0;
            // 
            // rootFilterStats
            // 
            this.rootFilterStats.ColumnCount = 1;
            this.rootFilterStats.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 1150F));
            this.rootFilterStats.Controls.Add(this.filtersLayout, 0, 0);
            this.rootFilterStats.Controls.Add(this.statsFlow, 0, 1);
            this.rootFilterStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootFilterStats.Location = new System.Drawing.Point(12, 12);
            this.rootFilterStats.Name = "rootFilterStats";
            this.rootFilterStats.RowCount = 2;
            this.rootFilterStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.rootFilterStats.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 88F));
            this.rootFilterStats.Size = new System.Drawing.Size(1150, 91);
            this.rootFilterStats.TabIndex = 0;
            // 
            // filtersLayout
            // 
            this.filtersLayout.ColumnCount = 6;
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 13F));
            this.filtersLayout.Controls.Add(this.btnSearch, 0, 0);
            this.filtersLayout.Controls.Add(this.dateLayout, 1, 0);
            this.filtersLayout.Controls.Add(this.cbSubscriberFilter, 2, 0);
            this.filtersLayout.Controls.Add(this.cbStatusFilter, 3, 0);
            this.filtersLayout.Controls.Add(this.txtSearch, 4, 0);
            this.filtersLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filtersLayout.Location = new System.Drawing.Point(3, 3);
            this.filtersLayout.Name = "filtersLayout";
            this.filtersLayout.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.filtersLayout.Size = new System.Drawing.Size(1144, 32);
            this.filtersLayout.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnSearch.Location = new System.Drawing.Point(1066, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 26);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "🔍 بحث";
            // 
            // dateLayout
            // 
            this.dateLayout.ColumnCount = 4;
            this.dateLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.dateLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dateLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.dateLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.dateLayout.Controls.Add(this.lblFrom, 0, 0);
            this.dateLayout.Controls.Add(this.dtFrom, 1, 0);
            this.dateLayout.Controls.Add(this.lblTo, 2, 0);
            this.dateLayout.Controls.Add(this.dtTo, 3, 0);
            this.dateLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dateLayout.Location = new System.Drawing.Point(709, 3);
            this.dateLayout.Name = "dateLayout";
            this.dateLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.dateLayout.Size = new System.Drawing.Size(351, 26);
            this.dateLayout.TabIndex = 1;
            // 
            // lblFrom
            // 
            this.lblFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFrom.Location = new System.Drawing.Point(318, 0);
            this.lblFrom.Name = "lblFrom";
            this.lblFrom.Size = new System.Drawing.Size(30, 26);
            this.lblFrom.TabIndex = 0;
            this.lblFrom.Text = "من:";
            this.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtFrom
            // 
            this.dtFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtFrom.Location = new System.Drawing.Point(179, 3);
            this.dtFrom.Name = "dtFrom";
            this.dtFrom.Size = new System.Drawing.Size(133, 20);
            this.dtFrom.TabIndex = 1;
            // 
            // lblTo
            // 
            this.lblTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTo.Location = new System.Drawing.Point(143, 0);
            this.lblTo.Name = "lblTo";
            this.lblTo.Size = new System.Drawing.Size(30, 26);
            this.lblTo.TabIndex = 2;
            this.lblTo.Text = "إلى:";
            this.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtTo
            // 
            this.dtTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtTo.Location = new System.Drawing.Point(3, 3);
            this.dtTo.Name = "dtTo";
            this.dtTo.Size = new System.Drawing.Size(134, 20);
            this.dtTo.TabIndex = 3;
            // 
            // cbSubscriberFilter
            // 
            this.cbSubscriberFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbSubscriberFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSubscriberFilter.Location = new System.Drawing.Point(478, 3);
            this.cbSubscriberFilter.Name = "cbSubscriberFilter";
            this.cbSubscriberFilter.Size = new System.Drawing.Size(225, 21);
            this.cbSubscriberFilter.TabIndex = 2;
            // 
            // cbStatusFilter
            // 
            this.cbStatusFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbStatusFilter.Location = new System.Drawing.Point(331, 3);
            this.cbStatusFilter.Name = "cbStatusFilter";
            this.cbStatusFilter.Size = new System.Drawing.Size(141, 21);
            this.cbStatusFilter.TabIndex = 3;
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearch.Location = new System.Drawing.Point(16, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(309, 20);
            this.txtSearch.TabIndex = 4;
            // 
            // statsFlow
            // 
            this.statsFlow.Controls.Add(this.lblTotalInvoices);
            this.statsFlow.Controls.Add(this.lblTotalAmount);
            this.statsFlow.Controls.Add(this.lblPaidAmount);
            this.statsFlow.Controls.Add(this.lblRemainingAmount);
            this.statsFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statsFlow.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.statsFlow.Location = new System.Drawing.Point(3, 41);
            this.statsFlow.Name = "statsFlow";
            this.statsFlow.Size = new System.Drawing.Size(1144, 82);
            this.statsFlow.TabIndex = 1;
            this.statsFlow.WrapContents = false;
            // 
            // lblTotalInvoices
            // 
            this.lblTotalInvoices.Location = new System.Drawing.Point(921, 0);
            this.lblTotalInvoices.Name = "lblTotalInvoices";
            this.lblTotalInvoices.Size = new System.Drawing.Size(220, 62);
            this.lblTotalInvoices.TabIndex = 0;
            this.lblTotalInvoices.Text = "📄 إجمالي الفواتير\n0";
            this.lblTotalInvoices.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTotalAmount
            // 
            this.lblTotalAmount.Location = new System.Drawing.Point(695, 0);
            this.lblTotalAmount.Name = "lblTotalAmount";
            this.lblTotalAmount.Size = new System.Drawing.Size(220, 62);
            this.lblTotalAmount.TabIndex = 1;
            this.lblTotalAmount.Text = "💰 إجمالي المبالغ\n0";
            this.lblTotalAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblPaidAmount
            // 
            this.lblPaidAmount.Location = new System.Drawing.Point(469, 0);
            this.lblPaidAmount.Name = "lblPaidAmount";
            this.lblPaidAmount.Size = new System.Drawing.Size(220, 62);
            this.lblPaidAmount.TabIndex = 2;
            this.lblPaidAmount.Text = "💳 المدفوع\n0";
            this.lblPaidAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblRemainingAmount
            // 
            this.lblRemainingAmount.Location = new System.Drawing.Point(243, 0);
            this.lblRemainingAmount.Name = "lblRemainingAmount";
            this.lblRemainingAmount.Size = new System.Drawing.Size(220, 62);
            this.lblRemainingAmount.TabIndex = 3;
            this.lblRemainingAmount.Text = "⚠️ المتبقي\n0";
            this.lblRemainingAmount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cardToolbar
            // 
            this.cardToolbar.Controls.Add(this.toolbarFlow);
            this.cardToolbar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardToolbar.Location = new System.Drawing.Point(13, 134);
            this.cardToolbar.Name = "cardToolbar";
            this.cardToolbar.Padding = new System.Windows.Forms.Padding(10, 6, 10, 6);
            this.cardToolbar.Size = new System.Drawing.Size(1174, 44);
            this.cardToolbar.TabIndex = 1;
            // 
            // toolbarFlow
            // 
            this.toolbarFlow.Controls.Add(this.btnPrint);
            this.toolbarFlow.Controls.Add(this.btnPrintAll);
            this.toolbarFlow.Controls.Add(this.btnSMS);
            this.toolbarFlow.Controls.Add(this.btnSMSAll);
            this.toolbarFlow.Controls.Add(this.btnExport);
            this.toolbarFlow.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolbarFlow.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.toolbarFlow.Location = new System.Drawing.Point(10, 6);
            this.toolbarFlow.Name = "toolbarFlow";
            this.toolbarFlow.Size = new System.Drawing.Size(1154, 32);
            this.toolbarFlow.TabIndex = 0;
            this.toolbarFlow.WrapContents = false;
            // 
            // btnPrint
            // 
            this.btnPrint.Location = new System.Drawing.Point(1076, 3);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(75, 23);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "🖨️ طباعة مختارة";
            // 
            // btnPrintAll
            // 
            this.btnPrintAll.Location = new System.Drawing.Point(995, 3);
            this.btnPrintAll.Name = "btnPrintAll";
            this.btnPrintAll.Size = new System.Drawing.Size(75, 23);
            this.btnPrintAll.TabIndex = 1;
            this.btnPrintAll.Text = "🖨️ طباعة الكل";
            // 
            // btnSMS
            // 
            this.btnSMS.Location = new System.Drawing.Point(914, 3);
            this.btnSMS.Name = "btnSMS";
            this.btnSMS.Size = new System.Drawing.Size(75, 23);
            this.btnSMS.TabIndex = 2;
            this.btnSMS.Text = "📱 SMS مختارة";
            // 
            // btnSMSAll
            // 
            this.btnSMSAll.Location = new System.Drawing.Point(833, 3);
            this.btnSMSAll.Name = "btnSMSAll";
            this.btnSMSAll.Size = new System.Drawing.Size(75, 23);
            this.btnSMSAll.TabIndex = 3;
            this.btnSMSAll.Text = "📱 SMS للكل";
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(752, 3);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 4;
            this.btnExport.Text = "📊 تصدير Excel";
            // 
            // cardContent
            // 
            this.cardContent.Controls.Add(this.tabControl);
            this.cardContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cardContent.Location = new System.Drawing.Point(13, 184);
            this.cardContent.Name = "cardContent";
            this.cardContent.Padding = new System.Windows.Forms.Padding(6);
            this.cardContent.Size = new System.Drawing.Size(1174, 523);
            this.cardContent.TabIndex = 2;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabInvoices);
            this.tabControl.Controls.Add(this.tabSms);
            this.tabControl.Controls.Add(this.tabPreview);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.ItemSize = new System.Drawing.Size(160, 36);
            this.tabControl.Location = new System.Drawing.Point(6, 6);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1162, 511);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabIndex = 0;
            // 
            // tabInvoices
            // 
            this.tabInvoices.Controls.Add(this.dgvInvoices);
            this.tabInvoices.Location = new System.Drawing.Point(4, 40);
            this.tabInvoices.Name = "tabInvoices";
            this.tabInvoices.Padding = new System.Windows.Forms.Padding(8);
            this.tabInvoices.Size = new System.Drawing.Size(1154, 467);
            this.tabInvoices.TabIndex = 0;
            this.tabInvoices.Text = "📄 الفواتير";
            // 
            // dgvInvoices
            // 
            this.dgvInvoices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInvoices.Location = new System.Drawing.Point(8, 8);
            this.dgvInvoices.Name = "dgvInvoices";
            this.dgvInvoices.Size = new System.Drawing.Size(1138, 451);
            this.dgvInvoices.TabIndex = 0;
            // 
            // tabSms
            // 
            this.tabSms.Controls.Add(this.dgvSmsLogs);
            this.tabSms.Location = new System.Drawing.Point(4, 40);
            this.tabSms.Name = "tabSms";
            this.tabSms.Padding = new System.Windows.Forms.Padding(8);
            this.tabSms.Size = new System.Drawing.Size(1154, 467);
            this.tabSms.TabIndex = 1;
            this.tabSms.Text = "💬 سجل الرسائل";
            // 
            // dgvSmsLogs
            // 
            this.dgvSmsLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSmsLogs.Location = new System.Drawing.Point(8, 8);
            this.dgvSmsLogs.Name = "dgvSmsLogs";
            this.dgvSmsLogs.Size = new System.Drawing.Size(1138, 451);
            this.dgvSmsLogs.TabIndex = 0;
            // 
            // tabPreview
            // 
            this.tabPreview.Controls.Add(this.wbPreview);
            this.tabPreview.Location = new System.Drawing.Point(4, 40);
            this.tabPreview.Name = "tabPreview";
            this.tabPreview.Padding = new System.Windows.Forms.Padding(8);
            this.tabPreview.Size = new System.Drawing.Size(1154, 467);
            this.tabPreview.TabIndex = 2;
            this.tabPreview.Text = "👁️ المعاينة";
            // 
            // wbPreview
            // 
            this.wbPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wbPreview.Location = new System.Drawing.Point(8, 8);
            this.wbPreview.Name = "wbPreview";
            this.wbPreview.ScriptErrorsSuppressed = true;
            this.wbPreview.Size = new System.Drawing.Size(1138, 451);
            this.wbPreview.TabIndex = 0;
            // 
            // loadingPanel
            // 
            this.loadingPanel.Controls.Add(this.loadingContainer);
            this.loadingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.loadingPanel.Location = new System.Drawing.Point(0, 0);
            this.loadingPanel.Name = "loadingPanel";
            this.loadingPanel.Size = new System.Drawing.Size(1200, 720);
            this.loadingPanel.TabIndex = 1;
            this.loadingPanel.Visible = false;
            // 
            // loadingContainer
            // 
            this.loadingContainer.Controls.Add(this.lblLoading);
            this.loadingContainer.Location = new System.Drawing.Point(0, 0);
            this.loadingContainer.Name = "loadingContainer";
            this.loadingContainer.Size = new System.Drawing.Size(240, 110);
            this.loadingContainer.TabIndex = 0;
            // 
            // lblLoading
            // 
            this.lblLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLoading.Location = new System.Drawing.Point(0, 0);
            this.lblLoading.Name = "lblLoading";
            this.lblLoading.Size = new System.Drawing.Size(240, 110);
            this.lblLoading.TabIndex = 0;
            this.lblLoading.Text = "جاري التحميل...";
            this.lblLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InvoicePrintPreviewForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1200, 720);
            this.Controls.Add(this.mainContainer);
            this.Controls.Add(this.loadingPanel);
            this.Name = "InvoicePrintPreviewForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "معاينة وطباعة الفواتير";
            this.mainContainer.ResumeLayout(false);
            this.cardFilterStats.ResumeLayout(false);
            this.rootFilterStats.ResumeLayout(false);
            this.filtersLayout.ResumeLayout(false);
            this.filtersLayout.PerformLayout();
            this.dateLayout.ResumeLayout(false);
            this.statsFlow.ResumeLayout(false);
            this.cardToolbar.ResumeLayout(false);
            this.toolbarFlow.ResumeLayout(false);
            this.cardContent.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.tabInvoices.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvInvoices)).EndInit();
            this.tabSms.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSmsLogs)).EndInit();
            this.tabPreview.ResumeLayout(false);
            this.loadingPanel.ResumeLayout(false);
            this.loadingContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}*/
