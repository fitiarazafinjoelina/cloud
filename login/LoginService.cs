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

    public LoginService(AppDbContext context, TokenService tokenService, PinService pinService, EmailService emailService)
    {
        _context = context;
        _tokenService = tokenService;
        _pinService = pinService;
        _emailService = emailService;
    }


    public string login(LoginDTO login)
    {
        User user = _context.Users.FirstOrDefault(u => u.Email == login.Email);
        if (user == null)
        {
            Console.WriteLine("Helloooo");
            throw new KeyNotFoundException("This email address is not found");
        }
        if (user.NbTentative > 3)
        {
            throw new Exception("Nb tentative max atteinte");
        }
        Console.WriteLine(user);
        // Console.WriteLine(PasswordHelper.HashPassword(login.Password));
        // Console.WriteLine(PasswordHelper.VerifyPassword(login.Password,user.Password));
        if (!PasswordHelper.VerifyPassword(login.Password,user.Password))
        {
            user.NbTentative++;
            _context.SaveChanges();
            throw new Exception("Wrong password");
        }


        TemporaryToken token = _tokenService.CreateLoginTemporaryTokenAsync(user.IdUser);
        UserToken userToken = new UserToken();
        userToken.token = token.Value;
        userToken.user = user;
        Pin pin = _pinService.CreatePin(userToken.user.IdUser).Result;
        _emailService.SendEmailOtpAsync("Nigga",user.Email,pin.PinNumber.ToString());
        return userToken.token;
    }

    public string pin(string token, string pin)
    {
        User user = _tokenService.getUserByTemporaryToken(token);
        bool isValid = _pinService.VerifyPin(user.IdUser, pin);
        if (user.NbTentative > 3)
        {
            throw new Exception("Nb tentative max atteinte");
        }
        if (!isValid)
        {
            user.NbTentative++;
            _context.SaveChanges();
            throw new Exception("Invalid pin");
        }
        else
        {
            user.NbTentative = 0;
            _context.SaveChanges();
            string tokene = _tokenService.CreateLoginTokenAsync(user.IdUser).Result.Value;
            return tokene;
        }
    }

    public async void SendInitEmail(string email)
    {
        User user = _context.Users.FirstOrDefault(u => u.Email == email);
        string token = _tokenService.CreateLoginTemporaryTokenAsync(user.IdUser).Value;
        // User user = _tokenService.getUserByTemporaryToken(token);
        await _emailService.SendEmailAsync("Fits",user.Email,"Reinitialisation of nb de tentative","get Login/init-nb-tentative/"+token);
    }
    public void InitNbTentative(string token)
    {
        User user = _tokenService.getUserByTemporaryToken(token);
        user.NbTentative = 0;
        _context.SaveChanges();
    }
}