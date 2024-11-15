using System.Globalization;
using DomainObjects.Subscription;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.AndroidPublisher.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using PaymentService.Models;
using PaymentService.Models.Emails;
using PaymentService.Models.GooglePlayBilling;
using PaymentService.Services.Interfaces;
using PaymentService.Services.Robokassa.Implementations.Recurring;
using PaymentService.Services.Robokassa.Interfaces;

namespace PaymentService.Services.GooglePlayBilling;

public class GooglePlayBillingProcessor : IPaymentProcessor<SubscriptionEvent>, IDisposable
{
    private readonly AndroidPublisherService _androidPublisherService;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IPaymentMailService _paymentMailService;
    private readonly EndpointsConfig _endpointsConfig;
    private readonly MailRecurringService _mailService;
    private readonly IUserService _userService;
    private readonly IPlanService _planService;
    private readonly string _appName;

    public GooglePlayBillingProcessor(string appName, ISubscriptionService subscriptionService, IUserService userService, EndpointsConfig endpointsConfig, IPlanService planService, IPaymentMailService paymentMailService, MailRecurringService mailService)
    {
        _appName = appName;
        _subscriptionService = subscriptionService;
        _userService = userService;
        _endpointsConfig = endpointsConfig;
        _planService = planService;
        _paymentMailService = paymentMailService;
        _mailService = mailService;

        var credential = GoogleCredential.FromFile("/app/googleCredentials.json");

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
    
    /*
     * Заметки:
     * - гугловские подписки ид можно вынести к обычным в бд просто YearlyGoogle or MonthGoogle
     * - бесплатная подписка идет по умолчанию одна
     * - в subscriptionEvent нужно будет прокидывать @event.UserId = userId до вызова
     * - удаляем воркеров рассылки только при истечении или отзыва подписки? 
     */

    public async Task ProcessAsync(SubscriptionEvent callback)
    {
        // Получаем активную подписку пользователя
        var activeSubscriptionId = await _userService.GetActiveSubscription(callback.UserId);

        // Получаем пользователя
        var user = await _userService.GetUserById(callback.UserId);
        
        if (activeSubscriptionId == null)
        {
            // todo: обработать исключение
        }

        if (callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Purchased)
        {
            // todo: откуда из уведомления взять название подписки
            var planId = "monthly_google_mock";
            
            // Создаем новую подписку
            var subscriptionId = await _subscriptionService.CreateSubscription(
                planId,
                PaymentServiceType.GooglePlay);

            // Получаем созданную подписку
            var subscription = await _subscriptionService.GetSubscription(subscriptionId);

            // Получаем план
            var plan = await _planService.GetPlan(planId);

            var successEmail = new SuccessPaymentEmailModel
            {
                CancelSubscriptionBaseUrl = _endpointsConfig.CancelSubscriptionBaseUrl,
                UserId = callback.UserId,
                Hash = _userService.GenerateUserIdHash(callback.UserId),
                Email = user.Email,
                SubStartDate = subscription.CreationDate,
                SubEndDate = subscription.ExpirationDate,
                SubName = plan.Name
            };

            // Отправляем письмо об успешной подписке
            await _paymentMailService.SendSuccessPaymentEmail(successEmail);
            
            // Активируем подписку
            await _subscriptionService.ActivateSubscription(subscriptionId);

            // Устанавливаем подписку пользователю
            await _subscriptionService.SetSubscription(callback.UserId, callback.SubscriptionNotification.SubscriptionId, subscriptionId);

            // Формируем письмо рекуррентной отправки письма
            var recurrentPaymentEmail = new RecurrentPaymentEmailModel
            {
                CancelSubscriptionBaseUrl = _endpointsConfig.CancelSubscriptionBaseUrl,
                UserId = callback.UserId,
                Hash = _userService.GenerateUserIdHash(callback.UserId),
                Email = user.Email,
                NextSubDate = subscription.ExpirationDate,
                Sum = plan.Price.ToString(CultureInfo.InvariantCulture),
                SubName = plan.Name,
            };
            
            // Запускаем рекуррентную рассылку о подписке 
            _mailService.PlanMonthlyPaymentNotificationMail(recurrentPaymentEmail);
        }

        // Подписка была отменена пользователем
        else if (callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Canceled)
        {
            // Деактивируем подписку
            await _subscriptionService.DeactivateSubscription(activeSubscriptionId!);
        }
        
        // Подписка была восстановлена
        else if(callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Restarted)
        {
            // Получаем активную подписку пользователя
            var subscriptionId = await _userService.GetActiveSubscription(activeSubscriptionId!);
            
            // Снова активируем подписку
            await _subscriptionService.ActivateSubscription(subscriptionId!);
        }
        
        // Подписка продлена
        else if(callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Renewed)
        {
            // Получаем план предыдущей подписки
            var currentSubscription = await  _subscriptionService.GetSubscription(activeSubscriptionId!);
            
            // Создаем новую подписку
            var subscriptionId = await _subscriptionService.CreateSubscription(currentSubscription.PlanId, PaymentServiceType.GooglePlay);
            
            // Получаем созданную подписку
            var subscription = await _subscriptionService.GetSubscription(activeSubscriptionId!);
            
            // Устанавливаем созданную подписку
            await _subscriptionService.SetSubscription(callback.UserId, currentSubscription.PlanId, subscriptionId);
            
            // Получаем план
            var plan = await _planService.GetPlan(currentSubscription.PlanId);
            
            // Формируем письмо
            var successEmail = new SuccessPaymentEmailModel
            {
                CancelSubscriptionBaseUrl = _endpointsConfig.CancelSubscriptionBaseUrl,
                UserId = callback.UserId,
                Hash = _userService.GenerateUserIdHash(callback.UserId),
                Email = user.Email,
                SubStartDate = subscription.CreationDate,
                SubEndDate = subscription.ExpirationDate,
                SubName = plan.Name
            };

            // Отправляем сообщение об емэйле
            await _paymentMailService.SendSuccessPaymentEmail(successEmail);

            // Активируем созданную подписку
            await _subscriptionService.ActivateSubscription(subscriptionId);
        }
        
        // Подписка была отозвана у пользователя
        else if(callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Revoked)
        {
            // Удаляем подписку и устанавливаем новую бесплатную
            await _subscriptionService.InsureSubscription(activeSubscriptionId!);
            _subscriptionService.CancelPaymentReccuringJobs(callback.UserId);
        }
        
        // Подписка истекла
        else if(callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Expired)
        {
            // Удаляем подписку и устанавливаем новую бесплатную
            await _subscriptionService.InsureSubscription(activeSubscriptionId!);
            _subscriptionService.CancelPaymentReccuringJobs(callback.UserId);
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