using Google.Apis.AndroidPublisher.v3;
using Google.Apis.AndroidPublisher.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using PaymentService.Models.GooglePlayBilling;
using PaymentService.Services.Interfaces;
using Subscription = DomainObjects.Subscription.Subscription;

namespace PaymentService.Services.GooglePlayBilling;

public class GooglePlayBillingProcessor : IPaymentProcessor<SubscriptionEvent>, IDisposable
{
    private readonly ISubscriptionService _subscriptionService;
    private readonly AndroidPublisherService _androidPublisherService;
    private readonly string _appName;

    public GooglePlayBillingProcessor(string appName, ISubscriptionService subscriptionService)
    {
        _appName = appName;
        _subscriptionService = subscriptionService;

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
        if(@event.PackageName != _appName) throw new ApplicationException($"The package name {@event.PackageName} does not match the application name {_appName}");
        
        if (@event.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Purchased)
        {
            await _androidPublisherService.Purchases.Subscriptions
                .Acknowledge(new SubscriptionPurchasesAcknowledgeRequest(), @event.PackageName,
                    @event.SubscriptionNotification.SubscriptionId, @event.SubscriptionNotification.PurchaseToken)
                .ExecuteAsync();
        }
    }

    public async Task ProcessAsync(SubscriptionEvent callback)
    {
        if (callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Purchased)
        {
            //устанавливаем новую подкиску
            //план -> callback.SubscriptionNotification.SubscriptionId == "yearly" ? true : false;
        }
        else if (callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Canceled)
        {
            // деактивируем подписку
            _subscriptionService.DeactivateSubscription();
        }
        else if(callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Restarted)
        {
            _subscriptionService.ActivateSubscription();
            // снова активируем подписку
        }
        else if(callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Renewed)
        {
            // автопродление, создаем новую подписку активированную
        }
        else if(callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Revoked)
        {
            // удаляем подписку и ставим бесплатную
        }
        else if(callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Expired)
        {
            
        }
    }

    public async Task CancelAsync(string userId)
    {
        var x = await _androidPublisherService.Purchases.Subscriptions
            .Cancel()
            .ExecuteAsync();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _androidPublisherService.Dispose();
    }
}