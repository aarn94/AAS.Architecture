using System;

namespace AAS.Architecture.Services
{
    public interface IDateTimeProvider
    {
        DateTime Now { get; }
        
        DateTime UtcNow { get; }
    }
}