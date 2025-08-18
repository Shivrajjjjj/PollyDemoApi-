using Xunit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using PollyDemoApi.Policies;
using Polly.Wrap;
using Polly;

namespace PollyDemoApi.Tests.Policies
{
    public class PollyPolicyFactoryTests
    {
        private readonly IConfiguration _config;
        private readonly NullLogger _logger;

        public PollyPolicyFactoryTests()
        {
            // Use in-memory configuration for tests
            _config = new ConfigurationBuilder().AddInMemoryCollection().Build();
            _logger = NullLogger.Instance;
        }

        [Fact]
        public void CreateDatabasePolicy_ShouldReturnPolicyWrap()
        {
            // Act
            AsyncPolicyWrap<object> policy = PollyPolicyFactory.CreateDatabasePolicy(_config, _logger);

            // Assert
            Assert.NotNull(policy);
            Assert.IsType<AsyncPolicyWrap<object>>(policy); // Optional type check
        }

        [Fact]
        public void CreateExternalServicePolicy_ShouldReturnPolicyWrap()
        {
            // Act
            AsyncPolicyWrap<string> policy = PollyPolicyFactory.CreateExternalServicePolicy(_config, _logger);

            // Assert
            Assert.NotNull(policy);
            Assert.IsType<AsyncPolicyWrap<string>>(policy); // Optional type check
        }
    }
}
