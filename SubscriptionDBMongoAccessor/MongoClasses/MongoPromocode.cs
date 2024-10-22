using DomainObjects.Subscription;
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
    internal class MongoPromocode
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("promocode")]
        public string? Promocode { get; set; }

        [BsonElement("discountType")]
        public BillingPromocodeDiscountType? DiscountType { get; set; }

        [BsonElement("discountValue")]
        public double? DiscountValue { get; set; }
    }
}
