using PaymentService.Models.Emails;

namespace PaymentService.Services.Robokassa.Interfaces;

public interface IRobokassaMailService
{
    Task SendRecurrentPaymentEmail(RecurrentPaymentEmailModel paymentModel);
    Task SendSuccessPaymentEmail(SuccessPaymentEmailModel paymentEmailModel);
    Task SendFailPaymentEmail(FailedPaymentEmailModel failedPaymentEmailModel);
}
