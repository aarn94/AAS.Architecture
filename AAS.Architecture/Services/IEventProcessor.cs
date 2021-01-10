using System.Collections.Generic;
using System.Threading.Tasks;
using AAS.Architecture.Events;

namespace AAS.Architecture.Services
{
    public interface IEventProcessor
    {
        Task ProcessAsync(IEnumerable<IDomainEvent> events);
    }
}