using System;
using System.Threading.Tasks;

namespace PollyDemoApi.Services
{
    public class FakeUnreliableService
    {
        private static readonly Random _random = new();

        public async Task<string> GetDataAsync()
        {
            await Task.Delay(500); // Simulate latency

            int roll = _random.Next(1, 5); // 75% chance to fail
            if (roll != 3)
            {
                Console.WriteLine($"[FakeUnreliableService] Failure simulated (roll={roll})");
                throw new Exception("Simulated failure");
            }

            Console.WriteLine($"[FakeUnreliableService] Success (roll={roll})");
            return "{ \"status\": \"success\", \"data\": \"Weather data retrieved\" }";
        }
    }
}