using System;

namespace AAS.Architecture.Entities
{
    public interface IBaseAggregate<T> where T : IEquatable<T>
    {
        public AggregateId<T> Id { get; set; }
    }
}