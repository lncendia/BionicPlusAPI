using DomainObjects.Pregnancy;

namespace BionicPlusAPI.Data.Interfaces
{
    public interface IPregnancy
    {
        public Task<Pregnancy> AddPregnancy(Pregnancy pregnancy);
        public Task AddWeighing(string id, Weighing weighing);
        public Task ChangeWeighing(string id, Weighing weighing);
        public Task DeleteWeighing(string id, DateOnly weighingDate);
        public Task ArchivePregnancy(string id);
        public Task DeletePregnancy(string id);
        public Task<List<Pregnancy>> GetPregnancies(bool isActive);
    }
}
