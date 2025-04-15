using BackgroundJobCodingChallenge.Messages;
using BackgroundJobCodingChallenge.Services;
using BackgroundJobCodingChallenge.StateModels;

namespace BackgroundJobCodingChallenge.Workers;

/// Worker to process customer actions. Meant to be used for high throughput and batch jobs in parallel
public class CustomerActionWorker(IDatabaseService db, IQueueService queue, ITriggerService trigger)
    : BaseWorker<CustomerActionState, CustomerActionMessage>(db, queue, trigger, "CustomerActionWorker")
{
    public override async Task ExecuteWorkerLogicAsync(CustomerActionMessage? message, CancellationToken ct)
    {
        if (message == null) return;

        // Execute tenant-scoped customer action
        Console.WriteLine($"Processed customer action {message.ActionName} for customer {message.CustomerId} for {message.TenantId}");
        await Task.CompletedTask;
    }

    public override Task LoadStateAsync()
    {
        throw new NotImplementedException();
    }
}
