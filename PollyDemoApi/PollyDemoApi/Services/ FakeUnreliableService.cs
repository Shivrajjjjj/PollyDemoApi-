using System;
namespace PollyDemoApi.Services
{
    public class FakeUnreliableService
    {
        private static readonly Random _random = new();

        public async Task<string> GetDataAsync()
        {
            await Task.Delay(500); // Simulate latency

            if (_random.Next(1, 5) != 3) // 75% chance to fail
                throw new Exception("Simulated failure");

            return "{ \"status\": \"success\", \"data\": \"Weather data retrieved\" }";
        }
    }
}