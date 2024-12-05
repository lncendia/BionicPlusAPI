using DomainObjects.Pregnancy.Localizations;
using Hangfire;
using MailSenderLibrary.Interfaces;
using MailSenderLibrary.Models;
using PaymentService.Constants;
using PaymentService.Models.Emails;
using PaymentService.Services.Robokassa.Interfaces;

namespace PaymentService.Services.Robokassa.Implementations;

public class PaymentMailService : IPaymentMailService
{
    private readonly IEmailService _mailService;
    private readonly ILogger<PaymentMailService> _logger;

    public PaymentMailService(IEmailService mailService, ILogger<PaymentMailService> logger)
    {
        _mailService = mailService;
        _logger = logger;
    }

    [Queue("emails")]
    public async Task SendRecurrentPaymentEmail(RecurrentPaymentEmailModel recurrentPaymentEmailModel)
    {
        var emailContent = GeneratePaymentMessage(recurrentPaymentEmailModel);
        await _mailService.SendEmailAsync(emailContent);
    }

    [Queue("emails")]
    public async Task SendSuccessPaymentEmail(SuccessPaymentEmailModel successPaymentEmailModel)
    {
        var emailContent = GeneratePaymentMessage(successPaymentEmailModel);
            
        await _mailService.SendEmailAsync(emailContent);
    }

    [Queue("emails")]
    public async Task SendFailPaymentEmail(FailedPaymentEmailModel failedPaymentEmailModel)
    {
        var emailContent = GeneratePaymentMessage(failedPaymentEmailModel);
        await _mailService.SendEmailAsync(emailContent);
    }

    private EmailMessage GeneratePaymentMessage(PaymentEmailModel model)
    {
        var dateFormat = model.Language.GetDateFormat();
            
        switch (model)
        {
            case RecurrentPaymentEmailModel recurrentPaymentEmail:
                var recurrentContent = EmailTemplatesPath.PaymentNotification(recurrentPaymentEmail.Language);
                    
                recurrentContent = File.ReadAllText(recurrentContent)
                    .Replace("{{subName}}", recurrentPaymentEmail.SubName)
                    .Replace("{{sum}}", recurrentPaymentEmail.Sum)
                    .Replace("{{nextSubDate}}", recurrentPaymentEmail.NextSubDate.ToString(dateFormat))
                    .Replace("{{cancelSubscription}}", recurrentPaymentEmail.GenerateCancelSubscriptionUrl());
                    
                var recurrentMessage = MessageFormatter(recurrentPaymentEmail.Email, recurrentContent, "Уведомление о подписке");
                return recurrentMessage;
                
            case SuccessPaymentEmailModel successPaymentEmail:
                var successContent = EmailTemplatesPath.SubscriptionConfirmation(successPaymentEmail.Language);
                    
                successContent = File.ReadAllText(successContent)
                    .Replace("{{subName}}", successPaymentEmail.SubName)
                    .Replace("{{subStartDate}}", successPaymentEmail.SubStartDate.ToString(dateFormat))
                    .Replace("{{subEndDate}}", successPaymentEmail.SubEndDate.ToString(dateFormat))
                    .Replace("{{cancelSubscription}}", successPaymentEmail.GenerateCancelSubscriptionUrl());
                    
                var successMessage = MessageFormatter(successPaymentEmail.Email, successContent, "Уведомление о подписке");
                return successMessage;
                    
            case FailedPaymentEmailModel failedPaymentEmail:
                var failedContent = EmailTemplatesPath.PaymentFailure(failedPaymentEmail.Language);
                    
                failedContent = File.ReadAllText(failedContent)
                    .Replace("{{subName}}", failedPaymentEmail.SubName)
                    .Replace("{{price}}", failedPaymentEmail.Price)
                    .Replace("{{discount}}", failedPaymentEmail.Discount)
                    .Replace("{{totalPrice}}", failedPaymentEmail.GenerateCancelSubscriptionUrl())
                    .Replace("{{tryDate}}", failedPaymentEmail.TryDate.ToString(dateFormat));
                    
                var failedMessage = MessageFormatter(failedPaymentEmail.Email, failedContent, "Уведомление об ошибке");
                return failedMessage;
                    
            default:
                _logger.LogError($"Invalid model type {model}");
                throw new Exception("Invalid model type");
        }
    }

    private EmailMessage MessageFormatter(string email, string content, string subject) => new EmailMessage(new List<string> { email }, subject, content);
}