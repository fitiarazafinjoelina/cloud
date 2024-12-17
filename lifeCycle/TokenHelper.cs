using System;
using System.Security.Cryptography;
using System.Text;

namespace cloud.lifeCycle
{
    public static class TokenHelper
    {
        public static string GenerateToken(int userId)
        {
            using (var hmac = new HMACSHA256())
            {
                var key = Encoding.UTF8.GetBytes($"{userId}-{Guid.NewGuid()}-{DateTime.UtcNow}");
                var hash = hmac.ComputeHash(key);
                var base64Token = Convert.ToBase64String(hash);

                var urlSafeToken = base64Token.Replace("+", "-").Replace("/", "_").TrimEnd('=');

                return urlSafeToken;
            }

        }
    }
}