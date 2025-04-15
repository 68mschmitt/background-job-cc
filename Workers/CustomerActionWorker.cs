using BackgroundJobCodingChallenge.Messages;
using BackgroundJobCodingChallenge.Services;

namespace BackgroundJobCodingChallenge.Workers;

/// Worker to process customer actions. Meant to be used for high throughput and batch jobs in parallel
public class CustomerActionWorker(IDatabaseService db, IQueueService queue, ITriggerService trigger) : BaseWorker<WorkerStateRecord, CustomerActionMessage>(db, queue, trigger, "CustomerActionWorker")
{
    public override async Task ExecuteWorkerLogicAsync(CustomerActionMessage? message, CancellationToken ct)
    {
        if (message == null) return;

        // Execute tenant-scoped customer action
        await ProcessCustomerAsync(message.TenantId, message.CustomerId, ct);
    }

#pragma warning disable IDE0060 // Remove unused parameter
    private static async Task ProcessCustomerAsync(string tenantId, Guid customerId, CancellationToken ct)
#pragma warning restore IDE0060 // Remove unused parameter
    {
        Console.WriteLine($"Processed customer {customerId} for {tenantId}");
        await Task.CompletedTask;
    }

}
