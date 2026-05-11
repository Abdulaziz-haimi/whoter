using System.Security.Cryptography;
using System.Text;

namespace WaterCollector.BackendApi.Security
{
    public static class PasswordHasher
    {
        public static string Sha256Hex(string password)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password ?? string.Empty);
                byte[] hash = sha.ComputeHash(bytes);

                var sb = new StringBuilder();
                foreach (byte b in hash)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }
    }
}
