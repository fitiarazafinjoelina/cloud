using System.Net.Http.Headers;
using cloud.lifeCycle;
using cloud.Model;
using cloud.Utils;
using Microsoft.AspNetCore.Mvc;

namespace cloud.login;
[ApiController]
[Route("api/user/[controller]")]
public class LoginController:ControllerBase
{
    private readonly LoginService loginService;
    private readonly HttpClient _client;
    private readonly TokenService tokenService;
    [HttpPost("login")]
    public async Task<ResponseBody> Login([FromBody] LoginDTO login)
    {
        ResponseBody response = new ResponseBody();
        try
        {
            UserToken user = loginService.login(login);
            response.StatusCode = 200;
            response.Data = user.user;
            response.Message = "";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(user.token);
        }
        catch (Exception e)
        {
            response.StatusCode = 500;
            response.Message = e.Message;
        }
        return response;
    }
}