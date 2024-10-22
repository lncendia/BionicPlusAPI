using DomainObjects.Pregnancy.Localizations;
using DomainObjects.Pregnancy;
using PregnancyDBMongoAccessor.Models;

namespace BionicPlusAPI.Data.Interfaces
{
    public interface IAnswerRepository
    {
        Task<List<Answer>> GetAnswerByCardId(string id, LocalizationsLanguage language);
        Task<Answer> AddAnswer(string cardId, Answer answer, LocalizationsLanguage language);
        Task<bool> UpdateAnswer(string cardId, Answer answer, LocalizationsLanguage language);
        Task<bool> AssociateAnswer(string cardId, string relatedCardId, Guid answerId);
        Task<bool> DeleteAnswer(string cardId, Guid answerId);
    }
}
