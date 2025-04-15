using BackgroundJobCodingChallenge.Messages;
using BackgroundJobCodingChallenge.Services;
using BackgroundJobCodingChallenge.StateModels;

namespace BackgroundJobCodingChallenge.Workers;

public class FinancialSyncWorker(
    IDatabaseService db,
    IQueueService queue,
    ITriggerService trigger
    ) : BaseWorker<FinancialSyncState, FinancialSyncMessage>(db, queue, trigger, "FinancialSyncWorker")
{
    private FinancialSyncState? state;

    public override async Task ExecuteWorkerLogicAsync(FinancialSyncMessage? message, CancellationToken ct)
    {
        if (message == null) return;

        // Simulate saving transaction to database
        Console.WriteLine($"Saving transaction {message.TransactionId} for tenant {message.TenantId}");
        await Task.Delay(50, ct); // simulate I/O
    }

    public override async Task LoadStateAsync()
    {
        // The state would be in orchestrator, or in the db, depending on the data we need
        state ??= new FinancialSyncState { LastSyncedTransactionId = 0 };
        await Task.CompletedTask;
    }
}

