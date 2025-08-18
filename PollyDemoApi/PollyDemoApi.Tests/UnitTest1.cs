using Xunit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Polly;
using Polly.Wrap;
using PollyDemoApi.Data;
using PollyDemoApi.Controllers;
using PollyDemoApi.Services;
using PollyDemoApi.Policies;
using System.Threading.Tasks;
using System;

namespace PollyDemoApi.Tests
{
    public class UnitTest1
    {
        // --------------------------
        // Simple test
        // --------------------------
        [Fact]
        public void Test1_ShouldPass()
        {
            Assert.True(true);
        }

        // --------------------------
        // In-memory DbContext setup
        // --------------------------
        private AppDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // unique DB per test
                .Options;
            return new AppDbContext(options);
        }

        // --------------------------
        // TestDbController Tests
        // --------------------------
        [Fact]
        public void Controller_CanBeConstructed()
        {
            var db = GetInMemoryDb();
            var policy = Policy.WrapAsync<object>(
                Policy.NoOpAsync<object>(),
                Policy.NoOpAsync<object>()
            );
            var executor = new ResilientDbExecutor(policy);

            var controller = new TestDbController(db, executor);

            Assert.NotNull(controller);
        }

        [Fact]
        public async Task AddTestItem_ShouldAddTestItem()
        {
            var db = GetInMemoryDb();
            var policy = Policy.WrapAsync<object>(
                Policy.NoOpAsync<object>(),
                Policy.NoOpAsync<object>()
            );
            var executor = new ResilientDbExecutor(policy);
            var controller = new TestDbController(db, executor);

            await controller.AddItem("TestItem");

            var item = await db.TestItems.FirstOrDefaultAsync();
            Assert.NotNull(item);
            Assert.Equal("TestItem", item.Name);
        }

        // --------------------------
        // Polly Policy Factory Tests
        // --------------------------
        [Fact]
        public void CreateDatabasePolicy_ShouldReturnPolicyWrap()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var logger = NullLogger.Instance;

            AsyncPolicyWrap<object> policy = PollyPolicyFactory.CreateDatabasePolicy(config, logger);

            Assert.NotNull(policy);
        }

        [Fact]
        public void CreateExternalServicePolicy_ShouldReturnPolicyWrap()
        {
            var config = new ConfigurationBuilder().AddInMemoryCollection().Build();
            var logger = NullLogger.Instance;

            AsyncPolicyWrap<string> policy = PollyPolicyFactory.CreateExternalServicePolicy(config, logger);

            Assert.NotNull(policy);
        }

        // --------------------------
        // ResilientDbExecutor Tests
        // --------------------------
        private AsyncPolicyWrap<object> GetNoOpPolicy()
        {
            return Policy.WrapAsync(
                Policy.NoOpAsync<object>(),
                Policy.NoOpAsync<object>()
            );
        }

        [Fact]
        public async Task ExecuteAsync_ShouldRunDelegate()
        {
            var executor = new ResilientDbExecutor(GetNoOpPolicy());
            bool wasCalled = false;

            var result = await executor.ExecuteAsync(() =>
            {
                wasCalled = true;
                return Task.FromResult<object>("Executed");
            });

            Assert.True(wasCalled);
            Assert.Equal("Executed", result);
        }

        [Fact]
        public async Task ExecuteAsync_WhenException_Throws()
        {
            var executor = new ResilientDbExecutor(GetNoOpPolicy());

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
