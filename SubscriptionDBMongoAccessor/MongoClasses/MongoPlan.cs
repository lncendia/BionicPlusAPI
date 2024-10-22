using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DomainObjects.Subscription;

namespace SubscriptionDBMongoAccessor.MongoClasses
{
    internal class MongoPlan
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; } = string.Empty;

        [BsonElement("billingPeriod")]
        public BillingPeriod BillingPeriod { get; set; }

        [BsonElement("billingUnit")]
        public int BillingUnit { get; set; }

        [BsonElement("creationDate")]
        public DateTime CreationDate { get; set; }

        [BsonElement("expirationDate")]
        public DateTime ExpirationDate { get; set; }

        [BsonElement("resellerId")]
        public string ResellerId { get; set; } = string.Empty;

        [BsonElement("price")]
        public decimal Price { get; set; }

        [BsonElement("currency")]
        public Currency Currency { get; set; }

        [BsonElement("limits")]
        public Limit? Limits { get; set; }
    }
}
