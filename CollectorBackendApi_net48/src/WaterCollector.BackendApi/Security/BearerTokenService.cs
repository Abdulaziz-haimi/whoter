using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace WaterCollector.BackendApi.Security
{
    public sealed class BearerTokenService
    {
        public string CreateToken(int userId, string userName, string fullName, int collectorId, string collectorName, out DateTime expiresAt)
        {
            var key = GetRequiredAppSetting("Jwt:Key");
            var issuer = GetRequiredAppSetting("Jwt:Issuer");
            var audience = GetRequiredAppSetting("Jwt:Audience");
            var expiryMinutes = int.TryParse(ConfigurationManager.AppSettings["Jwt:ExpiryMinutes"], out var m) ? m : 480;

            expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);
            var payload = new TokenPayload
            {
                iss = issuer,
                aud = audience,
                uid = userId,
                uname = userName,
                fullName = fullName,
                collectorId = collectorId,
                collectorName = collectorName,
                exp = ToUnixSeconds(expiresAt)
            };

            var json = JsonConvert.SerializeObject(payload);
            var body = Base64UrlEncode(Encoding.UTF8.GetBytes(json));
            var sig = ComputeSignature(body, key);
            return body + "." + sig;
        }

        public ClaimsPrincipal Validate(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new UnauthorizedAccessException("الرمز غير موجود.");
            var parts = token.Split('.');
            if (parts.Length != 2) throw new UnauthorizedAccessException("الرمز غير صالح.");

            var key = GetRequiredAppSetting("Jwt:Key");
            var expected = ComputeSignature(parts[0], key);
            if (!FixedTimeEquals(expected, parts[1])) throw new UnauthorizedAccessException("توقيع الرمز غير صالح.");

            var json = Encoding.UTF8.GetString(Base64UrlDecode(parts[0]));
            var payload = JsonConvert.DeserializeObject<TokenPayload>(json);
            if (payload == null) throw new UnauthorizedAccessException("الرمز غير صالح.");

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            if (payload.exp <= now) throw new UnauthorizedAccessException("انتهت صلاحية الجلسة.");

            var issuer = GetRequiredAppSetting("Jwt:Issuer");
            var audience = GetRequiredAppSetting("Jwt:Audience");
            if (!string.Equals(payload.iss, issuer, StringComparison.Ordinal) || !string.Equals(payload.aud, audience, StringComparison.Ordinal))
                throw new UnauthorizedAccessException("مصدر الرمز غير صالح.");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, payload.uid.ToString()),
                new Claim(ClaimTypes.Name, payload.uname ?? string.Empty),
                new Claim("collector_id", payload.collectorId.ToString()),
                new Claim("collector_name", payload.collectorName ?? string.Empty)
            };
            if (!string.IsNullOrWhiteSpace(payload.fullName)) claims.Add(new Claim("full_name", payload.fullName));
            var identity = new ClaimsIdentity(claims, "Bearer");
            return new ClaimsPrincipal(identity);
        }

        private static string GetRequiredAppSetting(string key)
        {
            var value = ConfigurationManager.AppSettings[key];
            if (string.IsNullOrWhiteSpace(value)) throw new ConfigurationErrorsException($"AppSettings['{key}'] غير موجود.");
            return value;
        }

        private static string ComputeSignature(string body, string secret)
        {
            using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                return Base64UrlEncode(hmac.ComputeHash(Encoding.UTF8.GetBytes(body)));
            }
        }

        private static bool FixedTimeEquals(string a, string b)
        {
            var aa = Encoding.UTF8.GetBytes(a ?? string.Empty);
            var bb = Encoding.UTF8.GetBytes(b ?? string.Empty);
            if (aa.Length != bb.Length) return false;
            var diff = 0;
            for (var i = 0; i < aa.Length; i++) diff |= aa[i] ^ bb[i];
            return diff == 0;
        }

        private static long ToUnixSeconds(DateTime dt) => new DateTimeOffset(dt).ToUnixTimeSeconds();
        private static string Base64UrlEncode(byte[] input) => Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        private static byte[] Base64UrlDecode(string input)
        {
            var s = input.Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4)
            {
                case 2: s += "=="; break;
                case 3: s += "="; break;
            }
            return Convert.FromBase64String(s);
        }

        private sealed class TokenPayload
        {
            public string iss { get; set; }
            public string aud { get; set; }
            public int uid { get; set; }
            public string uname { get; set; }
            public string fullName { get; set; }
            public int collectorId { get; set; }
            public string collectorName { get; set; }
            public long exp { get; set; }
        }
    }
}
