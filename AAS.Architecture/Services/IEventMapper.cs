using System.Collections.Generic;
using AAS.Architecture.Events;
using Convey.CQRS.Events;

namespace AAS.Architecture.Services
{
    public interface IEventMapper
    {
        IEvent Map(IDomainEvent @event);
        IEnumerable<IEvent> MapAll(IEnumerable<IDomainEvent> events);
    }
}