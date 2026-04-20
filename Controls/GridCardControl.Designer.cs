using System.Windows.Forms;
namespace water3.Controls
{
    partial class GridCardControl
    {
  

            private System.ComponentModel.IContainer components = null;

            private Panel pnlCard;
            private Label lblTitle;
            private Label lblSubTitle;
            private Panel pnlSeparator;
            private DataGridView dgvGrid;

            protected override void Dispose(bool disposing)
            {
                if (disposing && (components != null))
                    components.Dispose();

                base.Dispose(disposing);
            }

            private void InitializeComponent()
            {
                this.pnlCard = new System.Windows.Forms.Panel();
                this.dgvGrid = new System.Windows.Forms.DataGridView();
                this.pnlSeparator = new System.Windows.Forms.Panel();
                this.lblSubTitle = new System.Windows.Forms.Label();
                this.lblTitle = new System.Windows.Forms.Label();
                this.pnlCard.SuspendLayout();
                ((System.ComponentModel.ISupportInitialize)(this.dgvGrid)).BeginInit();
                this.SuspendLayout();
                // 
                // pnlCard
                // 
                this.pnlCard.Controls.Add(this.dgvGrid);
                this.pnlCard.Controls.Add(this.pnlSeparator);
                this.pnlCard.Controls.Add(this.lblSubTitle);
                this.pnlCard.Controls.Add(this.lblTitle);
                this.pnlCard.Dock = System.Windows.Forms.DockStyle.Fill;
                this.pnlCard.Location = new System.Drawing.Point(0, 0);
                this.pnlCard.Name = "pnlCard";
                this.pnlCard.Size = new System.Drawing.Size(580, 360);
                this.pnlCard.TabIndex = 0;
                // 
                // dgvGrid
                // 
                this.dgvGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                this.dgvGrid.Dock = System.Windows.Forms.DockStyle.Fill;
                this.dgvGrid.Location = new System.Drawing.Point(0, 61);
                this.dgvGrid.Name = "dgvGrid";
                this.dgvGrid.Size = new System.Drawing.Size(580, 299);
                this.dgvGrid.TabIndex = 3;
                // 
                // pnlSeparator
                // 
                this.pnlSeparator.Dock = System.Windows.Forms.DockStyle.Top;
                this.pnlSeparator.Location = new System.Drawing.Point(0, 60);
                this.pnlSeparator.Name = "pnlSeparator";
                this.pnlSeparator.Size = new System.Drawing.Size(580, 1);
                this.pnlSeparator.TabIndex = 2;
                // 
                // lblSubTitle
                // 
                this.lblSubTitle.Dock = System.Windows.Forms.DockStyle.Top;
                this.lblSubTitle.Location = new System.Drawing.Point(0, 32);
                this.lblSubTitle.Name = "lblSubTitle";
                this.lblSubTitle.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
                this.lblSubTitle.Size = new System.Drawing.Size(580, 28);
                this.lblSubTitle.TabIndex = 1;
                this.lblSubTitle.Text = " ";
                this.lblSubTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                // 
                // lblTitle
                // 
                this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
                this.lblTitle.Location = new System.Drawing.Point(0, 0);
                this.lblTitle.Name = "lblTitle";
                this.lblTitle.Padding = new System.Windows.Forms.Padding(12, 0, 12, 0);
                this.lblTitle.Size = new System.Drawing.Size(580, 32);
                this.lblTitle.TabIndex = 0;
                this.lblTitle.Text = "   عنوان البطاقة";
                this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
                // 
                // GridCardControl
                // 
                this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
                this.Controls.Add(this.pnlCard);
                this.Name = "GridCardControl";
                this.Size = new System.Drawing.Size(580, 360);
                this.pnlCard.ResumeLayout(false);
                ((System.ComponentModel.ISupportInitialize)(this.dgvGrid)).EndInit();
                this.ResumeLayout(false);
            }
        }
    }
