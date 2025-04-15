using BackgroundJobCodingChallenge.Constants;
using BackgroundJobCodingChallenge.Messages;
using BackgroundJobCodingChallenge.Services;

namespace BackgroundJobCodingChallenge.Orchestrators;

public class CustomerActionOrchestrator(IQueueService queue)
{
    private readonly IQueueService _queue = queue;
    private readonly List<string> _tenants = ["A1", "B2"];

    public async Task RunAsync(CancellationToken ct)
    {
        foreach (var tenantId in _tenants)
        {
            var actionNames = new string[] { "EmailCustomer", "ApplyCredits" };
            foreach (var actionName in actionNames)
            {
                for (int i = 0; i < 100; i++) // Simulate 100 customers
                {
                    await _queue.QueueMessageAsync((int)QueueIds.CustomerActions, new CustomerActionMessage
                    {
                        TenantId = tenantId,
                        CustomerId = Guid.NewGuid(),
                        ActionName = actionName
                    }, ct);
                }
            }
        }
    }
}
