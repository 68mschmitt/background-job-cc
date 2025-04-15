namespace BackgroundJobCodingChallenge.Workers;

/// Worker state, when we need to keep track of state
/// For the purposes of this project, this will facilitate the cursor position in the financial data sync
public class WorkerStateRecord
{
    public string WorkerName { get; set; } = default!;
    public string? TenantId { get; set; }
    public string SerializedState { get; set; } = default!;
}
