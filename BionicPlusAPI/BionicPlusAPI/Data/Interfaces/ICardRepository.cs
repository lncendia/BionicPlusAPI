using BionicPlusAPI.Models;
using DomainObjects.Pregnancy;
using DomainObjects.Pregnancy.Localizations;

namespace BionicPlusAPI.Data.Interfaces
{
    public interface ICardRepository
    {
        Task<Card> GetCardByIdAsync(string id, LocalizationsLanguage language, bool withAnnotation);
        Task<List<Card>> GetBasicCardsByCategoryAsync(Category? category, LocalizationsLanguage language, bool withAnnotation);
        Task<AnnotationResponse> GetCards(string? title, CardType? cardType, Category? category, LocalizationsLanguage language, bool withAnnotation);
        Task<Card> GetCardByTitleAsync(string title, LocalizationsLanguage language, bool withAnnotation);
        Task<bool> DeleteCardByIdAsync(string id);
        Task<Card> AddCardAsync(Card card, LocalizationsLanguage language);
        Task<bool> UpdateCard(Card card, LocalizationsLanguage language);
    }
}
