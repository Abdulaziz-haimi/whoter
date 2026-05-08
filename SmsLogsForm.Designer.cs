namespace water3
{
    partial class SmsLogsForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel rootLayout;
        private System.Windows.Forms.Panel filterPanel;
        private System.Windows.Forms.TableLayoutPanel filterMainLayout;
        private System.Windows.Forms.Panel titlePanel;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.TableLayoutPanel filtersLayout;
        private System.Windows.Forms.FlowLayoutPanel buttonsPanel;

        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cmbStatusFilter;

        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.DateTimePicker dateFromPicker;

        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.DateTimePicker dateToPicker;

        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.TextBox txtPhoneFilter;

        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearchGeneral;

        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnResend;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnInvoiceDetails;

        private System.Windows.Forms.Panel gridPanel;
        private System.Windows.Forms.DataGridView smsLogsDataGridView;

        private System.Windows.Forms.Panel statsPanel;
        private System.Windows.Forms.Label statsLabel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.rootLayout = new System.Windows.Forms.TableLayoutPanel();
            this.filterPanel = new System.Windows.Forms.Panel();
            this.filterMainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.titlePanel = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.filtersLayout = new System.Windows.Forms.TableLayoutPanel();
            this.buttonsPanel = new System.Windows.Forms.FlowLayoutPanel();

            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbStatusFilter = new System.Windows.Forms.ComboBox();

            this.lblFrom = new System.Windows.Forms.Label();
            this.dateFromPicker = new System.Windows.Forms.DateTimePicker();

            this.lblTo = new System.Windows.Forms.Label();
            this.dateToPicker = new System.Windows.Forms.DateTimePicker();

            this.lblPhone = new System.Windows.Forms.Label();
            this.txtPhoneFilter = new System.Windows.Forms.TextBox();

            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearchGeneral = new System.Windows.Forms.TextBox();

            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnResend = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnInvoiceDetails = new System.Windows.Forms.Button();

            this.gridPanel = new System.Windows.Forms.Panel();
            this.smsLogsDataGridView = new System.Windows.Forms.DataGridView();

            this.statsPanel = new System.Windows.Forms.Panel();
            this.statsLabel = new System.Windows.Forms.Label();

            this.rootLayout.SuspendLayout();
            this.filterPanel.SuspendLayout();
            this.filterMainLayout.SuspendLayout();
            this.titlePanel.SuspendLayout();
            this.filtersLayout.SuspendLayout();
            this.buttonsPanel.SuspendLayout();
            this.gridPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.smsLogsDataGridView)).BeginInit();
            this.statsPanel.SuspendLayout();
            this.SuspendLayout();

            // 
            // Form
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.MinimumSize = new System.Drawing.Size(1000, 620);
            this.Name = "SmsLogsForm";
            this.Text = "سجلات الرسائل النصية";
            this.Load += new System.EventHandler(this.SmsLogsForm_Load);

            // 
            // rootLayout
            // 
            this.rootLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootLayout.ColumnCount = 1;
            this.rootLayout.RowCount = 3;
            this.rootLayout.Padding = new System.Windows.Forms.Padding(12);
            this.rootLayout.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.rootLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 132F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rootLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 42F));

            // 
            // filterPanel
            // 
            this.filterPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterPanel.Padding = new System.Windows.Forms.Padding(12);
            this.filterPanel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 10);
            this.filterPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // 
            // filterMainLayout
            // 
            this.filterMainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filterMainLayout.ColumnCount = 1;
            this.filterMainLayout.RowCount = 2;
            this.filterMainLayout.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.filterMainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 44F));
            this.filterMainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filterMainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

            // 
            // titlePanel
            // 
            this.titlePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.titlePanel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.titlePanel.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);

            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblTitle.Width = 280;
            this.lblTitle.Text = "سجلات الرسائل النصية";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 13F, System.Drawing.FontStyle.Bold);

            // 
            // lblSubtitle
            // 
            this.lblSubtitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSubtitle.Text = "متابعة الرسائل المرسلة والفاشلة والمعلقة مع إمكانية إعادة الإرسال والتصدير";
            this.lblSubtitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblSubtitle.Font = new System.Drawing.Font("Tahoma", 8.5F, System.Drawing.FontStyle.Regular);

            this.titlePanel.Controls.Add(this.lblSubtitle);
            this.titlePanel.Controls.Add(this.lblTitle);

            // 
            // filtersLayout
            // 
            this.filtersLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.filtersLayout.ColumnCount = 11;
            this.filtersLayout.RowCount = 1;
            this.filtersLayout.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.filtersLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));

            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 135F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.filtersLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 500F));

            // 
            // cmbStatusFilter
            // 
            this.cmbStatusFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatusFilter.Margin = new System.Windows.Forms.Padding(4, 12, 4, 8);
            this.cmbStatusFilter.RightToLeft = System.Windows.Forms.RightToLeft.Yes;

            // 
            // lblStatus
            // 
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Text = "الحالة:";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblStatus.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);

            // 
            // dateFromPicker
            // 
            this.dateFromPicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dateFromPicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateFromPicker.Margin = new System.Windows.Forms.Padding(4, 12, 4, 8);
            this.dateFromPicker.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dateFromPicker.RightToLeftLayout = true;

            // 
            // lblFrom
            // 
            this.lblFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFrom.Text = "من تاريخ:";
            this.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblFrom.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);

            // 
            // dateToPicker
            // 
            this.dateToPicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dateToPicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateToPicker.Margin = new System.Windows.Forms.Padding(4, 12, 4, 8);
            this.dateToPicker.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dateToPicker.RightToLeftLayout = true;

            // 
            // lblTo
            // 
            this.lblTo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTo.Text = "إلى تاريخ:";
            this.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTo.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);

            // 
            // txtPhoneFilter
            // 
            this.txtPhoneFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPhoneFilter.Margin = new System.Windows.Forms.Padding(4, 12, 4, 8);
            this.txtPhoneFilter.RightToLeft = System.Windows.Forms.RightToLeft.Yes;

            // 
            // lblPhone
            // 
            this.lblPhone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPhone.Text = "الهاتف:";
            this.lblPhone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblPhone.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);

            // 
            // txtSearchGeneral
            // 
            this.txtSearchGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSearchGeneral.Margin = new System.Windows.Forms.Padding(4, 12, 4, 8);
            this.txtSearchGeneral.RightToLeft = System.Windows.Forms.RightToLeft.Yes;

            // 
            // lblSearch
            // 
            this.lblSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSearch.Text = "بحث:";
            this.lblSearch.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSearch.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);

            // 
            // buttonsPanel
            // 
            this.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonsPanel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.buttonsPanel.WrapContents = false;
            this.buttonsPanel.AutoScroll = false;
            this.buttonsPanel.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.buttonsPanel.Margin = System.Windows.Forms.Padding.Empty;

            // 
            // buttons
            // 
            SetupButton(this.btnRefresh, "تحديث", 88);
            SetupButton(this.btnReset, "إعادة تعيين", 105);
            SetupButton(this.btnResend, "إعادة إرسال", 105);
            SetupButton(this.btnExport, "تصدير CSV", 105);
            SetupButton(this.btnInvoiceDetails, "تفاصيل الفاتورة", 125);

            this.buttonsPanel.Controls.Add(this.btnInvoiceDetails);
            this.buttonsPanel.Controls.Add(this.btnExport);
            this.buttonsPanel.Controls.Add(this.btnResend);
            this.buttonsPanel.Controls.Add(this.btnReset);
            this.buttonsPanel.Controls.Add(this.btnRefresh);

            this.filtersLayout.Controls.Add(this.cmbStatusFilter, 0, 0);
            this.filtersLayout.Controls.Add(this.lblStatus, 1, 0);
            this.filtersLayout.Controls.Add(this.dateFromPicker, 2, 0);
            this.filtersLayout.Controls.Add(this.lblFrom, 3, 0);
            this.filtersLayout.Controls.Add(this.dateToPicker, 4, 0);
            this.filtersLayout.Controls.Add(this.lblTo, 5, 0);
            this.filtersLayout.Controls.Add(this.txtPhoneFilter, 6, 0);
            this.filtersLayout.Controls.Add(this.lblPhone, 7, 0);
            this.filtersLayout.Controls.Add(this.txtSearchGeneral, 8, 0);
            this.filtersLayout.Controls.Add(this.lblSearch, 9, 0);
            this.filtersLayout.Controls.Add(this.buttonsPanel, 10, 0);

            this.filterMainLayout.Controls.Add(this.titlePanel, 0, 0);
            this.filterMainLayout.Controls.Add(this.filtersLayout, 0, 1);

            this.filterPanel.Controls.Add(this.filterMainLayout);

            // 
            // gridPanel
            // 
            this.gridPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPanel.Padding = new System.Windows.Forms.Padding(8);
            this.gridPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // 
            // smsLogsDataGridView
            // 
            this.smsLogsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.smsLogsDataGridView.BackgroundColor = System.Drawing.Color.White;
            this.smsLogsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.smsLogsDataGridView.AllowUserToAddRows = false;
            this.smsLogsDataGridView.AllowUserToDeleteRows = false;
            this.smsLogsDataGridView.ReadOnly = true;
            this.smsLogsDataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.smsLogsDataGridView.MultiSelect = true;
            this.smsLogsDataGridView.RowHeadersVisible = false;
            this.smsLogsDataGridView.AutoGenerateColumns = false;
            this.smsLogsDataGridView.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.smsLogsDataGridView.EnableHeadersVisualStyles = false;

            this.gridPanel.Controls.Add(this.smsLogsDataGridView);

            // 
            // statsPanel
            // 
            this.statsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statsPanel.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);

            // 
            // statsLabel
            // 
            this.statsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.statsLabel.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.statsLabel.Text = "الإحصائيات: إجمالي الرسائل: 0 | ناجح: 0 | فاشل: 0 | معلق: 0";

            this.statsPanel.Controls.Add(this.statsLabel);

            this.rootLayout.Controls.Add(this.filterPanel, 0, 0);
            this.rootLayout.Controls.Add(this.gridPanel, 0, 1);
            this.rootLayout.Controls.Add(this.statsPanel, 0, 2);

            this.Controls.Add(this.rootLayout);

            this.rootLayout.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterMainLayout.ResumeLayout(false);
            this.titlePanel.ResumeLayout(false);
            this.filtersLayout.ResumeLayout(false);
            this.filtersLayout.PerformLayout();
            this.buttonsPanel.ResumeLayout(false);
            this.gridPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.smsLogsDataGridView)).EndInit();
            this.statsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void SetupButton(System.Windows.Forms.Button btn, string text, int width)
        {
            btn.Text = text;
            btn.Size = new System.Drawing.Size(width, 34);
            btn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.Cursor = System.Windows.Forms.Cursors.Hand;
            btn.Font = new System.Drawing.Font("Tahoma", 8.8F, System.Drawing.FontStyle.Bold);
            btn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        }
    }
}