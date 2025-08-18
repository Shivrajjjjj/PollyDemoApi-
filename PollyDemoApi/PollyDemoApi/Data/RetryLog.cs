namespace PollyDemoApi.Data
{
    public class RetryLog
    {
        public int Id { get; set; }
        public string? PolicyName { get; set; }
        public int RetryCount { get; set; }
        public bool Success { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}