using BionicPlusAPI.Data.Interfaces;
using DomainObjects.Pregnancy;
using Microsoft.Extensions.Options;
using PregnancyDBMongoAccessor.Infrastracture;
using PregnancyDBMongoAccessor;
using DomainObjects.Pregnancy.Localizations;
using System.Data;
using MongoDB.Driver.Core.Authentication;
using MongoDB.Driver.Core.Servers;
using BionicPlusAPI.Helpers;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Runtime.CompilerServices;
using BionicPlusAPI.Services.Interfaces;
using Amazon.S3.Model.Internal.MarshallTransformations;
using BionicPlusAPI.Models.Exceptions;
using DomainObjects.Subscription;

namespace BionicPlusAPI.Data.Concrete
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly PregnancyDbAccessor _dbAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUsageService _usageService;

        public SurveyRepository(IOptions<DbSettings> settings, IHttpContextAccessor httpContextAccessor, IUsageService usageService)
        {
            _dbAccessor = new PregnancyDbAccessor(settings.Value);
            _httpContextAccessor = httpContextAccessor;
            _usageService = usageService;
        }

        public async Task<ExtendedSurvey> GetSurveyByIdAsync(string id, LocalizationsLanguage language)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier); 
            var survey = await _dbAccessor.GetSurveyByIdAsync(id, userId);
            var associatedCardIds = new List<string>();

            if(survey.Answers != null)
            {
                foreach (var ans in survey.Answers)
                {
                    associatedCardIds.Add(ans.CardId);
                }
            }
            var associatedCards = await _dbAccessor.PullAssociatedCards(associatedCardIds, language);

            var extendedSurvey = EnsureAnnotation(survey, associatedCards);

            return extendedSurvey;
        }

        public async Task DeleteSurveyByIdAsync(string id)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _dbAccessor.DeleteSurveyByIdAsync(id, userId);
        }


        public async Task RateSurvey(string id, Rating rating)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _dbAccessor.RateSurveyAsync(id, userId, rating);
        }

        public async Task<bool> Rollback(string surveyId, string cardId)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(await CheckRollbackUsage(userId))
            {
                var newUsage = await _usageService.DecrementUsageAsync(LimitKind.Rollback, userId);

                if(newUsage != null)
                {
                    return await _dbAccessor.Rollback(surveyId, cardId, userId);
                }
            }
            else
            {
                throw new UsageExceededException("There is no rollback left");
            }

            return false;
        }

        private ExtendedSurvey EnsureAnnotation(Survey survey, List<Card> associatedCard)
        {
            var extendedAnswers = new List<ExtendedSurveyAnswer>();
            if (survey.Answers != null)
            {
                foreach (var ans in survey.Answers)
                {
                    extendedAnswers.Add(new ExtendedSurveyAnswer
                    {
                        AnswerId = ans.AnswerId,
                        Card = associatedCard.FirstOrDefault(c => c.Id == ans.CardId)?.ToAnnotation(),
                        AnswerTitle = associatedCard.FirstOrDefault(c => c.Id == ans.CardId)?.Answers?.FirstOrDefault(a => a.Id == ans.AnswerId)?.Title,
                        UserInputType = ans.UserInputType,
                        UserInputValue = ans.UserInputValue,
                        CardId = null
                    }); 
                }
            }

            return new ExtendedSurvey { 
                StartDate = survey.StartDate,
                Answers = extendedAnswers,
                DeleteDate = survey.DeleteDate,
                EndDate = survey.EndDate,
                CurrentCardId = survey.CurrentCardId,
                Id = survey.Id,
                LocalizationLanguage = survey.LocalizationLanguage,
                Type = survey.Type,
                UserId = survey.UserId,
                Rating = survey.Rating,
            };
        }

        public async Task<List<SurveyMy>> GetSurveysByTagAsync(SurveyTag tag, LocalizationsLanguage language)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var surveys = await _dbAccessor.GetSurveysByTagAsync(tag, userId);
            var mySurveys = new List<SurveyMy>();

            var answerInfo = new Answer();
            var relatedCard = new Annotation();
            foreach (var survey in surveys)
            {
                if(survey.Answers != null && survey.Answers.Any())
                {
                    var lastAnswer = survey.Answers?.Last();
                    try
                    {
                        answerInfo = await _dbAccessor.GetAnswerByIdAsync(lastAnswer?.CardId!, lastAnswer?.AnswerId, language);
                        relatedCard = (await _dbAccessor.GetCardByIdAsync(lastAnswer?.CardId!, language)).ToAnnotation();
                    }
                    catch (FormatException ex)
                    {
                        //todo log
                    }
                    mySurveys.Add(ToSurveyMy(survey, lastAnswer, answerInfo, relatedCard));
                }
                else
                {
                    mySurveys.Add(ToSurveyMy(survey, null, answerInfo, relatedCard));
                }
                
            }

            return mySurveys;
        }

        public async Task<(string, Card, bool)> PostAnswer(string surveyId, SurveyAnswer surveyAnswer, LocalizationsLanguage language)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);

            Survey survey = await GetSurvey(surveyId, surveyAnswer, userId, language);
            string? cardId = await GetNextCardId(surveyAnswer, survey);
            Card? nextCard = await GetNextCard(cardId, survey);

            if (nextCard.CardType == CardType.Result)
            {
                // todo send message : "That survey was helpful?"
                await _dbAccessor.EndSurvey(survey.Id);
            }

            if (nextCard is not null && survey is not null && survey.Id is not null)
            {
                var updateStatus = await _dbAccessor.PostAnswer(survey.Id, userId, surveyAnswer, nextCard.Id);
                if (updateStatus.MatchedCount == 1)
                {
                    return (survey.Id, nextCard, nextCard.CardType == CardType.Result);
                }
            }
            //Todo custom exceptions and status codes

            throw new FormatException($"Something went wrong with post answer");
        }

        private async Task<Card> GetNextCard(string? cardId, Survey? survey)
        {
            if (cardId is not null && survey is not null)
            {
                return await _dbAccessor.GetCardByIdAsync(cardId, survey.LocalizationLanguage);
            }

            throw new FormatException($"Answer or Survey can not be null");
        }

        private async Task<string?> GetNextCardId(SurveyAnswer surveyAnswer, Survey survey)
        {
            if (surveyAnswer.AnswerId != null && surveyAnswer.AnswerId != Guid.Empty)
            {
                var answer = await _dbAccessor.GetAnswerByIdAsync(surveyAnswer.CardId, surveyAnswer.AnswerId, survey.LocalizationLanguage);
                return answer.RelatedCard?.Id;
            }
            else
            {
                var card = await _dbAccessor.GetCardByIdAsync(surveyAnswer.CardId, survey.LocalizationLanguage);
                return ConditionHelper.ResolveCardId(card, surveyAnswer);
            }
        }

        private async Task<Survey> GetSurvey(string? surveyId, SurveyAnswer surveyAnswer, string userId, LocalizationsLanguage language)
        {
            if (string.IsNullOrWhiteSpace(surveyId))
            {
                var isUsageOk = await CheckSurveyUsage(userId);
                if (isUsageOk)
                {
                    var newUsage = await _usageService.DecrementUsageAsync(LimitKind.Survey, userId);
                    if(newUsage != null)
                    {
                        return await _dbAccessor.StartSurveyAsync(surveyAnswer, userId, language);
                    }
                    else
                    {
                        throw new ArgumentException("New survey not started. Reason: problem with decrement usage");
                    }
                }
                else
                {
                    throw new UsageExceededException("There is no survey completion left");
                }    
            }
            else
            {
                return await _dbAccessor.GetSurveyByIdAsync(surveyId, userId);
            }
        }

        private async Task<bool> CheckSurveyUsage(string userId)
        {
            var usage = await _usageService.GetUsageAsync(userId);
            if(usage != null && usage.SurveyUsage > 0)
            {
                return true;
            }
            return false;
        }

        private async Task<bool> CheckRollbackUsage(string userId)
        {
            var usage = await _usageService.GetUsageAsync(userId);
            if (usage != null && usage.RollbackUsage > 0)
            {
                return true;
            }
            return false;
        }

        private SurveyMy ToSurveyMy(Survey survey, SurveyAnswer? lastAnswer, Answer ansInfo, Annotation relatedCard)
        {
            

            return new SurveyMy
            {
                CurrentCardId = survey.CurrentCardId,
                DeleteDate = survey.DeleteDate,
                EndDate = survey.EndDate,
                StartDate = survey.StartDate,
                Id = survey.Id,
                LocalizationLanguage = survey.LocalizationLanguage,
                Answer = lastAnswer == null ? null : new ExtendedSurveyAnswer
                {
                    AnswerId = lastAnswer?.AnswerId,
                    Card = relatedCard,
                    AnswerTitle = ansInfo.Title,
                    UserInputType = lastAnswer?.UserInputType,
                    UserInputValue = lastAnswer?.UserInputValue,
                    CardId = null,
                    ImageInfo = ansInfo.ImageInfo
                },
                Type = survey.Type,
                UserId = survey.UserId,
                Rating = survey.Rating,
            };
        }
    }
}
