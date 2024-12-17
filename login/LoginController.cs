using System.Net.Http.Headers;
using cloud.lifeCycle;
using cloud.login.PinManager;
using cloud.user;
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
    [HttpPost("")]
    public async Task<ResponseBody> Login([FromBody] LoginDTO login)
    {
        ResponseBody response = new ResponseBody();
        try
        {
            string user = loginService.login(login).ToString();
            response.StatusCode = 200;
            response.Data = "Success";
            response.Message = "";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(user);
        }
        catch (Exception e)
        {
            response.StatusCode = 500;
            response.Message = e.Message;
        }
        return response;
    }
    [HttpPost("pin-validation")]
    public async Task<ResponseBody> authent([FromHeader] string Authorization,[FromBody] PinDTO pin)
    {
        ResponseBody response = new ResponseBody();
        try
        {
            string user = loginService.pin(Authorization,pin.pin).ToString();
            response.StatusCode = 200;
            response.Data = "Success";
            response.Message = "";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(user);
        }
        catch (Exception e)
        {
            response.StatusCode = 500;
            response.Message = e.Message;
        }
        return response;
    }
}