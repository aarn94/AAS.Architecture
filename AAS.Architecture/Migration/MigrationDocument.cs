using System;
using Convey.Types;

namespace AAS.Architecture.Migration
{
    public class MigrationDocument: IIdentifiable<Guid>
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
        public string Name { get; set; }
    }
}