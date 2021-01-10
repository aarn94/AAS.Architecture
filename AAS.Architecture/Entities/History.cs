using System;
using Convey.Types;
using SmartAnalyzers.CSharpExtensions.Annotations;

namespace AAS.Architecture.Entities
{
    [InitRequired, InitOnly]
    public class History<T>: IIdentifiable<Guid>
        where T: IIdentifiable<Guid>
    {
        public T Data { get; set; }
        public Guid Id { get; set; }
    }
}