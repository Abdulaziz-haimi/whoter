using System;
using System.Drawing;
using System.Windows.Forms;

namespace water3.Licensing
{
    public class LicenseActivationForm : Form
    {
        private Label lblTitle;
        private Label lblMachineTitle;
        private TextBox txtMachineId;
        private Label lblLicense;
        private TextBox txtLicense;
        private Button btnActivate;
        private Button btnCopyMachineId;
        private Button btnExit;

        public LicenseActivationForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "تفعيل النظام";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(620, 430);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Font = new Font("Tahoma", 10F);

            lblTitle = new Label();
            lblTitle.Text = "تفعيل نظام إدارة المياه";
            lblTitle.Font = new Font("Tahoma", 16F, FontStyle.Bold);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 70;

            lblMachineTitle = new Label();
            lblMachineTitle.Text = "رقم الجهاز:";
            lblMachineTitle.Location = new Point(40, 90);
            lblMachineTitle.Size = new Size(520, 25);

            txtMachineId = new TextBox();
            txtMachineId.Location = new Point(40, 120);
            txtMachineId.Size = new Size(520, 30);
            txtMachineId.ReadOnly = true;
            txtMachineId.TextAlign = HorizontalAlignment.Center;
            txtMachineId.Text = MachineFingerprint.GetMachineId();

            btnCopyMachineId = new Button();
            btnCopyMachineId.Text = "نسخ رقم الجهاز";
            btnCopyMachineId.Location = new Point(40, 160);
            btnCopyMachineId.Size = new Size(160, 35);
            btnCopyMachineId.Click += BtnCopyMachineId_Click;

            lblLicense = new Label();
            lblLicense.Text = "مفتاح التفعيل:";
            lblLicense.Location = new Point(40, 210);
            lblLicense.Size = new Size(520, 25);

            txtLicense = new TextBox();
            txtLicense.Location = new Point(40, 240);
            txtLicense.Size = new Size(520, 80);
            txtLicense.Multiline = true;
            txtLicense.ScrollBars = ScrollBars.Vertical;

            btnActivate = new Button();
            btnActivate.Text = "تفعيل";
            btnActivate.Location = new Point(400, 340);
            btnActivate.Size = new Size(160, 40);
            btnActivate.Click += BtnActivate_Click;

            btnExit = new Button();
            btnExit.Text = "خروج";
            btnExit.Location = new Point(40, 340);
            btnExit.Size = new Size(160, 40);
            btnExit.Click += delegate { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblMachineTitle);
            this.Controls.Add(txtMachineId);
            this.Controls.Add(btnCopyMachineId);
            this.Controls.Add(lblLicense);
            this.Controls.Add(txtLicense);
            this.Controls.Add(btnActivate);
            this.Controls.Add(btnExit);
        }

        private void BtnCopyMachineId_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(txtMachineId.Text);
            MessageBox.Show("تم نسخ رقم الجهاز.", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnActivate_Click(object sender, EventArgs e)
        {
            LicenseStatus status = LicenseService.Activate(txtLicense.Text.Trim());

            if (!status.IsValid)
            {
                MessageBox.Show(status.Message, "فشل التفعيل", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("تم تفعيل النظام بنجاح.", "تم التفعيل", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
