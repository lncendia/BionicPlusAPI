using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PregnancyDBMongoAccessor.MongoClasses
{
    internal class MongoRating
    {
        [BsonElement("description")]
        [BsonIgnoreIfNull]
        public string? Description { get; set; }

        [BsonElement("score")]
        [BsonIgnoreIfNull]
        public int? Score { get; set; }
    }
}
