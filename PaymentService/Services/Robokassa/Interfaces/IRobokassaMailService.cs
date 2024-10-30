namespace PaymentService.Services.Robokassa.Interfaces;

public interface IRobokassaMailService
{
    Task SendRecurrentPaymentEmailAsync(string mailAddress, DateTime nextSubDate, string sum, string subName);
    Task SendSuccessPaymentEmailAsync(string mailAddress, DateTime subStartDate, DateTime subEndDate, string subName);
    Task SendFailPaymentEmailAsync(string mailAddress);
}