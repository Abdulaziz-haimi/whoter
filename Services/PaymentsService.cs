using System;
using water3.Repositories;

namespace water3.Services
{
    public class PaymentsService
    {
        private readonly PaymentsRepository _paymentsRepo;

        public PaymentsService(PaymentsRepository paymentsRepo)
        {
            _paymentsRepo = paymentsRepo;
        }

        public void AddPayment(int subscriberId, int collectorId, DateTime paymentDate, decimal amount, string paymentTypeDb, string notes)
        {
            if (paymentDate.Date > DateTime.Today)
                throw new InvalidOperationException("لا يمكن التسجيل بتاريخ مستقبلي");

            if (_paymentsRepo.IsPeriodClosed(paymentDate))
                throw new InvalidOperationException("لا يمكن التسجيل: الفترة المحاسبية مغلقة");

            _paymentsRepo.PayOldestInvoice(subscriberId, collectorId, paymentDate, amount, paymentTypeDb, notes);
        }

        internal void AddPayment(object payment)
        {
            throw new NotImplementedException();
        }
    }
}