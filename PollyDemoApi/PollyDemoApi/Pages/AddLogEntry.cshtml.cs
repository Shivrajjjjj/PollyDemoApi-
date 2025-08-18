using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PollyDemoApi.Data;
//using PollyDemoApi.Models;
using PollyDemoApi.Services;

namespace PollyDemoApi.Pages
{
    public class AddLogEntryModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ResilientDbExecutor _executor;

        public AddLogEntryModel(AppDbContext context, ResilientDbExecutor executor)
        {
            _context = context;
            _executor = executor;
        }

        [BindProperty]
        public LogEntry NewLogEntry { get; set; } = new();

        public string Result { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            await _executor.ExecuteAsync(async () =>
            {
                _context.LogEntries.Add(NewLogEntry);
                await _context.SaveChangesAsync();
            });

            Result = "Log entry added!";
            NewLogEntry = new();
            return Page();
        }
    }
}