using DomainObjects.Pregnancy;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;


namespace PregnancyDBMongoAccessor.MongoClasses
{
    internal class MongoParameter
    {
        [BsonElement("parameterType")]
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public ParameterType ParameterType { get; set; }

        [BsonElement("expression")]
        public MongoExpression? Expression { get; set; } 

        [BsonElement("value")]
        public string? Value { get; set; } = string.Empty;
    }
}
