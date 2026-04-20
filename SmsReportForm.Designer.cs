namespace water3
{
    partial class SmsReportForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.FlowLayoutPanel panelTop;

        private System.Windows.Forms.Label lblFrom;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.Label lblStatusTitle;
        private System.Windows.Forms.Label lblCollectorTitle;

        private System.Windows.Forms.DateTimePicker dtFrom;
        private System.Windows.Forms.DateTimePicker dtTo;

        private System.Windows.Forms.ComboBox ddlStatus;
        private System.Windows.Forms.ComboBox ddlCollectors;

        private System.Windows.Forms.Button btnLoad;

        private System.Windows.Forms.Label lblSent;
        private System.Windows.Forms.Label lblFailed;
        private System.Windows.Forms.Label lblSkipped;
        private System.Windows.Forms.Label lblTotalAmount;

        private System.Windows.Forms.DataGridView dgv;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.panelTop = new System.Windows.Forms.FlowLayoutPanel();

            this.lblFrom = new System.Windows.Forms.Label();
            this.lblTo = new System.Windows.Forms.Label();
            this.lblStatusTitle = new System.Windows.Forms.Label();
            this.lblCollectorTitle = new System.Windows.Forms.Label();

            this.dtFrom = new System.Windows.Forms.DateTimePicker();
            this.dtTo = new System.Windows.Forms.DateTimePicker();

            this.ddlStatus = new System.Windows.Forms.ComboBox();
            this.ddlCollectors = new System.Windows.Forms.ComboBox();

            this.btnLoad = new System.Windows.Forms.Button();

            this.lblSent = new System.Windows.Forms.Label();
            this.lblFailed = new System.Windows.Forms.Label();
            this.lblSkipped = new System.Windows.Forms.Label();
            this.lblTotalAmount = new System.Windows.Forms.Label();

            this.dgv = new System.Windows.Forms.DataGridView();

            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();

            // =========================
            // panelTop
            // =========================
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Height = 90;
            this.panelTop.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.panelTop.Padding = new System.Windows.Forms.Padding(10);
            this.panelTop.WrapContents = false;
            this.panelTop.AutoScroll = true;

            // labels fixed size better in flow layout
            this.lblFrom.AutoSize = true;
            this.lblFrom.Text = "من";

            this.lblTo.AutoSize = true;
            this.lblTo.Text = "إلى";

            this.lblStatusTitle.AutoSize = true;
            this.lblStatusTitle.Text = "الحالة";

            this.lblCollectorTitle.AutoSize = true;
            this.lblCollectorTitle.Text = "المحصل";

            // =========================
            // dtFrom / dtTo
            // =========================
            this.dtFrom.Width = 120;
            this.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;

            this.dtTo.Width = 120;
            this.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Short;

            // =========================
            // ddlStatus
            // =========================
            this.ddlStatus.Width = 120;
            this.ddlStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // =========================
            // ddlCollectors
            // =========================
            this.ddlCollectors.Width = 150;
            this.ddlCollectors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;

            // =========================
            // btnLoad
            // =========================
            this.btnLoad.Text = "عرض";
            this.btnLoad.Width = 100;

            // =========================
            // Counters labels
            // =========================
            this.lblSent.AutoSize = true;

            this.lblFailed.AutoSize = true;

            this.lblSkipped.AutoSize = true;

            this.lblTotalAmount.AutoSize = true;

            // add controls to top panel (RightToLeft)
            this.panelTop.Controls.Add(this.lblTotalAmount);
            this.panelTop.Controls.Add(this.lblSkipped);
            this.panelTop.Controls.Add(this.lblFailed);
            this.panelTop.Controls.Add(this.lblSent);

            this.panelTop.Controls.Add(this.btnLoad);

            this.panelTop.Controls.Add(this.ddlCollectors);
            this.panelTop.Controls.Add(this.lblCollectorTitle);

            this.panelTop.Controls.Add(this.ddlStatus);
            this.panelTop.Controls.Add(this.lblStatusTitle);

            this.panelTop.Controls.Add(this.dtTo);
            this.panelTop.Controls.Add(this.lblTo);

            this.panelTop.Controls.Add(this.dtFrom);
            this.panelTop.Controls.Add(this.lblFrom);

            // =========================
            // dgv
            // =========================
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.ReadOnly = true;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.RowHeadersVisible = false;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;

            // =========================
            // SmsReportForm
            // =========================
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 700);
            this.Name = "SmsReportForm";
            this.Text = "SmsReportForm";

            this.Controls.Add(this.dgv);
            this.Controls.Add(this.panelTop);

            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
