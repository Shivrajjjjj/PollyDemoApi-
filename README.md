

```markdown
# PollyDemoApi Dashboard & Tests

A lightweight ASP.NET Core Razor Pages UI and accompanying test project to visualize, test, and validate Polly resilience policies (Retry, Timeout, Circuit Breaker, Fallback) configured via `appsettings.json`.

## 🚀 Features

- View current Polly configuration values
- Trigger API calls to test resilience behavior
- Display raw API responses for diagnostics
- Unit tests for controllers, services, and Polly policies
- In-memory database support for testing
- Config-driven setup with SSL and antiforgery support

## 🛠️ Tech Stack

- ASP.NET Core 8.0
- Polly (.NET resilience library)
- Entity Framework Core (InMemory + SQL Server)
- xUnit + Moq (unit testing)
- Bootstrap 5 (minimal styling)
- Configurable via `appsettings.json`



````

## ⚙️ Configuration

Update `appsettings.json` with your desired Polly settings:

```json
"PollySettings": {
  "RetryCount": 3,
  "RetryBaseDelayMs": 200,
  "CircuitBreakerAllowedFailures": 5,
  "CircuitBreakerBreakSeconds": 30,
  "TimeoutSeconds": 10,
  "BulkheadMaxParallelization": 10,
  "BulkheadMaxQueuingActions": 20
}
````

## 📦 Required Packages

**API Project (`PollyDemoApi`)**:

```bash
dotnet add package Polly --version 7.2.3
dotnet add package Polly.Extensions.Http
dotnet add package Microsoft.Extensions.Http
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet tool install --global dotnet-ef
dotnet dev-certs https --trust
```

**Tests Project (`PollyDemoApi.Tests`)**:

```bash
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
dotnet add package Moq
dotnet add package Microsoft.AspNetCore.Mvc
dotnet add package Polly
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

## 🧪 Running Tests

1. Navigate to the test project:

```bash
cd PollyDemoApi.Tests
```

2. Run all tests:

```bash
dotnet test
```

3. Tests include:

* Controller CRUD tests using in-memory database
* ResilientDbExecutor tests for Polly-wrapped execution
* PollyPolicyFactory tests for database & external service policies
* Simple sanity check `Test1_ShouldPass`

## 🔐 Notes

* In-memory database uses unique names per test to avoid state conflicts.
* SSL trust is required for HTTPS locally.
* Polly policies are injected via DI and wrapped using `PolicyWrap`.
* Unit tests ensure resilience behavior works as expected.

```


```
