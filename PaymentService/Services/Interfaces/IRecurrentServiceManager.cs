namespace PaymentService.Services.Interfaces
{
    public interface IRecurrentServiceManager
    {
        public bool CancelAllRecurrentJobByUserId(string userId);
    }
}
