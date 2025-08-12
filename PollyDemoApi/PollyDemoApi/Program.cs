using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Timeout;
using Polly.CircuitBreaker;
using Polly.Wrap;
using PollyDemoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Read Polly settings from appsettings.json
var pollyConfig = builder.Configuration.GetSection("PollySettings");

int retryCount = pollyConfig.GetValue<int>("RetryCount");
int retryBaseDelayMs = pollyConfig.GetValue<int>("RetryBaseDelayMs");
int circuitBreakerAllowedFailures = pollyConfig.GetValue<int>("CircuitBreakerAllowedFailures");
int circuitBreakerBreakSeconds = pollyConfig.GetValue<int>("CircuitBreakerBreakSeconds");
int timeoutSeconds = pollyConfig.GetValue<int>("TimeoutSeconds");

builder.Services.AddControllers();
builder.Services.AddSingleton<FakeUnreliableService>();

// ==================== Retry Policy ====================
var retryPolicy = Policy<string>
    .Handle<Exception>()
    .WaitAndRetryAsync(
        retryCount,
        retryAttempt => TimeSpan.FromMilliseconds(retryBaseDelayMs * Math.Pow(2, retryAttempt)),
        onRetry: (delegateResult, timespan, retryAttempt, context) =>
        {
            Console.WriteLine($"[Retry] Attempt {retryAttempt} after {timespan.TotalMilliseconds}ms due to: {delegateResult.Exception?.Message}");
        });

// ==================== Circuit Breaker Policy ====================
var circuitBreakerPolicy = Policy<string>
    .Handle<Exception>()
    .CircuitBreakerAsync(
        circuitBreakerAllowedFailures,
        TimeSpan.FromSeconds(circuitBreakerBreakSeconds),
        onBreak: (delegateResult, breakDelay) =>
        {
            Console.WriteLine($"[Circuit] Opened for {breakDelay.TotalSeconds}s due to: {delegateResult.Exception?.Message}");
        },
        onReset: () => Console.WriteLine("[Circuit] Reset"),
        onHalfOpen: () => Console.WriteLine("[Circuit] Half-open"));

// ==================== Timeout Policy ====================
var timeoutPolicy = Policy.TimeoutAsync<string>(
    TimeSpan.FromSeconds(timeoutSeconds),
    TimeoutStrategy.Pessimistic,
    onTimeoutAsync: (context, timespan, task, exception) =>
    {
        Console.WriteLine($"[Timeout] Execution timed out after {timespan.TotalSeconds}s.");
        return Task.CompletedTask;
    });

// ==================== Fallback Policy ====================
var fallbackPolicy = Policy<string>
    .Handle<Exception>()
    .FallbackAsync(
        fallbackValue: "{ \"status\": \"fallback\", \"data\": \"This is fallback response\" }",
        onFallbackAsync: (delegateResult, context) =>
        {
            Console.WriteLine($"[Fallback] Triggered because: {delegateResult.Exception?.Message}");
            return Task.CompletedTask;
        });

// ==================== Combine Policies ====================
AsyncPolicyWrap<string> resilienceWrap = Policy.WrapAsync(
    fallbackPolicy,
    retryPolicy,
    circuitBreakerPolicy,
    timeoutPolicy
);
builder.Services.AddHttpClient();
builder.Services.AddSingleton(resilienceWrap);
builder.Services.AddRazorPages();
var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
app.MapRazorPages();
app.MapGet("/", () => "PollyDemoApi is running. Try /api/weather/getdata"); 
app.Run();