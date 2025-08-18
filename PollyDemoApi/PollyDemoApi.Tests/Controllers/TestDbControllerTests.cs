using Xunit;
using Microsoft.EntityFrameworkCore;
using Polly;
using PollyDemoApi.Controllers;
using PollyDemoApi.Data;
using PollyDemoApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PollyDemoApi.Tests.Controllers
{
    public class TestDbControllerTests
    {
        // Use unique in-memory database per test to avoid shared state
        private AppDbContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

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

        [Fact]
        public async Task GetAll_ShouldReturnAllItems()
        {
            var db = GetInMemoryDb();
            var policy = Policy.WrapAsync<object>(
                Policy.NoOpAsync<object>(),
                Policy.NoOpAsync<object>()
            );
            var executor = new ResilientDbExecutor(policy);
            var controller = new TestDbController(db, executor);

            // Add 2 items
            await controller.AddItem("Item1");
            await controller.AddItem("Item2");

            var result = await controller.GetAll() as OkObjectResult;
            var items = result?.Value as List<TestItem>;

            Assert.NotNull(items);
            Assert.Equal(2, items.Count); // now isolated DB, should pass
        }
    }
}
