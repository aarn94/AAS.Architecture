using System.Collections.Generic;

namespace AAS.Architecture.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> ToList<T>(this T element, params T[] array) => new List<T>{new List<T> {element}, array};
    }
}