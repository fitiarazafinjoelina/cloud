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

    [HttpGet]
    public  ResponseBody GetUser([FromHeader] String Authorization) {
        // if(Authorization == null)
        ResponseBody responseBody = new ResponseBody();
        try {
            User user = _tokenService.getUserByToken(Authorization);
            responseBody.StatusCode = 200;
            responseBody.Data = new {
                Email = user.Email,
                Username = user.Username,
                Password = user.Password
            };
        }
        catch (Exception e) {
            responseBody.StatusCode = 500;
            responseBody.Data = e.Message;
        }

        return responseBody;
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
    public IActionResult Modification([FromHeader] string Authorization , UserModifDTO userModifDto) {
        User user = _tokenService.getUserByToken(Authorization);
        
        user.Username = userModifDto.Username;
            
        _context.Users.Update(user);
        _context.SaveChanges();

        return Ok();
    }
    
    [HttpPost]
    public IActionResult ChangePassword([FromHeader] string Authorization , UserPasswordDTO userPasswordDto) {
        User user = _tokenService.getUserByToken(Authorization);
        
        user.Password = PasswordHelper.HashPassword(userPasswordDto.Password);
        
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