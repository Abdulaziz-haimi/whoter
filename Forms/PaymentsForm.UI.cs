using System;
using System.Drawing;
using System.Windows.Forms;
using water3.Utils;

namespace water3.Forms
{
    public partial class PaymentsForm
    {
        private readonly ErrorProvider _err = new ErrorProvider();

        private void ApplyTheme()
        {
            _err.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            _err.RightToLeft = true;

            this.Font = new System.Drawing.Font("Segoe UI", 10);

            lblCurrentBalance.ForeColor = Color.DarkRed;
            lblCurrentBalance.Font = new System.Drawing.Font("Segoe UI", 11, FontStyle.Bold);

            toolTip1.SetToolTip(btnAdd, "تسجيل دفعة");
            toolTip1.SetToolTip(btnRefresh, "تحديث البيانات");
            toolTip1.SetToolTip(btnClear, "مسح الحقول");
            toolTip1.SetToolTip(btnExport, "تصدير");

            MakeHover(btnAdd, btnAdd.BackColor, ControlPaint.Light(btnAdd.BackColor));
            MakeHover(btnRefresh, btnRefresh.BackColor, ControlPaint.Light(btnRefresh.BackColor));
            MakeHover(btnClear, btnClear.BackColor, ControlPaint.Light(btnClear.BackColor));
            MakeHover(btnExport, btnExport.BackColor, ControlPaint.Light(btnExport.BackColor));

            txtAmount.BorderStyle = BorderStyle.FixedSingle;
            txtNotes.BorderStyle = BorderStyle.FixedSingle;

            ddlCollectors.FlatStyle = FlatStyle.Flat;
            ddlPaymentType.FlatStyle = FlatStyle.Flat;

            pnlMessage.Visible = false;
            pnlMessage.Padding = new Padding(10);
            lblMessage.Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold);

            txtSubscriberSearch.BorderStyle = BorderStyle.None;
            txtSubscriberSearch.Font = new System.Drawing.Font("Segoe UI", 10f);
            txtSubscriberSearch.BackColor = Color.White;
            if (string.IsNullOrWhiteSpace(txtSubscriberSearch.Text) || txtSubscriberSearch.Text == SearchPlaceholder)
                SetSearchPlaceholder();
            else
                txtSubscriberSearch.ForeColor = Color.Black;
            txtSubscriberSearch.TextAlign = HorizontalAlignment.Right;

            pnlSearch.BackColor = Color.White;
            // تأكيد الأيقونات (حتى لو الديزاين ما طبّقها)
            btnAdd.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnAdd.IconChar = FontAwesome.Sharp.IconChar.PlusCircle;
            btnAdd.IconColor = Color.White;
            btnAdd.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnAdd.ImageAlign = ContentAlignment.MiddleRight;  // مناسب لواجهة RTL
            btnAdd.TextAlign = ContentAlignment.MiddleCenter;

            btnRefresh.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnRefresh.IconChar = FontAwesome.Sharp.IconChar.Rotate;
            btnRefresh.IconColor = Color.White;
            btnRefresh.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnRefresh.ImageAlign = ContentAlignment.MiddleRight;
            btnRefresh.TextAlign = ContentAlignment.MiddleCenter;

            btnClear.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnClear.IconChar = FontAwesome.Sharp.IconChar.Broom;
            btnClear.IconColor = Color.FromArgb(33, 37, 41);
            btnClear.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnClear.ImageAlign = ContentAlignment.MiddleRight;
            btnClear.TextAlign = ContentAlignment.MiddleCenter;

            btnExport.IconFont = FontAwesome.Sharp.IconFont.Auto;
            btnExport.IconChar = FontAwesome.Sharp.IconChar.FileExcel;
            btnExport.IconColor = Color.White;
            btnExport.Text = ""; // لأنه زر أيقونة فقط
        }

        private void MakeHover(Control c, Color normal, Color hover)
        {
            c.MouseEnter += (s, e) => c.BackColor = hover;
            c.MouseLeave += (s, e) => c.BackColor = normal;
        }

        private void ClearErrors()
        {
            _err.SetError(txtSubscriberSearch, "");
            _err.SetError(txtAmount, "");
            _err.SetError(ddlCollectors, "");
        }

        private void SetError(Control c, string msg) => _err.SetError(c, msg);

        private void ShowMsg(string msg, bool isError)
        {
            if (_msgTimer != null) _msgTimer.Stop();

            pnlMessage.Visible = !string.IsNullOrWhiteSpace(msg);
            lblMessage.ForeColor = isError ? Color.FromArgb(220, 53, 69) : Color.FromArgb(40, 167, 69);
            lblMessage.Text = msg;

            pnlMessage.BackColor = isError
                ? Color.FromArgb(255, 235, 238)
                : Color.FromArgb(232, 245, 233);

            if (!isError && pnlMessage.Visible && _msgTimer != null)
                _msgTimer.Start();
        }

     private void ClearAfterSuccess()
{
    ClearErrors();

    txtAmount.Clear();
    txtNotes.Clear();
    dtpPaymentDate.Value = DateTime.Today;

    if (ddlPaymentType.Items.Count > 0)
        ddlPaymentType.SelectedIndex = 0;

    selectedSubscriberID = null;
    SetSearchPlaceholder();
    lblCurrentBalance.Text = "0.00 ريال";
    HideSubscribersPopup();

    txtSubscriberSearch.Focus();
}
        private void ClearInputs()
        {
            txtAmount.Clear();
            txtNotes.Clear();

            pnlMessage.Visible = false;
            lblMessage.Text = "";

            HideSubscribersPopup();

            _suppressSuggest = true;
            SetSearchPlaceholder();
            _suppressSuggest = false;

            selectedSubscriberID = null;
            lblCurrentBalance.Text = "0.00 ريال";

            txtSubscriberSearch.Focus();
        }

        private void WireAmountInput()
        {
            txtAmount.KeyPress += (s, e) =>
            {
                if (char.IsControl(e.KeyChar)) return;

                char ch = MoneyParser.NormalizeArabicDigit(e.KeyChar);

                if (char.IsDigit(ch)) { e.KeyChar = ch; return; }

                if (ch == '.')
                {
                    if (!txtAmount.Text.Contains(".")) return;
                    e.Handled = true;
                    return;
                }

                e.Handled = true;
            };

            txtAmount.TextChanged += (s, e) =>
            {
                if (!txtAmount.Focused) return;

                string norm = MoneyParser.NormalizeMoneyText(txtAmount.Text);
                if (txtAmount.Text != norm)
                {
                    int pos = txtAmount.SelectionStart;
                    txtAmount.Text = norm;
                    txtAmount.SelectionStart = Math.Min(pos, txtAmount.Text.Length);
                }
            };

            txtAmount.Leave += (s, e) =>
            {
                decimal v;
                if (MoneyParser.TryParse(txtAmount.Text, out v))
                    txtAmount.Text = v.ToString("N2");
            };
        }
    }
}