using System;
using System.Collections.Generic;
using System.Linq;
using AAS.Architecture.Entities;
using AAS.Architecture.Events;

namespace AAS.Architecture.EventStore
{
    public abstract class EventAggregateRoot<T>: AggregateRoot<T>, IEventBaseAggregate<T> where T : IEquatable<T>, new()
    {
        private readonly ICollection<IDomainEvent> uncommittedEvents = new LinkedList<IDomainEvent>();
        
        protected override void AddEvent(IDomainEvent @event)
        {
            AddEventToList(@event);
            
            ApplyEvent(@event);
            uncommittedEvents.Add(@event);
        }

        public void NewEvent(ISyncDomainEvent @event) => AddEvent(@event);

        public void ApplyEvent(IDomainEvent @event)
        {
            Version++;
            Apply(@event);
        }
        
        public IEnumerable<IDomainEvent> GetUncommittedEvents() => uncommittedEvents;

        public void ClearUncommittedEvents() => uncommittedEvents.Clear();
        public void LoadFromHistory(IEnumerable<ISyncDomainEvent> historyEvents)
        {
            var events = historyEvents.ToList().OrderBy(e => e.Time);
            
            foreach (var domainEvent in events)
                ApplyEvent(domainEvent);
        }

        public abstract void Apply(IDomainEvent @event);
    }
    
}