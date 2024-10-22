using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PregnancyDBMongoAccessor.MongoClasses
{
    internal class MongoWeighing
    {
        [BsonElement("date")]
        public DateOnly Date { get; set; }

        [BsonElement("value")]
        public double Value { get; set; }
    }
}
