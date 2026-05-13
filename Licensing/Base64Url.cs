using System;

namespace water3.Licensing
{
    public static class Base64Url
    {
        public static string Encode(byte[] data)
        {
            return Convert.ToBase64String(data)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

        public static byte[] Decode(string text)
        {
            string s = text.Replace('-', '+').Replace('_', '/');

            switch (s.Length % 4)
            {
                case 2:
                    s += "==";
                    break;
                case 3:
                    s += "=";
                    break;
            }

            return Convert.FromBase64String(s);
        }
    }
}
