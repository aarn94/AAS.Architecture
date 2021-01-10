using System;
using AAS.Architecture.Exceptions;

namespace AAS.Architecture.Entities
{
    public class AggregateId<T> : IEquatable<AggregateId<T>> where T:IEquatable<T>
    {
        public T Value { get; }

        public AggregateId(T value)
        {
            if (value.Equals(default(T)))
            {
                throw new InvalidAggregateIdException(value);
            }

            Value = value;
        }

        public bool Equals(AggregateId<T> other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((AggregateId<T>) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public static implicit operator T(AggregateId<T> id)
            => id.Value;

        public static implicit operator AggregateId<T>(T id)
            => new AggregateId<T>(id);

        public override string ToString() => Value.ToString();
    }
}