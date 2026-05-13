using System;
using System.Collections.Generic;
using water3.Models;
using water3.Repositories;

namespace water3.Services
{
    public class RevenueExpenseStatementService
    {
        private readonly RevenueExpenseStatementRepository _repo =
            new RevenueExpenseStatementRepository();

        private readonly ExpenseRepository _expenseRepo =
            new ExpenseRepository();

        public RevenueExpenseStatementResult GetStatement(RevenueExpenseStatementFilter filter)
        {
            if (filter == null)
                throw new ArgumentNullException("filter");

            if (filter.FromDate.Date > filter.ToDate.Date)
                throw new InvalidOperationException("تاريخ البداية يجب أن يكون قبل تاريخ النهاية.");

            return _repo.GetStatement(filter);
        }

        public List<ExpenseCategoryItem> GetExpenseCategories()
        {
            return _expenseRepo.GetCategories();
        }
    }
}