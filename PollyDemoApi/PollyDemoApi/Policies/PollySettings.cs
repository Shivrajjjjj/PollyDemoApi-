namespace PollyDemoApi.Policies;

public class PollySettings
{
    public int RetryCount { get; set; }
    public int RetrySleepDurationSeconds { get; set; }
    public int CircuitBreakerFailureThreshold { get; set; }
    public int CircuitBreakerDurationOfBreakSeconds { get; set; }
    public int TimeoutSeconds { get; set; }
    public int MaxParallelization { get; set; }
    public int MaxQueuingActions { get; set; }
}
