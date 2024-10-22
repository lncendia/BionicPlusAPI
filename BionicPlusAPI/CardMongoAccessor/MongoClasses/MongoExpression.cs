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
    internal class MongoExpression
    {
        [BsonElement("leftParameter")]
        public MongoParameter LeftParameter { get; set; } = new MongoParameter();

        [BsonElement("rightParameter")]
        public MongoParameter RightParameter { get; set; } = new MongoParameter();

        [BsonElement("operationType")]
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public OperationType OperationType { get; set; }

        [BsonElement("cardId")]
        public string CardId { get; set; } = string.Empty;
    }
}
