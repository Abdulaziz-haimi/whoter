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
  

        public class CollectorService
        {
            private readonly AuditLogService _audit = new AuditLogService();

            public List<CollectorItem> GetAll()
            {
                var list = new List<CollectorItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.Collectors_GetAll", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new CollectorItem
                            {
                                CollectorID = Convert.ToInt32(dr["CollectorID"]),
                                Name = Convert.ToString(dr["Name"]),
                                Phone = dr["Phone"] == DBNull.Value ? null : Convert.ToString(dr["Phone"])
                            });
                        }
                    }
                }

                return list;
            }

            public List<CollectorItem> Search(string q)
            {
                var list = new List<CollectorItem>();

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.Collectors_Search", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@q", string.IsNullOrWhiteSpace(q) ? string.Empty : q.Trim());
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new CollectorItem
                            {
                                CollectorID = Convert.ToInt32(dr["CollectorID"]),
                                Name = Convert.ToString(dr["Name"]),
                                Phone = dr["Phone"] == DBNull.Value ? null : Convert.ToString(dr["Phone"])
                            });
                        }
                    }
                }

                return list;
            }

            public int Insert(string name, string phone)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new InvalidOperationException("اسم المحصل مطلوب.");

                int newId;

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.Collectors_Insert", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Name", name.Trim());
                    cmd.Parameters.AddWithValue("@Phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone.Trim());
                    con.Open();
                    newId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                _audit.Log(
                    action: "CREATE_COLLECTOR",
                    tableName: "Collectors",
                    recordId: newId,
                    details: $"تم إنشاء محصل جديد: {name}",
                    entityName: name);

                return newId;
            }

            public void Update(int collectorId, string name, string phone)
            {
                if (collectorId <= 0)
                    throw new InvalidOperationException("معرف المحصل غير صحيح.");

                if (string.IsNullOrWhiteSpace(name))
                    throw new InvalidOperationException("اسم المحصل مطلوب.");

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.Collectors_Update", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CollectorID", collectorId);
                    cmd.Parameters.AddWithValue("@Name", name.Trim());
                    cmd.Parameters.AddWithValue("@Phone", string.IsNullOrWhiteSpace(phone) ? (object)DBNull.Value : phone.Trim());
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                _audit.Log(
                    action: "UPDATE_COLLECTOR",
                    tableName: "Collectors",
                    recordId: collectorId,
                    details: $"تم تعديل بيانات المحصل: {name}",
                    entityName: name);
            }

            public void Delete(int collectorId, string collectorName = null)
            {
                if (collectorId <= 0)
                    throw new InvalidOperationException("معرف المحصل غير صحيح.");

                using (SqlConnection con = Db.GetConnection())
                using (SqlCommand cmd = new SqlCommand("dbo.Collectors_Delete", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CollectorID", collectorId);
                    con.Open();
                    cmd.ExecuteNonQuery();
                }

                _audit.Log(
                    action: "DELETE_COLLECTOR",
                    tableName: "Collectors",
                    recordId: collectorId,
                    details: "تم حذف المحصل",
                    entityName: collectorName);
            }
        }
    }