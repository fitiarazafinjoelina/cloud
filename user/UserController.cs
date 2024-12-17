using cloud.Database;
using cloud.lifeCycle;
using Microsoft.AspNetCore.Mvc;

namespace cloud.user;

[ApiController]
[Route("/api/[controller]/[action]")]

public class UserController: ControllerBase {
    private readonly AppDbContext _context;
    private readonly TokenService _tokenService;
    
    public UserController(AppDbContext context, TokenService tokenService) {
        _context = context;
        _tokenService = tokenService;
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
    
    [HttpPost]
    public IActionResult Modification([FromHeader] string Authorization ,UserInscriptionDTO userInscriptionDto) {
        User user = _tokenService.getUserByToken(Authorization);
        
        user.Username = userInscriptionDto.Username;
        user.Email = userInscriptionDto.Email;
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