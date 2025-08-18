namespace PollyDemoApi.Data
{
    public class LogEntry
    {
        public int Id { get; set; }
        public string? Level { get; set; }
        public string? Message { get; set; }
        public string? StackTrace { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}