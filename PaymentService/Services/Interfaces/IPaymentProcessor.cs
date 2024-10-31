using DomainObjects.Subscription;
using PaymentService.Models;

namespace PaymentService.Services.Interfaces;

public interface IPaymentProcessor
{
    Task VerifyAsync(Subscription subscription);
    Task ProcessAsync(string userId, string subscriptionId);
    Task CancelAsync(string userId);
}