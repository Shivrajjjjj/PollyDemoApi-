public class ExternalServiceCall
{
    public int Id { get; set; }
    public string? ServiceName { get; set; }
    public string? RequestPayload { get; set; }
    public string? ResponsePayload { get; set; }
    public int StatusCode { get; set; }
    public bool IsSuccessful { get; set; }
    public DateTime AttemptedAt { get; set; } = DateTime.UtcNow;
}