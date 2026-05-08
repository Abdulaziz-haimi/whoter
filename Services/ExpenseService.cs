using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using water3.Models;
using water3.Repositories;
namespace water3.Services
{
  
        public class ExpenseService
        {
            private readonly ExpenseRepository _repo = new ExpenseRepository();
            private readonly AuditLogService _audit = new AuditLogService();

            public List<ExpenseCategoryItem> GetCategories()
            {
                return _repo.GetCategories().Where(x => x.IsActive).ToList();
            }

            public List<ExpenseHeaderItem> GetExpenses(DateTime? fromDate, DateTime? toDate, int? categoryId, string categoryType)
            {
                return _repo.GetExpenses(fromDate, toDate, categoryId, categoryType);
            }

            public ExpenseHeaderItem GetExpenseHeader(int expenseId)
            {
                return _repo.GetExpenseHeader(expenseId);
            }

            public List<ExpenseLineItem> GetExpenseLines(int expenseId)
            {
                return _repo.GetExpenseLines(expenseId);
            }

            public ExpenseSaveResult SaveExpense(ExpenseHeaderItem header, List<ExpenseLineItem> lines)
            {
                Validate(header, lines);

                int? createdBy = CurrentUser.IsLoggedIn ? (int?)CurrentUser.UserID : null;
                ExpenseSaveResult result = _repo.SaveExpense(header, lines, createdBy);

                if (result == null)
                    throw new InvalidOperationException("تعذر حفظ الحركة.");

                _audit.Log(
                    action: "SAVE_EXPENSE",
                    tableName: "Expenses",
                    recordId: result.ExpenseID,
                    details: $"تم حفظ حركة مصروف/شراء رقم {result.ExpenseNumber} بمبلغ {result.TotalAmount:N2}",
                    entityName: result.ExpenseNumber);

                return result;
            }

        private void Validate(ExpenseHeaderItem header, List<ExpenseLineItem> lines)
        {
            if (header == null)
                throw new InvalidOperationException("بيانات الحركة غير متوفرة.");

            if (header.ExpenseDate == default(DateTime))
                throw new InvalidOperationException("تاريخ الحركة مطلوب.");

            if (header.CategoryID <= 0)
                throw new InvalidOperationException("اختر التصنيف.");

            if (string.IsNullOrWhiteSpace(header.PaymentMethod))
                throw new InvalidOperationException("اختر طريقة الدفع.");

            if (header.PaymentMethod != "Cash" &&
                header.PaymentMethod != "Transfer" &&
                header.PaymentMethod != "Cheque" &&
                header.PaymentMethod != "Credit")
            {
                throw new InvalidOperationException("طريقة الدفع غير صحيحة.");
            }

            if (header.CashAccountID.HasValue && header.CashAccountID.Value <= 0)
                header.CashAccountID = null;

            if (header.CounterAccountID.HasValue && header.CounterAccountID.Value <= 0)
                header.CounterAccountID = null;

            if (lines == null || lines.Count == 0)
                throw new InvalidOperationException("أدخل بندًا واحدًا على الأقل.");

            foreach (ExpenseLineItem line in lines)
            {
                if (line == null)
                    throw new InvalidOperationException("يوجد بند غير صحيح.");

                if (string.IsNullOrWhiteSpace(line.ItemName))
                    throw new InvalidOperationException("اسم البند مطلوب.");

                if (line.Qty <= 0)
                    throw new InvalidOperationException("الكمية يجب أن تكون أكبر من صفر.");

                if (line.UnitPrice < 0)
                    throw new InvalidOperationException("سعر الوحدة غير صحيح.");

                if (line.TargetAccountID.HasValue && line.TargetAccountID.Value <= 0)
                    line.TargetAccountID = null;
            }
        }
        public ExpenseSaveResult UpdateExpense(ExpenseHeaderItem header, List<ExpenseLineItem> lines)
        {
            Validate(header, lines);

            ExpenseSaveResult result = _repo.UpdateExpense(header, lines);

            if (result == null)
                throw new InvalidOperationException("تعذر تعديل الحركة.");

            _audit.Log(
                action: "UPDATE_EXPENSE",
                tableName: "Expenses",
                recordId: result.ExpenseID,
                details: $"تم تعديل حركة رقم {result.ExpenseNumber} بمبلغ {result.TotalAmount:N2}",
                entityName: result.ExpenseNumber);

            return result;
        }

        public void DeleteExpense(int expenseId)
        {
            if (expenseId <= 0)
                throw new InvalidOperationException("معرف الحركة غير صحيح.");

            _repo.DeleteExpense(expenseId);

            _audit.Log(
                action: "DELETE_EXPENSE",
                tableName: "Expenses",
                recordId: expenseId,
                details: "تم حذف حركة مصروف/شراء/خسارة",
                entityName: expenseId.ToString());
        }
      
        public List<AccountLookupItem> GetAccounts()
        {
            return _repo.GetExpenseAccounts();
        }

        public int SaveCategory(ExpenseCategoryItem item)
        {
            if (item == null)
                throw new InvalidOperationException("بيانات التصنيف غير متوفرة.");

            if (string.IsNullOrWhiteSpace(item.CategoryName))
                throw new InvalidOperationException("اسم التصنيف مطلوب.");

            if (string.IsNullOrWhiteSpace(item.CategoryType))
                throw new InvalidOperationException("نوع التصنيف مطلوب.");

            int categoryId = _repo.SaveCategory(item);

            _audit.Log(
                action: item.CategoryID > 0 ? "UPDATE_EXPENSE_CATEGORY" : "CREATE_EXPENSE_CATEGORY",
                tableName: "ExpenseCategories",
                recordId: categoryId,
                details: $"تم حفظ تصنيف {item.CategoryName} من النوع {item.CategoryType}",
                entityName: item.CategoryName);

            return categoryId;
        }

        public void DeleteCategory(int categoryId)
        {
            if (categoryId <= 0)
                throw new InvalidOperationException("معرف التصنيف غير صحيح.");

            _repo.DeleteCategory(categoryId);

            _audit.Log(
                action: "DELETE_EXPENSE_CATEGORY",
                tableName: "ExpenseCategories",
                recordId: categoryId,
                details: "تم حذف تصنيف مصروف/مشتريات/خسائر",
                entityName: categoryId.ToString());
        }

    }
}