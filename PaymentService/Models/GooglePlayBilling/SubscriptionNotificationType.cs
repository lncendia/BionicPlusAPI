namespace PaymentService.Models.GooglePlayBilling;

public enum SubscriptionNotificationType
{
    /// <summary>
    /// Подписка была восстановлена после блокировки учетной записи.
    /// </summary>
    Recovered,

    /// <summary>
    /// Активная подписка продлена.
    /// </summary>
    Renewed,

    /// <summary>
    /// Подписка была добровольно или принудительно отменена.
    /// Для добровольной отмены отправляется, когда пользователь отменяет подписку.
    /// </summary>
    Canceled,

    /// <summary>
    /// Приобретена новая подписка.
    /// </summary>
    Purchased,

    /// <summary>
    /// Подписка заблокирована (если включена).
    /// </summary>
    OnHold,

    /// <summary>
    /// Для подписки вступил льготный период (если он включен).
    /// </summary>
    InGracePeriod,

    /// <summary>
    /// Пользователь восстановил свою подписку в меню «Play» > «Аккаунт» > «Подписки».
    /// Подписка была отменена, но срок ее действия еще не истек на момент восстановления пользователя.
    /// Дополнительную информацию см. в разделе Реставрации.
    /// </summary>
    Restarted,

    /// <summary>
    /// Изменение цены подписки успешно подтверждено пользователем.
    /// </summary>
    PriceChangeConfirmed,

    /// <summary>
    /// Время повторения подписки было продлено.
    /// </summary>
    Deferred,

    /// <summary>
    /// Подписка приостановлена.
    /// </summary>
    Paused,

    /// <summary>
    /// График приостановки подписки был изменен.
    /// </summary>
    PauseScheduleChanged,

    /// <summary>
    /// Подписка была отозвана у пользователя до истечения срока ее действия.
    /// </summary>
    Revoked,

    /// <summary>
    /// Срок действия подписки истек.
    /// </summary>
    Expired,

    /// <summary>
    /// Ожидающая транзакция подписки отменена.
    /// </summary>
    PendingPurchaseCanceled
}
