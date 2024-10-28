namespace AuthService.Models;

public enum AuthErrorCode
{
    Undefined = 0,
    PasswordTooShort = 1,
    PasswordRequiresNonAlphanumeric = 2,
    PasswordRequiresUpper = 3,
    PasswordIncorrect = 4,
    UserNotExists = 5,
    PasswordMismatch = 6,
    InvalidToken = 7,
    CaptchaNotPassed = 8,
    SecurityStampOutdated = 9
}
