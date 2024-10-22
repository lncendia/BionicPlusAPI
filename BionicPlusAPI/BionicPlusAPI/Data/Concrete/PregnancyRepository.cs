using BionicPlusAPI.Data.Interfaces;
using DomainObjects.Pregnancy;
using Microsoft.Extensions.Options;
using PregnancyDBMongoAccessor;
using PregnancyDBMongoAccessor.Infrastracture;
using System.Security.Claims;
using ZstdSharp.Unsafe;

namespace BionicPlusAPI.Data.Concrete
{
    public class PregnancyRepository : IPregnancy
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly PregnancyDbAccessor _dbAccessor;

        public PregnancyRepository(IOptions<DbSettings> settings, IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor;
            _dbAccessor = new PregnancyDbAccessor(settings.Value);
        }

        public async Task<Pregnancy> AddPregnancy(Pregnancy pregnancy)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var createdPregnancy = await _dbAccessor.AddPregnancy(pregnancy, Guid.Parse(userId));

            return createdPregnancy;
        }

        public async Task AddWeighing(string id, Weighing weighing)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _dbAccessor.AddWeighing(id, Guid.Parse(userId), weighing);
        }

        public async Task ArchivePregnancy(string id)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _dbAccessor.ArchivePregnancy(id, Guid.Parse(userId));
        }

        public async Task ChangeWeighing(string id, Weighing weighing)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _dbAccessor.ChangeWeighing(id, Guid.Parse(userId), weighing);
        }

        public async Task DeletePregnancy(string id)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _dbAccessor.DeletePregnancy(id, Guid.Parse(userId));
        }

        public async Task DeleteWeighing(string id, DateOnly weighingDate)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

            await _dbAccessor.DeleteWeighing(id, Guid.Parse(userId), weighingDate);
        }

        public async Task<List<Pregnancy>> GetPregnancies(bool isActive)
        {
            var userId = _httpContext.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pregnancy = await _dbAccessor.GetPregnancies(isActive, Guid.Parse(userId));
            return pregnancy;
        }
    }
}
