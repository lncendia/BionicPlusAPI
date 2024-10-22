using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Pregnancy
{
    public enum UserInputType
    {
        Undefined = 0,
        Temperature = 1,
        BloodPressure = 2,
        GestationPeriod = 3,
        PainLevel = 4,
        ProteinInUrine = 5,
        CustomAnswer = 6,
    }
}
