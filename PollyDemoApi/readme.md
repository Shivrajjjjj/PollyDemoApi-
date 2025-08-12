```markdown
# PollyDemoApi Dashboard

A lightweight ASP.NET Core Razor Pages UI to visualize and test Polly resilience policies (Retry, Timeout, Circuit Breaker, Fallback) configured via `appsettings.json`.

## 🚀 Features

- View current Polly configuration values
- Trigger API calls to test resilience behavior
- Display raw API responses for diagnostics
- Config-driven setup with support for SSL and antiforgery tokens

## 🛠️ Tech Stack

- ASP.NET Core Razor Pages
- Polly (.NET resilience library)
- Bootstrap 5 (minimal styling)
- Configurable via `appsettings.json`

## 📦 Project Structure

```
PollyDemoApi/
├── Pages/
│   └── PollyStatus.cshtml         # Razor UI for config and API testing
│   └── PollyStatus.cshtml.cs      # PageModel with config binding and API call
├── Controllers/
│   └── WeatherController.cs       # Sample API endpoint for testing
├── appsettings.json               # Polly policy configuration
├── Startup.cs / Program.cs        # Middleware and DI setup
```

## ⚙️ Configuration

Update `appsettings.json` with your desired Polly settings:

```json
"PollySettings": {
  "RetryCount": 3,
  "RetryBaseDelayMs": 200,
  "CircuitBreakerAllowedFailures": 5,
  "CircuitBreakerBreakSeconds": 30,
  "TimeoutSeconds": 10
}
```
## Package

## 1. dotnet add package Polly
## 2.dotnet add package Microsoft.Extensions.Http
## 3.dotnet dev-certs https --trust

## 🧪 How to Use

1. Run the project (`dotnet run`)
2. Navigate to `/PollyStatus`
3. View current Polly settings
4. Click **Call /api/weather/getdata** to test the API
5. Inspect the response and behavior under failure conditions

## 🔐 Notes

- Ensure SSL trust is configured if using HTTPS locally
- Antiforgery token is included in the form for secure POST
- Polly policies are injected via DI and wrapped using `PolicyWrap`


