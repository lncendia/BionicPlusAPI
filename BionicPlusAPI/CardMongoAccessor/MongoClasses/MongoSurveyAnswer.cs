using DomainObjects.Pregnancy;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PregnancyDBMongoAccessor.MongoClasses
{
    internal class MongoSurveyAnswer
    {
        [BsonElement("cardId")]
        public string? CardId { get; set; }

        [BsonElement("answerId")]
        [BsonIgnoreIfNull]
        public string? AnswerId { get; set; }

        [BsonElement("userInputType")]
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        [BsonIgnoreIfNull]
        public UserInputType? UserInputType { get; set; }

        [BsonElement("userInputValue")]
        [BsonIgnoreIfNull]
        public string? UserInputValue { get; set; }

        [BsonElement("Date")]
        public DateTime? Date { get; set; }
    }
}
