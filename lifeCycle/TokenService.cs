using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using cloud.Database;
using cloud.user;
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
            Token tok = _context.Tokens.FirstOrDefault(tokenClass => tokenClass.Value == token);
            if (tok == null)
            {
                throw new Exception("Invalid token");
            }
            if (IsTokenValidAsync(tok.Value))
            {
                return _context.Users.FirstOrDefault(userClass => userClass.IdUser == tok.UserId);
            }
            else
            {
                throw new Exception("Invalid token");
            }

            // return _context.Users.Find(1);
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
        public bool IsTokenValidAsync(string token)
        {
            Token tokena = _context.Tokens
                .Where(t => t.Value == token)
                .OrderByDescending(t => t.StartDate)
                .FirstOrDefault();
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