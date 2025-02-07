namespace cloud.login;

public class LoginPinDTO
{
    public string PinToken { get; set; }
    public int Pin { get; set; }

    public override string ToString()
    {
        return Pin.ToString();
    }
}