namespace AuthService.Services.Interfaces;

public interface ICaptchaValidator
{
    Task<bool> Validate(string captureToken);
}