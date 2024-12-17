using cloud.Database;
using cloud.helper;
using Microsoft.EntityFrameworkCore;

namespace cloud.pin;

public class PinService
{
    AppDbContext _context;
    PinSettings _pinSettings;

    public PinService(AppDbContext dbContext, PinSettings pinSettings)
    {
        _context = dbContext;
        _pinSettings = pinSettings;
    }
    public async Task<Pin> CreatePin(int userId)
    {
        string generatedPin = PinHelper.GeneratePin();

        Pin pin = new Pin
        {
            IdUser = userId,
            PinNumber = int.Parse(generatedPin),
            DateDebut = DateTime.UtcNow,
            DateFin = DateTime.UtcNow.AddSeconds(_pinSettings.PinExpirySeconds)
        };
        
        _context.Pins.Add(pin);
        await _context.SaveChangesAsync();

        return pin;
    }
    private async Task<Pin?> GetValidPinAsync(int userId)
    {
        var otp = await _context.Pins
            .Where(o => o.IdUser == userId && o.DateFin > DateTime.UtcNow)
            .OrderByDescending(o => o.DateFin)
            .FirstOrDefaultAsync();

        if (otp != null)
        {
            return otp; // Return the OTP if it is valid
        }

        return null;
    }
    
    public bool VerifyPin(int userId, string inputPin)
    {
        var storedPin = GetValidPinAsync(userId).Result;
        
        if (storedPin == null)
        {
            return false; 
        }
        
        var pin = int.Parse(inputPin);
        if (storedPin.PinNumber == pin)
        {
            return true;
        }

        return false;
    }
}