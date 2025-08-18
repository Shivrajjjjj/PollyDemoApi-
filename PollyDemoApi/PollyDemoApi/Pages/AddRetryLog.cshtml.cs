using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PollyDemoApi.Data;
//using PollyDemoApi.Models;
using PollyDemoApi.Services;

namespace PollyDemoApi.Pages
{
    public class AddRetryLogModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ResilientDbExecutor _executor;

        public AddRetryLogModel(AppDbContext context, ResilientDbExecutor executor)
        {
            _context = context;
            _executor = executor;
        }

        [BindProperty]
        public RetryLog NewRetryLog { get; set; } = new();

        public string Result { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            await _executor.ExecuteAsync(async () =>
            {
                _context.RetryLogs.Add(NewRetryLog);
                await _context.SaveChangesAsync();
            });

            Result = "Retry log saved!";
            NewRetryLog = new();
            return Page();
        }
    }
}