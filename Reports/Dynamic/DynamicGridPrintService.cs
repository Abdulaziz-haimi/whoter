using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace water3.Services
{
    public class DynamicGridPrintService
    {
        private class CompanyInfo
        {
            public string CompanyName { get; set; }
            public string SystemName { get; set; }
            public string Phone { get; set; }
            public string Mobile { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string LogoPath { get; set; }
        }

        private class PrintColumn
        {
            public DataGridViewColumn GridColumn { get; set; }
            public string HeaderText { get; set; }
            public int PrintWidth { get; set; }
            public bool IsSummable { get; set; }
            public decimal Total { get; set; }
        }

        private DataGridView _grid;
        private List<PrintColumn> _columns = new List<PrintColumn>();
        private List<DataGridViewRow> _rows = new List<DataGridViewRow>();

        private string _reportTitle;
        private string _reportType;
        private string _fromDateText;
        private string _toDateText;
        private string _subscriberText;
        private string _categoryTypeText;
        private string _searchText;

        private int _rowIndex;
        private int _pageNumber;
        private bool _grandTotalPrinted;

        private readonly Color Primary = Color.FromArgb(0, 102, 204);
        private readonly Color HeaderBack = Color.FromArgb(0, 102, 204);
        private readonly Color Border = Color.FromArgb(150, 150, 150);
        private readonly Color AltRow = Color.FromArgb(245, 247, 250);
        private readonly Color TotalBack = Color.FromArgb(232, 244, 255);
        private readonly Color InfoBack = Color.FromArgb(248, 250, 252);

        public void PrintPreview(DataGridView grid, string reportTitle)
        {
            PrintPreview(
                grid,
                reportTitle,
                reportTitle,
                "غير محدد",
                "غير محدد",
                "غير محدد",
                "غير محدد",
                ""
            );
        }

        public void PrintPreview(
            DataGridView grid,
            string reportTitle,
            string reportType,
            string fromDateText,
            string toDateText,
            string subscriberText,
            string categoryTypeText,
            string searchText)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            _grid = grid;

            PrepareRowsAndColumns();

            if (_rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للطباعة.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_columns.Count == 0)
            {
                MessageBox.Show("لا توجد أعمدة ظاهرة للطباعة.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _reportTitle = string.IsNullOrWhiteSpace(reportTitle) ? "تقرير" : reportTitle;
            _reportType = string.IsNullOrWhiteSpace(reportType) ? _reportTitle : reportType;
            _fromDateText = string.IsNullOrWhiteSpace(fromDateText) ? "غير محدد" : fromDateText;
            _toDateText = string.IsNullOrWhiteSpace(toDateText) ? "غير محدد" : toDateText;
            _subscriberText = string.IsNullOrWhiteSpace(subscriberText) ? "غير محدد" : subscriberText;
            _categoryTypeText = string.IsNullOrWhiteSpace(categoryTypeText) ? "غير محدد" : categoryTypeText;
            _searchText = string.IsNullOrWhiteSpace(searchText) ? "لا يوجد" : searchText;

            CalculateColumnTotals();

            PrintDocument doc = new PrintDocument();
            doc.DocumentName = _reportTitle;
            doc.DefaultPageSettings.Landscape = true;
            doc.DefaultPageSettings.Margins = new Margins(35, 35, 35, 35);

            doc.BeginPrint += delegate
            {
                _rowIndex = 0;
                _pageNumber = 1;
                _grandTotalPrinted = false;
            };

            doc.PrintPage += PrintPage;

            using (PrintPreviewDialog preview = new PrintPreviewDialog())
            {
                preview.Document = doc;
                preview.WindowState = FormWindowState.Maximized;
                preview.Text = "معاينة الطباعة - " + _reportTitle;
                preview.ShowDialog();
            }
        }

        private void PrepareRowsAndColumns()
        {
            _columns = _grid.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .Select(c => new PrintColumn
                {
                    GridColumn = c,
                    HeaderText = string.IsNullOrWhiteSpace(c.HeaderText) ? c.Name : c.HeaderText,
                    IsSummable = IsSummableColumn(c),
                    Total = 0m
                })
                .ToList();

            _rows = _grid.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .ToList();
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            Rectangle bounds = e.MarginBounds;
            CalculateColumnWidths(bounds.Width);

            int y = DrawPageHeader(e);

            int tableX = bounds.Left;
            int tableWidth = bounds.Width;
            int rowHeight = 28;
            int totalRowHeight = 32;
            int footerHeight = 30;
            int bottomLimit = bounds.Bottom - footerHeight;

            DrawTableHeader(e.Graphics, tableX, y, tableWidth, rowHeight);
            y += rowHeight;

            while (_rowIndex < _rows.Count)
            {
                if (y + rowHeight > bottomLimit)
                {
                    DrawFooter(e.Graphics, bounds);
                    _pageNumber++;
                    e.HasMorePages = true;
                    return;
                }

                DrawTableRow(e.Graphics, _rows[_rowIndex], tableX, y, rowHeight, _rowIndex);
                y += rowHeight;
                _rowIndex++;
            }

            // صف الإجمالي يظهر مرة واحدة فقط في آخر ورقة بعد انتهاء كل البيانات
            if (!_grandTotalPrinted)
            {
                if (y + totalRowHeight > bottomLimit)
                {
                    DrawFooter(e.Graphics, bounds);
                    _pageNumber++;
                    e.HasMorePages = true;
                    return;
                }

                DrawGrandTotalRow(e.Graphics, tableX, y, totalRowHeight);
                y += totalRowHeight;
                _grandTotalPrinted = true;
            }

            DrawFooter(e.Graphics, bounds);
            _pageNumber++;
            e.HasMorePages = false;
        }

        private int DrawPageHeader(PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = e.MarginBounds;
            CompanyInfo info = LoadCompanyInfo();

            int x = bounds.Left;
            int y = bounds.Top;
            int width = bounds.Width;

            Font companyFont = new Font("Tahoma", 15F, FontStyle.Bold);
            Font systemFont = new Font("Tahoma", 9.5F, FontStyle.Bold);
            Font infoFont = new Font("Tahoma", 8.2F);
            Font titleFont = new Font("Tahoma", 13F, FontStyle.Bold);
            Font labelFont = new Font("Tahoma", 7.8F, FontStyle.Bold);
            Font valueFont = new Font("Tahoma", 8.2F, FontStyle.Bold);

            StringFormat rtlRight = CreateRtlFormat(StringAlignment.Far);
            StringFormat rtlCenter = CreateRtlFormat(StringAlignment.Center);

            int logoSize = 64;
            int logoX = bounds.Right - logoSize;
            int logoY = y;

            Image logo = LoadLogo(info.LogoPath);

            if (logo != null)
            {
                g.DrawImage(logo, new Rectangle(logoX, logoY, logoSize, logoSize));
                logo.Dispose();
            }

            RectangleF companyRect = new RectangleF(x, y, width - logoSize - 12, 24);
            RectangleF systemRect = new RectangleF(x, y + 24, width - logoSize - 12, 20);
            RectangleF contactRect = new RectangleF(x, y + 44, width - logoSize - 12, 18);
            RectangleF addressRect = new RectangleF(x, y + 62, width - logoSize - 12, 18);

            g.DrawString(info.CompanyName, companyFont, Brushes.Black, companyRect, rtlRight);
            g.DrawString(info.SystemName, systemFont, Brushes.Black, systemRect, rtlRight);

            string contacts = JoinParts(
                AddPrefix("الهاتف", info.Phone),
                AddPrefix("الجوال", info.Mobile),
                AddPrefix("البريد", info.Email)
            );

            g.DrawString(contacts, infoFont, Brushes.Black, contactRect, rtlRight);
            g.DrawString(AddPrefix("العنوان", info.Address), infoFont, Brushes.Black, addressRect, rtlRight);

            int lineY = y + 84;
            g.DrawLine(Pens.Gray, x, lineY, bounds.Right, lineY);

            RectangleF titleRect = new RectangleF(x, lineY + 5, width, 28);
            g.DrawString(_reportTitle, titleFont, Brushes.Black, titleRect, rtlCenter);

            int infoGridY = lineY + 38;
            int infoGridHeight = 70;

            DrawReportInfoGrid(
                g,
                new Rectangle(x, infoGridY, width, infoGridHeight),
                labelFont,
                valueFont
            );

            return infoGridY + infoGridHeight + 10;
        }

        private void DrawReportInfoGrid(Graphics g, Rectangle rect, Font labelFont, Font valueFont)
        {
            int columns = 4;
            int rows = 2;
            int cellW = rect.Width / columns;
            int cellH = rect.Height / rows;

            string[,] labels =
            {
                { "نوع التقرير", "من تاريخ", "إلى تاريخ", "تاريخ الطباعة" },
                { "المشترك", "نوع المصروف", "البحث", "عدد السجلات" }
            };

            string[,] values =
            {
                { _reportType, _fromDateText, _toDateText, DateTime.Now.ToString("yyyy-MM-dd ") },
                { _subscriberText, _categoryTypeText, _searchText, _rows.Count.ToString("N0") }
            };

            StringFormat center = CreateRtlFormat(StringAlignment.Center);

            using (SolidBrush backBrush = new SolidBrush(InfoBack))
            using (Pen pen = new Pen(Border))
            using (SolidBrush labelBrush = new SolidBrush(Color.FromArgb(90, 90, 90)))
            {
                g.FillRectangle(backBrush, rect);
                g.DrawRectangle(pen, rect);

                for (int r = 0; r < rows; r++)
                {
                    int right = rect.Right;

                    for (int c = 0; c < columns; c++)
                    {
                        Rectangle cell = new Rectangle(
                            right - cellW,
                            rect.Top + (r * cellH),
                            cellW,
                            cellH
                        );

                        g.DrawRectangle(pen, cell);

                        Rectangle labelRect = new Rectangle(cell.X + 3, cell.Y + 2, cell.Width - 6, 14);
                        Rectangle valueRect = new Rectangle(cell.X + 3, cell.Y + 17, cell.Width - 6, cell.Height - 19);

                        g.DrawString(labels[r, c], labelFont, labelBrush, labelRect, center);
                        g.DrawString(ShortText(values[r, c], 35), valueFont, Brushes.Black, valueRect, center);

                        right -= cellW;
                    }
                }
            }
        }

        private void DrawTableHeader(Graphics g, int x, int y, int tableWidth, int height)
        {
            Font headerFont = new Font("Tahoma", 8.3F, FontStyle.Bold);
            StringFormat center = CreateRtlFormat(StringAlignment.Center);

            using (SolidBrush backBrush = new SolidBrush(HeaderBack))
            using (Pen pen = new Pen(Border))
            {
                int right = x + tableWidth;

                foreach (PrintColumn col in _columns)
                {
                    Rectangle rect = new Rectangle(right - col.PrintWidth, y, col.PrintWidth, height);

                    g.FillRectangle(backBrush, rect);
                    g.DrawRectangle(pen, rect);
                    g.DrawString(ShortText(col.HeaderText, 35), headerFont, Brushes.White, rect, center);

                    right -= col.PrintWidth;
                }
            }
        }

        private void DrawTableRow(Graphics g, DataGridViewRow row, int x, int y, int height, int rowIndex)
        {
            Font cellFont = new Font("Tahoma", 8F);
            StringFormat rightFormat = CreateRtlFormat(StringAlignment.Far);
            StringFormat centerFormat = CreateRtlFormat(StringAlignment.Center);

            using (SolidBrush backBrush = new SolidBrush(rowIndex % 2 == 0 ? Color.White : AltRow))
            using (Pen pen = new Pen(Border))
            {
                int right = x + _columns.Sum(c => c.PrintWidth);

                foreach (PrintColumn col in _columns)
                {
                    Rectangle rect = new Rectangle(right - col.PrintWidth, y, col.PrintWidth, height);

                    g.FillRectangle(backBrush, rect);
                    g.DrawRectangle(pen, rect);

                    string text = GetCellText(row, col.GridColumn);
                    StringFormat format = IsNumericText(text) ? centerFormat : rightFormat;

                    RectangleF textRect = new RectangleF(rect.X + 3, rect.Y + 2, rect.Width - 6, rect.Height - 4);
                    g.DrawString(ShortText(text, 45), cellFont, Brushes.Black, textRect, format);

                    right -= col.PrintWidth;
                }
            }
        }

        private void DrawGrandTotalRow(Graphics g, int x, int y, int height)
        {
            Font totalFont = new Font("Tahoma", 8.5F, FontStyle.Bold);
            StringFormat center = CreateRtlFormat(StringAlignment.Center);
            StringFormat rightFormat = CreateRtlFormat(StringAlignment.Far);

            bool labelPrinted = false;
            bool hasSummableColumn = _columns.Any(c => c.IsSummable);

            using (SolidBrush backBrush = new SolidBrush(TotalBack))
            using (Pen pen = new Pen(Border))
            {
                int right = x + _columns.Sum(c => c.PrintWidth);

                foreach (PrintColumn col in _columns)
                {
                    Rectangle rect = new Rectangle(right - col.PrintWidth, y, col.PrintWidth, height);

                    g.FillRectangle(backBrush, rect);
                    g.DrawRectangle(pen, rect);

                    string text = "";

                    if (col.IsSummable)
                    {
                        text = col.Total.ToString("N2");
                    }
                    else if (!labelPrinted)
                    {
                        text = hasSummableColumn
                            ? "الإجمالي"
                            : "الإجمالي - عدد السجلات: " + _rows.Count.ToString("N0");

                        labelPrinted = true;
                    }

                    RectangleF textRect = new RectangleF(rect.X + 3, rect.Y + 2, rect.Width - 6, rect.Height - 4);
                    g.DrawString(ShortText(text, 45), totalFont, Brushes.Black, textRect,
                        col.IsSummable ? center : rightFormat);

                    right -= col.PrintWidth;
                }
            }
        }

        private void DrawFooter(Graphics g, Rectangle bounds)
        {
            Font footerFont = new Font("Tahoma", 8F, FontStyle.Bold);
            StringFormat center = CreateRtlFormat(StringAlignment.Center);
            StringFormat left = CreateRtlFormat(StringAlignment.Near);

            int y = bounds.Bottom + 8;

            RectangleF pageRect = new RectangleF(bounds.Left, y, bounds.Width, 20);
            g.DrawString("صفحة " + _pageNumber, footerFont, Brushes.Gray, pageRect, center);

            RectangleF dateRect = new RectangleF(bounds.Left, y, bounds.Width, 20);
            g.DrawString("تاريخ الطباعة: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                footerFont, Brushes.Gray, dateRect, left);
        }

        private void CalculateColumnWidths(int availableWidth)
        {
            if (_columns == null || _columns.Count == 0)
                return;

            int count = _columns.Count;
            int originalTotal = _columns.Sum(c => Math.Max(50, c.GridColumn.Width));

            if (originalTotal <= 0)
                originalTotal = count * 80;

            int minWidth = Math.Max(25, Math.Min(65, availableWidth / count));

            if (minWidth * count > availableWidth)
                minWidth = Math.Max(20, availableWidth / count);

            int used = 0;

            foreach (PrintColumn col in _columns)
            {
                int baseWidth = Math.Max(50, col.GridColumn.Width);
                int w = (int)Math.Round((baseWidth / (double)originalTotal) * availableWidth);

                if (w < minWidth)
                    w = minWidth;

                col.PrintWidth = w;
                used += w;
            }

            int diff = availableWidth - used;

            if (_columns.Count > 0)
                _columns[_columns.Count - 1].PrintWidth += diff;
        }

        private void CalculateColumnTotals()
        {
            foreach (PrintColumn col in _columns)
            {
                col.Total = 0m;

                if (!col.IsSummable)
                    continue;

                foreach (DataGridViewRow row in _rows)
                {
                    decimal value;

                    if (TryGetDecimal(row.Cells[col.GridColumn.Index].Value, out value))
                        col.Total += value;
                }
            }
        }

        private CompanyInfo LoadCompanyInfo()
        {
            CompanyInfo info = new CompanyInfo();

            try
            {
                AppSchemaService.EnsureProductionTables();

                info.CompanyName = AppSettingsService.Get("Company.Name", "مؤسسة المياه");
                info.SystemName = AppSettingsService.Get("System.Name", "نظام إدارة المياه");
                info.Phone = AppSettingsService.Get("Company.Phone", "");
                info.Mobile = AppSettingsService.Get("Company.Mobile", info.Phone);
                info.Email = AppSettingsService.Get("Company.Email", "");
                info.Address = AppSettingsService.Get("Company.Address", "");
                info.LogoPath = AppSettingsService.Get("Company.LogoPath", "");
            }
            catch
            {
                info.CompanyName = "مؤسسة المياه";
                info.SystemName = "نظام إدارة المياه";
                info.Phone = "";
                info.Mobile = "";
                info.Email = "";
                info.Address = "";
                info.LogoPath = "";
            }

            return info;
        }

        private Image LoadLogo(string logoPath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(logoPath) && File.Exists(logoPath))
                {
                    using (Image img = Image.FromFile(logoPath))
                    {
                        return new Bitmap(img);
                    }
                }

                string fallback = Path.Combine(Application.StartupPath, "Assets", "company_logo.png");

                if (File.Exists(fallback))
                {
                    using (Image img = Image.FromFile(fallback))
                    {
                        return new Bitmap(img);
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        private string GetCellText(DataGridViewRow row, DataGridViewColumn column)
        {
            try
            {
                object value = row.Cells[column.Index].Value;

                if (value == null || value == DBNull.Value)
                    return string.Empty;

                if (value is DateTime)
                    return ((DateTime)value).ToString("yyyy-MM-dd");

                if (value is bool)
                    return ((bool)value) ? "نعم" : "لا";

                object formatted = row.Cells[column.Index].FormattedValue;

                if (formatted != null)
                    return Convert.ToString(formatted);

                return Convert.ToString(value);
            }
            catch
            {
                return string.Empty;
            }
        }

        private bool IsSummableColumn(DataGridViewColumn col)
        {
            string name = (col.DataPropertyName + " " + col.Name + " " + col.HeaderText).ToLowerInvariant();

            if (name.Contains("id") ||
                name.Contains("code") ||
                name.Contains("number") ||
                name.Contains("no") ||
                name.Contains("رقم") ||
                name.Contains("كود") ||
                name.Contains("معرف"))
                return false;

            if (name.Contains("amount") ||
                name.Contains("total") ||
                name.Contains("paid") ||
                name.Contains("due") ||
                name.Contains("balance") ||
                name.Contains("price") ||
                name.Contains("cost") ||
                name.Contains("debit") ||
                name.Contains("credit") ||
                name.Contains("المبلغ") ||
                name.Contains("الإجمالي") ||
                name.Contains("اجمالي") ||
                name.Contains("المدفوع") ||
                name.Contains("المتبقي") ||
                name.Contains("الرصيد") ||
                name.Contains("السعر") ||
                name.Contains("التكلفة") ||
                name.Contains("دائن") ||
                name.Contains("مدين"))
                return true;

            return false;
        }

        private bool TryGetDecimal(object value, out decimal number)
        {
            number = 0m;

            if (value == null || value == DBNull.Value)
                return false;

            if (value is decimal)
            {
                number = (decimal)value;
                return true;
            }

            if (value is int || value is long || value is short || value is byte ||
                value is double || value is float)
            {
                number = Convert.ToDecimal(value);
                return true;
            }

            string text = Convert.ToString(value);

            if (string.IsNullOrWhiteSpace(text))
                return false;

            text = text.Replace(",", "").Trim();

            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
                return true;

            if (decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out number))
                return true;

            return false;
        }

        private bool IsNumericText(string text)
        {
            decimal number;
            return TryGetDecimal(text, out number);
        }

        private string ShortText(string text, int max)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = text.Replace("\r", " ").Replace("\n", " ").Trim();

            if (text.Length <= max)
                return text;

            return text.Substring(0, max) + "...";
        }

        private string AddPrefix(string title, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return title + ": " + value;
        }

        private string JoinParts(params string[] parts)
        {
            List<string> clean = parts
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();

            return string.Join("   |   ", clean.ToArray());
        }

        private StringFormat CreateRtlFormat(StringAlignment alignment)
        {
            StringFormat format = new StringFormat();
            format.Alignment = alignment;
            format.LineAlignment = StringAlignment.Center;
            format.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            format.Trimming = StringTrimming.EllipsisCharacter;
            return format;
        }
    }
}
/*using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace water3.Services
{
    public class DynamicGridPrintService
    {
        private class CompanyInfo
        {
            public string CompanyName { get; set; }
            public string SystemName { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public string Address { get; set; }
            public string LogoPath { get; set; }
        }

        private class PrintColumn
        {
            public DataGridViewColumn GridColumn { get; set; }
            public string HeaderText { get; set; }
            public int PrintWidth { get; set; }
        }

        private DataGridView _grid;
        private List<PrintColumn> _columns = new List<PrintColumn>();
        private List<DataGridViewRow> _rows = new List<DataGridViewRow>();

        private string _reportTitle;
        private string _reportType;
        private string _periodInfo;
        private string _filterInfo;
        private string _totalsInfo;

        private int _rowIndex;
        private int _pageNumber;

        private readonly Color Primary = Color.FromArgb(0, 102, 204);
        private readonly Color Border = Color.FromArgb(160, 160, 160);
        private readonly Color AltRow = Color.FromArgb(245, 247, 250);

        public void PrintPreview(DataGridView grid, string reportTitle)
        {
            PrintPreview(
                grid,
                reportTitle,
                reportTitle,
                "الفترة: غير محددة",
                "الفلاتر: غير محددة",
                BuildTotalsFromGrid(grid)
            );
        }

        public void PrintPreview(
            DataGridView grid,
            string reportTitle,
            string reportType,
            string periodInfo,
            string filterInfo,
            string totalsInfo)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            if (grid.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للطباعة.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _grid = grid;
            _reportTitle = string.IsNullOrWhiteSpace(reportTitle) ? "تقرير" : reportTitle;
            _reportType = string.IsNullOrWhiteSpace(reportType) ? _reportTitle : reportType;
            _periodInfo = string.IsNullOrWhiteSpace(periodInfo) ? "الفترة: غير محددة" : periodInfo;
            _filterInfo = string.IsNullOrWhiteSpace(filterInfo) ? "الفلاتر: غير محددة" : filterInfo;
            _totalsInfo = string.IsNullOrWhiteSpace(totalsInfo) ? BuildTotalsFromGrid(grid) : totalsInfo;

            PrepareRowsAndColumns();

            if (_columns.Count == 0)
            {
                MessageBox.Show("لا توجد أعمدة ظاهرة للطباعة.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrintDocument doc = new PrintDocument();
            doc.DocumentName = _reportTitle;
            doc.DefaultPageSettings.Landscape = true;
            doc.DefaultPageSettings.Margins = new Margins(35, 35, 35, 35);

            doc.BeginPrint += delegate
            {
                _rowIndex = 0;
                _pageNumber = 1;
            };

            doc.PrintPage += PrintPage;

            using (PrintPreviewDialog preview = new PrintPreviewDialog())
            {
                preview.Document = doc;
                preview.WindowState = FormWindowState.Maximized;
                preview.Text = "معاينة الطباعة - " + _reportTitle;
                preview.ShowDialog();
            }
        }

        private void PrepareRowsAndColumns()
        {
            _columns = _grid.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => c.Visible)
                .OrderBy(c => c.DisplayIndex)
                .Select(c => new PrintColumn
                {
                    GridColumn = c,
                    HeaderText = string.IsNullOrWhiteSpace(c.HeaderText) ? c.Name : c.HeaderText
                })
                .ToList();

            _rows = _grid.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .ToList();
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            Rectangle bounds = e.MarginBounds;

            CalculateColumnWidths(bounds.Width);

            int y = DrawCompanyAndReportHeader(e);

            y += 8;

            int tableX = bounds.Left;
            int tableWidth = bounds.Width;
            int rowHeight = 28;
            int footerHeight = 26;

            DrawTableHeader(e.Graphics, tableX, y, tableWidth, rowHeight);
            y += rowHeight;

            while (_rowIndex < _rows.Count)
            {
                if (y + rowHeight > bounds.Bottom - footerHeight)
                {
                    DrawFooter(e.Graphics, bounds);
                    _pageNumber++;
                    e.HasMorePages = true;
                    return;
                }

                DrawTableRow(e.Graphics, _rows[_rowIndex], tableX, y, rowHeight, _rowIndex);
                y += rowHeight;
                _rowIndex++;
            }

            DrawFooter(e.Graphics, bounds);
            _pageNumber++;
            e.HasMorePages = false;
        }

        private int DrawCompanyAndReportHeader(PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Rectangle bounds = e.MarginBounds;

            CompanyInfo info = LoadCompanyInfo();

            Font companyFont = new Font("Tahoma", 14F, FontStyle.Bold);
            Font systemFont = new Font("Tahoma", 9.5F, FontStyle.Bold);
            Font infoFont = new Font("Tahoma", 8.3F);
            Font titleFont = new Font("Tahoma", 13F, FontStyle.Bold);
            Font smallBoldFont = new Font("Tahoma", 8.3F, FontStyle.Bold);

            StringFormat rtlRight = CreateRtlFormat(StringAlignment.Far);
            StringFormat rtlCenter = CreateRtlFormat(StringAlignment.Center);

            int x = bounds.Left;
            int y = bounds.Top;
            int width = bounds.Width;

            int logoSize = 68;
            int logoX = bounds.Right - logoSize;
            int logoY = y;

            Image logo = LoadLogo(info.LogoPath);

            if (logo != null)
            {
                g.DrawImage(logo, new Rectangle(logoX, logoY, logoSize, logoSize));
                logo.Dispose();
            }

            RectangleF companyRect = new RectangleF(x, y, width - logoSize - 12, 24);
            RectangleF systemRect = new RectangleF(x, y + 24, width - logoSize - 12, 20);
            RectangleF infoRect1 = new RectangleF(x, y + 44, width - logoSize - 12, 18);
            RectangleF infoRect2 = new RectangleF(x, y + 62, width - logoSize - 12, 18);

            g.DrawString(info.CompanyName, companyFont, Brushes.Black, companyRect, rtlRight);
            g.DrawString(info.SystemName, systemFont, Brushes.Black, systemRect, rtlRight);

            string line1 = JoinParts(
                AddPrefix("الهاتف", info.Phone),
                AddPrefix("البريد", info.Email)
            );

            string line2 = AddPrefix("العنوان", info.Address);

            g.DrawString(line1, infoFont, Brushes.Black, infoRect1, rtlRight);
            g.DrawString(line2, infoFont, Brushes.Black, infoRect2, rtlRight);

            int lineY = y + 84;
            g.DrawLine(Pens.Gray, x, lineY, bounds.Right, lineY);

            RectangleF titleRect = new RectangleF(x, lineY + 6, width, 26);
            g.DrawString(_reportTitle, titleFont, Brushes.Black, titleRect, rtlCenter);

            int infoY = lineY + 36;

            DrawInfoText(g, "نوع التقرير: " + _reportType, x, infoY, width, smallBoldFont, rtlRight);
            infoY += 18;

            DrawInfoText(g, _periodInfo, x, infoY, width, smallBoldFont, rtlRight);
            infoY += 18;

            DrawInfoText(g, _filterInfo, x, infoY, width, smallBoldFont, rtlRight);
            infoY += 18;

            DrawInfoText(g, _totalsInfo, x, infoY, width, smallBoldFont, rtlRight);
            infoY += 22;

            return infoY;
        }

        private void DrawInfoText(Graphics g, string text, int x, int y, int width, Font font, StringFormat format)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            RectangleF rect = new RectangleF(x, y, width, 18);
            g.DrawString(text, font, Brushes.Black, rect, format);
        }

        private void DrawTableHeader(Graphics g, int x, int y, int tableWidth, int height)
        {
            Font headerFont = new Font("Tahoma", 8.3F, FontStyle.Bold);
            StringFormat center = CreateRtlFormat(StringAlignment.Center);

            using (SolidBrush backBrush = new SolidBrush(Primary))
            using (Pen pen = new Pen(Border))
            {
                int right = x + tableWidth;

                foreach (PrintColumn col in _columns)
                {
                    Rectangle rect = new Rectangle(right - col.PrintWidth, y, col.PrintWidth, height);

                    g.FillRectangle(backBrush, rect);
                    g.DrawRectangle(pen, rect);
                    g.DrawString(ShortText(col.HeaderText, 35), headerFont, Brushes.White, rect, center);

                    right -= col.PrintWidth;
                }
            }
        }

        private void DrawTableRow(Graphics g, DataGridViewRow row, int x, int y, int height, int rowIndex)
        {
            Font cellFont = new Font("Tahoma", 8F);
            StringFormat rightFormat = CreateRtlFormat(StringAlignment.Far);
            StringFormat centerFormat = CreateRtlFormat(StringAlignment.Center);

            Brush textBrush = Brushes.Black;

            using (SolidBrush backBrush = new SolidBrush(rowIndex % 2 == 0 ? Color.White : AltRow))
            using (Pen pen = new Pen(Border))
            {
                int right = x + _columns.Sum(c => c.PrintWidth);

                foreach (PrintColumn col in _columns)
                {
                    Rectangle rect = new Rectangle(right - col.PrintWidth, y, col.PrintWidth, height);

                    g.FillRectangle(backBrush, rect);
                    g.DrawRectangle(pen, rect);

                    string text = GetCellText(row, col.GridColumn);
                    StringFormat format = IsNumericText(text) ? centerFormat : rightFormat;

                    RectangleF textRect = new RectangleF(rect.X + 3, rect.Y + 2, rect.Width - 6, rect.Height - 4);
                    g.DrawString(ShortText(text, 45), cellFont, textBrush, textRect, format);

                    right -= col.PrintWidth;
                }
            }
        }

        private void DrawFooter(Graphics g, Rectangle bounds)
        {
            Font footerFont = new Font("Tahoma", 8F, FontStyle.Bold);

            StringFormat center = CreateRtlFormat(StringAlignment.Center);
            StringFormat left = CreateRtlFormat(StringAlignment.Near);

            int y = bounds.Bottom + 8;

            RectangleF pageRect = new RectangleF(bounds.Left, y, bounds.Width, 20);
            g.DrawString("صفحة " + _pageNumber, footerFont, Brushes.Gray, pageRect, center);

            RectangleF dateRect = new RectangleF(bounds.Left, y, bounds.Width, 20);
            g.DrawString("تاريخ الطباعة: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                footerFont, Brushes.Gray, dateRect, left);
        }

        private void CalculateColumnWidths(int availableWidth)
        {
            if (_columns == null || _columns.Count == 0)
                return;

            int originalTotal = _columns.Sum(c => Math.Max(50, c.GridColumn.Width));

            if (originalTotal <= 0)
                originalTotal = _columns.Count * 80;

            int minWidth = Math.Max(35, Math.Min(70, availableWidth / _columns.Count));

            int used = 0;

            foreach (PrintColumn col in _columns)
            {
                int baseWidth = Math.Max(50, col.GridColumn.Width);
                int w = (int)Math.Round((baseWidth / (double)originalTotal) * availableWidth);

                if (w < minWidth)
                    w = minWidth;

                col.PrintWidth = w;
                used += w;
            }

            if (used != availableWidth && _columns.Count > 0)
            {
                int diff = availableWidth - used;
                _columns[_columns.Count - 1].PrintWidth += diff;

                if (_columns[_columns.Count - 1].PrintWidth < 30)
                    _columns[_columns.Count - 1].PrintWidth = 30;
            }
        }

        private CompanyInfo LoadCompanyInfo()
        {
            CompanyInfo info = new CompanyInfo();

            try
            {
                AppSchemaService.EnsureProductionTables();

                info.CompanyName = AppSettingsService.Get("Company.Name", "مؤسسة المياه");
                info.SystemName = AppSettingsService.Get("System.Name", "نظام إدارة المياه");
                info.Phone = AppSettingsService.Get("Company.Phone", "");
                info.Email = AppSettingsService.Get("Company.Email", "");
                info.Address = AppSettingsService.Get("Company.Address", "");
                info.LogoPath = AppSettingsService.Get("Company.LogoPath", "");
            }
            catch
            {
                info.CompanyName = "مؤسسة المياه";
                info.SystemName = "نظام إدارة المياه";
                info.Phone = "";
                info.Email = "";
                info.Address = "";
                info.LogoPath = "";
            }

            return info;
        }

        private Image LoadLogo(string logoPath)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(logoPath) && File.Exists(logoPath))
                {
                    using (Image img = Image.FromFile(logoPath))
                    {
                        return new Bitmap(img);
                    }
                }

                string fallback = Path.Combine(Application.StartupPath, "Assets", "company_logo.png");

                if (File.Exists(fallback))
                {
                    using (Image img = Image.FromFile(fallback))
                    {
                        return new Bitmap(img);
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        private string GetCellText(DataGridViewRow row, DataGridViewColumn column)
        {
            try
            {
                object value = row.Cells[column.Index].Value;

                if (value == null || value == DBNull.Value)
                    return string.Empty;

                if (value is DateTime)
                    return ((DateTime)value).ToString("yyyy-MM-dd");

                if (value is bool)
                    return ((bool)value) ? "نعم" : "لا";

                object formatted = row.Cells[column.Index].FormattedValue;

                if (formatted != null)
                    return Convert.ToString(formatted);

                return Convert.ToString(value);
            }
            catch
            {
                return string.Empty;
            }
        }

        private string BuildTotalsFromGrid(DataGridView grid)
        {
            try
            {
                if (grid == null)
                    return "الإجمالي: لا توجد بيانات";

                int rows = grid.Rows.Cast<DataGridViewRow>().Count(r => !r.IsNewRow);
                int cols = grid.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible);

                List<string> totals = new List<string>();

                foreach (DataGridViewColumn col in grid.Columns.Cast<DataGridViewColumn>().Where(c => c.Visible))
                {
                    if (!IsSummableColumn(col))
                        continue;

                    decimal sum = 0m;
                    bool hasValue = false;

                    foreach (DataGridViewRow row in grid.Rows)
                    {
                        if (row.IsNewRow)
                            continue;

                        object value = row.Cells[col.Index].Value;

                        if (value == null || value == DBNull.Value)
                            continue;

                        decimal number;

                        if (decimal.TryParse(Convert.ToString(value), out number))
                        {
                            sum += number;
                            hasValue = true;
                        }
                    }

                    if (hasValue)
                    {
                        totals.Add(col.HeaderText + ": " + sum.ToString("N2"));

                        if (totals.Count >= 3)
                            break;
                    }
                }

                string result = "الإجمالي: عدد السجلات " + rows.ToString("N0") +
                                " | عدد الأعمدة " + cols.ToString("N0");

                if (totals.Count > 0)
                    result += " | " + string.Join(" | ", totals.ToArray());

                return result;
            }
            catch
            {
                return "الإجمالي: غير متاح";
            }
        }

        private bool IsSummableColumn(DataGridViewColumn col)
        {
            string name = (col.DataPropertyName + " " + col.Name + " " + col.HeaderText).ToLowerInvariant();

            if (name.Contains("id") ||
                name.Contains("code") ||
                name.Contains("number") ||
                name.Contains("no") ||
                name.Contains("رقم") ||
                name.Contains("كود") ||
                name.Contains("معرف"))
                return false;

            if (name.Contains("amount") ||
                name.Contains("total") ||
                name.Contains("paid") ||
                name.Contains("due") ||
                name.Contains("balance") ||
                name.Contains("price") ||
                name.Contains("cost") ||
                name.Contains("المبلغ") ||
                name.Contains("الإجمالي") ||
                name.Contains("اجمالي") ||
                name.Contains("المدفوع") ||
                name.Contains("المتبقي") ||
                name.Contains("الرصيد") ||
                name.Contains("السعر") ||
                name.Contains("التكلفة"))
                return true;

            return false;
        }

        private bool IsNumericText(string text)
        {
            decimal number;
            return decimal.TryParse(text, out number);
        }

        private string ShortText(string text, int max)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = text.Replace("\r", " ").Replace("\n", " ").Trim();

            if (text.Length <= max)
                return text;

            return text.Substring(0, max) + "...";
        }

        private string AddPrefix(string title, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return title + ": " + value;
        }

        private string JoinParts(params string[] parts)
        {
            List<string> clean = parts
                .Where(p => !string.IsNullOrWhiteSpace(p))
                .ToList();

            return string.Join("   |   ", clean.ToArray());
        }

        private StringFormat CreateRtlFormat(StringAlignment alignment)
        {
            StringFormat format = new StringFormat();
            format.Alignment = alignment;
            format.LineAlignment = StringAlignment.Center;
            format.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            format.Trimming = StringTrimming.EllipsisCharacter;
            return format;
        }
    }
}
*/