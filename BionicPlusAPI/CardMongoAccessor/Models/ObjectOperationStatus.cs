using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PregnancyDBMongoAccessor.Models
{
    public class ObjectOperationStatus
    {
        public long MatchedCount { get; set; }
        public long ModifiedCount { get; set; }
        public long DeletedCount { get; set; }
    }
}
