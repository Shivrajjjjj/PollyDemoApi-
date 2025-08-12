using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

public class PollyStatusModel : PageModel
{
    private readonly IConfiguration _config;
    private readonly IHttpClientFactory _httpClientFactory;

    public PollyStatusModel(IConfiguration config, IHttpClientFactory httpClientFactory)
    {
        _config = config;
        _httpClientFactory = httpClientFactory;
    }

    public int RetryCount { get; set; }
    public int RetryBaseDelayMs { get; set; }
    public int CircuitBreakerAllowedFailures { get; set; }
    public int CircuitBreakerBreakSeconds { get; set; }
    public int TimeoutSeconds { get; set; }

    public string? ApiResponse { get; set; }

    public void OnGet()
    {
        var section = _config.GetSection("PollySettings");
        RetryCount = section.GetValue<int>("RetryCount");
        RetryBaseDelayMs = section.GetValue<int>("RetryBaseDelayMs");
        CircuitBreakerAllowedFailures = section.GetValue<int>("CircuitBreakerAllowedFailures");
        CircuitBreakerBreakSeconds = section.GetValue<int>("CircuitBreakerBreakSeconds");
        TimeoutSeconds = section.GetValue<int>("TimeoutSeconds");
    }


    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OnPostAsync()
    {
        OnGet(); // Reload Polly config values

        var client = new HttpClient(); // No SSL bypass needed now

        try
        {
            var response = await client.GetAsync("https://localhost:7105/api/weather/getdata"); 
            ApiResponse = await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            ApiResponse = $"Error: {ex.Message}\n\nDetails: {ex.InnerException?.Message}";
        }

        return Page();
    }
}