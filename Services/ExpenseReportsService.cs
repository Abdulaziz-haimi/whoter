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
  
        public class ExpenseReportsService
        {
            private readonly ExpenseReportsRepository _repo = new ExpenseReportsRepository();
            private readonly AuditLogService _audit = new AuditLogService();

            public List<ExpenseReportRow> GetExpenseReport(DateTime fromDate, DateTime toDate, string categoryType, int? categoryId)
            {
                return _repo.GetExpenseRows(fromDate, toDate, categoryType, categoryId);
            }

            public List<ExpenseSummaryRow> GetCategorySummary(DateTime fromDate, DateTime toDate, string categoryType)
            {
                return _repo.GetSummaryRows(fromDate, toDate, categoryType);
            }

            public ExpenseVoucherHeaderRow GetVoucherHeader(int expenseId)
            {
                return _repo.GetVoucherHeader(expenseId);
            }

            public List<ExpenseVoucherLineRow> GetVoucherLines(int expenseId)
            {
                return _repo.GetVoucherLines(expenseId);
            }

            public void LogReportOpen(string reportKey, string details = null)
            {
                _audit.Log(
                    action: "OPEN_EXPENSE_REPORT",
                    tableName: "Expenses",
                    recordId: null,
                    details: details ?? reportKey,
                    entityName: reportKey);
            }
        }
    }