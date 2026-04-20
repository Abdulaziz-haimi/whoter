using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using Font = System.Drawing.Font;

namespace water3
{
    public partial class SubscribersBillingReportForm : Form
    {
        // ===== Printing =====
        private PrintDocument printDoc;
        private int _printRowIndex = 0;
        private int[] _colWidths;
        private int _pageNumber = 1;
        private bool _printedTotals = false;

        // Presets
        private const string FormKey = "SubscribersBillingReportForm";

        public SubscribersBillingReportForm()
        {
            InitializeComponent();

            Text = "تقرير القراءات والفواتير - جميع المشتركين";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            BackColor = Color.White;

            ApplyTheme();
            InitUserControlOptions();   // ✅ بدل ما تبني usercontrol في BuildLayout
            InitPrinting();
            WireEvents();

            dtTo.Value = DateTime.Today;
            dtFrom.Value = DateTime.Today.AddDays(-30);

            LoadCollectors();
            SetMeterPlaceholder(true);
            UpdateFilterVisibility();

            LoadReport();
        }

        private void ApplyTheme()
        {
            // Fonts
            var f11 = new Font("Segoe UI", 11F, FontStyle.Regular);
            var f11b = new Font("Segoe UI", 11F, FontStyle.Bold);

            lblSearchTitle.Font = f11b;
            lblFromTitle.Font = f11;
            lblToTitle.Font = f11;
            lblReportTypeTitle.Font = f11;
            lblFilterModeTitle.Font = f11;

            txtSearch.Font = f11;
            dtFrom.Font = f11;
            dtTo.Font = f11;
            ddlReportType.Font = f11;
            ddlFilterMode.Font = f11;
            ddlCollectors.Font = f11;
            txtMeterFilter.Font = f11;

            // Buttons
            StylePrimary(btnApply);
            StyleLight(btnRefresh);
            StyleLight(btnExportExcel);
            StylePrimary(btnPrint);
            btnPrint.BackColor = Color.FromArgb(90, 90, 90);

            // Grid styles
            dgv.Font = f11;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 87, 183);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = f11b;
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 232, 255);
            dgv.RowsDefaultCellStyle.BackColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 247, 255);

            // Totals styles
            totalsPanel.BackColor = Color.FromArgb(247, 251, 255);
            StyleTotalLabel(lblCount);
            StyleTotalLabel(lblTotalConsumption);
            StyleTotalLabel(lblTotalInvoices);
        }

        private void StylePrimary(Button b)
        {
            b.Height = 34;
            b.BackColor = Color.FromArgb(0, 106, 204);
            b.ForeColor = Color.White;
            b.FlatStyle = FlatStyle.Flat;
            b.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            b.Cursor = Cursors.Hand;
            b.FlatAppearance.BorderSize = 0;
        }

        private void StyleLight(Button b)
        {
            b.Height = 34;
            b.BackColor = Color.White;
            b.ForeColor = Color.FromArgb(0, 106, 204);
            b.FlatStyle = FlatStyle.Flat;
            b.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            b.Cursor = Cursors.Hand;
            b.FlatAppearance.BorderColor = Color.FromArgb(0, 106, 204);
            b.FlatAppearance.BorderSize = 1;
        }

        private void StyleTotalLabel(Label l)
        {
            l.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            l.ForeColor = Color.DarkBlue;
        }

        private void InitUserControlOptions()
        {
            // نفس الذي كان في BuildLayout
            var cols = new List<string>
            {
                "التاريخ","المشترك","العداد","الاستهلاك","قيمة الفاتورة","الحالة","المحصل","رقم الفاتورة"
            };
            var defaults = new[] { "التاريخ", "المشترك", "العداد", "الاستهلاك", "قيمة الفاتورة", "المحصل" };

            reportOptions.SetColumns(cols, defaults);

            reportOptions.SetSortFields(new[]
            {
                "التاريخ","المشترك","العداد","الاستهلاك","قيمة الفاتورة","الحالة","المحصل","رقم الفاتورة"
            }, defaultField: "التاريخ");
        }

        private void WireEvents()
        {
            btnApply.Click += (s, e) => LoadReport();
            btnRefresh.Click += (s, e) => { LoadCollectors(); LoadReport(); };
            btnExportExcel.Click += (s, e) => ExportToExcelCsv();
            btnPrint.Click += (s, e) => PrintGrid();

            ddlFilterMode.SelectedIndexChanged += (s, e) => UpdateFilterVisibility();

            txtMeterFilter.GotFocus += (s, e) => SetMeterPlaceholder(false);
            txtMeterFilter.LostFocus += (s, e) => SetMeterPlaceholder(true);

            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; LoadReport(); } };
            txtMeterFilter.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; LoadReport(); } };

            reportOptions.SavePresetClicked += (s, e) => SavePreset();
            reportOptions.LoadPresetClicked += (s, e) => LoadPreset();

            reportOptions.OptionsChanged += (s, e) =>
            {
                var opt = reportOptions.GetOptions();
                ApplyUserControlColumnsToGrid(opt);
                ApplyUserControlSortToGrid(opt);
            };
        }
        // ==========================
        // ✅ Apply UserControl -> Grid
        // ==========================
        private static readonly Dictionary<string, string> ColumnMap = new Dictionary<string, string>
        {
            ["التاريخ"] = "تاريخ الفاتورة",
            ["المشترك"] = "اسم المشترك",
            ["العداد"] = "رقم العداد",
            ["الاستهلاك"] = "الاستهلاك",
            ["قيمة الفاتورة"] = "إجمالي الفاتورة",
            ["الحالة"] = "حالة الفاتورة",
            ["المحصل"] = "المحصل",
            ["رقم الفاتورة"] = "رقم الفاتورة",
        };

        private static readonly Dictionary<string, string> SortMap = new Dictionary<string, string>
        {
            ["التاريخ"] = "تاريخ الفاتورة",
            ["المشترك"] = "اسم المشترك",
            ["العداد"] = "رقم العداد",
            ["الاستهلاك"] = "الاستهلاك",
            ["قيمة الفاتورة"] = "إجمالي الفاتورة",
            ["الحالة"] = "حالة الفاتورة",
            ["المحصل"] = "المحصل",
            ["رقم الفاتورة"] = "رقم الفاتورة",
        };

        private void ApplyUserControlColumnsToGrid(ReportOptionsPanel.ReportOptions opt)
        {
            if (dgv.DataSource == null || dgv.Columns.Count == 0) return;

            if (dgv.Columns.Contains("SubscriberID"))
                dgv.Columns["SubscriberID"].Visible = false;

            if (opt?.SelectedColumns == null || opt.SelectedColumns.Count == 0) return;

            var visibleReal = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var uiCol in opt.SelectedColumns)
            {
                if (ColumnMap.TryGetValue(uiCol, out var real))
                    visibleReal.Add(real);
            }

            foreach (DataGridViewColumn c in dgv.Columns)
            {
                if (c.Name == "SubscriberID") { c.Visible = false; continue; }
                c.Visible = visibleReal.Contains(c.Name);
            }
        }

        // ==========================
        // ضع هنا باقي دوالك كما هي:
        // UpdateFilterVisibility / SetMeterPlaceholder / LoadCollectors / LoadReport
        // PaintNumberColumns / CalcTotalsFromGrid / SavePreset / LoadPreset ...
        // ExportToExcelCsv / Printing ...
        // ==========================
        // ==========================
        // ✅ Load collectors
        // ==========================
        private void LoadCollectors()
        {
            ddlCollectors.Items.Clear();
            ddlCollectors.Items.Add(new ComboItem("كل المحصلين", "0"));

            using (var con = Db.GetConnection())
            using (var da = new SqlDataAdapter("SELECT CollectorID, Name FROM Collectors ORDER BY Name", con))
            {
                var dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow r in dt.Rows)
                    ddlCollectors.Items.Add(new ComboItem(r["Name"].ToString(), r["CollectorID"].ToString()));
            }

            ddlCollectors.SelectedIndex = 0;
        }

        // ==========================
        // ✅ Load report (with Collector column)
        // ==========================
        private void LoadReport()
        {
            DateTime from = dtFrom.Value.Date;
            DateTime to = dtTo.Value.Date;
            if (from > to)
            {
                MessageBox.Show("تاريخ (من) يجب أن يكون أقل أو يساوي (إلى).");
                return;
            }

            string q = (txtSearch.Text ?? "").Trim();
            string likeq = "%" + q + "%";

            string reportType = ddlReportType.Text;
            string filterMode = ddlFilterMode.Text;

            int collectorId = 0;
            if (ddlCollectors.Visible && ddlCollectors.SelectedItem is ComboItem ci)
                int.TryParse(ci.Value, out collectorId);

            string meterKey = "";
            if (txtMeterFilter.Visible && txtMeterFilter.ForeColor != Color.Gray)
                meterKey = (txtMeterFilter.Text ?? "").Trim();

            string likeMeter = "%" + meterKey + "%";

            // فلترة حسب المحصل: وجود سداد على الفاتورة داخل الفترة من هذا المحصل
            string collectorExistsClause = @"
AND (
    @CollectorID = 0
    OR EXISTS (
        SELECT 1
        FROM Payments p
        WHERE p.InvoiceID = I.InvoiceID
          AND p.CollectorID = @CollectorID
          AND p.PaymentDate >= @from AND p.PaymentDate <= @to
    )
)";

            string meterClause = @"
AND (
    @MeterKey = N''
    OR ISNULL(M.MeterNumber,N'') LIKE @likeMeter
)";

            string baseSearchClause = @"
AND (
    @q = N''
    OR S.Name LIKE @likeq
    OR ISNULL(S.PhoneNumber,N'') LIKE @likeq
    OR ISNULL(S.Address,N'') LIKE @likeq
    OR ISNULL(M.MeterNumber,N'') LIKE @likeq
)";

            // ✅ عمود المحصل: نجيب آخر سداد (داخل الفترة) على الفاتورة
            // لو ما فيه سداد داخل الفترة، يطلع فاضي
            string collectorApply = @"
OUTER APPLY (
    SELECT TOP 1 p.CollectorID, p.PaymentDate
    FROM Payments p
    WHERE p.InvoiceID = I.InvoiceID
      AND p.PaymentDate >= @from AND p.PaymentDate <= @to
    ORDER BY p.PaymentDate DESC, p.PaymentID DESC
) px
LEFT JOIN Collectors C ON C.CollectorID = px.CollectorID
";

            string sql;

            if (reportType.StartsWith("آخر فاتورة"))
            {
                sql = @"
SELECT
    S.SubscriberID,
    S.Name        AS [اسم المشترك],
    ISNULL(M.MeterNumber, N'') AS [رقم العداد],
    ISNULL(S.PhoneNumber, N'') AS [الهاتف],
    ISNULL(S.Address, N'')     AS [العنوان],

    CAST(ISNULL(R.PreviousReading,0) AS DECIMAL(18,2)) AS [القراءة السابقة],
    CAST(ISNULL(R.CurrentReading,0)  AS DECIMAL(18,2)) AS [القراءة الحالية],
    CAST(ISNULL(R.Consumption,0)     AS DECIMAL(18,2)) AS [الاستهلاك],

    CAST(ISNULL(I.TotalAmount,0)     AS DECIMAL(18,2)) AS [إجمالي الفاتورة],
    ISNULL(I.Status, N'')            AS [حالة الفاتورة],
    I.InvoiceDate AS [تاريخ الفاتورة],
    I.InvoiceID   AS [رقم الفاتورة],

    ISNULL(C.Name, N'') AS [المحصل]
FROM Subscribers S

OUTER APPLY (
    SELECT TOP 1 sm.MeterID
    FROM SubscriberMeters sm
    WHERE sm.SubscriberID = S.SubscriberID
    ORDER BY sm.IsPrimary DESC, sm.SubscriberMeterID DESC
) smx
LEFT JOIN Meters M ON M.MeterID = smx.MeterID

OUTER APPLY (
    SELECT TOP 1 *
    FROM Invoices i
    WHERE i.SubscriberID = S.SubscriberID
      AND i.InvoiceDate >= @from AND i.InvoiceDate <= @to
    ORDER BY i.InvoiceDate DESC, i.InvoiceID DESC
) I

LEFT JOIN Readings R ON R.ReadingID = I.ReadingID
" + collectorApply + @"
WHERE S.IsActive = 1
  AND I.InvoiceID IS NOT NULL
" + baseSearchClause + @"
" + (filterMode == "حسب العداد" ? meterClause : "") + @"
" + (filterMode == "حسب المحصل" ? collectorExistsClause : "") + @"
ORDER BY S.Name;";
            }
            else
            {
                sql = @"
SELECT
    S.SubscriberID,
    S.Name        AS [اسم المشترك],
    ISNULL(M.MeterNumber, N'') AS [رقم العداد],
    ISNULL(S.PhoneNumber, N'') AS [الهاتف],
    ISNULL(S.Address, N'')     AS [العنوان],

    CAST(ISNULL(R.PreviousReading,0) AS DECIMAL(18,2)) AS [القراءة السابقة],
    CAST(ISNULL(R.CurrentReading,0)  AS DECIMAL(18,2)) AS [القراءة الحالية],
    CAST(ISNULL(R.Consumption,0)     AS DECIMAL(18,2)) AS [الاستهلاك],

    CAST(ISNULL(I.TotalAmount,0)     AS DECIMAL(18,2)) AS [إجمالي الفاتورة],
    ISNULL(I.Status, N'')            AS [حالة الفاتورة],
    I.InvoiceDate AS [تاريخ الفاتورة],
    I.InvoiceID   AS [رقم الفاتورة],

    ISNULL(C.Name, N'') AS [المحصل]
FROM Invoices I
INNER JOIN Subscribers S ON S.SubscriberID = I.SubscriberID

OUTER APPLY (
    SELECT TOP 1 sm.MeterID
    FROM SubscriberMeters sm
    WHERE sm.SubscriberID = S.SubscriberID
    ORDER BY sm.IsPrimary DESC, sm.SubscriberMeterID DESC
) smx
LEFT JOIN Meters M ON M.MeterID = smx.MeterID

LEFT JOIN Readings R ON R.ReadingID = I.ReadingID
" + collectorApply + @"
WHERE S.IsActive = 1
  AND I.InvoiceDate >= @from AND I.InvoiceDate <= @to
" + baseSearchClause + @"
" + (filterMode == "حسب العداد" ? meterClause : "") + @"
" + (filterMode == "حسب المحصل" ? collectorExistsClause : "") + @"
ORDER BY I.InvoiceDate DESC, S.Name;";
            }

            var dt = new DataTable();
            using (var con = Db.GetConnection())
            using (var da = new SqlDataAdapter(sql, con))
            {
                da.SelectCommand.Parameters.AddWithValue("@from", from);
                da.SelectCommand.Parameters.AddWithValue("@to", to);

                da.SelectCommand.Parameters.AddWithValue("@q", q);
                da.SelectCommand.Parameters.AddWithValue("@likeq", likeq);

                da.SelectCommand.Parameters.AddWithValue("@CollectorID", collectorId);

                da.SelectCommand.Parameters.AddWithValue("@MeterKey", meterKey);
                da.SelectCommand.Parameters.AddWithValue("@likeMeter", likeMeter);

                da.Fill(dt);
            }

            dgv.DataSource = dt;

            if (dgv.Columns.Contains("SubscriberID"))
                dgv.Columns["SubscriberID"].Visible = false;

            // ✅ Apply user control columns + sort
            var uiOpt = reportOptions.GetOptions();
            ApplyUserControlColumnsToGrid(uiOpt);
            ApplyUserControlSortToGrid(uiOpt);

            PaintNumberColumns();
            CalcTotalsFromGrid();
        }

        private void PaintNumberColumns()
        {
            if (dgv.Columns.Contains("إجمالي الفاتورة"))
                dgv.Columns["إجمالي الفاتورة"].DefaultCellStyle.ForeColor = Color.DarkRed;

            if (dgv.Columns.Contains("الاستهلاك"))
                dgv.Columns["الاستهلاك"].DefaultCellStyle.ForeColor = Color.DarkBlue;

            if (dgv.Columns.Contains("حالة الفاتورة"))
                dgv.Columns["حالة الفاتورة"].DefaultCellStyle.ForeColor = Color.FromArgb(80, 80, 80);

            if (dgv.Columns.Contains("المحصل"))
                dgv.Columns["المحصل"].DefaultCellStyle.ForeColor = Color.FromArgb(0, 90, 60);
        }

        private void CalcTotalsFromGrid()
        {
            int count = 0;
            decimal totalCons = 0m;
            decimal totalInv = 0m;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                count++;

                if (dgv.Columns.Contains("الاستهلاك") && dgv.Columns["الاستهلاك"].Visible)
                {
                    decimal.TryParse(row.Cells["الاستهلاك"].Value?.ToString(), out decimal c);
                    totalCons += c;
                }
                else if (dgv.Columns.Contains("الاستهلاك"))
                {
                    // حتى لو مخفي: احسب من المصدر
                    decimal.TryParse(row.Cells["الاستهلاك"].Value?.ToString(), out decimal c);
                    totalCons += c;
                }

                if (dgv.Columns.Contains("إجمالي الفاتورة"))
                {
                    decimal.TryParse(row.Cells["إجمالي الفاتورة"].Value?.ToString(), out decimal t);
                    totalInv += t;
                }
            }

            lblCount.Text = $"عدد السجلات: {count:N0}";
            lblTotalConsumption.Text = $"إجمالي الاستهلاك: {totalCons:N2}";
            lblTotalInvoices.Text = $"إجمالي الفواتير: {totalInv:N2}";
        }
        private void SetMeterPlaceholder(bool enable)
        {
            if (!txtMeterFilter.Visible) return;

            if (enable && string.IsNullOrWhiteSpace(txtMeterFilter.Text))
            {
                txtMeterFilter.ForeColor = Color.Gray;
                txtMeterFilter.Text = "اكتب رقم العداد...";
            }
            else if (!enable && txtMeterFilter.ForeColor == Color.Gray)
            {
                txtMeterFilter.Text = "";
                txtMeterFilter.ForeColor = Color.Black;
            }
        }
        private void InitPrinting()
        {
            printDoc = new PrintDocument();
            printDoc.PrintPage += PrintDoc_PrintPage;
        }
        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            var g = e.Graphics;
            var margin = e.MarginBounds;
            int y = margin.Top;

            string title = $"تقرير القراءات والفواتير - {ddlReportType.Text} | الفترة: {dtFrom.Value:yyyy/MM/dd} إلى {dtTo.Value:yyyy/MM/dd}";
            var f = GetFilterTextForHeader();
            if (!string.IsNullOrWhiteSpace(f)) title += " | " + f;

            using (var fTitle = new Font("Segoe UI", 12, FontStyle.Bold))
            {
                g.DrawString(title, fTitle, Brushes.Black, margin.Left, y);
                y += 30;
            }

            var visibleCols = GetVisibleColumns();
            if (_colWidths == null || _colWidths.Length != visibleCols.Length)
                _colWidths = CalcColumnWidths(margin.Width, visibleCols);

            int x = margin.Left;
            int headerHeight = 28;

            using (var headerBrush = new SolidBrush(Color.FromArgb(0, 87, 183)))
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
                int need = 70;
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
                    g.DrawString(lblCount.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
                    g.DrawString(lblTotalConsumption.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
                    g.DrawString(lblTotalInvoices.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
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
        // ==========================
        // ✅ Presets (Save / Load) form + usercontrol
        // ==========================
        private void SavePreset()
        {
            string name = PromptDialog.Show("حفظ قالب", "اسم القالب:", "قالب 1");
            name = (name ?? "").Trim();
            if (name.Length == 0) return;

            var preset = ReadPresetFromUi();
            string json = JsonConvert.SerializeObject(preset);

            using (var con = Db.GetConnection())
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
            using (var con = Db.GetConnection())
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
            using (var con = Db.GetConnection())
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

            var preset = JsonConvert.DeserializeObject<BillingReportPreset>(json);
            ApplyPresetToUi(preset);

            MessageBox.Show("تم تحميل القالب ✅");

            LoadReport();
        }
        private BillingReportPreset ReadPresetFromUi()
        {
            int collectorId = 0;
            if (ddlCollectors.Visible && ddlCollectors.SelectedItem is ComboItem ci)
                int.TryParse(ci.Value, out collectorId);

            string meterKey = "";
            if (txtMeterFilter.Visible && txtMeterFilter.ForeColor != Color.Gray)
                meterKey = (txtMeterFilter.Text ?? "").Trim();

            return new BillingReportPreset
            {
                From = dtFrom.Value.Date,
                To = dtTo.Value.Date,
                SearchText = (txtSearch.Text ?? "").Trim(),
                ReportType = ddlReportType.SelectedItem?.ToString() ?? "",
                FilterMode = ddlFilterMode.SelectedItem?.ToString() ?? "بدون",
                CollectorId = collectorId,
                MeterKey = meterKey,
                Ui = reportOptions.GetOptions()
            };
        }

        private void ApplyPresetToUi(BillingReportPreset preset)
        {
            if (preset == null) return;

            if (preset.From != default(DateTime)) dtFrom.Value = preset.From;
            if (preset.To != default(DateTime)) dtTo.Value = preset.To;

            txtSearch.Text = preset.SearchText ?? "";

            if (!string.IsNullOrWhiteSpace(preset.ReportType) && ddlReportType.Items.Contains(preset.ReportType))
                ddlReportType.SelectedItem = preset.ReportType;

            if (!string.IsNullOrWhiteSpace(preset.FilterMode) && ddlFilterMode.Items.Contains(preset.FilterMode))
                ddlFilterMode.SelectedItem = preset.FilterMode;

            UpdateFilterVisibility();

            // Collector
            if (ddlCollectors.Visible)
            {
                for (int i = 0; i < ddlCollectors.Items.Count; i++)
                {
                    if (ddlCollectors.Items[i] is ComboItem ci && int.TryParse(ci.Value, out int id) && id == preset.CollectorId)
                    {
                        ddlCollectors.SelectedIndex = i;
                        break;
                    }
                }
            }

            // MeterKey
            if (txtMeterFilter.Visible)
            {
                txtMeterFilter.ForeColor = Color.Black;
                txtMeterFilter.Text = preset.MeterKey ?? "";
                SetMeterPlaceholder(true);
            }

            // UserControl
            if (preset.Ui != null)
                reportOptions.ApplyOptions(preset.Ui);
        }

        private class BillingReportPreset
        {
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public string SearchText { get; set; } = "";
            public string ReportType { get; set; } = "";
            public string FilterMode { get; set; } = "بدون";
            public int CollectorId { get; set; } = 0;
            public string MeterKey { get; set; } = "";

            public ReportOptionsPanel.ReportOptions Ui { get; set; } = new ReportOptionsPanel.ReportOptions();
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
        private void UpdateFilterVisibility()
        {
            string mode = ddlFilterMode.Text;

            ddlCollectors.Visible = (mode == "حسب المحصل");
            txtMeterFilter.Visible = (mode == "حسب العداد");

            if (!txtMeterFilter.Visible)
            {
                txtMeterFilter.Text = "";
                txtMeterFilter.ForeColor = Color.Black;
            }
            else
            {
                SetMeterPlaceholder(true);
            }
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
        private void ApplyUserControlSortToGrid(ReportOptionsPanel.ReportOptions opt)
        {
            if (dgv.DataSource == null || dgv.Columns.Count == 0) return;
            if (opt == null) return;

            var key = opt.SortBy ?? "التاريخ";
            if (!SortMap.TryGetValue(key, out var realCol)) return;
            if (!dgv.Columns.Contains(realCol)) return;

            try
            {
                dgv.Columns[realCol].SortMode = DataGridViewColumnSortMode.Programmatic;
                dgv.Sort(dgv.Columns[realCol],
                    opt.SortDesc ? System.ComponentModel.ListSortDirection.Descending : System.ComponentModel.ListSortDirection.Ascending);
            }
            catch
            {
                if (dgv.DataSource is DataTable dt)
                {
                    try
                    {
                        dt.DefaultView.Sort = $"[{realCol}] {(opt.SortDesc ? "DESC" : "ASC")}";
                        dgv.DataSource = dt.DefaultView.ToTable();
                    }
                    catch { }
                }
            }
        }

        // ==========================
        // ✅ Export CSV
        // ==========================
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
                sfd.FileName = $"SubscribersReport_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var sb = new StringBuilder();

                    sb.AppendLine(EscapeCsv($"تقرير القراءات والفواتير - {ddlReportType.Text}"));
                    sb.AppendLine(EscapeCsv($"الفترة: {dtFrom.Value:yyyy/MM/dd} إلى {dtTo.Value:yyyy/MM/dd}"));
                    var f = GetFilterTextForHeader();
                    if (!string.IsNullOrWhiteSpace(f)) sb.AppendLine(EscapeCsv("الفلترة: " + f));
                    sb.AppendLine();

                    // header
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        if (!dgv.Columns[i].Visible) continue;
                        sb.Append(EscapeCsv(dgv.Columns[i].HeaderText));
                        sb.Append(",");
                    }
                    sb.AppendLine();

                    // rows
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;

                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            if (!dgv.Columns[i].Visible) continue;
                            sb.Append(EscapeCsv(row.Cells[i].Value?.ToString() ?? ""));
                            sb.Append(",");
                        }
                        sb.AppendLine();
                    }

                    // totals
                    sb.AppendLine();
                    sb.AppendLine(EscapeCsv("الإجماليات:"));
                    sb.AppendLine(EscapeCsv(lblCount.Text));
                    sb.AppendLine(EscapeCsv(lblTotalConsumption.Text));
                    sb.AppendLine(EscapeCsv(lblTotalInvoices.Text));

                    File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(true));
                    MessageBox.Show("تم التصدير بنجاح ✅");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("فشل التصدير: " + ex.Message);
                }
            }
        }

        private string GetFilterTextForHeader()
        {
            if (ddlFilterMode.Text == "حسب المحصل")
                return "حسب المحصل: " + (ddlCollectors.SelectedItem is ComboItem ci ? ci.Text : "");
            if (ddlFilterMode.Text == "حسب العداد")
            {
                string mk = (txtMeterFilter.Visible && txtMeterFilter.ForeColor != Color.Gray) ? (txtMeterFilter.Text ?? "").Trim() : "";
                return "حسب العداد: " + mk;
            }
            return "";
        }

        private string EscapeCsv(string s)
        {
            if (s == null) return "";
            if (s.Contains(",") || s.Contains("\"") || s.Contains("\n"))
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        // ==========================
        // ✅ Printing
        // ==========================
        //private void InitPrinting()
        //{
        //    printDoc = new PrintDocument();
        //    printDoc.PrintPage += PrintDoc_PrintPage;
        //}

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
                _colWidths = null;
                _printedTotals = false;

                preview.Document = printDoc;
                preview.Width = 1100;
                preview.Height = 800;
                preview.ShowDialog();
            }
        }



       

        private Button MakePrimaryButton(string text, int width)
        {
            var b = new Button
            {
                Text = text,
                Height = 34,
                Width = width,
                BackColor = Color.FromArgb(0, 106, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 0, 0, 0)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private Button MakeLightButton(string text, int width)
        {
            var b = new Button
            {
                Text = text,
                Height = 34,
                Width = width,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(0, 106, 204),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 0, 0, 0)
            };
            b.FlatAppearance.BorderColor = Color.FromArgb(0, 106, 204);
            b.FlatAppearance.BorderSize = 1;
            return b;
        }

        private Label MakeTotalLabel(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
        }

        private class ComboItem
        {
            public string Text { get; }
            public string Value { get; }
            public ComboItem(string text, string value) { Text = text; Value = value; }
            public override string ToString() => Text;
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
            // ✨ بقية كودك انسخه كما هو بدون تغيير (الدوال الطويلة)
            // فقط احذف BuildLayout() و HookEvents() القديمتين لأنهم صاروا Designer + WireEvents.
        }
}

/*using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using Font = System.Drawing.Font;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace water3
{
    public partial class SubscribersBillingReportForm : Form
    {
        // ===== UI =====
        private TextBox txtSearch, txtMeterFilter;
        private DateTimePicker dtFrom, dtTo;
        private ComboBox ddlReportType;

        // Filters
        private ComboBox ddlFilterMode;      // بدون / حسب المحصل / حسب العداد
        private ComboBox ddlCollectors;      // يظهر فقط عند "حسب المحصل"

        private Button btnApply, btnRefresh, btnExportExcel, btnPrint;

        private const string FormKey = "SubscribersBillingReportForm";

        private DataGridView dgv;

        // Totals
        private Label lblCount, lblTotalConsumption, lblTotalInvoices;

        // ✅ UserControl
        private ReportOptionsPanel reportOptions;

        // ===== Printing =====
        private PrintDocument printDoc;
        private int _printRowIndex = 0;
        private int[] _colWidths;
        private int _pageNumber = 1;
        private bool _printedTotals = false;

        public SubscribersBillingReportForm()
        {
            Text = "تقرير القراءات والفواتير - جميع المشتركين";
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            BackColor = Color.White;
            FormBorderStyle = FormBorderStyle.None;
            Dock = DockStyle.Fill;

            BuildLayout();
            InitPrinting();
            HookEvents();

            dtTo.Value = DateTime.Today;
            dtFrom.Value = DateTime.Today.AddDays(-30);

            LoadCollectors();

            // placeholder meter
            SetMeterPlaceholder(true);

            LoadReport();
        }

        private void HookEvents()
        {
            btnApply.Click += (s, e) => LoadReport();
            btnRefresh.Click += (s, e) => { LoadCollectors(); LoadReport(); };
            btnExportExcel.Click += (s, e) => ExportToExcelCsv();
            btnPrint.Click += (s, e) => PrintGrid();

            ddlFilterMode.SelectedIndexChanged += (s, e) => UpdateFilterVisibility();

            txtMeterFilter.GotFocus += (s, e) => SetMeterPlaceholder(false);
            txtMeterFilter.LostFocus += (s, e) => SetMeterPlaceholder(true);

            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; LoadReport(); }
            };
            txtMeterFilter.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; LoadReport(); }
            };

            // UserControl events
            reportOptions.SavePresetClicked += (s, e) => SavePreset();
            reportOptions.LoadPresetClicked += (s, e) => LoadPreset();

            // اختياري: تطبيق تغييرات الأعمدة/الترتيب بدون إعادة الاستعلام
            reportOptions.OptionsChanged += (s, e) =>
            {
                var opt = reportOptions.GetOptions();
                ApplyUserControlColumnsToGrid(opt);
                ApplyUserControlSortToGrid(opt);
            };
        }

        private void BuildLayout()
        {
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(15),
                BackColor = Color.White
            };

            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 95));   // tools
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 120));  // report options
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // grid
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 65));   // totals
            Controls.Add(mainLayout);

            // ===== Top tools =====
            var topPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(0, 10, 0, 0)
            };

            // Search
            topPanel.Controls.Add(new Label
            {
                Text = "بحث:",
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Margin = new Padding(0, 6, 8, 0)
            });
            txtSearch = new TextBox { Width = 220, Font = new Font("Segoe UI", 11) };
            topPanel.Controls.Add(txtSearch);

            // Date from/to
            topPanel.Controls.Add(new Label { Text = "من:", AutoSize = true, Font = new Font("Segoe UI", 11), Margin = new Padding(18, 6, 8, 0) });
            dtFrom = new DateTimePicker { Width = 125, Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 11) };
            topPanel.Controls.Add(dtFrom);

            topPanel.Controls.Add(new Label { Text = "إلى:", AutoSize = true, Font = new Font("Segoe UI", 11), Margin = new Padding(10, 6, 8, 0) });
            dtTo = new DateTimePicker { Width = 125, Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 11) };
            topPanel.Controls.Add(dtTo);

            // Report type
            topPanel.Controls.Add(new Label { Text = "نوع العرض:", AutoSize = true, Font = new Font("Segoe UI", 11), Margin = new Padding(18, 6, 8, 0) });
            ddlReportType = new ComboBox
            {
                Width = 245,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };
            ddlReportType.Items.AddRange(new object[]
            {
                "آخر فاتورة لكل مشترك (داخل الفترة)",
                "كل الفواتير داخل الفترة (تفصيلي)"
            });
            ddlReportType.SelectedIndex = 0;
            topPanel.Controls.Add(ddlReportType);

            // Filter mode
            topPanel.Controls.Add(new Label { Text = "فلترة:", AutoSize = true, Font = new Font("Segoe UI", 11), Margin = new Padding(18, 6, 8, 0) });
            ddlFilterMode = new ComboBox
            {
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11)
            };
            ddlFilterMode.Items.AddRange(new object[] { "بدون", "حسب المحصل", "حسب العداد" });
            ddlFilterMode.SelectedIndex = 0;
            topPanel.Controls.Add(ddlFilterMode);

            // Collector dropdown
            ddlCollectors = new ComboBox
            {
                Width = 220,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11),
                Visible = false
            };
            topPanel.Controls.Add(ddlCollectors);

            // Meter filter textbox
            txtMeterFilter = new TextBox
            {
                Width = 170,
                Font = new Font("Segoe UI", 11),
                Visible = false
            };
            topPanel.Controls.Add(txtMeterFilter);

            // Buttons
            btnApply = MakePrimaryButton("تطبيق", 90);
            topPanel.Controls.Add(btnApply);

            btnRefresh = MakeLightButton("تحديث", 90);
            topPanel.Controls.Add(btnRefresh);

            btnExportExcel = MakeLightButton("تصدير CSV", 120);
            topPanel.Controls.Add(btnExportExcel);

            btnPrint = MakePrimaryButton("طباعة", 90);
            btnPrint.BackColor = Color.FromArgb(90, 90, 90);
            topPanel.Controls.Add(btnPrint);

            mainLayout.Controls.Add(topPanel, 0, 0);

            // ===== Report Options UserControl =====
            reportOptions = new ReportOptionsPanel { Dock = DockStyle.Fill };

            // أعمدة UI (خيارات المستخدم)
            var cols = new List<string>
            {
                "التاريخ","المشترك","العداد","الاستهلاك","قيمة الفاتورة","الحالة","المحصل","رقم الفاتورة"
            };

            var defaults = new[] { "التاريخ", "المشترك", "العداد", "الاستهلاك", "قيمة الفاتورة", "المحصل" };

            reportOptions.SetColumns(cols, defaults);

            reportOptions.SetSortFields(new[]
            {
                "التاريخ","المشترك","العداد","الاستهلاك","قيمة الفاتورة","الحالة","المحصل","رقم الفاتورة"
            }, defaultField: "التاريخ");

            mainLayout.Controls.Add(reportOptions, 0, 1);

            // ===== Grid =====
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                Font = new Font("Segoe UI", 11),
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                RowTemplate = { Height = 36 },
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 87, 183);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 232, 255);
            dgv.RowsDefaultCellStyle.BackColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 247, 255);

            mainLayout.Controls.Add(dgv, 0, 2);

            // ===== Totals =====
            var totalsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.FromArgb(247, 251, 255),
                Padding = new Padding(10)
            };
            totalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            totalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            totalsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            lblCount = MakeTotalLabel("عدد السجلات: 0");
            lblTotalConsumption = MakeTotalLabel("إجمالي الاستهلاك: 0.00");
            lblTotalInvoices = MakeTotalLabel("إجمالي الفواتير: 0.00");

            totalsPanel.Controls.Add(lblCount, 0, 0);
            totalsPanel.Controls.Add(lblTotalConsumption, 1, 0);
            totalsPanel.Controls.Add(lblTotalInvoices, 2, 0);

            mainLayout.Controls.Add(totalsPanel, 0, 3);

            UpdateFilterVisibility();
        }

        private void UpdateFilterVisibility()
        {
            string mode = ddlFilterMode.Text;

            ddlCollectors.Visible = (mode == "حسب المحصل");
            txtMeterFilter.Visible = (mode == "حسب العداد");

            if (!txtMeterFilter.Visible)
            {
                txtMeterFilter.Text = "";
                txtMeterFilter.ForeColor = Color.Black;
            }
            else
            {
                SetMeterPlaceholder(true);
            }
        }

        private void SetMeterPlaceholder(bool enable)
        {
            if (!txtMeterFilter.Visible) return;

            if (enable && string.IsNullOrWhiteSpace(txtMeterFilter.Text))
            {
                txtMeterFilter.ForeColor = Color.Gray;
                txtMeterFilter.Text = "اكتب رقم العداد...";
            }
            else if (!enable && txtMeterFilter.ForeColor == Color.Gray)
            {
                txtMeterFilter.Text = "";
                txtMeterFilter.ForeColor = Color.Black;
            }
        }

        // ==========================
        // ✅ Load collectors
        // ==========================
        private void LoadCollectors()
        {
            ddlCollectors.Items.Clear();
            ddlCollectors.Items.Add(new ComboItem("كل المحصلين", "0"));

            using (var con = Db.GetConnection())
            using (var da = new SqlDataAdapter("SELECT CollectorID, Name FROM Collectors ORDER BY Name", con))
            {
                var dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow r in dt.Rows)
                    ddlCollectors.Items.Add(new ComboItem(r["Name"].ToString(), r["CollectorID"].ToString()));
            }

            ddlCollectors.SelectedIndex = 0;
        }

        // ==========================
        // ✅ Load report (with Collector column)
        // ==========================
        private void LoadReport()
        {
            DateTime from = dtFrom.Value.Date;
            DateTime to = dtTo.Value.Date;
            if (from > to)
            {
                MessageBox.Show("تاريخ (من) يجب أن يكون أقل أو يساوي (إلى).");
                return;
            }

            string q = (txtSearch.Text ?? "").Trim();
            string likeq = "%" + q + "%";

            string reportType = ddlReportType.Text;
            string filterMode = ddlFilterMode.Text;

            int collectorId = 0;
            if (ddlCollectors.Visible && ddlCollectors.SelectedItem is ComboItem ci)
                int.TryParse(ci.Value, out collectorId);

            string meterKey = "";
            if (txtMeterFilter.Visible && txtMeterFilter.ForeColor != Color.Gray)
                meterKey = (txtMeterFilter.Text ?? "").Trim();

            string likeMeter = "%" + meterKey + "%";

            // فلترة حسب المحصل: وجود سداد على الفاتورة داخل الفترة من هذا المحصل
            string collectorExistsClause = @"
AND (
    @CollectorID = 0
    OR EXISTS (
        SELECT 1
        FROM Payments p
        WHERE p.InvoiceID = I.InvoiceID
          AND p.CollectorID = @CollectorID
          AND p.PaymentDate >= @from AND p.PaymentDate <= @to
    )
)";

            string meterClause = @"
AND (
    @MeterKey = N''
    OR ISNULL(M.MeterNumber,N'') LIKE @likeMeter
)";

            string baseSearchClause = @"
AND (
    @q = N''
    OR S.Name LIKE @likeq
    OR ISNULL(S.PhoneNumber,N'') LIKE @likeq
    OR ISNULL(S.Address,N'') LIKE @likeq
    OR ISNULL(M.MeterNumber,N'') LIKE @likeq
)";

            // ✅ عمود المحصل: نجيب آخر سداد (داخل الفترة) على الفاتورة
            // لو ما فيه سداد داخل الفترة، يطلع فاضي
            string collectorApply = @"
OUTER APPLY (
    SELECT TOP 1 p.CollectorID, p.PaymentDate
    FROM Payments p
    WHERE p.InvoiceID = I.InvoiceID
      AND p.PaymentDate >= @from AND p.PaymentDate <= @to
    ORDER BY p.PaymentDate DESC, p.PaymentID DESC
) px
LEFT JOIN Collectors C ON C.CollectorID = px.CollectorID
";

            string sql;

            if (reportType.StartsWith("آخر فاتورة"))
            {
                sql = @"
SELECT
    S.SubscriberID,
    S.Name        AS [اسم المشترك],
    ISNULL(M.MeterNumber, N'') AS [رقم العداد],
    ISNULL(S.PhoneNumber, N'') AS [الهاتف],
    ISNULL(S.Address, N'')     AS [العنوان],

    CAST(ISNULL(R.PreviousReading,0) AS DECIMAL(18,2)) AS [القراءة السابقة],
    CAST(ISNULL(R.CurrentReading,0)  AS DECIMAL(18,2)) AS [القراءة الحالية],
    CAST(ISNULL(R.Consumption,0)     AS DECIMAL(18,2)) AS [الاستهلاك],

    CAST(ISNULL(I.TotalAmount,0)     AS DECIMAL(18,2)) AS [إجمالي الفاتورة],
    ISNULL(I.Status, N'')            AS [حالة الفاتورة],
    I.InvoiceDate AS [تاريخ الفاتورة],
    I.InvoiceID   AS [رقم الفاتورة],

    ISNULL(C.Name, N'') AS [المحصل]
FROM Subscribers S

OUTER APPLY (
    SELECT TOP 1 sm.MeterID
    FROM SubscriberMeters sm
    WHERE sm.SubscriberID = S.SubscriberID
    ORDER BY sm.IsPrimary DESC, sm.SubscriberMeterID DESC
) smx
LEFT JOIN Meters M ON M.MeterID = smx.MeterID

OUTER APPLY (
    SELECT TOP 1 *
    FROM Invoices i
    WHERE i.SubscriberID = S.SubscriberID
      AND i.InvoiceDate >= @from AND i.InvoiceDate <= @to
    ORDER BY i.InvoiceDate DESC, i.InvoiceID DESC
) I

LEFT JOIN Readings R ON R.ReadingID = I.ReadingID
" + collectorApply + @"
WHERE S.IsActive = 1
  AND I.InvoiceID IS NOT NULL
" + baseSearchClause + @"
" + (filterMode == "حسب العداد" ? meterClause : "") + @"
" + (filterMode == "حسب المحصل" ? collectorExistsClause : "") + @"
ORDER BY S.Name;";
            }
            else
            {
                sql = @"
SELECT
    S.SubscriberID,
    S.Name        AS [اسم المشترك],
    ISNULL(M.MeterNumber, N'') AS [رقم العداد],
    ISNULL(S.PhoneNumber, N'') AS [الهاتف],
    ISNULL(S.Address, N'')     AS [العنوان],

    CAST(ISNULL(R.PreviousReading,0) AS DECIMAL(18,2)) AS [القراءة السابقة],
    CAST(ISNULL(R.CurrentReading,0)  AS DECIMAL(18,2)) AS [القراءة الحالية],
    CAST(ISNULL(R.Consumption,0)     AS DECIMAL(18,2)) AS [الاستهلاك],

    CAST(ISNULL(I.TotalAmount,0)     AS DECIMAL(18,2)) AS [إجمالي الفاتورة],
    ISNULL(I.Status, N'')            AS [حالة الفاتورة],
    I.InvoiceDate AS [تاريخ الفاتورة],
    I.InvoiceID   AS [رقم الفاتورة],

    ISNULL(C.Name, N'') AS [المحصل]
FROM Invoices I
INNER JOIN Subscribers S ON S.SubscriberID = I.SubscriberID

OUTER APPLY (
    SELECT TOP 1 sm.MeterID
    FROM SubscriberMeters sm
    WHERE sm.SubscriberID = S.SubscriberID
    ORDER BY sm.IsPrimary DESC, sm.SubscriberMeterID DESC
) smx
LEFT JOIN Meters M ON M.MeterID = smx.MeterID

LEFT JOIN Readings R ON R.ReadingID = I.ReadingID
" + collectorApply + @"
WHERE S.IsActive = 1
  AND I.InvoiceDate >= @from AND I.InvoiceDate <= @to
" + baseSearchClause + @"
" + (filterMode == "حسب العداد" ? meterClause : "") + @"
" + (filterMode == "حسب المحصل" ? collectorExistsClause : "") + @"
ORDER BY I.InvoiceDate DESC, S.Name;";
            }

            var dt = new DataTable();
            using (var con = Db.GetConnection())
            using (var da = new SqlDataAdapter(sql, con))
            {
                da.SelectCommand.Parameters.AddWithValue("@from", from);
                da.SelectCommand.Parameters.AddWithValue("@to", to);

                da.SelectCommand.Parameters.AddWithValue("@q", q);
                da.SelectCommand.Parameters.AddWithValue("@likeq", likeq);

                da.SelectCommand.Parameters.AddWithValue("@CollectorID", collectorId);

                da.SelectCommand.Parameters.AddWithValue("@MeterKey", meterKey);
                da.SelectCommand.Parameters.AddWithValue("@likeMeter", likeMeter);

                da.Fill(dt);
            }

            dgv.DataSource = dt;

            if (dgv.Columns.Contains("SubscriberID"))
                dgv.Columns["SubscriberID"].Visible = false;

            // ✅ Apply user control columns + sort
            var uiOpt = reportOptions.GetOptions();
            ApplyUserControlColumnsToGrid(uiOpt);
            ApplyUserControlSortToGrid(uiOpt);

            PaintNumberColumns();
            CalcTotalsFromGrid();
        }

        private void PaintNumberColumns()
        {
            if (dgv.Columns.Contains("إجمالي الفاتورة"))
                dgv.Columns["إجمالي الفاتورة"].DefaultCellStyle.ForeColor = Color.DarkRed;

            if (dgv.Columns.Contains("الاستهلاك"))
                dgv.Columns["الاستهلاك"].DefaultCellStyle.ForeColor = Color.DarkBlue;

            if (dgv.Columns.Contains("حالة الفاتورة"))
                dgv.Columns["حالة الفاتورة"].DefaultCellStyle.ForeColor = Color.FromArgb(80, 80, 80);

            if (dgv.Columns.Contains("المحصل"))
                dgv.Columns["المحصل"].DefaultCellStyle.ForeColor = Color.FromArgb(0, 90, 60);
        }

        private void CalcTotalsFromGrid()
        {
            int count = 0;
            decimal totalCons = 0m;
            decimal totalInv = 0m;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                count++;

                if (dgv.Columns.Contains("الاستهلاك") && dgv.Columns["الاستهلاك"].Visible)
                {
                    decimal.TryParse(row.Cells["الاستهلاك"].Value?.ToString(), out decimal c);
                    totalCons += c;
                }
                else if (dgv.Columns.Contains("الاستهلاك"))
                {
                    // حتى لو مخفي: احسب من المصدر
                    decimal.TryParse(row.Cells["الاستهلاك"].Value?.ToString(), out decimal c);
                    totalCons += c;
                }

                if (dgv.Columns.Contains("إجمالي الفاتورة"))
                {
                    decimal.TryParse(row.Cells["إجمالي الفاتورة"].Value?.ToString(), out decimal t);
                    totalInv += t;
                }
            }

            lblCount.Text = $"عدد السجلات: {count:N0}";
            lblTotalConsumption.Text = $"إجمالي الاستهلاك: {totalCons:N2}";
            lblTotalInvoices.Text = $"إجمالي الفواتير: {totalInv:N2}";
        }

        // ==========================
        // ✅ Presets (Save / Load) form + usercontrol
        // ==========================
        private void SavePreset()
        {
            string name = PromptDialog.Show("حفظ قالب", "اسم القالب:", "قالب 1");
            name = (name ?? "").Trim();
            if (name.Length == 0) return;

            var preset = ReadPresetFromUi();
            string json = JsonConvert.SerializeObject(preset);

            using (var con = Db.GetConnection())
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
            using (var con = Db.GetConnection())
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
            using (var con = Db.GetConnection())
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

            var preset = JsonConvert.DeserializeObject<BillingReportPreset>(json);
            ApplyPresetToUi(preset);

            MessageBox.Show("تم تحميل القالب ✅");

            LoadReport();
        }

        private BillingReportPreset ReadPresetFromUi()
        {
            int collectorId = 0;
            if (ddlCollectors.Visible && ddlCollectors.SelectedItem is ComboItem ci)
                int.TryParse(ci.Value, out collectorId);

            string meterKey = "";
            if (txtMeterFilter.Visible && txtMeterFilter.ForeColor != Color.Gray)
                meterKey = (txtMeterFilter.Text ?? "").Trim();

            return new BillingReportPreset
            {
                From = dtFrom.Value.Date,
                To = dtTo.Value.Date,
                SearchText = (txtSearch.Text ?? "").Trim(),
                ReportType = ddlReportType.SelectedItem?.ToString() ?? "",
                FilterMode = ddlFilterMode.SelectedItem?.ToString() ?? "بدون",
                CollectorId = collectorId,
                MeterKey = meterKey,
                Ui = reportOptions.GetOptions()
            };
        }

        private void ApplyPresetToUi(BillingReportPreset preset)
        {
            if (preset == null) return;

            if (preset.From != default(DateTime)) dtFrom.Value = preset.From;
            if (preset.To != default(DateTime)) dtTo.Value = preset.To;

            txtSearch.Text = preset.SearchText ?? "";

            if (!string.IsNullOrWhiteSpace(preset.ReportType) && ddlReportType.Items.Contains(preset.ReportType))
                ddlReportType.SelectedItem = preset.ReportType;

            if (!string.IsNullOrWhiteSpace(preset.FilterMode) && ddlFilterMode.Items.Contains(preset.FilterMode))
                ddlFilterMode.SelectedItem = preset.FilterMode;

            UpdateFilterVisibility();

            // Collector
            if (ddlCollectors.Visible)
            {
                for (int i = 0; i < ddlCollectors.Items.Count; i++)
                {
                    if (ddlCollectors.Items[i] is ComboItem ci && int.TryParse(ci.Value, out int id) && id == preset.CollectorId)
                    {
                        ddlCollectors.SelectedIndex = i;
                        break;
                    }
                }
            }

            // MeterKey
            if (txtMeterFilter.Visible)
            {
                txtMeterFilter.ForeColor = Color.Black;
                txtMeterFilter.Text = preset.MeterKey ?? "";
                SetMeterPlaceholder(true);
            }

            // UserControl
            if (preset.Ui != null)
                reportOptions.ApplyOptions(preset.Ui);
        }

        private class BillingReportPreset
        {
            public DateTime From { get; set; }
            public DateTime To { get; set; }
            public string SearchText { get; set; } = "";
            public string ReportType { get; set; } = "";
            public string FilterMode { get; set; } = "بدون";
            public int CollectorId { get; set; } = 0;
            public string MeterKey { get; set; } = "";

            public ReportOptionsPanel.ReportOptions Ui { get; set; } = new ReportOptionsPanel.ReportOptions();
        }

        // ==========================
        // ✅ Apply UserControl -> Grid
        // ==========================
        private static readonly Dictionary<string, string> ColumnMap = new Dictionary<string, string>
        {
            ["التاريخ"] = "تاريخ الفاتورة",
            ["المشترك"] = "اسم المشترك",
            ["العداد"] = "رقم العداد",
            ["الاستهلاك"] = "الاستهلاك",
            ["قيمة الفاتورة"] = "إجمالي الفاتورة",
            ["الحالة"] = "حالة الفاتورة",
            ["المحصل"] = "المحصل",
            ["رقم الفاتورة"] = "رقم الفاتورة",
        };

        private static readonly Dictionary<string, string> SortMap = new Dictionary<string, string>
        {
            ["التاريخ"] = "تاريخ الفاتورة",
            ["المشترك"] = "اسم المشترك",
            ["العداد"] = "رقم العداد",
            ["الاستهلاك"] = "الاستهلاك",
            ["قيمة الفاتورة"] = "إجمالي الفاتورة",
            ["الحالة"] = "حالة الفاتورة",
            ["المحصل"] = "المحصل",
            ["رقم الفاتورة"] = "رقم الفاتورة",
        };

        private void ApplyUserControlColumnsToGrid(ReportOptionsPanel.ReportOptions opt)
        {
            if (dgv.DataSource == null || dgv.Columns.Count == 0) return;

            if (dgv.Columns.Contains("SubscriberID"))
                dgv.Columns["SubscriberID"].Visible = false;

            if (opt?.SelectedColumns == null || opt.SelectedColumns.Count == 0) return;

            var visibleReal = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var uiCol in opt.SelectedColumns)
            {
                if (ColumnMap.TryGetValue(uiCol, out var real))
                    visibleReal.Add(real);
            }

            foreach (DataGridViewColumn c in dgv.Columns)
            {
                if (c.Name == "SubscriberID") { c.Visible = false; continue; }
                c.Visible = visibleReal.Contains(c.Name);
            }
        }

        private void ApplyUserControlSortToGrid(ReportOptionsPanel.ReportOptions opt)
        {
            if (dgv.DataSource == null || dgv.Columns.Count == 0) return;
            if (opt == null) return;

            var key = opt.SortBy ?? "التاريخ";
            if (!SortMap.TryGetValue(key, out var realCol)) return;
            if (!dgv.Columns.Contains(realCol)) return;

            try
            {
                dgv.Columns[realCol].SortMode = DataGridViewColumnSortMode.Programmatic;
                dgv.Sort(dgv.Columns[realCol],
                    opt.SortDesc ? System.ComponentModel.ListSortDirection.Descending : System.ComponentModel.ListSortDirection.Ascending);
            }
            catch
            {
                if (dgv.DataSource is DataTable dt)
                {
                    try
                    {
                        dt.DefaultView.Sort = $"[{realCol}] {(opt.SortDesc ? "DESC" : "ASC")}";
                        dgv.DataSource = dt.DefaultView.ToTable();
                    }
                    catch { }
                }
            }
        }

        // ==========================
        // ✅ Export CSV
        // ==========================
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
                sfd.FileName = $"SubscribersReport_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                if (sfd.ShowDialog() != DialogResult.OK) return;

                try
                {
                    var sb = new StringBuilder();

                    sb.AppendLine(EscapeCsv($"تقرير القراءات والفواتير - {ddlReportType.Text}"));
                    sb.AppendLine(EscapeCsv($"الفترة: {dtFrom.Value:yyyy/MM/dd} إلى {dtTo.Value:yyyy/MM/dd}"));
                    var f = GetFilterTextForHeader();
                    if (!string.IsNullOrWhiteSpace(f)) sb.AppendLine(EscapeCsv("الفلترة: " + f));
                    sb.AppendLine();

                    // header
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        if (!dgv.Columns[i].Visible) continue;
                        sb.Append(EscapeCsv(dgv.Columns[i].HeaderText));
                        sb.Append(",");
                    }
                    sb.AppendLine();

                    // rows
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;

                        for (int i = 0; i < dgv.Columns.Count; i++)
                        {
                            if (!dgv.Columns[i].Visible) continue;
                            sb.Append(EscapeCsv(row.Cells[i].Value?.ToString() ?? ""));
                            sb.Append(",");
                        }
                        sb.AppendLine();
                    }

                    // totals
                    sb.AppendLine();
                    sb.AppendLine(EscapeCsv("الإجماليات:"));
                    sb.AppendLine(EscapeCsv(lblCount.Text));
                    sb.AppendLine(EscapeCsv(lblTotalConsumption.Text));
                    sb.AppendLine(EscapeCsv(lblTotalInvoices.Text));

                    File.WriteAllText(sfd.FileName, sb.ToString(), new UTF8Encoding(true));
                    MessageBox.Show("تم التصدير بنجاح ✅");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("فشل التصدير: " + ex.Message);
                }
            }
        }

        private string GetFilterTextForHeader()
        {
            if (ddlFilterMode.Text == "حسب المحصل")
                return "حسب المحصل: " + (ddlCollectors.SelectedItem is ComboItem ci ? ci.Text : "");
            if (ddlFilterMode.Text == "حسب العداد")
            {
                string mk = (txtMeterFilter.Visible && txtMeterFilter.ForeColor != Color.Gray) ? (txtMeterFilter.Text ?? "").Trim() : "";
                return "حسب العداد: " + mk;
            }
            return "";
        }

        private string EscapeCsv(string s)
        {
            if (s == null) return "";
            if (s.Contains(",") || s.Contains("\"") || s.Contains("\n"))
                return "\"" + s.Replace("\"", "\"\"") + "\"";
            return s;
        }

        // ==========================
        // ✅ Printing
        // ==========================
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
                _colWidths = null;
                _printedTotals = false;

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

            string title = $"تقرير القراءات والفواتير - {ddlReportType.Text} | الفترة: {dtFrom.Value:yyyy/MM/dd} إلى {dtTo.Value:yyyy/MM/dd}";
            var f = GetFilterTextForHeader();
            if (!string.IsNullOrWhiteSpace(f)) title += " | " + f;

            using (var fTitle = new Font("Segoe UI", 12, FontStyle.Bold))
            {
                g.DrawString(title, fTitle, Brushes.Black, margin.Left, y);
                y += 30;
            }

            var visibleCols = GetVisibleColumns();
            if (_colWidths == null || _colWidths.Length != visibleCols.Length)
                _colWidths = CalcColumnWidths(margin.Width, visibleCols);

            int x = margin.Left;
            int headerHeight = 28;

            using (var headerBrush = new SolidBrush(Color.FromArgb(0, 87, 183)))
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
                int need = 70;
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
                    g.DrawString(lblCount.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
                    g.DrawString(lblTotalConsumption.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
                    g.DrawString(lblTotalInvoices.Text, fTot, Brushes.Black, margin.Left, y); y += 18;
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

        private Button MakePrimaryButton(string text, int width)
        {
            var b = new Button
            {
                Text = text,
                Height = 34,
                Width = width,
                BackColor = Color.FromArgb(0, 106, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 0, 0, 0)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private Button MakeLightButton(string text, int width)
        {
            var b = new Button
            {
                Text = text,
                Height = 34,
                Width = width,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(0, 106, 204),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(10, 0, 0, 0)
            };
            b.FlatAppearance.BorderColor = Color.FromArgb(0, 106, 204);
            b.FlatAppearance.BorderSize = 1;
            return b;
        }

        private Label MakeTotalLabel(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.DarkBlue
            };
        }

        private class ComboItem
        {
            public string Text { get; }
            public string Value { get; }
            public ComboItem(string text, string value) { Text = text; Value = value; }
            public override string ToString() => Text;
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
*/