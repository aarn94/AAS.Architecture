using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace AAS.Architecture.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TResult> LeftOuterJoin<TOuter, TInner, TKey, TResult>(
            [NotNull] this IEnumerable<TOuter> outer,
            [NotNull] IEnumerable<TInner> inner,
            [NotNull] Func<TOuter, TKey> outerKeySelector,
            [NotNull] Func<TInner, TKey> innerKeySelector,
            [NotNull] Func<TOuter, TInner, TResult> resultSelector) =>
            outer.GroupJoin(inner, outerKeySelector, innerKeySelector, (a, b) => new
            {
                a, b
            }).SelectMany(x => x.b.DefaultIfEmpty(), (x, b) => resultSelector(x.a, b));
    }
}