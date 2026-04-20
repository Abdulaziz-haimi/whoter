namespace water3
{
    partial class TemplatePreviewForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtPreview;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtPreview = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            this.txtPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPreview.Multiline = true;
            this.txtPreview.ReadOnly = true;
            this.txtPreview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPreview.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtPreview.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtPreview.BackColor = System.Drawing.Color.White;

            this.ClientSize = new System.Drawing.Size(520, 420);
            this.Controls.Add(this.txtPreview);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "TemplatePreviewForm";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
/*namespace water3
{
    partial class TemplatePreviewForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TextBox txtPreview;

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
            this.txtPreview = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtPreview
            // 
            this.txtPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPreview.Location = new System.Drawing.Point(0, 0);
            this.txtPreview.Multiline = true;
            this.txtPreview.ReadOnly = true;
            this.txtPreview.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPreview.Font = new System.Drawing.Font("Tahoma", 10F);
            this.txtPreview.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.txtPreview.BackColor = System.Drawing.Color.White;
            // 
            // TemplatePreviewForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 400);
            this.Controls.Add(this.txtPreview);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Name = "TemplatePreviewForm";
            this.Text = "TemplatePreviewForm";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
*/