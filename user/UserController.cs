using cloud.Database;
using cloud.email;
using cloud.helper;
using cloud.userValidation;
using cloud.Utils;
using Microsoft.AspNetCore.Mvc;

namespace cloud.user;

[ApiController]
[Route("/api/[controller]/[action]")]

public class UserController: ControllerBase {
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;

    public UserController(AppDbContext context, EmailService emailService) {
        _context = context;
        _emailService = emailService;
    }

    [HttpPost]
    public async Task<ResponseBody> Inscription(UserInscriptionDTO userInscriptionDto) {
        ResponseBody body = new ResponseBody();

        try {
            UserValidation userValidation = new UserValidation() {
                Username = userInscriptionDto.Username,
                Email = userInscriptionDto.Email,
                Password = PasswordHelper.HashPassword(userInscriptionDto.Password),
            };

            _context.UserValidations.Add(userValidation);
            _context.SaveChanges();

            await _emailService.SendEmailAsync("Test Email", userInscriptionDto.Email, "PIN Confirmation", EmailHelper.GetValidationEmail(userValidation.Id));
            body.StatusCode = 200;
            body.Message = "Un mail est envoye pour confirmer votre compte";
        }
        catch (Exception e) {
            body.StatusCode = 500;
            body.Message = e.Message;
        }

        return body;
    }

    


    [HttpGet]
    public IActionResult Users() {
        // List<string> list = new List<string>();
        // list.Add("hello");
        return Ok(_context.Users.ToList());
    }
}