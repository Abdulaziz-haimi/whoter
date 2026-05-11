using System;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WaterCollector.BackendApi.Security
{
    public sealed class BearerTokenService
    {
        private readonly string _secret;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _minutes;

        public BearerTokenService()
        {
            _secret = ConfigurationManager.AppSettings["Jwt:SecretKey"];
            if (string.IsNullOrWhiteSpace(_secret))
                _secret = "CHANGE_THIS_SECRET_KEY_WATER_COLLECTOR_2026_LONG_VALUE";

            _issuer = ConfigurationManager.AppSettings["Jwt:Issuer"] ?? "water3";
            _audience = ConfigurationManager.AppSettings["Jwt:Audience"] ?? "collector-mobile";

            if (!int.TryParse(ConfigurationManager.AppSettings["Jwt:TokenMinutes"], out _minutes) || _minutes <= 0)
                _minutes = 1440;
        }

        public string CreateToken(
            int userId,
            string userName,
            string fullName,
            int collectorId,
            string collectorName,
            out DateTime expiresAt)
        {
            var now = DateTime.UtcNow;
            expiresAt = now.AddMinutes(_minutes);

            var header = new
            {
                alg = "HS256",
                typ = "JWT"
            };

            var payload = new
            {
                iss = _issuer,
                aud = _audience,
                exp = ToUnixTimeSeconds(expiresAt),
                iat = ToUnixTimeSeconds(now),
                sub = userId.ToString(),
                name = userName,
                full_name = fullName,
                collector_id = collectorId,
                collector_name = collectorName
            };

            string encodedHeader = Base64UrlEncode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header)));
            string encodedPayload = Base64UrlEncode(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload)));
            string signingInput = encodedHeader + "." + encodedPayload;
            string signature = ComputeSignature(signingInput);

            return signingInput + "." + signature;
        }

        public JObject ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new UnauthorizedAccessException("الرمز غير موجود.");

            string[] parts = token.Split('.');
            if (parts.Length != 3)
                throw new UnauthorizedAccessException("صيغة الرمز غير صحيحة.");

            string signingInput = parts[0] + "." + parts[1];
            string expected = ComputeSignature(signingInput);

            if (!SlowEquals(expected, parts[2]))
                throw new UnauthorizedAccessException("توقيع الرمز غير صحيح.");

            string payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(parts[1]));
            var payload = JObject.Parse(payloadJson);

            long exp = payload.Value<long>("exp");
            if (DateTime.UtcNow > FromUnixTimeSeconds(exp))
                throw new UnauthorizedAccessException("انتهت صلاحية الرمز.");

            string iss = payload.Value<string>("iss");
            string aud = payload.Value<string>("aud");

            if (!string.Equals(iss, _issuer, StringComparison.Ordinal))
                throw new UnauthorizedAccessException("مصدر الرمز غير صحيح.");

            if (!string.Equals(aud, _audience, StringComparison.Ordinal))
                throw new UnauthorizedAccessException("جمهور الرمز غير صحيح.");

            return payload;
        }

        private string ComputeSignature(string signingInput)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret)))
            {
                return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(signingInput)));
            }
        }

        private static bool SlowEquals(string a, string b)
        {
            if (a == null || b == null)
                return false;

            int diff = a.Length ^ b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= a[i] ^ b[i];

            return diff == 0;
        }

        private static long ToUnixTimeSeconds(DateTime value)
        {
            return (long)(value.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        private static DateTime FromUnixTimeSeconds(long seconds)
        {
            return new DateTime(1970, 1, 1).AddSeconds(seconds);
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static byte[] Base64UrlDecode(string input)
        {
            string output = input.Replace('-', '+').Replace('_', '/');
            switch (output.Length % 4)
            {
                case 2: output += "=="; break;
                case 3: output += "="; break;
                case 0: break;
                default: throw new FormatException("Invalid base64url string.");
            }
            return Convert.FromBase64String(output);
        }
    }
}
