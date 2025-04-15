namespace BackgroundJobCodingChallenge.Messages;

/// An action message could be tenant (carwash location) specific. Then further specificity to address the customer
public class CustomerActionMessage
{
    public string TenantId { get; set; } = default!;
    public Guid CustomerId { get; set; }
    public string ActionName { get; set; } = "default";
}
