using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace AAS.Architecture.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrWhiteSpace([CanBeNull] this string field) => string.IsNullOrWhiteSpace(field);

        public static bool IsEmail([NotNull] this string text)
        {
            var EmailRegex = new Regex(
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture);

            return EmailRegex.IsMatch(text);
        }

        [CanBeNull]
        public static string TakeIfNotNullOrWhiteSpace([CanBeNull] this string text, [CanBeNull] string defaultValue)
            => text.IsNullOrWhiteSpace() ? defaultValue : text;
    }
}