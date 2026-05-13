using System;
using System.Drawing;
using System.IO;
using water3.Services;

namespace water3.Utils
{
    public class CompanyPrintInfoData
    {
        public string CompanyName { get; set; }
        public string SystemName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string LogoPath { get; set; }
        public string InvoiceTitle { get; set; }
        public string InvoiceFooter { get; set; }
        public string Currency { get; set; }
    }

    public static class CompanyPrintInfo
    {
        public static CompanyPrintInfoData Get()
        {
            AppSchemaService.EnsureProductionTables();

            return new CompanyPrintInfoData
            {
                CompanyName = AppSettingsService.Get("Company.Name", "مؤسسة المياه"),
                SystemName = AppSettingsService.Get("Company.SystemName", "نظام إدارة المياه"),
                Phone = AppSettingsService.Get("Company.Phone", ""),
                Address = AppSettingsService.Get("Company.Address", ""),
                Email = AppSettingsService.Get("Company.Email", ""),
                LogoPath = AppSettingsService.Get("Company.LogoPath", ""),
                InvoiceTitle = AppSettingsService.Get("Invoice.Title", "فاتورة مياه"),
                InvoiceFooter = AppSettingsService.Get("Invoice.Footer", "شكراً لتعاملكم معنا"),
                Currency = AppSettingsService.Get("Invoice.Currency", "ريال")
            };
        }

        public static Image LoadLogo(string logoPath)
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
            }
            catch
            {
                // لا توقف الطباعة إذا لم يمكن تحميل الشعار.
            }

            return null;
        }

        public static string GetLogoFileUri(string logoPath)
        {
            if (string.IsNullOrWhiteSpace(logoPath))
                return string.Empty;

            if (!File.Exists(logoPath))
                return string.Empty;

            return "file:///" + logoPath.Replace("\\", "/");
        }
    }
}
