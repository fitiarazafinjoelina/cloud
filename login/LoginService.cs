using cloud.Database;
using cloud.helper;
using cloud.user;
using Org.BouncyCastle.Asn1.Cms;

namespace cloud.login;

public class LoginService
{
    private readonly AppDbContext _context;

    public User login(LoginDTO login)
    {
        User user = _context.Users.FirstOrDefault(u => u.Email == login.Email);
        
        if (user == null)
        {
            throw new KeyNotFoundException("This email address is not found");
        }
        if (user.NbTentative > 3)
        {
            throw new Exception("Nb tentative max atteinte");
        }
        string hashedPassword = PasswordHelper.HashPassword(login.Password);
        if (user.Password != hashedPassword)
        {
            user.NbTentative++;
            _context.SaveChanges();
            throw new Exception("Wrong password");
        }
        user.Password = "";
        return user;
    }
}