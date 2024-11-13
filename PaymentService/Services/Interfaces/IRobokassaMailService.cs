using PaymentService.Models.Emails;

namespace PaymentService.Services.Interfaces;

public interface IRobokassaMailService
{
    Task SendRecurrentPaymentEmail(RecurrentPaymentEmailModel paymentModel);
    Task SendSuccessPaymentEmail(SuccessPaymentEmailModel paymentEmailModel);
    Task SendFailPaymentEmail(FailedPaymentEmailModel failedPaymentEmailModel);
}
