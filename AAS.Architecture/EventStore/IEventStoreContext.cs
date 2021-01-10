﻿﻿using NEventStore;

  namespace AAS.Architecture.EventStore
{
    public interface IEventStoreContext
    {
        public IStoreEvents Events { get; }
    }
}