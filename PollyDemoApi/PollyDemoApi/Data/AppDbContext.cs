using Microsoft.EntityFrameworkCore;

namespace PollyDemoApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<TestItem> TestItems { get; set; }
        public DbSet<ApiRequest> ApiRequests { get; set; }
        public DbSet<RetryLog> RetryLogs { get; set; }
        public DbSet<ExternalServiceCall> ExternalServiceCalls { get; set; }
        public DbSet<LogEntry> LogEntries { get; set; }
    }
}