using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using water3.Models;
namespace water3.Repositories
{
 
        public class ExpenseRepository
        {
            public List<ExpenseCategoryItem> GetCategories()
            {
                var list = new List<ExpenseCategoryItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.usp_ExpenseCategories_GetAll", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new ExpenseCategoryItem
                            {
                                CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                CategoryName = Convert.ToString(dr["CategoryName"]),
                                CategoryType = Convert.ToString(dr["CategoryType"]),
                                DefaultAccountID = dr["DefaultAccountID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["DefaultAccountID"]),
                                Notes = dr["Notes"] == DBNull.Value ? null : Convert.ToString(dr["Notes"]),
                                IsActive = Convert.ToBoolean(dr["IsActive"])
                            });
                        }
                    }
                }

                return list;
            }

            public List<ExpenseHeaderItem> GetExpenses(DateTime? fromDate, DateTime? toDate, int? categoryId, string categoryType)
            {
                var list = new List<ExpenseHeaderItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.usp_Expenses_GetAll", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.HasValue ? (object)fromDate.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ToDate", toDate.HasValue ? (object)toDate.Value.Date : DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryID", categoryId.HasValue ? (object)categoryId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@CategoryType", string.IsNullOrWhiteSpace(categoryType) ? (object)DBNull.Value : categoryType);

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new ExpenseHeaderItem
                            {
                                ExpenseID = Convert.ToInt32(dr["ExpenseID"]),
                                ExpenseNumber = Convert.ToString(dr["ExpenseNumber"]),
                                ExpenseDate = Convert.ToDateTime(dr["ExpenseDate"]),
                                CategoryName = Convert.ToString(dr["CategoryName"]),
                                CategoryType = Convert.ToString(dr["CategoryType"]),
                                SupplierName = dr["SupplierName"] == DBNull.Value ? null : Convert.ToString(dr["SupplierName"]),
                                Description = dr["Description"] == DBNull.Value ? null : Convert.ToString(dr["Description"]),
                                TotalAmount = Convert.ToDecimal(dr["TotalAmount"]),
                                PaymentMethod = Convert.ToString(dr["PaymentMethod"]),
                                IsPosted = Convert.ToBoolean(dr["IsPosted"]),
                                Status = Convert.ToString(dr["Status"])
                            });
                        }
                    }
                }

                return list;
            }

            public ExpenseHeaderItem GetExpenseHeader(int expenseId)
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.usp_Expense_GetDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExpenseID", expenseId);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new ExpenseHeaderItem
                            {
                                ExpenseID = Convert.ToInt32(dr["ExpenseID"]),
                                ExpenseNumber = Convert.ToString(dr["ExpenseNumber"]),
                                ExpenseDate = Convert.ToDateTime(dr["ExpenseDate"]),
                                CategoryID = Convert.ToInt32(dr["CategoryID"]),
                                CategoryName = Convert.ToString(dr["CategoryName"]),
                                CategoryType = Convert.ToString(dr["CategoryType"]),
                                SupplierName = dr["SupplierName"] == DBNull.Value ? null : Convert.ToString(dr["SupplierName"]),
                                Description = dr["Description"] == DBNull.Value ? null : Convert.ToString(dr["Description"]),
                                Notes = dr["Notes"] == DBNull.Value ? null : Convert.ToString(dr["Notes"]),
                                TotalAmount = Convert.ToDecimal(dr["TotalAmount"]),
                                PaymentMethod = Convert.ToString(dr["PaymentMethod"]),
                                CashAccountID = dr["CashAccountID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["CashAccountID"]),
                                CounterAccountID = dr["CounterAccountID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["CounterAccountID"]),
                                JournalID = dr["JournalID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["JournalID"]),
                                IsPosted = Convert.ToBoolean(dr["IsPosted"]),
                                Status = Convert.ToString(dr["Status"])
                            };
                        }
                    }
                }

                return null;
            }

            public List<ExpenseLineItem> GetExpenseLines(int expenseId)
            {
                var list = new List<ExpenseLineItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.usp_Expense_GetDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExpenseID", expenseId);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.NextResult())
                        {
                            while (dr.Read())
                            {
                                list.Add(new ExpenseLineItem
                                {
                                    ItemName = Convert.ToString(dr["ItemName"]),
                                    Qty = Convert.ToDecimal(dr["Qty"]),
                                    UnitPrice = Convert.ToDecimal(dr["UnitPrice"]),
                                    TargetAccountID = dr["TargetAccountID"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["TargetAccountID"]),
                                    Notes = dr["Notes"] == DBNull.Value ? null : Convert.ToString(dr["Notes"])
                                });
                            }
                        }
                    }
                }

                return list;
            }

            public ExpenseSaveResult SaveExpense(
                ExpenseHeaderItem header,
                List<ExpenseLineItem> lines,
                int? createdBy)
            {
                if (header == null)
                    throw new ArgumentNullException(nameof(header));

                DataTable tvp = new DataTable();
                tvp.Columns.Add("ItemName", typeof(string));
                tvp.Columns.Add("Qty", typeof(decimal));
                tvp.Columns.Add("UnitPrice", typeof(decimal));
                tvp.Columns.Add("TargetAccountID", typeof(int));
                tvp.Columns.Add("Notes", typeof(string));

                if (lines != null)
                {
                    foreach (var line in lines)
                    {
                        tvp.Rows.Add(
                            line.ItemName,
                            line.Qty,
                            line.UnitPrice,
                            line.TargetAccountID.HasValue ? (object)line.TargetAccountID.Value : DBNull.Value,
                            string.IsNullOrWhiteSpace(line.Notes) ? (object)DBNull.Value : line.Notes.Trim());
                    }
                }

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.usp_Expense_Save", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter expenseIdParam = new SqlParameter("@ExpenseID", SqlDbType.Int)
                    {
                        Direction = ParameterDirection.InputOutput,
                        Value = header.ExpenseID > 0 ? (object)header.ExpenseID : DBNull.Value
                    };
                    cmd.Parameters.Add(expenseIdParam);

                    cmd.Parameters.AddWithValue("@ExpenseDate", header.ExpenseDate.Date);
                    cmd.Parameters.AddWithValue("@CategoryID", header.CategoryID);
                    cmd.Parameters.AddWithValue("@SupplierName", string.IsNullOrWhiteSpace(header.SupplierName) ? (object)DBNull.Value : header.SupplierName.Trim());
                    cmd.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(header.Description) ? (object)DBNull.Value : header.Description.Trim());
                    cmd.Parameters.AddWithValue("@Notes", string.IsNullOrWhiteSpace(header.Notes) ? (object)DBNull.Value : header.Notes.Trim());
                    cmd.Parameters.AddWithValue("@PaymentMethod", header.PaymentMethod);
                    cmd.Parameters.AddWithValue("@CashAccountID", header.CashAccountID.HasValue ? (object)header.CashAccountID.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@CounterAccountID", header.CounterAccountID.HasValue ? (object)header.CounterAccountID.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy.HasValue ? (object)createdBy.Value : DBNull.Value);

                    SqlParameter linesParam = cmd.Parameters.AddWithValue("@Lines", tvp);
                    linesParam.SqlDbType = SqlDbType.Structured;
                    linesParam.TypeName = "dbo.ExpenseLineType";

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new ExpenseSaveResult
                            {
                                ExpenseID = Convert.ToInt32(dr["ExpenseID"]),
                                ExpenseNumber = Convert.ToString(dr["ExpenseNumber"]),
                                TotalAmount = Convert.ToDecimal(dr["TotalAmount"])
                            };
                        }
                    }
                }

                return null;
        }
    
public ExpenseSaveResult UpdateExpense(ExpenseHeaderItem header, List<ExpenseLineItem> lines)
        {
            if (header == null)
                throw new ArgumentNullException(nameof(header));

            DataTable tvp = new DataTable();
            tvp.Columns.Add("ItemName", typeof(string));
            tvp.Columns.Add("Qty", typeof(decimal));
            tvp.Columns.Add("UnitPrice", typeof(decimal));
            tvp.Columns.Add("TargetAccountID", typeof(int));
            tvp.Columns.Add("Notes", typeof(string));

            if (lines != null)
            {
                foreach (var line in lines)
                {
                    tvp.Rows.Add(
                        line.ItemName,
                        line.Qty,
                        line.UnitPrice,
                        line.TargetAccountID.HasValue ? (object)line.TargetAccountID.Value : DBNull.Value,
                        string.IsNullOrWhiteSpace(line.Notes) ? (object)DBNull.Value : line.Notes.Trim());
                }
            }

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("dbo.usp_Expense_Update", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ExpenseID", header.ExpenseID);
                cmd.Parameters.AddWithValue("@ExpenseDate", header.ExpenseDate.Date);
                cmd.Parameters.AddWithValue("@CategoryID", header.CategoryID);
                cmd.Parameters.AddWithValue("@SupplierName", string.IsNullOrWhiteSpace(header.SupplierName) ? (object)DBNull.Value : header.SupplierName.Trim());
                cmd.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(header.Description) ? (object)DBNull.Value : header.Description.Trim());
                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrWhiteSpace(header.Notes) ? (object)DBNull.Value : header.Notes.Trim());
                cmd.Parameters.AddWithValue("@PaymentMethod", header.PaymentMethod);
                cmd.Parameters.AddWithValue("@CashAccountID", header.CashAccountID.HasValue ? (object)header.CashAccountID.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@CounterAccountID", header.CounterAccountID.HasValue ? (object)header.CounterAccountID.Value : DBNull.Value);

                SqlParameter linesParam = cmd.Parameters.AddWithValue("@Lines", tvp);
                linesParam.SqlDbType = SqlDbType.Structured;
                linesParam.TypeName = "dbo.ExpenseLineType";

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new ExpenseSaveResult
                        {
                            ExpenseID = Convert.ToInt32(dr["ExpenseID"]),
                            ExpenseNumber = Convert.ToString(dr["ExpenseNumber"]),
                            TotalAmount = Convert.ToDecimal(dr["TotalAmount"])
                        };
                    }
                }
            }

            return null;
        }

        public void DeleteExpense(int expenseId)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("dbo.usp_Expense_Delete", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@ExpenseID", expenseId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
  
        public List<AccountLookupItem> GetExpenseAccounts()
        {
            var list = new List<AccountLookupItem>();

            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand(@"
        SELECT AccountID, AccountCode, AccountName
        FROM dbo.Accounts
        ORDER BY AccountCode, AccountName", con))
            {
                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        list.Add(new AccountLookupItem
                        {
                            AccountID = Convert.ToInt32(dr["AccountID"]),
                            AccountCode = Convert.ToString(dr["AccountCode"]),
                            AccountName = Convert.ToString(dr["AccountName"])
                        });
                    }
                }
            }

            return list;
        }

        public int SaveCategory(ExpenseCategoryItem item)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("dbo.usp_ExpenseCategory_Save", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CategoryID", item.CategoryID > 0 ? (object)item.CategoryID : DBNull.Value);
                cmd.Parameters.AddWithValue("@CategoryName", item.CategoryName);
                cmd.Parameters.AddWithValue("@CategoryType", item.CategoryType);
                cmd.Parameters.AddWithValue("@DefaultAccountID", item.DefaultAccountID.HasValue ? (object)item.DefaultAccountID.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@Notes", string.IsNullOrWhiteSpace(item.Notes) ? (object)DBNull.Value : item.Notes.Trim());
                cmd.Parameters.AddWithValue("@IsActive", item.IsActive);

                con.Open();
                object result = cmd.ExecuteScalar();
                return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
        }

        public void DeleteCategory(int categoryId)
        {
            using (SqlConnection con = Db.GetConnection())
            using (SqlCommand cmd = new SqlCommand("dbo.usp_ExpenseCategory_Delete", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CategoryID", categoryId);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }
       
    }
}