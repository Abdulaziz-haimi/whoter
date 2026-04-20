using System;
using System.Data;
using System.Threading.Tasks;
using water3.Repositories;

namespace water3.Services
{
    public class SmsQueueService
    {
        private readonly SmsLogsRepository _smsRepo;
        private readonly AndroidSmsGatewayClient _gateway;

        public SmsQueueService(SmsLogsRepository smsRepo, AndroidSmsGatewayClient gateway)
        {
            _smsRepo = smsRepo;
            _gateway = gateway;
        }

        public async Task<(int sent, int failed)> SendPendingAndFailedAsync(int maxRetry = 3, int delayMs = 200)
        {
            var dt = _smsRepo.GetPendingOrFailed(maxRetry);
            int sent = 0, failed = 0;

            // اختبار اتصال قبل الإرسال
            var (ok, raw) = await _gateway.HealthAsync();
            if (!ok) throw new InvalidOperationException("الهاتف غير متصل أو خدمة SMS Gateway متوقفة.\n" + raw);

            foreach (DataRow r in dt.Rows)
            {
                int smsId = Convert.ToInt32(r["SmsID"]);
                string phone = Convert.ToString(r["PhoneNumber"]);
                string msg = Convert.ToString(r["Message"]);

                if (string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(msg))
                {
                    _smsRepo.MarkFailed(smsId, "Invalid phone/message");
                    failed++;
                    continue;
                }

                var (sOk, status, resp, outId) = await _gateway.SendSmsAsync(phone, msg);

                if (sOk)
                {
                    _smsRepo.MarkSent(smsId);
                    sent++;
                }
                else
                {
                    _smsRepo.MarkFailed(smsId, resp ?? status ?? "Failed");
                    failed++;
                }

                await Task.Delay(delayMs);
            }

            return (sent, failed);
        }
    }
}
/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

using water3.Repositories;
using water3.Models;
using water3.Services;
namespace water3.Services
{
   

        public class SmsQueueService
        {
            private readonly SmsLogsRepository _repo;
            private readonly AndroidSmsGatewayClient _gateway;

            public SmsQueueService(SmsLogsRepository repo, AndroidSmsGatewayClient gateway)
            {
                _repo = repo;
                _gateway = gateway;
            }

            public string BuildMessage(SmsTemplateType type, string name, int invoiceId, decimal remaining, decimal paid, decimal total)
            {
                // عدل النصوص كما تريد
                switch (type)
                {
                    case SmsTemplateType.NewInvoice:
                        return $"عزيزي {name}، تم إصدار فاتورة مياه رقم {invoiceId}. الإجمالي: {total:N2} ريال. المتبقي: {remaining:N2} ريال.";
                    case SmsTemplateType.Payment:
                        return $"شكرًا {name}، تم تسجيل سداد للفاتورة رقم {invoiceId}. المدفوع: {paid:N2} ريال. المتبقي: {remaining:N2} ريال.";
                    case SmsTemplateType.Arrears:
                        return $"تنبيه {name}، لديك متأخرات على الفاتورة رقم {invoiceId}. المتبقي: {remaining:N2} ريال. يرجى السداد.";
                    default:
                        return $"عزيزي {name}، فاتورة رقم {invoiceId}. المتبقي: {remaining:N2} ريال.";
                }
            }

            public void EnqueueFromInvoices(DataTable invoices, IEnumerable<int> invoiceIds, SmsTemplateType type)
            {
                var idSet = new HashSet<int>(invoiceIds);

                foreach (DataRow r in invoices.Rows)
                {
                    int invId = Convert.ToInt32(r["رقم الفاتورة"]);
                    if (!idSet.Contains(invId)) continue;

                    int subscriberId = Convert.ToInt32(r["SubscriberID"]);
                    string name = Convert.ToString(r["اسم المشترك"]);
                    string phone = Convert.ToString(r["رقم الهاتف"]);

                    decimal total = Convert.ToDecimal(r["المبلغ الإجمالي"]);
                    decimal paid = Convert.ToDecimal(r["المدفوع"]);
                    decimal remain = Convert.ToDecimal(r["المتبقي"]);

                    if (string.IsNullOrWhiteSpace(phone)) continue;

                    string msg = BuildMessage(type, name, invId, remain, paid, total);
                    _repo.InsertPending(msg, phone, invId, subscriberId, (int)type);
                }
            }

            public async Task<(int sent, int failed)> SendPendingAsync(List<SmsQueueItem> pending)
            {
                // اختبار اتصال
                var (ok, raw) = await _gateway.HealthAsync();
                if (!ok) throw new InvalidOperationException("SMS Gateway غير متصل: " + raw);

                int sent = 0, failed = 0;

                foreach (var item in pending)
                {
                    try
                    {
                        var (sOk, status, resp, outId) = await _gateway.SendSmsAsync(item.PhoneNumber, item.Message);
                        if (sOk)
                        {
                            _repo.MarkSent(item.SmsID, $"outId={outId}; {status}");
                            sent++;
                        }
                        else
                        {
                            _repo.MarkFailed(item.SmsID, resp);
                            failed++;
                        }
                        await Task.Delay(250);
                    }
                    catch (Exception ex)
                    {
                        _repo.MarkFailed(item.SmsID, ex.Message);
                        failed++;
                    }
                }

                return (sent, failed);
            }
        }
    }
*/
