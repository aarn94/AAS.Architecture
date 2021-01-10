using System;

namespace AAS.Architecture.Exceptions
{
    public class InvalidAggregateIdException : Exception
    {
        public object Id { get; }

        public InvalidAggregateIdException(object id) : base($"Invalid aggregate id: {id.ToString()}")
            => Id = id;
    }
}