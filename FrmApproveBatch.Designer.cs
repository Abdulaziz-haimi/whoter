using System.Drawing;
using System.Windows.Forms;

namespace water3
{
    partial class FrmApproveBatch
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel mainLayout;
        private Panel pnlTop;
        private Label lblSyncBatchID;
        private TextBox txtSyncBatchID;
        private CheckBox chkUseDate;
        private DateTimePicker dtFrom;
        private DateTimePicker dtTo;
        private Label lblLimit;
        private NumericUpDown numLimit;
        private CheckBox chkSendSms;
        private Button btnRun;
        private TextBox txtResult;
        private DataGridView dgvResult;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mainLayout = new TableLayoutPanel();
            this.pnlTop = new Panel();
            this.lblSyncBatchID = new Label();
            this.txtSyncBatchID = new TextBox();
            this.chkUseDate = new CheckBox();
            this.dtFrom = new DateTimePicker();
            this.dtTo = new DateTimePicker();
            this.lblLimit = new Label();
            this.numLimit = new NumericUpDown();
            this.chkSendSms = new CheckBox();
            this.btnRun = new Button();
            this.txtResult = new TextBox();
            this.dgvResult = new DataGridView();
            this.mainLayout.SuspendLayout();
            this.pnlTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).BeginInit();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.pnlTop, 0, 0);
            this.mainLayout.Controls.Add(this.txtResult, 0, 1);
            this.mainLayout.Controls.Add(this.dgvResult, 0, 2);
            this.mainLayout.Dock = DockStyle.Fill;
            this.mainLayout.Location = new Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new Padding(10);
            this.mainLayout.RowCount = 3;
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.mainLayout.Size = new Size(980, 620);
            // 
            // pnlTop
            // 
            this.pnlTop.BackColor = Color.WhiteSmoke;
            this.pnlTop.Controls.Add(this.lblSyncBatchID);
            this.pnlTop.Controls.Add(this.txtSyncBatchID);
            this.pnlTop.Controls.Add(this.chkUseDate);
            this.pnlTop.Controls.Add(this.dtFrom);
            this.pnlTop.Controls.Add(this.dtTo);
            this.pnlTop.Controls.Add(this.lblLimit);
            this.pnlTop.Controls.Add(this.numLimit);
            this.pnlTop.Controls.Add(this.chkSendSms);
            this.pnlTop.Controls.Add(this.btnRun);
            this.pnlTop.Dock = DockStyle.Fill;
            // 
            // lblSyncBatchID
            // 
            this.lblSyncBatchID.AutoSize = true;
            this.lblSyncBatchID.Location = new Point(850, 18);
            this.lblSyncBatchID.Text = "رقم الدفعة:";
            // 
            // txtSyncBatchID
            // 
            this.txtSyncBatchID.Location = new Point(720, 15);
            this.txtSyncBatchID.Size = new Size(120, 24);
            // 
            // chkUseDate
            // 
            this.chkUseDate.AutoSize = true;
            this.chkUseDate.Location = new Point(650, 17);
            this.chkUseDate.Text = "فترة";
            // 
            // dtFrom
            // 
            this.dtFrom.Format = DateTimePickerFormat.Short;
            this.dtFrom.Location = new Point(510, 15);
            this.dtFrom.Size = new Size(130, 24);
            // 
            // dtTo
            // 
            this.dtTo.Format = DateTimePickerFormat.Short;
            this.dtTo.Location = new Point(370, 15);
            this.dtTo.Size = new Size(130, 24);
            // 
            // lblLimit
            // 
            this.lblLimit.AutoSize = true;
            this.lblLimit.Location = new Point(300, 18);
            this.lblLimit.Text = "الحد الأقصى:";
            // 
            // numLimit
            // 
            this.numLimit.Location = new Point(200, 15);
            this.numLimit.Minimum = 1;
            this.numLimit.Maximum = 100000;
            this.numLimit.Value = 100;
            this.numLimit.Size = new Size(90, 24);
            // 
            // chkSendSms
            // 
            this.chkSendSms.AutoSize = true;
            this.chkSendSms.Location = new Point(90, 17);
            this.chkSendSms.Text = "إرسال SMS";
            // 
            // btnRun
            // 
            this.btnRun.Location = new Point(20, 12);
            this.btnRun.Size = new Size(60, 30);
            this.btnRun.Text = "تنفيذ";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // txtResult
            // 
            this.txtResult.Dock = DockStyle.Fill;
            this.txtResult.Multiline = true;
            this.txtResult.ReadOnly = true;
            this.txtResult.BackColor = Color.White;
            // 
            // dgvResult
            // 
            this.dgvResult.Dock = DockStyle.Fill;
            this.dgvResult.BackgroundColor = Color.White;
            this.dgvResult.ReadOnly = true;
            this.dgvResult.AllowUserToAddRows = false;
            this.dgvResult.AllowUserToDeleteRows = false;
            this.dgvResult.RowHeadersVisible = false;
            this.dgvResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // 
            // FrmApproveBatch
            // 
            this.AutoScaleDimensions = new SizeF(8F, 17F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(980, 620);
            this.Controls.Add(this.mainLayout);
            this.Font = new Font("Tahoma", 10F);
            this.Name = "FrmApproveBatch";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "الاعتماد الجماعي";
            this.mainLayout.ResumeLayout(false);
            this.mainLayout.PerformLayout();
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvResult)).EndInit();
            this.ResumeLayout(false);
        }
    }
}