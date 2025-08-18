using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PollyDemoApi.Data;
//using PollyDemoApi.Models;
using PollyDemoApi.Services;

namespace PollyDemoApi.Pages
{
    public class AddExternalCallModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ResilientDbExecutor _executor;

        public AddExternalCallModel(AppDbContext context, ResilientDbExecutor executor)
        {
            _context = context;
            _executor = executor;
        }

        [BindProperty]
        public ExternalServiceCall NewExternalCall { get; set; } = new();

        public string Result { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            await _executor.ExecuteAsync(async () =>
            {
                _context.ExternalServiceCalls.Add(NewExternalCall);
                await _context.SaveChangesAsync();
            });

            Result = "External service call logged!";
            NewExternalCall = new();
            return Page();
        }
    }
}