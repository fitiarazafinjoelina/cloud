namespace cloud.helper;
using System;

public class PinHelper
{
    public static string GeneratePin(int length = 6)
    {
        var random = new Random();
        string otp = string.Empty;

        for (int i = 0; i < length; i++)
        {
            otp += random.Next(0, 10); // Generates a digit between 0-9
        }

        return otp;
    }
}
