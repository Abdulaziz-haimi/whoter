using System;
using System.Collections.Generic;
using System.IO;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
namespace water3.Forms
{
  

    public static class HtmlReportRenderer
    {
        public static string Render(string templatePath, Dictionary<string, string> placeholders)
        {
            // قراءة ملف القالب
            string templateContent = File.ReadAllText(templatePath);

            // استبدال العناصر النائبة
            foreach (var placeholder in placeholders)
            {
                templateContent = templateContent.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
            }

            return templateContent;
        }

        // يمكنك إضافة overload آخر إذا كنت تستخدم مكانات معقدة
        public static string Render(string templatePath, Dictionary<string, object> placeholders)
        {
            string templateContent = File.ReadAllText(templatePath);

            foreach (var placeholder in placeholders)
            {
                templateContent = templateContent.Replace($"{{{{{placeholder.Key}}}}}",
                    placeholder.Value?.ToString() ?? "");
            }

            return templateContent;
        }
    }
}