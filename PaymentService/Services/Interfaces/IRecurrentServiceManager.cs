namespace PaymentService.Services.Interfaces
{
    public interface IRecurrentServiceManager
    {
        public bool CancelAllRecurrentJobByUserId(string userId);
        public bool CancelRecurringPaymentsJobByUserId(string userId);
    }
}
