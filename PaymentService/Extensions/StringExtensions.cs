using System.Security.Cryptography;
using System.Text;

namespace PaymentService.Extensions;

/// <summary>
/// Extension methods for strings
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Computes the HMAC-SHA256 hash of a string.
    /// </summary>
    /// <param name="message">The string to hash</param>
    /// <param name="key">The secret key to use for the HMAC</param>
    /// <returns>The HMAC-SHA256 hash as a Base64 encoded string</returns>
    public static string ComputeHmac(this string message, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
        return Convert.ToBase64String(hashBytes);
    }
}