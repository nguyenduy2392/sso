using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Sso.Core.Helpers;

public static class PasswordHelper
{
    public static string HashPassword(string password, string salt)
    {
        var hashed = KeyDerivation.Pbkdf2(
            password: password,
            salt: Encoding.UTF8.GetBytes(salt),
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000,
            numBytesRequested: 256 / 8
        );
        return Convert.ToBase64String(hashed);
    }

    public static bool VerifyPassword(string password, string hashedPassword, string salt)
    {
        return HashPassword(password, salt) == hashedPassword;
    }
}
