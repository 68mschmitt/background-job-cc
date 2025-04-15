namespace BackgroundJobCodingChallenge.StateModels;

/// Keep track of the finance sync state
public class WorkerStateBase
{
    public string WorkerName { get; set; } = "";
    public string TenantId { get; set; } = "";
    public Guid CustomerId { get; set; }
    public string? SerializedState { get; set; }
}
