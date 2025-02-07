namespace cloud.login;

public class LoginDTO
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class TokenLoginDTO
{
    public string Token { get; set; }
    public string Password { get; set; }
}