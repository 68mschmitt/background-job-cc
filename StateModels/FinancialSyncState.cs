namespace BackgroundJobCodingChallenge.StateModels;

/// Keep track of the finance sync state
public class FinancialSyncState : WorkerStateBase
{
    public int LastSyncedTransactionId { get; set; }
}
