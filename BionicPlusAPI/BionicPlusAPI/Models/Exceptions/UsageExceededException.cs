namespace BionicPlusAPI.Models.Exceptions
{
    public class UsageExceededException : Exception
    {
        public UsageExceededException(string message) : base(message)
        {
            
        }
    }
}
