using DomainObjects.Subscription;

namespace AuthService.Models;

public class ProfileSubscription
{
    public ProfileSubscription(Subscription subscription)
    {
        Id = subscription.Id;
        ExpirationDate = subscription.ExpirationDate;
        Status = subscription.Status;
    }

    public string? Id { get; set; }
    public DateTime ExpirationDate { get; set; }
    
    // Todo: В будущем нужно вынести эту переменную в метод получения подписки
    public SubscriptionStatus Status { get; set; }
}