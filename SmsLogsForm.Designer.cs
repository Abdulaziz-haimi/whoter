namespace water3
{
    partial class SmsLogsForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel filterPanel;
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

            this.filterPanel = new System.Windows.Forms.Panel();
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

            this.smsLogsDataGridView = new System.Windows.Forms.DataGridView();

            this.statsPanel = new System.Windows.Forms.Panel();
            this.statsLabel = new System.Windows.Forms.Label();

            this.filterPanel.SuspendLayout();
            this.buttonsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.smsLogsDataGridView)).BeginInit();
            this.statsPanel.SuspendLayout();
            this.SuspendLayout();

            // ================= Form =================
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1200, 700);
            this.Name = "SmsLogsForm";
            this.Text = "SmsLogsForm";
            this.Load += new System.EventHandler(this.SmsLogsForm_Load);

            // ================= filterPanel =================
            this.filterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterPanel.Height = 110;
            this.filterPanel.BackColor = System.Drawing.Color.FromArgb(236, 240, 241);
            this.filterPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            int y1 = 12;
            int y2 = 52;

            // Status
            this.lblStatus.Text = "حالة الرسالة:";
            this.lblStatus.Location = new System.Drawing.Point(20, y1);
            this.lblStatus.Size = new System.Drawing.Size(90, 25);
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.cmbStatusFilter.Location = new System.Drawing.Point(120, y1);
            this.cmbStatusFilter.Size = new System.Drawing.Size(160, 25);
            this.cmbStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // From
            this.lblFrom.Text = "من تاريخ:";
            this.lblFrom.Location = new System.Drawing.Point(300, y1);
            this.lblFrom.Size = new System.Drawing.Size(70, 25);
            this.lblFrom.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.dateFromPicker.Location = new System.Drawing.Point(380, y1);
            this.dateFromPicker.Size = new System.Drawing.Size(130, 25);
            this.dateFromPicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;

            // To
            this.lblTo.Text = "إلى تاريخ:";
            this.lblTo.Location = new System.Drawing.Point(530, y1);
            this.lblTo.Size = new System.Drawing.Size(70, 25);
            this.lblTo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.dateToPicker.Location = new System.Drawing.Point(610, y1);
            this.dateToPicker.Size = new System.Drawing.Size(130, 25);
            this.dateToPicker.Format = System.Windows.Forms.DateTimePickerFormat.Short;

            // Phone
            this.lblPhone.Text = "رقم الهاتف:";
            this.lblPhone.Location = new System.Drawing.Point(20, y2);
            this.lblPhone.Size = new System.Drawing.Size(90, 25);
            this.lblPhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtPhoneFilter.Location = new System.Drawing.Point(120, y2);
            this.txtPhoneFilter.Size = new System.Drawing.Size(160, 25);

            // Search
            this.lblSearch.Text = "بحث عام:";
            this.lblSearch.Location = new System.Drawing.Point(300, y2);
            this.lblSearch.Size = new System.Drawing.Size(70, 25);
            this.lblSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtSearchGeneral.Location = new System.Drawing.Point(380, y2);
            this.txtSearchGeneral.Size = new System.Drawing.Size(360, 25);

            // ================= buttonsPanel =================
            this.buttonsPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonsPanel.Width = 620;
            this.buttonsPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.buttonsPanel.WrapContents = false;
            this.buttonsPanel.Padding = new System.Windows.Forms.Padding(10, 12, 10, 10);
            this.buttonsPanel.BackColor = System.Drawing.Color.Transparent;

            // btnRefresh
            this.btnRefresh.Text = "تحديث";
            this.btnRefresh.Size = new System.Drawing.Size(120, 60);
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);

            // btnReset
            this.btnReset.Text = "إعادة تعيين";
            this.btnReset.Size = new System.Drawing.Size(120, 60);
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(149, 165, 166);
            this.btnReset.ForeColor = System.Drawing.Color.White;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);

            // btnResend
            this.btnResend.Text = "إعادة إرسال";
            this.btnResend.Size = new System.Drawing.Size(120, 60);
            this.btnResend.BackColor = System.Drawing.Color.FromArgb(230, 126, 34);
            this.btnResend.ForeColor = System.Drawing.Color.White;
            this.btnResend.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnResend.FlatAppearance.BorderSize = 0;
            this.btnResend.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);

            // btnExport
            this.btnExport.Text = "تصدير Excel";
            this.btnExport.Size = new System.Drawing.Size(120, 60);
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(39, 174, 96);
            this.btnExport.ForeColor = System.Drawing.Color.White;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExport.FlatAppearance.BorderSize = 0;
            this.btnExport.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);

            // btnInvoiceDetails
            this.btnInvoiceDetails.Text = "تفاصيل الفاتورة";
            this.btnInvoiceDetails.Size = new System.Drawing.Size(120, 60);
            this.btnInvoiceDetails.BackColor = System.Drawing.Color.FromArgb(155, 89, 182);
            this.btnInvoiceDetails.ForeColor = System.Drawing.Color.White;
            this.btnInvoiceDetails.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInvoiceDetails.FlatAppearance.BorderSize = 0;
            this.btnInvoiceDetails.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);

            this.buttonsPanel.Controls.Add(this.btnRefresh);
            this.buttonsPanel.Controls.Add(this.btnReset);
            this.buttonsPanel.Controls.Add(this.btnResend);
            this.buttonsPanel.Controls.Add(this.btnExport);
            this.buttonsPanel.Controls.Add(this.btnInvoiceDetails);

            // Add controls to filterPanel
            this.filterPanel.Controls.Add(this.lblStatus);
            this.filterPanel.Controls.Add(this.cmbStatusFilter);
            this.filterPanel.Controls.Add(this.lblFrom);
            this.filterPanel.Controls.Add(this.dateFromPicker);
            this.filterPanel.Controls.Add(this.lblTo);
            this.filterPanel.Controls.Add(this.dateToPicker);
            this.filterPanel.Controls.Add(this.lblPhone);
            this.filterPanel.Controls.Add(this.txtPhoneFilter);
            this.filterPanel.Controls.Add(this.lblSearch);
            this.filterPanel.Controls.Add(this.txtSearchGeneral);
            this.filterPanel.Controls.Add(this.buttonsPanel);

            // ================= smsLogsDataGridView =================
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

            // ================= statsPanel =================
            this.statsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statsPanel.Height = 40;
            this.statsPanel.BackColor = System.Drawing.Color.FromArgb(52, 73, 94);
            this.statsPanel.ForeColor = System.Drawing.Color.White;

            this.statsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.statsLabel.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold);
            this.statsLabel.Text = "الإحصائيات: إجمالي الرسائل: 0 | ناجح: 0 | فاشل: 0 | معلق: 0";

            this.statsPanel.Controls.Add(this.statsLabel);

            // ================= Add to form =================
            this.Controls.Add(this.smsLogsDataGridView);
            this.Controls.Add(this.statsPanel);
            this.Controls.Add(this.filterPanel);

            this.filterPanel.ResumeLayout(false);
            this.buttonsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.smsLogsDataGridView)).EndInit();
            this.statsPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
