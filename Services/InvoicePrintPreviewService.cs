using System;
using System.Data;
using water3.Repositories;

namespace water3.Services
{
    public class InvoicePrintPreviewService
    {
        private readonly SmsLogsRepository _smsRepo;

        public InvoicePrintPreviewService(SmsLogsRepository smsRepo)
        {
            _smsRepo = smsRepo;
        }

        public DataTable GetPendingInvoiceMessages(DateTime from, DateTime toExclusive, int subscriberId, string searchLike, string invoiceStatus = null)
            => _smsRepo.GetPendingInvoiceMessages(from, toExclusive, subscriberId, searchLike, invoiceStatus);

        public DataTable GetPendingPaymentMessages(DateTime from, DateTime toExclusive, int subscriberId, string searchLike, string paymentMethodDb = null)
            => _smsRepo.GetPendingPaymentMessages(from, toExclusive, subscriberId, searchLike, paymentMethodDb);

        public DataTable GetSmsLogs(DateTime from, DateTime toExclusive, int subscriberId, string searchLike)
            => _smsRepo.GetLogs(from, toExclusive, subscriberId, searchLike);

        public void MarkSent(int smsId) => _smsRepo.MarkSent(smsId);

        public void MarkFailed(int smsId, string reason) => _smsRepo.MarkFailed(smsId, reason);
    }
}
/*using System;
using System.Collections.Generic;
using System.Data;
using water3.Models;
using water3.Repositories;
using water3.Utils;

namespace water3.Services
{
    public class InvoicePrintPreviewService
    {
        private readonly InvoiceRepository _invRepo;
        private readonly MessageTemplatesRepository _tplRepo;
        private readonly SmsLogsRepository _smsRepo;

        public InvoicePrintPreviewService(
            InvoiceRepository invRepo,
            MessageTemplatesRepository tplRepo,
            SmsLogsRepository smsRepo)
        {
            _invRepo = invRepo;
            _tplRepo = tplRepo;
            _smsRepo = smsRepo;
        }

        public DataTable GetInvoices(InvoiceFilters f) => _invRepo.GetInvoicesTable(f);

        public DataTable GetSmsLogs(InvoiceFilters f) =>
            _smsRepo.GetLogs(f.From, f.ToExclusive, f.SubscriberId, f.SearchLike);

        public List<MessageTemplate> GetActiveTemplates(string lang = null) =>
            _tplRepo.GetActiveTemplates(lang);

        public int QueueMessagesFromInvoices(DataTable invoices, IEnumerable<int> invoiceIds, int templateId)
        {
            var tpl = _tplRepo.GetById(templateId);
            if (tpl == null) throw new InvalidOperationException("القالب غير موجود");

            var idSet = new HashSet<int>(invoiceIds);
            int queued = 0;

            foreach (DataRow r in invoices.Rows)
            {
                int invId = Convert.ToInt32(r["رقم الفاتورة"]);
                if (!idSet.Contains(invId)) continue;

                int subscriberId = Convert.ToInt32(r["SubscriberID"]);
                string name = Convert.ToString(r["اسم المشترك"]);
                string phone = Convert.ToString(r["رقم الهاتف"]);

                decimal total = ToDec(r["المبلغ الإجمالي"]);
                decimal paid = ToDec(r["المدفوع"]);
                decimal remain = ToDec(r["المتبقي"]);
                decimal arrears = r.Table.Columns.Contains("المتأخرات") ? ToDec(r["المتأخرات"]) : 0m;

                string invDate = Convert.ToString(r["تاريخ الفاتورة"]);

                if (string.IsNullOrWhiteSpace(phone)) continue;

                // منع التكرار لنفس Invoice + Template طالما Pending/Failed
                if (_smsRepo.ExistsPendingForInvoiceTemplate(invId, templateId)) continue;

                var tokens = new Dictionary<string, string>
                {
                    ["SubscriberName"] = name ?? "",
                    ["InvoiceID"] = invId.ToString(),
                    ["TotalAmount"] = total.ToString("N2"),
                    ["PaidAmount"] = paid.ToString("N2"),
                    ["RemainingAmount"] = remain.ToString("N2"),
                    ["Arrears"] = arrears.ToString("N2"),
                    ["InvoiceDate"] = invDate ?? ""
                };

                string msg = TemplateEngine.Render(tpl.TemplateText, tokens);
                if (string.IsNullOrWhiteSpace(msg)) continue;

                _smsRepo.InsertPending(invId, subscriberId, phone, msg, templateId, collectorId: null);
                queued++;
            }

            return queued;
        }

        private decimal ToDec(object o)
        {
            if (o == null || o == DBNull.Value) return 0m;
            decimal.TryParse(o.ToString(), out var x);
            return x;
        }
    }
}
*/