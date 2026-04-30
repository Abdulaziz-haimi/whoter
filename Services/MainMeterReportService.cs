using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using water3.Models;
using water3.Repositories;
namespace water3.Services
{

        public class MainMeterReportService
        {
            private readonly MainMeterReportRepository _repo = new MainMeterReportRepository();
            private readonly AuditLogService _audit = new AuditLogService();

            public List<MainMeterLookupItem> GetMainMeters()
            {
                return _repo.GetMainMeters();
            }

            public decimal GetLastReading(int mainMeterId)
            {
                if (mainMeterId <= 0)
                    throw new InvalidOperationException("اختر العداد الرئيسي أولًا.");

                return _repo.GetLastMainMeterReading(mainMeterId);
            }

            public MainMeterReportResult SaveCycle(
                int mainMeterId,
                DateTime reportDate,
                decimal currentReading,
                string notes,
                decimal? totalFromQasem)
            {
                if (mainMeterId <= 0)
                    throw new InvalidOperationException("اختر العداد الرئيسي أولًا.");

                if (currentReading < 0)
                    throw new InvalidOperationException("القراءة الحالية غير صحيحة.");

                var result = _repo.SaveMainMeterReadingAndGenerateReport(
                    mainMeterId,
                    reportDate,
                    currentReading,
                    notes,
                    totalFromQasem);

                if (result == null)
                    throw new InvalidOperationException("تعذر إنشاء التقرير.");

                _audit.Log(
                    action: "SAVE_MAIN_METER_REPORT",
                    tableName: "MeterReports",
                    recordId: null,
                    details: $"تم إنشاء تقرير عداد رئيسي رقم {mainMeterId} بتاريخ {reportDate:yyyy-MM-dd} والفاقد = {result.WaterLoss}",
                    entityName: "MainMeterReport");

                return result;
            }

            public List<SubMeterReadingSummaryItem> GetSubMeterSummary(DateTime reportDate)
            {
                return _repo.GetSubMeterReadingsByDate(reportDate);
            }
        }
    }