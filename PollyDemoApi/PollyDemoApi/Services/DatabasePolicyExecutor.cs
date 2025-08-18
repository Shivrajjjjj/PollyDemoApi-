using Microsoft.Extensions.Logging;
using Polly.Wrap;
using System;
using System.Threading.Tasks;

namespace PollyDemoApi.Services
{
    public class DatabasePolicyExecutor
    {
        private readonly ILogger<DatabasePolicyExecutor> _logger;
        private readonly AsyncPolicyWrap<object> _policyWrap;

        public DatabasePolicyExecutor(ILogger<DatabasePolicyExecutor> logger, AsyncPolicyWrap<object> policyWrap)
        {
            _logger = logger;
            _policyWrap = policyWrap;
        }

        public async Task<T?> ExecuteAsync<T>(Func<Task<T>> action)
        {
            var result = await _policyWrap.ExecuteAsync(async () => (object?)await action());
            return result is T typedResult ? typedResult : default;
        }

        public async Task ExecuteAsync(Func<Task> action)
        {
            await _policyWrap.ExecuteAsync(async () =>
            {
                await action();
                return new object();
            });
        }
    }
}