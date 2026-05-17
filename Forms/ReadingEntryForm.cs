using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace water3.Forms
{
    public partial class ReadingEntryForm : Form
    {
        private readonly string connStr = Db.ConnectionString;

        private DataTable _table = new DataTable();

        private readonly PrintDocument _printDocument = new PrintDocument();
        private readonly PrintPreviewDialog _previewDialog = new PrintPreviewDialog();

        private int _printRowIndex = 0;
        private int _printPageNo = 1;
        private List<DataRowView> _printRows = new List<DataRowView>();

        private bool _isMovingToNextReadingCell = false;

        private TableLayoutPanel rootLayout;

        private TableLayoutPanel statsLayout;
        private Panel pnlTotalCard;
        private Panel pnlDoneCard;
        private Panel pnlPendingCard;
        private Label lblTotalCaption;
        private Label lblTotalValue;
        private Label lblDoneCaption;
        private Label lblDoneValue;
        private Label lblPendingCaption;
        private Label lblPendingValue;

        private Panel pnlFiltersCard;
        private TableLayoutPanel filtersLayout;
        private Label lblArea;
        private ComboBox cmbArea;
        private Label lblReadingDate;
        private DateTimePicker dtpReadingDate;
        private Label lblSearch;
        private TextBox txtSearch;
        private FlowLayoutPanel pnlButtons;
        private Button btnLoad;
        private Button btnSaveAll;
        private Button btnPrintSheet;
        private Button btnRefresh;

        private Panel pnlGridCard;
        private Label lblGridTitle;
        private DataGridView dgvReadings;

        private static readonly Color ThemePageBack = Color.FromArgb(244, 247, 251);
        private static readonly Color ThemeCardBack = Color.White;
        private static readonly Color ThemeBorder = Color.FromArgb(220, 228, 238);
        private static readonly Color ThemeHeader = Color.FromArgb(15, 47, 112);
        private static readonly Color ThemeText = Color.FromArgb(30, 41, 59);
        private static readonly Color ThemePrimary = Color.FromArgb(37, 99, 235);
        private static readonly Color ThemeSuccess = Color.FromArgb(22, 163, 74);
        private static readonly Color ThemeWarning = Color.FromArgb(234, 88, 12);
        private static readonly Color ThemeInfo = Color.FromArgb(14, 165, 233);
        private static readonly Color ThemeSecondary = Color.FromArgb(100, 116, 139);

        public ReadingEntryForm()
        {
            InitializeComponent();

            BuildModernLayout();

            DoubleBuffered = true;
            EnableGridDoubleBuffer();

            ApplyTheme();
            WireEvents();
            BuildGridSchema();
            LoadAreas();

            dtpReadingDate.Value = DateTime.Today;

            _previewDialog.Document = _printDocument;
            _previewDialog.WindowState = FormWindowState.Maximized;
            _previewDialog.Text = "معاينة طباعة كشف متابعة القراءات";

            _printDocument.DocumentName = "كشف متابعة تسجيل القراءات الجديدة";
            _printDocument.DefaultPageSettings.Landscape = true;
            _printDocument.DefaultPageSettings.Margins = new Margins(35, 35, 35, 45);
            _printDocument.PrintPage += PrintDocument_PrintPage;
        }

        private void BuildModernLayout()
        {
            SuspendLayout();

            Controls.Clear();

            Text = "كشف متابعة تسجيل القراءات الجديدة";
            BackColor = ThemePageBack;
            Font = new Font("Tahoma", 10F);
            RightToLeft = RightToLeft.Yes;
            RightToLeftLayout = true;
            MinimumSize = new Size(900, 520);
            AutoScroll = false;

            rootLayout = new TableLayoutPanel();
            statsLayout = new TableLayoutPanel();

            pnlTotalCard = new Panel();
            pnlDoneCard = new Panel();
            pnlPendingCard = new Panel();

            lblTotalCaption = new Label();
            lblTotalValue = new Label();
            lblDoneCaption = new Label();
            lblDoneValue = new Label();
            lblPendingCaption = new Label();
            lblPendingValue = new Label();

            pnlFiltersCard = new Panel();
            filtersLayout = new TableLayoutPanel();
            lblArea = new Label();
            cmbArea = new ComboBox();
            lblReadingDate = new Label();
            dtpReadingDate = new DateTimePicker();
            lblSearch = new Label();
            txtSearch = new TextBox();
            pnlButtons = new FlowLayoutPanel();
            btnLoad = new Button();
            btnSaveAll = new Button();
            btnPrintSheet = new Button();
            btnRefresh = new Button();

            pnlGridCard = new Panel();
            lblGridTitle = new Label();
            dgvReadings = new DataGridView();

            rootLayout.Dock = DockStyle.Fill;
            rootLayout.BackColor = ThemePageBack;
            rootLayout.ColumnCount = 1;
            rootLayout.RowCount = 3;
            rootLayout.Padding = new Padding(10, 8, 10, 14);
            rootLayout.Margin = new Padding(0);
            rootLayout.RightToLeft = RightToLeft.Yes;
            rootLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rootLayout.RowStyles.Clear();
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 96F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 140F));
            rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            BuildStatsSection();
            BuildFiltersSection();
            BuildGridSection();

            rootLayout.Controls.Add(statsLayout, 0, 0);
            rootLayout.Controls.Add(pnlFiltersCard, 0, 1);
            rootLayout.Controls.Add(pnlGridCard, 0, 2);

            Controls.Add(rootLayout);

            ResumeLayout(true);
        }

        private void BuildStatsSection()
        {
            statsLayout.Dock = DockStyle.Fill;
            statsLayout.Margin = new Padding(0, 0, 0, 10);
            statsLayout.ColumnCount = 3;
            statsLayout.RowCount = 1;
            statsLayout.RightToLeft = RightToLeft.Yes;
            statsLayout.ColumnStyles.Clear();
            statsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            statsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.333F));
            statsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.334F));
            statsLayout.RowStyles.Clear();
            statsLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            BuildStatCard(pnlTotalCard, lblTotalCaption, lblTotalValue, "إجمالي العدادات");
            BuildStatCard(pnlDoneCard, lblDoneCaption, lblDoneValue, "المسجلة");
            BuildStatCard(pnlPendingCard, lblPendingCaption, lblPendingValue, "المتبقية");

            pnlTotalCard.Margin = new Padding(4, 0, 4, 0);
            pnlDoneCard.Margin = new Padding(4, 0, 4, 0);
            pnlPendingCard.Margin = new Padding(4, 0, 4, 0);

            statsLayout.Controls.Clear();
            statsLayout.Controls.Add(pnlTotalCard, 0, 0);
            statsLayout.Controls.Add(pnlDoneCard, 1, 0);
            statsLayout.Controls.Add(pnlPendingCard, 2, 0);
        }

        private void BuildStatCard(Panel card, Label caption, Label value, string captionText)
        {
            card.Dock = DockStyle.Fill;
            card.Margin = new Padding(4);
            card.Padding = new Padding(18, 10, 18, 10);

            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.ColumnCount = 1;
            layout.RowCount = 2;
            layout.BackColor = Color.Transparent;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            caption.Text = captionText;
            caption.Dock = DockStyle.Fill;
            caption.AutoSize = false;
            caption.TextAlign = ContentAlignment.MiddleRight;
            caption.Padding = new Padding(0, 0, 0, 2);

            value.Text = "0";
            value.Dock = DockStyle.Fill;
            value.AutoSize = false;
            value.TextAlign = ContentAlignment.MiddleLeft;
            value.Padding = new Padding(0, 0, 6, 0);

            layout.Controls.Add(caption, 0, 0);
            layout.Controls.Add(value, 0, 1);

            card.Controls.Clear();
            card.Controls.Add(layout);
        }

        private void BuildFiltersSection()
        {
            pnlFiltersCard.Dock = DockStyle.Fill;
            pnlFiltersCard.Margin = new Padding(0, 0, 0, 8);
            pnlFiltersCard.Padding = new Padding(14);

            filtersLayout.Dock = DockStyle.Fill;
            filtersLayout.RightToLeft = RightToLeft.Yes;
            filtersLayout.ColumnCount = 8;
            filtersLayout.RowCount = 2;
            filtersLayout.ColumnStyles.Clear();

            filtersLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 70F));
            filtersLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            filtersLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 95F));
            filtersLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            filtersLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            filtersLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260F));
            filtersLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            filtersLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 1F));

            filtersLayout.RowStyles.Clear();
            filtersLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            filtersLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));

            lblArea.Text = "المنطقة";
            lblReadingDate.Text = "تاريخ القراءة";
            lblSearch.Text = "بحث";

            PrepareFilterLabel(lblArea);
            PrepareFilterLabel(lblReadingDate);
            PrepareFilterLabel(lblSearch);

            cmbArea.Dock = DockStyle.Fill;
            cmbArea.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbArea.Margin = new Padding(4);

            dtpReadingDate.Dock = DockStyle.Fill;
            dtpReadingDate.Margin = new Padding(4);
            dtpReadingDate.Format = DateTimePickerFormat.Custom;
            dtpReadingDate.CustomFormat = "yyyy/MM/dd";
            dtpReadingDate.RightToLeft = RightToLeft.Yes;
            dtpReadingDate.RightToLeftLayout = true;

            txtSearch.Dock = DockStyle.Fill;
            txtSearch.Margin = new Padding(4);
            txtSearch.TextAlign = HorizontalAlignment.Right;

            pnlButtons.Dock = DockStyle.Fill;
            pnlButtons.Margin = new Padding(0, 2, 0, 0);
            pnlButtons.Padding = new Padding(0);
            pnlButtons.FlowDirection = FlowDirection.RightToLeft;
            pnlButtons.WrapContents = false;
            pnlButtons.AutoSize = false;

            btnLoad.Size = new Size(130, 34);
            btnSaveAll.Size = new Size(130, 34);
            btnPrintSheet.Size = new Size(130, 34);
            btnRefresh.Size = new Size(100, 34);

            pnlButtons.Controls.Clear();
            pnlButtons.Controls.Add(btnLoad);
            pnlButtons.Controls.Add(btnSaveAll);
            pnlButtons.Controls.Add(btnPrintSheet);
            pnlButtons.Controls.Add(btnRefresh);

            filtersLayout.Controls.Add(lblArea, 0, 0);
            filtersLayout.Controls.Add(cmbArea, 1, 0);
            filtersLayout.Controls.Add(lblReadingDate, 2, 0);
            filtersLayout.Controls.Add(dtpReadingDate, 3, 0);
            filtersLayout.Controls.Add(lblSearch, 4, 0);
            filtersLayout.Controls.Add(txtSearch, 5, 0);
            filtersLayout.Controls.Add(pnlButtons, 6, 0);

            Panel spacer = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            filtersLayout.Controls.Add(spacer, 0, 1);
            filtersLayout.SetColumnSpan(spacer, 8);

            pnlFiltersCard.Controls.Clear();
            pnlFiltersCard.Controls.Add(filtersLayout);
        }

        private void PrepareFilterLabel(Label lbl)
        {
            lbl.Dock = DockStyle.Fill;
            lbl.AutoSize = false;
            lbl.TextAlign = ContentAlignment.MiddleRight;
        }

        private void BuildGridSection()
        {
            pnlGridCard.Dock = DockStyle.Fill;
            pnlGridCard.Margin = new Padding(0);
            pnlGridCard.Padding = new Padding(12, 10, 12, 18);

            TableLayoutPanel gridLayout = new TableLayoutPanel();
            gridLayout.Dock = DockStyle.Fill;
            gridLayout.ColumnCount = 1;
            gridLayout.RowCount = 2;
            gridLayout.Margin = new Padding(0);
            gridLayout.Padding = new Padding(0);
            gridLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            gridLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            gridLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            lblGridTitle.Text = "بيانات المتابعة";
            lblGridTitle.Dock = DockStyle.Fill;
            lblGridTitle.AutoSize = false;
            lblGridTitle.TextAlign = ContentAlignment.MiddleRight;

            dgvReadings.Dock = DockStyle.Fill;
            dgvReadings.Margin = new Padding(0, 0, 0, 6);
            dgvReadings.AllowUserToAddRows = false;
            dgvReadings.AllowUserToDeleteRows = false;
            dgvReadings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReadings.EditMode = DataGridViewEditMode.EditOnEnter;
            dgvReadings.RowHeadersVisible = false;
            dgvReadings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReadings.ScrollBars = ScrollBars.Both;

            gridLayout.Controls.Add(lblGridTitle, 0, 0);
            gridLayout.Controls.Add(dgvReadings, 0, 1);

            pnlGridCard.Controls.Clear();
            pnlGridCard.Controls.Add(gridLayout);
        }

        private void ApplyTheme()
        {
            BackColor = ThemePageBack;
            rootLayout.BackColor = ThemePageBack;

            StyleCard(pnlTotalCard, Color.FromArgb(237, 244, 255));
            StyleCard(pnlDoneCard, Color.FromArgb(235, 250, 242));
            StyleCard(pnlPendingCard, Color.FromArgb(255, 244, 232));

            StyleCard(pnlFiltersCard, ThemeCardBack);
            StyleCard(pnlGridCard, ThemeCardBack);

            StyleLabel(lblTotalCaption, ThemeText, 10F, FontStyle.Bold);
            StyleLabel(lblDoneCaption, ThemeText, 10F, FontStyle.Bold);
            StyleLabel(lblPendingCaption, ThemeText, 10F, FontStyle.Bold);

            StyleLabel(lblTotalValue, ThemePrimary, 22F, FontStyle.Bold);
            StyleLabel(lblDoneValue, ThemeSuccess, 22F, FontStyle.Bold);
            StyleLabel(lblPendingValue, ThemeWarning, 22F, FontStyle.Bold);

            StyleLabel(lblArea, ThemeText, 9.5F, FontStyle.Regular);
            StyleLabel(lblReadingDate, ThemeText, 9.5F, FontStyle.Regular);
            StyleLabel(lblSearch, ThemeText, 9.5F, FontStyle.Regular);
            StyleLabel(lblGridTitle, ThemeText, 10.5F, FontStyle.Bold);

            StyleInput(txtSearch);
            StyleCombo(cmbArea);
            StyleDatePicker(dtpReadingDate);

            StyleButton(btnLoad, "تحميل العدادات", ThemePrimary);
            StyleButton(btnSaveAll, "حفظ القراءات", ThemeSuccess);
            StyleButton(btnPrintSheet, "طباعة الكشف", ThemeInfo);
            StyleButton(btnRefresh, "تحديث", ThemeSecondary);

            StyleGrid();
        }

        private void StyleLabel(Label lbl, Color color, float size, FontStyle style)
        {
            lbl.ForeColor = color;
            lbl.Font = new Font("Tahoma", size, style);
            lbl.BackColor = Color.Transparent;
        }

        private void StyleInput(TextBox txt)
        {
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor = Color.White;
            txt.ForeColor = ThemeText;
            txt.Font = new Font("Tahoma", 10.5F);
        }

        private void StyleCombo(ComboBox cmb)
        {
            cmb.Font = new Font("Tahoma", 10.5F);
            cmb.BackColor = Color.White;
            cmb.ForeColor = ThemeText;
            cmb.FlatStyle = FlatStyle.Flat;
            cmb.RightToLeft = RightToLeft.Yes;
        }

        private void StyleDatePicker(DateTimePicker dtp)
        {
            dtp.Font = new Font("Tahoma", 10.5F);
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = "yyyy/MM/dd";
            dtp.RightToLeft = RightToLeft.Yes;
            dtp.RightToLeftLayout = true;
        }

        private void StyleButton(Button btn, string text, Color backColor)
        {
            btn.Text = text;
            btn.FlatStyle = FlatStyle.Flat;
            btn.UseVisualStyleBackColor = false;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.08F);
            btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.08F);
            btn.BackColor = backColor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Tahoma", 9.5F, FontStyle.Bold);
            btn.TextAlign = ContentAlignment.MiddleCenter;
            btn.TextImageRelation = TextImageRelation.TextBeforeImage;
            btn.Cursor = Cursors.Hand;
            btn.Margin = new Padding(5, 0, 5, 0);
            btn.Padding = new Padding(0);
            btn.AutoEllipsis = false;
        }

        private void StyleCard(Panel panel, Color backColor)
        {
            panel.BackColor = backColor;
            panel.BorderStyle = BorderStyle.None;

            panel.Paint -= DrawCardBorder;
            panel.Paint += DrawCardBorder;

            panel.Resize -= Panel_Resize;
            panel.Resize += Panel_Resize;
        }

        private void Panel_Resize(object sender, EventArgs e)
        {
            Control c = sender as Control;
            if (c != null)
                c.Invalidate();
        }

        private void DrawCardBorder(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle shadowRect = new Rectangle(2, 2, Math.Max(1, panel.Width - 5), Math.Max(1, panel.Height - 5));
            Rectangle rect = new Rectangle(0, 0, Math.Max(1, panel.Width - 2), Math.Max(1, panel.Height - 2));

            using (GraphicsPath shadowPath = CreateRoundRectanglePath(shadowRect, 8))
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(18, 15, 23, 42)))
            {
                e.Graphics.FillPath(shadowBrush, shadowPath);
            }

            using (GraphicsPath path = CreateRoundRectanglePath(rect, 8))
            using (SolidBrush backBrush = new SolidBrush(panel.BackColor))
            using (Pen pen = new Pen(ThemeBorder, 1F))
            {
                e.Graphics.FillPath(backBrush, path);
                e.Graphics.DrawPath(pen, path);
            }
        }

        private GraphicsPath CreateRoundRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }

        private void StyleGrid()
        {
            dgvReadings.EnableHeadersVisualStyles = false;
            dgvReadings.RightToLeft = RightToLeft.Yes;
            dgvReadings.BackgroundColor = Color.White;
            dgvReadings.BorderStyle = BorderStyle.None;
            dgvReadings.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvReadings.GridColor = Color.FromArgb(226, 232, 240);
            dgvReadings.RowHeadersVisible = false;
            dgvReadings.MultiSelect = false;
            dgvReadings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReadings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvReadings.AllowUserToAddRows = false;
            dgvReadings.AllowUserToDeleteRows = false;
            dgvReadings.AllowUserToResizeRows = false;
            dgvReadings.EditMode = DataGridViewEditMode.EditOnEnter;
            dgvReadings.ScrollBars = ScrollBars.Both;

            dgvReadings.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvReadings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvReadings.ColumnHeadersHeight = 38;

            dgvReadings.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            dgvReadings.ColumnHeadersDefaultCellStyle.ForeColor = ThemeText;
            dgvReadings.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.5F, FontStyle.Bold);
            dgvReadings.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvReadings.DefaultCellStyle.BackColor = Color.White;
            dgvReadings.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            dgvReadings.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgvReadings.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgvReadings.DefaultCellStyle.Font = new Font("Tahoma", 9.5F);
            dgvReadings.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvReadings.DefaultCellStyle.Padding = new Padding(1);

            dgvReadings.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvReadings.RowTemplate.Height = 32;
            dgvReadings.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        private void WireEvents()
        {
            btnLoad.Click += (s, e) => LoadMetersForFollowUp();
            btnSaveAll.Click += (s, e) => SaveAllReadingsWithProcedure();
            btnPrintSheet.Click += (s, e) => PreviewSheet();
            btnRefresh.Click += (s, e) => LoadMetersForFollowUp();

            txtSearch.TextChanged += (s, e) => ApplyFilter();

            dgvReadings.CellEndEdit += DgvReadings_CellEndEdit;
            dgvReadings.DataError += (s, e) => { e.Cancel = false; };

            dgvReadings.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvReadings.IsCurrentCellDirty)
                    dgvReadings.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
        }

        private void LoadAreas()
        {
            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection cn = new SqlConnection(connStr))
                using (SqlDataAdapter da = new SqlDataAdapter(@"
SELECT DISTINCT
    ISNULL(NULLIF(LTRIM(RTRIM(Location)), ''), N'بدون تحديد') AS AreaName
FROM dbo.Meters
WHERE IsActive = 1
ORDER BY AreaName;", cn))
                {
                    da.Fill(dt);
                }

                DataRow allRow = dt.NewRow();
                allRow["AreaName"] = "الكل";
                dt.Rows.InsertAt(allRow, 0);

                cmbArea.DisplayMember = "AreaName";
                cmbArea.ValueMember = "AreaName";
                cmbArea.DataSource = dt;
            }
            catch
            {
                cmbArea.Items.Clear();
                cmbArea.Items.Add("الكل");
                cmbArea.SelectedIndex = 0;
            }
        }

        private void BuildGridSchema()
        {
            _table = new DataTable();

            _table.Columns.Add("SubscriberID", typeof(int));
            _table.Columns.Add("MeterID", typeof(int));
            _table.Columns.Add("م", typeof(int));
            _table.Columns.Add("رقم الحساب", typeof(string));
            _table.Columns.Add("اسم المشترك", typeof(string));
            _table.Columns.Add("رقم العداد", typeof(string));
            _table.Columns.Add("الموقع", typeof(string));
            _table.Columns.Add("تاريخ آخر قراءة", typeof(string));
            _table.Columns.Add("القراءة السابقة", typeof(decimal));
            _table.Columns.Add("القراءة الجديدة", typeof(string));
            _table.Columns.Add("الاستهلاك", typeof(decimal));
            _table.Columns.Add("ملاحظات", typeof(string));
            _table.Columns.Add("تم", typeof(bool));

            dgvReadings.DataSource = _table;

            FormatGrid();
            UpdateSummary();
        }

        private void LoadMetersForFollowUp()
        {
            try
            {
                string area = cmbArea.Text.Trim();

                using (SqlConnection cn = new SqlConnection(connStr))
                using (SqlDataAdapter da = new SqlDataAdapter(@"
SELECT DISTINCT
    s.SubscriberID,
    m.MeterID,
    ISNULL(a.AccountCode, CAST(s.AccountID AS nvarchar(50))) AS AccountNo,
    s.Name AS SubscriberName,
    m.MeterNumber,
    ISNULL(NULLIF(LTRIM(RTRIM(m.Location)), ''), N'بدون تحديد') AS MeterLocation,
    ISNULL(lastR.CurrentReading, 0) AS PreviousReading,
    CASE 
        WHEN lastR.ReadingDate IS NULL THEN N'لا توجد'
        ELSE CONVERT(nvarchar(10), lastR.ReadingDate, 111)
    END AS LastReadingDate
FROM dbo.Subscribers s
INNER JOIN dbo.SubscriberMeters sm ON sm.SubscriberID = s.SubscriberID
INNER JOIN dbo.Meters m ON m.MeterID = sm.MeterID
LEFT JOIN dbo.Accounts a ON a.AccountID = s.AccountID
OUTER APPLY
(
    SELECT TOP 1 
        r.CurrentReading,
        r.ReadingDate
    FROM dbo.Readings r
    WHERE r.SubscriberID = s.SubscriberID
      AND r.MeterID = m.MeterID
    ORDER BY r.ReadingDate DESC, r.ReadingID DESC
) lastR
WHERE s.IsActive = 1
  AND m.IsActive = 1
  AND
  (
      @Area = N'الكل'
      OR ISNULL(NULLIF(LTRIM(RTRIM(m.Location)), ''), N'بدون تحديد') = @Area
  )
ORDER BY MeterLocation, SubscriberName, MeterNumber;", cn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@Area", area);

                    DataTable source = new DataTable();
                    da.Fill(source);

                    BuildGridSchema();

                    int i = 1;

                    foreach (DataRow src in source.Rows)
                    {
                        DataRow row = _table.NewRow();

                        row["SubscriberID"] = Convert.ToInt32(src["SubscriberID"]);
                        row["MeterID"] = Convert.ToInt32(src["MeterID"]);
                        row["م"] = i++;
                        row["رقم الحساب"] = src["AccountNo"] == DBNull.Value ? "" : src["AccountNo"].ToString();
                        row["اسم المشترك"] = src["SubscriberName"] == DBNull.Value ? "" : src["SubscriberName"].ToString();
                        row["رقم العداد"] = src["MeterNumber"] == DBNull.Value ? "" : src["MeterNumber"].ToString();
                        row["الموقع"] = src["MeterLocation"] == DBNull.Value ? "" : src["MeterLocation"].ToString();
                        row["تاريخ آخر قراءة"] = src["LastReadingDate"] == DBNull.Value ? "لا توجد" : src["LastReadingDate"].ToString();
                        row["القراءة السابقة"] = src["PreviousReading"] == DBNull.Value ? 0m : Convert.ToDecimal(src["PreviousReading"]);
                        row["القراءة الجديدة"] = "";
                        row["الاستهلاك"] = 0m;
                        row["ملاحظات"] = "";
                        row["تم"] = false;

                        _table.Rows.Add(row);
                    }
                }

                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                    ApplyFilter();
                else
                    UpdateSummary();

                lblGridTitle.Text = "بيانات المتابعة";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "تعذر تحميل كشف العدادات.\n\n" + ex.Message,
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void FormatGrid()
        {
            if (dgvReadings.Columns.Count == 0)
                return;

            StyleGrid();

            foreach (DataGridViewColumn col in dgvReadings.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                col.Resizable = DataGridViewTriState.True;
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgvReadings.Columns.Contains("SubscriberID"))
                dgvReadings.Columns["SubscriberID"].Visible = false;

            if (dgvReadings.Columns.Contains("MeterID"))
                dgvReadings.Columns["MeterID"].Visible = false;

            ConfigureGridColumn("م", 45, true, DataGridViewContentAlignment.MiddleCenter);
            ConfigureGridColumn("رقم الحساب", 105, true, DataGridViewContentAlignment.MiddleCenter);
            ConfigureGridColumn("اسم المشترك", 190, true, DataGridViewContentAlignment.MiddleRight);
            ConfigureGridColumn("رقم العداد", 105, true, DataGridViewContentAlignment.MiddleCenter);
            ConfigureGridColumn("الموقع", 150, true, DataGridViewContentAlignment.MiddleRight);
            ConfigureGridColumn("تاريخ آخر قراءة", 105, true, DataGridViewContentAlignment.MiddleCenter);
            ConfigureGridColumn("القراءة السابقة", 105, true, DataGridViewContentAlignment.MiddleCenter);
            ConfigureGridColumn("القراءة الجديدة", 115, false, DataGridViewContentAlignment.MiddleCenter);
            ConfigureGridColumn("الاستهلاك", 90, true, DataGridViewContentAlignment.MiddleCenter);
            ConfigureGridColumn("ملاحظات", 150, false, DataGridViewContentAlignment.MiddleRight);
            ConfigureGridColumn("تم", 55, true, DataGridViewContentAlignment.MiddleCenter);

            if (dgvReadings.Columns.Contains("القراءة السابقة"))
                dgvReadings.Columns["القراءة السابقة"].DefaultCellStyle.Format = "0.##";

            if (dgvReadings.Columns.Contains("الاستهلاك"))
                dgvReadings.Columns["الاستهلاك"].DefaultCellStyle.Format = "0.##";

            if (dgvReadings.Columns.Contains("القراءة الجديدة"))
            {
                dgvReadings.Columns["القراءة الجديدة"].DefaultCellStyle.BackColor = Color.FromArgb(255, 251, 235);
                dgvReadings.Columns["القراءة الجديدة"].DefaultCellStyle.ForeColor = Color.FromArgb(120, 53, 15);
                dgvReadings.Columns["القراءة الجديدة"].DefaultCellStyle.SelectionBackColor = Color.FromArgb(254, 243, 199);
                dgvReadings.Columns["القراءة الجديدة"].DefaultCellStyle.SelectionForeColor = Color.FromArgb(120, 53, 15);
                dgvReadings.Columns["القراءة الجديدة"].DefaultCellStyle.Font = new Font("Tahoma", 9.5F, FontStyle.Bold);
            }

            if (dgvReadings.Columns.Contains("تاريخ آخر قراءة"))
            {
                dgvReadings.Columns["تاريخ آخر قراءة"].DefaultCellStyle.BackColor = Color.FromArgb(239, 246, 255);
                dgvReadings.Columns["تاريخ آخر قراءة"].DefaultCellStyle.ForeColor = Color.FromArgb(30, 64, 175);
            }

            if (dgvReadings.Columns.Contains("ملاحظات"))
                dgvReadings.Columns["ملاحظات"].DefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            if (dgvReadings.Columns.Contains("تم"))
                dgvReadings.Columns["تم"].DefaultCellStyle.BackColor = Color.FromArgb(240, 253, 244);

            SetColumnDisplayIndexes();
        }

        private void ConfigureGridColumn(string columnName, float fillWeight, bool readOnly, DataGridViewContentAlignment alignment)
        {
            if (!dgvReadings.Columns.Contains(columnName))
                return;

            DataGridViewColumn col = dgvReadings.Columns[columnName];
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            col.FillWeight = fillWeight;
            col.MinimumWidth = Math.Max(45, (int)fillWeight);
            col.ReadOnly = readOnly;
            col.DefaultCellStyle.Alignment = alignment;
        }

        private void SetColumnDisplayIndexes()
        {
            string[] order =
            {
                "م",
                "رقم الحساب",
                "اسم المشترك",
                "رقم العداد",
                "الموقع",
                "تاريخ آخر قراءة",
                "القراءة السابقة",
                "القراءة الجديدة",
                "الاستهلاك",
                "ملاحظات",
                "تم"
            };

            for (int i = 0; i < order.Length; i++)
            {
                if (dgvReadings.Columns.Contains(order[i]))
                    dgvReadings.Columns[order[i]].DisplayIndex = i;
            }
        }

        private void EnableGridDoubleBuffer()
        {
            try
            {
                var prop = typeof(DataGridView).GetProperty(
                    "DoubleBuffered",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic);

                if (prop != null)
                    prop.SetValue(dgvReadings, true, null);
            }
            catch
            {
            }
        }

        private void ApplyFilter()
        {
            if (_table == null || _table.DefaultView == null)
                return;

            string keyword = txtSearch.Text.Trim().Replace("'", "''");

            if (string.IsNullOrWhiteSpace(keyword))
            {
                _table.DefaultView.RowFilter = "";
            }
            else
            {
                _table.DefaultView.RowFilter =
                    "[اسم المشترك] LIKE '%" + keyword + "%' " +
                    "OR [رقم الحساب] LIKE '%" + keyword + "%' " +
                    "OR [رقم العداد] LIKE '%" + keyword + "%' " +
                    "OR [الموقع] LIKE '%" + keyword + "%' " +
                    "OR [تاريخ آخر قراءة] LIKE '%" + keyword + "%'";
            }

            UpdateSummaryFromView();
        }

        private void UpdateSummary()
        {
            int total = _table.Rows.Count;

            int done = _table.Rows.Cast<DataRow>()
                .Count(r => r["تم"] != DBNull.Value && Convert.ToBoolean(r["تم"]));

            lblTotalValue.Text = total.ToString();
            lblDoneValue.Text = done.ToString();
            lblPendingValue.Text = (total - done).ToString();
        }

        private void UpdateSummaryFromView()
        {
            int total = _table.DefaultView.Count;
            int done = 0;

            foreach (DataRowView drv in _table.DefaultView)
            {
                if (drv["تم"] != DBNull.Value && Convert.ToBoolean(drv["تم"]))
                    done++;
            }

            lblTotalValue.Text = total.ToString();
            lblDoneValue.Text = done.ToString();
            lblPendingValue.Text = (total - done).ToString();
        }

        private void DgvReadings_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (e.RowIndex >= dgvReadings.Rows.Count)
                return;

            DataGridViewRow row = dgvReadings.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            string colName = dgvReadings.Columns[e.ColumnIndex].Name;

            if (colName == "القراءة الجديدة")
            {
                HandleReadingEdit(row, e.RowIndex);
            }
            else if (colName == "ملاحظات")
            {
                string current = Convert.ToString(row.Cells["القراءة الجديدة"].Value);
                decimal temp;

                if (TryParseDecimal(current, out temp))
                    row.Cells["تم"].Value = true;
            }

            UpdateSummaryFromView();
        }

        private void HandleReadingEdit(DataGridViewRow row, int rowIndex)
        {
            decimal previous = ToDecimal(row.Cells["القراءة السابقة"].Value);
            decimal current;

            string currentText = row.Cells["القراءة الجديدة"].Value == null
                ? ""
                : row.Cells["القراءة الجديدة"].Value.ToString().Trim();

            if (string.IsNullOrWhiteSpace(currentText))
            {
                row.Cells["الاستهلاك"].Value = 0m;
                row.Cells["تم"].Value = false;
                ResetRowStyle(row);
                return;
            }

            if (!TryParseDecimal(currentText, out current))
            {
                row.Cells["الاستهلاك"].Value = 0m;
                row.Cells["تم"].Value = false;
                MarkRowInvalid(row);

                MessageBox.Show(
                    "القراءة الجديدة غير صحيحة.",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            if (current < previous)
            {
                row.Cells["القراءة الجديدة"].Value = "";
                row.Cells["الاستهلاك"].Value = 0m;
                row.Cells["تم"].Value = false;
                MarkRowInvalid(row);

                string lastDate = "";
                if (row.Cells["تاريخ آخر قراءة"] != null && row.Cells["تاريخ آخر قراءة"].Value != null)
                    lastDate = row.Cells["تاريخ آخر قراءة"].Value.ToString();

                MessageBox.Show(
                    "القراءة الجديدة لا يمكن أن تكون أقل من القراءة السابقة." +
                    "\n\nالقراءة السابقة: " + previous.ToString("0.##") +
                    "\nتاريخ آخر قراءة: " + (string.IsNullOrWhiteSpace(lastDate) ? "غير محدد" : lastDate),
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            row.Cells["القراءة الجديدة"].Value = current.ToString("0.##");
            row.Cells["الاستهلاك"].Value = current - previous;
            row.Cells["تم"].Value = true;

            MarkRowValid(row);
            MoveToNextReadingCellDeferred(rowIndex);
        }

        private void MoveToNextReadingCellDeferred(int rowIndex)
        {
            if (_isMovingToNextReadingCell)
                return;

            if (rowIndex < 0 || rowIndex >= dgvReadings.Rows.Count - 1)
                return;

            _isMovingToNextReadingCell = true;

            try
            {
                dgvReadings.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (IsDisposed || dgvReadings.IsDisposed || !dgvReadings.IsHandleCreated)
                            return;

                        if (rowIndex < 0 || rowIndex >= dgvReadings.Rows.Count - 1)
                            return;

                        DataGridViewRow nextRow = dgvReadings.Rows[rowIndex + 1];

                        if (nextRow == null || nextRow.IsNewRow || !nextRow.Visible)
                            return;

                        DataGridViewCell nextCell = nextRow.Cells["القراءة الجديدة"];

                        if (nextCell == null || nextCell.ReadOnly)
                            return;

                        dgvReadings.CurrentCell = nextCell;
                        dgvReadings.BeginEdit(true);
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _isMovingToNextReadingCell = false;
                    }
                }));
            }
            catch
            {
                _isMovingToNextReadingCell = false;
            }
        }

        private void ResetRowStyle(DataGridViewRow row)
        {
            row.DefaultCellStyle.BackColor = Color.White;
            row.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
        }

        private void MarkRowValid(DataGridViewRow row)
        {
            row.DefaultCellStyle.BackColor = Color.FromArgb(236, 253, 245);
            row.DefaultCellStyle.ForeColor = Color.FromArgb(22, 101, 52);
        }

        private void MarkRowInvalid(DataGridViewRow row)
        {
            row.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242);
            row.DefaultCellStyle.ForeColor = Color.FromArgb(153, 27, 27);
        }

        private void SaveAllReadingsWithProcedure()
        {
            dgvReadings.EndEdit();

            if (_table.Rows.Count == 0)
            {
                MessageBox.Show(
                    "لا توجد عدادات محملة للحفظ.",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            List<DataRow> rowsToSave = new List<DataRow>();

            foreach (DataRow row in _table.Rows)
            {
                decimal currentReading;

                if (TryParseDecimal(Convert.ToString(row["القراءة الجديدة"]), out currentReading))
                    rowsToSave.Add(row);
            }

            if (rowsToSave.Count == 0)
            {
                MessageBox.Show(
                    "لم يتم إدخال أي قراءة جديدة بعد.",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            int successCount = 0;
            List<string> errors = new List<string>();

            using (SqlConnection cn = new SqlConnection(connStr))
            {
                cn.Open();

                foreach (DataRow row in rowsToSave)
                {
                    int subscriberId = Convert.ToInt32(row["SubscriberID"]);
                    int meterId = Convert.ToInt32(row["MeterID"]);
                    string subscriberName = Convert.ToString(row["اسم المشترك"]);
                    string meterNumber = Convert.ToString(row["رقم العداد"]);
                    decimal currentReading = ToDecimal(row["القراءة الجديدة"]);
                    string notes = row["ملاحظات"] == DBNull.Value ? "" : Convert.ToString(row["ملاحظات"]);

                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("dbo.AddReadingAndGenerateInvoice", cn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 120;

                            cmd.Parameters.AddWithValue("@SubscriberID", subscriberId);
                            cmd.Parameters.AddWithValue("@MeterID", meterId);
                            cmd.Parameters.AddWithValue("@ReadingDate", dtpReadingDate.Value.Date);
                            cmd.Parameters.AddWithValue("@CurrentReading", currentReading);

                            cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = DBNull.Value;
                            cmd.Parameters.Add("@ServiceFees", SqlDbType.Decimal).Value = DBNull.Value;

                            if (string.IsNullOrWhiteSpace(notes))
                                cmd.Parameters.AddWithValue("@Notes", DBNull.Value);
                            else
                                cmd.Parameters.AddWithValue("@Notes", notes);

                            cmd.ExecuteNonQuery();
                        }

                        successCount++;
                        row["تم"] = true;
                        MarkRowValidByDataRow(row);
                    }
                    catch (SqlException ex)
                    {
                        string errorText = ExtractSqlErrorMessage(ex);
                        errors.Add("- " + subscriberName + " / " + meterNumber + ": " + errorText);
                        row["تم"] = false;
                    }
                    catch (Exception ex)
                    {
                        errors.Add("- " + subscriberName + " / " + meterNumber + ": " + ex.Message);
                        row["تم"] = false;
                    }
                }
            }

            UpdateSummary();

            if (successCount > 0 && errors.Count == 0)
            {
                MessageBox.Show(
                    "تم حفظ " + successCount + " قراءة وإنشاء الفواتير بنجاح ✅",
                    "نجاح",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadMetersForFollowUp();
                return;
            }

            if (successCount > 0 && errors.Count > 0)
            {
                string msg =
                    "تم حفظ " + successCount + " قراءة بنجاح، وتعذر حفظ " + errors.Count + " قراءة.\n\n" +
                    string.Join("\n", errors.Take(10));

                if (errors.Count > 10)
                    msg += "\n\n... وهناك " + (errors.Count - 10) + " أخطاء إضافية";

                MessageBox.Show(
                    msg,
                    "حفظ جزئي",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                LoadMetersForFollowUp();
                return;
            }

            MessageBox.Show(
                "تعذر حفظ القراءات.\n\n" + string.Join("\n", errors.Take(10)),
                "خطأ",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private string ExtractSqlErrorMessage(SqlException ex)
        {
            if (ex == null)
                return "خطأ غير معروف";

            foreach (SqlError err in ex.Errors)
            {
                if (!string.IsNullOrWhiteSpace(err.Message))
                    return err.Message;
            }

            return ex.Message;
        }

        private void MarkRowValidByDataRow(DataRow row)
        {
            foreach (DataGridViewRow gridRow in dgvReadings.Rows)
            {
                DataRowView drv = gridRow.DataBoundItem as DataRowView;

                if (drv != null && drv.Row == row)
                {
                    MarkRowValid(gridRow);
                    break;
                }
            }
        }

        private sealed class CompanyPrintInfo
        {
            public string Name { get; set; }
            public string Subtitle { get; set; }
            public string Address { get; set; }
            public string Mobile { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string AccountNo { get; set; }
            public string LogoPath { get; set; }
        }

        private CompanyPrintInfo LoadCompanyPrintInfo()
        {
            CompanyPrintInfo info = new CompanyPrintInfo
            {
                Name = "مؤسسة المياه",
                Subtitle = "نظام إدارة المياه",
                Address = "",
                Mobile = "",
                Phone = "",
                Email = "",
                AccountNo = "",
                LogoPath = ""
            };

            try
            {
                Dictionary<string, string> map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                using (SqlConnection cn = new SqlConnection(connStr))
                using (SqlCommand cmd = new SqlCommand(@"
DECLARE @T TABLE
(
    SettingKey nvarchar(200),
    SettingValue nvarchar(max)
);

IF OBJECT_ID(N'dbo.AppSettings', N'U') IS NOT NULL
BEGIN
    INSERT INTO @T(SettingKey, SettingValue)
    SELECT SettingKey, SettingValue
    FROM dbo.AppSettings
    WHERE SettingKey IS NOT NULL;
END

IF OBJECT_ID(N'dbo.Settings', N'U') IS NOT NULL
BEGIN
    INSERT INTO @T(SettingKey, SettingValue)
    SELECT SettingName, SettingValue
    FROM dbo.Settings
    WHERE SettingName IS NOT NULL;
END

SELECT SettingKey, SettingValue
FROM @T;", cn))
                {
                    cn.Open();

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            string key = rd["SettingKey"] == DBNull.Value ? "" : rd["SettingKey"].ToString();
                            string val = rd["SettingValue"] == DBNull.Value ? "" : rd["SettingValue"].ToString();

                            if (!string.IsNullOrWhiteSpace(key))
                                map[key.Trim()] = (val ?? "").Trim();
                        }
                    }
                }

                info.Name = PickSetting(map, info.Name, "Company.Name", "CompanyName", "اسم الشركة", "System.Name");
                info.Subtitle = PickSetting(map, info.Subtitle, "Company.SystemName", "CompanySubtitle", "وصف الشركة", "System.Name");
                info.Address = PickSetting(map, info.Address, "Company.Address", "CompanyAddress", "عنوان الشركة");
                info.Phone = PickSetting(map, info.Phone, "Company.Phone", "CompanyPhone", "هاتف الشركة");
                info.Mobile = PickSetting(map, info.Mobile, "Company.Mobile", "CompanyMobile", "موبايل الشركة");
                info.Email = PickSetting(map, info.Email, "Company.Email", "CompanyEmail", "البريد الإلكتروني");
                info.AccountNo = PickSetting(map, info.AccountNo, "Company.AccountNo", "Company.BankAccount", "Company.AccountNumber", "رقم الحساب");
                info.LogoPath = PickSetting(map, info.LogoPath, "Company.LogoPath", "CompanyLogoPath", "شعار الشركة");
            }
            catch
            {
                // في حالة عدم وجود جدول الإعدادات، يتم استخدام القيم الافتراضية.
            }

            return info;
        }

        private string PickSetting(Dictionary<string, string> map, string fallback, params string[] keys)
        {
            foreach (string key in keys)
            {
                if (map.ContainsKey(key) && !string.IsNullOrWhiteSpace(map[key]))
                    return map[key];
            }

            return fallback ?? "";
        }

        private int DrawCompanyHeader(Graphics g, Rectangle margin, int y, string reportTitle)
        {
            CompanyPrintInfo company = LoadCompanyPrintInfo();

            int headerHeight = 132;
            Rectangle headerRect = new Rectangle(margin.Left, y, margin.Width, headerHeight);

            using (SolidBrush headerBack = new SolidBrush(Color.White))
            using (Pen borderPen = new Pen(Color.FromArgb(90, 90, 90), 1))
            {
                g.FillRectangle(headerBack, headerRect);
                g.DrawRectangle(borderPen, headerRect);
            }

            Rectangle logoRect = new Rectangle(headerRect.Right - 88, headerRect.Top + 12, 66, 66);

            if (!string.IsNullOrWhiteSpace(company.LogoPath) && File.Exists(company.LogoPath))
            {
                try
                {
                    using (Image logo = Image.FromFile(company.LogoPath))
                    {
                        g.DrawImage(logo, logoRect);
                    }
                }
                catch
                {
                    DrawArabicCentered(g, "الشعار", new Font("Tahoma", 8F, FontStyle.Regular), Brushes.Gray, logoRect);
                }
            }

            Rectangle companyTextRect = new Rectangle(
                headerRect.Left + 22,
                headerRect.Top + 8,
                headerRect.Width - 132,
                86
            );

            string addressLine = company.Address ?? "";
            string contactLine = BuildCompanyContactLine(company);

            using (Font companyFont = new Font("Tahoma", 16F, FontStyle.Bold))
            using (Font subFont = new Font("Tahoma", 10.5F, FontStyle.Bold))
            using (Font infoFont = new Font("Tahoma", 9F, FontStyle.Regular))
            using (Font reportFont = new Font("Tahoma", 10F, FontStyle.Bold))
            using (SolidBrush darkBrush = new SolidBrush(ThemeHeader))
            using (SolidBrush textBrush = new SolidBrush(Color.Black))
            using (Pen linePen = new Pen(Color.FromArgb(180, 180, 180)))
            {
                Rectangle r1 = new Rectangle(companyTextRect.Left, companyTextRect.Top, companyTextRect.Width, 28);
                Rectangle r2 = new Rectangle(companyTextRect.Left, companyTextRect.Top + 28, companyTextRect.Width, 22);
                Rectangle r3 = new Rectangle(companyTextRect.Left, companyTextRect.Top + 51, companyTextRect.Width, 19);
                Rectangle r4 = new Rectangle(companyTextRect.Left, companyTextRect.Top + 70, companyTextRect.Width, 19);

                DrawArabicCentered(g, company.Name, companyFont, darkBrush, r1);
                DrawArabicCentered(g, company.Subtitle, subFont, textBrush, r2);
                DrawArabicCentered(g, addressLine, infoFont, textBrush, r3);
                DrawArabicCentered(g, contactLine, infoFont, textBrush, r4);

                int lineY = headerRect.Top + 96;
                g.DrawLine(linePen, headerRect.Left + 10, lineY, headerRect.Right - 10, lineY);

                Rectangle reportRect = new Rectangle(headerRect.Left + 10, lineY + 3, headerRect.Width - 20, 28);
                DrawArabicCentered(g, reportTitle, reportFont, darkBrush, reportRect);
            }

            return headerRect.Bottom + 12;
        }

        private string BuildCompanyContactLine(CompanyPrintInfo company)
        {
            List<string> parts = new List<string>();

            if (!string.IsNullOrWhiteSpace(company.Mobile))
                parts.Add("موبايل: " + company.Mobile);

            if (!string.IsNullOrWhiteSpace(company.Phone))
                parts.Add("هاتف: " + company.Phone);

            if (!string.IsNullOrWhiteSpace(company.Email))
                parts.Add("البريد: " + company.Email);

            if (!string.IsNullOrWhiteSpace(company.AccountNo))
                parts.Add("رقم الحساب: " + company.AccountNo);

            return string.Join("    |    ", parts);
        }

        private void DrawArabicCentered(Graphics g, string text, Font font, Brush brush, Rectangle rect)
        {
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                sf.Trimming = StringTrimming.EllipsisCharacter;

                g.DrawString(text ?? "", font, brush, rect, sf);
            }
        }

        private void DrawArabicRight(Graphics g, string text, Font font, Brush brush, Rectangle rect)
        {
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Near;
                sf.LineAlignment = StringAlignment.Center;
                sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                sf.Trimming = StringTrimming.EllipsisCharacter;

                Rectangle padded = new Rectangle(rect.X + 4, rect.Y, Math.Max(1, rect.Width - 8), rect.Height);
                g.DrawString(text ?? "", font, brush, padded, sf);
            }
        }

        private void PreviewSheet()
        {
            if (_table.Rows.Count == 0)
            {
                MessageBox.Show(
                    "قم بتحميل العدادات أولاً.",
                    "تنبيه",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                return;
            }

            ResetPrintState();
            _previewDialog.ShowDialog(this);
        }

        private void ResetPrintState()
        {
            _printRowIndex = 0;
            _printPageNo = 1;
            _printRows = _table.DefaultView.Cast<DataRowView>().ToList();
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle m = e.MarginBounds;

            g.PageUnit = GraphicsUnit.Display;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            using (Font normalFont = new Font("Tahoma", 8.3F, FontStyle.Regular))
            using (Font headerFont = new Font("Tahoma", 8.5F, FontStyle.Bold))
            using (Pen pen = new Pen(Color.Black))
            {
                int y = m.Top;

                string reportTitle =
                    "كشف متابعة تسجيل القراءات الجديدة"
                    + "    |    المنطقة: " + cmbArea.Text
                    + "    |    تاريخ القراءة: " + dtpReadingDate.Value.ToString("yyyy/MM/dd")
                    + "    |    عدد السجلات: " + _printRows.Count;

                y = DrawCompanyHeader(g, m, y, reportTitle);

                string[] headers =
                {
                    "م",
                    "رقم الحساب",
                    "اسم المشترك",
                    "رقم العداد",
                    "الموقع",
                    "تاريخ آخر قراءة",
                    "القراءة السابقة",
                    "القراءة الجديدة",
                    "الاستهلاك",
                    "ملاحظات"
                };

                int[] widths = GetPrintColumnWidths(m.Width);

                int headerHeight = 30;
                int rowHeight = 25;
                int bottomLimit = m.Bottom - 34;

                DrawPrintRowRtl(
                    g,
                    headers,
                    widths,
                    m,
                    y,
                    headerHeight,
                    headerFont,
                    pen,
                    true,
                    -1
                );

                y += headerHeight;

                while (_printRowIndex < _printRows.Count)
                {
                    if (y + rowHeight > bottomLimit)
                    {
                        DrawPrintFooter(g, m);
                        _printPageNo++;
                        e.HasMorePages = true;
                        return;
                    }

                    DataRowView drv = _printRows[_printRowIndex];

                    string[] vals =
                    {
                        Convert.ToString(drv["م"]),
                        Convert.ToString(drv["رقم الحساب"]),
                        Convert.ToString(drv["اسم المشترك"]),
                        Convert.ToString(drv["رقم العداد"]),
                        Convert.ToString(drv["الموقع"]),
                        Convert.ToString(drv["تاريخ آخر قراءة"]),
                        FormatNumber(drv["القراءة السابقة"]),
                        Convert.ToString(drv["القراءة الجديدة"]),
                        FormatNumber(drv["الاستهلاك"]),
                        Convert.ToString(drv["ملاحظات"])
                    };

                    DrawPrintRowRtl(
                        g,
                        vals,
                        widths,
                        m,
                        y,
                        rowHeight,
                        normalFont,
                        pen,
                        false,
                        _printRowIndex
                    );

                    y += rowHeight;
                    _printRowIndex++;
                }

                DrawPrintFooter(g, m);

                e.HasMorePages = false;
                _printRowIndex = 0;
            }
        }

        private void DrawPrintRowRtl(
            Graphics g,
            string[] cells,
            int[] widths,
            Rectangle margin,
            int y,
            int height,
            Font font,
            Pen borderPen,
            bool isHeader,
            int rowIndex)
        {
            Color rowBack = rowIndex % 2 == 0 ? Color.White : Color.FromArgb(248, 250, 252);
            Color backColor = isHeader ? ThemeHeader : rowBack;
            Brush textBrush = isHeader ? Brushes.White : Brushes.Black;

            using (SolidBrush backBrush = new SolidBrush(backColor))
            {
                int right = margin.Right;

                for (int i = 0; i < cells.Length; i++)
                {
                    Rectangle rect = new Rectangle(
                        right - widths[i],
                        y,
                        widths[i],
                        height
                    );

                    g.FillRectangle(backBrush, rect);
                    g.DrawRectangle(borderPen, rect);

                    string text = CleanPrintText(cells[i], i == 9 ? 28 : 22);

                    if (!isHeader && (i == 2 || i == 4 || i == 9))
                        DrawArabicRight(g, text, font, textBrush, rect);
                    else
                        DrawArabicCentered(g, text, font, textBrush, rect);

                    right -= widths[i];
                }
            }
        }

        private void DrawPrintFooter(Graphics g, Rectangle margin)
        {
            using (Font footerFont = new Font("Tahoma", 8F, FontStyle.Regular))
            using (SolidBrush footerBrush = new SolidBrush(Color.Gray))
            {
                Rectangle footerRect = new Rectangle(margin.Left, margin.Bottom + 8, margin.Width, 22);

                string footer =
                    "صفحة: " + _printPageNo
                    + "    |    تاريخ الطباعة: " + DateTime.Now.ToString("yyyy/MM/dd HH:mm");

                DrawArabicCentered(g, footer, footerFont, footerBrush, footerRect);
            }
        }

        private string CleanPrintText(string text, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            text = text.Replace("\r", " ").Replace("\n", " ").Trim();

            if (text.Length <= maxLength)
                return text;

            return text.Substring(0, maxLength) + "...";
        }

        private int[] GetPrintColumnWidths(int totalWidth)
        {
            double[] ratios =
            {
                0.045, // م
                0.090, // رقم الحساب
                0.170, // اسم المشترك
                0.095, // رقم العداد
                0.130, // الموقع
                0.095, // تاريخ آخر قراءة
                0.095, // القراءة السابقة
                0.095, // القراءة الجديدة
                0.075, // الاستهلاك
                0.110  // ملاحظات
            };

            int[] widths = new int[ratios.Length];
            int used = 0;

            for (int i = 0; i < ratios.Length; i++)
            {
                widths[i] = (int)Math.Floor(totalWidth * ratios[i]);
                used += widths[i];
            }

            int diff = totalWidth - used;

            if (widths.Length > 0)
                widths[widths.Length - 1] += diff;

            return widths;
        }

        private decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0m;

            decimal d;
            string s = NormalizeDigits(value.ToString());

            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                return d;

            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                return d;

            if (decimal.TryParse(s, out d))
                return d;

            return 0m;
        }

        private bool TryParseDecimal(string input, out decimal value)
        {
            input = NormalizeDigits(input);

            return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value)
                || decimal.TryParse(input, NumberStyles.Any, CultureInfo.CurrentCulture, out value)
                || decimal.TryParse(input, out value);
        }

        private string NormalizeDigits(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            return input
                .Replace('٠', '0')
                .Replace('١', '1')
                .Replace('٢', '2')
                .Replace('٣', '3')
                .Replace('٤', '4')
                .Replace('٥', '5')
                .Replace('٦', '6')
                .Replace('٧', '7')
                .Replace('٨', '8')
                .Replace('٩', '9')
                .Replace('٫', '.')
                .Replace('٬', ',')
                .Trim();
        }

        private string FormatNumber(object value)
        {
            decimal d = ToDecimal(value);
            return d.ToString("0.##");
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

using System.Data.SqlClient;

using System.Globalization;

using System.Drawing.Printing;

namespace water3.Forms
{
    public partial class ReadingEntryForm : Form
    {


            private readonly string connStr = Db.ConnectionString;

            private DataTable _table = new DataTable();

            private readonly PrintDocument _printDocument = new PrintDocument();
            private readonly PrintPreviewDialog _previewDialog = new PrintPreviewDialog();

            private int _printRowIndex = 0;
            private int _printPageNo = 1;
            private List<DataRowView> _printRows = new List<DataRowView>();
        private bool _isMovingToNextReadingCell = false;
        public ReadingEntryForm()
            {
            InitializeComponent();

            DoubleBuffered = true;
            EnableGridDoubleBuffer();

            ApplyTheme();
            WireEvents();
            BuildGridSchema();
            LoadAreas();

            dtpReadingDate.Value = DateTime.Today;

                _previewDialog.Document = _printDocument;
                _previewDialog.WindowState = FormWindowState.Maximized;

                _printDocument.DefaultPageSettings.Landscape = true;
                _printDocument.PrintPage += PrintDocument_PrintPage;
            }

        #region Theme

        private void ApplyTheme()
        {
            BackColor = Color.FromArgb(244, 247, 251);

            StyleCard(pnlHeaderCard, Color.White);
            StyleCard(pnlFiltersCard, Color.White);
            StyleCard(pnlGridCard, Color.White);

            StyleStatCard(pnlTotalCard, Color.FromArgb(237, 244, 255));
            StyleStatCard(pnlDoneCard, Color.FromArgb(235, 250, 242));
            StyleStatCard(pnlPendingCard, Color.FromArgb(255, 244, 232));

            lblTitle.ForeColor = Color.FromArgb(19, 52, 121);
            lblSubtitle.ForeColor = Color.FromArgb(100, 116, 139);
            lblGridTitle.ForeColor = Color.FromArgb(30, 41, 59);

            lblArea.ForeColor = Color.FromArgb(51, 65, 85);
            lblReadingDate.ForeColor = Color.FromArgb(51, 65, 85);
            lblSearch.ForeColor = Color.FromArgb(51, 65, 85);

            txtSearch.BorderStyle = BorderStyle.FixedSingle;
            txtSearch.BackColor = Color.White;
            txtSearch.Font = new Font("Tahoma", 10.5F);

            cmbArea.Font = new Font("Tahoma", 10.5F);
            cmbArea.FlatStyle = FlatStyle.Flat;

            dtpReadingDate.Font = new Font("Tahoma", 10.5F);
            dtpReadingDate.Format = DateTimePickerFormat.Custom;
            dtpReadingDate.CustomFormat = "yyyy/MM/dd";

            StyleButton(btnLoad, Color.FromArgb(37, 99, 235));
            StyleButton(btnSaveAll, Color.FromArgb(22, 163, 74));
            StyleButton(btnPrintSheet, Color.FromArgb(14, 165, 233));
            StyleButton(btnRefresh, Color.FromArgb(100, 116, 139));

            lblTotalCaption.ForeColor = Color.FromArgb(30, 41, 59);
            lblDoneCaption.ForeColor = Color.FromArgb(30, 41, 59);
            lblPendingCaption.ForeColor = Color.FromArgb(30, 41, 59);

            lblTotalValue.ForeColor = Color.FromArgb(37, 99, 235);
            lblDoneValue.ForeColor = Color.FromArgb(22, 163, 74);
            lblPendingValue.ForeColor = Color.FromArgb(234, 88, 12);

            dgvReadings.EnableHeadersVisualStyles = false;
            dgvReadings.BackgroundColor = Color.White;
            dgvReadings.BorderStyle = BorderStyle.None;
            dgvReadings.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvReadings.GridColor = Color.FromArgb(226, 232, 240);
            dgvReadings.RowHeadersVisible = false;
            dgvReadings.MultiSelect = false;

            dgvReadings.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dgvReadings.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            dgvReadings.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            dgvReadings.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            dgvReadings.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvReadings.ColumnHeadersHeight = 42;

            dgvReadings.DefaultCellStyle.BackColor = Color.White;
            dgvReadings.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            dgvReadings.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgvReadings.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgvReadings.DefaultCellStyle.Font = new Font("Tahoma", 10F);
            dgvReadings.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvReadings.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvReadings.RowTemplate.Height = 38;
        }

        private void StyleCard(Panel panel, Color backColor)
        {
            panel.BackColor = backColor;
            panel.BorderStyle = BorderStyle.None;

            panel.Paint -= DrawCardBorder;
            panel.Paint += DrawCardBorder;
        }

        private void StyleStatCard(Panel panel, Color backColor)
        {
            panel.BackColor = backColor;
            panel.BorderStyle = BorderStyle.None;

            panel.Paint -= DrawCardBorder;
            panel.Paint += DrawCardBorder;
        }

        private void DrawCardBorder(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null) return;

            Rectangle rect = panel.ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;

            using (Pen pen = new Pen(Color.FromArgb(220, 228, 238)))
            {
                e.Graphics.DrawRectangle(pen, rect);
            }
        }

        private void StyleButton(Button btn, Color backColor)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(backColor, 0.08F);
            btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(backColor, 0.08F);
            btn.BackColor = backColor;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Tahoma", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        #endregion

        #region Events

        private void WireEvents()
            {
                btnLoad.Click += (s, e) => LoadMetersForFollowUp();
                btnSaveAll.Click += (s, e) => SaveAllReadingsWithProcedure();
                btnPrintSheet.Click += (s, e) => PreviewSheet();
                btnRefresh.Click += (s, e) => LoadMetersForFollowUp();

                txtSearch.TextChanged += (s, e) => ApplyFilter();

                dgvReadings.CellEndEdit += DgvReadings_CellEndEdit;
                dgvReadings.DataError += (s, e) => { e.Cancel = false; };
                dgvReadings.CurrentCellDirtyStateChanged += (s, e) =>
                {
                    if (dgvReadings.IsCurrentCellDirty)
                        dgvReadings.CommitEdit(DataGridViewDataErrorContexts.Commit);
                };
            }

            #endregion

            #region Loaders

            private void LoadAreas()
            {
                try
                {
                    DataTable dt = new DataTable();

                    using (SqlConnection cn = new SqlConnection(connStr))
                    using (SqlDataAdapter da = new SqlDataAdapter(@"
SELECT DISTINCT
    ISNULL(NULLIF(LTRIM(RTRIM(Location)), ''), N'بدون تحديد') AS AreaName
FROM dbo.Meters
WHERE IsActive = 1
ORDER BY AreaName;", cn))
                    {
                        da.Fill(dt);
                    }

                    DataRow allRow = dt.NewRow();
                    allRow["AreaName"] = "الكل";
                    dt.Rows.InsertAt(allRow, 0);

                    cmbArea.DisplayMember = "AreaName";
                    cmbArea.ValueMember = "AreaName";
                    cmbArea.DataSource = dt;
                }
                catch
                {
                    cmbArea.Items.Clear();
                    cmbArea.Items.Add("الكل");
                    cmbArea.SelectedIndex = 0;
                }
            }

            private void BuildGridSchema()
            {
                _table = new DataTable();

                _table.Columns.Add("SubscriberID", typeof(int));
                _table.Columns.Add("MeterID", typeof(int));
                _table.Columns.Add("م", typeof(int));
                _table.Columns.Add("رقم الحساب", typeof(string));
                _table.Columns.Add("اسم المشترك", typeof(string));
                _table.Columns.Add("رقم العداد", typeof(string));
                _table.Columns.Add("الموقع", typeof(string));
                _table.Columns.Add("القراءة السابقة", typeof(decimal));
                _table.Columns.Add("القراءة الجديدة", typeof(string));
                _table.Columns.Add("الاستهلاك", typeof(decimal));
                _table.Columns.Add("ملاحظات", typeof(string));
                _table.Columns.Add("تم", typeof(bool));

                dgvReadings.DataSource = _table;

                FormatGrid();
                UpdateSummary();
            }

            private void LoadMetersForFollowUp()
            {
                try
                {
                    string area = cmbArea.Text.Trim();

                    using (SqlConnection cn = new SqlConnection(connStr))
                    using (SqlDataAdapter da = new SqlDataAdapter(@"
SELECT DISTINCT
    s.SubscriberID,
    m.MeterID,
    ISNULL(a.AccountCode, CAST(s.AccountID AS nvarchar(50))) AS AccountNo,
    s.Name AS SubscriberName,
    m.MeterNumber,
    ISNULL(NULLIF(LTRIM(RTRIM(m.Location)), ''), N'بدون تحديد') AS MeterLocation,
    ISNULL(lastR.CurrentReading, 0) AS PreviousReading
FROM dbo.Subscribers s
INNER JOIN dbo.SubscriberMeters sm ON sm.SubscriberID = s.SubscriberID
INNER JOIN dbo.Meters m ON m.MeterID = sm.MeterID
LEFT JOIN dbo.Accounts a ON a.AccountID = s.AccountID
OUTER APPLY
(
    SELECT TOP 1 r.CurrentReading
    FROM dbo.Readings r
    WHERE r.SubscriberID = s.SubscriberID
      AND r.MeterID = m.MeterID
    ORDER BY r.ReadingDate DESC, r.ReadingID DESC
) lastR
WHERE s.IsActive = 1
  AND m.IsActive = 1
  AND
  (
      @Area = N'الكل'
      OR ISNULL(NULLIF(LTRIM(RTRIM(m.Location)), ''), N'بدون تحديد') = @Area
  )
ORDER BY MeterLocation, s.Name, m.MeterNumber;", cn))
                    {
                        da.SelectCommand.Parameters.AddWithValue("@Area", area);

                        DataTable source = new DataTable();
                        da.Fill(source);

                        BuildGridSchema();

                        int i = 1;
                        foreach (DataRow src in source.Rows)
                        {
                            DataRow row = _table.NewRow();
                            row["SubscriberID"] = Convert.ToInt32(src["SubscriberID"]);
                            row["MeterID"] = Convert.ToInt32(src["MeterID"]);
                            row["م"] = i++;
                            row["رقم الحساب"] = src["AccountNo"] == DBNull.Value ? "" : src["AccountNo"].ToString();
                            row["اسم المشترك"] = src["SubscriberName"] == DBNull.Value ? "" : src["SubscriberName"].ToString();
                            row["رقم العداد"] = src["MeterNumber"] == DBNull.Value ? "" : src["MeterNumber"].ToString();
                            row["الموقع"] = src["MeterLocation"] == DBNull.Value ? "" : src["MeterLocation"].ToString();
                            row["القراءة السابقة"] = src["PreviousReading"] == DBNull.Value ? 0m : Convert.ToDecimal(src["PreviousReading"]);
                            row["القراءة الجديدة"] = "";
                            row["الاستهلاك"] = 0m;
                            row["ملاحظات"] = "";
                            row["تم"] = false;
                            _table.Rows.Add(row);
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                        ApplyFilter();
                    else
                        UpdateSummary();

                    lblGridTitle.Text = "بيانات المتابعة";
                }
                catch (Exception ex)
                {
                    MessageBox.Show("تعذر تحميل كشف العدادات.\n\n" + ex.Message, "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        private void FormatGrid()
        {
            if (dgvReadings.Columns.Count == 0) return;

            foreach (DataGridViewColumn col in dgvReadings.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;

            dgvReadings.Columns["SubscriberID"].Visible = false;
            dgvReadings.Columns["MeterID"].Visible = false;

            dgvReadings.Columns["م"].FillWeight = 42;
            dgvReadings.Columns["رقم الحساب"].FillWeight = 88;
            dgvReadings.Columns["اسم المشترك"].FillWeight = 175;
            dgvReadings.Columns["رقم العداد"].FillWeight = 92;
            dgvReadings.Columns["الموقع"].FillWeight = 135;
            dgvReadings.Columns["القراءة السابقة"].FillWeight = 95;
            dgvReadings.Columns["القراءة الجديدة"].FillWeight = 100;
            dgvReadings.Columns["الاستهلاك"].FillWeight = 88;
            dgvReadings.Columns["ملاحظات"].FillWeight = 150;
            dgvReadings.Columns["تم"].FillWeight = 50;

            dgvReadings.Columns["القراءة السابقة"].ReadOnly = true;
            dgvReadings.Columns["الاستهلاك"].ReadOnly = true;
            dgvReadings.Columns["تم"].ReadOnly = true;

            foreach (DataGridViewColumn col in dgvReadings.Columns)
            {
                if (col.Name != "القراءة الجديدة" &&
                    col.Name != "ملاحظات" &&
                    col.Name != "SubscriberID" &&
                    col.Name != "MeterID")
                {
                    col.ReadOnly = true;
                }
            }

            dgvReadings.Columns["اسم المشترك"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;

            dgvReadings.Columns["الموقع"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;

            dgvReadings.Columns["ملاحظات"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;

            dgvReadings.Columns["القراءة السابقة"].DefaultCellStyle.Format = "0.##";
            dgvReadings.Columns["الاستهلاك"].DefaultCellStyle.Format = "0.##";

            dgvReadings.Columns["القراءة الجديدة"].DefaultCellStyle.BackColor =
                Color.FromArgb(255, 251, 235);

            dgvReadings.Columns["القراءة الجديدة"].DefaultCellStyle.Font =
                new Font("Tahoma", 10F, FontStyle.Bold);

            dgvReadings.Columns["ملاحظات"].DefaultCellStyle.BackColor =
                Color.FromArgb(248, 250, 252);

            dgvReadings.Columns["تم"].DefaultCellStyle.BackColor =
                Color.FromArgb(240, 253, 244);
        }
        private void EnableGridDoubleBuffer()
        {
            try
            {
                var prop = typeof(DataGridView).GetProperty(
                    "DoubleBuffered",
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.NonPublic);

                if (prop != null)
                    prop.SetValue(dgvReadings, true, null);
            }
            catch
            {
                // تجاهل أي خطأ غير مؤثر
            }
        }
        #endregion

        #region Filter + Summary

        private void ApplyFilter()
            {
                if (_table == null || _table.DefaultView == null)
                    return;

                string keyword = txtSearch.Text.Trim().Replace("'", "''");

                if (string.IsNullOrWhiteSpace(keyword))
                {
                    _table.DefaultView.RowFilter = "";
                }
                else
                {
                    _table.DefaultView.RowFilter =
                        "[اسم المشترك] LIKE '%" + keyword + "%' " +
                        "OR [رقم الحساب] LIKE '%" + keyword + "%' " +
                        "OR [رقم العداد] LIKE '%" + keyword + "%' " +
                        "OR [الموقع] LIKE '%" + keyword + "%'";
                }

                UpdateSummaryFromView();
            }

            private void UpdateSummary()
            {
                int total = _table.Rows.Count;
                int done = _table.Rows.Cast<DataRow>()
                    .Count(r => r["تم"] != DBNull.Value && Convert.ToBoolean(r["تم"]));

                lblTotalValue.Text = total.ToString();
                lblDoneValue.Text = done.ToString();
                lblPendingValue.Text = (total - done).ToString();
            }

            private void UpdateSummaryFromView()
            {
                int total = _table.DefaultView.Count;
                int done = 0;

                foreach (DataRowView drv in _table.DefaultView)
                {
                    if (drv["تم"] != DBNull.Value && Convert.ToBoolean(drv["تم"]))
                        done++;
                }

                lblTotalValue.Text = total.ToString();
                lblDoneValue.Text = done.ToString();
                lblPendingValue.Text = (total - done).ToString();
            }

        #endregion

        #region Grid Editing

        private void DgvReadings_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (e.RowIndex >= dgvReadings.Rows.Count)
                return;

            DataGridViewRow row = dgvReadings.Rows[e.RowIndex];

            if (row.IsNewRow)
                return;

            string colName = dgvReadings.Columns[e.ColumnIndex].Name;

            if (colName == "القراءة الجديدة")
            {
                HandleReadingEdit(row, e.RowIndex);
            }
            else if (colName == "ملاحظات")
            {
                string current = Convert.ToString(row.Cells["القراءة الجديدة"].Value);
                decimal temp;

                if (TryParseDecimal(current, out temp))
                    row.Cells["تم"].Value = true;
            }

            UpdateSummaryFromView();
        }

        private void HandleReadingEdit(DataGridViewRow row, int rowIndex)
            {
                decimal previous = ToDecimal(row.Cells["القراءة السابقة"].Value);
                decimal current;

                string currentText = row.Cells["القراءة الجديدة"].Value == null
                    ? ""
                    : row.Cells["القراءة الجديدة"].Value.ToString().Trim();

                if (string.IsNullOrWhiteSpace(currentText))
                {
                    row.Cells["الاستهلاك"].Value = 0m;
                    row.Cells["تم"].Value = false;
                    ResetRowStyle(row);
                    return;
                }

                if (!TryParseDecimal(currentText, out current))
                {
                    row.Cells["الاستهلاك"].Value = 0m;
                    row.Cells["تم"].Value = false;
                    MarkRowInvalid(row);

                    MessageBox.Show("القراءة الجديدة غير صحيحة.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (current < previous)
                {
                    row.Cells["القراءة الجديدة"].Value = "";
                    row.Cells["الاستهلاك"].Value = 0m;
                    row.Cells["تم"].Value = false;
                    MarkRowInvalid(row);

                    MessageBox.Show("القراءة الجديدة لا يمكن أن تكون أقل من القراءة السابقة.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                row.Cells["القراءة الجديدة"].Value = current.ToString("0.##");
                row.Cells["الاستهلاك"].Value = current - previous;
                row.Cells["تم"].Value = true;
                MarkRowValid(row);
            MoveToNextReadingCellDeferred(rowIndex);
            //if (rowIndex < dgvReadings.Rows.Count - 1)
            //{
            //    dgvReadings.CurrentCell = dgvReadings.Rows[rowIndex + 1].Cells["القراءة الجديدة"];
            //    dgvReadings.BeginEdit(true);
            //}
        }
        private void MoveToNextReadingCellDeferred(int rowIndex)
        {
            if (_isMovingToNextReadingCell)
                return;

            if (rowIndex < 0 || rowIndex >= dgvReadings.Rows.Count - 1)
                return;

            _isMovingToNextReadingCell = true;

            try
            {
                dgvReadings.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (IsDisposed || dgvReadings.IsDisposed || !dgvReadings.IsHandleCreated)
                            return;

                        if (rowIndex < 0 || rowIndex >= dgvReadings.Rows.Count - 1)
                            return;

                        DataGridViewRow nextRow = dgvReadings.Rows[rowIndex + 1];

                        if (nextRow == null || nextRow.IsNewRow || !nextRow.Visible)
                            return;

                        DataGridViewCell nextCell = nextRow.Cells["القراءة الجديدة"];

                        if (nextCell == null || nextCell.ReadOnly)
                            return;

                        dgvReadings.CurrentCell = nextCell;
                        dgvReadings.BeginEdit(true);
                    }
                    catch
                    {
                        // منع توقف البرنامج لو حصل تغيير في الفلتر أو الصفوف أثناء النقل
                    }
                    finally
                    {
                        _isMovingToNextReadingCell = false;
                    }
                }));
            }
            catch
            {
                _isMovingToNextReadingCell = false;
            }
        }
        private void ResetRowStyle(DataGridViewRow row)
            {
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.FromArgb(33, 37, 41);
            }

            private void MarkRowValid(DataGridViewRow row)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(236, 253, 245);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(22, 101, 52);
            }

            private void MarkRowInvalid(DataGridViewRow row)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(153, 27, 27);
            }

            #endregion

            #region Save With Procedure

            private void SaveAllReadingsWithProcedure()
            {
                dgvReadings.EndEdit();

                if (_table.Rows.Count == 0)
                {
                    MessageBox.Show("لا توجد عدادات محملة للحفظ.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                List<DataRow> rowsToSave = new List<DataRow>();

                foreach (DataRow row in _table.Rows)
                {
                    decimal currentReading;
                    if (TryParseDecimal(Convert.ToString(row["القراءة الجديدة"]), out currentReading))
                        rowsToSave.Add(row);
                }

                if (rowsToSave.Count == 0)
                {
                    MessageBox.Show("لم يتم إدخال أي قراءة جديدة بعد.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int successCount = 0;
                List<string> errors = new List<string>();

                using (SqlConnection cn = new SqlConnection(connStr))
                {
                    cn.Open();

                    foreach (DataRow row in rowsToSave)
                    {
                        int subscriberId = Convert.ToInt32(row["SubscriberID"]);
                        int meterId = Convert.ToInt32(row["MeterID"]);
                        string subscriberName = Convert.ToString(row["اسم المشترك"]);
                        string meterNumber = Convert.ToString(row["رقم العداد"]);
                        decimal currentReading = ToDecimal(row["القراءة الجديدة"]);
                        string notes = row["ملاحظات"] == DBNull.Value ? "" : Convert.ToString(row["ملاحظات"]);

                        try
                        {
                            using (SqlCommand cmd = new SqlCommand("dbo.AddReadingAndGenerateInvoice", cn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.CommandTimeout = 120;

                                cmd.Parameters.AddWithValue("@SubscriberID", subscriberId);
                                cmd.Parameters.AddWithValue("@MeterID", meterId);
                                cmd.Parameters.AddWithValue("@ReadingDate", dtpReadingDate.Value.Date);
                                cmd.Parameters.AddWithValue("@CurrentReading", currentReading);

                                // نجعل الإجراء يحددها تلقائياً من الخطة/الثوابت
                                cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = DBNull.Value;
                                cmd.Parameters.Add("@ServiceFees", SqlDbType.Decimal).Value = DBNull.Value;

                                if (string.IsNullOrWhiteSpace(notes))
                                    cmd.Parameters.AddWithValue("@Notes", DBNull.Value);
                                else
                                    cmd.Parameters.AddWithValue("@Notes", notes);

                                cmd.ExecuteNonQuery();
                            }

                            successCount++;
                            row["تم"] = true;
                            MarkRowValidByDataRow(row);
                        }
                        catch (SqlException ex)
                        {
                            string errorText = ExtractSqlErrorMessage(ex);
                            errors.Add($"- {subscriberName} / {meterNumber}: {errorText}");
                            row["تم"] = false;
                        }
                        catch (Exception ex)
                        {
                            errors.Add($"- {subscriberName} / {meterNumber}: {ex.Message}");
                            row["تم"] = false;
                        }
                    }
                }

                UpdateSummary();

                if (successCount > 0 && errors.Count == 0)
                {
                    MessageBox.Show($"تم حفظ {successCount} قراءة وإنشاء الفواتير بنجاح ✅", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadMetersForFollowUp();
                    return;
                }

                if (successCount > 0 && errors.Count > 0)
                {
                    string msg =
                        $"تم حفظ {successCount} قراءة بنجاح، وتعذر حفظ {errors.Count} قراءة.\n\n" +
                        string.Join("\n", errors.Take(10));

                    if (errors.Count > 10)
                        msg += $"\n\n... وهناك {errors.Count - 10} أخطاء إضافية";

                    MessageBox.Show(msg, "حفظ جزئي", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    LoadMetersForFollowUp();
                    return;
                }

                MessageBox.Show(
                    "تعذر حفظ القراءات.\n\n" + string.Join("\n", errors.Take(10)),
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            private string ExtractSqlErrorMessage(SqlException ex)
            {
                if (ex == null) return "خطأ غير معروف";

                foreach (SqlError err in ex.Errors)
                {
                    if (!string.IsNullOrWhiteSpace(err.Message))
                        return err.Message;
                }

                return ex.Message;
            }

            private void MarkRowValidByDataRow(DataRow row)
            {
                foreach (DataGridViewRow gridRow in dgvReadings.Rows)
                {
                    if (gridRow.DataBoundItem is DataRowView drv && drv.Row == row)
                    {
                        MarkRowValid(gridRow);
                        break;
                    }
                }
            }

            #endregion

            #region Print

            private void PreviewSheet()
            {
                if (_table.Rows.Count == 0)
                {
                    MessageBox.Show("قم بتحميل العدادات أولاً.", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                ResetPrintState();
                _previewDialog.ShowDialog(this);
            }

            private void ResetPrintState()
            {
                _printRowIndex = 0;
                _printPageNo = 1;
                _printRows = _table.DefaultView.Cast<DataRowView>().ToList();
            }

            private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
            {
                Graphics g = e.Graphics;
                Rectangle m = e.MarginBounds;

                using (Font titleFont = new Font("Tahoma", 15, FontStyle.Bold))
                using (Font normalFont = new Font("Tahoma", 9, FontStyle.Regular))
                using (Font headerFont = new Font("Tahoma", 9, FontStyle.Bold))
                using (Pen pen = new Pen(Color.Black))
                {
                    int y = m.Top;

                    g.DrawString("كشف متابعة تسجيل القراءات الجديدة", titleFont, Brushes.Black, m.Left, y);
                    y += 28;

                    g.DrawString("المنطقة: " + cmbArea.Text, normalFont, Brushes.Black, m.Left, y);
                    g.DrawString("تاريخ القراءة: " + dtpReadingDate.Value.ToString("yyyy/MM/dd"), normalFont, Brushes.Black, m.Left + 260, y);
                    g.DrawString("عدد السجلات: " + _printRows.Count, normalFont, Brushes.Black, m.Left + 520, y);
                    y += 24;

                    string[] headers =
                    {
                    "م",
                    "رقم الحساب",
                    "اسم المشترك",
                    "رقم العداد",
                    "الموقع",
                    "القراءة السابقة",
                    "القراءة الجديدة",
                    "الاستهلاك",
                    "ملاحظات"
                };

                    int[] widths = GetPrintColumnWidths(m.Width);

                    int x = m.Left;
                    int headerHeight = 28;
                    int rowHeight = 26;

                    for (int i = 0; i < headers.Length; i++)
                    {
                        Rectangle rect = new Rectangle(x, y, widths[i], headerHeight);
                        g.FillRectangle(Brushes.LightGray, rect);
                        g.DrawRectangle(pen, rect);
                        DrawCentered(g, headers[i], headerFont, rect);
                        x += widths[i];
                    }

                    y += headerHeight;

                    while (_printRowIndex < _printRows.Count)
                    {
                        if (y + rowHeight > m.Bottom - 35)
                        {
                            e.HasMorePages = true;
                            _printPageNo++;
                            return;
                        }

                        DataRowView drv = _printRows[_printRowIndex];

                        string[] vals =
                        {
                        Convert.ToString(drv["م"]),
                        Convert.ToString(drv["رقم الحساب"]),
                        Convert.ToString(drv["اسم المشترك"]),
                        Convert.ToString(drv["رقم العداد"]),
                        Convert.ToString(drv["الموقع"]),
                        FormatNumber(drv["القراءة السابقة"]),
                        Convert.ToString(drv["القراءة الجديدة"]),
                        FormatNumber(drv["الاستهلاك"]),
                        Convert.ToString(drv["ملاحظات"])
                    };

                        x = m.Left;

                        for (int i = 0; i < vals.Length; i++)
                        {
                            Rectangle rect = new Rectangle(x, y, widths[i], rowHeight);
                            g.DrawRectangle(pen, rect);
                            DrawCell(g, vals[i], normalFont, rect);
                            x += widths[i];
                        }

                        y += rowHeight;
                        _printRowIndex++;
                    }

                    y += 10;
                    g.DrawString("صفحة: " + _printPageNo, normalFont, Brushes.Black, m.Right - 80, y);

                    e.HasMorePages = false;
                    _printRowIndex = 0;
                }
            }

            private int[] GetPrintColumnWidths(int totalWidth)
            {
                double[] ratios = { 0.05, 0.10, 0.18, 0.10, 0.15, 0.10, 0.10, 0.08, 0.14 };
                int[] widths = new int[ratios.Length];

                int used = 0;
                for (int i = 0; i < ratios.Length; i++)
                {
                    widths[i] = (int)(totalWidth * ratios[i]);
                    used += widths[i];
                }

                if (used < totalWidth)
                    widths[widths.Length - 1] += (totalWidth - used);

                return widths;
            }

            private void DrawCentered(Graphics g, string text, Font font, Rectangle rect)
            {
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Center;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                    g.DrawString(text ?? "", font, Brushes.Black, rect, sf);
                }
            }

            private void DrawCell(Graphics g, string text, Font font, Rectangle rect)
            {
                using (StringFormat sf = new StringFormat())
                {
                    sf.Alignment = StringAlignment.Near;
                    sf.LineAlignment = StringAlignment.Center;
                    sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;

                    Rectangle padded = new Rectangle(rect.X + 3, rect.Y, rect.Width - 6, rect.Height);
                    g.DrawString(text ?? "", font, Brushes.Black, padded, sf);
                }
            }

            #endregion

            #region Helpers

            private decimal ToDecimal(object value)
            {
                if (value == null || value == DBNull.Value)
                    return 0m;

                decimal d;
                string s = NormalizeDigits(value.ToString());

                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out d))
                    return d;

                if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out d))
                    return d;

                if (decimal.TryParse(s, out d))
                    return d;

                return 0m;
            }

            private bool TryParseDecimal(string input, out decimal value)
            {
                input = NormalizeDigits(input);

                return decimal.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out value)
                    || decimal.TryParse(input, NumberStyles.Any, CultureInfo.CurrentCulture, out value)
                    || decimal.TryParse(input, out value);
            }

            private string NormalizeDigits(string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                    return string.Empty;

                return input
                    .Replace('٠', '0').Replace('١', '1').Replace('٢', '2').Replace('٣', '3').Replace('٤', '4')
                    .Replace('٥', '5').Replace('٦', '6').Replace('٧', '7').Replace('٨', '8').Replace('٩', '9')
                    .Replace('٫', '.')
                    .Replace('٬', ',')
                    .Trim();
            }

            private string FormatNumber(object value)
            {
                decimal d = ToDecimal(value);
                return d.ToString("0.##");
            }

            #endregion
        }
    }*/