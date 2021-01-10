using System.Collections.Generic;

namespace AAS.Architecture.Extensions
{
    public static class ListExtensions
    {
        // ReSharper disable once MA0016
        public static void Add<T>(this List<T> @this, IEnumerable<T> items) => @this.AddRange(items);
    }
}