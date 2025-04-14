namespace BackgroundJobCodingChallenge.Messages;

public class CustomerActionMessage
{
    public string TenantId { get; set; } = default!;
    public Guid CustomerId { get; set; }
}
