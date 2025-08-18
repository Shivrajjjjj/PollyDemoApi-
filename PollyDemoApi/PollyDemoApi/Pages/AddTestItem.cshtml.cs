using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PollyDemoApi.Data;
//using PollyDemoApi.Models;
using PollyDemoApi.Services;

namespace PollyDemoApi.Pages
{
    public class AddTestItemModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly ResilientDbExecutor _executor;

        public AddTestItemModel(AppDbContext context, ResilientDbExecutor executor)
        {
            _context = context;
            _executor = executor;
        }

        [BindProperty]
        public string Name { get; set; } = string.Empty;

        public string Result { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Result = "Name cannot be empty.";
                return Page();
            }

            var item = new TestItem { Name = Name };

            await _executor.ExecuteAsync(async () =>
            {
                _context.TestItems.Add(item);
                await _context.SaveChangesAsync();
            });

            Result = $"Item '{Name}' added successfully!";
            Name = string.Empty;
            return Page();
        }
    }
}