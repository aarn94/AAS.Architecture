using System;
using System.Collections.Generic;
using AAS.Architecture.Entities;
using AAS.Architecture.Events;

namespace AAS.Architecture.EventStore
{
    public interface IEventBaseAggregate<T>: IBaseAggregate<T> where T : IEquatable<T>
    {
        void NewEvent(ISyncDomainEvent @event);
        void ApplyEvent(IDomainEvent @event);

        IEnumerable<IDomainEvent> GetUncommittedEvents();

        void ClearUncommittedEvents();

        void LoadFromHistory(IEnumerable<ISyncDomainEvent> historyEvents);
    }
}