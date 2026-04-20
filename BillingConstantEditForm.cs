using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace water3
{
    public partial class BillingConstantEditForm : Form
    {
        public DateTime EffectiveFrom => dtEffective.Value.Date;

        public decimal UnitPrice =>
            decimal.TryParse(NormalizeNumber(txtUnitPrice.Text), NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : 0m;

        public decimal ServiceFees =>
            decimal.TryParse(NormalizeNumber(txtServiceFees.Text), NumberStyles.Any, CultureInfo.InvariantCulture, out var v) ? v : 0m;

        public bool IsActive => chkActive.Checked;
        public string Notes => (txtNotes.Text ?? "").Trim();

        public BillingConstantEditForm(DataGridViewRow row)
        {
            InitializeComponent();

            // ربط الأحداث خارج InitializeComponent
            btnOk.Click += BtnOk_Click;
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            // تحسين إدخال الأرقام (اختياري)
            txtUnitPrice.KeyPress += Numeric_KeyPress;
            txtServiceFees.KeyPress += Numeric_KeyPress;

            if (row != null) LoadFromRow(row);
        }

        private void LoadFromRow(DataGridViewRow row)
        {
            if (row.Cells["EffectiveFrom"].Value != null && row.Cells["EffectiveFrom"].Value != DBNull.Value)
                dtEffective.Value = Convert.ToDateTime(row.Cells["EffectiveFrom"].Value);

            txtUnitPrice.Text = row.Cells["UnitPrice"].Value?.ToString() ?? "0";
            txtServiceFees.Text = row.Cells["ServiceFees"].Value?.ToString() ?? "0";
            chkActive.Checked = row.Cells["IsActive"].Value != null && row.Cells["IsActive"].Value != DBNull.Value
                                && Convert.ToBoolean(row.Cells["IsActive"].Value);
            txtNotes.Text = row.Cells["Notes"].Value?.ToString() ?? "";
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (!TryParseMoney(txtUnitPrice.Text, out var up) || up < 0)
            {
                MessageBox.Show("سعر الوحدة غير صحيح.");
                txtUnitPrice.Focus();
                txtUnitPrice.SelectAll();
                return;
            }

            if (!TryParseMoney(txtServiceFees.Text, out var sf) || sf < 0)
            {
                MessageBox.Show("رسوم الخدمة غير صحيحة.");
                txtServiceFees.Focus();
                txtServiceFees.SelectAll();
                return;
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private static bool TryParseMoney(string input, out decimal value)
        {
            var s = NormalizeNumber(input);
            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        private static string NormalizeNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "0";
            input = input.Trim();

            // يسمح بفاصلة أو نقطة، ويوحّدها لنقطة
            input = input.Replace(",", ".");

            // إزالة أي شيء غير رقم/نقطة/سالب
            var sb = new System.Text.StringBuilder();
            bool dot = false;
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsDigit(c)) sb.Append(c);
                else if (c == '-' && sb.Length == 0) sb.Append(c);
                else if (c == '.' && !dot) { sb.Append('.'); dot = true; }
            }

            var outS = sb.ToString();
            return string.IsNullOrWhiteSpace(outS) ? "0" : outS;
        }

        private void Numeric_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) return;

            // أرقام فقط + نقطة + فاصلة
            if (char.IsDigit(e.KeyChar)) return;

            if (e.KeyChar == '.' || e.KeyChar == ',')
            {
                var tb = sender as TextBox;
                if (tb != null && tb.Text.Contains(".") == false && tb.Text.Contains(",") == false)
                    return;
            }

            e.Handled = true;
        }
    }
}
