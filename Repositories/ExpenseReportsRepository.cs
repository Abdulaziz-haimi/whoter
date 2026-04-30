using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using water3.Models;
namespace water3.Repositories
{

        public class ExpenseReportsRepository
        {
            public List<ExpenseReportRow> GetExpenseRows(DateTime fromDate, DateTime toDate, string categoryType, int? categoryId)
            {
                var list = new List<ExpenseReportRow>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.rpt_Expenses_Generic", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Date);
                    cmd.Parameters.AddWithValue("@CategoryType", string.IsNullOrWhiteSpace(categoryType) ? (object)DBNull.Value : categoryType);
                    cmd.Parameters.AddWithValue("@CategoryID", categoryId.HasValue ? (object)categoryId.Value : DBNull.Value);

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new ExpenseReportRow
                            {
                                Dt1 = Convert.ToDateTime(dr["Dt1"]),
                                Col1 = Convert.ToString(dr["Col1"]),
                                Col2 = Convert.ToString(dr["Col2"]),
                                Col3 = Convert.ToString(dr["Col3"]),
                                Col4 = Convert.ToString(dr["Col4"]),
                                Col5 = Convert.ToString(dr["Col5"]),
                                Num1 = Convert.ToDecimal(dr["Num1"]),
                                RowId = Convert.ToInt32(dr["RowId"]),
                                RefId1 = dr["RefId1"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["RefId1"]),
                                RefId2 = dr["RefId2"] == DBNull.Value ? (int?)null : Convert.ToInt32(dr["RefId2"])
                            });
                        }
                    }
                }

                return list;
            }

            public List<ExpenseSummaryRow> GetSummaryRows(DateTime fromDate, DateTime toDate, string categoryType)
            {
                var list = new List<ExpenseSummaryRow>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.rpt_ExpenseSummary_ByCategory", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FromDate", fromDate.Date);
                    cmd.Parameters.AddWithValue("@ToDate", toDate.Date);
                    cmd.Parameters.AddWithValue("@CategoryType", string.IsNullOrWhiteSpace(categoryType) ? (object)DBNull.Value : categoryType);

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new ExpenseSummaryRow
                            {
                                Col1 = Convert.ToString(dr["Col1"]),
                                Col2 = Convert.ToString(dr["Col2"]),
                                Num1 = Convert.ToDecimal(dr["Num1"]),
                                Num2 = Convert.ToDecimal(dr["Num2"]),
                                RowId = Convert.ToInt32(dr["RowId"])
                            });
                        }
                    }
                }

                return list;
            }

            public ExpenseVoucherHeaderRow GetVoucherHeader(int expenseId)
            {
                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.rpt_ExpenseVoucher", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@ExpenseID", expenseId);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            return new ExpenseVoucherHeaderRow
                            {
                                ExpenseID = Convert.ToInt32(dr["ExpenseID"]),
                                ExpenseNumber = Convert.ToString(dr["ExpenseNumber"]),
                                ExpenseDate = Convert.ToDateTime(dr["ExpenseDate"]),
                                CategoryName = Convert.ToString(dr["CategoryName"]),
                                CategoryType = Convert.ToString(dr["CategoryType"]),
                                SupplierName = Convert.ToString(dr["SupplierName"]),
                                Description = Convert.ToString(dr["Description"]),
                                Notes = Convert.ToString(dr["Notes"]),
                                TotalAmount = Convert.ToDecimal(dr["TotalAmount"]),
                                PaymentMethod = Convert.ToString(dr["PaymentMethod"]),
                                IsPosted = Convert.ToBoolean(dr["IsPosted"]),
                                Status = Convert.ToString(dr["Status"]),
                                CreatedByName = Convert.ToString(dr["CreatedByName"])
                            };
                        }
                    }
                }

                return null;
            }

            public List<ExpenseVoucherLineRow> GetVoucherLines(int expenseId)
            {
                var list = new List<ExpenseVoucherLineRow>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.rpt_ExpenseVoucher", con))
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
                                list.Add(new ExpenseVoucherLineRow
                                {
                                    ExpenseLineID = Convert.ToInt32(dr["ExpenseLineID"]),
                                    ItemName = Convert.ToString(dr["ItemName"]),
                                    Qty = Convert.ToDecimal(dr["Qty"]),
                                    UnitPrice = Convert.ToDecimal(dr["UnitPrice"]),
                                    LineTotal = Convert.ToDecimal(dr["LineTotal"]),
                                    TargetAccountName = Convert.ToString(dr["TargetAccountName"]),
                                    Notes = Convert.ToString(dr["Notes"])
                                });
                            }
                        }
                    }
                }

                return list;
            }
        }
    }