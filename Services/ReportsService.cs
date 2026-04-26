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
namespace water3.Services
{

        public class ReportsService
        {
            private readonly AuditLogService _audit = new AuditLogService();

            public List<GenericReportRow> GetInvoicesReport(DateTime fromDate, DateTime toDate, int? subscriberId)
            {
                return ExecuteGenericReport(
                    "dbo.rpt_Invoices_Generic",
                    new SqlParameter("@FromDate", fromDate.Date),
                    new SqlParameter("@ToDate", toDate.Date),
                    new SqlParameter("@SubscriberID", subscriberId.HasValue ? (object)subscriberId.Value : DBNull.Value));
            }

            public List<GenericReportRow> GetPaymentsReport(DateTime fromDate, DateTime toDate, int? subscriberId)
            {
                return ExecuteGenericReport(
                    "dbo.rpt_Payments_Generic",
                    new SqlParameter("@FromDate", fromDate.Date),
                    new SqlParameter("@ToDate", toDate.Date),
                    new SqlParameter("@SubscriberID", subscriberId.HasValue ? (object)subscriberId.Value : DBNull.Value));
            }

            public List<GenericReportRow> GetReceiptsReport(DateTime fromDate, DateTime toDate, int? subscriberId)
            {
                return ExecuteGenericReport(
                    "dbo.rpt_Receipts_Generic",
                    new SqlParameter("@FromDate", fromDate.Date),
                    new SqlParameter("@ToDate", toDate.Date),
                    new SqlParameter("@SubscriberID", subscriberId.HasValue ? (object)subscriberId.Value : DBNull.Value));
            }

            public List<GenericReportRow> GetAccountStatement(DateTime fromDate, DateTime toDate, int subscriberId)
            {
                return ExecuteGenericReport(
                    "dbo.rpt_AccountStatement_Generic",
                    new SqlParameter("@SubscriberID", subscriberId),
                    new SqlParameter("@FromDate", fromDate.Date),
                    new SqlParameter("@ToDate", toDate.Date));
            }

            public DataTable GetAgingReceivables(DateTime asOfDate)
            {
                return ExecuteTableReport(
                    "dbo.rpt_AgingReceivables",
                    new SqlParameter("@AsOfDate", asOfDate.Date));
            }

            public DataTable GetGeneralJournal(DateTime fromDate, DateTime toDate, string source = null)
            {
                return ExecuteTableReport(
                    "dbo.rpt_GeneralJournal",
                    new SqlParameter("@FromDate", fromDate.Date),
                    new SqlParameter("@ToDate", toDate.Date),
                    new SqlParameter("@Source", string.IsNullOrWhiteSpace(source) ? (object)DBNull.Value : source));
            }

            public DataTable GetTrialBalance(DateTime asOfDate)
            {
                return ExecuteTableReport(
                    "dbo.rpt_TrialBalance",
                    new SqlParameter("@AsOfDate", asOfDate.Date));
            }

            public List<SubscriberLookupItem> GetSubscribers()
            {
                var list = new List<SubscriberLookupItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(@"
                SELECT SubscriberID, Name, PhoneNumber, Address
                FROM dbo.Subscribers
                WHERE ISNULL(IsActive, 1) = 1
                ORDER BY Name", con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new SubscriberLookupItem
                            {
                                SubscriberID = Convert.ToInt32(dr["SubscriberID"]),
                                Name = Convert.ToString(dr["Name"]),
                                PhoneNumber = dr["PhoneNumber"] == DBNull.Value ? null : Convert.ToString(dr["PhoneNumber"]),
                                Address = dr["Address"] == DBNull.Value ? null : Convert.ToString(dr["Address"])
                            });
                        }
                    }
                }

                return list;
            }

            public void LogReportOpen(string reportKey, string details = null)
            {
                _audit.Log(
                    action: "OPEN_REPORT",
                    tableName: "Reports",
                    recordId: null,
                    details: details ?? reportKey,
                    entityName: reportKey);
            }

            private List<GenericReportRow> ExecuteGenericReport(string procedure, params SqlParameter[] parameters)
            {
                var list = new List<GenericReportRow>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(procedure, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null && parameters.Length > 0)
                        cmd.Parameters.AddRange(parameters);

                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new GenericReportRow
                            {
                                Dt1 = HasColumn(dr, "Dt1") && dr["Dt1"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(dr["Dt1"]) : null,
                                Col1 = GetString(dr, "Col1"),
                                Col2 = GetString(dr, "Col2"),
                                Col3 = GetString(dr, "Col3"),
                                Col4 = GetString(dr, "Col4"),
                                Col5 = GetString(dr, "Col5"),
                                Num1 = GetDecimal(dr, "Num1"),
                                Num2 = GetDecimal(dr, "Num2"),
                                Num3 = GetDecimal(dr, "Num3"),
                                Num4 = GetDecimal(dr, "Num4"),
                                Num5 = GetDecimal(dr, "Num5"),
                                RowId = GetInt(dr, "RowId"),
                                RefId1 = GetInt(dr, "RefId1"),
                                RefId2 = GetInt(dr, "RefId2")
                            });
                        }
                    }
                }

                return list;
            }

            private DataTable ExecuteTableReport(string procedure, params SqlParameter[] parameters)
            {
                DataTable dt = new DataTable();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand(procedure, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null && parameters.Length > 0)
                        cmd.Parameters.AddRange(parameters);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }

                return dt;
            }

            private bool HasColumn(SqlDataReader dr, string columnName)
            {
                for (int i = 0; i < dr.FieldCount; i++)
                    if (dr.GetName(i) == columnName)
                        return true;
                return false;
            }

            private string GetString(SqlDataReader dr, string name)
            {
                return HasColumn(dr, name) && dr[name] != DBNull.Value ? Convert.ToString(dr[name]) : null;
            }

            private decimal? GetDecimal(SqlDataReader dr, string name)
            {
                return HasColumn(dr, name) && dr[name] != DBNull.Value ? (decimal?)Convert.ToDecimal(dr[name]) : null;
            }

            private int? GetInt(SqlDataReader dr, string name)
            {
                return HasColumn(dr, name) && dr[name] != DBNull.Value ? (int?)Convert.ToInt32(dr[name]) : null;
            }
        }
    }