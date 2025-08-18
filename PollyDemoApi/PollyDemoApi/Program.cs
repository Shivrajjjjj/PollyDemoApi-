using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly.Wrap;
using PollyDemoApi.Data;
using PollyDemoApi.Policies;
using PollyDemoApi.Services;

var builder = WebApplication.CreateBuilder(args);

// ==================== Polly Settings ====================
var pollyConfig = builder.Configuration.GetSection("PollySettings");

int retryCount = pollyConfig.GetValue<int>("RetryCount", 3);
int retryBaseDelayMs = pollyConfig.GetValue<int>("RetryBaseDelayMs", 200);
int circuitBreakerAllowedFailures = pollyConfig.GetValue<int>("CircuitBreakerAllowedFailures", 2);
int circuitBreakerBreakSeconds = pollyConfig.GetValue<int>("CircuitBreakerBreakSeconds", 30);
int timeoutSeconds = pollyConfig.GetValue<int>("TimeoutSeconds", 2);

// ==================== Logger Factory ====================
var loggerFactory = LoggerFactory.Create(config => config.AddConsole());
var dbLogger = loggerFactory.CreateLogger("DatabasePolicy");
var externalLogger = loggerFactory.CreateLogger("ExternalPolicy");

// ==================== Services ====================
builder.Services.AddControllers();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<FakeUnreliableService>();

// ==================== DbContext ====================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ==================== Polly Policies ====================
var dbPolicy = PollyPolicyFactory.CreateDatabasePolicy(builder.Configuration, dbLogger);
builder.Services.AddSingleton<AsyncPolicyWrap<object>>(dbPolicy);

var externalPolicy = PollyPolicyFactory.CreateExternalServicePolicy(builder.Configuration, externalLogger);
builder.Services.AddSingleton<AsyncPolicyWrap<string>>(externalPolicy);

// ==================== HttpClient ====================
builder.Services.AddHttpClient();

// ==================== Custom Executors ====================
builder.Services.AddScoped<DatabasePolicyExecutor>();
builder.Services.AddScoped<ResilientDbExecutor>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapControllers();
app.MapRazorPages();
app.MapGet("/", () => "PollyDemoApi is running. Try /api/weather/getdata");

app.Run();