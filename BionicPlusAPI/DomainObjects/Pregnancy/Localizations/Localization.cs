using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainObjects.Pregnancy.Localizations
{
    public class Localization
    {
        public LocalizationType Type { get; private set; }
        public Dictionary<LocalizationsLanguage, string> LocalizationValues { get; private set; } = new Dictionary<LocalizationsLanguage, string>();
        public Localization(LocalizationType type, LocalizationsLanguage? language, string? value)
        {
            Type = type;
            if(value is not null)
            {
                SetLocalizationValue(language, value);
            }
        }

        public void SetLocalizationValue(LocalizationsLanguage? language, string value)
        {
            if(language != null)
            {
                LocalizationValues.Add((LocalizationsLanguage)language, value);
            }
        }

        public string GetLocalizationByLanguage(LocalizationsLanguage language)
        {
            string? value = String.Empty;
            LocalizationValues.TryGetValue(language, out value);
            return value ?? String.Empty;
        }

    }
}
