using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace water3.Services
{
   
   
        public class SubscriberInvoicePrintService
        {
            private class CompanyInfo
            {
                public string CompanyName { get; set; }
                public string CompanyNameEn { get; set; }
                public string SystemName { get; set; }
                public string Phone { get; set; }
                public string Mobile { get; set; }
                public string Address { get; set; }
                public string LogoPath { get; set; }
                public string Currency { get; set; }
            }

            private class InvoiceData
            {
                public string InvoiceNo { get; set; }
                public string Date { get; set; }
                public string FromDate { get; set; }
                public string ToDate { get; set; }

                public string SubscriberName { get; set; }
                public string MeterNo { get; set; }
                public string Address { get; set; }
                public string Square { get; set; }

                public decimal Previous { get; set; }
                public decimal Current { get; set; }
                public decimal Consumption { get; set; }
                public decimal UnitPrice { get; set; }
                public decimal ConsumptionValue { get; set; }
                public decimal ServiceFees { get; set; }
                public decimal Arrears { get; set; }
                public decimal Total { get; set; }
                public decimal Paid { get; set; }
                public decimal Balance { get; set; }

                public string Status { get; set; }
                public string TotalWords { get; set; }
            }
        private string _defaultFromDateText = "";
        private string _defaultToDateText = "";
        private DataGridView _grid;
            private List<DataGridViewRow> _rows;
            private int _invoiceIndex;
            private int _pageNumber;

            private readonly Color Blue = Color.FromArgb(0, 160, 210);
            private readonly Color DarkBlue = Color.FromArgb(0, 51, 120);
            private readonly Color LightBlue = Color.FromArgb(230, 245, 255);
            private readonly Color GridBack = Color.FromArgb(248, 250, 252);
            private readonly Color Red = Color.FromArgb(220, 38, 38);

        public void PrintPreview(DataGridView grid)
        {
            PrintPreview(grid, "", "");
        }

        public void PrintPreview(DataGridView grid, string fromDateText, string toDateText)
        {
            if (grid == null)
                throw new ArgumentNullException("grid");

            _grid = grid;
            _defaultFromDateText = fromDateText ?? "";
            _defaultToDateText = toDateText ?? "";

            _rows = grid.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .ToList();

            if (_rows.Count == 0)
            {
                MessageBox.Show("لا توجد فواتير للطباعة.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PrintDocument doc = new PrintDocument();
            doc.DocumentName = "فواتير المشتركين";
            doc.DefaultPageSettings.Landscape = true;
            doc.DefaultPageSettings.Margins = new Margins(35, 35, 30, 30);

            doc.BeginPrint += delegate
            {
                _invoiceIndex = 0;
                _pageNumber = 1;
            };

            doc.PrintPage += PrintPage;

            using (PrintPreviewDialog preview = new PrintPreviewDialog())
            {
                preview.Document = doc;
                preview.WindowState = FormWindowState.Maximized;
                preview.Text = "معاينة طباعة فواتير المشتركين";
                preview.ShowDialog();
            }
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
            {
                if (_invoiceIndex >= _rows.Count)
                {
                    e.HasMorePages = false;
                    return;
                }

                e.Graphics.TextRenderingHint =
                    System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

                CompanyInfo company = LoadCompanyInfo();
                InvoiceData invoice = BuildInvoiceData(_rows[_invoiceIndex], _invoiceIndex + 1, company);

                DrawInvoice(e.Graphics, e.MarginBounds, company, invoice);

                DrawFooter(e.Graphics, e.MarginBounds);

                _invoiceIndex++;
                _pageNumber++;

                e.HasMorePages = _invoiceIndex < _rows.Count;
            }

            private void DrawInvoice(Graphics g, Rectangle bounds, CompanyInfo company, InvoiceData inv)
            {
                int invoiceWidth = Math.Min(780, bounds.Width);
                int x = bounds.Left + ((bounds.Width - invoiceWidth) / 2);
                int y = bounds.Top;
                int w = invoiceWidth;

                Font arTitleFont = new Font("Tahoma", 12F, FontStyle.Bold);
                Font enTitleFont = new Font("Tahoma", 12F, FontStyle.Bold);
                Font smallFont = new Font("Tahoma", 8F, FontStyle.Bold);
                Font normalFont = new Font("Tahoma", 8.5F, FontStyle.Bold);
                Font tableFont = new Font("Tahoma", 8F, FontStyle.Bold);
                Font valueFont = new Font("Tahoma", 8.2F, FontStyle.Bold);

                StringFormat center = CenterFormat();
                StringFormat right = RightFormat();
                StringFormat left = LeftFormat();

                // Header: Arabic name - logo - English name
                int logoSize = 52;
                int logoX = x + (w / 2) - (logoSize / 2);

                Rectangle arRect = new Rectangle(x, y + 4, (w / 2) - 40, 55);
                Rectangle enRect = new Rectangle(x + (w / 2) + 40, y + 4, (w / 2) - 40, 55);

                g.DrawString(company.CompanyName, arTitleFont, new SolidBrush(DarkBlue), arRect, center);
                g.DrawString(company.CompanyNameEn, enTitleFont, new SolidBrush(DarkBlue), enRect, center);

                Image logo = LoadLogo(company.LogoPath);

                if (logo != null)
                {
                    g.DrawImage(logo, new Rectangle(logoX, y, logoSize, logoSize));
                    logo.Dispose();
                }
                else
                {
                    g.DrawString("logo", smallFont, Brushes.Black,
                        new Rectangle(logoX, y + 10, logoSize, 20), center);
                }

                y += 64;

                // Address / mobile bar
                DrawCell(g, new Rectangle(x, y, w, 30),
                    "العنوان / " + company.Address + "    /    جوال مصعب / ت: " + company.Mobile,
                    normalFont, Color.White, Brushes.Black, true, center);

                y += 40;

                // Invoice title row
                int titleW = 210;
                int sideW = (w - titleW - 20) / 2;

                Rectangle dateRect = new Rectangle(x, y, sideW, 38);
                Rectangle titleRect = new Rectangle(x + sideW + 10, y, titleW, 38);
                Rectangle noRect = new Rectangle(x + sideW + titleW + 20, y, sideW, 38);

                DrawCell(g, dateRect, "التاريخ : " + inv.Date, normalFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, titleRect, "فاتورة استهلاك مياه", normalFont, Blue, Brushes.White, true, center);
                DrawCell(g, noRect, "الرقم : " + inv.InvoiceNo, normalFont, Color.White, new SolidBrush(Red), true, center);

                y += 48;

                // Main grid
                int col = w / 8;
                int rowH = 30;

                // Section row
                DrawCell(g, new Rectangle(x, y, col * 3, rowH), "قراءة العداد", tableFont, Blue, Brushes.White, true, center);
                DrawCell(g, new Rectangle(x + col * 3, y, col * 3, rowH), "بيانات المشترك", tableFont, Blue, Brushes.White, true, center);
                DrawCell(g, new Rectangle(x + col * 6, y, col * 2, rowH), "الفترة", tableFont, Blue, Brushes.White, true, center);

                y += rowH;

                DrawCell(g, new Rectangle(x, y, col * 3, rowH), "قراءة العداد", tableFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 3, y, col, rowH), "رقم العداد", tableFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 4, y, col * 2, rowH), "اسم المشترك", tableFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 6, y, col, rowH), "إلى تاريخ", tableFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 7, y, w - (col * 7), rowH), "من تاريخ", tableFont, Color.White, Brushes.Black, true, center);

                y += rowH;

                DrawCell(g, new Rectangle(x, y, col * 3, rowH), "", valueFont, GridBack, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 3, y, col, rowH), inv.MeterNo, valueFont, GridBack, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 4, y, col * 2, rowH), inv.SubscriberName, valueFont, GridBack, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 6, y, col, rowH), inv.ToDate, valueFont, GridBack, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 7, y, w - (col * 7), rowH), inv.FromDate, valueFont, GridBack, Brushes.Black, true, center);

                y += rowH;

                DrawCell(g, new Rectangle(x, y, col, rowH), "ك. الاستهلاك", tableFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col, y, col, rowH), "ق. الحالية", tableFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 2, y, col, rowH), "ق. السابقة", tableFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 3, y, col, rowH), "", tableFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 4, y, col * 2, rowH), "المربع", tableFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 6, y, col * 2, rowH), "العنوان", tableFont, Color.White, Brushes.Black, true, center);

                y += rowH;

                DrawCell(g, new Rectangle(x, y, col, rowH), inv.Consumption.ToString("N2"), valueFont, GridBack, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col, y, col, rowH), inv.Current.ToString("N2"), valueFont, GridBack, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 2, y, col, rowH), inv.Previous.ToString("N2"), valueFont, GridBack, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 3, y, col, rowH), "", valueFont, GridBack, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 4, y, col * 2, rowH), inv.Square, valueFont, GridBack, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 6, y, col * 2, rowH), inv.Address, valueFont, GridBack, Brushes.Black, true, center);

                y += rowH;

                DrawCell(g, new Rectangle(x, y, col, rowH), "القيمة الإجمالية ر.ي", tableFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col, y, col, rowH), "القيمة للاستهلاك", tableFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 2, y, col, rowH), "إ.ر. الاستهلاك", tableFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 3, y, col, rowH), "قيمة الاستهلاك", tableFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 4, y, col, rowH), "رسوم خدمات", tableFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 5, y, col, rowH), "المتأخرات", tableFont, Color.White, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 6, y, col * 2, rowH), "القيمة المستحقة", tableFont, Color.White, Brushes.Black, true, center);

                y += rowH;

                DrawCell(g, new Rectangle(x, y, col, rowH), inv.Total.ToString("N2"), valueFont, GridBack, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col, y, col, rowH), inv.Consumption.ToString("N2"), valueFont, GridBack, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 2, y, col, rowH), inv.UnitPrice.ToString("N2"), valueFont, GridBack, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 3, y, col, rowH), inv.ConsumptionValue.ToString("N2"), valueFont, GridBack, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 4, y, col, rowH), inv.ServiceFees.ToString("N2"), valueFont, GridBack, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 5, y, col, rowH), inv.Arrears.ToString("N2"), valueFont, GridBack, Brushes.Black, true, center);
                DrawCell(g, new Rectangle(x + col * 6, y, col * 2, rowH), inv.Total.ToString("N2"), valueFont, GridBack, Brushes.Black, true, center);

                y += rowH + 12;

                // Bottom totals
                int leftBoxW = (w - 12) / 2;
                int rightBoxW = w - leftBoxW - 12;

                DrawCell(g, new Rectangle(x, y, leftBoxW, 34),
                    "إجمالي المبلغ المستحق كتابة", tableFont, Color.White, Brushes.Black, true, center);

                DrawCell(g, new Rectangle(x + leftBoxW + 12, y, rightBoxW / 2, 34),
                    "الرصيد الحالي", tableFont, Color.White, Brushes.Black, true, center);

                DrawCell(g, new Rectangle(x + leftBoxW + 12 + (rightBoxW / 2), y, rightBoxW - (rightBoxW / 2), 34),
                    "المدفوعة السابقة", tableFont, Color.White, Brushes.Black, true, center);

                y += 34;

                DrawCell(g, new Rectangle(x, y, leftBoxW, 34),
                    inv.TotalWords, valueFont, GridBack, Brushes.Black, true, center);

                DrawCell(g, new Rectangle(x + leftBoxW + 12, y, rightBoxW / 2, 34),
                    inv.Balance.ToString("N2"), valueFont, GridBack, Brushes.Black, true, center);

                DrawCell(g, new Rectangle(x + leftBoxW + 12 + (rightBoxW / 2), y, rightBoxW - (rightBoxW / 2), 34),
                    inv.Paid.ToString("N2"), valueFont, GridBack, Brushes.Black, true, center);

                y += 50;

                Font noteFont = new Font("Tahoma", 8F, FontStyle.Bold);

                string note1 = "• نرجو سرعة تسديد الفاتورة في مدة أقصاها 3 أيام من إصدارها، وفي حالة التأخير سيتم قطع الخدمة ولن يتم إعادتها إلا بغرامة مالية.";
                string note2 = "• يخلي مسؤولو المشروع أي مشترك من ارتكاب أي مخالفات كالتوصيل في أماكن غير مسموحة أو غير مطابقة للتصميم.";

                g.DrawString(note1, noteFont, Brushes.Black, new Rectangle(x, y, w, 22), right);
                y += 22;
                g.DrawString(note2, noteFont, Brushes.Black, new Rectangle(x, y, w, 22), right);
            }

            private void DrawFooter(Graphics g, Rectangle bounds)
            {
                Font footerFont = new Font("Tahoma", 8F, FontStyle.Bold);

                StringFormat center = CenterFormat();
                StringFormat left = LeftFormat();

                int y = bounds.Bottom + 6;

                g.DrawString("صفحة " + _pageNumber,
                    footerFont,
                    Brushes.Gray,
                    new Rectangle(bounds.Left, y, bounds.Width, 20),
                    center);

                g.DrawString("تاريخ الطباعة: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    footerFont,
                    Brushes.Gray,
                    new Rectangle(bounds.Left, y, bounds.Width, 20),
                    left);
            }

            private void DrawCell(
                Graphics g,
                Rectangle rect,
                string text,
                Font font,
                Color backColor,
                Brush foreBrush,
                bool border,
                StringFormat format)
            {
                using (SolidBrush back = new SolidBrush(backColor))
                using (Pen pen = new Pen(Blue, 1.5F))
                {
                    g.FillRectangle(back, rect);

                    if (border)
                        g.DrawRectangle(pen, rect);

                    Rectangle inner = new Rectangle(rect.X + 4, rect.Y + 2, rect.Width - 8, rect.Height - 4);

                    g.DrawString(text ?? string.Empty, font, foreBrush, inner, format);
                }
            }

        private InvoiceData BuildInvoiceData(DataGridViewRow row, int index, CompanyInfo company)
        {
            InvoiceData inv = new InvoiceData();

            inv.InvoiceNo = GetText(row,
                "رقم الفاتورة",
                "رقم الفاتوره",
                "InvoiceNo",
                "InvoiceNumber",
                "رقم المرجع",
                "RefNo"
            );

            if (string.IsNullOrWhiteSpace(inv.InvoiceNo))
                inv.InvoiceNo = index.ToString("000000");

            inv.Date = GetText(row,
                "تاريخ الفاتورة",
                "تاريخ الفاتوره",
                "التاريخ",
                "InvoiceDate",
                "Date"
            );

            if (string.IsNullOrWhiteSpace(inv.Date))
                inv.Date = DateTime.Today.ToString("yyyy-MM-dd");

            inv.FromDate = GetText(row,
                "من تاريخ",
                "FromDate",
                "PeriodFrom"
            );

            inv.ToDate = GetText(row,
                "إلى تاريخ",
                "الى تاريخ",
                "ToDate",
                "PeriodTo"
            );

            // لا تحاول أخذ من/إلى من أعمدة عشوائية؛ إذا غير موجودة استخدم فلتر الشاشة
            if (string.IsNullOrWhiteSpace(inv.FromDate))
                inv.FromDate = string.IsNullOrWhiteSpace(_defaultFromDateText) ? "غير محدد" : _defaultFromDateText;

            if (string.IsNullOrWhiteSpace(inv.ToDate))
                inv.ToDate = string.IsNullOrWhiteSpace(_defaultToDateText) ? "غير محدد" : _defaultToDateText;

            inv.SubscriberName = GetText(row,
                "اسم المشترك",
                "اسم المستهلك",
                "المشترك",
                "Name",
                "SubscriberName",
                "CustomerName"
            );

            inv.MeterNo = GetText(row,
                "رقم العداد",
                "Meter",
                "MeterNo",
                "MeterNumber"
            );

            inv.Address = GetText(row,
                "العنوان",
                "Address"
            );

            inv.Square = GetText(row,
                "المربع",
                "Square",
                "Block"
            );

            inv.Previous = GetDecimal(row,
                "ق. السابقة",
                "ق السابقة",
                "السابقة",
                "قراءة سابقة",
                "Previous",
                "Prev"
            );

            inv.Current = GetDecimal(row,
                "ق. الحالية",
                "ق الحالية",
                "الحالية",
                "قراءة حالية",
                "Current",
                "Curr"
            );

            inv.Consumption = GetDecimal(row,
                "ك. الاستهلاك",
                "الاستهلاك",
                "الاستهلاك م3",
                "Consumption",
                "Cons"
            );

            inv.UnitPrice = GetDecimal(row,
                "سعر الوحدة",
                "سعر وحدة",
                "UnitPrice",
                "Price"
            );

            inv.ConsumptionValue = GetDecimal(row,
                "قيمة الاستهلاك",
                "القيمة للاستهلاك",
                "ConsumptionValue",
                "ConsValue"
            );

            inv.ServiceFees = GetDecimal(row,
                "رسوم الخدمة",
                "رسوم خدمات",
                "ServiceFees",
                "Fees"
            );

            inv.Arrears = GetDecimal(row,
                "المتأخرات",
                "المتاخرات",
                "Arrears"
            );

            inv.Total = GetDecimal(row,
                "الإجمالي",
                "اجمالي",
                "القيمة المستحقة",
                "القيمة الاجمالية",
                "Total",
                "Amount",
                "المبلغ"
            );

            inv.Paid = GetDecimal(row,
                "المدفوعة السابقة",
                "المدفوع",
                "Paid"
            );

            inv.Balance = GetDecimal(row,
                "الرصيد الحالي",
                "المتبقي",
                "Balance",
                "Remaining"
            );

            inv.Status = GetText(row,
                "الحالة",
                "Status"
            );

            // حسابات احتياطية إذا لم تكن الأعمدة موجودة
            if (inv.Consumption <= 0 && inv.Current > 0 && inv.Previous >= 0)
                inv.Consumption = inv.Current - inv.Previous;

            if (inv.ConsumptionValue <= 0 && inv.Consumption > 0 && inv.UnitPrice > 0)
                inv.ConsumptionValue = inv.Consumption * inv.UnitPrice;

            if (inv.Total <= 0)
                inv.Total = inv.ConsumptionValue + inv.ServiceFees + inv.Arrears;

            if (inv.Balance <= 0 && inv.Total > 0)
                inv.Balance = inv.Total - inv.Paid;

            inv.TotalWords = NumberToArabicWords(inv.Total, company.Currency);

            return inv;
        }
        private string GetText(DataGridViewRow row, params string[] possibleNames)
        {
            object value = GetCellValue(row, possibleNames);

            if (value == null || value == DBNull.Value)
                return string.Empty;

            if (value is DateTime)
                return ((DateTime)value).ToString("yyyy-MM-dd");

            return Convert.ToString(value);
        }

        private decimal GetDecimal(DataGridViewRow row, params string[] possibleNames)
        {
            object value = GetCellValue(row, possibleNames);

            decimal number;

            if (TryDecimal(value, out number))
                return number;

            return 0m;
        }

        private object GetCellValue(DataGridViewRow row, params string[] possibleNames)
        {
            if (row == null || row.DataGridView == null)
                return null;

            // المرحلة الأولى: تطابق كامل فقط
            foreach (DataGridViewColumn col in row.DataGridView.Columns)
            {
                string header = Normalize(col.HeaderText);
                string name = Normalize(col.Name);
                string prop = Normalize(col.DataPropertyName);

                foreach (string p in possibleNames)
                {
                    string target = Normalize(p);

                    if (string.IsNullOrWhiteSpace(target))
                        continue;

                    if (header == target || name == target || prop == target)
                        return row.Cells[col.Index].Value;
                }
            }

            // المرحلة الثانية: تطابق جزئي آمن، مع منع الكلمات القصيرة مثل "من" و "إلى"
            foreach (DataGridViewColumn col in row.DataGridView.Columns)
            {
                string header = Normalize(col.HeaderText);
                string name = Normalize(col.Name);
                string prop = Normalize(col.DataPropertyName);

                foreach (string p in possibleNames)
                {
                    string target = Normalize(p);

                    if (string.IsNullOrWhiteSpace(target))
                        continue;

                    // منع المطابقة الخاطئة للألفاظ القصيرة
                    if (target.Length < 4)
                        continue;

                    if (header.Contains(target) || name.Contains(target) || prop.Contains(target))
                        return row.Cells[col.Index].Value;
                }
            }

            return null;
        }

        private string Normalize(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            return value
                .Replace("أ", "ا")
                .Replace("إ", "ا")
                .Replace("آ", "ا")
                .Replace("ى", "ي")
                .Replace("ة", "ه")
                .Replace("ؤ", "و")
                .Replace("ئ", "ي")
                .Replace(".", "")
                .Replace(":", "")
                .Replace("/", "")
                .Replace("\\", "")
                .Replace(" ", "")
                .Replace("_", "")
                .Replace("-", "")
                .Trim()
                .ToLowerInvariant();
        }
        //private string GetText(DataGridViewRow row, params string[] possibleNames)
        //    {
        //        object value = GetCellValue(row, possibleNames);

        //        if (value == null || value == DBNull.Value)
        //            return string.Empty;

        //        if (value is DateTime)
        //            return ((DateTime)value).ToString("yyyy-MM-dd");

        //        return Convert.ToString(value);
        //    }

        //private decimal GetDecimal(DataGridViewRow row, params string[] possibleNames)
        //{
        //    object value = GetCellValue(row, possibleNames);
        //    decimal number;

        //    if (TryDecimal(value, out number))
        //        return number;

        //    return 0m;
        //}

        //private object GetCellValue(DataGridViewRow row, params string[] possibleNames)
        //{
        //    if (row == null || row.DataGridView == null)
        //        return null;

        //    foreach (DataGridViewColumn col in row.DataGridView.Columns)
        //    {
        //        string header = Normalize(col.HeaderText);
        //        string name = Normalize(col.Name);
        //        string prop = Normalize(col.DataPropertyName);

        //        foreach (string p in possibleNames)
        //        {
        //            string target = Normalize(p);

        //            if (header == target || name == target || prop == target)
        //                return row.Cells[col.Index].Value;
        //        }
        //    }

        //    foreach (DataGridViewColumn col in row.DataGridView.Columns)
        //    {
        //        string header = Normalize(col.HeaderText);
        //        string name = Normalize(col.Name);
        //        string prop = Normalize(col.DataPropertyName);

        //        foreach (string p in possibleNames)
        //        {
        //            string target = Normalize(p);

        //            if (header.Contains(target) || name.Contains(target) || prop.Contains(target))
        //                return row.Cells[col.Index].Value;
        //        }
        //    }

        //    return null;
        //}

        //private string Normalize(string value)
        //{
        //    if (string.IsNullOrWhiteSpace(value))
        //        return string.Empty;

        //    return value
        //        .Replace("أ", "ا")
        //        .Replace("إ", "ا")
        //        .Replace("آ", "ا")
        //        .Replace("ى", "ي")
        //        .Replace("ة", "ه")
        //        .Replace(" ", "")
        //        .Replace("_", "")
        //        .Replace("-", "")
        //        .Trim()
        //        .ToLowerInvariant();
        //}

        private bool TryDecimal(object value, out decimal number)
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

            private CompanyInfo LoadCompanyInfo()
            {
                CompanyInfo info = new CompanyInfo();

                try
                {
                    AppSchemaService.EnsureProductionTables();

                    info.CompanyName = AppSettingsService.Get("Company.Name", "مشروع المدينة للمياه النقية");
                    info.CompanyNameEn = AppSettingsService.Get("Company.NameEn", "Al-Madena Project for Pure Water");
                    info.SystemName = AppSettingsService.Get("System.Name", "نظام إدارة المياه");
                    info.Phone = AppSettingsService.Get("Company.Phone", "");
                    info.Mobile = AppSettingsService.Get("Company.Mobile", info.Phone);
                    info.Address = AppSettingsService.Get("Company.Address", "");
                    info.LogoPath = AppSettingsService.Get("Company.LogoPath", "");
                    info.Currency = AppSettingsService.Get("Invoice.Currency", "ريال");
                }
                catch
                {
                    info.CompanyName = "مشروع المدينة للمياه النقية";
                    info.CompanyNameEn = "Al-Madena Project for Pure Water";
                    info.SystemName = "نظام إدارة المياه";
                    info.Phone = "";
                    info.Mobile = "";
                    info.Address = "";
                    info.LogoPath = "";
                    info.Currency = "ريال";
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

            private string NumberToArabicWords(decimal amount, string currency)
            {
                long integer = (long)Math.Round(amount, 0);

                if (integer == 0)
                    return "صفر " + currency;

                return "فقط " + ConvertNumber(integer) + " " + currency + " لا غير";
            }

            private string ConvertNumber(long number)
            {
                if (number < 0)
                    return "سالب " + ConvertNumber(Math.Abs(number));

                if (number < 1000)
                    return ConvertUnder1000((int)number);

                if (number < 1000000)
                {
                    long thousands = number / 1000;
                    long rest = number % 1000;

                    string text = ConvertScale(thousands, "ألف", "ألفان", "آلاف");

                    if (rest > 0)
                        text += " و " + ConvertUnder1000((int)rest);

                    return text;
                }

                if (number < 1000000000)
                {
                    long millions = number / 1000000;
                    long rest = number % 1000000;

                    string text = ConvertScale(millions, "مليون", "مليونان", "ملايين");

                    if (rest > 0)
                        text += " و " + ConvertNumber(rest);

                    return text;
                }

                return number.ToString("N0");
            }

            private string ConvertScale(long number, string one, string two, string plural)
            {
                if (number == 1)
                    return one;

                if (number == 2)
                    return two;

                if (number >= 3 && number <= 10)
                    return ConvertUnder1000((int)number) + " " + plural;

                return ConvertUnder1000((int)number) + " " + one;
            }

            private string ConvertUnder1000(int number)
            {
                string[] ones =
                {
                "", "واحد", "اثنان", "ثلاثة", "أربعة", "خمسة",
                "ستة", "سبعة", "ثمانية", "تسعة", "عشرة",
                "أحد عشر", "اثنا عشر", "ثلاثة عشر", "أربعة عشر", "خمسة عشر",
                "ستة عشر", "سبعة عشر", "ثمانية عشر", "تسعة عشر"
            };

                string[] tens =
                {
                "", "", "عشرون", "ثلاثون", "أربعون", "خمسون",
                "ستون", "سبعون", "ثمانون", "تسعون"
            };

                string[] hundreds =
                {
                "", "مائة", "مائتان", "ثلاثمائة", "أربعمائة", "خمسمائة",
                "ستمائة", "سبعمائة", "ثمانمائة", "تسعمائة"
            };

                if (number == 0)
                    return "";

                if (number < 20)
                    return ones[number];

                if (number < 100)
                {
                    int one = number % 10;
                    int ten = number / 10;

                    if (one == 0)
                        return tens[ten];

                    return ones[one] + " و " + tens[ten];
                }

                int h = number / 100;
                int rest = number % 100;

                if (rest == 0)
                    return hundreds[h];

                return hundreds[h] + " و " + ConvertUnder1000(rest);
            }

            private StringFormat CenterFormat()
            {
                StringFormat f = new StringFormat();
                f.Alignment = StringAlignment.Center;
                f.LineAlignment = StringAlignment.Center;
                f.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                f.Trimming = StringTrimming.EllipsisCharacter;
                return f;
            }

            private StringFormat RightFormat()
            {
                StringFormat f = new StringFormat();
                f.Alignment = StringAlignment.Near;
                f.LineAlignment = StringAlignment.Center;
                f.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                f.Trimming = StringTrimming.EllipsisCharacter;
                return f;
            }

            private StringFormat LeftFormat()
            {
                StringFormat f = new StringFormat();
                f.Alignment = StringAlignment.Far;
                f.LineAlignment = StringAlignment.Center;
                f.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                f.Trimming = StringTrimming.EllipsisCharacter;
                return f;
            }
        }
    }