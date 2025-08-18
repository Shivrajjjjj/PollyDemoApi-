namespace PollyDemoApi.Data
{
    public class ApiRequest
    {
        public int Id { get; set; }
        public string? Endpoint { get; set; }
        public string? RequestBody { get; set; }
        public string? ResponseBody { get; set; }
        public int StatusCode { get; set; }
        public bool Success { get; set; }
        public int RetryCount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}