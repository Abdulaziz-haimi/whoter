using System;
using water3.Repositories;
using water3.Models;

namespace water3.Services
{
    public class ReadingService
    {
        private readonly ReadingsRepository _readingsRepo;

        public ReadingService(ReadingsRepository readingsRepo)
        {
            _readingsRepo = readingsRepo;
        }

        public decimal GetPreviousReading(int subscriberId, int meterId, DateTime asOfDate)
        {
            return _readingsRepo.GetPreviousReading(subscriberId, meterId, asOfDate);
        }

        public MeterReadingInfo GetLastReadingInfo(int meterId)
        {
            return _readingsRepo.GetLastReadingInfo(meterId);
        }

        public MeterInvoiceInfo GetLastInvoiceInfo(int meterId)
        {
            return _readingsRepo.GetLastInvoiceInfo(meterId);
        }

        public void AddReadingAndInvoice(AddReadingRequest req)
        {
            _readingsRepo.AddReadingAndGenerateInvoice(req);
        }
    }
}
/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using water3.Repositories;
using water3.Models;
namespace water3.Services
{
  

        public class ReadingService
        {
            private readonly ReadingsRepository _readingsRepo;

            public ReadingService(ReadingsRepository readingsRepo)
            {
                _readingsRepo = readingsRepo;
            }

            public decimal GetPreviousReadingSafe(int subscriberId, int meterId)
            {
                try { return _readingsRepo.GetPreviousReading(subscriberId, meterId); }
                catch { return 0m; }
            }

            public MeterReadingInfo GetLastReadingSafe(int meterId)
            {
                try { return _readingsRepo.GetLastReadingInfo(meterId); }
                catch { return new MeterReadingInfo(); }
            }

            public MeterInvoiceInfo GetLastInvoiceSafe(int meterId)
            {
                try { return _readingsRepo.GetLastInvoiceInfo(meterId); }
                catch { return new MeterInvoiceInfo(); }
            }

            public void AddReadingAndInvoice(AddReadingRequest req)
            {
                _readingsRepo.AddReadingAndGenerateInvoice(req);
            }
        }
    }
*/
