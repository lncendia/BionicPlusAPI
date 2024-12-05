using DomainObjects.Subscription;

namespace AuthService.Models;

public class ProfileSubscription
{
    public ProfileSubscription(Subscription subscription)
    {
        Id = subscription.Id;
        ExpirationDate = subscription.ExpirationDate;
        Status = subscription.Status;
        GooglePurchaseToken = subscription.GooglePurchaseToken;
    }

    public string? Id { get; set; }
    public string? GooglePurchaseToken { get; set; }
    public DateTime ExpirationDate { get; set; }

    // Todo: В будущем нужно вынести эту переменную в метод получения подписки
    public SubscriptionStatus Status { get; set; }
}