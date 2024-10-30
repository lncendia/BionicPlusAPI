namespace AuthService.Services.Interfaces;

/// <summary>
/// Интерфейс для валидации капчи
/// </summary>
public interface ICaptchaValidator
{
    /// <summary>
    /// Валидирует капчу асинхронно
    /// </summary>
    /// <param name="captureToken">Токен капчи для валидации</param>
    /// <returns>Задача, возвращающая true, если капча валидна, иначе false</returns>
    Task ValidateAsync(string captureToken);
}
