namespace PaymentService.Models.Emails;

/// <summary>
/// Модель для формирования email-письма об успешном пейменте
/// </summary>
public class SuccessPaymentEmailModel: PaymentEmailModel
{
    public string SubName { get; set; } = string.Empty;  
    public DateTime SubStartDate { get; set; }
    public DateTime SubEndDate { get; set; }
}