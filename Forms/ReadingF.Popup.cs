using System;
using System.Windows.Forms;
using water3.Models;

namespace water3.Forms
{
    public partial class ReadingF : Form
    {
        private void WireSuggestionEvents()
        {
            txtSubscriberSearch.PreviewKeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Tab)
                    e.IsInputKey = true;
            };

            txtSubscriberSearch.TextChanged += TxtSubscriberSearch_TextChanged_UI;
            txtSubscriberSearch.KeyDown += TxtSubscriberSearch_KeyDown_UI;

            txtSubscriberSearch.Leave += (s, e) =>
                BeginInvoke((MethodInvoker)(() =>
                {
                    var p = PointToClient(Cursor.Position);
                    if (!lstSubscriberSuggestions.Bounds.Contains(p))
                        lstSubscriberSuggestions.Visible = false;
                }));

            lstSubscriberSuggestions.MouseMove += (s, e) =>
            {
                int idx = lstSubscriberSuggestions.IndexFromPoint(e.Location);
                if (idx >= 0 &&
                    idx < lstSubscriberSuggestions.Items.Count &&
                    idx != lstSubscriberSuggestions.SelectedIndex)
                {
                    lstSubscriberSuggestions.SelectedIndex = idx;
                }
            };

            lstSubscriberSuggestions.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                    SelectSubscriberFromList_UI();
            };
        }

        private void TxtSubscriberSearch_TextChanged_UI(object sender, EventArgs e)
        {
            if (_suppressSearch)
                return;

            SelectedSubscriberID = null;
            SelectedMeterID = null;
            _latestReadingDate = null;

            btnAdd.Enabled = false;
            lblMessage.Text = "";
            txtPreviousReading.Text = "0";
            txtCurrentReading.Clear();
            txtConsumption.Clear();

            lblLastReading.Text = "آخر قراءة: ---";
            lblLastInvoice.Text = "آخر فاتورة: ---";
            LoadDefaultTariff();

            string text = (txtSubscriberSearch.Text ?? "").Trim();
            if (text.Length < 2)
            {
                lstSubscriberSuggestions.Visible = false;
                return;
            }

            var items = SearchSubscribersAndMeters(text);
            if (items == null || items.Count == 0)
            {
                lstSubscriberSuggestions.Visible = false;
                return;
            }

            lstSubscriberSuggestions.DataSource = null;
            lstSubscriberSuggestions.DataSource = items;
            lstSubscriberSuggestions.DisplayMember = "DisplayText";

            if (lstSubscriberSuggestions.SelectedIndex < 0 && lstSubscriberSuggestions.Items.Count > 0)
                lstSubscriberSuggestions.SelectedIndex = 0;

            PositionSuggestionList();
            lstSubscriberSuggestions.Visible = true;
            lstSubscriberSuggestions.BringToFront();

            txtSubscriberSearch.Focus();
            txtSubscriberSearch.SelectionStart = txtSubscriberSearch.Text.Length;
        }

        private void TxtSubscriberSearch_KeyDown_UI(object sender, KeyEventArgs e)
        {
            bool visible = lstSubscriberSuggestions.Visible && lstSubscriberSuggestions.Items.Count > 0;

            if (!visible)
                return;

            if (e.KeyCode == Keys.Down)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                lstSubscriberSuggestions.SelectedIndex =
                    Math.Min(lstSubscriberSuggestions.SelectedIndex + 1,
                             lstSubscriberSuggestions.Items.Count - 1);
                return;
            }

            if (e.KeyCode == Keys.Up)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                lstSubscriberSuggestions.SelectedIndex =
                    Math.Max(lstSubscriberSuggestions.SelectedIndex - 1, 0);
                return;
            }

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                SelectSubscriberFromList_UI();
                return;
            }

            if (e.KeyCode == Keys.Escape)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                lstSubscriberSuggestions.Visible = false;
            }
        }

        private void PositionSuggestionList()
        {
            if (!txtSubscriberSearch.IsHandleCreated)
                return;

            var screen = txtSubscriberSearch.Parent.PointToScreen(
                new System.Drawing.Point(txtSubscriberSearch.Left, txtSubscriberSearch.Bottom + 2));

            var client = PointToClient(screen);

            lstSubscriberSuggestions.Width = txtSubscriberSearch.Width;
            lstSubscriberSuggestions.Left = client.X;
            lstSubscriberSuggestions.Top = client.Y;

            int rows = Math.Min(8, Math.Max(1, lstSubscriberSuggestions.Items.Count));
            lstSubscriberSuggestions.Height = (rows * lstSubscriberSuggestions.ItemHeight) + 6;

            lstSubscriberSuggestions.BringToFront();
        }

        private void SelectSubscriberFromList_UI()
        {
            if (lstSubscriberSuggestions.SelectedItem == null)
                return;

            var item = lstSubscriberSuggestions.SelectedItem as SubscriberMeterSuggestion;
            if (item == null)
                return;

            _suppressSearch = true;
            try
            {
                SelectedSubscriberID = item.SubscriberID;
                SelectedMeterID = item.MeterID;

                txtSubscriberSearch.Text = item.DisplayText;
                txtSubscriberSearch.SelectionStart = txtSubscriberSearch.Text.Length;
                lstSubscriberSuggestions.Visible = false;
            }
            finally
            {
                _suppressSearch = false;
            }

            LoadTariffForSubscriber(item.SubscriberID, dtpReadingDate.Value.Date);
            RefreshSelectedMeterData(clearCurrentInput: true);

            txtCurrentReading.Focus();
            txtCurrentReading.SelectAll();
        }
    }
}
/*using System;
using System.Linq;
using System.Windows.Forms;
using water3.Models;

namespace water3.Forms
{
    public partial class ReadingF : Form
    {
        private void WireSuggestionEvents()
        {
            txtSubscriberSearch.PreviewKeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Tab) e.IsInputKey = true;
            };

            txtSubscriberSearch.TextChanged += TxtSubscriberSearch_TextChanged_UI;
            txtSubscriberSearch.KeyDown += TxtSubscriberSearch_KeyDown_UI;

            txtSubscriberSearch.Leave += (s, e) =>
                BeginInvoke((MethodInvoker)(() =>
                {
                    var p = PointToClient(Cursor.Position);
                    if (!lstSubscriberSuggestions.Bounds.Contains(p))
                        lstSubscriberSuggestions.Visible = false;
                }));

            lstSubscriberSuggestions.MouseMove += (s, e) =>
            {
                int idx = lstSubscriberSuggestions.IndexFromPoint(e.Location);
                if (idx >= 0 && idx < lstSubscriberSuggestions.Items.Count &&
                    idx != lstSubscriberSuggestions.SelectedIndex)
                    lstSubscriberSuggestions.SelectedIndex = idx;
            };

            lstSubscriberSuggestions.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left) SelectSubscriberFromList_UI();
            };
        }

        private void TxtSubscriberSearch_TextChanged_UI(object sender, EventArgs e)
        {
            if (_suppressSearch) return;

            SelectedSubscriberID = null;
            SelectedMeterID = null;

            btnAdd.Enabled = false;
            lblMessage.Text = "";
            txtPreviousReading.Text = "0";
            txtConsumption.Text = "";
            lblLastReading.Text = "آخر قراءة: ---";
            lblLastInvoice.Text = "آخر فاتورة: ---";
            LoadDefaultTariff();

            string text = (txtSubscriberSearch.Text ?? "").Trim();
            if (text.Length < 2)
            {
                lstSubscriberSuggestions.Visible = false;
                return;
            }

            var items = SearchSubscribersAndMeters(text);
            if (items.Count == 0)
            {
                lstSubscriberSuggestions.Visible = false;
                return;
            }

            lstSubscriberSuggestions.DataSource = items;
            lstSubscriberSuggestions.DisplayMember = "DisplayText";

            if (lstSubscriberSuggestions.SelectedIndex < 0 && lstSubscriberSuggestions.Items.Count > 0)
                lstSubscriberSuggestions.SelectedIndex = 0;

            PositionSuggestionList();
            lstSubscriberSuggestions.Visible = true;
            lstSubscriberSuggestions.BringToFront();

            txtSubscriberSearch.Focus();
            txtSubscriberSearch.SelectionStart = txtSubscriberSearch.Text.Length;
        }

        private void TxtSubscriberSearch_KeyDown_UI(object sender, KeyEventArgs e)
        {
            bool visible = lstSubscriberSuggestions.Visible && lstSubscriberSuggestions.Items.Count > 0;

            if (visible)
            {
                if (e.KeyCode == Keys.Down)
                {
                    e.Handled = true; e.SuppressKeyPress = true;
                    lstSubscriberSuggestions.SelectedIndex = Math.Min(lstSubscriberSuggestions.SelectedIndex + 1, lstSubscriberSuggestions.Items.Count - 1);
                    return;
                }

                if (e.KeyCode == Keys.Up)
                {
                    e.Handled = true; e.SuppressKeyPress = true;
                    lstSubscriberSuggestions.SelectedIndex = Math.Max(lstSubscriberSuggestions.SelectedIndex - 1, 0);
                    return;
                }

                if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Tab)
                {
                    e.Handled = true; e.SuppressKeyPress = true;
                    SelectSubscriberFromList_UI();
                    return;
                }

                if (e.KeyCode == Keys.Escape)
                {
                    e.Handled = true; e.SuppressKeyPress = true;
                    lstSubscriberSuggestions.Visible = false;
                    return;
                }
            }
        }

        private void PositionSuggestionList()
        {
            if (!txtSubscriberSearch.IsHandleCreated) return;

            var screen = txtSubscriberSearch.Parent.PointToScreen(new System.Drawing.Point(txtSubscriberSearch.Left, txtSubscriberSearch.Bottom + 2));
            var client = PointToClient(screen);

            lstSubscriberSuggestions.Width = txtSubscriberSearch.Width;
            lstSubscriberSuggestions.Left = client.X;
            lstSubscriberSuggestions.Top = client.Y;

            int rows = Math.Min(8, Math.Max(1, lstSubscriberSuggestions.Items.Count));
            lstSubscriberSuggestions.Height = (rows * lstSubscriberSuggestions.ItemHeight) + 6;

            lstSubscriberSuggestions.BringToFront();
        }

        private void SelectSubscriberFromList_UI()
        {
            if (lstSubscriberSuggestions.SelectedItem == null) return;

            var item = lstSubscriberSuggestions.SelectedItem as SubscriberMeterSuggestion;
            if (item == null) return;

            _suppressSearch = true;
            try
            {
                SelectedSubscriberID = item.SubscriberID;
                SelectedMeterID = item.MeterID;

                txtSubscriberSearch.Text = item.DisplayText;
                txtSubscriberSearch.SelectionStart = txtSubscriberSearch.Text.Length;
                lstSubscriberSuggestions.Visible = false;
            }
            finally { _suppressSearch = false; }

            LoadSelectionData(item.SubscriberID, item.MeterID);
            LoadTariffForSubscriber(item.SubscriberID, dtpReadingDate.Value);

            txtCurrentReading.Focus();
            txtCurrentReading.SelectAll();
        }
    }
}
*/