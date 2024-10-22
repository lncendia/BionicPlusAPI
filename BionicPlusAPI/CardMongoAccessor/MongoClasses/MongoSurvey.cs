using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainObjects.Pregnancy;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using DomainObjects.Pregnancy.Localizations;

namespace PregnancyDBMongoAccessor.MongoClasses
{
    internal class MongoSurvey
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; }

        [BsonElement("surveyType")]
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public SurveyType? Type { get; set; }

        [JsonPropertyName("answers")]
        public List<MongoSurveyAnswer>? Answers { get; set; }

        [BsonElement("currentCardId")]
        public string? CurrentCardId { get; set; }

        [BsonElement("startDate")]
        [BsonIgnoreIfNull]
        public DateTime? StartDate { get; set; }

        [BsonElement("endDate")]
        [BsonIgnoreIfNull]
        public DateTime? EndDate { get; set; }

        [BsonElement("deleteDate")]
        [BsonIgnoreIfNull]
        public DateTime? DeleteDate { get; set; }

        [BsonElement("localization")]
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public LocalizationsLanguage? LocalizationLanguage { get; set; }

        [BsonElement("rating")]
        [BsonIgnoreIfNull]
        public MongoRating? Rating { get; set; }
    }
}
