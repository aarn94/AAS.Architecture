using System;
using Convey.Types;

namespace AAS.Architecture.DataBase.Sequence
{
    public class SequenceDocument: IIdentifiable<Guid>
    {
        public string Name { get; set; }

        public long Value { get; set; }

        public Guid Id { get; set; }
    }
}