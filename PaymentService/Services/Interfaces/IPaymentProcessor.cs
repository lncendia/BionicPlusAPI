using DomainObjects.Subscription;

namespace PaymentService.Services.Interfaces;

public interface IPaymentProcessor
{
    Task VerifyAsync(Subscription subscription);
    Task ProcessAsync();
    Task CancelAsync(string userId);
}