using System;
using System.Collections.Generic;
using System.Linq;

namespace water3.Utils
{
    public class SmsTemplateVariableInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string[] Types { get; set; } = Array.Empty<string>();

        public override string ToString()
        {
            return "{" + Name + "} - " + Description;
        }
    }

    public static class SmsTemplateVariables
    {
        private static readonly List<SmsTemplateVariableInfo> _all =
            new List<SmsTemplateVariableInfo>
            {
                // =========================
                // مشتركة
                // =========================
                new SmsTemplateVariableInfo { Name = "SubscriberName",      Description = "اسم المشترك", Types = new[] { "Invoice", "Payment", "Late" } },
                new SmsTemplateVariableInfo { Name = "PhoneNumber",         Description = "رقم الهاتف", Types = new[] { "Invoice", "Payment", "Late" } },

                new SmsTemplateVariableInfo { Name = "InvoiceID",           Description = "رقم الفاتورة الداخلي", Types = new[] { "Invoice", "Payment", "Late" } },
                new SmsTemplateVariableInfo { Name = "InvoiceNumber",       Description = "رقم الفاتورة / رقم المستند", Types = new[] { "Invoice", "Payment", "Late" } },

                new SmsTemplateVariableInfo { Name = "TotalAmount",         Description = "مبلغ الفترة الحالية فقط", Types = new[] { "Invoice", "Payment", "Late" } },
                new SmsTemplateVariableInfo { Name = "Arrears",             Description = "المتأخرات السابقة", Types = new[] { "Invoice", "Payment", "Late" } },
                new SmsTemplateVariableInfo { Name = "GrandTotal",          Description = "الإجمالي = مبلغ الفترة + المتأخرات", Types = new[] { "Invoice", "Payment", "Late" } },

                new SmsTemplateVariableInfo { Name = "PaidTotal",           Description = "إجمالي ما تم سداده على نفس الفاتورة", Types = new[] { "Invoice", "Payment", "Late" } },
                new SmsTemplateVariableInfo { Name = "Remaining",           Description = "المتبقي على نفس الفاتورة فقط", Types = new[] { "Invoice", "Payment", "Late" } },

                new SmsTemplateVariableInfo { Name = "RemainingBalance",    Description = "الرصيد الحالي الكلي للمشترك: موجب = عليه، سالب = له", Types = new[] { "Invoice", "Payment", "Late" } },
                new SmsTemplateVariableInfo { Name = "RemainingBalanceAbs", Description = "القيمة المطلقة للرصد الحالي بدون إشارة سالبة", Types = new[] { "Invoice", "Payment", "Late" } },
                new SmsTemplateVariableInfo { Name = "CurrentDue",          Description = "المبلغ الذي على المشترك الآن إن كان مدينًا", Types = new[] { "Invoice", "Payment", "Late" } },
                new SmsTemplateVariableInfo { Name = "CurrentCredit",       Description = "الرصيد المقدم المتبقي للمشترك إن كان له رصيد", Types = new[] { "Invoice", "Payment", "Late" } },
                new SmsTemplateVariableInfo { Name = "BalanceDirection",    Description = "اتجاه الرصيد الحالي: عليه / له / متوازن", Types = new[] { "Invoice", "Payment", "Late" } },

                new SmsTemplateVariableInfo { Name = "MeterID",             Description = "رقم العداد الداخلي", Types = new[] { "Invoice", "Payment", "Late" } },
                new SmsTemplateVariableInfo { Name = "MeterNumber",         Description = "رقم العداد", Types = new[] { "Invoice", "Payment", "Late" } },

                new SmsTemplateVariableInfo { Name = "InvoiceDate",         Description = "تاريخ الفاتورة", Types = new[] { "Invoice", "Payment", "Late" } },
                new SmsTemplateVariableInfo { Name = "Notes",               Description = "الملاحظات", Types = new[] { "Invoice", "Payment", "Late" } },
                new SmsTemplateVariableInfo { Name = "Today",               Description = "تاريخ اليوم", Types = new[] { "Invoice", "Payment", "Late" } },

                // =========================
                // الفاتورة فقط
                // =========================
                new SmsTemplateVariableInfo { Name = "Consumption",         Description = "الاستهلاك", Types = new[] { "Invoice" } },
                new SmsTemplateVariableInfo { Name = "CurrentReading",      Description = "القراءة الحالية", Types = new[] { "Invoice" } },
                new SmsTemplateVariableInfo { Name = "PreviousReading",     Description = "القراءة السابقة", Types = new[] { "Invoice" } },

                // =========================
                // السداد فقط
                // =========================
                new SmsTemplateVariableInfo { Name = "PaymentID",           Description = "رقم السداد الداخلي", Types = new[] { "Payment" } },
                new SmsTemplateVariableInfo { Name = "ReceiptID",           Description = "رقم الإيصال الداخلي", Types = new[] { "Payment" } },
                new SmsTemplateVariableInfo { Name = "ReceiptNumber",       Description = "رقم الإيصال", Types = new[] { "Payment" } },

                new SmsTemplateVariableInfo { Name = "Amount",              Description = "مبلغ سجل الدفع نفسه", Types = new[] { "Payment" } },
                new SmsTemplateVariableInfo { Name = "TotalReceived",       Description = "إجمالي المبلغ المستلم في الإيصال", Types = new[] { "Payment" } },
                new SmsTemplateVariableInfo { Name = "PaidToInvoices",      Description = "المبلغ الذي تم توزيعه على الفواتير من الإيصال", Types = new[] { "Payment" } },
                new SmsTemplateVariableInfo { Name = "CreditAmount",        Description = "المبلغ الذي تحول إلى رصيد مقدم من نفس الإيصال", Types = new[] { "Payment" } },

                new SmsTemplateVariableInfo { Name = "PaymentDate",         Description = "تاريخ السداد", Types = new[] { "Payment" } },
                new SmsTemplateVariableInfo { Name = "PaymentType",         Description = "طريقة السداد", Types = new[] { "Payment" } },
                new SmsTemplateVariableInfo { Name = "PaymentCategory",     Description = "فئة السداد: NormalPayment / AdvanceCredit / CreditSettlement", Types = new[] { "Payment" } },

                new SmsTemplateVariableInfo { Name = "InvoicesCount",       Description = "عدد الفواتير المتأثرة بهذا الإيصال", Types = new[] { "Payment" } },
                new SmsTemplateVariableInfo { Name = "InvoiceList",         Description = "قائمة أرقام الفواتير المتأثرة بالإيصال", Types = new[] { "Payment" } },

                // =========================
                // المتأخرات فقط
                // =========================
                new SmsTemplateVariableInfo { Name = "LateDays",            Description = "عدد أيام التأخير", Types = new[] { "Late" } }
            };

        public static IReadOnlyList<SmsTemplateVariableInfo> GetAll()
        {
            return _all;
        }

        public static IReadOnlyList<SmsTemplateVariableInfo> GetByType(string templateType)
        {
            if (string.IsNullOrWhiteSpace(templateType))
                return _all.OrderBy(x => x.Name).ToList();

            return _all
                .Where(x => x.Types.Any(t => string.Equals(t, templateType, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(x => x.Name)
                .ToList();
        }

        public static HashSet<string> GetNamesByType(string templateType)
        {
            return new HashSet<string>(
                GetByType(templateType).Select(x => x.Name),
                StringComparer.OrdinalIgnoreCase);
        }

        public static Dictionary<string, string> CreateSampleValues(string templateType)
        {
            var type = (templateType ?? string.Empty).Trim();

            var data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["SubscriberName"] = "أحمد محمد",
                ["PhoneNumber"] = "777123456",

                ["InvoiceID"] = "6253",
                ["InvoiceNumber"] = "INV-2026-006253",

                ["PaymentID"] = "4158",
                ["ReceiptID"] = "1142",
                ["ReceiptNumber"] = "REC-2026-001142",

                ["TotalAmount"] = "1600.00",
                ["Arrears"] = "900.00",
                ["GrandTotal"] = "2500.00",

                ["Amount"] = "1600.00",
                ["TotalReceived"] = "2000.00",
                ["PaidToInvoices"] = "1600.00",
                ["CreditAmount"] = "400.00",

                ["PaidTotal"] = "1500.00",
                ["Remaining"] = "1000.00",

                ["RemainingBalance"] = "1000.00",
                ["RemainingBalanceAbs"] = "1000.00",
                ["CurrentDue"] = "1000.00",
                ["CurrentCredit"] = "0.00",
                ["BalanceDirection"] = "عليه",

                ["Consumption"] = "2.00",
                ["CurrentReading"] = "7.00",
                ["PreviousReading"] = "5.00",

                ["MeterID"] = "12",
                ["MeterNumber"] = "MTR-0012",

                ["InvoiceDate"] = DateTime.Today.ToString("yyyy-MM-dd"),
                ["PaymentDate"] = DateTime.Today.ToString("yyyy-MM-dd"),

                ["PaymentType"] = "Cash",
                ["PaymentCategory"] = "NormalPayment",

                ["Notes"] = "مثال تجريبي",
                ["LateDays"] = "15",

                ["InvoicesCount"] = "2",
                ["InvoiceList"] = "6252,6253",
                ["Today"] = DateTime.Today.ToString("yyyy-MM-dd")
            };

            // تحسين عينات المعاينة حسب نوع القالب
            if (type.Equals("Invoice", StringComparison.OrdinalIgnoreCase))
            {
                data["Amount"] = "0.00";
                data["TotalReceived"] = "0.00";
                data["PaidToInvoices"] = "0.00";
                data["CreditAmount"] = "0.00";
                data["PaymentType"] = "";
                data["PaymentCategory"] = "";
                data["PaymentDate"] = "";
                data["ReceiptID"] = "";
                data["ReceiptNumber"] = "";
                data["PaymentID"] = "";
                data["InvoicesCount"] = "0";
                data["InvoiceList"] = "";
            }
            else if (type.Equals("Payment", StringComparison.OrdinalIgnoreCase))
            {
                data["Consumption"] = "0.00";
                data["CurrentReading"] = "0.00";
                data["PreviousReading"] = "0.00";
                data["LateDays"] = "0";

                // مثال سداد نتج عنه رصيد مقدم
                data["RemainingBalance"] = "-400.00";
                data["RemainingBalanceAbs"] = "400.00";
                data["CurrentDue"] = "0.00";
                data["CurrentCredit"] = "400.00";
                data["BalanceDirection"] = "له";
            }
            else if (type.Equals("Late", StringComparison.OrdinalIgnoreCase))
            {
                data["Consumption"] = "0.00";
                data["CurrentReading"] = "0.00";
                data["PreviousReading"] = "0.00";

                data["Amount"] = "0.00";
                data["TotalReceived"] = "0.00";
                data["PaidToInvoices"] = "0.00";
                data["CreditAmount"] = "0.00";
                data["PaymentType"] = "";
                data["PaymentCategory"] = "";
                data["PaymentDate"] = "";
                data["ReceiptID"] = "";
                data["ReceiptNumber"] = "";
                data["PaymentID"] = "";
                data["InvoicesCount"] = "0";
                data["InvoiceList"] = "";

                data["RemainingBalance"] = "2500.00";
                data["RemainingBalanceAbs"] = "2500.00";
                data["CurrentDue"] = "2500.00";
                data["CurrentCredit"] = "0.00";
                data["BalanceDirection"] = "عليه";
            }

            return data;
        }
    }
}