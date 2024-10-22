using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainObjects.Pregnancy;
using System.Text.Json.Serialization;

namespace PregnancyDBMongoAccessor.MongoClasses
{
    [BsonIgnoreExtraElements]
    internal class MongoPregnancy
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("multiple")]
        public bool Multiple { get; set; }

        [BsonElement("weighings")]
        public List<MongoWeighing>? Weighings { get; set; }

        [BsonElement("isActive")]
        public bool IsActive { get; set; }

        [BsonElement("userGuid")]
        public Guid UserGuid { get; set; }

        [BsonElement("whenArchived")]
        public DateTime? WhenArchived { get; set; }
    }
}
