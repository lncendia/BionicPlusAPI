using Google.Apis.AndroidPublisher.v3;
using Google.Apis.AndroidPublisher.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using PaymentService.Models.GooglePlayBilling;
using PaymentService.Services.Interfaces;
using Subscription = DomainObjects.Subscription.Subscription;

namespace PaymentService.Services.GooglePlayBilling;

public class GooglePlayBillingProcessor : IPaymentProcessor, IDisposable
{
    private readonly AndroidPublisherService _androidPublisherService;
    private readonly string _appName;

    public GooglePlayBillingProcessor(string appName)
    {
        _appName = appName;

        var credential = GoogleCredential.FromFile("");

        credential = credential.CreateScoped(AndroidPublisherService.ScopeConstants.Androidpublisher);

        _androidPublisherService = new AndroidPublisherService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = appName
        });
    }

    public async Task VerifyAsync(SubscriptionEvent @event)
    {
        await _androidPublisherService.Purchases.Subscriptions
            .Acknowledge(new SubscriptionPurchasesAcknowledgeRequest(), @event.PackageName,
                @event.SubscriptionNotification.SubscriptionId, @event.SubscriptionNotification.PurchaseToken)
            .ExecuteAsync();
    }

    public Task VerifyAsync(Subscription subscription)
    {
        throw new NotImplementedException();
    }

    public Task ProcessAsync(string userId, string subscriptionId)
    {
        throw new NotImplementedException();
    }

    public async Task CancelAsync(string userId)
    {
        var x = await _androidPublisherService.Purchases.Subscriptions
            .Cancel()
            .Acknowledge(new SubscriptionPurchasesAcknowledgeRequest(), "", subscriptionId, token)
            .ExecuteAsync();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _androidPublisherService.Dispose();
    }
}