namespace PaymentService.Services.Robokassa.Interfaces;

public interface IPaymentMailService
{
    Task SendRecurrentPaymentEmailAsync(string mailAddress, DateTime nextSubDate, string sum, string subName);
    Task SendSuccessPaymentEmailAsync(string mailAddress, DateTime subStartDate, DateTime subEndDate, string subName);
    Task SendFailPaymentEmailAsync(string mailAddress);
}