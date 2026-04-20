using System.Windows.Forms;

namespace water3.Forms
{
    partial class ReadingF
    {
        private System.ComponentModel.IContainer components = null;

        private TableLayoutPanel mainLayout;
        private FlowLayoutPanel topPanel;

        private Label lblTariff;
        private Label lblLastInvoice;
        private Label lblLastReading;

        private GroupBox group;
        private TableLayoutPanel table;

        private Label lblSubTitle;
        private TextBox txtSubscriberSearch;

        private Label lblDateTitle;
        private DateTimePicker dtpReadingDate;

        private Label lblPrevTitle;
        private TextBox txtPreviousReading;

        private Label lblCurTitle;
        private TextBox txtCurrentReading;

        private Label lblConsTitle;
        private TextBox txtConsumption;

        private Label lblNotesTitle;
        private TextBox txtNotes;

        private Button btnAdd;
        private Button btnImportExcel;
        private Button btnImportExcelPreview;
        private Button btnConfirmImport;

        private Label lblMessage;
        private ListBox lstSubscriberSuggestions;
        private DataGridView dgvExcelPreview;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.topPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTariff = new System.Windows.Forms.Label();
            this.lblLastInvoice = new System.Windows.Forms.Label();
            this.lblLastReading = new System.Windows.Forms.Label();
            this.group = new System.Windows.Forms.GroupBox();
            this.table = new System.Windows.Forms.TableLayoutPanel();
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.txtSubscriberSearch = new System.Windows.Forms.TextBox();
            this.lblDateTitle = new System.Windows.Forms.Label();
            this.dtpReadingDate = new System.Windows.Forms.DateTimePicker();
            this.lblPrevTitle = new System.Windows.Forms.Label();
            this.txtPreviousReading = new System.Windows.Forms.TextBox();
            this.lblCurTitle = new System.Windows.Forms.Label();
            this.txtCurrentReading = new System.Windows.Forms.TextBox();
            this.lblConsTitle = new System.Windows.Forms.Label();
            this.txtConsumption = new System.Windows.Forms.TextBox();
            this.lblNotesTitle = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnImportExcel = new System.Windows.Forms.Button();
            this.btnImportExcelPreview = new System.Windows.Forms.Button();
            this.dgvExcelPreview = new System.Windows.Forms.DataGridView();
            this.btnConfirmImport = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lstSubscriberSuggestions = new System.Windows.Forms.ListBox();
            this.mainLayout.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.group.SuspendLayout();
            this.table.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExcelPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.BackColor = System.Drawing.Color.White;
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.topPanel, 0, 0);
            this.mainLayout.Controls.Add(this.group, 0, 1);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new System.Windows.Forms.Padding(18, 14, 18, 14);
            this.mainLayout.RowCount = 3;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.mainLayout.Size = new System.Drawing.Size(1180, 700);
            this.mainLayout.TabIndex = 0;
            // 
            // topPanel
            // 
            this.topPanel.AutoScroll = true;
            this.topPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(242)))), ((int)(((byte)(247)))));
            this.topPanel.Controls.Add(this.lblTariff);
            this.topPanel.Controls.Add(this.lblLastInvoice);
            this.topPanel.Controls.Add(this.lblLastReading);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.topPanel.Location = new System.Drawing.Point(21, 17);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(10);
            this.topPanel.Size = new System.Drawing.Size(1138, 64);
            this.topPanel.TabIndex = 0;
            this.topPanel.WrapContents = false;
            // 
            // lblTariff
            // 
            this.lblTariff.AutoSize = true;
            this.lblTariff.Location = new System.Drawing.Point(10, 16);
            this.lblTariff.Margin = new System.Windows.Forms.Padding(28, 6, 0, 0);
            this.lblTariff.Name = "lblTariff";
            this.lblTariff.Size = new System.Drawing.Size(54, 13);
            this.lblTariff.TabIndex = 0;
            this.lblTariff.Text = "التعرفة: ---";
            // 
            // lblLastInvoice
            // 
            this.lblLastInvoice.AutoSize = true;
            this.lblLastInvoice.Location = new System.Drawing.Point(92, 16);
            this.lblLastInvoice.Margin = new System.Windows.Forms.Padding(28, 6, 0, 0);
            this.lblLastInvoice.Name = "lblLastInvoice";
            this.lblLastInvoice.Size = new System.Drawing.Size(72, 13);
            this.lblLastInvoice.TabIndex = 1;
            this.lblLastInvoice.Text = "آخر فاتورة: ---";
            // 
            // lblLastReading
            // 
            this.lblLastReading.AutoSize = true;
            this.lblLastReading.Location = new System.Drawing.Point(192, 16);
            this.lblLastReading.Margin = new System.Windows.Forms.Padding(28, 6, 0, 0);
            this.lblLastReading.Name = "lblLastReading";
            this.lblLastReading.Size = new System.Drawing.Size(67, 13);
            this.lblLastReading.TabIndex = 2;
            this.lblLastReading.Text = "آخر قراءة: ---";
            // 
            // group
            // 
            this.group.BackColor = System.Drawing.Color.WhiteSmoke;
            this.group.Controls.Add(this.table);
            this.group.Dock = System.Windows.Forms.DockStyle.Fill;
            this.group.Location = new System.Drawing.Point(21, 87);
            this.group.Name = "group";
            this.group.Padding = new System.Windows.Forms.Padding(13, 10, 13, 7);
            this.group.Size = new System.Drawing.Size(1138, 595);
            this.group.TabIndex = 1;
            this.group.TabStop = false;
            this.group.Text = "إضافة قراءة جديدة";
            // 
            // table
            // 
            this.table.BackColor = System.Drawing.Color.WhiteSmoke;
            this.table.ColumnCount = 4;
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table.Controls.Add(this.lblSubTitle, 0, 0);
            this.table.Controls.Add(this.txtSubscriberSearch, 1, 0);
            this.table.Controls.Add(this.lblDateTitle, 2, 0);
            this.table.Controls.Add(this.dtpReadingDate, 3, 0);
            this.table.Controls.Add(this.lblPrevTitle, 0, 1);
            this.table.Controls.Add(this.txtPreviousReading, 1, 1);
            this.table.Controls.Add(this.lblCurTitle, 2, 1);
            this.table.Controls.Add(this.txtCurrentReading, 3, 1);
            this.table.Controls.Add(this.lblConsTitle, 0, 2);
            this.table.Controls.Add(this.txtConsumption, 1, 2);
            this.table.Controls.Add(this.lblNotesTitle, 2, 2);
            this.table.Controls.Add(this.txtNotes, 3, 2);
            this.table.Controls.Add(this.btnAdd, 1, 3);
            this.table.Controls.Add(this.btnImportExcel, 2, 3);
            this.table.Controls.Add(this.btnImportExcelPreview, 3, 3);
            this.table.Controls.Add(this.dgvExcelPreview, 0, 4);
            this.table.Controls.Add(this.btnConfirmImport, 2, 5);
            this.table.Controls.Add(this.lblMessage, 3, 5);
            this.table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table.Location = new System.Drawing.Point(13, 23);
            this.table.Name = "table";
            this.table.Padding = new System.Windows.Forms.Padding(12);
            this.table.RowCount = 6;
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.table.Size = new System.Drawing.Size(1112, 565);
            this.table.TabIndex = 0;
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblSubTitle.AutoSize = true;
            this.lblSubTitle.Location = new System.Drawing.Point(983, 27);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(77, 13);
            this.lblSubTitle.TabIndex = 0;
            this.lblSubTitle.Text = "المشترك/العداد:";
            // 
            // txtSubscriberSearch
            // 
            this.txtSubscriberSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSubscriberSearch.Location = new System.Drawing.Point(569, 15);
            this.txtSubscriberSearch.Name = "txtSubscriberSearch";
            this.txtSubscriberSearch.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtSubscriberSearch.Size = new System.Drawing.Size(408, 20);
            this.txtSubscriberSearch.TabIndex = 1;
            // 
            // lblDateTitle
            // 
            this.lblDateTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDateTitle.AutoSize = true;
            this.lblDateTitle.Location = new System.Drawing.Point(429, 27);
            this.lblDateTitle.Name = "lblDateTitle";
            this.lblDateTitle.Size = new System.Drawing.Size(69, 13);
            this.lblDateTitle.TabIndex = 2;
            this.lblDateTitle.Text = "تاريخ القراءة:";
            // 
            // dtpReadingDate
            // 
            this.dtpReadingDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtpReadingDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpReadingDate.Location = new System.Drawing.Point(15, 15);
            this.dtpReadingDate.Name = "dtpReadingDate";
            this.dtpReadingDate.Size = new System.Drawing.Size(408, 20);
            this.dtpReadingDate.TabIndex = 3;
            // 
            // lblPrevTitle
            // 
            this.lblPrevTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPrevTitle.AutoSize = true;
            this.lblPrevTitle.Location = new System.Drawing.Point(983, 71);
            this.lblPrevTitle.Name = "lblPrevTitle";
            this.lblPrevTitle.Size = new System.Drawing.Size(77, 13);
            this.lblPrevTitle.TabIndex = 4;
            this.lblPrevTitle.Text = "القراءة السابقة:";
            // 
            // txtPreviousReading
            // 
            this.txtPreviousReading.BackColor = System.Drawing.Color.Gainsboro;
            this.txtPreviousReading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPreviousReading.Location = new System.Drawing.Point(569, 59);
            this.txtPreviousReading.Name = "txtPreviousReading";
            this.txtPreviousReading.ReadOnly = true;
            this.txtPreviousReading.Size = new System.Drawing.Size(408, 20);
            this.txtPreviousReading.TabIndex = 5;
            this.txtPreviousReading.Text = "0";
            this.txtPreviousReading.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblCurTitle
            // 
            this.lblCurTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblCurTitle.AutoSize = true;
            this.lblCurTitle.Location = new System.Drawing.Point(429, 71);
            this.lblCurTitle.Name = "lblCurTitle";
            this.lblCurTitle.Size = new System.Drawing.Size(73, 13);
            this.lblCurTitle.TabIndex = 6;
            this.lblCurTitle.Text = "القراءة الحالية:";
            // 
            // txtCurrentReading
            // 
            this.txtCurrentReading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCurrentReading.Location = new System.Drawing.Point(15, 59);
            this.txtCurrentReading.Name = "txtCurrentReading";
            this.txtCurrentReading.Size = new System.Drawing.Size(408, 20);
            this.txtCurrentReading.TabIndex = 7;
            this.txtCurrentReading.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblConsTitle
            // 
            this.lblConsTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblConsTitle.AutoSize = true;
            this.lblConsTitle.Location = new System.Drawing.Point(983, 123);
            this.lblConsTitle.Name = "lblConsTitle";
            this.lblConsTitle.Size = new System.Drawing.Size(53, 13);
            this.lblConsTitle.TabIndex = 8;
            this.lblConsTitle.Text = "الاستهلاك:";
            // 
            // txtConsumption
            // 
            this.txtConsumption.BackColor = System.Drawing.Color.Gainsboro;
            this.txtConsumption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConsumption.Location = new System.Drawing.Point(569, 103);
            this.txtConsumption.Name = "txtConsumption";
            this.txtConsumption.ReadOnly = true;
            this.txtConsumption.Size = new System.Drawing.Size(408, 20);
            this.txtConsumption.TabIndex = 9;
            this.txtConsumption.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblNotesTitle
            // 
            this.lblNotesTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblNotesTitle.AutoSize = true;
            this.lblNotesTitle.Location = new System.Drawing.Point(429, 123);
            this.lblNotesTitle.Name = "lblNotesTitle";
            this.lblNotesTitle.Size = new System.Drawing.Size(48, 13);
            this.lblNotesTitle.TabIndex = 10;
            this.lblNotesTitle.Text = "ملاحظات:";
            // 
            // txtNotes
            // 
            this.txtNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNotes.Location = new System.Drawing.Point(15, 103);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(408, 54);
            this.txtNotes.TabIndex = 11;
            // 
            // btnAdd
            // 
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAdd.Enabled = false;
            this.btnAdd.Location = new System.Drawing.Point(569, 163);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(408, 46);
            this.btnAdd.TabIndex = 12;
            this.btnAdd.Text = "إضافة";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnImportExcel
            // 
            this.btnImportExcel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImportExcel.Location = new System.Drawing.Point(429, 163);
            this.btnImportExcel.Name = "btnImportExcel";
            this.btnImportExcel.Size = new System.Drawing.Size(134, 46);
            this.btnImportExcel.TabIndex = 13;
            this.btnImportExcel.Text = "استيراد مباشر";
            this.btnImportExcel.UseVisualStyleBackColor = true;
            // 
            // btnImportExcelPreview
            // 
            this.btnImportExcelPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnImportExcelPreview.Location = new System.Drawing.Point(15, 163);
            this.btnImportExcelPreview.Name = "btnImportExcelPreview";
            this.btnImportExcelPreview.Size = new System.Drawing.Size(408, 46);
            this.btnImportExcelPreview.TabIndex = 14;
            this.btnImportExcelPreview.Text = "معاينة ملف Excel قبل الاستيراد";
            this.btnImportExcelPreview.UseVisualStyleBackColor = true;
            // 
            // dgvExcelPreview
            // 
            this.dgvExcelPreview.AllowUserToAddRows = false;
            this.dgvExcelPreview.AllowUserToDeleteRows = false;
            this.dgvExcelPreview.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvExcelPreview.BackgroundColor = System.Drawing.Color.White;
            this.dgvExcelPreview.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.table.SetColumnSpan(this.dgvExcelPreview, 4);
            this.dgvExcelPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvExcelPreview.Location = new System.Drawing.Point(15, 215);
            this.dgvExcelPreview.Name = "dgvExcelPreview";
            this.dgvExcelPreview.ReadOnly = true;
            this.dgvExcelPreview.RowHeadersVisible = false;
            this.dgvExcelPreview.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvExcelPreview.Size = new System.Drawing.Size(1082, 285);
            this.dgvExcelPreview.TabIndex = 15;
            this.dgvExcelPreview.Visible = false;
            // 
            // btnConfirmImport
            // 
            this.btnConfirmImport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnConfirmImport.Enabled = false;
            this.btnConfirmImport.Location = new System.Drawing.Point(429, 506);
            this.btnConfirmImport.Name = "btnConfirmImport";
            this.btnConfirmImport.Size = new System.Drawing.Size(134, 44);
            this.btnConfirmImport.TabIndex = 16;
            this.btnConfirmImport.Text = "تأكيد الاستيراد";
            this.btnConfirmImport.UseVisualStyleBackColor = true;
            // 
            // lblMessage
            // 
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Location = new System.Drawing.Point(15, 503);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(408, 50);
            this.lblMessage.TabIndex = 17;
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lstSubscriberSuggestions
            // 
            this.lstSubscriberSuggestions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstSubscriberSuggestions.IntegralHeight = false;
            this.lstSubscriberSuggestions.Location = new System.Drawing.Point(0, 0);
            this.lstSubscriberSuggestions.Name = "lstSubscriberSuggestions";
            this.lstSubscriberSuggestions.Size = new System.Drawing.Size(120, 170);
            this.lstSubscriberSuggestions.TabIndex = 1;
            this.lstSubscriberSuggestions.Visible = false;
            // 
            // ReadingF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1180, 700);
            this.Controls.Add(this.mainLayout);
            this.Controls.Add(this.lstSubscriberSuggestions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ReadingF";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "إدخال قراءة عداد";
            this.mainLayout.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.group.ResumeLayout(false);
            this.table.ResumeLayout(false);
            this.table.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExcelPreview)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
/*namespace water3.Forms
{
    partial class ReadingF
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel mainLayout;
        private System.Windows.Forms.FlowLayoutPanel topPanel;

        private System.Windows.Forms.Label lblTariff;
        private System.Windows.Forms.Label lblLastInvoice;
        private System.Windows.Forms.Label lblLastReading;

        private System.Windows.Forms.GroupBox group;
        private System.Windows.Forms.TableLayoutPanel table;

        private System.Windows.Forms.Label lblSubTitle;
        private System.Windows.Forms.TextBox txtSubscriberSearch;

        private System.Windows.Forms.Label lblDateTitle;
        private System.Windows.Forms.DateTimePicker dtpReadingDate;

        private System.Windows.Forms.Label lblPrevTitle;
        private System.Windows.Forms.TextBox txtPreviousReading;

        private System.Windows.Forms.Label lblCurTitle;
        private System.Windows.Forms.TextBox txtCurrentReading;

        private System.Windows.Forms.Label lblConsTitle;
        private System.Windows.Forms.TextBox txtConsumption;

        private System.Windows.Forms.Label lblNotesTitle;
        private System.Windows.Forms.TextBox txtNotes;

        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Label lblMessage;

        private System.Windows.Forms.ListBox lstSubscriberSuggestions;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.topPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblTariff = new System.Windows.Forms.Label();
            this.lblLastInvoice = new System.Windows.Forms.Label();
            this.lblLastReading = new System.Windows.Forms.Label();
            this.group = new System.Windows.Forms.GroupBox();
            this.table = new System.Windows.Forms.TableLayoutPanel();
            this.lblSubTitle = new System.Windows.Forms.Label();
            this.txtSubscriberSearch = new System.Windows.Forms.TextBox();
            this.lblDateTitle = new System.Windows.Forms.Label();
            this.dtpReadingDate = new System.Windows.Forms.DateTimePicker();
            this.lblPrevTitle = new System.Windows.Forms.Label();
            this.txtPreviousReading = new System.Windows.Forms.TextBox();
            this.lblCurTitle = new System.Windows.Forms.Label();
            this.txtCurrentReading = new System.Windows.Forms.TextBox();
            this.lblConsTitle = new System.Windows.Forms.Label();
            this.txtConsumption = new System.Windows.Forms.TextBox();
            this.lblNotesTitle = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.lstSubscriberSuggestions = new System.Windows.Forms.ListBox();
            this.mainLayout.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.group.SuspendLayout();
            this.table.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.BackColor = System.Drawing.Color.White;
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.topPanel, 0, 0);
            this.mainLayout.Controls.Add(this.group, 0, 1);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new System.Windows.Forms.Padding(18, 14, 18, 14);
            this.mainLayout.RowCount = 3;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 320F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.Size = new System.Drawing.Size(1093, 553);
            this.mainLayout.TabIndex = 0;
            // 
            // topPanel
            // 
            this.topPanel.AutoScroll = true;
            this.topPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(242)))), ((int)(((byte)(247)))));
            this.topPanel.Controls.Add(this.lblTariff);
            this.topPanel.Controls.Add(this.lblLastInvoice);
            this.topPanel.Controls.Add(this.lblLastReading);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.topPanel.Location = new System.Drawing.Point(21, 17);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(10);
            this.topPanel.Size = new System.Drawing.Size(1051, 64);
            this.topPanel.TabIndex = 0;
            this.topPanel.WrapContents = false;
            // 
            // lblTariff
            // 
            this.lblTariff.AutoSize = true;
            this.lblTariff.Location = new System.Drawing.Point(10, 16);
            this.lblTariff.Margin = new System.Windows.Forms.Padding(28, 6, 0, 0);
            this.lblTariff.Name = "lblTariff";
            this.lblTariff.Size = new System.Drawing.Size(54, 13);
            this.lblTariff.TabIndex = 0;
            this.lblTariff.Text = "التعرفة: ---";
            // 
            // lblLastInvoice
            // 
            this.lblLastInvoice.AutoSize = true;
            this.lblLastInvoice.Location = new System.Drawing.Point(92, 16);
            this.lblLastInvoice.Margin = new System.Windows.Forms.Padding(28, 6, 0, 0);
            this.lblLastInvoice.Name = "lblLastInvoice";
            this.lblLastInvoice.Size = new System.Drawing.Size(72, 13);
            this.lblLastInvoice.TabIndex = 1;
            this.lblLastInvoice.Text = "آخر فاتورة: ---";
            // 
            // lblLastReading
            // 
            this.lblLastReading.AutoSize = true;
            this.lblLastReading.Location = new System.Drawing.Point(192, 16);
            this.lblLastReading.Margin = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.lblLastReading.Name = "lblLastReading";
            this.lblLastReading.Size = new System.Drawing.Size(67, 13);
            this.lblLastReading.TabIndex = 2;
            this.lblLastReading.Text = "آخر قراءة: ---";
            // 
            // group
            // 
            this.group.BackColor = System.Drawing.Color.WhiteSmoke;
            this.group.Controls.Add(this.table);
            this.group.Dock = System.Windows.Forms.DockStyle.Fill;
            this.group.Location = new System.Drawing.Point(21, 87);
            this.group.Name = "group";
            this.group.Padding = new System.Windows.Forms.Padding(13, 10, 13, 7);
            this.group.Size = new System.Drawing.Size(1051, 314);
            this.group.TabIndex = 1;
            this.group.TabStop = false;
            this.group.Text = "إضافة قراءة جديدة";
            // 
            // table
            // 
            this.table.BackColor = System.Drawing.Color.WhiteSmoke;
            this.table.ColumnCount = 4;
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table.Controls.Add(this.lblSubTitle, 0, 0);
            this.table.Controls.Add(this.txtSubscriberSearch, 1, 0);
            this.table.Controls.Add(this.lblDateTitle, 2, 0);
            this.table.Controls.Add(this.dtpReadingDate, 3, 0);
            this.table.Controls.Add(this.lblPrevTitle, 0, 1);
            this.table.Controls.Add(this.txtPreviousReading, 1, 1);
            this.table.Controls.Add(this.lblCurTitle, 2, 1);
            this.table.Controls.Add(this.txtCurrentReading, 3, 1);
            this.table.Controls.Add(this.lblConsTitle, 0, 2);
            this.table.Controls.Add(this.txtConsumption, 1, 2);
            this.table.Controls.Add(this.lblNotesTitle, 2, 2);
            this.table.Controls.Add(this.txtNotes, 3, 2);
            this.table.Controls.Add(this.btnAdd, 1, 3);
            this.table.Controls.Add(this.lblMessage, 3, 3);
            this.table.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table.Location = new System.Drawing.Point(13, 23);
            this.table.Name = "table";
            this.table.Padding = new System.Windows.Forms.Padding(12);
            this.table.RowCount = 4;
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.table.Size = new System.Drawing.Size(1025, 284);
            this.table.TabIndex = 0;
            // 
            // lblSubTitle
            // 
            this.lblSubTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblSubTitle.AutoSize = true;
            this.lblSubTitle.Location = new System.Drawing.Point(896, 27);
            this.lblSubTitle.Name = "lblSubTitle";
            this.lblSubTitle.Size = new System.Drawing.Size(77, 13);
            this.lblSubTitle.TabIndex = 0;
            this.lblSubTitle.Text = "المشترك/العداد:";
            // 
            // txtSubscriberSearch
            // 
            this.txtSubscriberSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSubscriberSearch.Location = new System.Drawing.Point(516, 15);
            this.txtSubscriberSearch.Name = "txtSubscriberSearch";
            this.txtSubscriberSearch.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtSubscriberSearch.Size = new System.Drawing.Size(374, 20);
            this.txtSubscriberSearch.TabIndex = 1;
            // 
            // lblDateTitle
            // 
            this.lblDateTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDateTitle.AutoSize = true;
            this.lblDateTitle.Location = new System.Drawing.Point(396, 27);
            this.lblDateTitle.Name = "lblDateTitle";
            this.lblDateTitle.Size = new System.Drawing.Size(69, 13);
            this.lblDateTitle.TabIndex = 2;
            this.lblDateTitle.Text = "تاريخ القراءة:";
            // 
            // dtpReadingDate
            // 
            this.dtpReadingDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dtpReadingDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpReadingDate.Location = new System.Drawing.Point(15, 15);
            this.dtpReadingDate.Name = "dtpReadingDate";
            this.dtpReadingDate.Size = new System.Drawing.Size(375, 20);
            this.dtpReadingDate.TabIndex = 3;
            // 
            // lblPrevTitle
            // 
            this.lblPrevTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPrevTitle.AutoSize = true;
            this.lblPrevTitle.Location = new System.Drawing.Point(896, 71);
            this.lblPrevTitle.Name = "lblPrevTitle";
            this.lblPrevTitle.Size = new System.Drawing.Size(77, 13);
            this.lblPrevTitle.TabIndex = 4;
            this.lblPrevTitle.Text = "القراءة السابقة:";
            // 
            // txtPreviousReading
            // 
            this.txtPreviousReading.BackColor = System.Drawing.Color.Gainsboro;
            this.txtPreviousReading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPreviousReading.Location = new System.Drawing.Point(516, 59);
            this.txtPreviousReading.Name = "txtPreviousReading";
            this.txtPreviousReading.ReadOnly = true;
            this.txtPreviousReading.Size = new System.Drawing.Size(374, 20);
            this.txtPreviousReading.TabIndex = 5;
            this.txtPreviousReading.Text = "0";
            this.txtPreviousReading.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblCurTitle
            // 
            this.lblCurTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblCurTitle.AutoSize = true;
            this.lblCurTitle.Location = new System.Drawing.Point(396, 71);
            this.lblCurTitle.Name = "lblCurTitle";
            this.lblCurTitle.Size = new System.Drawing.Size(73, 13);
            this.lblCurTitle.TabIndex = 6;
            this.lblCurTitle.Text = "القراءة الحالية:";
            // 
            // txtCurrentReading
            // 
            this.txtCurrentReading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCurrentReading.Location = new System.Drawing.Point(15, 59);
            this.txtCurrentReading.Name = "txtCurrentReading";
            this.txtCurrentReading.Size = new System.Drawing.Size(375, 20);
            this.txtCurrentReading.TabIndex = 7;
            this.txtCurrentReading.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblConsTitle
            // 
            this.lblConsTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblConsTitle.AutoSize = true;
            this.lblConsTitle.Location = new System.Drawing.Point(896, 115);
            this.lblConsTitle.Name = "lblConsTitle";
            this.lblConsTitle.Size = new System.Drawing.Size(53, 13);
            this.lblConsTitle.TabIndex = 8;
            this.lblConsTitle.Text = "الاستهلاك:";
            // 
            // txtConsumption
            // 
            this.txtConsumption.BackColor = System.Drawing.Color.Gainsboro;
            this.txtConsumption.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConsumption.Location = new System.Drawing.Point(516, 103);
            this.txtConsumption.Name = "txtConsumption";
            this.txtConsumption.ReadOnly = true;
            this.txtConsumption.Size = new System.Drawing.Size(374, 20);
            this.txtConsumption.TabIndex = 9;
            this.txtConsumption.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblNotesTitle
            // 
            this.lblNotesTitle.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblNotesTitle.AutoSize = true;
            this.lblNotesTitle.Location = new System.Drawing.Point(396, 115);
            this.lblNotesTitle.Name = "lblNotesTitle";
            this.lblNotesTitle.Size = new System.Drawing.Size(48, 13);
            this.lblNotesTitle.TabIndex = 10;
            this.lblNotesTitle.Text = "ملاحظات:";
            // 
            // txtNotes
            // 
            this.txtNotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNotes.Location = new System.Drawing.Point(15, 103);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(375, 48);
            this.txtNotes.TabIndex = 11;
            // 
            // btnAdd
            // 
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnAdd.Enabled = false;
            this.btnAdd.Location = new System.Drawing.Point(516, 163);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(374, 106);
            this.btnAdd.TabIndex = 12;
            this.btnAdd.Text = "إضافة";
            // 
            // lblMessage
            // 
            this.lblMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMessage.Location = new System.Drawing.Point(15, 160);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(375, 112);
            this.lblMessage.TabIndex = 13;
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lstSubscriberSuggestions
            // 
            this.lstSubscriberSuggestions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstSubscriberSuggestions.IntegralHeight = false;
            this.lstSubscriberSuggestions.Location = new System.Drawing.Point(0, 0);
            this.lstSubscriberSuggestions.Name = "lstSubscriberSuggestions";
            this.lstSubscriberSuggestions.Size = new System.Drawing.Size(120, 170);
            this.lstSubscriberSuggestions.TabIndex = 1;
            this.lstSubscriberSuggestions.Visible = false;
            // 
            // ReadingF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1093, 553);
            this.Controls.Add(this.mainLayout);
            this.Controls.Add(this.lstSubscriberSuggestions);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ReadingF";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "إدخال قراءة عداد";
            this.mainLayout.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.group.ResumeLayout(false);
            this.table.ResumeLayout(false);
            this.table.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}*/
