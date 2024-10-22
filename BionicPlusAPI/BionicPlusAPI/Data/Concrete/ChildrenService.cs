using BionicPlusAPI.Data.Interfaces;
using DomainObjects.Pregnancy.Children;
using Microsoft.Extensions.Options;
using PregnancyDBMongoAccessor;
using PregnancyDBMongoAccessor.Infrastracture;
using System.Security.Claims;

namespace BionicPlusAPI.Data.Concrete
{
    public class ChildrenService : IChildrenService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly PregnancyDbAccessor _dbAccessor;

        public ChildrenService(IOptions<DbSettings> settings, IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor;
            _dbAccessor = new PregnancyDbAccessor(settings.Value);
        }

        public async Task<string> AddChild(Child child)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var id = await _dbAccessor.AddChild(child, Guid.Parse(userId));

            return id;
        }

        public async Task AddMeasurement(string childId, ChildMeasurement measurement)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _dbAccessor.AddMeasurement(childId, Guid.Parse(userId), measurement);
        }

        public async Task ArchiveChild(string id)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _dbAccessor.ArchiveChild(id, Guid.Parse(userId));
        }

        public async Task DeleteChild(string id)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _dbAccessor.DeleteChild(id, Guid.Parse(userId));
        }

        public async Task DeleteMeasurement(string childId, DateOnly date)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _dbAccessor.DeleteMeasurement(childId, date, Guid.Parse(userId));
        }

        public async Task<List<Child>> GetChildren(bool isActive)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var children = await _dbAccessor.GetChildren(isActive, Guid.Parse(userId));
            return children;
        }

        public async Task UpdateMeasurement(string childId, DateOnly date, ChildMeasurement measurement)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _dbAccessor.UpdateMeasurement(childId, date, Guid.Parse(userId), measurement);
        }
    }
}
