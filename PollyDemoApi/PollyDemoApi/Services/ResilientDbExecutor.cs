using Polly.Wrap;
using System;
using System.Threading.Tasks;

namespace PollyDemoApi.Services
{
    public class ResilientDbExecutor
    {
        private readonly AsyncPolicyWrap<object> _policy;

        public ResilientDbExecutor(AsyncPolicyWrap<object> policy)
        {
            _policy = policy;
        }

        public async Task<T?> ExecuteAsync<T>(Func<Task<T>> operation)
        {
            var result = await _policy.ExecuteAsync(async () => (object?)await operation());
            return result is T typed ? typed : default;
        }

        public async Task ExecuteAsync(Func<Task> operation)
        {
            await _policy.ExecuteAsync(async () =>
            {
                await operation();
                return new object();
            });
        }
    }
}