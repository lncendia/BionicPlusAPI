using DomainObjects.Subscription;

namespace AuthService.Models;

public class ProfileSubscription
{
    public ProfileSubscription(Subscription subscription)
    {
        Id = subscription.Id;
        ExpirationDate = subscription.ExpirationDate;
    }

    public string? Id { get; set; }
    public DateTime ExpirationDate { get; set; }
}