namespace water3
{
    partial class BillingConstantsForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel toolbar;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnToggleActive;
        private System.Windows.Forms.Button btnRefresh;

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
            this.toolbar = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnToggleActive = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.dgv = new System.Windows.Forms.DataGridView();

            this.toolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
            this.SuspendLayout();

            // =========================
            // Form
            // =========================
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.Name = "BillingConstantsForm";
            this.Text = "إعدادات الثوابت (التعرفة)";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            // =========================
            // toolbar
            // =========================
            this.toolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolbar.Height = 50;
            this.toolbar.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.toolbar.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);

            // =========================
            // btnAdd
            // =========================
            this.btnAdd.Text = "إضافة";
            this.btnAdd.Size = new System.Drawing.Size(110, 32);
            this.btnAdd.Location = new System.Drawing.Point(10, 9);
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;

            // =========================
            // btnEdit
            // =========================
            this.btnEdit.Text = "تعديل";
            this.btnEdit.Size = new System.Drawing.Size(110, 32);
            this.btnEdit.Location = new System.Drawing.Point(125, 9);
            this.btnEdit.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.btnEdit.ForeColor = System.Drawing.Color.White;
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.FlatAppearance.BorderSize = 0;
            this.btnEdit.Cursor = System.Windows.Forms.Cursors.Hand;

            // =========================
            // btnDelete
            // =========================
            this.btnDelete.Text = "حذف";
            this.btnDelete.Size = new System.Drawing.Size(110, 32);
            this.btnDelete.Location = new System.Drawing.Point(240, 9);
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;

            // =========================
            // btnToggleActive
            // =========================
            this.btnToggleActive.Text = "تفعيل/إيقاف";
            this.btnToggleActive.Size = new System.Drawing.Size(120, 32);
            this.btnToggleActive.Location = new System.Drawing.Point(355, 9);
            this.btnToggleActive.BackColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.btnToggleActive.ForeColor = System.Drawing.Color.White;
            this.btnToggleActive.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleActive.FlatAppearance.BorderSize = 0;
            this.btnToggleActive.Cursor = System.Windows.Forms.Cursors.Hand;

            // =========================
            // btnRefresh
            // =========================
            this.btnRefresh.Text = "تحديث";
            this.btnRefresh.Size = new System.Drawing.Size(110, 32);
            this.btnRefresh.Location = new System.Drawing.Point(480, 9);
            this.btnRefresh.BackColor = System.Drawing.Color.White;
            this.btnRefresh.ForeColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(44, 62, 80);
            this.btnRefresh.FlatAppearance.BorderSize = 1;
            this.btnRefresh.Cursor = System.Windows.Forms.Cursors.Hand;

            this.toolbar.Controls.Add(this.btnAdd);
            this.toolbar.Controls.Add(this.btnEdit);
            this.toolbar.Controls.Add(this.btnDelete);
            this.toolbar.Controls.Add(this.btnToggleActive);
            this.toolbar.Controls.Add(this.btnRefresh);

            // =========================
            // dgv
            // =========================
            this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv.AllowUserToAddRows = false;
            this.dgv.AllowUserToDeleteRows = false;
            this.dgv.AllowUserToResizeRows = false;
            this.dgv.MultiSelect = false;
            this.dgv.ReadOnly = true;
            this.dgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv.RowHeadersVisible = false;
            this.dgv.BackgroundColor = System.Drawing.Color.White;
            this.dgv.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dgv.EnableHeadersVisualStyles = false;
            this.dgv.ColumnHeadersHeight = 40;
            this.dgv.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(245, 246, 248);
            this.dgv.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dgv.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold);
            this.dgv.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(225, 235, 250);
            this.dgv.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;
            this.dgv.RowTemplate.Height = 34;

            // =========================
            // Add controls
            // =========================
            this.Controls.Add(this.dgv);
            this.Controls.Add(this.toolbar);

            this.toolbar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion
    }
}
