using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PregnancyDBMongoAccessor.MongoClasses
{
    internal class MongoCondition
    {
        [BsonElement("expressions")]
        public List<MongoExpression> Expressions { get; set; } = new List<MongoExpression>();

        [BsonElement("defaultCardId")]
        public string DefaultCardId { get; set; } = string.Empty;
    }
}
