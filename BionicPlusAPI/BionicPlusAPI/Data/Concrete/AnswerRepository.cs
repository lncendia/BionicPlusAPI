using BionicPlusAPI.Data.Interfaces;
using DomainObjects.Pregnancy;
using DomainObjects.Pregnancy.Localizations;
using Microsoft.Extensions.Options;
using PregnancyDBMongoAccessor.Infrastracture;
using PregnancyDBMongoAccessor;
using PregnancyDBMongoAccessor.Models;
using BionicPlusAPI.Helpers;

namespace BionicPlusAPI.Data.Concrete
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly PregnancyDbAccessor _dbAccessor;
        private readonly IImageService _imageService;

        public AnswerRepository(IOptions<DbSettings> settings, IImageService imageService)
        {
            _dbAccessor = new PregnancyDbAccessor(settings.Value);
            _imageService = imageService;
        }

        public async Task<bool> DeleteAnswer(string cardId, Guid answerId)
        {
            var card = await _dbAccessor.GetCardByIdAsync(cardId, LocalizationsLanguage.Undifined);
            var updateStatus = await _dbAccessor.DeleteAnswerAsync(cardId, answerId);

            //Todo custom exceptions and status codes
            if (updateStatus.MatchedCount != 1)
            {
                return false;
            }
            await DeleteImage(card, answerId);
            
            return true;
        }

        public async Task<List<Answer>> GetAnswerByCardId(string id, LocalizationsLanguage language)
        {
            var answers = await _dbAccessor.GetAnswersByCardIdAsync(id, language);
            return answers;
        }

        public async Task<Answer> AddAnswer(string cardId, Answer answer, LocalizationsLanguage language)
        {
            answer = EnsureAnswerId(answer);
            var updateStatus = await _dbAccessor.AddAnswerAsync(cardId, answer, language);

            if (updateStatus.MatchedCount != 1)
            {
                throw new ArgumentException("Answer not added. Reason: Card not found or answer isn't correct");
            }
            return answer;
        }



        public async Task<bool> AssociateAnswer(string cardId, string relatedCardId, Guid answerId)
        {
            if (string.IsNullOrWhiteSpace(relatedCardId))
            {
                return false;
            }

            var updateStatus = await _dbAccessor.AssociateAnswer(cardId, relatedCardId, answerId);

            //Todo custom exceptions and status codes
            if (updateStatus.MatchedCount != 1)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateAnswer(string cardId, Answer answer, LocalizationsLanguage language)
        {
            if (answer.ImageInfo != null)
            {
                var card = await _dbAccessor.GetCardByIdAsync(cardId, LocalizationsLanguage.Undifined);
                await DeleteImage(card, answer.Id ?? Guid.Empty);
            }

            var updateStatus = await _dbAccessor.UpdateAnswer(cardId, answer, language);
            
            

            //Todo custom exceptions and status codes
            if (updateStatus.MatchedCount != 1)
            {
                return false;
            }

            return true;
        }

        private async Task DeleteImage(Card card, Guid answerId)
        {
            if (card.Answers != null && card.Answers.Any())
            {
                var ans = card.Answers.FirstOrDefault(a => a.Id == answerId);
                if (ans != null)
                {
                    if (ans.ImageInfo is not null)
                    {
                        await _imageService.DeleteImageAsync(ans.ImageInfo.ImageGuid + ans.ImageInfo.ImageType);
                    }
                }
            }
        }

        private Answer EnsureAnswerId(Answer answer)
        {
            answer.Id = Guid.NewGuid();
            return answer;
        }
    }
}
