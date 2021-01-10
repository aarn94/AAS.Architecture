using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
// ReSharper disable VSTHRD003

namespace AAS.Architecture.Extensions
{
    public static class TaskExtensions
    {
        public static async Task<TResult> MapAsync<TSource, TResult>(
            this Task<TSource> task,
            Func<TSource, TResult> selector,
            bool continueOnCapturedContext = false) =>
            selector(await task.ConfigureAwait(continueOnCapturedContext));
        
        public static async ValueTask<TResult> MapAsync<TSource, TResult>(
            this ValueTask<TSource> task,
            Func<TSource, TResult> selector,
            bool continueOnCapturedContext = false) =>
            selector(await task.ConfigureAwait(continueOnCapturedContext));
        
        public static ConfiguredTaskAwaitable<TResult> WithoutCapturingContext<TResult>(this Task<TResult> task) =>
            task.ConfigureAwait(false);

        public static ConfiguredTaskAwaitable WithoutCapturingContext(this Task task) => task.ConfigureAwait(continueOnCapturedContext: false);
    }
}