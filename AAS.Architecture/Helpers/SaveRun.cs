using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Polly;

namespace AAS.Architecture.Helpers
{
    public class SaveRun
    {
        public static Task<TDto> RunWithRepeatAsync<TDto, TLog>(Func<Task<TDto>> apiLogic, [NotNull] string actionName, ILogger<TLog> logger, int retryCount = 5, long waitTimeMs = 1000) =>
            Policy.Handle<Exception>()
                .WaitAndRetryAsync(retryCount:retryCount, sleepDurationProvider:(errorNumber) => TimeSpan.FromMilliseconds(waitTimeMs * errorNumber), (exception, span, number, context) => logger.LogWarning(exception, $"Repeating {actionName} number {number}"))
                .ExecuteAsync(apiLogic);
        
        public static  Task RunWithRepeatAsync<TLog>(Func<Task> apiLogic, [NotNull] string actionName, ILogger<TLog> logger, int retryCount = 5, long waitTimeMs = 1000) =>
            Policy.Handle<Exception>()
                .WaitAndRetryAsync(retryCount:retryCount, sleepDurationProvider:(errorNumber) => TimeSpan.FromMilliseconds(waitTimeMs * errorNumber), (exception, span, number, context) => logger.LogWarning(exception, $"Repeating {actionName} number {number}"))
                .ExecuteAsync(apiLogic);
    }
}