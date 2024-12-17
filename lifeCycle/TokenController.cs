using Microsoft.AspNetCore.Mvc;

namespace cloud.lifeCycle;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    public TokenService _tokenService;

    public TokenController(TokenService tokenService)
    {
        _tokenService = tokenService;
    }
    
    /*[HttpPost("login")]
    public async Task<IActionResult> CheckToken(string email)
    {
        // Find user by email
        var user = await _userService.GetUserByEmailAsync(email);
        if (user == null)
        {
            return NotFound("User not found");
        }

        // Check if the token is valid
        bool isTokenValid = await _tokenService.IsTokenValidAsync(user.IdUser);

        if (isTokenValid)
        {
            return Ok("Token is valid");
        }

        return Unauthorized("Token is invalid or expired");
    }*/

    [HttpPost("valid")]
    public async Task<bool> ValidateToken([FromHeader] string Authorization)
    {
        if (string.IsNullOrEmpty(Authorization))
        {
            return false;
        }
        Console.WriteLine(Authorization);

        var isValid = _tokenService.IsTokenValidAsync(Authorization);

        return isValid;
    }

    
}
