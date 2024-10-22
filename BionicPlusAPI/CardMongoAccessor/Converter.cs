using DomainObjects.Pregnancy;
using DomainObjects.Pregnancy.Children;
using DomainObjects.Pregnancy.Localizations;
using PregnancyDBMongoAccessor.MongoClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PregnancyDBMongoAccessor
{
    internal static class Converter
    {
        public static MongoCard? Convert(this Card card, LocalizationsLanguage? language)
        {
            var title = new Localization(LocalizationType.Title, language, card.Title);
            var description = new Localization(LocalizationType.Description, language, card.Description);
            var solutionSteps = new Localization(LocalizationType.SolutionSteps, language, card.SolutionSteps);
            if (card == null)
            {
                return null;
            }
            return new MongoCard
            {
                Id = card.Id,
                Description = description.Convert(),
                Title = title.Convert(),
                SolutionSteps = solutionSteps.Convert(),
                Answers = card.Answers?.ConvertAll(a => Convert(a, language)),
                Condition = card.Condition?.Convert(),
                EmergencyLvl = card.EmergencyLvl,
                References = card.References,
                UserInputType = card.UserInputType,
                CardType = card.CardType,
                ImgInfo = card.ImgInfo,
                Category = card.Category,
            };
        }

        public static Card Convert(this MongoCard card, LocalizationsLanguage? language)
        {
            return new Card
            {
                Id = card.Id,
                Description = card.Description?.GetLocalizationByLanguage(language),
                Title = card.Title?.GetLocalizationByLanguage(language)!,
                SolutionSteps = card.SolutionSteps?.GetLocalizationByLanguage(language),
                Answers = card.Answers?.ConvertAll(a => Convert(a, language)),
                Condition = card.Condition?.Convert(),
                EmergencyLvl = card.EmergencyLvl,
                References = card.References,
                UserInputType = card.UserInputType,
                CardType = card.CardType,
                ImgInfo = card.ImgInfo,
                Category = card.Category,
            };
        }

        private static MongoLocalization Convert(this Localization localization)
        {
            MongoLocalization mongoLocalization = new MongoLocalization();
            foreach(var value in localization.LocalizationValues)
            {
                mongoLocalization.LocalizationValues.Add(value.Key.ToString(), value.Value);
            }
            mongoLocalization.Type = localization.Type;
            return mongoLocalization;
        }

        private static string GetLocalizationByLanguage(this MongoLocalization mongoLocalization, LocalizationsLanguage? language)
        {
            string? value = String.Empty;
            mongoLocalization.LocalizationValues.TryGetValue(language.ToString()!, out value);
            return value ?? String.Empty;
        }

        public static Answer Convert(this MongoAnswer mongoAnswer, LocalizationsLanguage? language)
        {
            return new Answer
            {
                Description = mongoAnswer.Description?.GetLocalizationByLanguage(language),
                Id = mongoAnswer.Id,
                ImageInfo = mongoAnswer.ImgInfo,
                Title = mongoAnswer.Title?.GetLocalizationByLanguage(language),
                RelatedCard = new Annotation { Id = mongoAnswer.RelatedCardId },
            };
        }

        public static MongoAnswer Convert(this Answer answer, LocalizationsLanguage? language)
        {
            var title = new Localization(LocalizationType.Title, language, answer.Title);
            var description = new Localization(LocalizationType.Description, language, answer.Description);

            return new MongoAnswer
            {
                Id = answer.Id,
                ImgInfo = answer.ImageInfo,
                Title = title.Convert(),
                Description = description.Convert(),
                RelatedCardId = answer.RelatedCard?.Id,
            };
        }

        public static MongoSurveyAnswer Convert(this SurveyAnswer answer)
        {
            return new MongoSurveyAnswer
            {
                AnswerId = answer.AnswerId.ToString(),
                CardId = answer.CardId,
                UserInputType = answer.UserInputType,
                UserInputValue = answer.UserInputValue
            };
        }

        public static SurveyAnswer Convert(this MongoSurveyAnswer answer)
        {
            return new SurveyAnswer
            {
                AnswerId = string.IsNullOrWhiteSpace(answer.AnswerId) ? Guid.Empty : Guid.Parse(answer.AnswerId),
                CardId = answer.CardId!,
                UserInputType = answer.UserInputType,
                UserInputValue = answer.UserInputValue,
            };
        }

        public static Condition? Convert(this MongoCondition condition)
        {
            if (condition == null)
            {
                return null;
            }

            return new Condition
            {
                DefaultCard = new Annotation { Id = condition.DefaultCardId },
                Expressions = condition.Expressions.ConvertAll(a => a.Convert())
            };
        }

        public static MongoCondition? Convert(this Condition condition)
        {
            if (condition == null)
            {
                return null;
            }

            return new MongoCondition
            {
                DefaultCardId = condition.DefaultCard!.Id!,
                Expressions = condition.Expressions.ConvertAll(a => a.Convert())
            };
        }

        public static Expression? Convert(this MongoExpression condition)
        {
            if (condition == null) return null;
            return new Expression
            {
                Card = new Annotation { Id = condition.CardId },
                LeftParameter = condition.LeftParameter.Convert(),
                RightParameter = condition.RightParameter.Convert(),
                OperationType = condition.OperationType,
            };
        }

        public static MongoExpression? Convert(this Expression condition)
        {
            if (condition == null) return null;
            return new MongoExpression
            {
                CardId = condition.Card?.Id ?? string.Empty,
                LeftParameter = condition.LeftParameter.Convert(),
                RightParameter = condition.RightParameter.Convert(),
                OperationType = condition.OperationType,
            };
        }

        public static Parameter Convert(this MongoParameter condition)
        {
           
            return new Parameter
            {
                ParameterType = condition.ParameterType,
                Value = condition.Value ?? "",
                Expression = condition.Expression?.Convert(),
            };
        }

        public static MongoParameter Convert(this Parameter condition)
        {
            return new MongoParameter
            {
                ParameterType = condition.ParameterType,
                Value = condition.Value?.ToString(),
                Expression = condition.Expression?.Convert(),
            };
        }

        public static Survey Convert(this MongoSurvey survey)
        {
            return new Survey
            {
                Id = survey.Id,
                CurrentCardId = survey.CurrentCardId,
                DeleteDate = survey.DeleteDate,
                EndDate = survey.EndDate,
                StartDate = survey.StartDate,
                LocalizationLanguage = survey.LocalizationLanguage,
                Type = survey.Type,
                Rating = survey.Rating.Convert(),
                UserId = survey.UserId,
                Answers = survey.Answers?.ConvertAll(a => a.Convert())
            };
        }

        public static MongoSurvey Convert(this Survey survey)
        {
            return new MongoSurvey
            {
                Id = survey.Id,
                CurrentCardId = survey.CurrentCardId,
                DeleteDate = survey.DeleteDate,
                EndDate = survey.EndDate,
                StartDate = survey.StartDate,
                LocalizationLanguage = survey.LocalizationLanguage,
                Type = survey.Type,
                Rating = survey.Rating.Convert(),
                UserId = survey.UserId,
                Answers = survey.Answers?.ConvertAll(a => a.Convert())
            };
        }

        public static MongoRating? Convert(this Rating? rating)
        {
            if(rating == null)
            {
                return null;
            }

            return new MongoRating
            {
                Description = rating.Description,
                Score = rating.Score,
            };
        }

        public static Rating? Convert(this MongoRating? rating)
        {
            if (rating == null)
            {
                return null;
            }

            return new Rating
            {
                Description = rating.Description,
                Score = rating.Score,
            };
        }

        public static Pregnancy Convert(this MongoPregnancy mongoPregnancy)
        {
            return new Pregnancy
            {
                Date = mongoPregnancy.Date,
                Id = mongoPregnancy.Id,
                IsActive = mongoPregnancy.IsActive,
                Multiple = mongoPregnancy.Multiple,
                Weighings = mongoPregnancy.Weighings?.Select(w => w.Convert()).ToList(),
                WhenArchived = mongoPregnancy.WhenArchived
            };
        }

        public static MongoPregnancy Convert(this Pregnancy pregnancy, Guid userGuid)
        {
            return new MongoPregnancy
            {
                Date = pregnancy.Date,
                Id = pregnancy.Id,
                IsActive = pregnancy.IsActive,
                Multiple = pregnancy.Multiple,
                Weighings = pregnancy.Weighings?.Select(w => w.Convert()).ToList(),
                UserGuid = userGuid,
                WhenArchived = pregnancy.WhenArchived
            };
        }

        public static MongoWeighing Convert(this Weighing weighing)
        {
            return new MongoWeighing
            {
                Date = weighing.Date,
                Value = weighing.Value
            };
        }

        public static Weighing Convert(this MongoWeighing weighing)
        {
            return new Weighing
            {
                Date = weighing.Date,
                Value = weighing.Value
            };
        }

        public static Child Convert(this MongoChild child)
        {
            return new Child
            {
                Date = child.Date,
                Name = child.Name,
                Gender = child.Gender,
                Id = child.Id ?? string.Empty,
                IsActive = child.IsActive,
                Measurements = child.Measurements?.Select(m => m.Convert()).ToList(),
                WhenArchived = child.WhenArchived
            };
        }

        public static MongoChild Convert(this Child child, Guid userGuid)
        {
            return new MongoChild
            {
                Date = child.Date,
                Name = child.Name,
                Gender = child.Gender,
                Id = child.Id,
                IsActive = child.IsActive,
                Measurements = child.Measurements?.Select(m => m.Convert()).ToList(),
                UserGuid = userGuid,
                WhenArchived = child.WhenArchived
            };
        }

        public static MongoMeasurement Convert(this ChildMeasurement measurement)
        {
            return new MongoMeasurement
            {
                Date = measurement.Date,
                Height = measurement.Height,
                Weight = measurement.Weight
            };
        }

        public static ChildMeasurement Convert(this MongoMeasurement measurement)
        {
            return new ChildMeasurement
            {
                Date = measurement.Date,
                Height = measurement.Height,
                Weight = measurement.Weight
            };
        }
    }
}
