using System;
using System.Globalization;

namespace AAS.Architecture.Exceptions
{
    public abstract class DomainException : Exception
    {
        public string Lang { get; }
        public abstract string Code { get; }
        public abstract string TranslationKey { get; }
        public virtual string[] TranslationParameters => Array.Empty<string>();

        protected DomainException(string message) : base(message) => Lang = CultureInfo.CurrentCulture.Name;
    }
}