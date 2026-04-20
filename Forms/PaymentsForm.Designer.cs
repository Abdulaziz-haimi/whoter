
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace water3.Forms
{
    partial class PaymentsForm
    {

        private IContainer components = null;

        // Root
        private TableLayoutPanel mainLayout;

        private Panel pnlContent;
        private TableLayoutPanel contentLayout;

        // Add group
        private GroupBox grpAdd;
        private TableLayoutPanel gridAdd;

        // Search
        private Label lblSubSearch;
        private Panel pnlSearch;
        private TableLayoutPanel searchLayout;
        private TextBox txtSubscriberSearch;
        private IconPictureBox picSearch;

        // Balance
        private Label lblBalanceTitle;
        private Panel pnlBalance;
        private Label lblCurrentBalance;

        // Collector / Date
        private Label lblCollectorTitle;
        private ComboBox ddlCollectors;

        private Label lblDateTitle;
        private DateTimePicker dtpPaymentDate;

        // Amount / PayType / Notes
        private Label lblAmountTitle;
        private TextBox txtAmount;

        private Label lblPayTypeTitle;
        private ComboBox ddlPaymentType;

        private Label lblNotesTitle;
        private TextBox txtNotes;

        // Buttons (Icon)
        private TableLayoutPanel buttonsLayout;
        private IconButton btnAdd;
        private IconButton btnRefresh;
        private IconButton btnClear;

        // Message
        private Panel pnlMessage;
        private Label lblMessage;

        // Grid header + grid
        private Panel pnlGridHeader;
        private Label lblGridTitle;
        private FlowLayoutPanel headerButtons;
        private IconButton btnExport;

        private DataGridView dgv;

        private ToolTip toolTip1;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.picSearch = new FontAwesome.Sharp.IconPictureBox();
            this.btnAdd = new FontAwesome.Sharp.IconButton();
            this.btnRefresh = new FontAwesome.Sharp.IconButton();
            this.btnClear = new FontAwesome.Sharp.IconButton();
            this.btnExport = new FontAwesome.Sharp.IconButton();
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.contentLayout = new System.Windows.Forms.TableLayoutPanel();
            this.grpAdd = new System.Windows.Forms.GroupBox();
            this.gridAdd = new System.Windows.Forms.TableLayoutPanel();
            this.lblSubSearch = new System.Windows.Forms.Label();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.searchLayout = new System.Windows.Forms.TableLayoutPanel();
            this.txtSubscriberSearch = new System.Windows.Forms.TextBox();
            this.lblBalanceTitle = new System.Windows.Forms.Label();
            this.pnlBalance = new System.Windows.Forms.Panel();
            this.lblCurrentBalance = new System.Windows.Forms.Label();
            this.lblCollectorTitle = new System.Windows.Forms.Label();
            this.ddlCollectors = new System.Windows.Forms.ComboBox();
            this.lblDateTitle = new System.Windows.Forms.Label();
            this.dtpPaymentDate = new System.Windows.Forms.DateTimePicker();
            this.lblAmountTitle = new System.Windows.Forms.Label();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.lblPayTypeTitle = new System.Windows.Forms.Label();
            this.ddlPaymentType = new System.Windows.Forms.ComboBox();
            this.lblNotesTitle = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.pnlMessage = new System.Windows.Forms.Panel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.buttonsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlGridHeader = new System.Windows.Forms.Panel();
            this.headerButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.lblGridTitle = new System.Windows.Forms.Label();
            this.dgv = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.picSearch)).BeginInit();
            this.mainLayout.SuspendLayout();
            this.pnlContent.SuspendLayout();
            this.contentLayout.SuspendLayout();
            this.grpAdd.SuspendLayout();
            this.gridAdd.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.searchLayout.SuspendLayout();
            this.pnlBalance.SuspendLayout();
            this.pnlMessage.SuspendLayout();
            this.buttonsLayout.SuspendLayout();
            this.pnlGridHeader.SuspendLayout();
            this.headerButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // picSearch
            // 
            this.picSearch.BackColor = System.Drawing.Color.White;
            this.picSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(87)))), ((int)(((byte)(183)))));
            this.picSearch.IconChar = FontAwesome.Sharp.IconChar.MagnifyingGlass;
            this.picSearch.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(87)))), ((int)(((byte)(183)))));
            this.picSearch.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.picSearch.IconSize = 30;
            this.picSearch.Location = new System.Drawing.Point(0, 0);
            this.picSearch.Margin = new System.Windows.Forms.Padding(0);
            this.picSearch.Name = "picSearch";
            this.picSearch.Size = new System.Drawing.Size(30, 30);
            this.picSearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.picSearch.TabIndex = 1;
            this.picSearch.TabStop = false;
            this.toolTip1.SetToolTip(this.picSearch, "بحث");
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(87)))), ((int)(((byte)(183)))));
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.IconChar = FontAwesome.Sharp.IconChar.PlusCircle;
            this.btnAdd.IconColor = System.Drawing.Color.White;
            this.btnAdd.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnAdd.IconSize = 22;
            this.btnAdd.Location = new System.Drawing.Point(340, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(269, 36);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "تسجيل";
            this.btnAdd.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.btnAdd, "إضافة دفعة");
            this.btnAdd.UseVisualStyleBackColor = false;
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(23)))), ((int)(((byte)(162)))), ((int)(((byte)(184)))));
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefresh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.IconChar = FontAwesome.Sharp.IconChar.Rotate;
            this.btnRefresh.IconColor = System.Drawing.Color.White;
            this.btnRefresh.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnRefresh.IconSize = 22;
            this.btnRefresh.Location = new System.Drawing.Point(187, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(147, 36);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "تحديث";
            this.btnRefresh.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.btnRefresh, "تحديث");
            this.btnRefresh.UseVisualStyleBackColor = false;
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(193)))), ((int)(((byte)(7)))));
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnClear.FlatAppearance.BorderSize = 0;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.btnClear.IconChar = FontAwesome.Sharp.IconChar.Broom;
            this.btnClear.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.btnClear.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnClear.IconSize = 22;
            this.btnClear.Location = new System.Drawing.Point(3, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(178, 36);
            this.btnClear.TabIndex = 2;
            this.btnClear.Text = "مسح";
            this.btnClear.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip1.SetToolTip(this.btnClear, "مسح الحقول");
            this.btnClear.UseVisualStyleBackColor = false;
            // 
            // btnExport
            // 
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.btnExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExport.FlatAppearance.BorderSize = 0;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.IconChar = FontAwesome.Sharp.IconChar.FileExcel;
            this.btnExport.IconColor = System.Drawing.Color.White;
            this.btnExport.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnExport.IconSize = 20;
            this.btnExport.Location = new System.Drawing.Point(0, 0);
            this.btnExport.Margin = new System.Windows.Forms.Padding(0);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(44, 28);
            this.btnExport.TabIndex = 0;
            this.toolTip1.SetToolTip(this.btnExport, "تصدير");
            this.btnExport.UseVisualStyleBackColor = false;
            // 
            // mainLayout
            // 
            this.mainLayout.BackColor = this.BackColor;
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.pnlContent, 0, 0);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new System.Windows.Forms.Padding(20);
            this.mainLayout.RowCount = 1;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Size = new System.Drawing.Size(1200, 700);
            this.mainLayout.TabIndex = 0;
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.pnlContent.Controls.Add(this.contentLayout);
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(23, 23);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.pnlContent.Size = new System.Drawing.Size(1154, 654);
            this.pnlContent.TabIndex = 1;
            // 
            // contentLayout
            // 
            this.contentLayout.BackColor = this.pnlContent.BackColor;
            this.contentLayout.ColumnCount = 1;
            this.contentLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contentLayout.Controls.Add(this.grpAdd, 0, 0);
            this.contentLayout.Controls.Add(this.pnlGridHeader, 0, 1);
            this.contentLayout.Controls.Add(this.dgv, 0, 2);
            this.contentLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentLayout.Location = new System.Drawing.Point(0, 10);
            this.contentLayout.Name = "contentLayout";
            this.contentLayout.RowCount = 3;
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 280F));
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.contentLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.contentLayout.Size = new System.Drawing.Size(1154, 644);
            this.contentLayout.TabIndex = 0;
            // 
            // grpAdd
            // 
            this.grpAdd.BackColor = System.Drawing.Color.White;
            this.grpAdd.Controls.Add(this.gridAdd);
            this.grpAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAdd.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpAdd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(87)))), ((int)(((byte)(183)))));
            this.grpAdd.Location = new System.Drawing.Point(3, 3);
            this.grpAdd.Name = "grpAdd";
            this.grpAdd.Padding = new System.Windows.Forms.Padding(15);
            this.grpAdd.Size = new System.Drawing.Size(1148, 274);
            this.grpAdd.TabIndex = 0;
            this.grpAdd.TabStop = false;
            this.grpAdd.Text = "إضافة دفعة جديدة";
            // 
            // gridAdd
            // 
            this.gridAdd.BackColor = System.Drawing.Color.White;
            this.gridAdd.ColumnCount = 6;
            this.gridAdd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.gridAdd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35F));
            this.gridAdd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 112F));
            this.gridAdd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.gridAdd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.gridAdd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.gridAdd.Controls.Add(this.lblSubSearch, 0, 0);
            this.gridAdd.Controls.Add(this.pnlSearch, 1, 0);
            this.gridAdd.Controls.Add(this.lblBalanceTitle, 2, 0);
            this.gridAdd.Controls.Add(this.pnlBalance, 3, 0);
            this.gridAdd.Controls.Add(this.lblCollectorTitle, 0, 1);
            this.gridAdd.Controls.Add(this.ddlCollectors, 1, 1);
            this.gridAdd.Controls.Add(this.lblDateTitle, 2, 1);
            this.gridAdd.Controls.Add(this.dtpPaymentDate, 3, 1);
            this.gridAdd.Controls.Add(this.lblAmountTitle, 0, 2);
            this.gridAdd.Controls.Add(this.txtAmount, 1, 2);
            this.gridAdd.Controls.Add(this.lblPayTypeTitle, 2, 2);
            this.gridAdd.Controls.Add(this.ddlPaymentType, 3, 2);
            this.gridAdd.Controls.Add(this.lblNotesTitle, 4, 2);
            this.gridAdd.Controls.Add(this.txtNotes, 5, 2);
            this.gridAdd.Controls.Add(this.pnlMessage, 0, 3);
            this.gridAdd.Controls.Add(this.buttonsLayout, 3, 3);
            this.gridAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAdd.Location = new System.Drawing.Point(15, 33);
            this.gridAdd.Name = "gridAdd";
            this.gridAdd.RowCount = 4;
            this.gridAdd.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.gridAdd.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.gridAdd.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.gridAdd.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.gridAdd.Size = new System.Drawing.Size(1118, 226);
            this.gridAdd.TabIndex = 0;
            // 
            // lblSubSearch
            // 
            this.lblSubSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSubSearch.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblSubSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblSubSearch.Location = new System.Drawing.Point(1001, 0);
            this.lblSubSearch.Name = "lblSubSearch";
            this.lblSubSearch.Size = new System.Drawing.Size(114, 70);
            this.lblSubSearch.TabIndex = 0;
            this.lblSubSearch.Text = "بحث المشترك:";
            this.lblSubSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlSearch
            // 
            this.pnlSearch.BackColor = System.Drawing.Color.White;
            this.pnlSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSearch.Controls.Add(this.searchLayout);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearch.Location = new System.Drawing.Point(733, 3);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.pnlSearch.Size = new System.Drawing.Size(262, 64);
            this.pnlSearch.TabIndex = 1;
            // 
            // searchLayout
            // 
            this.searchLayout.ColumnCount = 2;
            this.searchLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.searchLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.searchLayout.Controls.Add(this.txtSubscriberSearch, 0, 0);
            this.searchLayout.Controls.Add(this.picSearch, 1, 0);
            this.searchLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchLayout.Location = new System.Drawing.Point(10, 8);
            this.searchLayout.Margin = new System.Windows.Forms.Padding(0);
            this.searchLayout.Name = "searchLayout";
            this.searchLayout.RowCount = 1;
            this.searchLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 46F));
            this.searchLayout.Size = new System.Drawing.Size(240, 46);
            this.searchLayout.TabIndex = 0;
            // 
            // txtSubscriberSearch
            // 
            this.txtSubscriberSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSubscriberSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSubscriberSearch.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtSubscriberSearch.Location = new System.Drawing.Point(30, 0);
            this.txtSubscriberSearch.Margin = new System.Windows.Forms.Padding(0);
            this.txtSubscriberSearch.MaxLength = 200;
            this.txtSubscriberSearch.Name = "txtSubscriberSearch";
            this.txtSubscriberSearch.Size = new System.Drawing.Size(210, 18);
            this.txtSubscriberSearch.TabIndex = 0;
            this.txtSubscriberSearch.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblBalanceTitle
            // 
            this.lblBalanceTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblBalanceTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblBalanceTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblBalanceTitle.Location = new System.Drawing.Point(621, 0);
            this.lblBalanceTitle.Name = "lblBalanceTitle";
            this.lblBalanceTitle.Size = new System.Drawing.Size(106, 70);
            this.lblBalanceTitle.TabIndex = 2;
            this.lblBalanceTitle.Text = "الرصيد:";
            this.lblBalanceTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlBalance
            // 
            this.pnlBalance.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(240)))), ((int)(((byte)(255)))));
            this.gridAdd.SetColumnSpan(this.pnlBalance, 3);
            this.pnlBalance.Controls.Add(this.lblCurrentBalance);
            this.pnlBalance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBalance.Location = new System.Drawing.Point(3, 3);
            this.pnlBalance.Name = "pnlBalance";
            this.pnlBalance.Padding = new System.Windows.Forms.Padding(8);
            this.pnlBalance.Size = new System.Drawing.Size(612, 64);
            this.pnlBalance.TabIndex = 3;
            // 
            // lblCurrentBalance
            // 
            this.lblCurrentBalance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrentBalance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblCurrentBalance.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(167)))), ((int)(((byte)(69)))));
            this.lblCurrentBalance.Location = new System.Drawing.Point(8, 8);
            this.lblCurrentBalance.Name = "lblCurrentBalance";
            this.lblCurrentBalance.Size = new System.Drawing.Size(596, 48);
            this.lblCurrentBalance.TabIndex = 0;
            this.lblCurrentBalance.Text = "0.00 ريال";
            this.lblCurrentBalance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCollectorTitle
            // 
            this.lblCollectorTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCollectorTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblCollectorTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblCollectorTitle.Location = new System.Drawing.Point(1001, 70);
            this.lblCollectorTitle.Name = "lblCollectorTitle";
            this.lblCollectorTitle.Size = new System.Drawing.Size(114, 50);
            this.lblCollectorTitle.TabIndex = 4;
            this.lblCollectorTitle.Text = "المحصل:";
            this.lblCollectorTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ddlCollectors
            // 
            this.ddlCollectors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ddlCollectors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlCollectors.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.ddlCollectors.Location = new System.Drawing.Point(733, 73);
            this.ddlCollectors.Name = "ddlCollectors";
            this.ddlCollectors.Size = new System.Drawing.Size(262, 25);
            this.ddlCollectors.TabIndex = 5;
            // 
            // lblDateTitle
            // 
            this.lblDateTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDateTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblDateTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblDateTitle.Location = new System.Drawing.Point(621, 70);
            this.lblDateTitle.Name = "lblDateTitle";
            this.lblDateTitle.Size = new System.Drawing.Size(106, 50);
            this.lblDateTitle.TabIndex = 6;
            this.lblDateTitle.Text = "تاريخ الدفع:";
            this.lblDateTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // dtpPaymentDate
            // 
            this.gridAdd.SetColumnSpan(this.dtpPaymentDate, 3);
            this.dtpPaymentDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtpPaymentDate.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.dtpPaymentDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpPaymentDate.Location = new System.Drawing.Point(3, 73);
            this.dtpPaymentDate.Name = "dtpPaymentDate";
            this.dtpPaymentDate.RightToLeftLayout = true;
            this.dtpPaymentDate.Size = new System.Drawing.Size(612, 25);
            this.dtpPaymentDate.TabIndex = 7;
            // 
            // lblAmountTitle
            // 
            this.lblAmountTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblAmountTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblAmountTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblAmountTitle.Location = new System.Drawing.Point(1001, 120);
            this.lblAmountTitle.Name = "lblAmountTitle";
            this.lblAmountTitle.Size = new System.Drawing.Size(114, 60);
            this.lblAmountTitle.TabIndex = 8;
            this.lblAmountTitle.Text = "المبلغ:";
            this.lblAmountTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtAmount
            // 
            this.txtAmount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAmount.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtAmount.Location = new System.Drawing.Point(733, 123);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(262, 25);
            this.txtAmount.TabIndex = 9;
            this.txtAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblPayTypeTitle
            // 
            this.lblPayTypeTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPayTypeTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblPayTypeTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblPayTypeTitle.Location = new System.Drawing.Point(621, 120);
            this.lblPayTypeTitle.Name = "lblPayTypeTitle";
            this.lblPayTypeTitle.Size = new System.Drawing.Size(106, 60);
            this.lblPayTypeTitle.TabIndex = 10;
            this.lblPayTypeTitle.Text = "طريقة الدفع:";
            this.lblPayTypeTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ddlPaymentType
            // 
            this.ddlPaymentType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ddlPaymentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlPaymentType.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.ddlPaymentType.Location = new System.Drawing.Point(430, 123);
            this.ddlPaymentType.Name = "ddlPaymentType";
            this.ddlPaymentType.Size = new System.Drawing.Size(185, 25);
            this.ddlPaymentType.TabIndex = 11;
            // 
            // lblNotesTitle
            // 
            this.lblNotesTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNotesTitle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblNotesTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblNotesTitle.Location = new System.Drawing.Point(310, 120);
            this.lblNotesTitle.Name = "lblNotesTitle";
            this.lblNotesTitle.Size = new System.Drawing.Size(114, 60);
            this.lblNotesTitle.TabIndex = 12;
            this.lblNotesTitle.Text = "ملاحظات:";
            this.lblNotesTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtNotes
            // 
            this.txtNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNotes.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.txtNotes.Location = new System.Drawing.Point(3, 123);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(301, 54);
            this.txtNotes.TabIndex = 13;
            // 
            // pnlMessage
            // 
            this.pnlMessage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(240)))), ((int)(((byte)(255)))));
            this.gridAdd.SetColumnSpan(this.pnlMessage, 3);
            this.pnlMessage.Controls.Add(this.lblMessage);
            this.pnlMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMessage.Location = new System.Drawing.Point(621, 183);
            this.pnlMessage.Name = "pnlMessage";
            this.pnlMessage.Padding = new System.Windows.Forms.Padding(8);
            this.pnlMessage.Size = new System.Drawing.Size(494, 40);
            this.pnlMessage.TabIndex = 14;
            this.pnlMessage.Visible = false;
            // 
            // lblMessage
            // 
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.lblMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(87)))), ((int)(((byte)(183)))));
            this.lblMessage.Location = new System.Drawing.Point(8, 8);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(478, 24);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // buttonsLayout
            // 
            this.buttonsLayout.ColumnCount = 3;
            this.gridAdd.SetColumnSpan(this.buttonsLayout, 3);
            this.buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.buttonsLayout.Controls.Add(this.btnAdd, 0, 0);
            this.buttonsLayout.Controls.Add(this.btnRefresh, 1, 0);
            this.buttonsLayout.Controls.Add(this.btnClear, 2, 0);
            this.buttonsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonsLayout.Location = new System.Drawing.Point(3, 183);
            this.buttonsLayout.Name = "buttonsLayout";
            this.buttonsLayout.RowCount = 1;
            this.buttonsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));
            this.buttonsLayout.Size = new System.Drawing.Size(612, 40);
            this.buttonsLayout.TabIndex = 15;
            // 
            // pnlGridHeader
            // 
            this.pnlGridHeader.BackColor = System.Drawing.Color.White;
            this.pnlGridHeader.Controls.Add(this.headerButtons);
            this.pnlGridHeader.Controls.Add(this.lblGridTitle);
            this.pnlGridHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGridHeader.Location = new System.Drawing.Point(3, 283);
            this.pnlGridHeader.Name = "pnlGridHeader";
            this.pnlGridHeader.Padding = new System.Windows.Forms.Padding(15, 8, 15, 8);
            this.pnlGridHeader.Size = new System.Drawing.Size(1148, 44);
            this.pnlGridHeader.TabIndex = 1;
            // 
            // headerButtons
            // 
            this.headerButtons.AutoSize = true;
            this.headerButtons.Controls.Add(this.btnExport);
            this.headerButtons.Dock = System.Windows.Forms.DockStyle.Right;
            this.headerButtons.Location = new System.Drawing.Point(1089, 8);
            this.headerButtons.Name = "headerButtons";
            this.headerButtons.Size = new System.Drawing.Size(44, 28);
            this.headerButtons.TabIndex = 0;
            this.headerButtons.WrapContents = false;
            // 
            // lblGridTitle
            // 
            this.lblGridTitle.AutoSize = true;
            this.lblGridTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblGridTitle.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblGridTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.lblGridTitle.Location = new System.Drawing.Point(15, 8);
            this.lblGridTitle.Name = "lblGridTitle";
            this.lblGridTitle.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblGridTitle.Size = new System.Drawing.Size(114, 26);
            this.lblGridTitle.TabIndex = 1;
            this.lblGridTitle.Text = "سجل المدفوعات";
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(240)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(87)))), ((int)(((byte)(183)))));
            this.dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.BackgroundColor = System.Drawing.Color.White;
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(240)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(87)))), ((int)(((byte)(183)))));
            dataGridViewCellStyle2.Padding = new System.Windows.Forms.Padding(5);
            this.dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dgv.ColumnHeadersHeight = 40;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(5);
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(240)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(87)))), ((int)(((byte)(183)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv.DefaultCellStyle = dataGridViewCellStyle3;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.Location = new System.Drawing.Point(3, 333);
            this.dgv.MultiSelect = false;
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersVisible = false;
            this.dgv.RowTemplate.Height = 35;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(1148, 308);
            this.dgv.TabIndex = 2;
            // 
            // PaymentsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.mainLayout);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "PaymentsForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "إدارة المدفوعات";
            ((System.ComponentModel.ISupportInitialize)(this.picSearch)).EndInit();
            this.mainLayout.ResumeLayout(false);
            this.pnlContent.ResumeLayout(false);
            this.contentLayout.ResumeLayout(false);
            this.grpAdd.ResumeLayout(false);
            this.gridAdd.ResumeLayout(false);
            this.gridAdd.PerformLayout();
            this.pnlSearch.ResumeLayout(false);
            this.searchLayout.ResumeLayout(false);
            this.searchLayout.PerformLayout();
            this.pnlBalance.ResumeLayout(false);
            this.pnlMessage.ResumeLayout(false);
            this.buttonsLayout.ResumeLayout(false);
            this.pnlGridHeader.ResumeLayout(false);
            this.pnlGridHeader.PerformLayout();
            this.headerButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }

        // موجود لتفادي أخطاء الـ Designer
        private void PaymentsForm_Load(object sender, System.EventArgs e) { }
    }
}

/*namespace water3
{
    partial class PaymentsForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel mainLayout;

        private System.Windows.Forms.GroupBox grpAdd;
        private System.Windows.Forms.TableLayoutPanel gridAdd;

        private System.Windows.Forms.Label lblSubSearch;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.TextBox txtSubscriberSearch;
        private System.Windows.Forms.ListBox lstSubscribers;

        private System.Windows.Forms.Label lblBalanceTitle;
        private System.Windows.Forms.Label lblCurrentBalance;

        private System.Windows.Forms.Label lblCollectorTitle;
        private System.Windows.Forms.ComboBox ddlCollectors;

        private System.Windows.Forms.Label lblDateTitle;
        private System.Windows.Forms.DateTimePicker dtpPaymentDate;

        private System.Windows.Forms.Label lblAmountTitle;
        private System.Windows.Forms.TextBox txtAmount;

        private System.Windows.Forms.Label lblPayTypeTitle;
        private System.Windows.Forms.ComboBox ddlPaymentType;

        private System.Windows.Forms.Label lblNotesTitle;
        private System.Windows.Forms.TextBox txtNotes;

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblMessage;

        private System.Windows.Forms.DataGridView dgv;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.grpAdd = new System.Windows.Forms.GroupBox();
            this.gridAdd = new System.Windows.Forms.TableLayoutPanel();
            this.lblSubSearch = new System.Windows.Forms.Label();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.lstSubscribers = new System.Windows.Forms.ListBox();
            this.txtSubscriberSearch = new System.Windows.Forms.TextBox();
            this.lblBalanceTitle = new System.Windows.Forms.Label();
            this.lblCurrentBalance = new System.Windows.Forms.Label();
            this.lblCollectorTitle = new System.Windows.Forms.Label();
            this.ddlCollectors = new System.Windows.Forms.ComboBox();
            this.lblDateTitle = new System.Windows.Forms.Label();
            this.dtpPaymentDate = new System.Windows.Forms.DateTimePicker();
            this.lblAmountTitle = new System.Windows.Forms.Label();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.lblPayTypeTitle = new System.Windows.Forms.Label();
            this.ddlPaymentType = new System.Windows.Forms.ComboBox();
            this.lblNotesTitle = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.mainLayout.SuspendLayout();
            this.grpAdd.SuspendLayout();
            this.gridAdd.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.mainLayout.Controls.Add(this.grpAdd, 0, 0);
            this.mainLayout.Controls.Add(this.dgv, 0, 1);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new System.Windows.Forms.Padding(15);
            this.mainLayout.RowCount = 2;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 290F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Size = new System.Drawing.Size(1093, 553);
            this.mainLayout.TabIndex = 0;
            // 
            // grpAdd
            // 
            this.grpAdd.BackColor = System.Drawing.Color.WhiteSmoke;
            this.grpAdd.Controls.Add(this.gridAdd);
            this.grpAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpAdd.Location = new System.Drawing.Point(18, 18);
            this.grpAdd.Name = "grpAdd";
            this.grpAdd.Size = new System.Drawing.Size(1057, 284);
            this.grpAdd.TabIndex = 0;
            this.grpAdd.TabStop = false;
            this.grpAdd.Text = "إضافة دفعة جديدة";
            // 
            // gridAdd
            // 
            this.gridAdd.ColumnCount = 7;
            this.gridAdd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.829365F));
            this.gridAdd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.35714F));
            this.gridAdd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.72493F));
            this.gridAdd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.18651F));
            this.gridAdd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 6.25F));
            this.gridAdd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 29.6627F));
            this.gridAdd.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.gridAdd.Controls.Add(this.pnlSearch, 1, 0);
            this.gridAdd.Controls.Add(this.lblCurrentBalance, 4, 0);
            this.gridAdd.Controls.Add(this.lblCollectorTitle, 0, 1);
            this.gridAdd.Controls.Add(this.lblDateTitle, 3, 1);
            this.gridAdd.Controls.Add(this.dtpPaymentDate, 4, 1);
            this.gridAdd.Controls.Add(this.txtAmount, 1, 2);
            this.gridAdd.Controls.Add(this.ddlPaymentType, 3, 2);
            this.gridAdd.Controls.Add(this.lblNotesTitle, 4, 2);
            this.gridAdd.Controls.Add(this.txtNotes, 5, 2);
            this.gridAdd.Controls.Add(this.btnAdd, 0, 3);
            this.gridAdd.Controls.Add(this.btnRefresh, 2, 3);
            this.gridAdd.Controls.Add(this.lblMessage, 3, 3);
            this.gridAdd.Controls.Add(this.ddlCollectors, 1, 1);
            this.gridAdd.Controls.Add(this.lblAmountTitle, 0, 2);
            this.gridAdd.Controls.Add(this.lblPayTypeTitle, 2, 2);
            this.gridAdd.Controls.Add(this.lblSubSearch, 0, 0);
            this.gridAdd.Controls.Add(this.lblBalanceTitle, 3, 0);
            this.gridAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAdd.Location = new System.Drawing.Point(3, 16);
            this.gridAdd.Name = "gridAdd";
            this.gridAdd.Padding = new System.Windows.Forms.Padding(10);
            this.gridAdd.RowCount = 4;
            this.gridAdd.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 129F));
            this.gridAdd.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.gridAdd.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 39F));
            this.gridAdd.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            this.gridAdd.Size = new System.Drawing.Size(1051, 265);
            this.gridAdd.TabIndex = 0;
            // 
            // lblSubSearch
            // 
            this.lblSubSearch.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblSubSearch.AutoEllipsis = true;
            this.lblSubSearch.AutoSize = true;
            this.lblSubSearch.Location = new System.Drawing.Point(955, 68);
            this.lblSubSearch.Name = "lblSubSearch";
            this.lblSubSearch.Size = new System.Drawing.Size(69, 13);
            this.lblSubSearch.TabIndex = 0;
            this.lblSubSearch.Text = "بحث المشترك:";
            // 
            // pnlSearch
            // 
            this.gridAdd.SetColumnSpan(this.pnlSearch, 2);
            this.pnlSearch.Controls.Add(this.lstSubscribers);
            this.pnlSearch.Controls.Add(this.txtSubscriberSearch);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearch.Location = new System.Drawing.Point(541, 13);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(408, 123);
            this.pnlSearch.TabIndex = 1;
            // 
            // lstSubscribers
            // 
            this.lstSubscribers.ColumnWidth = 7;
            this.lstSubscribers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstSubscribers.Location = new System.Drawing.Point(0, 20);
            this.lstSubscribers.Name = "lstSubscribers";
            this.lstSubscribers.Size = new System.Drawing.Size(408, 103);
            this.lstSubscribers.TabIndex = 0;
            this.lstSubscribers.Visible = false;
            // 
            // txtSubscriberSearch
            // 
            this.txtSubscriberSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtSubscriberSearch.Location = new System.Drawing.Point(0, 0);
            this.txtSubscriberSearch.Name = "txtSubscriberSearch";
            this.txtSubscriberSearch.Size = new System.Drawing.Size(408, 20);
            this.txtSubscriberSearch.TabIndex = 1;
            // 
            // lblBalanceTitle
            // 
            this.lblBalanceTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblBalanceTitle.AutoSize = true;
            this.lblBalanceTitle.Location = new System.Drawing.Point(398, 68);
            this.lblBalanceTitle.Name = "lblBalanceTitle";
            this.lblBalanceTitle.Size = new System.Drawing.Size(72, 13);
            this.lblBalanceTitle.TabIndex = 2;
            this.lblBalanceTitle.Text = "الرصيد الحالي:";
            // 
            // lblCurrentBalance
            // 
            this.lblCurrentBalance.BackColor = System.Drawing.Color.White;
            this.gridAdd.SetColumnSpan(this.lblCurrentBalance, 2);
            this.lblCurrentBalance.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrentBalance.Location = new System.Drawing.Point(36, 10);
            this.lblCurrentBalance.Name = "lblCurrentBalance";
            this.lblCurrentBalance.Size = new System.Drawing.Size(356, 129);
            this.lblCurrentBalance.TabIndex = 3;
            this.lblCurrentBalance.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCollectorTitle
            // 
            this.lblCollectorTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblCollectorTitle.AutoSize = true;
            this.lblCollectorTitle.Location = new System.Drawing.Point(955, 154);
            this.lblCollectorTitle.Name = "lblCollectorTitle";
            this.lblCollectorTitle.Size = new System.Drawing.Size(64, 13);
            this.lblCollectorTitle.TabIndex = 4;
            this.lblCollectorTitle.Text = "اسم المحصل:";
            // 
            // ddlCollectors
            // 
            this.gridAdd.SetColumnSpan(this.ddlCollectors, 2);
            this.ddlCollectors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ddlCollectors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlCollectors.Location = new System.Drawing.Point(541, 142);
            this.ddlCollectors.Name = "ddlCollectors";
            this.ddlCollectors.Size = new System.Drawing.Size(408, 21);
            this.ddlCollectors.TabIndex = 5;
            // 
            // lblDateTitle
            // 
            this.lblDateTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDateTitle.AutoSize = true;
            this.lblDateTitle.Location = new System.Drawing.Point(398, 154);
            this.lblDateTitle.Name = "lblDateTitle";
            this.lblDateTitle.Size = new System.Drawing.Size(62, 13);
            this.lblDateTitle.TabIndex = 6;
            this.lblDateTitle.Text = "تاريخ الدفع:";
            // 
            // dtpPaymentDate
            // 
            this.gridAdd.SetColumnSpan(this.dtpPaymentDate, 2);
            this.dtpPaymentDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtpPaymentDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpPaymentDate.Location = new System.Drawing.Point(36, 142);
            this.dtpPaymentDate.Name = "dtpPaymentDate";
            this.dtpPaymentDate.Size = new System.Drawing.Size(356, 20);
            this.dtpPaymentDate.TabIndex = 7;
            // 
            // lblAmountTitle
            // 
            this.lblAmountTitle.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblAmountTitle.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.lblAmountTitle.Location = new System.Drawing.Point(959, 183);
            this.lblAmountTitle.Name = "lblAmountTitle";
            this.lblAmountTitle.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblAmountTitle.Size = new System.Drawing.Size(76, 38);
            this.lblAmountTitle.TabIndex = 8;
            this.lblAmountTitle.Text = "المبلغ:";
            this.lblAmountTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblAmountTitle.UseWaitCursor = true;
            // 
            // txtAmount
            // 
            this.txtAmount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAmount.Location = new System.Drawing.Point(649, 186);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(300, 20);
            this.txtAmount.TabIndex = 9;
            // 
            // lblPayTypeTitle
            // 
            this.lblPayTypeTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPayTypeTitle.AutoSize = true;
            this.lblPayTypeTitle.Location = new System.Drawing.Point(541, 196);
            this.lblPayTypeTitle.Name = "lblPayTypeTitle";
            this.lblPayTypeTitle.Size = new System.Drawing.Size(63, 13);
            this.lblPayTypeTitle.TabIndex = 10;
            this.lblPayTypeTitle.Text = "طريقة الدفع:";
            this.lblPayTypeTitle.Click += new System.EventHandler(this.lblPayTypeTitle_Click);
            // 
            // ddlPaymentType
            // 
            this.ddlPaymentType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ddlPaymentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ddlPaymentType.Location = new System.Drawing.Point(398, 186);
            this.ddlPaymentType.Name = "ddlPaymentType";
            this.ddlPaymentType.Size = new System.Drawing.Size(137, 21);
            this.ddlPaymentType.TabIndex = 11;
            // 
            // lblNotesTitle
            // 
            this.lblNotesTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblNotesTitle.AutoSize = true;
            this.lblNotesTitle.Location = new System.Drawing.Point(335, 196);
            this.lblNotesTitle.Name = "lblNotesTitle";
            this.lblNotesTitle.Size = new System.Drawing.Size(48, 13);
            this.lblNotesTitle.TabIndex = 12;
            this.lblNotesTitle.Text = "ملاحظات:";
            // 
            // txtNotes
            // 
            this.txtNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNotes.Location = new System.Drawing.Point(36, 186);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.Size = new System.Drawing.Size(293, 33);
            this.txtNotes.TabIndex = 13;
            // 
            // btnAdd
            // 
            this.gridAdd.SetColumnSpan(this.btnAdd, 2);
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAdd.Location = new System.Drawing.Point(649, 225);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(389, 27);
            this.btnAdd.TabIndex = 14;
            this.btnAdd.Text = "إضافة دفعة";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnRefresh.Location = new System.Drawing.Point(541, 225);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(102, 27);
            this.btnRefresh.TabIndex = 15;
            this.btnRefresh.Text = "تحديث";
            // 
            // lblMessage
            // 
            this.gridAdd.SetColumnSpan(this.lblMessage, 3);
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Location = new System.Drawing.Point(36, 222);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(499, 33);
            this.lblMessage.TabIndex = 16;
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgv
            // 
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.BackgroundColor = System.Drawing.Color.White;
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(18, 308);
            this.dgv.MultiSelect = false;
            this.dgv.Name = "dgv";
            this.dgv.ReadOnly = true;
            this.dgv.RowHeadersVisible = false;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.Size = new System.Drawing.Size(1057, 227);
            this.dgv.TabIndex = 1;
            // 
            // PaymentsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1093, 553);
            this.Controls.Add(this.mainLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PaymentsForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "إدارة المدفوعات";
            this.mainLayout.ResumeLayout(false);
            this.grpAdd.ResumeLayout(false);
            this.gridAdd.ResumeLayout(false);
            this.gridAdd.PerformLayout();
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
*/