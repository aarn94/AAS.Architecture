using System;

namespace AAS.Architecture.Exceptions
{
    public class ExternalException: Exception
    {
        public string Code { get; }

        public ExternalException(string message, string code): base(message) => Code = code;
    }
}