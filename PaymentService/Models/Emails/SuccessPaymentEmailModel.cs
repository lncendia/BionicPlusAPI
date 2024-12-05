namespace PaymentService.Models.Emails;

public class SuccessPaymentEmailModel: PaymentEmailModel
{
    public string SubName { get; set; } = string.Empty;  
    public DateTime SubStartDate { get; set; }
    public DateTime SubEndDate { get; set; }
}