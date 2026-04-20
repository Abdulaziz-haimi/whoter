using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using water3.Models;
using water3.Services;

namespace water3.Forms
{
    public partial class AccountStatementForm : Form
    {
        private const string FormKey = "AccountStatementForm";

        private readonly AccountStatementService _svc;

        private PrintDocument printDoc;
        private int _printRowIndex = 0;
        private int[] _colWidths;
        private int _pageNumber = 1;
        private bool _printedTotals = false;

        private bool _uiReady = false;
        private bool _suppressAuto = false;

        private readonly Color Primary = Color.FromArgb(0, 87, 183);
        private readonly Color PrimaryDark = Color.FromArgb(0, 70, 150);
        private readonly Color Hover = Color.FromArgb(15, 105, 205);
        private readonly Color Border = Color.FromArgb(225, 230, 235);

        public AccountStatementForm()
            : this(water3.Db.ConnectionString)
        {
        }

        public AccountStatementForm(string connectionString)
        {
            InitializeComponent();

            _svc = new AccountStatementService(connectionString, FormKey);

            ApplyTheme();
            InitPrinting();
            HookEvents();
            InitDynamicReportUi();

            ddlReportType.Items.Clear();
            ddlReportType.Items.AddRange(new object[] { "تفصيلي", "إجمالي", "فواتير فقط", "مدفوعات فقط" });
            ddlReportType.SelectedIndex = 0;

            ddlGroupBy.Items.Clear();
            ddlGroupBy.Items.AddRange(new object[] { "بدون", "شهري", "نوع المستند", "البند" });
            ddlGroupBy.SelectedIndex = 0;

            dtTo.Value = DateTime.Today;
            dtFrom.Value = DateTime.Today.AddDays(-30);

            ClearView();
            _uiReady = true;
        }

        private void ApplyTheme()
        {
            AddCardBorder(pnlFiltersCard);
            AddCardBorder(pnlInfoCard);
            AddCardBorder(pnlGridCard);
            AddCardBorder(pnlTotalsCard);

            StyleIconButton(btnApply, "تطبيق");
            StyleIconButton(btnRefresh, "تحديث");
            StyleIconButton(btnExportExcel, "تصدير CSV");
            StyleIconButton(btnPrint, "طباعة");
            StyleIconButton(btnSavePreset, "حفظ قالب");
            StyleIconButton(btnLoadPreset, "تحميل قالب");

            lblSubscriberInfo.ForeColor = PrimaryDark;
            lblSubscriberInfo.Font = new Font("Segoe UI", 10f, FontStyle.Bold);

            foreach (var lab in new[] { lblOpen, lblDebitTotal, lblCreditTotal, lblNet, lblClose })
            {
                lab.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                lab.ForeColor = Color.FromArgb(25, 65, 120);
            }

            StyleGrid(dgv);

            foreach (var lab in new[] { lblReportType, lblFrom, lblTo, lblSubscriber, lblColumns, lblSortBy, lblGroupBy })
            {
                lab.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                lab.ForeColor = Color.FromArgb(60, 60, 60);
            }

            chkSortDesc.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            chkOnlyInvoices.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            chkOnlyPayments.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            chkSortDesc.ForeColor = chkOnlyInvoices.ForeColor = chkOnlyPayments.ForeColor = Color.FromArgb(60, 60, 60);

            clbColumns.Font = new Font("Segoe UI", 9.5f);
        }

        private void AddCardBorder(Panel p)
        {
            p.Paint += (s, e) =>
            {
                using (var pen = new Pen(Border))
                    e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            };
        }

        private void StyleIconButton(Button b, string tooltip)
        {
            b.BackColor = Primary;
            b.ForeColor = Color.White;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.Font = new Font("Segoe UI Emoji", 12f);
            b.Cursor = Cursors.Hand;

            b.MouseEnter += (s, e) => b.BackColor = Hover;
            b.MouseLeave += (s, e) => b.BackColor = Primary;

            var tt = new ToolTip();
            tt.SetToolTip(b, tooltip);
        }

        private void StyleGrid(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Primary;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 232, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;

            grid.RowsDefaultCellStyle.BackColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 247, 255);
            grid.GridColor = Color.FromArgb(235, 240, 245);

            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 40;
            grid.RowTemplate.Height = 36;
            grid.BorderStyle = BorderStyle.None;
        }

        private void HookEvents()
        {
            btnApply.Click += (s, e) => ApplyReportSafe();
            btnRefresh.Click += (s, e) => LoadSubscribers(ddlSubscribers.Text ?? "", true);
            btnExportExcel.Click += (s, e) => ExportToExcelCsv();
            btnPrint.Click += (s, e) => PrintGrid();

            btnSavePreset.Click += (s, e) => SavePreset();
            btnLoadPreset.Click += (s, e) => LoadPreset();

            chkOnlyInvoices.CheckedChanged += (s, e) =>
            {
                if (chkOnlyInvoices.Checked)
                    chkOnlyPayments.Checked = false;
            };

            chkOnlyPayments.CheckedChanged += (s, e) =>
            {
                if (chkOnlyPayments.Checked)
                    chkOnlyInvoices.Checked = false;
            };

            ddlSubscribers.TextUpdate += (s, e) =>
            {
                if (_suppressAuto) return;

                string key = (ddlSubscribers.Text ?? "").Trim();
                if (key.Length == 0)
                {
                    ddlSubscribers.Items.Clear();
                    return;
                }

                LoadSubscribers(key, true);
                ddlSubscribers.DroppedDown = ddlSubscribers.Items.Count > 0;
                ddlSubscribers.SelectionStart = ddlSubscribers.Text.Length;
            };

            ddlSubscribers.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;

                    if (!(ddlSubscribers.SelectedItem is ComboBoxItem) && ddlSubscribers.Items.Count > 0)
                        ddlSubscribers.SelectedIndex = 0;

                    ApplyReportSafe();
                }
            };
        }

        private void InitDynamicReportUi()
        {
            clbColumns.Items.Clear();

            var colsOrder = new List<string>
            {
                "التاريخ","البيان","البند","نوع المستند","رقم المستند","العداد",
                "مدين","دائن", AccountStatementService.RunningBalanceCol,
                "رقم الفاتورة","رقم السداد","رقم القيد"
            };

            foreach (var c in colsOrder)
            {
                bool isDefault =
                    c == "التاريخ" ||
                    c == "البيان" ||
                    c == "مدين" ||
                    c == "دائن" ||
                    c == AccountStatementService.RunningBalanceCol;

                clbColumns.Items.Add(c, isDefault);
            }

            ddlSortBy.Items.Clear();
            foreach (var k in AccountStatementService.AllowedSort.Keys)
                ddlSortBy.Items.Add(k);

            ddlSortBy.SelectedItem = "التاريخ";
        }

        private void ClearView()
        {
            _suppressAuto = true;
            try
            {
                ddlSubscribers.Text = "";
                ddlSubscribers.Items.Clear();
            }
            finally
            {
                _suppressAuto = false;
            }

            dgv.DataSource = null;
            lblSubscriberInfo.Text = "";
            SetTotals(0, 0, 0, 0, 0);
        }

        private void ApplyReportSafe()
        {
            if (!_uiReady) return;

            if (dtFrom.Value.Date > dtTo.Value.Date)
            {
                MessageBox.Show("تاريخ (من) يجب أن يكون أقل أو يساوي (إلى).");
                return;
            }

            if (!(ddlSubscribers.SelectedItem is ComboBoxItem))
            {
                string typed = (ddlSubscribers.Text ?? "").Trim();
                if (typed.Length > 0)
                    LoadSubscribers(typed, true);

                if (!(ddlSubscribers.SelectedItem is ComboBoxItem) && ddlSubscribers.Items.Count > 0)
                    ddlSubscribers.SelectedIndex = 0;
            }

            if (!(ddlSubscribers.SelectedItem is ComboBoxItem item))
            {
                MessageBox.Show("اختر المشترك من القائمة المنسدلة أو اكتب الاسم ثم اضغط Enter.");
                return;
            }

            if (!int.TryParse(item.Value, out int sid))
            {
                MessageBox.Show("قيمة المشترك غير صحيحة.");
                return;
            }

            ShowReport(sid, dtFrom.Value.Date, dtTo.Value.Date, ddlReportType.Text);
        }

        private void LoadSubscribers(string searchKey, bool keepTypedText)
        {
            _suppressAuto = true;
            try
            {
                string typed = keepTypedText ? (ddlSubscribers.Text ?? "") : "";
                int caret = keepTypedText ? ddlSubscribers.SelectionStart : 0;

                ddlSubscribers.BeginUpdate();
                ddlSubscribers.Items.Clear();

                var items = _svc.SearchSubscribers(searchKey);
                foreach (var x in items)
                    ddlSubscribers.Items.Add(x);

                ddlSubscribers.EndUpdate();

                if (keepTypedText)
                {
                    ddlSubscribers.Text = typed;
                    ddlSubscribers.SelectionStart = Math.Min(caret, ddlSubscribers.Text.Length);
                    ddlSubscribers.SelectionLength = 0;
                }

                if (ddlSubscribers.Items.Count > 0 && ddlSubscribers.SelectedIndex < 0)
                    ddlSubscribers.SelectedIndex = 0;
            }
            finally
            {
                _suppressAuto = false;
            }
        }

        private void ShowReport(int subscriberId, DateTime fromDate, DateTime toDate, string reportType)
        {
            var opt = ReadOptionsFromUi(subscriberId);
            var result = _svc.GetReport(subscriberId, fromDate, toDate, reportType, opt);

            if (result.Info == null)
            {
                dgv.DataSource = null;
                lblSubscriberInfo.Text = "";
                SetTotals(0, 0, 0, 0, 0);
                return;
            }

            lblSubscriberInfo.Text =
                $"المشترك: {result.Info.Name} | العداد: {result.Info.MeterNumber} | الهاتف: {result.Info.Phone} | العنوان: {result.Info.Address}";

            SetTotals(result.Opening, result.TotalDebit, result.TotalCredit, result.Net, result.Closing);

            dgv.DataSource = result.Data;

            if (dgv.Columns.Contains("StatementID"))
                dgv.Columns["StatementID"].Visible = false;

            if (dgv.Columns.Contains("مدين"))
                dgv.Columns["مدين"].DefaultCellStyle.ForeColor = Color.DarkRed;

            if (dgv.Columns.Contains("دائن"))
                dgv.Columns["دائن"].DefaultCellStyle.ForeColor = Color.DarkGreen;

            if (dgv.Columns.Contains(AccountStatementService.RunningBalanceCol))
                dgv.Columns[AccountStatementService.RunningBalanceCol].DefaultCellStyle.ForeColor = Color.DarkBlue;

            bool isSummary = (reportType == "إجمالي") || (opt.GroupBy != "بدون");
            if (!isSummary && dgv.Columns.Contains("مدين"))
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;

                    var val = row.Cells["مدين"].Value;
                    if (val == null || val == DBNull.Value) continue;

                    if (decimal.TryParse(val.ToString(), out decimal debitVal) && debitVal > 0)
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 232);
                }
            }
        }

        private ReportOptions ReadOptionsFromUi(int subscriberId)
        {
            var opt = new ReportOptions
            {
                SubscriberId = subscriberId,
                From = dtFrom.Value.Date,
                To = dtTo.Value.Date,
                SortBy = ddlSortBy.SelectedItem?.ToString() ?? "التاريخ",
                SortDesc = chkSortDesc.Checked,
                OnlyInvoices = chkOnlyInvoices.Checked,
                OnlyPayments = chkOnlyPayments.Checked,
                GroupBy = ddlGroupBy.SelectedItem?.ToString() ?? "بدون"
            };

            foreach (var item in clbColumns.CheckedItems)
                opt.SelectedColumns.Add(item.ToString());

            if (opt.SelectedColumns.Count == 0)
            {
                opt.SelectedColumns.AddRange(new[]
                {
                    "التاريخ",
                    "البيان",
                    "مدين",
                    "دائن",
                    AccountStatementService.RunningBalanceCol
                });
            }

            return opt;
        }

        private void SetTotals(decimal opening, decimal debit, decimal credit, decimal net, decimal closing)
        {
            lblOpen.Text = $"افتتاحي: {opening:N2}";
            lblDebitTotal.Text = $"إجمالي مدين: {debit:N2}";
            lblCreditTotal.Text = $"إجمالي دائن: {credit:N2}";
            lblNet.Text = $"الصافي: {net:N2}";
            lblClose.Text = $"ختامي: {closing:N2}";
        }

        private void ExportToExcelCsv()
        {
            if (dgv.DataSource == null || dgv.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للتصدير.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel CSV (*.csv)|*.csv";
                sfd.FileName = $"AccountStatement_{DateTime.Now:yyyyMMdd_HHmm}.csv";

                if (sfd.ShowDialog() != DialogResult.OK)
                    return;

                try
                {
                    var sb = new StringBuilder();

                    sb.AppendLine(EscapeCsv($"كشف حساب المشترك - {ddlReportType.Text}"));
                    sb.AppendLine(EscapeCsv($"الفترة: {dtFrom.Value:yyyy/MM/dd} إلى {dtTo.Value:yyyy/MM/dd}"));
                    sb.AppendLine(EscapeCsv(lblSubscriberInfo.Text));
                    sb.AppendLine();

                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        if (!dgv.Columns[i].Visible) continue;
                        sb.Append(EscapeCsv(dgv.Columns[i].HeaderText));
                        sb.Append(",");
                    }
                    sb.AppendLine();

                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;

                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            if (!dgv.Columns[i].Visible) continue;
                            var val = row.Cells[i].Value?.ToString() ?? "";
                            sb.Append(EscapeCsv(val));
                            sb.Append(",");
                        }
                        sb.AppendLine();
                    }

                    sb.AppendLine();
                    sb.AppendLine(EscapeCsv(lblOpen.Text));
                    sb.AppendLine(EscapeCsv(lblDebitTotal.Text));
                    sb.AppendLine(EscapeCsv(lblCreditTotal.Text));
                    sb.AppendLine(EscapeCsv(lblNet.Text));
                    sb.AppendLine(EscapeCsv(lblClose.Text));

                    File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(true));
                    MessageBox.Show("تم التصدير بنجاح ✅");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("فشل التصدير: " + ex.Message);
                }
            }
        }

        private string EscapeCsv(string s)
        {
            if (s == null) return "";
            if (s.Contains(",") || s.Contains("\"") || s.Contains("\n"))
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        private void InitPrinting()
        {
            printDoc = new PrintDocument();
            printDoc.PrintPage += PrintDoc_PrintPage;
        }

        private void PrintGrid()
        {
            if (dgv.DataSource == null || dgv.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للطباعة.");
                return;
            }

            using (PrintPreviewDialog preview = new PrintPreviewDialog())
            {
                _printRowIndex = 0;
                _pageNumber = 1;
                _printedTotals = false;
                _colWidths = null;

                preview.Document = printDoc;
                preview.Width = 1100;
                preview.Height = 800;
                preview.ShowDialog();
            }
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            var margin = e.MarginBounds;
            int y = margin.Top;

            string title = $"كشف حساب المشترك - {ddlReportType.Text} | الفترة: {dtFrom.Value:yyyy/MM/dd} إلى {dtTo.Value:yyyy/MM/dd}";
            using (var fTitle = new Font("Segoe UI", 12, FontStyle.Bold))
            {
                g.DrawString(title, fTitle, Brushes.Black, margin.Left, y);
                y += 30;
            }

            using (var fInfo = new Font("Segoe UI", 10, FontStyle.Regular))
            {
                g.DrawString(lblSubscriberInfo.Text, fInfo, Brushes.Black, margin.Left, y);
                y += 25;
            }

            var visibleCols = GetVisibleColumns();
            if (_colWidths == null || _colWidths.Length != visibleCols.Length)
                _colWidths = CalcColumnWidths(margin.Width, visibleCols);

            int x = margin.Left;
            int headerHeight = 28;

            using (var headerBrush = new SolidBrush(Primary))
                g.FillRectangle(headerBrush, new Rectangle(margin.Left, y, margin.Width, headerHeight));

            using (var fHeader = new Font("Segoe UI", 10, FontStyle.Bold))
            using (var bHeaderText = new SolidBrush(Color.White))
            using (var pGrid = new Pen(Color.LightGray))
            {
                for (int c = 0; c < visibleCols.Length; c++)
                {
                    int w = _colWidths[c];
                    var rect = new Rectangle(x, y, w, headerHeight);
                    g.DrawRectangle(pGrid, rect);
                    g.DrawString(visibleCols[c].HeaderText, fHeader, bHeaderText, rect, CenterFormat());
                    x += w;
                }
            }

            y += headerHeight;
            int rowHeight = 26;

            using (var fRow = new Font("Segoe UI", 9, FontStyle.Regular))
            using (var pGrid = new Pen(Color.LightGray))
            {
                while (_printRowIndex < dgv.Rows.Count)
                {
                    var row = dgv.Rows[_printRowIndex];
                    if (row.IsNewRow)
                    {
                        _printRowIndex++;
                        continue;
                    }

                    if (y + rowHeight > margin.Bottom)
                    {
                        e.HasMorePages = true;
                        _pageNumber++;
                        return;
                    }

                    x = margin.Left;
                    for (int c = 0; c < visibleCols.Length; c++)
                    {
                        int w = _colWidths[c];
                        var rect = new Rectangle(x, y, w, rowHeight);
                        g.DrawRectangle(pGrid, rect);

                        var val = row.Cells[visibleCols[c].Name].Value?.ToString() ?? "";
                        g.DrawString(val, fRow, Brushes.Black, rect, CenterFormat());
                        x += w;
                    }

                    y += rowHeight;
                    _printRowIndex++;
                }
            }

            if (!_printedTotals)
            {
                int need = 80;
                if (y + need > margin.Bottom)
                {
                    e.HasMorePages = true;
                    _pageNumber++;
                    return;
                }

                y += 10;
                using (var pen = new Pen(Color.Gray))
                    g.DrawLine(pen, margin.Left, y, margin.Right, y);

                y += 10;
                using (var fTotTitle = new Font("Segoe UI", 11, FontStyle.Bold))
                    g.DrawString("الإجماليات:", fTotTitle, Brushes.Black, margin.Left, y);

                y += 22;
                using (var fTot = new Font("Segoe UI", 10, FontStyle.Regular))
                {
                    g.DrawString(lblOpen.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
                    g.DrawString(lblDebitTotal.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
                    g.DrawString(lblCreditTotal.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
                    g.DrawString(lblNet.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
                    g.DrawString(lblClose.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
                }

                _printedTotals = true;
            }

            using (var fFooter = new Font("Segoe UI", 9, FontStyle.Italic))
                g.DrawString("صفحة: " + _pageNumber, fFooter, Brushes.Gray, margin.Left, margin.Bottom + 10);

            e.HasMorePages = false;
            _printRowIndex = 0;
            _colWidths = null;
            _printedTotals = false;
        }

        private StringFormat CenterFormat()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };
        }

        private DataGridViewColumn[] GetVisibleColumns()
        {
            var list = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn c in dgv.Columns)
                if (c.Visible) list.Add(c);

            return list.ToArray();
        }

        private int[] CalcColumnWidths(int totalWidth, DataGridViewColumn[] cols)
        {
            float totalWeight = 0;
            foreach (var c in cols)
                totalWeight += (c.FillWeight > 0 ? c.FillWeight : 100);

            int[] widths = new int[cols.Length];
            int used = 0;

            for (int i = 0; i < cols.Length; i++)
            {
                float w = (cols[i].FillWeight > 0 ? cols[i].FillWeight : 100);
                widths[i] = (int)Math.Floor(totalWidth * (w / totalWeight));
                used += widths[i];
            }

            int diff = totalWidth - used;
            if (diff != 0 && widths.Length > 0)
                widths[0] += diff;

            return widths;
        }

        private void SavePreset()
        {
            string name = PromptDialog.Show("حفظ قالب", "اسم القالب:", "قالب 1");
            name = (name ?? "").Trim();
            if (name.Length == 0) return;

            var opt = new ReportOptions
            {
                From = dtFrom.Value.Date,
                To = dtTo.Value.Date,
                SortBy = ddlSortBy.SelectedItem?.ToString() ?? "التاريخ",
                SortDesc = chkSortDesc.Checked,
                OnlyInvoices = chkOnlyInvoices.Checked,
                OnlyPayments = chkOnlyPayments.Checked,
                GroupBy = ddlGroupBy.SelectedItem?.ToString() ?? "بدون"
            };

            foreach (var item in clbColumns.CheckedItems)
                opt.SelectedColumns.Add(item.ToString());

            _svc.SavePreset(name, opt);
            MessageBox.Show("تم حفظ القالب ✅");
        }

        private void LoadPreset()
        {
            DataTable dt = _svc.GetPresetsList();

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد قوالب محفوظة.");
                return;
            }

            var sb = new StringBuilder();
            foreach (DataRow r in dt.Rows)
                sb.AppendLine($"{r["PresetID"]}: {r["PresetName"]}");

            string pick = PromptDialog.Show(
                "تحميل قالب",
                "اختر رقم القالب (PresetID):\n\n" + sb,
                dt.Rows[0]["PresetID"].ToString()
            );

            if (!int.TryParse((pick ?? "").Trim(), out int presetId))
                return;

            var opt = _svc.LoadPresetOptions(presetId);
            if (opt == null)
            {
                MessageBox.Show("تعذر تحميل القالب.");
                return;
            }

            ApplyOptionsToUi(opt);
            MessageBox.Show("تم تحميل القالب ✅");
        }

        private void ApplyOptionsToUi(ReportOptions opt)
        {
            if (opt == null) return;

            if (opt.From != default(DateTime)) dtFrom.Value = opt.From;
            if (opt.To != default(DateTime)) dtTo.Value = opt.To;

            if (!string.IsNullOrWhiteSpace(opt.SortBy) && ddlSortBy.Items.Contains(opt.SortBy))
                ddlSortBy.SelectedItem = opt.SortBy;

            chkSortDesc.Checked = opt.SortDesc;

            if (!string.IsNullOrWhiteSpace(opt.GroupBy) && ddlGroupBy.Items.Contains(opt.GroupBy))
                ddlGroupBy.SelectedItem = opt.GroupBy;

            chkOnlyInvoices.Checked = opt.OnlyInvoices;
            chkOnlyPayments.Checked = opt.OnlyPayments;

            for (int i = 0; i < clbColumns.Items.Count; i++)
                clbColumns.SetItemChecked(i, false);

            var set = new HashSet<string>(opt.SelectedColumns ?? new List<string>());
            for (int i = 0; i < clbColumns.Items.Count; i++)
            {
                string c = clbColumns.Items[i].ToString();
                if (set.Contains(c))
                    clbColumns.SetItemChecked(i, true);
            }
        }

        private static class PromptDialog
        {
            public static string Show(string title, string message, string defaultValue = "")
            {
                using (Form form = new Form())
                using (Label lbl = new Label())
                using (TextBox txt = new TextBox())
                using (Button btnOk = new Button())
                using (Button btnCancel = new Button())
                {
                    form.Text = title;
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.MinimizeBox = false;
                    form.MaximizeBox = false;
                    form.ClientSize = new Size(520, 150);
                    form.RightToLeft = RightToLeft.Yes;
                    form.RightToLeftLayout = true;

                    lbl.AutoSize = false;
                    lbl.Text = message;
                    lbl.SetBounds(12, 12, 496, 40);

                    txt.Text = defaultValue ?? "";
                    txt.SetBounds(12, 55, 496, 28);

                    btnOk.Text = "موافق";
                    btnOk.DialogResult = DialogResult.OK;
                    btnOk.SetBounds(320, 95, 90, 30);

                    btnCancel.Text = "إلغاء";
                    btnCancel.DialogResult = DialogResult.Cancel;
                    btnCancel.SetBounds(418, 95, 90, 30);

                    form.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });
                    form.AcceptButton = btnOk;
                    form.CancelButton = btnCancel;

                    return form.ShowDialog() == DialogResult.OK ? txt.Text : null;
                }
            }
        }
    }
}
/*using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using water3.Models;
using water3.Services;

namespace water3.Forms
{
 
        public partial class AccountStatementForm : Form
    {
        private const string FormKey = "AccountStatementForm";
         private readonly AccountStatementService _svc;
        //private CollectorService _svc;
        private PrintDocument printDoc;
        private int _printRowIndex = 0;
        private int[] _colWidths;
        private int _pageNumber = 1;
        private bool _printedTotals = false;

        private bool _uiReady = false;
        private bool _suppressAuto = false;

        // ثيم (نستخدمه في Style)
        private readonly Color Primary = Color.FromArgb(0, 87, 183);
        private readonly Color PrimaryDark = Color.FromArgb(0, 70, 150);
        private readonly Color Hover = Color.FromArgb(15, 105, 205);
        private readonly Color Border = Color.FromArgb(225, 230, 235);
            public AccountStatementForm()
                : this(water3.Db.ConnectionString) // Call the existing constructor with a default connection string
            { }
            public AccountStatementForm(string connectionString)
            {
                InitializeComponent();
                _svc = new AccountStatementService(connectionString, FormKey);

                ApplyTheme();      // تلوين وتنسيق الأزرار والجريد
                InitPrinting();
                HookEvents();
                InitDynamicReportUi();
                // قيم افتراضية
                ddlReportType.Items.Clear();
            ddlReportType.Items.AddRange(new object[] { "تفصيلي", "إجمالي", "فواتير فقط", "مدفوعات فقط" });
            ddlReportType.SelectedIndex = 0;

            ddlGroupBy.Items.Clear();
            ddlGroupBy.Items.AddRange(new object[] { "بدون", "شهري", "نوع المستند", "البند" });
            ddlGroupBy.SelectedIndex = 0;

            dtTo.Value = DateTime.Today;
            dtFrom.Value = DateTime.Today.AddDays(-30);

            ClearView();
            _uiReady = true;
        }

        private void ApplyTheme()
        {
            // كروت بحد بسيط
            AddCardBorder(pnlFiltersCard);
            AddCardBorder(pnlInfoCard);
            AddCardBorder(pnlGridCard);
            AddCardBorder(pnlTotalsCard);

            // أزرار الأيقونات
            StyleIconButton(btnApply, "تطبيق");
            StyleIconButton(btnRefresh, "تحديث");
            StyleIconButton(btnExportExcel, "تصدير CSV");
            StyleIconButton(btnPrint, "طباعة");
            StyleIconButton(btnSavePreset, "حفظ قالب");
            StyleIconButton(btnLoadPreset, "تحميل قالب");

            // معلومات المشترك
            lblSubscriberInfo.ForeColor = PrimaryDark;
            lblSubscriberInfo.Font = new Font("Segoe UI", 10f, FontStyle.Bold);

            // الإجماليات
            foreach (var lab in new[] { lblOpen, lblDebitTotal, lblCreditTotal, lblNet, lblClose })
            {
                lab.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                lab.ForeColor = Color.FromArgb(25, 65, 120);
            }

            // الجريد
            StyleGrid(dgv);

            // تسميات الحقول
            foreach (var lab in new[] { lblReportType, lblFrom, lblTo, lblSubscriber, lblColumns, lblSortBy, lblGroupBy })
            {
                lab.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                lab.ForeColor = Color.FromArgb(60, 60, 60);
            }

            // خيارات
            chkSortDesc.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            chkOnlyInvoices.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            chkOnlyPayments.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            chkSortDesc.ForeColor = chkOnlyInvoices.ForeColor = chkOnlyPayments.ForeColor = Color.FromArgb(60, 60, 60);

            clbColumns.Font = new Font("Segoe UI", 9.5f);
        }

        private void AddCardBorder(Panel p)
        {
            p.Paint += (s, e) =>
            {
                using (var pen = new Pen(Border))
                    e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            };
        }

        private void StyleIconButton(Button b, string tooltip)
        {
            b.BackColor = Primary;
            b.ForeColor = Color.White;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 0;
            b.Font = new Font("Segoe UI Emoji", 12f);
            b.Cursor = Cursors.Hand;

            b.MouseEnter += (s, e) => b.BackColor = Hover;
            b.MouseLeave += (s, e) => b.BackColor = Primary;

            var tt = new ToolTip();
            tt.SetToolTip(b, tooltip);
        }

        private void StyleGrid(DataGridView grid)
        {
            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Primary;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 232, 255);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;

            grid.RowsDefaultCellStyle.BackColor = Color.White;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 247, 255);
            grid.GridColor = Color.FromArgb(235, 240, 245);

            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 40;
            grid.RowTemplate.Height = 36;
            grid.BorderStyle = BorderStyle.None;
        }

        // ===== Events =====
        private void HookEvents()
        {
            btnApply.Click += (s, e) => ApplyReportSafe();

            btnRefresh.Click += (s, e) =>
                LoadSubscribers(ddlSubscribers.Text ?? "", keepTypedText: true);

            btnExportExcel.Click += (s, e) => ExportToExcelCsv();
            btnPrint.Click += (s, e) => PrintGrid();

            chkOnlyInvoices.CheckedChanged += (s, e) =>
            {
                if (chkOnlyInvoices.Checked) chkOnlyPayments.Checked = false;
            };
            chkOnlyPayments.CheckedChanged += (s, e) =>
            {
                if (chkOnlyPayments.Checked) chkOnlyInvoices.Checked = false;
            };

            btnSavePreset.Click += (s, e) => SavePreset();
            btnLoadPreset.Click += (s, e) => LoadPreset();

            ddlSubscribers.TextUpdate += (s, e) =>
            {
                if (_suppressAuto) return;

                string key = (ddlSubscribers.Text ?? "").Trim();
                if (key.Length == 0)
                {
                    ddlSubscribers.Items.Clear();
                    return;
                }

                LoadSubscribers(key, keepTypedText: true);
                ddlSubscribers.DroppedDown = ddlSubscribers.Items.Count > 0;
            };

            ddlSubscribers.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    if (!(ddlSubscribers.SelectedItem is ComboBoxItem) && ddlSubscribers.Items.Count > 0)
                        ddlSubscribers.SelectedIndex = 0;

                    ApplyReportSafe();
                }
            };
        }

        // ===== UI init =====
        private void InitDynamicReportUi()
        {
            clbColumns.Items.Clear();

            var colsOrder = new List<string>
            {
                "التاريخ","البيان","البند","نوع المستند","رقم المستند","العداد",
                "مدين","دائن", AccountStatementService.RunningBalanceCol,
                "رقم الفاتورة","رقم السداد","رقم القيد"
            };

            foreach (var c in colsOrder)
            {
                bool isDefault = c == "التاريخ" || c == "البيان" || c == "مدين" || c == "دائن" || c == AccountStatementService.RunningBalanceCol;
                clbColumns.Items.Add(c, isDefault);
            }

            ddlSortBy.Items.Clear();
            foreach (var k in AccountStatementService.AllowedSort.Keys)
                ddlSortBy.Items.Add(k);

            ddlSortBy.SelectedItem = "التاريخ";
        }

        private void ClearView()
        {
            _suppressAuto = true;
            try
            {
                ddlSubscribers.Text = "";
                ddlSubscribers.Items.Clear();
            }
            finally { _suppressAuto = false; }

            dgv.DataSource = null;
            lblSubscriberInfo.Text = "";
            SetTotals(0, 0, 0, 0, 0);
        }
        private void ApplyReportSafe()
        {
            if (!_uiReady) return;

            if (dtFrom.Value.Date > dtTo.Value.Date)
            {
                MessageBox.Show("تاريخ (من) يجب أن يكون أقل أو يساوي (إلى).");
                return;
            }

            if (!(ddlSubscribers.SelectedItem is ComboBoxItem it))
            {
                if (ddlSubscribers.Items.Count > 0)
                    ddlSubscribers.SelectedIndex = 0;
            }

            if (!(ddlSubscribers.SelectedItem is ComboBoxItem item))
            {
                MessageBox.Show("اختر المشترك من القائمة المنسدلة أو اكتب ثم اضغط Enter.");
                return;
            }

            if (!int.TryParse(item.Value, out int sid))
            {
                MessageBox.Show("قيمة المشترك غير صحيحة.");
                return;
            }

            ShowReport(sid, dtFrom.Value.Date, dtTo.Value.Date, ddlReportType.Text);
        }

        private void LoadSubscribers(string searchKey, bool keepTypedText)
        {
            _suppressAuto = true;
            try
            {
                string typed = keepTypedText ? (ddlSubscribers.Text ?? "") : "";
                int caret = keepTypedText ? ddlSubscribers.SelectionStart : 0;

                ddlSubscribers.BeginUpdate();
                ddlSubscribers.Items.Clear();

                var items = _svc.SearchSubscribers(searchKey);
                foreach (var x in items)
                    ddlSubscribers.Items.Add(x);

                ddlSubscribers.EndUpdate();

                if (keepTypedText)
                {
                    ddlSubscribers.Text = typed;
                    ddlSubscribers.SelectionStart = Math.Min(caret, ddlSubscribers.Text.Length);
                    ddlSubscribers.SelectionLength = 0;
                }

                if (ddlSubscribers.Items.Count > 0 && ddlSubscribers.SelectedIndex < 0)
                    ddlSubscribers.SelectedIndex = 0;
            }
            finally
            {
                _suppressAuto = false;
            }
        }

        private void ShowReport(int subscriberId, DateTime fromDate, DateTime toDate, string reportType)
        {
            // اقرأ الخيارات من الواجهة ثم مررها للخدمة
            var opt = ReadOptionsFromUi(subscriberId);

            var result = _svc.GetReport(subscriberId, fromDate, toDate, reportType, opt);

            if (result.Info == null)
            {
                dgv.DataSource = null;
                lblSubscriberInfo.Text = "";
                SetTotals(0, 0, 0, 0, 0);
                return;
            }

            lblSubscriberInfo.Text =
                $"المشترك: {result.Info.Name} | العداد: {result.Info.MeterNumber} | الهاتف: {result.Info.Phone} | العنوان: {result.Info.Address}";

            SetTotals(result.Opening, result.TotalDebit, result.TotalCredit, result.Net, result.Closing);

            dgv.DataSource = result.Data;

            // تنسيق أعمدة
            if (dgv.Columns.Contains("StatementID"))
                dgv.Columns["StatementID"].Visible = false;

            if (dgv.Columns.Contains("مدين"))
                dgv.Columns["مدين"].DefaultCellStyle.ForeColor = Color.DarkRed;

            if (dgv.Columns.Contains("دائن"))
                dgv.Columns["دائن"].DefaultCellStyle.ForeColor = Color.DarkGreen;

            if (dgv.Columns.Contains(AccountStatementService.RunningBalanceCol))
                dgv.Columns[AccountStatementService.RunningBalanceCol].DefaultCellStyle.ForeColor = Color.DarkBlue;

            // تلوين صفوف المدين (نفس سلوكك)
            bool isSummary = (reportType == "إجمالي") || (opt.GroupBy != "بدون");
            if (!isSummary && dgv.Columns.Contains("مدين"))
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;

                    var val = row.Cells["مدين"].Value;
                    if (val == null || val == DBNull.Value) continue;

                    if (decimal.TryParse(val.ToString(), out decimal debitVal) && debitVal > 0)
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 232);
                }
            }
        }
            private ReportOptions ReadOptionsFromUi(int subscriberId)
        {
            var opt = new ReportOptions
            {
                SubscriberId = subscriberId,
                From = dtFrom.Value.Date,
                To = dtTo.Value.Date,
                SortBy = ddlSortBy.SelectedItem?.ToString() ?? "التاريخ",
                SortDesc = chkSortDesc.Checked,
                OnlyInvoices = chkOnlyInvoices.Checked,
                OnlyPayments = chkOnlyPayments.Checked,
                GroupBy = ddlGroupBy.SelectedItem?.ToString() ?? "بدون",
            };

            foreach (var item in clbColumns.CheckedItems)
                opt.SelectedColumns.Add(item.ToString());

            if (opt.SelectedColumns.Count == 0)
                opt.SelectedColumns.AddRange(new[] { "التاريخ", "البيان", "مدين", "دائن", AccountStatementService.RunningBalanceCol });

            return opt;
        }

        // ===== Totals =====
        private void SetTotals(decimal opening, decimal debit, decimal credit, decimal net, decimal closing)
        {
            lblOpen.Text = $"افتتاحي: {opening:N2}";
            lblDebitTotal.Text = $"إجمالي مدين: {debit:N2}";
            lblCreditTotal.Text = $"إجمالي دائن: {credit:N2}";
            lblNet.Text = $"الصافي: {net:N2}";
            lblClose.Text = $"ختامي: {closing:N2}";
        }

        // ===== Export =====
        private void ExportToExcelCsv()
        {
            if (dgv.DataSource == null || dgv.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للتصدير.");
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Excel CSV (*.csv)|*.csv";
                sfd.FileName = $"AccountStatement_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var sb = new StringBuilder();

                    sb.AppendLine(EscapeCsv($"كشف حساب المشترك - {ddlReportType.Text}"));
                    sb.AppendLine(EscapeCsv($"الفترة: {dtFrom.Value:yyyy/MM/dd} إلى {dtTo.Value:yyyy/MM/dd}"));
                    sb.AppendLine(EscapeCsv(lblSubscriberInfo.Text));
                    sb.AppendLine();

                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        if (!dgv.Columns[i].Visible) continue;
                        sb.Append(EscapeCsv(dgv.Columns[i].HeaderText));
                        sb.Append(",");
                    }
                    sb.AppendLine();

                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;

                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            if (!dgv.Columns[i].Visible) continue;
                            var val = row.Cells[i].Value?.ToString() ?? "";
                            sb.Append(EscapeCsv(val));
                            sb.Append(",");
                        }
                        sb.AppendLine();
                    }

                    sb.AppendLine();
                    sb.AppendLine("-----,-----");
                    sb.AppendLine(EscapeCsv(lblOpen.Text));
                    sb.AppendLine(EscapeCsv(lblDebitTotal.Text));
                    sb.AppendLine(EscapeCsv(lblCreditTotal.Text));
                    sb.AppendLine(EscapeCsv(lblNet.Text));
                    sb.AppendLine(EscapeCsv(lblClose.Text));

                    File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(true));
                    MessageBox.Show("تم التصدير بنجاح ✅");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("فشل التصدير: " + ex.Message);
                }
            }
        }

        private string EscapeCsv(string s)
        {
            if (s == null) return "";
            if (s.Contains(",") || s.Contains("\"") || s.Contains("\n"))
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        // ===== Printing (كما هو) =====
        private void InitPrinting()
        {
            printDoc = new PrintDocument();
            printDoc.PrintPage += PrintDoc_PrintPage;
        }

        private void PrintGrid()
        {
            if (dgv.DataSource == null || dgv.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للطباعة.");
                return;
            }

            using (PrintPreviewDialog preview = new PrintPreviewDialog())
            {
                _printRowIndex = 0;
                _pageNumber = 1;
                _printedTotals = false;
                _colWidths = null;

                preview.Document = printDoc;
                preview.Width = 1100;
                preview.Height = 800;
                preview.ShowDialog();
            }
        }
       
        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            var margin = e.MarginBounds;
            int y = margin.Top;

            string title = $"كشف حساب المشترك - {ddlReportType.Text} | الفترة: {dtFrom.Value:yyyy/MM/dd} إلى {dtTo.Value:yyyy/MM/dd}";
            using (var fTitle = new Font("Segoe UI", 12, FontStyle.Bold))
            {
                g.DrawString(title, fTitle, Brushes.Black, margin.Left, y);
                y += 30;
            }

            using (var fInfo = new Font("Segoe UI", 10, FontStyle.Regular))
            {
                g.DrawString(lblSubscriberInfo.Text, fInfo, Brushes.Black, margin.Left, y);
                y += 25;
            }

            var visibleCols = GetVisibleColumns();
            if (_colWidths == null || _colWidths.Length != visibleCols.Length)
                _colWidths = CalcColumnWidths(margin.Width, visibleCols);

            int x = margin.Left;
            int headerHeight = 28;
            using (var headerBrush = new SolidBrush(Primary))
                g.FillRectangle(headerBrush, new Rectangle(margin.Left, y, margin.Width, headerHeight));

            using (var fHeader = new Font("Segoe UI", 10, FontStyle.Bold))
            using (var bHeaderText = new SolidBrush(Color.White))
            using (var pGrid = new Pen(Color.LightGray))
            {
                for (int c = 0; c < visibleCols.Length; c++)
                {
                    int w = _colWidths[c];
                    var rect = new Rectangle(x, y, w, headerHeight);
                    g.DrawRectangle(pGrid, rect);
                    g.DrawString(visibleCols[c].HeaderText, fHeader, bHeaderText, rect, CenterFormat());
                    x += w;
                }
            }

            y += headerHeight;
            int rowHeight = 26;

            using (var fRow = new Font("Segoe UI", 9, FontStyle.Regular))
            using (var pGrid = new Pen(Color.LightGray))
            {
                while (_printRowIndex < dgv.Rows.Count)
                {
                    var row = dgv.Rows[_printRowIndex];
                    if (row.IsNewRow) { _printRowIndex++; continue; }

                    if (y + rowHeight > margin.Bottom)
                    {
                        e.HasMorePages = true;
                        _pageNumber++;
                        return;
                    }

                    x = margin.Left;
                    for (int c = 0; c < visibleCols.Length; c++)
                    {
                        int w = _colWidths[c];
                        var rect = new Rectangle(x, y, w, rowHeight);
                        g.DrawRectangle(pGrid, rect);

                        var val = row.Cells[visibleCols[c].Name].Value?.ToString() ?? "";
                        g.DrawString(val, fRow, Brushes.Black, rect, CenterFormat());
                        x += w;
                    }

                    y += rowHeight;
                    _printRowIndex++;
                }
            }

            if (!_printedTotals)
            {
                int need = 80;
                if (y + need > margin.Bottom)
                {
                    e.HasMorePages = true;
                    _pageNumber++;
                    return;
                }

                y += 10;
                using (var pen = new Pen(Color.Gray))
                    g.DrawLine(pen, margin.Left, y, margin.Right, y);

                y += 10;
                using (var fTotTitle = new Font("Segoe UI", 11, FontStyle.Bold))
                    g.DrawString("الإجماليات:", fTotTitle, Brushes.Black, margin.Left, y);

                y += 22;
                using (var fTot = new Font("Segoe UI", 10, FontStyle.Regular))
                {
                    g.DrawString(lblOpen.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
                    g.DrawString(lblDebitTotal.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
                    g.DrawString(lblCreditTotal.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
                    g.DrawString(lblNet.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
                    g.DrawString(lblClose.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
                }

                _printedTotals = true;
            }

            using (var fFooter = new Font("Segoe UI", 9, FontStyle.Italic))
                g.DrawString("صفحة: " + _pageNumber, fFooter, Brushes.Gray, margin.Left, margin.Bottom + 10);

            e.HasMorePages = false;

            _printRowIndex = 0;
            _colWidths = null;
            _printedTotals = false;
        }
        private StringFormat CenterFormat()
        {
            return new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };
        }
        private DataGridViewColumn[] GetVisibleColumns()
        {
            var list = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn c in dgv.Columns)
                if (c.Visible) list.Add(c);
            return list.ToArray();
        }
        private int[] CalcColumnWidths(int totalWidth, DataGridViewColumn[] cols)
        {
            float totalWeight = 0;
            foreach (var c in cols)
                totalWeight += (c.FillWeight > 0 ? c.FillWeight : 100);

            int[] widths = new int[cols.Length];
            int used = 0;

            for (int i = 0; i < cols.Length; i++)
            {
                float w = (cols[i].FillWeight > 0 ? cols[i].FillWeight : 100);
                widths[i] = (int)Math.Floor(totalWidth * (w / totalWeight));
                used += widths[i];
            }

            int diff = totalWidth - used;
            if (diff != 0 && widths.Length > 0) widths[0] += diff;
            return widths;
        }
        private void SavePreset()
        {
            string name = PromptDialog.Show("حفظ قالب", "اسم القالب:", "قالب 1");
            name = (name ?? "").Trim();
            if (name.Length == 0) return;

            var opt = new ReportOptions
            {
                From = dtFrom.Value.Date,
                To = dtTo.Value.Date,
                SortBy = ddlSortBy.SelectedItem?.ToString() ?? "التاريخ",
                SortDesc = chkSortDesc.Checked,
                OnlyInvoices = chkOnlyInvoices.Checked,
                OnlyPayments = chkOnlyPayments.Checked,
                GroupBy = ddlGroupBy.SelectedItem?.ToString() ?? "بدون",
            };

            foreach (var item in clbColumns.CheckedItems)
                opt.SelectedColumns.Add(item.ToString());

            string json = JsonConvert.SerializeObject(opt);

            using (SqlConnection con = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
INSERT INTO dbo.ReportPresets (PresetName, FormKey, JsonOptions)
VALUES (@n, @k, @j);", con))
            {
                cmd.Parameters.AddWithValue("@n", name);
                cmd.Parameters.AddWithValue("@k", FormKey);
                cmd.Parameters.AddWithValue("@j", json);
                con.Open();
                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("تم حفظ القالب ✅");
        }

        private void LoadPreset()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = Db.GetConnection())
            using (var da = new SqlDataAdapter(@"
SELECT PresetID, PresetName
FROM dbo.ReportPresets
WHERE FormKey=@k
ORDER BY CreatedAt DESC;", con))
            {
                da.SelectCommand.Parameters.AddWithValue("@k", FormKey);
                da.Fill(dt);
            }

            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد قوالب محفوظة.");
                return;
            }

            var sb = new StringBuilder();
            foreach (DataRow r in dt.Rows)
                sb.AppendLine($"{r["PresetID"]}: {r["PresetName"]}");

            string pick = PromptDialog.Show(
                "تحميل قالب",
                "اختر رقم القالب (PresetID):\n\n" + sb.ToString(),
                dt.Rows[0]["PresetID"].ToString()
            );

            if (!int.TryParse((pick ?? "").Trim(), out int presetId)) return;

            string json = null;
            using (SqlConnection con = Db.GetConnection())
            using (var cmd = new SqlCommand(@"
SELECT JsonOptions
FROM dbo.ReportPresets
WHERE PresetID=@id AND FormKey=@k;", con))
            {
                cmd.Parameters.AddWithValue("@id", presetId);
                cmd.Parameters.AddWithValue("@k", FormKey);
                con.Open();
                json = cmd.ExecuteScalar() as string;
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                MessageBox.Show("تعذر تحميل القالب.");
                return;
            }

            var opt = JsonConvert.DeserializeObject<ReportOptions>(json);
            ApplyOptionsToUi(opt);

            MessageBox.Show("تم تحميل القالب ✅");
        }
        // ================== Prompt Dialog ==================

        private static class PromptDialog
        {
            public static string Show(string title, string message, string defaultValue = "")
            {
                using (Form form = new Form())
                using (Label lbl = new Label())
                using (TextBox txt = new TextBox())
                using (Button btnOk = new Button())
                using (Button btnCancel = new Button())
                {
                    form.Text = title;
                    form.FormBorderStyle = FormBorderStyle.FixedDialog;
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.MinimizeBox = false;
                    form.MaximizeBox = false;
                    form.ClientSize = new Size(520, 150);
                    form.RightToLeft = RightToLeft.Yes;
                    form.RightToLeftLayout = true;

                    lbl.AutoSize = false;
                    lbl.Text = message;
                    lbl.SetBounds(12, 12, 496, 40);

                    txt.Text = defaultValue ?? "";
                    txt.SetBounds(12, 55, 496, 28);

                    btnOk.Text = "موافق";
                    btnOk.DialogResult = DialogResult.OK;
                    btnOk.SetBounds(320, 95, 90, 30);

                    btnCancel.Text = "إلغاء";
                    btnCancel.DialogResult = DialogResult.Cancel;
                    btnCancel.SetBounds(418, 95, 90, 30);

                    form.Controls.AddRange(new Control[] { lbl, txt, btnOk, btnCancel });
                    form.AcceptButton = btnOk;
                    form.CancelButton = btnCancel;

                    return form.ShowDialog() == DialogResult.OK ? txt.Text : null;
                }
            }
        }
        private void ApplyOptionsToUi(ReportOptions opt)
        {
            if (opt == null) return;

            if (opt.From != default(DateTime)) dtFrom.Value = opt.From;
            if (opt.To != default(DateTime)) dtTo.Value = opt.To;

            if (!string.IsNullOrWhiteSpace(opt.SortBy) && ddlSortBy.Items.Contains(opt.SortBy))
                ddlSortBy.SelectedItem = opt.SortBy;

            chkSortDesc.Checked = opt.SortDesc;

            if (!string.IsNullOrWhiteSpace(opt.GroupBy) && ddlGroupBy.Items.Contains(opt.GroupBy))
                ddlGroupBy.SelectedItem = opt.GroupBy;

            chkOnlyInvoices.Checked = opt.OnlyInvoices;
            chkOnlyPayments.Checked = opt.OnlyPayments;

            for (int i = 0; i < clbColumns.Items.Count; i++)
                clbColumns.SetItemChecked(i, false);

            var set = new HashSet<string>(opt.SelectedColumns ?? new List<string>());
            for (int i = 0; i < clbColumns.Items.Count; i++)
            {
                string c = clbColumns.Items[i].ToString();
                if (set.Contains(c)) clbColumns.SetItemChecked(i, true);
            }
        }

        
    }
}

*/