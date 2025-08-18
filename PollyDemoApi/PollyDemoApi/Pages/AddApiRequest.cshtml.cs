using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PollyDemoApi.Data;
//using PollyDemoApi.Models;
using PollyDemoApi.Services;

namespace PollyDemoApi.Pages
{
    public class AddApiRequestModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ResilientDbExecutor _executor;

        public AddApiRequestModel(AppDbContext context, ResilientDbExecutor executor)
        {
            _context = context;
            _executor = executor;
        }

        [BindProperty]
        public ApiRequest NewApiRequest { get; set; } = new();

        public string Result { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            await _executor.ExecuteAsync(async () =>
            {
                _context.ApiRequests.Add(NewApiRequest);
                await _context.SaveChangesAsync();
            });

            Result = "API request logged successfully!";
            NewApiRequest = new();
            return Page();
        }
    }
}