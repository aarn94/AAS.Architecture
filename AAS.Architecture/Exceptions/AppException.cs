using System;
using System.Globalization;

namespace AAS.Architecture.Exceptions
{
    public abstract class AppException : Exception
    {
        public abstract string Code { get; }
        public abstract string TranslationKey { get; }
        public virtual string[] TranslationParameters => Array.Empty<string>();

        protected AppException(string message) : base(message) => Lang = CultureInfo.CurrentCulture.Name;
        public string Lang { get; }
    }
}