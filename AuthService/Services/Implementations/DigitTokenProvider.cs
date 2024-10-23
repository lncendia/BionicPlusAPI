using System.Globalization;
using IdentityLibrary;
using Microsoft.AspNetCore.Identity;

namespace AuthService.Services.Implementations;

public class DigitTokenProvider : PhoneNumberTokenProvider<ApplicationUser>
{
    public static string FourDigitPhone = "4DigitPhone";
    public const string FOUR_DIGIT_EMAIL = "4DigitEmail";

    public override Task<bool> CanGenerateTwoFactorTokenAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        return Task.FromResult(false);
    }

    public override async Task<string> GenerateAsync(string purpose, UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        var token = new SecurityToken(await manager.CreateSecurityTokenAsync(user));
        var modifier = await GetUserModifierAsync(purpose, manager, user);
        var code = Rfc6238AuthenticationService.GenerateCode(token, modifier, 4).ToString("D4", CultureInfo.InvariantCulture);

        return code;
    }
    public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<ApplicationUser> manager, ApplicationUser user)
    {
        if (!int.TryParse(token, out var code))
        {
            return false;
        }
        var securityToken = new SecurityToken(await manager.CreateSecurityTokenAsync(user));
        var modifier = await GetUserModifierAsync(purpose, manager, user);
        var valid = Rfc6238AuthenticationService.ValidateCode(securityToken, code, modifier, token.Length);
        return valid;
    }
}