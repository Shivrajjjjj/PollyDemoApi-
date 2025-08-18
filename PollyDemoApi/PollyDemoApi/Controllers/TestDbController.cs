using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PollyDemoApi.Data;
using PollyDemoApi.Services;

namespace PollyDemoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestDbController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ResilientDbExecutor _dbExecutor;

        public TestDbController(AppDbContext context, ResilientDbExecutor dbExecutor)
        {
            _context = context;
            _dbExecutor = dbExecutor;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddItem(string name)
        {
            var item = new TestItem { Name = name };

            await _dbExecutor.ExecuteAsync(async () =>
            {
                _context.TestItems.Add(item);
                await _context.SaveChangesAsync();
            });

            return Ok("Item added");
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var items = await _dbExecutor.ExecuteAsync(() =>
                _context.TestItems.AsNoTracking().ToListAsync());

            return Ok(items);
        }
    }
}