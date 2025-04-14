namespace BackgroundJobCodingChallenge.Workers;

public class WorkerStateRecord
{
    public string WorkerName { get; set; } = default!;
    public string? TenantId { get; set; }
    public string SerializedState { get; set; } = default!;
}
