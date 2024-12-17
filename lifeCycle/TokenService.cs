using System;
using System.Collections.Generic;
// using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using cloud.Database;
using cloud.user;
using cloud.temporaryToken;
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
            User user = _context.Users.FirstOrDefault(user => user.Email == email);
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
        }
        public User getUserByTemporaryToken(string token)
        {
            TemporaryToken tok = _context.TemporaryTokens.FirstOrDefault(tokenClass => tokenClass.Value == token);
            if (tok == null)
            {
                throw new Exception("Invalid token");
            }
            if (IsTemporaryTokenValidAsync(tok.Value))
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
        public TemporaryToken CreateLoginTemporaryTokenAsync(int userId)
        {
            string generatedToken = TokenHelper.GenerateToken(userId);

            TemporaryToken token = new TemporaryToken
            {
                UserId = userId,
                Value = generatedToken,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddHours(_tokenSettings.TokenExpiryHours)
            };

            _context.TemporaryTokens.Add(token);
            _context.SaveChanges();
            return token;
        }
        public bool IsTokenValidAsync(string token)
        {
            Console.WriteLine("D'accord");
            Token tokena =  _context.Tokens
                .Where(t => t.Value == token)
                .OrderByDescending(t => t.StartDate)
                .FirstOrDefault();
            if (tokena == null)
            {
                Console.WriteLine("Echo");
                return false;
            }
            if (tokena.EndDate < DateTime.UtcNow)
            {
                return false;
            }
            return true;
        }
        public bool IsTemporaryTokenValidAsync(string token)
        {
            Console.WriteLine("D'accord");
            TemporaryToken tokena =  _context.TemporaryTokens
                .Where(t => t.Value == token)
                .OrderByDescending(t => t.StartDate)
                .FirstOrDefault();
            if (tokena == null)
            {
                Console.WriteLine("Echo");
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