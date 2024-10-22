using DomainObjects.Pregnancy.Localizations;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PregnancyDBMongoAccessor.MongoClasses
{
    internal class MongoLocalization
    {
        [BsonElement("type")]
        public LocalizationType Type { get; set; }

        [BsonElement("values")]
        public Dictionary<string, string> LocalizationValues { get; set; } = new Dictionary<string, string>();
    }
}
