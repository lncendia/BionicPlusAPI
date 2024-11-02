# Изменения эндпоитнтов

## Этап 1

### Auth: RefreshToken
Теперь возвращает в теле модель со следующими полями:
```
    public bool Success { get; init; }
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
    public int RefreshTokenExpiryTime { get; init; }
```

### Auth: ForgotPassword
Теперь вместо `query` принимает `body` с двумя строковыми полями: `password` и `captcha`

## Этап 2

## Этап 3

### Auth: Profile
Теперь в ответе тела присутствует информация о подписке

```
"subscription": {
    "id": "671b72c013760afe9738cb83",
    "expirationDate": "2024-11-25T10:28:16.2182903Z"
  }
```

### PaymentService
Появился новый роут для отмены подписки по ссылке:
```
/payment/api/Subscription/cancel?userId={userId}&hash={hash}language={langEnum}
```

### Docker-compose
В docker-compose.yml появились новые поля для PaymentService:
1. Время до истечения срока жизни открытой подписки (созданной, не оплаченной)
2. Соль для шифрования хеша для проверки пользователя при отмене подписки из почтового письма

```
- SUBSCRIPTIONCONFIG__EXPIREDTIMEINMINUTS=30
- ENCRYPTIONCONFIG__USERIDSIGNINGKEY=${INPUT_JWT_ISSUER_SIGNING_KEY}
```

### Subscription.Statuses
Новые статусы подписки (добавился Failed): 
```
Active,
Pending,
Inactive,
Failed
```
