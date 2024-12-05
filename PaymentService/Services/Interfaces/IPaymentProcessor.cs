using PaymentService.Models;

namespace PaymentService.Services.Interfaces;

public interface IPaymentProcessor<in T>
{
    Task<CheckoutModel> CheckoutAsync(string planId, string? promocode);
    Task VerifyAsync(T callback);
    Task ProcessAsync(T callback);
    Task CancelAsync(string userId);
}