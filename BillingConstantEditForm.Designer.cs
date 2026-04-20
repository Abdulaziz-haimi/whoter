namespace water3
{
    partial class BillingConstantEditForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtEffective;

        private System.Windows.Forms.Label lblUnit;
        private System.Windows.Forms.TextBox txtUnitPrice;

        private System.Windows.Forms.Label lblFees;
        private System.Windows.Forms.TextBox txtServiceFees;

        private System.Windows.Forms.CheckBox chkActive;

        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.TextBox txtNotes;

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;

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

            this.lblDate = new System.Windows.Forms.Label();
            this.dtEffective = new System.Windows.Forms.DateTimePicker();

            this.lblUnit = new System.Windows.Forms.Label();
            this.txtUnitPrice = new System.Windows.Forms.TextBox();

            this.lblFees = new System.Windows.Forms.Label();
            this.txtServiceFees = new System.Windows.Forms.TextBox();

            this.chkActive = new System.Windows.Forms.CheckBox();

            this.lblNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();

            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            this.SuspendLayout();

            // =========================
            // Form
            // =========================
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 330);
            this.Font = new System.Drawing.Font("Tahoma", 9F);
            this.Name = "BillingConstantEditForm";
            this.Text = "ثوابت الفوترة";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // =========================
            // lblDate
            // =========================
            this.lblDate.AutoSize = true;
            this.lblDate.Location = new System.Drawing.Point(350, 25);
            this.lblDate.Text = "ساري من تاريخ";

            // =========================
            // dtEffective
            // =========================
            this.dtEffective.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtEffective.Location = new System.Drawing.Point(30, 20);
            this.dtEffective.Size = new System.Drawing.Size(300, 22);

            // =========================
            // lblUnit
            // =========================
            this.lblUnit.AutoSize = true;
            this.lblUnit.Location = new System.Drawing.Point(350, 65);
            this.lblUnit.Text = "سعر الوحدة";

            // =========================
            // txtUnitPrice
            // =========================
            this.txtUnitPrice.Location = new System.Drawing.Point(30, 60);
            this.txtUnitPrice.Size = new System.Drawing.Size(300, 22);

            // =========================
            // lblFees
            // =========================
            this.lblFees.AutoSize = true;
            this.lblFees.Location = new System.Drawing.Point(350, 105);
            this.lblFees.Text = "رسوم الخدمة";

            // =========================
            // txtServiceFees
            // =========================
            this.txtServiceFees.Location = new System.Drawing.Point(30, 100);
            this.txtServiceFees.Size = new System.Drawing.Size(300, 22);
            this.txtServiceFees.Text = "0";

            // =========================
            // chkActive
            // =========================
            this.chkActive.AutoSize = true;
            this.chkActive.Location = new System.Drawing.Point(30, 135);
            this.chkActive.Text = "نشط";
            this.chkActive.Checked = true;

            // =========================
            // lblNotes
            // =========================
            this.lblNotes.AutoSize = true;
            this.lblNotes.Location = new System.Drawing.Point(350, 170);
            this.lblNotes.Text = "ملاحظات";

            // =========================
            // txtNotes
            // =========================
            this.txtNotes.Location = new System.Drawing.Point(30, 165);
            this.txtNotes.Size = new System.Drawing.Size(300, 60);
            this.txtNotes.Multiline = true;
            this.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;

            // =========================
            // btnOk
            // =========================
            this.btnOk.Location = new System.Drawing.Point(240, 245);
            this.btnOk.Size = new System.Drawing.Size(90, 30);
            this.btnOk.Text = "حفظ";
            this.btnOk.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            this.btnOk.ForeColor = System.Drawing.Color.White;
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOk.FlatAppearance.BorderSize = 0;
            this.btnOk.Cursor = System.Windows.Forms.Cursors.Hand;

            // =========================
            // btnCancel
            // =========================
            this.btnCancel.Location = new System.Drawing.Point(140, 245);
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.Text = "إلغاء";
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;

            // =========================
            // Add Controls
            // =========================
            this.Controls.Add(this.lblDate);
            this.Controls.Add(this.dtEffective);

            this.Controls.Add(this.lblUnit);
            this.Controls.Add(this.txtUnitPrice);

            this.Controls.Add(this.lblFees);
            this.Controls.Add(this.txtServiceFees);

            this.Controls.Add(this.chkActive);

            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.txtNotes);

            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);

            // buttons behavior
            this.AcceptButton = this.btnOk;
            this.CancelButton = this.btnCancel;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}
