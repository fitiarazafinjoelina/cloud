using Microsoft.AspNetCore.Mvc;

namespace cloud.lifeCycle;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    public TokenService _tokenService;
    
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
    public async Task<Boolean> ValidateToken([FromBody] string token)
    {   
        var isValid = _tokenService.IsTokenValidAsync(token).Result;
        return isValid;
    }
    
}
