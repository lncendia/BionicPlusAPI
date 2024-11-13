namespace PaymentService.Models.Emails;

/// <summary>
/// Модель для формирования email-письма о рекуррентном списании
/// </summary>
public class RecurrentPaymentEmailModel: PaymentEmailModel
{
    public string SubName { get; set; } = string.Empty;
    public string Sum { get; set; } = string.Empty;
    public DateTime NextSubDate { get; set; }
}