namespace AAS.Architecture.Language
{
    public static class SupportedLanguage
    {
        public static string GetLanguage(string language) => string.Equals(language, "pl", System.StringComparison.OrdinalIgnoreCase) ? "pl-PL" : "en-US";
    }
}