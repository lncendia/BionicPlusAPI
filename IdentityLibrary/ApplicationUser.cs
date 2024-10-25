using AspNetCore.Identity.MongoDbCore.Models;
using DomainObjects.Pregnancy;
using DomainObjects.Pregnancy.UserProfile;
using DomainObjects.Subscription;
using MongoDbGenericRepository.Attributes;

namespace IdentityLibrary
{
    [CollectionName("users")]
    public class ApplicationUser : MongoIdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public TemperatureUnits TemperatureUnits { get; set; } = TemperatureUnits.C;
        public List<Category>? Statuses { get; set; }
        public MeasureSystem MeasureSystem { get; set; } = MeasureSystem.Metric;
        public double? Height { get; set; }
        public BillingProfile? BillingProfile { get; set; }
        public UserAgreement UserAgreement { get; set; } = new UserAgreement();
    }
}
