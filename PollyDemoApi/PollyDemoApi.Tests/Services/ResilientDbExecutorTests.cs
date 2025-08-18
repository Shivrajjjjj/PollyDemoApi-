using Xunit;
using Polly;
using Polly.Wrap;        // <- Add this
using PollyDemoApi.Services;
using System;
using System.Threading.Tasks;


namespace PollyDemoApi.Tests.Services
{
    public class ResilientDbExecutorTests
    {
        private AsyncPolicyWrap<object> GetNoOpPolicy()
        {
            // Combine two no-op policies for testing
            return Policy.WrapAsync(
                Policy.NoOpAsync<object>(),
                Policy.NoOpAsync<object>()
            );
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRunDelegate()
        {
            // Arrange
            var executor = new ResilientDbExecutor(GetNoOpPolicy());
            bool wasCalled = false;

            // Act
            var result = await executor.ExecuteAsync(() =>
            {
                wasCalled = true;
                return Task.FromResult<object>("Executed");
            });

            // Assert
            Assert.True(wasCalled);
            Assert.Equal("Executed", result);
        }

        [Fact]
        public async Task ExecuteAsync_WhenException_Throws()
        {
            // Arrange
            var executor = new ResilientDbExecutor(GetNoOpPolicy());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await executor.ExecuteAsync<object>(async () =>
                {
                    throw new InvalidOperationException("Test exception");
                });
            });
        }
    }
}
