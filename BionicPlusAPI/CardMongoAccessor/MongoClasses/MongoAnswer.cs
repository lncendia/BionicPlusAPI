using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using DomainObjects.Pregnancy;

namespace PregnancyDBMongoAccessor.MongoClasses
{
    internal class MongoAnswer
    {
        [BsonElement("answerId")]
        public Guid? Id { get; set; }

        [BsonElement("title")]
        public MongoLocalization? Title { get; set; }

        [BsonElement("description")]
        public MongoLocalization? Description { get; set; }

        [BsonElement("imgInfo")]
        public Image? ImgInfo { get; set; }

        [BsonElement("relatedCardId")]
        public string? RelatedCardId { get; set; }
    }
}