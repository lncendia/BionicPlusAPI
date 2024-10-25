# Изменения эндпоитнтов

## Этап 1

### RefreshToken
Теперь возвращает в теле модель со следующими полями:
```
    public bool Success { get; init; }
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public int RefreshTokenExpiryTime { get; init; }
```

### ForgotPassword
Теперь вместо `query` принимает `body` с двумя строковыми полями: `password` и `captcha`
