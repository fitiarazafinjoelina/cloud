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

    public LoginController(LoginService loginService, HttpClient client, TokenService tokenService)
    {
        this.loginService = loginService;
        _client = client;
        this.tokenService = tokenService;
    }
    
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
            // Console.WriteLine(user);
            // request.Headers.Add("Authorization", "YourCustomTokenHere");
            Response.Headers.Authorization = user;
            // _client.DefaultRequestHeaders.Add("Authorization",user);
        }
        catch (Exception e)
        {
            response.StatusCode = 500;
            response.Data = null;
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
            string user = loginService.pin(Authorization,pin.Pin).ToString();
            response.StatusCode = 200;
            response.Data = "Success";
            response.Message = "";
            Response.Headers.Authorization = user;
        }
        catch (Exception e)
        {
            response.StatusCode = 500;
            response.Message = e.Message;
        }
        return response;
    }

    [HttpPost("init-email")]
    public async Task<ResponseBody> InitEmail([FromBody] InitDTO initDto )
    {
        ResponseBody response = new ResponseBody();
        try
        {
            loginService.SendInitEmail(initDto.Email);
            response.StatusCode = 200;
            response.Data = "Mail successfully sent";
            response.Message = "";
        }
        catch (Exception e)
        {
            response.StatusCode = 500;
            response.Message = e.Message;
        }
        return response;
    }
    [HttpGet("init-nb-tentative/{uniqIdentifier}")]
    public async Task<ResponseBody> InitNbTentative([FromRoute] string uniqIdentifier)
    {
        ResponseBody response = new ResponseBody();
        try
        {
            loginService.InitNbTentative(uniqIdentifier);
            response.StatusCode = 200;
            response.Data = "Success";  
            response.Message = "";
        }
        catch (Exception e)
        {
            response.StatusCode = 500;
            response.Message = e.Message;
        }
        return response;
    }
}