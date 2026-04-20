using System;
using System.Windows.Forms;

namespace water3
{
    public partial class SettingEditForm : Form
    {
        public string SettingName { get; private set; }
        public string SettingValue { get; private set; }
        public string Description { get; private set; }

        private readonly bool isEdit;

        public SettingEditForm(DataGridViewRow row = null)
        {
            InitializeComponent();
            isEdit = row != null;

            if (isEdit)
            {
                txtName.Text = row.Cells["SettingName"].Value?.ToString() ?? "";
                txtValue.Text = row.Cells["SettingValue"].Value?.ToString() ?? "";
                txtDesc.Text = row.Cells["Description"].Value?.ToString() ?? "";
                Text = "تعديل إعداد";
            }
            else
            {
                Text = "إضافة إعداد";
            }

            btnSave.Click += (s, e) => SaveAndClose();
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
        }

        private void SaveAndClose()
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("اسم الإعداد مطلوب");
                return;
            }

            SettingName = txtName.Text.Trim();
            SettingValue = (txtValue.Text ?? "").Trim();
            Description = (txtDesc.Text ?? "").Trim();

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
/*using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace water3
{
    public partial class SettingEditForm : Form
    {
        public int SettingID { get; private set; }
        public string SettingName { get; private set; }
        public string SettingValue { get; private set; }
        public string Description { get; private set; }

        private readonly DataGridViewRow _row;

        public SettingEditForm(DataGridViewRow row = null)
        {
            InitializeComponent();

            _row = row;

            // خصائص الفورم
            this.Text = row == null ? "إضافة إعداد جديد" : "تعديل الإعداد";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Font = new Font("Tahoma", 9);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;

            FillFromRow(row);
        }

        private void FillFromRow(DataGridViewRow row)
        {
            if (row == null)
            {
                // إضافة
                SettingID = new Random().Next(100, 1000); // كما هو في كودك
                txtSettingName.Text = "";
                txtSettingValue.Text = "";
                txtDescription.Text = "";
                return;
            }

            // تعديل
            SettingID = SafeInt(row, "SettingID", new Random().Next(100, 1000));
            txtSettingName.Text = SafeString(row, "SettingName");
            txtSettingValue.Text = SafeString(row, "SettingValue");
            txtDescription.Text = SafeString(row, "Description");
        }

        private string SafeString(DataGridViewRow row, string col)
        {
            try
            {
                if (row?.Cells[col]?.Value == null) return "";
                return row.Cells[col].Value.ToString();
            }
            catch { return ""; }
        }

        private int SafeInt(DataGridViewRow row, string col, int fallback)
        {
            try
            {
                var v = row?.Cells[col]?.Value;
                if (v == null) return fallback;
                return int.TryParse(v.ToString(), out int x) ? x : fallback;
            }
            catch { return fallback; }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveSetting();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SaveSetting()
        {
            if (string.IsNullOrWhiteSpace(txtSettingName.Text))
            {
                MessageBox.Show("يرجى إدخال اسم الإعداد", "تحذير",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SettingName = txtSettingName.Text.Trim();
            SettingValue = txtSettingValue.Text ?? "";
            Description = txtDescription.Text ?? "";

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
*/