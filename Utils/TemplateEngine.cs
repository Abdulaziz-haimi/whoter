using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace water3.Utils
{
    
  
        public static class TemplateEngine
        {
            public static string Render(string templateText, Dictionary<string, string> tokens)
            {
                if (string.IsNullOrWhiteSpace(templateText)) return "";
                if (tokens == null || tokens.Count == 0) return templateText;

                string msg = templateText;
                foreach (var kv in tokens)
                {
                    msg = msg.Replace("{" + kv.Key + "}", kv.Value ?? "");
                }
                return msg;
            }
        }
    }