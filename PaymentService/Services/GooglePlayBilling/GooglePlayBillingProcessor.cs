using System.Globalization;
using DomainObjects.Subscription;
using Google.Apis.AndroidPublisher.v3;
using Google.Apis.AndroidPublisher.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using PaymentService.Models;
using PaymentService.Models.Emails;
using PaymentService.Models.GooglePlayBilling;
using PaymentService.Services.Interfaces;
using PaymentService.Services.Robokassa.Implementations.Recurring;
using PaymentService.Services.Robokassa.Interfaces;

namespace PaymentService.Services.GooglePlayBilling;

/// <summary>
/// Processor for handling Google Play billing events.
/// </summary>
public class GooglePlayBillingProcessor : IPaymentProcessor<GoogleCallback>, IDisposable
{
    /// <summary>
    /// Android Publisher Service for interacting with Google Play APIs.
    /// </summary>
    private readonly AndroidPublisherService _androidPublisherService;

    /// <summary>
    /// Service for managing subscriptions.
    /// </summary>
    private readonly ISubscriptionService _subscriptionService;

    /// <summary>
    /// Service for sending payment-related emails.
    /// </summary>
    private readonly IPaymentMailService _paymentMailService;

    /// <summary>
    /// Configuration for endpoints.
    /// </summary>
    private readonly EndpointsConfig _endpointsConfig;

    /// <summary>
    /// Service for handling recurring mail notifications.
    /// </summary>
    private readonly MailRecurringService _mailService;

    /// <summary>
    /// Service for managing user data.
    /// </summary>
    private readonly IUserService _userService;

    /// <summary>
    /// Service for managing plans.
    /// </summary>
    private readonly IPlanService _planService;

    /// <summary>
    /// The name of the application.
    /// </summary>
    private readonly string _appName;

    /// <summary>
    /// Constructor for initializing the GooglePlayBillingProcessor.
    /// </summary>
    /// <param name="subscriptionService">Service for managing subscriptions.</param>
    /// <param name="userService">Service for managing user data.</param>
    /// <param name="endpointsOptions">Configuration for endpoints.</param>
    /// <param name="planService">Service for managing plans.</param>
    /// <param name="paymentMailService">Service for sending payment-related emails.</param>
    /// <param name="mailService">Service for handling recurring mail notifications.</param>
    /// <param name="googlePlayOptions">Configuration for Google Play.</param>
    public GooglePlayBillingProcessor(ISubscriptionService subscriptionService, IUserService userService, IOptions<EndpointsConfig> endpointsOptions,
        IPlanService planService, IPaymentMailService paymentMailService, MailRecurringService mailService, IOptions<GooglePlayConfig> googlePlayOptions)
    {
        // Get the application name from the configuration
        var appName = googlePlayOptions.Value.AppName;
        
        // Load Google credentials from the specified file
        var credential = GoogleCredential.FromFile(googlePlayOptions.Value.CredentialsPath);

        // Initialize the application name
        _appName = appName;
        
        // Initialize the subscription service
        _subscriptionService = subscriptionService;
        
        // Initialize the user service
        _userService = userService;
        
        // Initialize the endpoints configuration
        _endpointsConfig = endpointsOptions.Value;
        
        // Initialize the plan service
        _planService = planService;
        
        // Initialize the payment mail service
        _paymentMailService = paymentMailService;
        
        // Initialize the mail service
        _mailService = mailService;

        // Create a scoped credential for Android Publisher API
        credential = credential.CreateScoped(AndroidPublisherService.ScopeConstants.Androidpublisher);

        // Initialize the Android Publisher Service
        _androidPublisherService = new AndroidPublisherService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = appName
        });
    }

    /// <summary>
    /// Initiates the checkout process for a subscription.
    /// </summary>
    /// <param name="planId">The ID of the plan.</param>
    /// <param name="promocode">The promo code.</param>
    /// <returns>A CheckoutModel containing the subscription ID.</returns>
    public async Task<CheckoutModel> CheckoutAsync(string planId, string? promocode)
    {
        // Create a new subscription
        var subscriptionId = await _subscriptionService.CreateSubscription
        (
            planId,
            PaymentServiceType.GooglePlay,
            "",
            promocode
        );

        // Return the checkout model with the subscription ID
        return new CheckoutModel { SubscriptionId = subscriptionId };
    }

    /// <summary>
    /// Verifies the subscription event.
    /// </summary>
    /// <param name="event">The subscription event.</param>
    /// <exception cref="ApplicationException">Thrown when the package name does not match the application name.</exception>
    public async Task VerifyAsync(GoogleCallback @event)
    {
        // Check if the package name matches the application name
        if (@event.PackageName != _appName)
            throw new ApplicationException(
                $"The package name {@event.PackageName} does not match the application name {_appName}");

        // If the subscription is purchased, acknowledge the purchase
        if (@event.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Purchased)
        {
            // Acknowledge the purchase
            await _androidPublisherService.Purchases.Subscriptions
                .Acknowledge(
                    new SubscriptionPurchasesAcknowledgeRequest(),
                    @event.PackageName,
                    @event.SubscriptionNotification.SubscriptionId,
                    @event.SubscriptionNotification.PurchaseToken
                )
                .ExecuteAsync();
        }
    }

    /// <summary>
    /// Processes the subscription event callback.
    /// </summary>
    /// <param name="callback">The subscription event callback.</param>
    /// <exception cref="ApplicationException">Thrown when the developer payload is invalid.</exception>
    public async Task ProcessAsync(GoogleCallback callback)
    {
        // Get the Google subscription details
        var googleSubscription = await _androidPublisherService.Purchases.Subscriptions
            .Get(
                callback.PackageName,
                callback.SubscriptionNotification.SubscriptionId,
                callback.SubscriptionNotification.PurchaseToken
            )
            .ExecuteAsync();

        // If the subscription is purchased
        if (callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Purchased)
        {
            // Split the developer payload to get user ID and subscription ID
            var developerSplit = googleSubscription.DeveloperPayload.Split('_', 2);

            // Check if the developer payload is valid
            if (developerSplit.Length != 2) throw new ApplicationException();

            // Get the user ID
            var userId = developerSplit[0];

            // Get the subscription ID
            var subscriptionId = developerSplit[1];

            // Get the subscription details
            var subscription = await _subscriptionService.GetSubscription(subscriptionId);

            // Set the Google purchase token for the subscription
            await _subscriptionService.SetGooglePurchaseToken(
                subscriptionId,
                googleSubscription.OrderId,
                callback.SubscriptionNotification.PurchaseToken
            );

            // Activate the subscription
            await _subscriptionService.ActivateSubscription(subscriptionId);

            // Set the subscription for the user
            await _subscriptionService.SetSubscription(userId, subscription.PlanId, subscriptionId);

            // Get the user details
            var user = await _userService.GetUserById(userId);

            // Get the plan details
            var plan = await _planService.GetPlan(subscription.PlanId);

            // Create a success payment email model
            var successEmail = new SuccessPaymentEmailModel
            {
                CancelSubscriptionBaseUrl = _endpointsConfig.CancelSubscriptionBaseUrl,
                UserId = userId,
                Hash = _userService.GenerateUserIdHash(userId),
                Email = user.Email,
                SubStartDate = subscription.CreationDate,
                SubEndDate = subscription.ExpirationDate,
                SubName = plan.Name
            };

            // Send the success payment email
            await _paymentMailService.SendSuccessPaymentEmail(successEmail);

            // Create a recurrent payment email model
            var recurrentPaymentEmail = new RecurrentPaymentEmailModel
            {
                CancelSubscriptionBaseUrl = _endpointsConfig.CancelSubscriptionBaseUrl,
                UserId = userId,
                Hash = _userService.GenerateUserIdHash(userId),
                Email = user.Email,
                NextSubDate = subscription.ExpirationDate,
                Sum = plan.Price.ToString(CultureInfo.InvariantCulture),
                SubName = plan.Name,
            };

            // Start the recurrent mailing job for the subscription
            _mailService.PlanMonthlyPaymentNotificationMail(recurrentPaymentEmail);
        }

        // If the subscription is canceled
        else if (callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Canceled)
        {
            // Get the subscription details by Google order ID
            var subscription = await _subscriptionService.GetSubscriptionByGoogleOrderId(googleSubscription.OrderId);

            // Deactivate the subscription
            await _subscriptionService.DeactivateSubscription(subscription.Id!);
        }

        // If the subscription is restarted
        else if (callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Restarted)
        {
            // Get the subscription details by Google order ID
            var subscription = await _subscriptionService.GetSubscriptionByGoogleOrderId(googleSubscription.OrderId);

            // Reactivate the subscription
            await _subscriptionService.ActivateSubscription(subscription.Id!);
        }

        // If the subscription is renewed
        else if (callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Renewed)
        {
            // Split the developer payload to get user ID and subscription ID
            var developerSplit = googleSubscription.DeveloperPayload.Split('_', 2);

            // Check if the developer payload is valid
            if (developerSplit.Length != 2) throw new ApplicationException();

            // Get the user ID
            var userId = developerSplit[0];

            // Get the subscription ID
            var subscriptionId = developerSplit[1];

            // Get the first subscription details
            var firstSubscription = await _subscriptionService.GetSubscription(subscriptionId);

            // Get the user details
            var user = await _userService.GetUserById(userId);

            // Create a new subscription
            var newSubscriptionId = await _subscriptionService.CreateSubscription(
                firstSubscription.PlanId,
                PaymentServiceType.GooglePlay
            );

            // Activate the new subscription
            await _subscriptionService.ActivateSubscription(subscriptionId);

            // Set the new subscription for the user
            await _subscriptionService.SetSubscription(userId, firstSubscription.PlanId, newSubscriptionId);

            // Get the new subscription details
            var newSubscription = await _subscriptionService.GetSubscription(newSubscriptionId);

            // Get the plan details
            var plan = await _planService.GetPlan(newSubscription.PlanId);

            // Create a success payment email model
            var successEmail = new SuccessPaymentEmailModel
            {
                CancelSubscriptionBaseUrl = _endpointsConfig.CancelSubscriptionBaseUrl,
                UserId = userId,
                Hash = _userService.GenerateUserIdHash(userId),
                Email = user.Email,
                SubStartDate = newSubscription.CreationDate,
                SubEndDate = newSubscription.ExpirationDate,
                SubName = plan.Name
            };

            // Send the success payment email
            await _paymentMailService.SendSuccessPaymentEmail(successEmail);
        }

        // If the subscription is revoked
        else if (callback.SubscriptionNotification.NotificationType == SubscriptionNotificationType.Revoked)
        {
            // Split the developer payload to get user ID
            var developerSplit = googleSubscription.DeveloperPayload.Split('_', 2);

            // Check if the developer payload is valid
            if (developerSplit.Length != 2) throw new ApplicationException();

            // Get the user ID
            var userId = developerSplit[0];

            // Get the subscription details by Google order ID
            var subscription = await _subscriptionService.GetSubscriptionByGoogleOrderId(googleSubscription.OrderId);

            // Deactivate the subscription
            await _subscriptionService.DeactivateSubscription(subscription.Id!);

            // Cancel the monthly mailing job
            _mailService.CancelMounthlyJob(userId);

            // Cancel the yearly mailing job
            _mailService.CancelYearlyJob(userId);

            // Ensure the subscription is removed and set a new free subscription
            await _subscriptionService.InsureSubscription(subscription.Id!);
        }
    }

    /// <summary>
    /// Cancels the subscription for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    public async Task CancelAsync(string userId)
    {
        // Get the active subscription ID for the user
        var subscriptionId = await _userService.GetActiveSubscription(userId);

        // If there is no active subscription, return
        if (subscriptionId == null) return;

        // Get the subscription details
        var subscription = await _subscriptionService.GetSubscription(subscriptionId);

        // Get the plan details
        var plan = await _planService.GetPlan(subscription.PlanId);
        
        // Cancel the subscription on Google Play
        await _androidPublisherService.Purchases.Subscriptions
            .Cancel(_appName, plan.GoogleSubscriptionId, subscription.GooglePurchaseToken)
            .ExecuteAsync();

        // Deactivate the subscription
        await _subscriptionService.DeactivateSubscription(subscriptionId);

        // Cancel the monthly mailing job
        _mailService.CancelMounthlyJob(userId);

        // Cancel the yearly mailing job
        _mailService.CancelYearlyJob(userId);
    }

    /// <summary>
    /// Disposes the resources used by the GooglePlayBillingProcessor.
    /// </summary>
    public void Dispose()
    {
        // Suppress finalization
        GC.SuppressFinalize(this);

        // Dispose the Android Publisher Service
        _androidPublisherService.Dispose();
    }
}