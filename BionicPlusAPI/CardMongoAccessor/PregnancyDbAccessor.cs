using DomainObjects.Infrastracture.Exceptions;
using DomainObjects.Pregnancy;
using DomainObjects.Pregnancy.Children;
using DomainObjects.Pregnancy.Localizations;
using Microsoft.VisualBasic;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using PregnancyDBMongoAccessor.Infrastracture;
using PregnancyDBMongoAccessor.Models;
using PregnancyDBMongoAccessor.MongoClasses;
using System.Collections.ObjectModel;

namespace PregnancyDBMongoAccessor
{
    public class PregnancyDbAccessor
    {
        private const string DB_NAME = "Pregnancy";
        private const string CARDS_COLLECTION_NAME = "Cards";
        private const string SURVEYS_COLLECTION_NAME = "Surveys";
        private const string PREGNANCYS_COLLECTION_NAME = "Pregnancies";
        private const string CHILDREN_COLLECTION_NAME = "Children";
        private readonly MongoClient _client;
        private readonly IMongoDatabase _db;
        private readonly IMongoCollection<MongoCard> _cardsCollection;
        private readonly IMongoCollection<MongoSurvey> _surveysCollection;
        private readonly IMongoCollection<MongoPregnancy> _pregnancyCollection;
        private readonly IMongoCollection<MongoChild> _childrenCollection;

        public PregnancyDbAccessor(DbSettings dbSettings)
        {
            _client = new MongoClient(dbSettings.ConnectionString);
            _db = _client.GetDatabase(DB_NAME);
            _cardsCollection = _db.GetCollection<MongoCard>(CARDS_COLLECTION_NAME);
            _surveysCollection = _db.GetCollection<MongoSurvey>(SURVEYS_COLLECTION_NAME);
            _pregnancyCollection = _db.GetCollection<MongoPregnancy>(PREGNANCYS_COLLECTION_NAME);
            _childrenCollection = _db.GetCollection<MongoChild>(CHILDREN_COLLECTION_NAME);
        }

        #region Cards
        public async Task<Card> GetCardByIdAsync(string? id, LocalizationsLanguage? language)
        {
            if(id is null)
            {
                throw new FormatException("Card id cannot be null");
            }
            var filter = Builders<MongoCard>.Filter.Where(p => p.Id == id);
            var card = (await _cardsCollection.FindAsync(filter)).FirstOrDefault();
            if (card is null)
            {
                throw new NotFoundException($"Not found card with {id} id");
            }
            return card.Convert(language) ?? throw new FormatException($"Not found card with {id} id");
        }

        public async Task<List<Card>> GetBasicCardsByCategory(Category? category, LocalizationsLanguage? language)
        {
            var filter = Builders<MongoCard>.Filter.Where(p => p.Category == category && p.CardType == CardType.Basic);

            var cards = (await _cardsCollection.FindAsync(filter)).ToList();
            if (cards is null)
            {
                throw new NotFoundException($"Not found basic card with {category} category");
            }
            return cards.ConvertAll(c => c.Convert(language)) ?? throw new FormatException($"Not found basic card with {category} category");
        }

        public async Task<Card> GetCardByTitleAsync(string title, LocalizationsLanguage language)
        {
            var filter = Builders<MongoCard>.Filter.Where(p => p.Title!.LocalizationValues[language.ToString()] == title);
            var card = (await _cardsCollection.FindAsync(filter)).FirstOrDefault();
            if(card is null)
            {
                throw new NotFoundException($"Not found card with {title} title");
            }
            return card.Convert(language) ?? throw new FormatException($"Not found card with {title} title");
        }

        public async Task<List<Card>> GetCardsAsync(string? title, CardType? cardType, Category? category, LocalizationsLanguage language)
        {
            var query = _cardsCollection.AsQueryable();
            if (cardType != null && cardType != CardType.Undefined)
            {
                query = query.Where(c => c.CardType == cardType);
            }

            if (category != null && category != Category.Undefined)
            {
                query = query.Where(c => c.Category == category);
            }

            if (title != null && title != string.Empty)
            {
                var preparedTitle = title.ToLowerInvariant();
                query = query.Where(t => t.Title!.LocalizationValues[language.ToString()] != null && t.Title!.LocalizationValues[language.ToString()].ToLowerInvariant().Contains(preparedTitle));
            }

            var cards = await query.ToListAsync();
            if (cards is null || !cards.Any())
            {
                throw new NotFoundException($"Not found card with {cardType} type {category} category {title} title");
            }
            return cards.ConvertAll(c => c.Convert(language)) ?? throw new FormatException($"Not found card with {cardType} type {category} category {title} title");
        }

        public async Task<List<Card>> GetCardsByNameAsync(string title, LocalizationsLanguage language)
        {
            var preparedTitle = title.ToLowerInvariant();
            var result = await _cardsCollection.AsQueryable().Where(t => t.Title!.LocalizationValues[language.ToString()] != null && t.Title!.LocalizationValues[language.ToString()].ToLowerInvariant().Contains(preparedTitle)).ToListAsync();
            return result.ConvertAll(c => c.Convert(language));
        }


        public async Task<Pregnancy> AddPregnancy(Pregnancy pregnancy, Guid userGuid)
        {
            var mongoPregnancy = pregnancy.Convert(userGuid);
            
            await _pregnancyCollection.InsertOneAsync(mongoPregnancy);

            return mongoPregnancy.Convert();
        }

        public async Task AddWeighing(string id, Guid userGuid, Weighing weighing)
        {
            var mongoWeighing = weighing.Convert();
            var filter = Builders<MongoPregnancy>.Filter.Where(x => x.Id == id && x.UserGuid == userGuid);

            var pregnancy = _pregnancyCollection.Find(filter).FirstOrDefault();

            if(pregnancy == null)
            {
                throw new FormatException($"Not found pregnancy with {id} id");
            }

            if (!pregnancy.IsActive)
            {
                throw new FormatException($"Can not add Weighing in archive pregnancy");
            }

            if(pregnancy.Weighings == null)
            {
                pregnancy.Weighings = new List<MongoWeighing>();
            }
            else
            {
                if (pregnancy.Weighings.Any(w => w.Date == weighing.Date))
                {
                    throw new FormatException($"Can not add Weighing. Weighing with date: {weighing.Date} already exist");
                }
            }
            pregnancy.Weighings.Add(weighing.Convert());

            await _pregnancyCollection.ReplaceOneAsync(filter, pregnancy);
        }

        public async Task ChangeWeighing(string id, Guid userGuid, Weighing weighing)
        {
            var mongoWeighing = weighing.Convert();
            var filter = Builders<MongoPregnancy>.Filter.Where(x => x.Id == id && x.UserGuid == userGuid);

            var pregnancy = _pregnancyCollection.Find(filter).FirstOrDefault();

            if (pregnancy == null)
            {
                throw new FormatException($"Not found pregnancy with {id} id");
            }

            if (!pregnancy.IsActive)
            {
                throw new FormatException($"Can not change Weighing in archive pregnancy");
            }

            if (pregnancy.Weighings == null)
            {
                throw new FormatException($"Weighing not found in pregnancy with id {id}");
            }

            var targetWeighing = pregnancy.Weighings.FirstOrDefault(w => w.Date == weighing.Date);

            if(targetWeighing == null)
            {
                throw new FormatException($"Weighing for date {weighing.Date} not found in pregnancy with id {id}");
            }

            targetWeighing.Value = weighing.Value;

            await _pregnancyCollection.ReplaceOneAsync(filter, pregnancy);
        }

        public async Task DeleteWeighing(string id, Guid userGuid, DateOnly date)
        {
            var filter = Builders<MongoPregnancy>.Filter.Where(x => x.Id == id && x.UserGuid == userGuid);

            var pregnancy = _pregnancyCollection.Find(filter).FirstOrDefault();

            if (pregnancy == null)
            {
                throw new FormatException($"Not found pregnancy with {id} id");
            }

            if (!pregnancy.IsActive)
            {
                throw new FormatException($"Can not delete Weighing in archive pregnancy");
            }

            if (pregnancy.Weighings == null)
            {
                throw new FormatException($"Weighing not found in pregnancy with id {id}");
            }

            
            var firstWeighing = pregnancy.Weighings.FirstOrDefault();

            var targetWeighing = pregnancy.Weighings.FirstOrDefault(w => w.Date == date);

            if (targetWeighing == null)
            {
                throw new FormatException($"Weighing for date {date} not found in pregnancy with id {id}");
            }

            if (targetWeighing == firstWeighing)
            {
                throw new FormatException($"Can not delete first weighing");
            }

            pregnancy.Weighings.Remove(targetWeighing);

            await _pregnancyCollection.ReplaceOneAsync(filter, pregnancy);
        }

        public async Task ArchivePregnancy(string id, Guid userGuid)
        {
            var filter = Builders<MongoPregnancy>.Filter.Where(x => x.Id == id && x.UserGuid == userGuid);

            var update = Builders<MongoPregnancy>.Update
                        .Set(x => x.IsActive, false)
                        .Set(x => x.WhenArchived, DateTime.UtcNow);

            var result = await _pregnancyCollection.UpdateOneAsync(filter, update);
        }

        public async Task DeletePregnancy(string id, Guid userGuid)
        {
            var filter = Builders<MongoPregnancy>.Filter.Where(x => x.Id == id && x.UserGuid == userGuid && x.IsActive != true);

            var result = await _pregnancyCollection.DeleteOneAsync(filter);
        }

        public async Task<Card> AddCardAsync(Card card, LocalizationsLanguage language)
        {
            if(card.Answers == null)
            {
                card.Answers = new List<Answer>();
            }
            if(card.Answers?.Any() ?? false)
            {
                card.Answers = EnsureAnswerId(card.Answers);
            }
            var mongoCard = card.Convert(language);
            if (mongoCard != null)
            {
                await _cardsCollection.InsertOneAsync(mongoCard);
                return mongoCard.Convert(language);
            }
            else
            {
                throw new FormatException("Card cannot be null");
            }
        }

        public async Task<ObjectOperationStatus> DeleteCardAsync(string cardId)
        {
            var filter = Builders<MongoCard>.Filter.Where(card => card.Id == cardId);
            var deleteResult = await _cardsCollection.DeleteOneAsync(filter);
            return new ObjectOperationStatus { DeletedCount = deleteResult.DeletedCount };
        }

            public async Task<List<Answer>> GetAnswersByCardIdAsync(string cardId, LocalizationsLanguage language)
        {
            var filter = Builders<MongoCard>.Filter.Where(p => p.Id == cardId);

            var card = (await _cardsCollection.FindAsync(filter)).FirstOrDefault();

            if (card is null)
            {
                throw new FormatException($"Not found card with {cardId} id");
            }

            return card.Answers?.ConvertAll(a => Converter.Convert(a, language)) ?? new List<Answer>();
        }

        public async Task<Answer> GetAnswerByIdAsync(string cardId, Guid? answerId, LocalizationsLanguage? language)
        {
            if (answerId == null)
            {
                throw new FormatException("answerId cannot be null");
            }

            var filter = Builders<MongoCard>.Filter.Where(p => p.Id == cardId);

            var card = (await _cardsCollection.FindAsync(filter)).FirstOrDefault();

            if (card is null)
            {
                throw new FormatException($"Not found card with {cardId} id");
            }

            var answers = card.Answers?.ConvertAll(a => Converter.Convert(a, language)) ?? new List<Answer>();

            var answer = answers.Where(a => a.Id == answerId).FirstOrDefault();

            if (answer is null)
            {
                throw new FormatException($"Not found answer with {answerId} id");
            }

            return answer;
        }

        public async Task<ObjectOperationStatus> DeleteAnswerAsync(string cardId, Guid answerId)
        {
            var filter = Builders<MongoCard>.Filter.Where(card => card.Id == cardId);
            var update = Builders<MongoCard>.Update.PullFilter(card => card.Answers, Builders<MongoAnswer>.Filter.Where(answer => answer.Id == answerId));
            var updateResult = await _cardsCollection.UpdateOneAsync(filter, update);
            return new ObjectOperationStatus { MatchedCount = updateResult.MatchedCount, ModifiedCount = updateResult.ModifiedCount };
        }

        public async Task<ObjectOperationStatus> AssociateAnswer(string cardId, string relatedCardId, Guid answerId)
        {
            var updateResult = await _cardsCollection.UpdateOneAsync(l => l.Id == cardId && l.Answers!.Any(l => l.Id == answerId),
                    Builders<MongoCard>.Update.Set(l => l.Answers.FirstMatchingElement().RelatedCardId, relatedCardId));
            return new ObjectOperationStatus { MatchedCount = updateResult.MatchedCount, ModifiedCount = updateResult.ModifiedCount };
        }

        public async Task<ObjectOperationStatus> UpdateAnswer(string cardId, Answer answer, LocalizationsLanguage language)
        {
            var filter = Builders<MongoCard>.Filter.Where(p => p.Id == cardId);
            var card = (await _cardsCollection.FindAsync(filter)).FirstOrDefault();

            var cardAnswer = card.Answers?.Where(a => a.Id == answer.Id).FirstOrDefault();
            if (cardAnswer is not null)
            {
                if(!string.IsNullOrWhiteSpace(answer.Title) && cardAnswer.Title != null)
                {
                    if (cardAnswer.Title.LocalizationValues.ContainsKey(language.ToString()))
                    {
                        cardAnswer.Title.LocalizationValues[language.ToString()] = answer.Title;
                    }
                    else
                    {
                        cardAnswer.Title.LocalizationValues.Add(language.ToString(), answer.Title);
                    }
                }
                
                if(!string.IsNullOrWhiteSpace(answer.Description) && cardAnswer.Description != null)
                {
                    if (cardAnswer.Description.LocalizationValues.ContainsKey(language.ToString()))
                    {
                        cardAnswer.Description.LocalizationValues[language.ToString()] = answer.Description; ;
                    }
                    else
                    {
                        cardAnswer.Description.LocalizationValues.Add(language.ToString(), answer.Description);
                    }
                }

                if (answer.ImageInfo != null)
                {
                    cardAnswer.ImgInfo = answer.ImageInfo;
                }
            }


            var updateResult = await _cardsCollection.UpdateOneAsync(l => l.Id == cardId && l.Answers!.Any(l => l.Id == answer.Id),
                    Builders<MongoCard>.Update.Set(l => l.Answers.FirstMatchingElement(), cardAnswer));
            return new ObjectOperationStatus { MatchedCount = updateResult.MatchedCount, ModifiedCount = updateResult.ModifiedCount };
        }

        public async Task<ObjectOperationStatus> UpdateCard(Card card, LocalizationsLanguage language)
        {
            if (string.IsNullOrWhiteSpace(card.Id))
            {
                throw new FormatException($"Card id cannot be null");
            }

            var filter = Builders<MongoCard>.Filter.Where(p => p.Id == card.Id);
            var mongoCard = (await _cardsCollection.FindAsync(filter)).FirstOrDefault();

            if (mongoCard is not null)
            {
                if (!string.IsNullOrWhiteSpace(card.Title))
                {
                    if (mongoCard.Title != null)
                    {
                        if (mongoCard.Title.LocalizationValues.ContainsKey(language.ToString()))
                        {
                            mongoCard.Title.LocalizationValues[language.ToString()] = card.Title;
                        }
                        else
                        {
                            mongoCard.Title.LocalizationValues.Add(language.ToString(), card.Title);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(card.Description))
                {
                    if (mongoCard.Description != null)
                    {
                        if (mongoCard.Description.LocalizationValues.ContainsKey(language.ToString()))
                        {
                            mongoCard.Description.LocalizationValues[language.ToString()] = card.Description;
                        }
                        else
                        {
                            mongoCard.Description.LocalizationValues.Add(language.ToString(), card.Description);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(card.SolutionSteps))
                {
                    if (mongoCard.SolutionSteps != null)
                    {
                        if (mongoCard.SolutionSteps.LocalizationValues.ContainsKey(language.ToString()))
                        {
                            mongoCard.SolutionSteps.LocalizationValues[language.ToString()] = card.SolutionSteps;
                        }
                        else
                        {
                            mongoCard.SolutionSteps.LocalizationValues.Add(language.ToString(), card.SolutionSteps);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(card.References))
                {
                    mongoCard.References = card.References;
                }

                if (card.UserInputType != mongoCard.UserInputType)
                {
                    mongoCard.UserInputType = card.UserInputType;
                }

                if (card.Condition != null)
                {
                    mongoCard.Condition = card.Condition.Convert();
                }

                if (card.EmergencyLvl != mongoCard.EmergencyLvl)
                {
                    mongoCard.EmergencyLvl = card.EmergencyLvl;
                }

                if (card.UserInputType != mongoCard.UserInputType)
                {
                    mongoCard.UserInputType = card.UserInputType;
                }

                if (card.ImgInfo != null)
                {
                    mongoCard.ImgInfo = card.ImgInfo;
                }

                if (card.CardType != CardType.Undefined)
                {
                    mongoCard.CardType = card.CardType;
                }

                if (card.Category != Category.Undefined)
                {
                    mongoCard.Category = card.Category;
                }
            }
            else
            {
                throw new FormatException($"Not found card with {card.Id} id");
            }


            var updateResult = await _cardsCollection.ReplaceOneAsync(filter, mongoCard);
            return new ObjectOperationStatus { MatchedCount = updateResult.MatchedCount, ModifiedCount = updateResult.ModifiedCount };
        }

        public async Task<ObjectOperationStatus> AddAnswerAsync(string cardId, Answer answer, LocalizationsLanguage language)
        {
            var mongoAnswer = answer.Convert(language);
            var filter = Builders<MongoCard>.Filter.Where(card => card.Id == cardId);
            var update = Builders<MongoCard>.Update.Push(card => card.Answers, mongoAnswer);
            var updateResult = await _cardsCollection.UpdateOneAsync(filter, update);
            return new ObjectOperationStatus { MatchedCount = updateResult.MatchedCount, ModifiedCount = updateResult.ModifiedCount };
        }

        private List<Answer> EnsureAnswerId(List<Answer> answers)
        {
            answers.ForEach(answer => EnsureAnswerId(answer));
            return answers;
        }

        private Answer EnsureAnswerId(Answer answer)
        {
            answer.Id = Guid.NewGuid();
            return answer;
        }
        #endregion

        #region Surveys
        public async Task<Survey> GetSurveyByIdAsync(string id, string userId)
        {
            var filter = Builders<MongoSurvey>.Filter.Where(p => p.Id == id && p.UserId == userId);
            var survey = (await _surveysCollection.FindAsync(filter)).FirstOrDefault();
            if (survey is null)
            {
                throw new FormatException($"Not found survey with {id} id");
            }
            return survey.Convert() ?? throw new FormatException($"Not found survey with {id} id");
        }

        public async Task DeleteSurveyByIdAsync(string id, string userId)
        {
            var filter = Builders<MongoSurvey>.Filter.Where(p => p.Id == id && p.UserId == userId);
            var deleteResult = (await _surveysCollection.DeleteOneAsync(filter));
            if (deleteResult.DeletedCount == 0)
            {
                throw new FormatException($"Not found survey with {id} id");
            }
        }

        public async Task RateSurveyAsync(string id, string userId, Rating rating)
        {
            var filter = Builders<MongoSurvey>.Filter.Where(p => p.Id == id && p.UserId == userId);
            var survey = (await _surveysCollection.FindAsync(filter)).FirstOrDefault();
            
            if(survey != null)
            {
                survey.Rating = rating.Convert();
                await _surveysCollection.ReplaceOneAsync(filter, survey);
            }
            else
            {
                throw new FormatException($"Not found survey with {id} id");
            }
        }

        public async Task<bool> Rollback(string surveyId, string cardId, string userId)
        {
            var filter = Builders<MongoSurvey>.Filter.Where(p => p.Id == surveyId && p.UserId == userId);
            var survey = (await _surveysCollection.FindAsync(filter)).FirstOrDefault();

            if (survey != null)
            {

                if (survey.EndDate != null)
                {
                    throw new FormatException($"Can't rollback ended survey");
                }

                var answers = survey.Answers;
                var newAnswers = new List<MongoSurveyAnswer>();
                foreach(var ans in answers)
                {
                    if(ans.CardId == cardId)
                    {
                        break;
                    }
                    newAnswers.Add(ans);
                }

                survey.Answers = newAnswers;
                survey.CurrentCardId = cardId;

                var updateResult = await _surveysCollection.ReplaceOneAsync(filter, survey);
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<Card>> PullAssociatedCards(List<string> cardIds, LocalizationsLanguage language)
        {
            var cardFilter = Builders<MongoCard>.Filter.In(c => c.Id, cardIds);
            var cards = await _cardsCollection.Find(cardFilter).ToListAsync();
            return cards.ConvertAll(c => c.Convert(language));
        }

        public async Task<List<Survey>> GetSurveysByTagAsync(SurveyTag tag, string userId)
        {
            var filter = Builders<MongoSurvey>.Filter.Where(p => p.UserId == userId);
            if (tag == SurveyTag.Incompleted)
            {
                filter &= Builders<MongoSurvey>.Filter.Where(p => p.EndDate == null);
            }
            var survey = (await _surveysCollection.FindAsync(filter)).ToList();
            if (survey is null || !survey.Any())
            {
                throw new NotFoundException($"Not found survey with {tag} tag");
            }
            return survey?.ConvertAll(s => s.Convert()) ?? throw new FormatException($"Not found survey with {tag} tag");
        }

        public async Task<Survey> StartSurveyAsync(SurveyAnswer answer, string userId, LocalizationsLanguage language)
        {
            var mongoAnswer = answer.Convert();
            mongoAnswer.Date = DateTime.UtcNow;

            var mongoSurvey = new MongoSurvey
            {
                UserId = userId,
                StartDate = DateTime.UtcNow,
                CurrentCardId = answer.CardId,
                Answers = new List<MongoSurveyAnswer>(),
                LocalizationLanguage = language,
                Type = SurveyType.Advisory,
            };

            await _surveysCollection.InsertOneAsync(mongoSurvey);
            if(string.IsNullOrWhiteSpace(mongoSurvey.Id))
            {
                throw new FormatException($"Can't start survey");
            }
            return mongoSurvey.Convert();
        }

        public async Task EndSurvey(string? surveyId)
        {
            if (string.IsNullOrWhiteSpace(surveyId))
            {
                throw new FormatException("surveyId can not be null");
            }
            var filter = Builders<MongoSurvey>.Filter.Where(s => s.Id == surveyId);
            var update = Builders<MongoSurvey>.Update.Set(s => s.EndDate, DateTime.Now);
            var updateResult = await _surveysCollection.UpdateOneAsync(filter, update);
        }

        public async Task<ObjectOperationStatus> PostAnswer(string surveyId, string userId, SurveyAnswer answer, string? currentCardId)
        {
            var mongoAnswer = answer.Convert();
            mongoAnswer.Date = DateTime.UtcNow;
            var filter = Builders<MongoSurvey>.Filter.Where(survey => survey.Id == surveyId && survey.UserId == userId);
            var update = Builders<MongoSurvey>.Update.Push(survey => survey.Answers, mongoAnswer).Set(s => s.CurrentCardId, currentCardId);
            var updateResult = await _surveysCollection.UpdateOneAsync(filter, update);
            return new ObjectOperationStatus { MatchedCount = updateResult.MatchedCount, ModifiedCount = updateResult.ModifiedCount };
        }

        public async Task<List<Pregnancy>> GetPregnancies(bool isActive, Guid userId)
        {
            var filter = Builders<MongoPregnancy>.Filter.Where(p => p.IsActive == isActive && p.UserGuid == userId);
            var pregnanies = await _pregnancyCollection.Find(filter).ToListAsync();

            if(pregnanies == null)
            {
                throw new FormatException($"Pregnanies not found for user {userId}");
            }

            return pregnanies.Select(p => p.Convert()).ToList();
        }
        #endregion

        #region Children

        public async Task<List<Child>> GetChildren(bool isActive, Guid userId)
        {
            var filter = Builders<MongoChild>.Filter.Where(p => p.IsActive == isActive && p.UserGuid == userId);
            var children = await _childrenCollection.Find(filter).ToListAsync();

            if (children == null)
            {
                throw new FormatException($"Children not found for user {userId}");
            }

            return children.Select(p => p.Convert()).ToList();
        }

        public async Task<string> AddChild(Child child, Guid userId)
        {
            var mongoChild = child.Convert(userId);

            await _childrenCollection.InsertOneAsync(mongoChild);

            return mongoChild.Id ?? "";
        }

        public async Task AddMeasurement(string childId, Guid userGuid, ChildMeasurement measurement)
        {
            var mongoMeasurement = measurement.Convert();
            var filter = Builders<MongoChild>.Filter.Where(x => x.Id == childId && x.UserGuid == userGuid);

            var child = _childrenCollection.Find(filter).FirstOrDefault();

            if (child == null)
            {
                throw new FormatException($"Not found child with {childId} id");
            }

            if (!child.IsActive)
            {
                throw new FormatException($"Can not add measurement in archive child");
            }

            if (child.Measurements == null)
            {
                child.Measurements = new List<MongoMeasurement>();
            }
            else
            {
                if (child.Measurements.Any(w => w.Date == measurement.Date))
                {
                    throw new FormatException($"Can not add Weighing. Weighing with date: {measurement.Date} already exist");
                }
            }
            child.Measurements.Add(measurement.Convert());

            await _childrenCollection.ReplaceOneAsync(filter, child);
        }

        public async Task ArchiveChild(string id, Guid userGuid)
        {
            var filter = Builders<MongoChild>.Filter.Where(x => x.Id == id && x.UserGuid == userGuid);

            var update = Builders<MongoChild>.Update
                        .Set(x => x.IsActive, false)
                        .Set(x => x.WhenArchived, DateTime.UtcNow);

            var result = await _childrenCollection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteChild(string id, Guid userGuid)
        {
            var filter = Builders<MongoChild>.Filter.Where(x => x.Id == id && x.UserGuid == userGuid && x.IsActive != true);

            var result = await _childrenCollection.DeleteOneAsync(filter);
        }

        public async Task DeleteMeasurement(string childId, DateOnly date, Guid guid)
        {
            var filter = Builders<MongoChild>.Filter.Where(x => x.Id == childId && x.UserGuid == guid);

            var child = _childrenCollection.Find(filter).FirstOrDefault();

            if (child == null)
            {
                throw new FormatException($"Not found child with {childId} id");
            }

            if (!child.IsActive)
            {
                throw new FormatException($"Can not delete measurement in archive child");
            }

            if (child.Measurements == null)
            {
                throw new FormatException($"Measurement not found in child with id {childId}");
            }


            var firstMeasurements = child.Measurements.FirstOrDefault();

            var targetMeasurements = child.Measurements.FirstOrDefault(w => w.Date == date);

            if (targetMeasurements == null)
            {
                throw new FormatException($"Measurement for date {date} not found in child with id {childId}");
            }

            if (targetMeasurements == firstMeasurements)
            {
                throw new FormatException($"Can not delete first Measurement");
            }

            child.Measurements.Remove(targetMeasurements);

            await _childrenCollection.ReplaceOneAsync(filter, child);
        }

        public async Task UpdateMeasurement(string childId, DateOnly date, Guid userGuid, ChildMeasurement measurement)
        {
            var mongoMeasurement = measurement.Convert();
            var filter = Builders<MongoChild>.Filter.Where(x => x.Id == childId && x.UserGuid == userGuid);

            var child = _childrenCollection.Find(filter).FirstOrDefault();

            if (child == null)
            {
                throw new FormatException($"Not found child with {childId} id");
            }

            if (!child.IsActive)
            {
                throw new FormatException($"Can not change measurement in archive child");
            }

            if (child.Measurements == null)
            {
                throw new FormatException($"Measurement not found in pregnancy with id {childId}");
            }

            var targetWeighing = child.Measurements.FirstOrDefault(w => w.Date == measurement.Date);

            if (targetWeighing == null)
            {
                throw new FormatException($"Measurement for date {measurement.Date} not found in pregnancy with id {childId}");
            }

            targetWeighing.Height = measurement.Height;
            targetWeighing.Weight = measurement.Weight;

            await _childrenCollection.ReplaceOneAsync(filter, child);
        }

        #endregion
    }
}
