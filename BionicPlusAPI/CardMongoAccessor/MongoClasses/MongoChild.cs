using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using DomainObjects.Pregnancy.Children;
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace PregnancyDBMongoAccessor.MongoClasses
{
    [BsonIgnoreExtraElements]
    internal class MongoChild
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("gender")]
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public Gender Gender { get; set; }

        [BsonElement("measurements")]
        public List<MongoMeasurement>? Measurements { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }

        [BsonElement("userGuid")]
        public Guid UserGuid { get; set; }

        [BsonElement("whenArchived")]
        public DateTime? WhenArchived { get; set; }
    }
}
