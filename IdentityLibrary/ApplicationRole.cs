using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace IdentityLibrary
{
    [CollectionName("roles")]
    public class ApplicationRole : MongoIdentityRole<Guid>
    {
    }
}
