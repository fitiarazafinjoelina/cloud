using cloud.Database;
using cloud.email;
using cloud.helper;
using cloud.userValidation;
using cloud.Utils;
using cloud.lifeCycle;
using Microsoft.AspNetCore.Mvc;

namespace cloud.user;

[ApiController]
[Route("/api/[controller]/[action]")]
public class UserController: ControllerBase {
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;
    private readonly TokenService _tokenService;
    private readonly UserService _userService;


    public UserController(AppDbContext context, EmailService emailService, TokenService tokenService, UserService userService) {
        _context = context;
        _emailService = emailService;
        _tokenService = tokenService;
        _userService = userService;
    }
    
    

    [HttpPost]
    public async Task<ResponseBody> Inscription(UserInscriptionDTO userInscriptionDto) {
        ResponseBody body = new ResponseBody();

        try {
            UserValidation userValidation = _userService.SignUpUser(userInscriptionDto);
            await _emailService.SendEmailAsync("Cloud", userInscriptionDto.Email, "Confirmation Compte", EmailHelper.GetValidationEmail(userValidation.Id));
            body.StatusCode = 200;
            body.Message = "Un mail a ete envoye pour confirmer votre compte";
        }
        catch (Exception e) {
            body.StatusCode = 500;
            body.Message = e.Message;
        }

        return body;
    }
    
    [HttpPost]
    public IActionResult Modification([FromHeader] string Authorization ,UserInscriptionDTO userInscriptionDto) {
        User user = _tokenService.getUserByToken(Authorization);

        if (!_emailService.CheckEmail(userInscriptionDto.Email))
        {
            throw new Exception("Email invalide");
        }
        user.Username = userInscriptionDto.Username;
        user.Email = userInscriptionDto.Email;
            
        _context.Users.Update(user);
        _context.SaveChanges();

        return Ok();
    }
    
    [HttpPost]
    public IActionResult ChangePassword([FromHeader] string Authorization ,UserInscriptionDTO userInscriptionDto) {
        User user = _tokenService.getUserByToken(Authorization);
        
        user.Password = userInscriptionDto.Password;
            
        _context.Users.Update(user);
        _context.SaveChanges();

        return Ok();
    }



    [HttpGet]
    public IActionResult Users() {
        // List<string> list = new List<string>();
        // list.Add("hello");
        return Ok(_context.Users.ToList());
    }
}