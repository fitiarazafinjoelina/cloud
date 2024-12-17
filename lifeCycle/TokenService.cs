using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using cloud.Database;
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
        public void createToken(string mail){
           // int userId = 
           // TokenHelper.GenerateToken(userId);
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

            // Save to database
            _context.Tokens.Add(token);
            await _context.SaveChangesAsync();

            return token;
        }
        

    }
}