using System;
using System.Collections.Generic;
using System.Linq;
using AAS.Architecture.Events;

namespace AAS.Architecture.Entities
{
    public abstract class AggregateRoot<T> : IBaseAggregate<T>
        where T: IEquatable<T>
    {
        private readonly ISet<IDomainEvent> events = new HashSet<IDomainEvent>();
        public IEnumerable<IDomainEvent> Events => events;
        public AggregateId<T> Id { get; set; }
        public int Version { get; set; }

        protected virtual void AddEvent(IDomainEvent @event)
        {
            if (!events.Any())
            {
                Version++;
            }

            events.Add(@event);
        }

        public void ClearEvents() => events.Clear();

        protected void AddEventToList(IDomainEvent @event) => this.events.Add(@event);
    }
}