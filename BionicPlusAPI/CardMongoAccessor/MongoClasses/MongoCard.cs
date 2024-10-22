using DomainObjects.Pregnancy;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using DomainObjects.Pregnancy.Localizations;

namespace PregnancyDBMongoAccessor.MongoClasses
{
    internal class MongoCard
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("title")]
        public MongoLocalization? Title { get; set; }

        [BsonElement("solutionSteps")]
        public MongoLocalization? SolutionSteps { get; set; }

        [BsonElement("description")]
        public MongoLocalization? Description { get; set; }

        [BsonElement("references")]
        public string? References { get; set; }

        [BsonElement("answers")]
        public List<MongoAnswer>? Answers { get; set; }

        [BsonElement("userInputType")]
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public UserInputType? UserInputType { get; set; }

        [BsonElement("condition")]
        public MongoCondition? Condition { get; set; }

        [BsonElement("emergencyLvl")]
        [JsonConverter(typeof(StringEnumConverter))]  
        [BsonRepresentation(BsonType.String)]
        public EmergencyLvl? EmergencyLvl { get; set; }

        [BsonElement("cardType")]
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public CardType? CardType { get; set; }

        [BsonElement("category")]
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public Category? Category { get; set; }

        [BsonElement("imgInfo")]
        public Image? ImgInfo { get; set; }
    }
}
