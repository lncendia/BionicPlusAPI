using MongoDB.Bson.Serialization.Attributes;

namespace PregnancyDBMongoAccessor.MongoClasses
{
    public class MongoMeasurement
    {
        [BsonElement("date")]
        public DateOnly Date { get; set; }

        [BsonElement("height")]
        public double? Height { get; set; }

        [BsonElement("weight")]
        public double? Weight { get; set; }
    }
}