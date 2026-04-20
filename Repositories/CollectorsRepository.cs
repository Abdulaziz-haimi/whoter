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
 

 
        public class CollectorsRepository
        {
            public List<CollectorItem> GetAll()
            {
                var list = new List<CollectorItem>();

                using (var con = Db.GetConnection())
                using (var da = new SqlDataAdapter("SELECT CollectorID, Name FROM Collectors ORDER BY Name", con))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    foreach (DataRow r in dt.Rows)
                    {
                        list.Add(new CollectorItem
                        {
                            CollectorID = int.Parse(r["CollectorID"].ToString()),
                            Name = r["Name"].ToString()
                        });
                    }
                }

                return list;
            }
        }
    }
