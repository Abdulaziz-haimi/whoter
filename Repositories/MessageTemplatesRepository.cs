using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;
using water3.Models;
namespace water3.Repositories
{
   

        public class MessageTemplatesRepository
        {
            public List<MessageTemplate> GetActiveTemplates(string language = null)
            {
                var list = new List<MessageTemplate>();

                using (var con = Db.GetConnection())
                using (var cmd = new SqlCommand(@"
SELECT TemplateID, TemplateName, TemplateText, TemplateType, IsActive, Language, CreatedAt
FROM dbo.MessageTemplates
WHERE IsActive = 1
  AND (@Lang IS NULL OR Language = @Lang)
ORDER BY TemplateName;", con))
                {
                    cmd.Parameters.Add("@Lang", SqlDbType.NVarChar, 20).Value = (object)language ?? DBNull.Value;

                    con.Open();
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new MessageTemplate
                            {
                                TemplateID = Convert.ToInt32(r["TemplateID"]),
                                TemplateName = Convert.ToString(r["TemplateName"]),
                                TemplateText = Convert.ToString(r["TemplateText"]),
                                TemplateType = Convert.ToString(r["TemplateType"]),
                                IsActive = Convert.ToBoolean(r["IsActive"]),
                                Language = Convert.ToString(r["Language"]),
                                CreatedAt = Convert.ToDateTime(r["CreatedAt"])
                            });
                        }
                    }
                }

                return list;
            }

            public MessageTemplate GetById(int templateId)
            {
                using (var con = Db.GetConnection())
                using (var cmd = new SqlCommand(@"
SELECT TOP 1 TemplateID, TemplateName, TemplateText, TemplateType, IsActive, Language, CreatedAt
FROM dbo.MessageTemplates
WHERE TemplateID = @ID;", con))
                {
                    cmd.Parameters.Add("@ID", SqlDbType.Int).Value = templateId;
                    con.Open();

                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read()) return null;

                        return new MessageTemplate
                        {
                            TemplateID = Convert.ToInt32(r["TemplateID"]),
                            TemplateName = Convert.ToString(r["TemplateName"]),
                            TemplateText = Convert.ToString(r["TemplateText"]),
                            TemplateType = Convert.ToString(r["TemplateType"]),
                            IsActive = Convert.ToBoolean(r["IsActive"]),
                            Language = Convert.ToString(r["Language"]),
                            CreatedAt = Convert.ToDateTime(r["CreatedAt"])
                        };
                    }
                }
            }
        }
    }