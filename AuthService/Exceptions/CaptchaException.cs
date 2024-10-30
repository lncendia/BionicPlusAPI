namespace AuthService.Exceptions;

public class CaptchaSecretException : CaptchaException
{
    public CaptchaSecretException(string message) : base(message) { }
}

public class CaptchaResponseException : CaptchaException
{
    public CaptchaResponseException(string message) : base(message) { }
}

public class CaptchaException : Exception
{
    public CaptchaException(string message) : base(message) { }
}
