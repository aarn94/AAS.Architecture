using System;
using System.Reactive.Linq;

namespace AAS.Architecture.Extensions
{
    public static class ObservableExtensions
    {
        public static IObservable<Tuple<TSource, TSource>>
            PairWithPrevious<TSource>(this IObservable<TSource> source) =>
            source.Scan(
                Tuple.Create(default(TSource), default(TSource)),
                (acc, current) => Tuple.Create(acc.Item2, current));
    }
}