namespace water3
{
    partial class InvoiceForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TableLayoutPanel mainLayout;

        private System.Windows.Forms.FlowLayoutPanel searchPanel;
        private System.Windows.Forms.Label lblSearchTitle;
        private System.Windows.Forms.ComboBox ddlSearch;
        private System.Windows.Forms.Button btnLoadDetails;
        private System.Windows.Forms.Button btnReload;

        private System.Windows.Forms.WebBrowser browser;
        private System.Windows.Forms.DataGridView dgv;

        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnPrint;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
            this.searchPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblSearchTitle = new System.Windows.Forms.Label();
            this.ddlSearch = new System.Windows.Forms.ComboBox();
            this.btnLoadDetails = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.browser = new System.Windows.Forms.WebBrowser();
            this.dgv = new System.Windows.Forms.DataGridView();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnPrint = new System.Windows.Forms.Button();
            this.mainLayout.SuspendLayout();
            this.searchPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainLayout
            // 
            this.mainLayout.ColumnCount = 1;
            this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F)); this.mainLayout.Controls.Add(this.searchPanel, 0, 0);
            this.mainLayout.Controls.Add(this.browser, 0, 1);
            this.mainLayout.Controls.Add(this.dgv, 0, 2);
            this.mainLayout.Controls.Add(this.pnlBottom, 0, 3);
            this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainLayout.Location = new System.Drawing.Point(0, 0);
            this.mainLayout.Name = "mainLayout";
            this.mainLayout.Padding = new System.Windows.Forms.Padding(10);
            this.mainLayout.RowCount = 4;
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 260F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.mainLayout.Size = new System.Drawing.Size(1000, 650);
            this.mainLayout.TabIndex = 0;
            // 
            // searchPanel
            // 
            this.searchPanel.Controls.Add(this.lblSearchTitle);
            this.searchPanel.Controls.Add(this.ddlSearch);
            this.searchPanel.Controls.Add(this.btnLoadDetails);
            this.searchPanel.Controls.Add(this.btnReload);
            this.searchPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.searchPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.searchPanel.Location = new System.Drawing.Point(13, 13);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Padding = new System.Windows.Forms.Padding(10);
            this.searchPanel.Size = new System.Drawing.Size(974, 74);
            this.searchPanel.TabIndex = 0;
            this.searchPanel.WrapContents = false;
            // 
            // lblSearchTitle
            // 
            this.lblSearchTitle.AutoSize = true;
            this.lblSearchTitle.Location = new System.Drawing.Point(20, 20);
            this.lblSearchTitle.Margin = new System.Windows.Forms.Padding(10);
            this.lblSearchTitle.Name = "lblSearchTitle";
            this.lblSearchTitle.Size = new System.Drawing.Size(161, 13);
            this.lblSearchTitle.TabIndex = 0;
            this.lblSearchTitle.Text = "بحث: اسم المشترك أو رقم الفاتورة";
            // 
            // ddlSearch
            // 
            this.ddlSearch.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.ddlSearch.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.ddlSearch.Location = new System.Drawing.Point(194, 13);
            this.ddlSearch.Name = "ddlSearch";
            this.ddlSearch.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ddlSearch.Size = new System.Drawing.Size(320, 21);
            this.ddlSearch.TabIndex = 1;
            // 
            // btnLoadDetails
            // 
            this.btnLoadDetails.Location = new System.Drawing.Point(520, 13);
            this.btnLoadDetails.Name = "btnLoadDetails";
            this.btnLoadDetails.Size = new System.Drawing.Size(140, 32);
            this.btnLoadDetails.TabIndex = 2;
            this.btnLoadDetails.Text = "عرض الفاتورة";
            // 
            // btnReload
            // 
            this.btnReload.Location = new System.Drawing.Point(666, 13);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(100, 32);
            this.btnReload.TabIndex = 3;
            this.btnReload.Text = "تحديث";
            // 
            // browser
            // 
            this.browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browser.Location = new System.Drawing.Point(13, 93);
            this.browser.Name = "browser";
            this.browser.ScriptErrorsSuppressed = true;
            this.browser.Size = new System.Drawing.Size(974, 254);
            this.browser.TabIndex = 1;
            // 
            // dgv
            // 
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.Location = new System.Drawing.Point(13, 353);
            this.dgv.Name = "dgv";
            this.dgv.Size = new System.Drawing.Size(974, 224);
            this.dgv.TabIndex = 2;
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnPrint);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBottom.Location = new System.Drawing.Point(13, 583);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(974, 54);
            this.pnlBottom.TabIndex = 3;
            // 
            // btnPrint
            // 
            this.btnPrint.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnPrint.Location = new System.Drawing.Point(777, -13);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(220, 40);
            this.btnPrint.TabIndex = 0;
            this.btnPrint.Text = "طباعة الفاتورة";
            // 
            // InvoiceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 650);
            this.Controls.Add(this.mainLayout);
            this.Name = "InvoiceForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "عرض الفواتير";
            this.mainLayout.ResumeLayout(false);
            this.searchPanel.ResumeLayout(false);
            this.searchPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }
    }
}
