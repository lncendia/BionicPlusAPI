
using DomainObjects.Pregnancy.Children;

namespace BionicPlusAPI.Data.Interfaces
{
    public interface IChildrenService
    {
        Task<string> AddChild(Child child);
        Task AddMeasurement(string childId, ChildMeasurement measurement);
        Task ArchiveChild(string id);
        Task DeleteChild(string id);
        Task DeleteMeasurement(string childId, DateOnly date);
        Task<List<Child>> GetChildren(bool isActive);
        Task UpdateMeasurement(string childId, DateOnly date, ChildMeasurement measurement);
    }
}
