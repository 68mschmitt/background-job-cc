namespace BackgroundJobCodingChallenge.Messages;

/// Designated message for a finance data sync
public class FinancialSyncMessage 
{
    public string TenantId { get; set; } = default!;
    public int TransactionId { get; set; }
}
