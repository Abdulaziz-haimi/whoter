using System.Drawing;
using System.Windows.Forms;

namespace water3
{
    partial class FrmMobileReceiptDetails
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel mainLayout;
        private Label lblHeader;
        private TextBox txtInfo;
        private DataGridView dgvLines;
        private Button btnClose;

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
            this.lblHeader = new Label();
            this.txtInfo = new TextBox();
            this.dgvLines = new DataGridView();
            this.btnClose = new Button();
            this.mainLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLines)).BeginInit();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.mainLayout.Controls.Add(this.lblHeader, 0, 0);
            this.mainLayout.Controls.Add(this.txtInfo, 0, 1);
            this.mainLayout.Controls.Add(this.dgvLines, 0, 2);
            this.mainLayout.Controls.Add(this.btnClose, 0, 3);
            this.mainLayout.Dock = DockStyle.Fill;
            this.mainLayout.Location = new Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new Padding(10);
            this.mainLayout.RowCount = 4;
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            this.mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
            this.mainLayout.Size = new Size(980, 620);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Dock = DockStyle.Fill;
            this.lblHeader.Font = new Font("Tahoma", 11F, FontStyle.Bold);
            this.lblHeader.Text = "تفاصيل السجل";
            this.lblHeader.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtInfo
            // 
            this.txtInfo.Dock = DockStyle.Fill;
            this.txtInfo.Multiline = true;
            this.txtInfo.ReadOnly = true;
            this.txtInfo.ScrollBars = ScrollBars.Vertical;
            this.txtInfo.BackColor = Color.White;
            // 
            // dgvLines
            // 
            this.dgvLines.Dock = DockStyle.Fill;
            this.dgvLines.BackgroundColor = Color.White;
            this.dgvLines.ReadOnly = true;
            this.dgvLines.AllowUserToAddRows = false;
            this.dgvLines.AllowUserToDeleteRows = false;
            this.dgvLines.RowHeadersVisible = false;
            this.dgvLines.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = AnchorStyles.Left;
            this.btnClose.Location = new Point(20, 8);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(100, 30);
            this.btnClose.Text = "إغلاق";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // FrmMobileReceiptDetails
            // 
            this.AutoScaleDimensions = new SizeF(8F, 17F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(980, 620);
            this.Controls.Add(this.mainLayout);
            this.Font = new Font("Tahoma", 10F);
            this.Name = "FrmMobileReceiptDetails";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "تفاصيل التحصيل المستورد";
            this.mainLayout.ResumeLayout(false);
            this.mainLayout.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLines)).EndInit();
            this.ResumeLayout(false);
        }
    }
}