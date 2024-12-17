using cloud.Database;
using Microsoft.AspNetCore.Mvc;

namespace cloud.user;

[ApiController]
[Route("/api/[controller]/[action]")]

public class UserController: ControllerBase {
    private readonly AppDbContext _context;

    public UserController(AppDbContext context) {
        _context = context;
    }

    [HttpPost]
    public IActionResult Inscription(UserInscriptionDTO userInscriptionDto) {
        User user = new User() {
            Username = userInscriptionDto.Username,
            Email = userInscriptionDto.Email,
            Password = userInscriptionDto.Password
        };

        _context.Users.Add(user);
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