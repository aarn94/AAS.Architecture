﻿using System;
 using AAS.Architecture.Events;

 namespace AAS.Architecture.EventStore
{
    public interface ISyncDomainEvent: IDomainEvent
    {
        public DateTime Time { get; }
    }
}