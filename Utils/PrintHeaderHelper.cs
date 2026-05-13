using System;
using System.Drawing;
using System.Drawing.Printing;

namespace water3.Utils
{
    public static class PrintHeaderHelper
    {
        public static int DrawCompanyHeader(
            PrintPageEventArgs e,
            string reportTitle,
            DateTime fromDate,
            DateTime toDate)
        {
            CompanyPrintInfoData info = CompanyPrintInfo.Get();

            int x = e.MarginBounds.Left;
            int y = e.MarginBounds.Top;
            int width = e.MarginBounds.Width;

            Font companyFont = new Font("Tahoma", 14F, FontStyle.Bold);
            Font systemFont = new Font("Tahoma", 10F, FontStyle.Bold);
            Font infoFont = new Font("Tahoma", 8.5F);
            Font titleFont = new Font("Tahoma", 13F, FontStyle.Bold);
            Font periodFont = new Font("Tahoma", 8.5F, FontStyle.Bold);

            StringFormat rtlRight = new StringFormat
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.DirectionRightToLeft
            };

            StringFormat rtlCenter = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.DirectionRightToLeft
            };

            int logoSize = 70;
            int logoX = x;
            int logoY = y;

            Image logo = CompanyPrintInfo.LoadLogo(info.LogoPath);

            if (logo != null)
            {
                e.Graphics.DrawImage(logo, new Rectangle(logoX, logoY, logoSize, logoSize));
                logo.Dispose();
            }

            RectangleF companyRect = new RectangleF(x + logoSize + 10, y, width - logoSize - 10, 24);
            RectangleF systemRect = new RectangleF(x + logoSize + 10, y + 24, width - logoSize - 10, 20);
            RectangleF addressRect = new RectangleF(x + logoSize + 10, y + 44, width - logoSize - 10, 18);
            RectangleF phoneRect = new RectangleF(x + logoSize + 10, y + 62, width - logoSize - 10, 18);

            e.Graphics.DrawString(Safe(info.CompanyName), companyFont, Brushes.Black, companyRect, rtlRight);
            e.Graphics.DrawString(Safe(info.SystemName), systemFont, Brushes.Black, systemRect, rtlRight);
            e.Graphics.DrawString("العنوان: " + Safe(info.Address), infoFont, Brushes.Black, addressRect, rtlRight);

            string contact = "الهاتف: " + Safe(info.Phone);
            if (!string.IsNullOrWhiteSpace(info.Email))
                contact += "    |    البريد: " + info.Email;

            e.Graphics.DrawString(contact, infoFont, Brushes.Black, phoneRect, rtlRight);

            int lineY = y + 84;
            e.Graphics.DrawLine(Pens.Gray, x, lineY, x + width, lineY);

            RectangleF titleRect = new RectangleF(x, lineY + 8, width, 28);
            e.Graphics.DrawString(reportTitle, titleFont, Brushes.Black, titleRect, rtlCenter);

            string period =
                "الفترة من: " + fromDate.ToString("yyyy-MM-dd") +
                " إلى: " + toDate.ToString("yyyy-MM-dd") +
                "    |    تاريخ الطباعة: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm");

            RectangleF periodRect = new RectangleF(x, lineY + 38, width, 22);
            e.Graphics.DrawString(period, periodFont, Brushes.Black, periodRect, rtlCenter);

            return lineY + 70;
        }

        public static void DrawCompanyFooter(
            PrintPageEventArgs e,
            int pageNumber)
        {
            CompanyPrintInfoData info = CompanyPrintInfo.Get();

            int x = e.MarginBounds.Left;
            int width = e.MarginBounds.Width;
            int y = e.MarginBounds.Bottom + 18;

            Font footerFont = new Font("Tahoma", 8F, FontStyle.Regular);
            Font pageFont = new Font("Tahoma", 8F, FontStyle.Bold);

            StringFormat rtlCenter = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                FormatFlags = StringFormatFlags.DirectionRightToLeft
            };

            e.Graphics.DrawLine(Pens.LightGray, x, y - 6, x + width, y - 6);

            string footer = string.IsNullOrWhiteSpace(info.InvoiceFooter)
                ? ""
                : info.InvoiceFooter.Trim();

            if (!string.IsNullOrWhiteSpace(footer))
            {
                e.Graphics.DrawString(footer, footerFont, Brushes.DimGray,
                    new RectangleF(x, y, width, 18), rtlCenter);
            }

            e.Graphics.DrawString("صفحة " + pageNumber.ToString(), pageFont, Brushes.DimGray,
                new RectangleF(x, y + 18, width, 18), rtlCenter);
        }

        private static string Safe(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
