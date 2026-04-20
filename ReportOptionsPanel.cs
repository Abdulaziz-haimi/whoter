using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace water3
{
    public partial class ReportOptionsPanel : UserControl
    {

            private GroupBox gb;
            private CheckedListBox clbColumns;
            private ComboBox ddlSortBy;
            private CheckBox chkSortDesc;

            private Button btnSavePreset;
            private Button btnLoadPreset;

            public event EventHandler OptionsChanged;
            public event EventHandler SavePresetClicked;
            public event EventHandler LoadPresetClicked;
             private bool _suspendEvents = false;

        public ReportOptionsPanel()
            {
                DoubleBuffered = true;
                BackColor = Color.White;
                BuildUi();
            }

            private void BuildUi()
            {
                gb = new GroupBox
                {
                    Dock = DockStyle.Fill,
                    Text = "خيارات التقرير",
                    Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 60, 120),
                    Padding = new Padding(10)
                };
                Controls.Add(gb);

                var layout = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 3,
                    RowCount = 2
                };

                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55f)); // columns
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30f)); // sort
                layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15f)); // buttons

                layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
                layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));

                gb.Controls.Add(layout);

                // Columns
                var pnlCols = new Panel { Dock = DockStyle.Fill };
                layout.Controls.Add(pnlCols, 0, 0);
                layout.SetRowSpan(pnlCols, 2);

                var lblCols = new Label
                {
                    Text = "الأعمدة:",
                    Dock = DockStyle.Top,
                    Height = 24,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold)
                };
                pnlCols.Controls.Add(lblCols);

                clbColumns = new CheckedListBox
                {
                    Dock = DockStyle.Fill,
                    CheckOnClick = true,
                    Font = new Font("Segoe UI", 10f),
                    IntegralHeight = false
                };
                pnlCols.Controls.Add(clbColumns);

                // Sort area
                var pnlSort = new Panel { Dock = DockStyle.Fill };
                layout.Controls.Add(pnlSort, 1, 0);

                var lblSort = new Label
                {
                    Text = "ترتيب حسب:",
                    Dock = DockStyle.Top,
                    Height = 24,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold)
                };
                pnlSort.Controls.Add(lblSort);

                ddlSortBy = new ComboBox
                {
                    Dock = DockStyle.Top,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Font = new Font("Segoe UI", 10f),
                    Height = 30
                };
                pnlSort.Controls.Add(ddlSortBy);

                chkSortDesc = new CheckBox
                {
                    Text = "تنازلي",
                    Dock = DockStyle.Top,
                    Height = 28,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold)
                };
                pnlSort.Controls.Add(chkSortDesc);

                // Buttons
                var pnlBtns = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    Padding = new Padding(0, 5, 0, 0)
                };
                layout.Controls.Add(pnlBtns, 2, 0);
                layout.SetRowSpan(pnlBtns, 2);

                btnSavePreset = new Button
                {
                    Text = "💾 حفظ قالب",
                    Width = 130,
                    Height = 32,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    BackColor = Color.FromArgb(0, 106, 204),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btnSavePreset.FlatAppearance.BorderSize = 0;

                btnLoadPreset = new Button
                {
                    Text = "📂 تحميل قالب",
                    Width = 130,
                    Height = 32,
                    Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                    BackColor = Color.White,
                    ForeColor = Color.FromArgb(0, 106, 204),
                    FlatStyle = FlatStyle.Flat
                };
                btnLoadPreset.FlatAppearance.BorderColor = Color.FromArgb(0, 106, 204);
                btnLoadPreset.FlatAppearance.BorderSize = 1;

                pnlBtns.Controls.Add(btnSavePreset);
                pnlBtns.Controls.Add(btnLoadPreset);

            // Events
            clbColumns.ItemCheck += (s, e) =>
            {
                if (_suspendEvents) return;

                void Raise() => OptionsChanged?.Invoke(this, EventArgs.Empty);

                if (IsHandleCreated)
                    BeginInvoke((Action)Raise);
                else
                    Raise();
            };

            ddlSortBy.SelectedIndexChanged += (s, e) => OptionsChanged?.Invoke(this, EventArgs.Empty);
                chkSortDesc.CheckedChanged += (s, e) => OptionsChanged?.Invoke(this, EventArgs.Empty);

                btnSavePreset.Click += (s, e) => SavePresetClicked?.Invoke(this, EventArgs.Empty);
                btnLoadPreset.Click += (s, e) => LoadPresetClicked?.Invoke(this, EventArgs.Empty);
            }

            // ===== Public API =====


            public void SetSortFields(IEnumerable<string> fields, string defaultField)
            {
                ddlSortBy.Items.Clear();
                foreach (var f in fields ?? Enumerable.Empty<string>())
                    ddlSortBy.Items.Add(f);

                if (!string.IsNullOrWhiteSpace(defaultField) && ddlSortBy.Items.Contains(defaultField))
                    ddlSortBy.SelectedItem = defaultField;
                else if (ddlSortBy.Items.Count > 0)
                    ddlSortBy.SelectedIndex = 0;
            }

            public ReportOptions GetOptions()
            {
                var opt = new ReportOptions
                {
                    SortBy = ddlSortBy.SelectedItem?.ToString() ?? "",
                    SortDesc = chkSortDesc.Checked
                };

                foreach (var item in clbColumns.CheckedItems)
                    opt.SelectedColumns.Add(item?.ToString() ?? "");

                return opt;
            }

        public void SetColumns(IEnumerable<string> columns, IEnumerable<string> defaultChecked)
        {
            _suspendEvents = true;
            try
            {
                clbColumns.Items.Clear();
                var def = new HashSet<string>(defaultChecked ?? Enumerable.Empty<string>());

                foreach (var c in columns ?? Enumerable.Empty<string>())
                    clbColumns.Items.Add(c, def.Contains(c));
            }
            finally
            {
                _suspendEvents = false;
            }
        }

        public void ApplyOptions(ReportOptions opt)
        {
            if (opt == null) return;

            _suspendEvents = true;
            try
            {
                if (!string.IsNullOrWhiteSpace(opt.SortBy) && ddlSortBy.Items.Contains(opt.SortBy))
                    ddlSortBy.SelectedItem = opt.SortBy;

                chkSortDesc.Checked = opt.SortDesc;

                for (int i = 0; i < clbColumns.Items.Count; i++)
                    clbColumns.SetItemChecked(i, false);

                var set = new HashSet<string>(opt.SelectedColumns ?? new List<string>());
                for (int i = 0; i < clbColumns.Items.Count; i++)
                {
                    var c = clbColumns.Items[i]?.ToString() ?? "";
                    if (set.Contains(c)) clbColumns.SetItemChecked(i, true);
                }
            }
            finally
            {
                _suspendEvents = false;
            }

            // بعد انتهاء التهيئة ارسل حدث واحد فقط
            OptionsChanged?.Invoke(this, EventArgs.Empty);
        }


        // ===== Nested DTO =====
        public class ReportOptions
            {
                public List<string> SelectedColumns { get; set; } = new List<string>();
                public string SortBy { get; set; } = "";
                public bool SortDesc { get; set; } = false;
            }
        }
    }
