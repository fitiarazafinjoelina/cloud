using System.Security.Cryptography;

namespace cloud.helper;


public class PasswordHelper
{
    private const int SaltSize = 20; // 128-bit
    private const int KeySize = 32; // 256-bit
    private const int Iterations = 10000; // Number of PBKDF2 iterations
    private const char Delimiter = '|'; // Separator for salt and hash in the stored value

    // Hash a password
    public static string HashPassword(string password)
    {
        // Generate a cryptographic random salt
        using var rng = RandomNumberGenerator.Create();
        byte[] salt = new byte[SaltSize];
        rng.GetBytes(salt);

        // Derive the hash
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(KeySize);

        // Return the salt and hash as a combined string
        return Convert.ToBase64String(salt) + Delimiter + Convert.ToBase64String(hash);
    }

    // Verify a password against a stored hash
    public static bool VerifyPassword(string password, string storedHash)
    {
        // Split the stored value into salt and hash
        string[] parts = storedHash.Split(Delimiter);
        if (parts.Length != 2)
            throw new FormatException("Invalid hash format.");

        byte[] salt = Convert.FromBase64String(parts[0]);
        byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

        // Derive a hash from the input password using the same salt
        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        byte[] computedHash = pbkdf2.GetBytes(KeySize);

        // Compare the computed hash with the stored hash securely
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
    }
}
