using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace water3
{
    partial class SubscriberForm
    {
        private IContainer components = null;

        private Panel mainLayout;
        private Panel pnlFieldsCard;
        private Panel pnlSearchCard;

        private TableLayoutPanel fieldsCardLayout;
        private TableLayoutPanel fieldsGrid;
        private TableLayoutPanel buttonsLayout;

        private FlowLayoutPanel pnlSubscriberButtons;
        private FlowLayoutPanel pnlMeterButtons;
        private FlowLayoutPanel pnlSearch;

        private SplitContainer splitGrids;
        private DataGridView dgvSubscribers;
        private DataGridView dgvMeters;

        private Label lblName;
        private Label lblPhone;
        private Label lblAddress;
        private Label lblAccount;
        private Label lblMeterReading;
        private Label lblMeterNo;
        private Label lblMeterLocation;

        private TextBox txtName;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private TextBox txtMeterReading;
        private TextBox txtNewMeterNumber;
        private TextBox txtNewMeterLocation;
        private TextBox txtSearch;

        private ComboBox cboAccount;
        private CheckBox chkIsPrimary;
        private DateTimePicker dateFilter;

        private Button btnAdd;
        private Button btnUpdate;
        private Button btnClear;
        private Button btnCreateAccount;
        private Button btnAddMeter;
        private Button btnSetPrimary;
        private Button btnUnlinkMeter;
        private Button btnImportData;
        private Button btnSaveImported;
        private Button btnSearch;
        private Button btnFilterDate;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.mainLayout = new System.Windows.Forms.Panel();
            this.splitGrids = new System.Windows.Forms.SplitContainer();
            this.dgvSubscribers = new System.Windows.Forms.DataGridView();
            this.dgvMeters = new System.Windows.Forms.DataGridView();
            this.pnlSearchCard = new System.Windows.Forms.Panel();
            this.pnlSearch = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnFilterDate = new System.Windows.Forms.Button();
            this.dateFilter = new System.Windows.Forms.DateTimePicker();
            this.pnlFieldsCard = new System.Windows.Forms.Panel();
            this.fieldsCardLayout = new System.Windows.Forms.TableLayoutPanel();
            this.fieldsGrid = new System.Windows.Forms.TableLayoutPanel();
            this.cboAccount = new System.Windows.Forms.ComboBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtMeterReading = new System.Windows.Forms.TextBox();
            this.lblMeterReading = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.lblPhone = new System.Windows.Forms.Label();
            this.txtNewMeterNumber = new System.Windows.Forms.TextBox();
            this.lblMeterNo = new System.Windows.Forms.Label();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.lblAddress = new System.Windows.Forms.Label();
            this.txtNewMeterLocation = new System.Windows.Forms.TextBox();
            this.lblMeterLocation = new System.Windows.Forms.Label();
            this.chkIsPrimary = new System.Windows.Forms.CheckBox();
            this.lblAccount = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.buttonsLayout = new System.Windows.Forms.TableLayoutPanel();
            this.pnlSubscriberButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnCreateAccount = new System.Windows.Forms.Button();
            this.pnlMeterButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddMeter = new System.Windows.Forms.Button();
            this.btnSetPrimary = new System.Windows.Forms.Button();
            this.btnUnlinkMeter = new System.Windows.Forms.Button();
            this.btnImportData = new System.Windows.Forms.Button();
            this.btnSaveImported = new System.Windows.Forms.Button();
            this.mainLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitGrids)).BeginInit();
            this.splitGrids.Panel1.SuspendLayout();
            this.splitGrids.Panel2.SuspendLayout();
            this.splitGrids.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubscribers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMeters)).BeginInit();
            this.pnlSearchCard.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.pnlFieldsCard.SuspendLayout();
            this.fieldsCardLayout.SuspendLayout();
            this.fieldsGrid.SuspendLayout();
            this.buttonsLayout.SuspendLayout();
            this.pnlSubscriberButtons.SuspendLayout();
            this.pnlMeterButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.BackColor = System.Drawing.Color.Transparent;
            this.mainLayout.Controls.Add(this.splitGrids);
            this.mainLayout.Controls.Add(this.pnlSearchCard);
            this.mainLayout.Controls.Add(this.pnlFieldsCard);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new System.Windows.Forms.Padding(12);
            this.mainLayout.Size = new System.Drawing.Size(1200, 700);
            this.mainLayout.TabIndex = 0;
            // 
            // splitGrids
            // 
            this.splitGrids.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitGrids.Location = new System.Drawing.Point(12, 321);
            this.splitGrids.Name = "splitGrids";
            this.splitGrids.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitGrids.Panel1
            // 
            this.splitGrids.Panel1.Controls.Add(this.dgvSubscribers);
            this.splitGrids.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.splitGrids.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.splitGrids.Panel1MinSize = 150;
            // 
            // splitGrids.Panel2
            // 
            this.splitGrids.Panel2.Controls.Add(this.dgvMeters);
            this.splitGrids.Panel2.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.splitGrids.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.splitGrids.Size = new System.Drawing.Size(1176, 367);
            this.splitGrids.SplitterDistance = 223;
            this.splitGrids.TabIndex = 0;
            // 
            // dgvSubscribers
            // 
            this.dgvSubscribers.AllowUserToAddRows = false;
            this.dgvSubscribers.AllowUserToDeleteRows = false;
            this.dgvSubscribers.BackgroundColor = System.Drawing.Color.White;
            this.dgvSubscribers.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvSubscribers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSubscribers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSubscribers.Location = new System.Drawing.Point(0, 0);
            this.dgvSubscribers.MultiSelect = false;
            this.dgvSubscribers.Name = "dgvSubscribers";
            this.dgvSubscribers.ReadOnly = true;
            this.dgvSubscribers.RowHeadersVisible = false;
            this.dgvSubscribers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSubscribers.Size = new System.Drawing.Size(1176, 217);
            this.dgvSubscribers.TabIndex = 0;
            // 
            // dgvMeters
            // 
            this.dgvMeters.AllowUserToAddRows = false;
            this.dgvMeters.AllowUserToDeleteRows = false;
            this.dgvMeters.BackgroundColor = System.Drawing.Color.White;
            this.dgvMeters.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvMeters.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMeters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMeters.Location = new System.Drawing.Point(0, 6);
            this.dgvMeters.MultiSelect = false;
            this.dgvMeters.Name = "dgvMeters";
            this.dgvMeters.ReadOnly = true;
            this.dgvMeters.RowHeadersVisible = false;
            this.dgvMeters.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvMeters.Size = new System.Drawing.Size(1176, 134);
            this.dgvMeters.TabIndex = 0;
            // 
            // pnlSearchCard
            // 
            this.pnlSearchCard.BackColor = System.Drawing.Color.White;
            this.pnlSearchCard.Controls.Add(this.pnlSearch);
            this.pnlSearchCard.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearchCard.Location = new System.Drawing.Point(12, 251);
            this.pnlSearchCard.Name = "pnlSearchCard";
            this.pnlSearchCard.Padding = new System.Windows.Forms.Padding(12);
            this.pnlSearchCard.Size = new System.Drawing.Size(1176, 70);
            this.pnlSearchCard.TabIndex = 1;
            // 
            // pnlSearch
            // 
            this.pnlSearch.Controls.Add(this.btnSearch);
            this.pnlSearch.Controls.Add(this.txtSearch);
            this.pnlSearch.Controls.Add(this.btnFilterDate);
            this.pnlSearch.Controls.Add(this.dateFilter);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearch.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlSearch.Location = new System.Drawing.Point(12, 12);
            this.pnlSearch.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(1152, 46);
            this.pnlSearch.TabIndex = 0;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(3, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 0;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(84, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(100, 22);
            this.txtSearch.TabIndex = 1;
            // 
            // btnFilterDate
            // 
            this.btnFilterDate.Location = new System.Drawing.Point(190, 3);
            this.btnFilterDate.Name = "btnFilterDate";
            this.btnFilterDate.Size = new System.Drawing.Size(75, 23);
            this.btnFilterDate.TabIndex = 2;
            // 
            // dateFilter
            // 
            this.dateFilter.Location = new System.Drawing.Point(271, 3);
            this.dateFilter.Name = "dateFilter";
            this.dateFilter.Size = new System.Drawing.Size(200, 22);
            this.dateFilter.TabIndex = 3;
            // 
            // pnlFieldsCard
            // 
            this.pnlFieldsCard.BackColor = System.Drawing.Color.White;
            this.pnlFieldsCard.Controls.Add(this.fieldsCardLayout);
            this.pnlFieldsCard.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlFieldsCard.Location = new System.Drawing.Point(12, 12);
            this.pnlFieldsCard.Name = "pnlFieldsCard";
            this.pnlFieldsCard.Padding = new System.Windows.Forms.Padding(12);
            this.pnlFieldsCard.Size = new System.Drawing.Size(1176, 239);
            this.pnlFieldsCard.TabIndex = 2;
            // 
            // fieldsCardLayout
            // 
            this.fieldsCardLayout.ColumnCount = 1;
            this.fieldsCardLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.fieldsCardLayout.Controls.Add(this.fieldsGrid, 0, 0);
            this.fieldsCardLayout.Controls.Add(this.buttonsLayout, 0, 1);
            this.fieldsCardLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldsCardLayout.Location = new System.Drawing.Point(12, 12);
            this.fieldsCardLayout.Margin = new System.Windows.Forms.Padding(0);
            this.fieldsCardLayout.Name = "fieldsCardLayout";
            this.fieldsCardLayout.RowCount = 2;
            this.fieldsCardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 154F));
            this.fieldsCardLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 61F));
            this.fieldsCardLayout.Size = new System.Drawing.Size(1152, 215);
            this.fieldsCardLayout.TabIndex = 0;
            // 
            // fieldsGrid
            // 
            this.fieldsGrid.BackColor = System.Drawing.Color.Transparent;
            this.fieldsGrid.ColumnCount = 4;
            this.fieldsGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.fieldsGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.fieldsGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
            this.fieldsGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.fieldsGrid.Controls.Add(this.cboAccount, 0, 0);
            this.fieldsGrid.Controls.Add(this.txtName, 2, 0);
            this.fieldsGrid.Controls.Add(this.txtMeterReading, 0, 1);
            this.fieldsGrid.Controls.Add(this.lblMeterReading, 1, 1);
            this.fieldsGrid.Controls.Add(this.txtPhone, 2, 1);
            this.fieldsGrid.Controls.Add(this.lblPhone, 3, 1);
            this.fieldsGrid.Controls.Add(this.lblMeterNo, 1, 2);
            this.fieldsGrid.Controls.Add(this.txtAddress, 2, 2);
            this.fieldsGrid.Controls.Add(this.lblAddress, 3, 2);
            this.fieldsGrid.Controls.Add(this.txtNewMeterLocation, 0, 3);
            this.fieldsGrid.Controls.Add(this.lblMeterLocation, 1, 3);
            this.fieldsGrid.Controls.Add(this.chkIsPrimary, 2, 3);
            this.fieldsGrid.Controls.Add(this.lblAccount, 3, 0);
            this.fieldsGrid.Controls.Add(this.lblName, 1, 0);
            this.fieldsGrid.Controls.Add(this.txtNewMeterNumber, 0, 2);
            this.fieldsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fieldsGrid.Location = new System.Drawing.Point(0, 0);
            this.fieldsGrid.Margin = new System.Windows.Forms.Padding(0);
            this.fieldsGrid.Name = "fieldsGrid";
            this.fieldsGrid.RowCount = 4;
            this.fieldsGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.fieldsGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.fieldsGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.fieldsGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.fieldsGrid.Size = new System.Drawing.Size(1152, 154);
            this.fieldsGrid.TabIndex = 0;
            // 
            // cboAccount
            // 
            this.cboAccount.Location = new System.Drawing.Point(1028, 3);
            this.cboAccount.Name = "cboAccount";
            this.cboAccount.Size = new System.Drawing.Size(121, 22);
            this.cboAccount.TabIndex = 0;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(474, 3);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(100, 22);
            this.txtName.TabIndex = 2;
            // 
            // txtMeterReading
            // 
            this.txtMeterReading.Location = new System.Drawing.Point(1049, 43);
            this.txtMeterReading.Multiline = true;
            this.txtMeterReading.Name = "txtMeterReading";
            this.txtMeterReading.Size = new System.Drawing.Size(100, 22);
            this.txtMeterReading.TabIndex = 4;
            // 
            // lblMeterReading
            // 
            this.lblMeterReading.Location = new System.Drawing.Point(658, 40);
            this.lblMeterReading.Name = "lblMeterReading";
            this.lblMeterReading.Size = new System.Drawing.Size(100, 23);
            this.lblMeterReading.TabIndex = 5;
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(474, 43);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(100, 22);
            this.txtPhone.TabIndex = 6;
            // 
            // lblPhone
            // 
            this.lblPhone.Location = new System.Drawing.Point(83, 40);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(100, 23);
            this.lblPhone.TabIndex = 7;
            // 
            // txtNewMeterNumber
            // 
            this.txtNewMeterNumber.Location = new System.Drawing.Point(1049, 83);
            this.txtNewMeterNumber.Name = "txtNewMeterNumber";
            this.txtNewMeterNumber.Size = new System.Drawing.Size(100, 22);
            this.txtNewMeterNumber.TabIndex = 8;
            // 
            // lblMeterNo
            // 
            this.lblMeterNo.Location = new System.Drawing.Point(658, 80);
            this.lblMeterNo.Name = "lblMeterNo";
            this.lblMeterNo.Size = new System.Drawing.Size(100, 23);
            this.lblMeterNo.TabIndex = 9;
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(474, 83);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(100, 22);
            this.txtAddress.TabIndex = 10;
            // 
            // lblAddress
            // 
            this.lblAddress.Location = new System.Drawing.Point(83, 80);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(100, 23);
            this.lblAddress.TabIndex = 11;
            // 
            // txtNewMeterLocation
            // 
            this.txtNewMeterLocation.Location = new System.Drawing.Point(1049, 123);
            this.txtNewMeterLocation.Name = "txtNewMeterLocation";
            this.txtNewMeterLocation.Size = new System.Drawing.Size(100, 22);
            this.txtNewMeterLocation.TabIndex = 12;
            // 
            // lblMeterLocation
            // 
            this.lblMeterLocation.Location = new System.Drawing.Point(658, 120);
            this.lblMeterLocation.Name = "lblMeterLocation";
            this.lblMeterLocation.Size = new System.Drawing.Size(100, 23);
            this.lblMeterLocation.TabIndex = 13;
            // 
            // chkIsPrimary
            // 
            this.fieldsGrid.SetColumnSpan(this.chkIsPrimary, 2);
            this.chkIsPrimary.Location = new System.Drawing.Point(470, 123);
            this.chkIsPrimary.Name = "chkIsPrimary";
            this.chkIsPrimary.Size = new System.Drawing.Size(104, 24);
            this.chkIsPrimary.TabIndex = 14;
            // 
            // lblAccount
            // 
            this.lblAccount.Location = new System.Drawing.Point(83, 0);
            this.lblAccount.Name = "lblAccount";
            this.lblAccount.Size = new System.Drawing.Size(100, 23);
            this.lblAccount.TabIndex = 1;
            // 
            // lblName
            // 
            this.lblName.Location = new System.Drawing.Point(658, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(100, 23);
            this.lblName.TabIndex = 3;
            // 
            // buttonsLayout
            // 
            this.buttonsLayout.ColumnCount = 2;
            this.buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.buttonsLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.buttonsLayout.Controls.Add(this.pnlSubscriberButtons, 0, 0);
            this.buttonsLayout.Controls.Add(this.pnlMeterButtons, 1, 0);
            this.buttonsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonsLayout.Location = new System.Drawing.Point(0, 164);
            this.buttonsLayout.Margin = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.buttonsLayout.Name = "buttonsLayout";
            this.buttonsLayout.RowCount = 1;
            this.buttonsLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.buttonsLayout.Size = new System.Drawing.Size(1152, 51);
            this.buttonsLayout.TabIndex = 1;
            // 
            // pnlSubscriberButtons
            // 
            this.pnlSubscriberButtons.Controls.Add(this.btnAdd);
            this.pnlSubscriberButtons.Controls.Add(this.btnUpdate);
            this.pnlSubscriberButtons.Controls.Add(this.btnClear);
            this.pnlSubscriberButtons.Controls.Add(this.btnCreateAccount);
            this.pnlSubscriberButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlSubscriberButtons.Location = new System.Drawing.Point(582, 0);
            this.pnlSubscriberButtons.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.pnlSubscriberButtons.Name = "pnlSubscriberButtons";
            this.pnlSubscriberButtons.Size = new System.Drawing.Size(570, 37);
            this.pnlSubscriberButtons.TabIndex = 0;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(3, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 0;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Location = new System.Drawing.Point(84, 3);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 23);
            this.btnUpdate.TabIndex = 1;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(165, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 2;
            // 
            // btnCreateAccount
            // 
            this.btnCreateAccount.Location = new System.Drawing.Point(246, 3);
            this.btnCreateAccount.Name = "btnCreateAccount";
            this.btnCreateAccount.Size = new System.Drawing.Size(75, 23);
            this.btnCreateAccount.TabIndex = 3;
            // 
            // pnlMeterButtons
            // 
            this.pnlMeterButtons.Controls.Add(this.btnAddMeter);
            this.pnlMeterButtons.Controls.Add(this.btnSetPrimary);
            this.pnlMeterButtons.Controls.Add(this.btnUnlinkMeter);
            this.pnlMeterButtons.Controls.Add(this.btnImportData);
            this.pnlMeterButtons.Controls.Add(this.btnSaveImported);
            this.pnlMeterButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.pnlMeterButtons.Location = new System.Drawing.Point(0, 0);
            this.pnlMeterButtons.Margin = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.pnlMeterButtons.Name = "pnlMeterButtons";
            this.pnlMeterButtons.Size = new System.Drawing.Size(570, 37);
            this.pnlMeterButtons.TabIndex = 1;
            // 
            // btnAddMeter
            // 
            this.btnAddMeter.Location = new System.Drawing.Point(3, 3);
            this.btnAddMeter.Name = "btnAddMeter";
            this.btnAddMeter.Size = new System.Drawing.Size(75, 23);
            this.btnAddMeter.TabIndex = 0;
            // 
            // btnSetPrimary
            // 
            this.btnSetPrimary.Location = new System.Drawing.Point(84, 3);
            this.btnSetPrimary.Name = "btnSetPrimary";
            this.btnSetPrimary.Size = new System.Drawing.Size(75, 23);
            this.btnSetPrimary.TabIndex = 1;
            // 
            // btnUnlinkMeter
            // 
            this.btnUnlinkMeter.Location = new System.Drawing.Point(165, 3);
            this.btnUnlinkMeter.Name = "btnUnlinkMeter";
            this.btnUnlinkMeter.Size = new System.Drawing.Size(75, 23);
            this.btnUnlinkMeter.TabIndex = 2;
            // 
            // btnImportData
            // 
            this.btnImportData.Location = new System.Drawing.Point(246, 3);
            this.btnImportData.Name = "btnImportData";
            this.btnImportData.Size = new System.Drawing.Size(75, 23);
            this.btnImportData.TabIndex = 3;
            // 
            // btnSaveImported
            // 
            this.btnSaveImported.Location = new System.Drawing.Point(327, 3);
            this.btnSaveImported.Name = "btnSaveImported";
            this.btnSaveImported.Size = new System.Drawing.Size(75, 23);
            this.btnSaveImported.TabIndex = 4;
            // 
            // SubscriberForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(242)))), ((int)(((byte)(245)))));
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Controls.Add(this.mainLayout);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(980, 650);
            this.Name = "SubscriberForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "إدارة المشتركين والعدادات";
            this.mainLayout.ResumeLayout(false);
            this.splitGrids.Panel1.ResumeLayout(false);
            this.splitGrids.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitGrids)).EndInit();
            this.splitGrids.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubscribers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMeters)).EndInit();
            this.pnlSearchCard.ResumeLayout(false);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlFieldsCard.ResumeLayout(false);
            this.fieldsCardLayout.ResumeLayout(false);
            this.fieldsGrid.ResumeLayout(false);
            this.fieldsGrid.PerformLayout();
            this.buttonsLayout.ResumeLayout(false);
            this.pnlSubscriberButtons.ResumeLayout(false);
            this.pnlMeterButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
