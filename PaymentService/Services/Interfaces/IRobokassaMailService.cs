using MailSenderLibrary.Models;

namespace PaymentService.Services.Interfaces
{
    public interface IRobokassaMailService
    {
        void SendRecurrentPaymentEmail(string mailAddress, DateTime nextSubDate, string sum, string subName);
        void SendSuccessPaymentEmail(string mailAddress, DateTime subStartDate, DateTime subEndDate, string subName);
        void SendFailPaymentEmail(string mailAddress);
    }
}
