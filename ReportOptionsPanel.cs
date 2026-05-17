using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace water3
{
    public partial class ReportOptionsPanel : UserControl
    {
        public event EventHandler SavePresetClicked;
        public event EventHandler LoadPresetClicked;
        public event EventHandler OptionsChanged;

        private TableLayoutPanel _root;
        private TableLayoutPanel _body;
        private FlowLayoutPanel _columnsFlow;
        private FlowLayoutPanel _buttonsFlow;
        private ComboBox _ddlSort;
        private CheckBox _chkDesc;
        private Button _btnSave;
        private Button _btnLoad;
        private Label _lblTitle;
        private Label _lblColumns;
        private Label _lblSort;

        private readonly List<CheckBox> _columnChecks = new List<CheckBox>();
        private readonly List<string> _allColumns = new List<string>();
        private bool _isApplying;

        public ReportOptionsPanel()
        {
            BuildUi();
        }

        private void BuildUi()
        {
            SuspendLayout();
            Controls.Clear();

            RightToLeft = RightToLeft.Yes;
            BackColor = Color.White;
            Padding = new Padding(8);
            MinimumSize = new Size(0, 96);

            _root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.White,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };
            _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            _root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _lblTitle = new Label
            {
                Dock = DockStyle.Fill,
                Text = "خيارات الكشف",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 54, 140),
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 4, 0)
            };

            _body = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 7,
                RowCount = 1,
                BackColor = Color.White,
                RightToLeft = RightToLeft.Yes,
                Padding = new Padding(0, 4, 0, 0),
                Margin = new Padding(0)
            };
            _body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 68F));   // الأعمدة
            _body.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));   // checkboxes
            _body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 78F));   // ترتيب حسب
            _body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));  // combo
            _body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 82F));   // تنازلي
            _body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 226F));  // buttons
            _body.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 1F));
            _body.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            _lblColumns = new Label
            {
                Dock = DockStyle.Fill,
                Text = "الأعمدة:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                TextAlign = ContentAlignment.TopRight,
                Padding = new Padding(0, 8, 4, 0)
            };

            _columnsFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = true,
                AutoScroll = true,
                BackColor = Color.White,
                Margin = new Padding(0, 0, 8, 0),
                Padding = new Padding(0)
            };

            _lblSort = new Label
            {
                Dock = DockStyle.Fill,
                Text = "ترتيب حسب:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                TextAlign = ContentAlignment.TopRight,
                Padding = new Padding(0, 8, 4, 0)
            };

            _ddlSort = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                Height = 30,
                Margin = new Padding(6, 4, 6, 0)
            };
            _ddlSort.SelectedIndexChanged += (s, e) => RaiseOptionsChanged();

            _chkDesc = new CheckBox
            {
                Dock = DockStyle.Top,
                Text = "تنازلي",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoSize = false,
                Height = 30,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 5, 0, 0),
                Margin = new Padding(0, 4, 0, 0)
            };
            _chkDesc.CheckedChanged += (s, e) => RaiseOptionsChanged();

            _buttonsFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                BackColor = Color.White,
                Margin = new Padding(0),
                Padding = new Padding(0, 2, 0, 0)
            };

            _btnSave = MakeButton("حفظ قالب", true, 106);
            _btnLoad = MakeButton("تحميل قالب", false, 106);
            _btnSave.Click += (s, e) => SavePresetClicked?.Invoke(this, EventArgs.Empty);
            _btnLoad.Click += (s, e) => LoadPresetClicked?.Invoke(this, EventArgs.Empty);
            _buttonsFlow.Controls.Add(_btnSave);
            _buttonsFlow.Controls.Add(_btnLoad);

            _body.Controls.Add(_lblColumns, 0, 0);
            _body.Controls.Add(_columnsFlow, 1, 0);
            _body.Controls.Add(_lblSort, 2, 0);
            _body.Controls.Add(_ddlSort, 3, 0);
            _body.Controls.Add(_chkDesc, 4, 0);
            _body.Controls.Add(_buttonsFlow, 5, 0);

            _root.Controls.Add(_lblTitle, 0, 0);
            _root.Controls.Add(_body, 0, 1);
            Controls.Add(_root);

            ResumeLayout(false);
        }

        private Button MakeButton(string text, bool primary, int width)
        {
            var b = new Button
            {
                Text = text,
                Width = width,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(6, 0, 0, 0),
                UseVisualStyleBackColor = false
            };

            if (primary)
            {
                b.BackColor = Color.FromArgb(0, 102, 204);
                b.ForeColor = Color.White;
                b.FlatAppearance.BorderSize = 0;
                b.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 86, 179);
            }
            else
            {
                b.BackColor = Color.White;
                b.ForeColor = Color.FromArgb(0, 102, 204);
                b.FlatAppearance.BorderColor = Color.FromArgb(0, 102, 204);
                b.FlatAppearance.BorderSize = 1;
                b.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 246, 255);
            }

            return b;
        }

        public void SetColumns(IEnumerable<string> columns, IEnumerable<string> defaultColumns)
        {
            _isApplying = true;
            try
            {
                _allColumns.Clear();
                if (columns != null)
                    _allColumns.AddRange(columns.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct());

                var defaults = new HashSet<string>(defaultColumns ?? _allColumns, StringComparer.OrdinalIgnoreCase);

                _columnsFlow.Controls.Clear();
                _columnChecks.Clear();

                foreach (string col in _allColumns)
                {
                    var chk = new CheckBox
                    {
                        Text = col,
                        Checked = defaults.Contains(col),
                        AutoSize = false,
                        Width = GetCheckWidth(col),
                        Height = 26,
                        Font = new Font("Segoe UI", 10F, FontStyle.Regular),
                        ForeColor = Color.FromArgb(33, 37, 41),
                        TextAlign = ContentAlignment.MiddleRight,
                        Margin = new Padding(8, 1, 0, 1),
                        RightToLeft = RightToLeft.Yes
                    };
                    chk.CheckedChanged += ColumnCheckChanged;
                    _columnChecks.Add(chk);
                    _columnsFlow.Controls.Add(chk);
                }
            }
            finally
            {
                _isApplying = false;
            }

            RaiseOptionsChanged();
        }

        private int GetCheckWidth(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 95;
            if (text.Length <= 6) return 92;
            if (text.Length <= 10) return 115;
            return 135;
        }

        private void ColumnCheckChanged(object sender, EventArgs e)
        {
            if (_isApplying) return;

            // لا تسمح بإخفاء كل الأعمدة حتى لا يصبح الجدول فارغًا
            if (_columnChecks.Count > 0 && !_columnChecks.Any(c => c.Checked))
            {
                var chk = sender as CheckBox;
                if (chk != null) chk.Checked = true;
                return;
            }

            RaiseOptionsChanged();
        }

        public void SetSortFields(IEnumerable<string> fields, string defaultField)
        {
            _isApplying = true;
            try
            {
                _ddlSort.Items.Clear();
                if (fields != null)
                {
                    foreach (string f in fields.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
                        _ddlSort.Items.Add(f);
                }

                if (!string.IsNullOrWhiteSpace(defaultField) && _ddlSort.Items.Contains(defaultField))
                    _ddlSort.SelectedItem = defaultField;
                else if (_ddlSort.Items.Count > 0)
                    _ddlSort.SelectedIndex = 0;
            }
            finally
            {
                _isApplying = false;
            }

            RaiseOptionsChanged();
        }

        public ReportOptions GetOptions()
        {
            var selected = _columnChecks
                .Where(c => c.Checked)
                .Select(c => c.Text)
                .ToList();

            if (selected.Count == 0)
                selected.AddRange(_allColumns);

            return new ReportOptions
            {
                SelectedColumns = selected,
                SortBy = _ddlSort.SelectedItem == null ? "" : _ddlSort.SelectedItem.ToString(),
                SortDesc = _chkDesc.Checked
            };
        }

        public void ApplyOptions(ReportOptions opt)
        {
            if (opt == null) return;

            _isApplying = true;
            try
            {
                var selected = new HashSet<string>(opt.SelectedColumns ?? new List<string>(), StringComparer.OrdinalIgnoreCase);
                if (selected.Count > 0)
                {
                    foreach (var chk in _columnChecks)
                        chk.Checked = selected.Contains(chk.Text);
                }

                if (!string.IsNullOrWhiteSpace(opt.SortBy) && _ddlSort.Items.Contains(opt.SortBy))
                    _ddlSort.SelectedItem = opt.SortBy;

                _chkDesc.Checked = opt.SortDesc;
            }
            finally
            {
                _isApplying = false;
            }

            RaiseOptionsChanged();
        }

        private void RaiseOptionsChanged()
        {
            if (_isApplying) return;
            OptionsChanged?.Invoke(this, EventArgs.Empty);
        }

        public class ReportOptions
        {
            public List<string> SelectedColumns { get; set; } = new List<string>();
            public string SortBy { get; set; } = "";
            public bool SortDesc { get; set; }
        }
    }
}

/*using System;
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
*/