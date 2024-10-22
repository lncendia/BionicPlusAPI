using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Subscription
{
    public class BillingProfile
    {
        public List<string> SubscriptionIds { get; set; } = new List<string>();
        public string ActiveSubscriptionId { get; set; } = string.Empty;
        public bool isFreePlan { get; set; }
    }
}
