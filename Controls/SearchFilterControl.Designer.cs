using System.Windows.Forms;
namespace water3.Controls
{

        partial class SearchFilterControl
        {
            private System.ComponentModel.IContainer components = null;

            private Panel pnlCard;
            private FlowLayoutPanel pnlSearch;
            private TextBox txtSearch;
            private Button btnSearch;
            private DateTimePicker dateFilter;
            private Button btnFilterDate;

            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                    components.Dispose();

                base.Dispose(disposing);
            }

            private void InitializeComponent()
            {
            this.pnlCard = new System.Windows.Forms.Panel();
            this.pnlSearch = new System.Windows.Forms.FlowLayoutPanel();
            this.btnFilterDate = new System.Windows.Forms.Button();
            this.dateFilter = new System.Windows.Forms.DateTimePicker();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pnlCard.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlCard
            // 
            this.pnlCard.Controls.Add(this.pnlSearch);
            this.pnlCard.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCard.Location = new System.Drawing.Point(0, 0);
            this.pnlCard.Name = "pnlCard";
            this.pnlCard.Size = new System.Drawing.Size(1180, 114);
            this.pnlCard.TabIndex = 0;
            // 
            // pnlSearch
            // 
            this.pnlSearch.Controls.Add(this.btnFilterDate);
            this.pnlSearch.Controls.Add(this.dateFilter);
            this.pnlSearch.Controls.Add(this.btnSearch);
            this.pnlSearch.Controls.Add(this.txtSearch);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearch.Location = new System.Drawing.Point(0, 0);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Padding = new System.Windows.Forms.Padding(0, 52, 0, 0);
            this.pnlSearch.Size = new System.Drawing.Size(1180, 114);
            this.pnlSearch.TabIndex = 0;
            // 
            // btnFilterDate
            // 
            this.btnFilterDate.Location = new System.Drawing.Point(3, 55);
            this.btnFilterDate.Name = "btnFilterDate";
            this.btnFilterDate.Size = new System.Drawing.Size(42, 42);
            this.btnFilterDate.TabIndex = 0;
            this.btnFilterDate.UseVisualStyleBackColor = true;
            // 
            // dateFilter
            // 
            this.dateFilter.Location = new System.Drawing.Point(51, 55);
            this.dateFilter.Name = "dateFilter";
            this.dateFilter.Size = new System.Drawing.Size(200, 20);
            this.dateFilter.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(257, 55);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(42, 42);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // txtSearch
            // 
            this.txtSearch.Location = new System.Drawing.Point(305, 55);
            this.txtSearch.Multiline = true;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(340, 38);
            this.txtSearch.TabIndex = 3;
            // 
            // SearchFilterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.pnlCard);
            this.Name = "SearchFilterControl";
            this.Size = new System.Drawing.Size(1180, 114);
            this.pnlCard.ResumeLayout(false);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.ResumeLayout(false);

            }
        }
    }