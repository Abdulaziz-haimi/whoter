using System.Globalization;
using System.Text;

namespace water3.Utils
{
    public static class MoneyParser
    {
        public static string NormalizeMoneyText(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            string s = NormalizeDigits(input.Trim());
            s = s.Replace("٬", "").Replace(",", "").Replace(" ", "");
            s = s.Replace("٫", ".");

            var sb = new StringBuilder();
            bool dotAdded = false;
            foreach (char c in s)
            {
                if (char.IsDigit(c)) sb.Append(c);
                else if (c == '.' && !dotAdded) { sb.Append('.'); dotAdded = true; }
            }
            return sb.ToString();
        }

        public static bool TryParse(string input, out decimal value)
        {
            value = 0m;
            string s = NormalizeMoneyText(input);
            if (string.IsNullOrWhiteSpace(s)) return false;

            return decimal.TryParse(
                s,
                NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                CultureInfo.InvariantCulture,
                out value
            );
        }

        public static char NormalizeArabicDigit(char c)
        {
            if (c >= '٠' && c <= '٩') return (char)('0' + (c - '٠'));
            if (c >= '۰' && c <= '۹') return (char)('0' + (c - '۰'));
            return c;
        }

        private static string NormalizeDigits(string input)
        {
            var sb = new StringBuilder(input.Length);
            foreach (char c in input) sb.Append(NormalizeArabicDigit(c));
            return sb.ToString();
        }
    }
}