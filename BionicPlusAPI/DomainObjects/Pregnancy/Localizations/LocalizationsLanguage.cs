

using Microsoft.VisualBasic;

namespace DomainObjects.Pregnancy.Localizations
{
    public enum LocalizationsLanguage
    {
        Undifined = 0,
        ru = 1,
        en = 2,
    }

    public static class LocalizationsLanguageExtensions
    {
        public static string GetDateFormat(this LocalizationsLanguage language)
        {
            return language switch
            {
                LocalizationsLanguage.ru => "dd.MM.yyyy",
                LocalizationsLanguage.en => "YYYY-MM-DD",
                _ => throw new ArgumentException("Unknown localization language")
            };
        }
    }
}
