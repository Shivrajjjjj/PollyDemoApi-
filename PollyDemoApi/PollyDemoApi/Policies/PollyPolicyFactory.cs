using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Bulkhead;
using Polly.Timeout;
using Polly.Wrap;
using System;
using System.Threading.Tasks;

namespace PollyDemoApi.Policies
{
    public static class PollyPolicyFactory
    {
        public static AsyncPolicyWrap<object> CreateDatabasePolicy(IConfiguration config, ILogger logger)
        {
            int retryCount = config.GetValue<int>("PollySettings:RetryCount", 3);
            int baseDelay = config.GetValue<int>("PollySettings:RetryBaseDelayMs", 200);
            int circuitBreak = config.GetValue<int>("PollySettings:CircuitBreakerAllowedFailures", 2);
            int circuitBreakTime = config.GetValue<int>("PollySettings:CircuitBreakerBreakSeconds", 15);
            int timeoutSeconds = config.GetValue<int>("PollySettings:TimeoutSeconds", 2);
            int maxParallel = config.GetValue<int>("PollySettings:BulkheadMaxParallelization", 10);
            int maxQueue = config.GetValue<int>("PollySettings:BulkheadMaxQueuingActions", 20);

            var retryPolicy = Policy<object>
                .Handle<Exception>()
                .WaitAndRetryAsync(retryCount,
                    attempt => TimeSpan.FromMilliseconds(baseDelay * Math.Pow(2, attempt)),
                    (ex, ts, attempt, ctx) =>
                        logger.LogWarning($"[DB Retry {attempt}] {ex.Exception?.Message}")
                );

            var circuitBreakerPolicy = Policy<object>
                .Handle<Exception>()
                .CircuitBreakerAsync(circuitBreak, TimeSpan.FromSeconds(circuitBreakTime),
                    onBreak: (ex, ts) => logger.LogWarning($"[DB Circuit] Opened for {ts.TotalSeconds}s"),
                    onReset: () => logger.LogInformation("[DB Circuit] Reset"),
                    onHalfOpen: () => logger.LogInformation("[DB Circuit] Half-open")
                );

            var timeoutPolicy = Policy.TimeoutAsync<object>(TimeSpan.FromSeconds(timeoutSeconds));

            var fallbackPolicy = Policy<object>
                .Handle<Exception>()
                .FallbackAsync(async (token) =>
                {
                    logger.LogWarning("[DB Fallback] Executed fallback.");
                    return await Task.FromResult<object?>(null);
                });

            var bulkheadPolicy = Policy.BulkheadAsync<object>(
                maxParallelization: maxParallel,
                maxQueuingActions: maxQueue,
                onBulkheadRejectedAsync: context =>
                {
                    logger.LogWarning("[DB Bulkhead] Rejected due to max concurrency.");
                    return Task.CompletedTask;
                });

            return Policy.WrapAsync(fallbackPolicy, bulkheadPolicy, circuitBreakerPolicy, retryPolicy, timeoutPolicy);
        }

        public static AsyncPolicyWrap<string> CreateExternalServicePolicy(IConfiguration config, ILogger logger)
        {
            int retryCount = config.GetValue<int>("PollySettings:RetryCount", 3);
            int baseDelay = config.GetValue<int>("PollySettings:RetryBaseDelayMs", 200);
            int circuitBreak = config.GetValue<int>("PollySettings:CircuitBreakerAllowedFailures", 2);
            int circuitBreakTime = config.GetValue<int>("PollySettings:CircuitBreakerBreakSeconds", 15);
            int timeoutSeconds = config.GetValue<int>("PollySettings:TimeoutSeconds", 2);
            int maxParallel = config.GetValue<int>("PollySettings:BulkheadMaxParallelization", 10);
            int maxQueue = config.GetValue<int>("PollySettings:BulkheadMaxQueuingActions", 20);

            var retryPolicy = Policy<string>
                .Handle<Exception>()
                .WaitAndRetryAsync(retryCount,
                    attempt => TimeSpan.FromMilliseconds(baseDelay * Math.Pow(2, attempt)),
                    (ex, ts, attempt, ctx) =>
                        logger.LogWarning($"[Service Retry {attempt}] {ex.Exception?.Message}")
                );

            var circuitBreakerPolicy = Policy<string>
                .Handle<Exception>()
                .CircuitBreakerAsync(circuitBreak, TimeSpan.FromSeconds(circuitBreakTime),
                    onBreak: (ex, ts) => logger.LogWarning($"[Service Circuit] Opened for {ts.TotalSeconds}s"),
                    onReset: () => logger.LogInformation("[Service Circuit] Reset"),
                    onHalfOpen: () => logger.LogInformation("[Service Circuit] Half-open")
                );

            var timeoutPolicy = Policy.TimeoutAsync<string>(TimeSpan.FromSeconds(timeoutSeconds));

            var fallbackPolicy = Policy<string>
                .Handle<Exception>()
                .FallbackAsync(async (ct) =>
                {
                    logger.LogWarning("[Service Fallback] Executed fallback.");
                    return "{ \"status\": \"fallback\", \"data\": null }";
                });

            var bulkheadPolicy = Policy.BulkheadAsync<string>(
                maxParallelization: maxParallel,
                maxQueuingActions: maxQueue,
                onBulkheadRejectedAsync: context =>
                {
                    logger.LogWarning("[Service Bulkhead] Rejected due to max concurrency.");
                    return Task.CompletedTask;
                });

            return Policy.WrapAsync(fallbackPolicy, bulkheadPolicy, circuitBreakerPolicy, retryPolicy, timeoutPolicy);
        }
    }
}
