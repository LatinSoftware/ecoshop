using System.Security.Cryptography;
using System.Text;

namespace UserService.Helpers;

public static class PasswordHasher
{
    public static string HashPassword(string password, out string salt)
    {

        byte[] saltBytes = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(saltBytes);
        }
        salt = Convert.ToBase64String(saltBytes);

        using var hmac = new HMACSHA256(saltBytes);
        byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashBytes);

    }

    public static bool VerifyPassword(string password, string salt, string hashedPassword)
    {
        byte[] saltBytes = Convert.FromBase64String(salt);

        using var hmac = new HMACSHA256(saltBytes);
        byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        string computedHash = Convert.ToBase64String(hashBytes);

        return computedHash == hashedPassword;
    }

}
