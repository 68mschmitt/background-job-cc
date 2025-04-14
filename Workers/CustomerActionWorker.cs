using BackgroundJobCodingChallenge.Messages;
using BackgroundJobCodingChallenge.Services;

namespace BackgroundJobCodingChallenge.Workers;

public class CustomerActionWorker(IDatabaseService db, IQueueService queue, ITriggerService trigger) : BaseWorker<WorkerStateRecord, CustomerActionMessage>(db, queue, trigger, "CustomerActionWorker")
{
    protected override async Task ExecuteWorkerLogicAsync(CustomerActionMessage? message, CancellationToken ct)
    {
        if (message == null) return;

        // Execute tenant-scoped customer action
        await ProcessCustomerAsync(message.TenantId, message.CustomerId, ct);
    }

    private async Task ProcessCustomerAsync(string tenantId, Guid customerId, CancellationToken ct)
    {
        Console.WriteLine($"Processed customer {customerId} for {tenantId}");
        await Task.CompletedTask;
    }

}
