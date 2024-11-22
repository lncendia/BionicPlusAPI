namespace PaymentService.Models.Emails;

public class RecurrentPaymentEmailModel: PaymentEmailModel
{
    public string SubName { get; set; } = string.Empty;
    public string Sum { get; set; } = string.Empty;
    public DateTime NextSubDate { get; set; }
}