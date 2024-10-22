using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DomainObjects.Pregnancy.UserProfile
{
    public class UserAgreement
    {
        [JsonPropertyName("whenOccured")]
        public DateTime? WhenOccured { get; set; }

        [JsonPropertyName("recurringPaymentAgreement")]
        public bool RecurringPaymentAgreement { get; set; } = false;

        [JsonPropertyName("personalDataAgreement")]
        public bool PersonalDataAgreement { get; set; } = false;

        [JsonPropertyName("userAgreementAgreement")]
        public bool UserAgreementAgreement { get; set; } = false;
    }
}
