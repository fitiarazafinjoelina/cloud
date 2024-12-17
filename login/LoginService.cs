using cloud.Database;
using cloud.email;
using cloud.helper;
using cloud.user;
using cloud.lifeCycle;
using cloud.user;
using cloud.pin;
using cloud.temporaryToken;
using Org.BouncyCastle.Asn1.Cms;

namespace cloud.login;

public class LoginService
{
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;
    private readonly PinService _pinService;
    private readonly EmailService _emailService;
    public async Task<string> login(LoginDTO login)
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

        TemporaryToken token = _tokenService.CreateLoginTemporaryTokenAsync(user.IdUser).Result;
        UserToken userToken = new UserToken();
        userToken.token = token.Value;
        userToken.user = user;
        user.Password = "";
        Pin pin = _pinService.CreatePin(userToken.user.IdUser).Result;
        await _emailService.SendEmailOtpAsync("Nigga",user.Email,pin.PinNumber.ToString());
        return userToken.token;
    }

    public string pin(string token, string pin)
    {
        User user = _tokenService.getUserByTemporaryToken(token);
        bool isValid = _pinService.VerifyPin(user.IdUser, pin);
        if (!isValid)
        {
            user.NbTentative++;
        }
        else
        {
            user.NbTentative = 0;
            return _tokenService.CreateLoginTokenAsync(user.IdUser).Result.Value;
        }
        _context.SaveChanges();
        return "";
    }

    public async void SendInitEmail(string token)
    {
        User user = _tokenService.getUserByTemporaryToken(token);
        await _emailService.SendEmailAsync("Fits",user.Email,"Reinitialisation of nb de tentative","get /init-nb-tentative");
    }
    public void InitNbTentative(string token)
    {
        User user = _tokenService.getUserByTemporaryToken(token);
        user.NbTentative = 0;
        _context.SaveChanges();
    }
}