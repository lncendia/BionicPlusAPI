namespace BionicPlusAPI.Helpers
{
    public static class StringUtils
    {
        public static string GetFirstCharacters(this string? text, int charactersCount)
        {
            if(string.IsNullOrWhiteSpace(text))
            {
                return string.Empty;
            }
            int offset = Math.Min(charactersCount, text.Length);
            return text.Substring(0, offset);
        }
    }
}
