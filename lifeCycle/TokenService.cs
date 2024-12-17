using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using cloud.Database;
using cloud.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace cloud.lifeCycle
{
    public class TokenService
    {
        private readonly AppDbContext _context;
        private readonly TokenSettings _tokenSettings;

        public TokenService(AppDbContext context, IOptions<TokenSettings> tokenSettings)
        {
            _context = context;
            _tokenSettings = tokenSettings.Value;
        }
        public string createToken(string email){
            User user = _context.Users.FirstOrDefaultAsync(user => user.Email == email).Result;
            int userId = user.IdUser;
            return TokenHelper.GenerateToken(userId);
        }

        public User getUserByToken(string token)
        {
            Token tok = _context.Tokens.FirstOrDefaultAsync(tokenClass => tokenClass.Value == token).Result;
            if (tok == null)
            {
                throw new Exception("Invalid token");
            }
            if (IsTokenValidAsync(tok.Value).Result)
            {
                return _context.Users.FirstOrDefaultAsync(userClass => userClass.IdUser == tok.UserId).Result;
            }
            else
            {
                throw new Exception("Invalid token");
            }
        }
        public async Task<Token> CreateLoginTokenAsync(int userId)
        {
            string generatedToken = TokenHelper.GenerateToken(userId);

            Token token = new Token
            {
                UserId = userId,
                Value = generatedToken,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(_tokenSettings.TokenExpiryHours)
            };

            _context.Tokens.Add(token);
            await _context.SaveChangesAsync();

            return token;
        }
        public async Task<bool> IsTokenValidAsync(string token)
        {
            var tokena = await _context.Tokens
                .Where(t => t.Value == token)
                .OrderByDescending(t => t.StartDate)
                .FirstOrDefaultAsync();
            if (tokena == null)
            {
                return false;
            }
            if (tokena.EndDate < DateTime.UtcNow)
            {
                return false;
            }
            return true;
        }

    }
}