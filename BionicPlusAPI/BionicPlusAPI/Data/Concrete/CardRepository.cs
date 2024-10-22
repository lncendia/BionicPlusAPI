using BionicPlusAPI.Data.Interfaces;
using BionicPlusAPI.Helpers;
using BionicPlusAPI.Models;
using DomainObjects.Pregnancy;
using DomainObjects.Pregnancy.Localizations;
using Microsoft.Extensions.Options;
using PregnancyDBMongoAccessor;
using PregnancyDBMongoAccessor.Infrastracture;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

namespace BionicPlusAPI.Data.Concrete
{
    public class CardRepository : ICardRepository
    {
        private readonly PregnancyDbAccessor _dbAccessor;
        private readonly IImageService _imageService;

        public CardRepository(IOptions<DbSettings> settings, IImageService imageService)
        {
            _dbAccessor = new PregnancyDbAccessor(settings.Value);
            _imageService = imageService;
        }

        public async Task<Card> AddCardAsync(Card card, LocalizationsLanguage language)
        {
            var addedCard = await _dbAccessor.AddCardAsync(card, language);
            return addedCard;
        }

        public async Task<bool> DeleteCardByIdAsync(string cardId)
        {
            var card = await _dbAccessor.GetCardByIdAsync(cardId, LocalizationsLanguage.Undifined);
            var updateStatus = await _dbAccessor.DeleteCardAsync(cardId);

            //Todo custom exceptions and status codes
            if (updateStatus.DeletedCount != 1)
            {
                return false;
            }
            if(card.Answers != null && card.Answers.Any())
            {
                foreach (var ans in card.Answers)
                {
                    if(ans.ImageInfo is not null)
                    {
                        await _imageService.DeleteImageAsync(ans.ImageInfo.ImageGuid + ans.ImageInfo.ImageType);
                    }
                }
            }
            if(card.ImgInfo != null && card.ImgInfo.ImageGuid != null && card.ImgInfo.ImageType != null)
            {
                await _imageService.DeleteImageAsync(card.ImgInfo.ImageGuid + card.ImgInfo.ImageType);
            }
            
            return true;
        }

        public async Task<List<Card>> GetBasicCardsByCategoryAsync(Category? category, LocalizationsLanguage language, bool withAnnotation)
        {
            var cards = await _dbAccessor.GetBasicCardsByCategory(category, language);
            var cardsWithAnnotation = new List<Card> ();
            if (withAnnotation)
            {
                foreach(var card in cards)
                {
                    cardsWithAnnotation.Add(await EnsureAnnotation(card, language));
                }
                return cardsWithAnnotation;
            }
            return cards;
        }

        public async Task<Card> GetCardByIdAsync(string id, LocalizationsLanguage language, bool withAnnotation)
        {
            var card = await _dbAccessor.GetCardByIdAsync(id, language);
            if(withAnnotation)
            {
                card = await EnsureAnnotation(card, language);
            }
            return card;
        }

        public async Task<Card> GetCardByTitleAsync(string title, LocalizationsLanguage language, bool withAnnotation)
        {
            var card = await _dbAccessor.GetCardByTitleAsync(title, language);
            if (withAnnotation)
            {
                card = await EnsureAnnotation(card, language);
            }
            return card;
        }

        public async Task<AnnotationResponse> GetCards(string? title, CardType? cardType, Category? category, LocalizationsLanguage language, bool withAnnotation)
        {
            var cards = await _dbAccessor.GetCardsAsync(title, cardType, category, language);
            var annotation = cards.ToAnnotation();
            return annotation;
        }


        public async Task<bool> UpdateCard(Card card, LocalizationsLanguage language)
        {
            var cardToUpdate = await GetCardByIdAsync(card.Id!, language, false);
            var updateStatus = await _dbAccessor.UpdateCard(card, language);

            if(cardToUpdate.ImgInfo != null 
                && cardToUpdate.ImgInfo.ImageGuid != null 
                && cardToUpdate.ImgInfo.ImageType != null 
                && card.ImgInfo != null 
                && card.ImgInfo.ImageGuid != null 
                && card.ImgInfo.ImageType != null)
            {
                await _imageService.DeleteImageAsync(cardToUpdate.ImgInfo.ImageGuid + cardToUpdate.ImgInfo.ImageType);
            }
            //Todo custom exceptions and status codes
            if (updateStatus.MatchedCount != 1)
            {
                return false;
            }
            return true;
        }


        private async Task<Card> EnsureAnnotation(Card card, LocalizationsLanguage language)
        {
            if(card.Answers != null && card.Answers.Any())
            {
                foreach(var ans in card.Answers)
                {
                    if(ans.RelatedCard?.Id != null)
                    {
                        var ansCard = await _dbAccessor.GetCardByIdAsync(ans.RelatedCard?.Id, language);
                        ans.RelatedCard = ansCard.ToAnnotation();
                    }
                }
            }
            if(card.Condition != null && card.Condition.DefaultCard != null)
            {
                var defaultCard = await _dbAccessor.GetCardByIdAsync(card.Condition.DefaultCard.Id, language);
                card.Condition.DefaultCard = defaultCard.ToAnnotation();
                if(card.Condition.Expressions != null && card.Condition.Expressions.Any())
                {
                    foreach(var expression in card.Condition.Expressions)
                    {
                        var expCard = await _dbAccessor.GetCardByIdAsync(expression.Card?.Id, language);
                        expression.Card = expCard.ToAnnotation();
                    }
                }
            }
            
            return card;
        }

    }
}
