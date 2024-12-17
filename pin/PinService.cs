using cloud.Database;
using cloud.helper;

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
        string generatedPin = OTPHelper.GenerateOTP();

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
}