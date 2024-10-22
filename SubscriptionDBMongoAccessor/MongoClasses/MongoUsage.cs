using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SubscriptionDBMongoAccessor.MongoClasses
{
    internal class MongoUsage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        public string? UserId { get; set; }

        [BsonElement("surveyUsage")]
        public int SurveyUsage { get; set; }

        [BsonElement("rollbackUsage")]
        public int RollbackUsage { get; set; }
    }
}
