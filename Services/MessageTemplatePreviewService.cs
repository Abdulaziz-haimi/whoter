using System.Collections.Generic;
using water3.Utils;

namespace water3.Services
{
    public class MessageTemplatePreviewService
    {
        public List<string> ValidateTemplate(string templateType, string templateText)
        {
            if (string.IsNullOrWhiteSpace(templateText))
                return new List<string>();

            var supported = SmsTemplateVariables.GetNamesByType(templateType);
            return SmsTemplateEngine.FindUnsupportedTokens(templateText, supported);
        }

        public string RenderPreview(string templateType, string templateText)
        {
            if (string.IsNullOrWhiteSpace(templateText))
                return string.Empty;

            var sample = SmsTemplateVariables.CreateSampleValues(templateType);
            return SmsTemplateEngine.Render(templateText, sample);
        }
    }
}