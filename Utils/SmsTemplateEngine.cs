using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace water3.Utils
{
    public static class SmsTemplateEngine
    {
        private static readonly Regex TokenRegex =
            new Regex(@"\{([A-Za-z0-9_]+)\}", RegexOptions.Compiled);

        public static string Render(string template, IDictionary<string, string> values)
        {
            if (string.IsNullOrWhiteSpace(template))
                return string.Empty;

            values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            return TokenRegex.Replace(template, match =>
            {
                string key = match.Groups[1].Value;

                if (values.TryGetValue(key, out string value))
                    return value ?? string.Empty;

                // مثل SendSMSDynamic: المتغير غير الموجود يصبح فارغاً
                return string.Empty;
            });
        }

        public static List<string> ExtractTokens(string template)
        {
            if (string.IsNullOrWhiteSpace(template))
                return new List<string>();

            return TokenRegex.Matches(template)
                .Cast<Match>()
                .Select(m => m.Groups[1].Value)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x)
                .ToList();
        }

        public static List<string> FindUnsupportedTokens(string template, IEnumerable<string> supportedTokens)
        {
            var supported = new HashSet<string>(
                supportedTokens ?? Enumerable.Empty<string>(),
                StringComparer.OrdinalIgnoreCase);

            return ExtractTokens(template)
                .Where(t => !supported.Contains(t))
                .OrderBy(x => x)
                .ToList();
        }
    }
}