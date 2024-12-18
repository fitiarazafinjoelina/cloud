using cloud.Database;
using cloud.lifeCycle;
using cloud.temporaryToken;
using cloud.user;

namespace cloud.uniqIdentifier;

public class UniqIndentifierService
{
    private readonly AppDbContext _context;
    private readonly TokenSettings _tokenSettings;
    public UniqIndentifierService(AppDbContext db,TokenSettings tokenSettings)
    {
        _context = db;
        _tokenSettings = tokenSettings;
    }
    public UniqIdentifier CreateUniqIdentifier(int userId)
    {
        string generatedToken = TokenHelper.GenerateToken(userId);
        UniqIdentifier token = new UniqIdentifier
        {
            UserId = userId,
            Uniqid = generatedToken,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddHours(_tokenSettings.TokenExpiryHours)
        };
        _context.UniqIdentifiers.Add(token);
        _context.SaveChanges();
        return token;
    }
    public bool IsUniqIdentifierValidAsync(string token)
    {
        Console.WriteLine("D'accord");
        UniqIdentifier tokena =  _context.UniqIdentifiers
            .Where(t => t.Uniqid == token)
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
    public User getUserByUniqIdentifier(string token)
    {
        UniqIdentifier tok = _context.UniqIdentifiers.FirstOrDefault(tokenClass => tokenClass.Uniqid == token);
        if (tok == null)
        {
            throw new Exception("Invalid token");
        }
        if (IsUniqIdentifierValidAsync(tok.Uniqid))
        {
            return _context.Users.FirstOrDefault(userClass => userClass.IdUser == tok.UserId);
        }
        else
        {
            throw new Exception("Invalid token");
        }

        // return _context.Users.Find(1);
    }
}