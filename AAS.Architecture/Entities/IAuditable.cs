using System;

namespace AAS.Architecture.Entities
{
    public interface IAuditable
    {
        public string ModifiedBy { get;  }
        public DateTime ModifiedOn { get; }
        public bool IsDeleted { get; }
    }
}