using System;
using System.Drawing;
using System.Windows.Forms;
using water3.Models;

namespace water3.Forms
{
    public partial class PaymentsForm
    {
        private bool _suppressSearchChanged;
        private bool _suppressSuggest;
        private Panel _subsPanel;
        private ListBox _subsList;
         private ToolStripDropDown _subsDrop;
        
        private ToolStripControlHost _subsHost;

        private readonly Color _popupBack = Color.White;
        private readonly Color _popupBorder = Color.FromArgb(225, 230, 235);
        private readonly Color _popupHover = Color.FromArgb(225, 240, 255);
        private readonly Color _popupText = Color.FromArgb(33, 37, 41);

        private void SubscriberSearch_GotFocus(object sender, EventArgs e)
        {
            if (IsPlaceholderActive())
            {
                _suppressSuggest = true;
                txtSubscriberSearch.Text = "";
                txtSubscriberSearch.ForeColor = Color.Black;
                _suppressSuggest = false;
            }

            var key = (txtSubscriberSearch.Text ?? string.Empty).Trim();
            if (key.Length > 0) LoadSubscribersForSearch(key, keepTypedText: true);
        }

        private void SubscriberSearch_LostFocus(object sender, EventArgs e)
        {
            if (_subsPanel != null && _subsPanel.Visible)
            {
                var panelRect = _subsPanel.RectangleToScreen(_subsPanel.ClientRectangle);
                if (panelRect.Contains(Cursor.Position))
                    return;
            }

            if (string.IsNullOrWhiteSpace(txtSubscriberSearch.Text))
                SetSearchPlaceholder();

            HideSubscribersPopup();
        }

        private void TxtSubscriberSearch_TextChanged(object sender, EventArgs e)
        {
            if (_suppressSearchChanged) return;
            if (_suppressSuggest) return;
            if (!txtSubscriberSearch.Focused) return;
            if (IsPlaceholderActive()) return;

            selectedSubscriberID = null;
            lblCurrentBalance.Text = "0.00 ريال";
            pnlMessage.Visible = false;
            lblMessage.Text = "";

            string key = (txtSubscriberSearch.Text ?? string.Empty).Trim();
            if (key.Length == 0) { HideSubscribersPopup(); return; }

            LoadSubscribersForSearch(key, keepTypedText: true);
        }

        private void TxtSubscriberSearch_KeyDown(object sender, KeyEventArgs e)
        {
            bool visible = _subsPanel != null && _subsPanel.Visible;

            if (visible)
            {
                if (e.KeyCode == Keys.Down)
                {
                    e.Handled = true; e.SuppressKeyPress = true;
                    if (_subsList.Items.Count > 0)
                        _subsList.SelectedIndex = Math.Min(_subsList.SelectedIndex + 1, _subsList.Items.Count - 1);
                    return;
                }

                if (e.KeyCode == Keys.Up)
                {
                    e.Handled = true; e.SuppressKeyPress = true;
                    if (_subsList.Items.Count > 0)
                        _subsList.SelectedIndex = Math.Max(_subsList.SelectedIndex - 1, 0);
                    return;
                }

                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    e.Handled = true; e.SuppressKeyPress = true;
                    CommitSubscriberSelection();
                    return;
                }

                if (e.KeyCode == Keys.Escape)
                {
                    e.Handled = true; e.SuppressKeyPress = true;
                    HideSubscribersPopup();
                    return;
                }
            }
            else
            {
                if (e.KeyCode == Keys.Down)
                {
                    e.Handled = true; e.SuppressKeyPress = true;
                    string key = (txtSubscriberSearch.Text ?? "").Trim();
                    if (key.Length > 0) LoadSubscribersForSearch(key, keepTypedText: true);
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true; e.SuppressKeyPress = true;
                    string key = (txtSubscriberSearch.Text ?? "").Trim();
                    if (key.Length > 0)
                    {
                        LoadSubscribersForSearch(key, keepTypedText: true);
                        if (_subsList.Items.Count > 0)
                        {
                            if (_subsList.SelectedIndex < 0) _subsList.SelectedIndex = 0;
                            CommitSubscriberSelection();
                        }
                    }
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    e.Handled = true; e.SuppressKeyPress = true;
                    HideSubscribersPopup();
                }
            }
        }
        private void SetSearchPlaceholder()
        {
            _suppressSuggest = true;
            txtSubscriberSearch.Text = SearchPlaceholder;
            txtSubscriberSearch.ForeColor = Color.Gray;
            _suppressSuggest = false;
        }

        private bool IsPlaceholderActive()
        {
            return (txtSubscriberSearch.Text ?? string.Empty).Trim() == SearchPlaceholder.Trim()
                   && txtSubscriberSearch.ForeColor == Color.Gray;
        }

        private void InitSubscribersPopup()
        {
            _subsPanel = new Panel
            {
                Visible = false,
                BackColor = _popupBorder,
                Padding = new Padding(1)
            };

            _subsList = new ListBox
            {
                BorderStyle = BorderStyle.None,
                IntegralHeight = false,
                DrawMode = DrawMode.OwnerDrawFixed,
                ItemHeight = 34,
                Font = new Font("Segoe UI", 10f, FontStyle.Regular),
                BackColor = _popupBack,
                ForeColor = _popupText,
                RightToLeft = this.RightToLeft,
                TabStop = false // ✅ لا يأخذ فوكس
            };

            _subsList.DrawItem += SubsList_DrawItem;

            _subsList.MouseMove += (s, e) =>
            {
                int idx = _subsList.IndexFromPoint(e.Location);
                if (idx >= 0 && idx < _subsList.Items.Count && idx != _subsList.SelectedIndex)
                    _subsList.SelectedIndex = idx;
            };

            _subsList.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    CommitSubscriberSelection();
            };

            _subsPanel.Controls.Add(_subsList);

            // ✅ أضفها للفورم فوق كل العناصر
            this.Controls.Add(_subsPanel);
            _subsPanel.BringToFront();
        }
        private void SubsList_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index < 0) return;

            bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            var bg = selected ? _popupHover : _popupBack;

            using (var b = new SolidBrush(bg))
                e.Graphics.FillRectangle(b, e.Bounds);

            string text = _subsList.Items[e.Index]?.ToString() ?? "";
            var rect = Rectangle.Inflate(e.Bounds, -10, 0);

            TextRenderer.DrawText(
                e.Graphics,
                text,
                _subsList.Font,
                rect,
                _popupText,
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.EndEllipsis |
                (this.RightToLeft == RightToLeft.Yes ? TextFormatFlags.RightToLeft : 0)
            );

            using (var pen = new Pen(Color.FromArgb(245, 247, 250)))
                e.Graphics.DrawLine(pen, e.Bounds.Left + 8, e.Bounds.Bottom - 1, e.Bounds.Right - 8, e.Bounds.Bottom - 1);

            e.DrawFocusRectangle();
        }

        private void SubsList_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                CommitSubscriberSelection();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                HideSubscribersPopup();
                txtSubscriberSearch.Focus();
            }
        }

        private void LoadSubscribersForSearch(string searchKey, bool keepTypedText)
        {
            if (_subsList == null) return;

            _suppressSuggest = true;
            try
            {
                string typed = keepTypedText ? (txtSubscriberSearch.Text ?? "") : "";
                int caret = keepTypedText ? txtSubscriberSearch.SelectionStart : 0;

                _subsList.BeginUpdate();
                _subsList.Items.Clear();

                var items = _subsRepo.Search(searchKey);
                foreach (var it in items) _subsList.Items.Add(it);

                _subsList.EndUpdate();

                if (keepTypedText)
                {
                    txtSubscriberSearch.Text = typed;
                    txtSubscriberSearch.SelectionStart = Math.Min(caret, txtSubscriberSearch.Text.Length);
                    txtSubscriberSearch.SelectionLength = 0;
                }

                if (_subsList.Items.Count > 0)
                {
                    if (_subsList.SelectedIndex < 0) _subsList.SelectedIndex = 0;
                    ShowSubscribersPopup();
                }
                else
                {
                    HideSubscribersPopup();
                }
            }
            finally
            {
                _suppressSuggest = false;
            }
        }
        private void ShowSubscribersPopup()
        {
            if (_subsPanel == null || _subsList == null) return;
            if (_subsList.Items.Count == 0) { HideSubscribersPopup(); return; }
            if (pnlSearch == null || pnlSearch.IsDisposed) return;

            int width = pnlSearch.Width;
            int rows = Math.Min(_subsList.Items.Count, 8);
            int height = (rows * _subsList.ItemHeight) + 10;

            // ✅ مكان pnlSearch على الفورم (client coords)
            Point belowSearch = this.PointToClient(
                pnlSearch.Parent.PointToScreen(new Point(pnlSearch.Left, pnlSearch.Bottom + 2))
            );

            _subsPanel.SetBounds(belowSearch.X, belowSearch.Y, width, height);
            _subsList.SetBounds(1, 1, width - 2, height - 2);

            _subsPanel.Visible = true;
            _subsPanel.BringToFront();

            // ✅ لا توقف الكتابة
            txtSubscriberSearch.Focus();
            txtSubscriberSearch.SelectionStart = txtSubscriberSearch.Text.Length;
            txtSubscriberSearch.SelectionLength = 0;
        }
        private void RepositionSubscribersPopup()
        {
            if (_subsPanel == null || !_subsPanel.Visible) return;
            ShowSubscribersPopup(); // يعيد حساب مكانها تحت البحث
        }

        private void HideSubscribersPopup()
        {
            if (_subsPanel != null)
                _subsPanel.Visible = false;
        }
        private void CommitSubscriberSelection()
        {
            if (_subsList == null || _subsList.Items.Count == 0) return;
            if (_subsList.SelectedIndex < 0) _subsList.SelectedIndex = 0;

            var item = _subsList.SelectedItem as SubscriberSearchItem;
            if (item == null) return;

            _suppressSearchChanged = true;
            _suppressSuggest = true;
            try
            {
                txtSubscriberSearch.Text = item.Name;
                txtSubscriberSearch.ForeColor = Color.Black;
                txtSubscriberSearch.SelectionStart = txtSubscriberSearch.Text.Length;
            }
            finally
            {
                _suppressSuggest = false;
                _suppressSearchChanged = false;
            }

            selectedSubscriberID = item.SubscriberID;

            HideSubscribersPopup();
            LoadSubscriberBalance(dtpPaymentDate.Value.Date);

            txtAmount.Focus();
            txtAmount.SelectAll();
        }
    }
}