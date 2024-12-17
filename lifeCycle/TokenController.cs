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
    public async Task<bool> ValidateToken([FromHeader] string Authorization)
    {
        if (string.IsNullOrEmpty(Authorization) || !Authorization.StartsWith("Bearer "))
        {
            return false; // Invalid or missing token
        }

        // Extract token from the Authorization header
        var token = Authorization.Substring("Bearer ".Length).Trim();

        var isValid = await _tokenService.IsTokenValidAsync(token);

        return isValid;
    }

    
}
