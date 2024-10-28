using DomainObjects.Subscription;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace SubscriptionDBMongoAccessor.MongoClasses
{
    [BsonIgnoreExtraElements]
    internal class MongoSubscription
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("creationDate")]
        public DateTime CreationDate { get; set; }

        [BsonElement("expirationDate")]
        public DateTime ExpirationDate { get; set; }

        [BsonElement("limits")]
        public Limit? Limits { get; set; }

        [BsonElement("invoiceId")]
        public int InvoiceId { get; set; } 

        [BsonElement("status")]
        public SubscriptionStatus Status { get; set; }

        [BsonElement("planId")]
        public string PlanId { get; set; } = string.Empty;

        [BsonElement("isRecurrent")]
        public bool isRecurrent { get; set; }

        [BsonElement("total")]
        public decimal Total { get; set; }

        [BsonElement("currency")]
        public Currency Currency { get; set; }

        [BsonElement("discount")]
        [BsonIgnoreIfDefault]
        public decimal Discount { get; set; }

        [BsonElement("promocode")]
        [BsonIgnoreIfDefault]
        public string? Promocode { get; set; }
        
        [BsonElement("cancellationTime")]
        public DateTime CancellationTime { get; set; }
    }
}
