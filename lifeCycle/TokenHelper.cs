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
                return Convert.ToBase64String(hmac.ComputeHash(key));
            }
        }
    }
}