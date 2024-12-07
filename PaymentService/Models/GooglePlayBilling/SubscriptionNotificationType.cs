namespace PaymentService.Models.GooglePlayBilling;

/// <summary>
/// Enum representing the type of subscription notification.
/// </summary>
public enum SubscriptionNotificationType
{
    /// <summary>
    /// The subscription was recovered after the account was blocked.
    /// </summary>
    Recovered = 1,

    /// <summary>
    /// The active subscription was renewed.
    /// </summary>
    Renewed,

    /// <summary>
    /// The subscription was canceled either voluntarily or involuntarily.
    /// For voluntary cancellation, it is sent when the user cancels the subscription.
    /// </summary>
    Canceled,

    /// <summary>
    /// A new subscription was purchased.
    /// </summary>
    Purchased,

    /// <summary>
    /// The subscription is on hold (if enabled).
    /// </summary>
    OnHold,

    /// <summary>
    /// The subscription has entered a grace period (if enabled).
    /// </summary>
    InGracePeriod,

    /// <summary>
    /// The user restored their subscription from the "Play" > "Account" > "Subscriptions" menu.
    /// The subscription was canceled, but its term had not yet expired at the time of the user's restoration.
    /// See the Restorations section for more information.
    /// </summary>
    Restarted,

    /// <summary>
    /// The subscription price change was successfully confirmed by the user.
    /// </summary>
    PriceChangeConfirmed,

    /// <summary>
    /// The subscription renewal time was extended.
    /// </summary>
    Deferred,

    /// <summary>
    /// The subscription was paused.
    /// </summary>
    Paused,

    /// <summary>
    /// The subscription pause schedule was changed.
    /// </summary>
    PauseScheduleChanged,

    /// <summary>
    /// The subscription was revoked from the user before its term expired.
    /// </summary>
    Revoked,

    /// <summary>
    /// The subscription term has expired.
    /// </summary>
    Expired,

    /// <summary>
    /// A pending subscription transaction was canceled.
    /// </summary>
    PendingPurchaseCanceled
}
