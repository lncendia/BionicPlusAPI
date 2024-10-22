using DomainObjects.Pregnancy;
using DomainObjects.Pregnancy.Localizations;

namespace BionicPlusAPI.Data.Interfaces
{
    public interface ISurveyRepository
    {
        Task<ExtendedSurvey> GetSurveyByIdAsync(string id, LocalizationsLanguage language);
        Task DeleteSurveyByIdAsync(string id);
        Task<List<SurveyMy>> GetSurveysByTagAsync(SurveyTag tag, LocalizationsLanguage language);
        Task<(string, Card, bool)> PostAnswer(string surveyId, SurveyAnswer surveyAnswer, LocalizationsLanguage language);

        Task RateSurvey(string id, Rating rating);
        Task<bool> Rollback(string surveyId, string cardId);
    }
}
