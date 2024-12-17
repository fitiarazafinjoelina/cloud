using cloud.Database;
using cloud.email;
using cloud.helper;
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
    public IActionResult Inscription(UserInscriptionDTO userInscriptionDto) {
        User user = new User() {
            Username = userInscriptionDto.Username,
            Email = userInscriptionDto.Email,
            Password = PasswordHelper.HashPassword(userInscriptionDto.Password)
        };

        _context.Users.Add(user);
        _emailService.SendEmailAsync("Test Email", userInscriptionDto.Email, "PIN Confirmation", "0000");


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