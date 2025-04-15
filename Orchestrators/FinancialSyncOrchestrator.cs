using System.Text.Json;
using BackgroundJobCodingChallenge.Constants;
using BackgroundJobCodingChallenge.Messages;
using BackgroundJobCodingChallenge.Services;
using BackgroundJobCodingChallenge.StateModels;
using BackgroundJobCodingChallenge.Workers;

namespace BackgroundJobCodingChallenge.Orchestrators;

// Meant to handle the cursor state tracking
// As well as queue the individual background processes for the financial data sync
public class FinancialSyncOrchestrator(IDatabaseService db, IQueueService queue)
{
    private readonly IDatabaseService _db = db;
    private readonly IQueueService _queue = queue;
    private readonly List<string> _tenants = ["A1", "B2", "C3"];

    public async Task RunAsync(CancellationToken ct)
    {
        foreach (var tenantId in _tenants)
        {
            // Get the record
            var record = await _db.GetAsync<FinancialSyncState, FinancialSyncState>(query =>
                query.Where(r => r.WorkerName == "FinancialSyncWorker" && r.TenantId == tenantId));

            // Deserialize the record
            FinancialSyncState? state = null;

            if (record?.SerializedState is string json)
            {
                state = JsonSerializer.Deserialize<FinancialSyncState>(json);
            }

            state ??= new FinancialSyncState { LastSyncedTransactionId = 0 };

            var newTransactionIds = GetMockTransactions(state.LastSyncedTransactionId);

            foreach (var transId in newTransactionIds)
            {
                await _queue.QueueMessageAsync((int)QueueIds.FinancialSync, new FinancialSyncMessage
                {
                    TenantId = tenantId,
                    TransactionId = transId
                }, ct);
            }

            // Update cursor
            var updatedState = new FinancialSyncState
            {
                LastSyncedTransactionId = newTransactionIds.LastOrDefault(state.LastSyncedTransactionId)
            };

            var updatedRecord = new FinancialSyncState
            {
                WorkerName = "FinancialSyncWorker",
                TenantId = tenantId,
                SerializedState = JsonSerializer.Serialize(updatedState)
            };

            await _db.UpdateAsync(updatedRecord);
        }
    }

    private static List<int> GetMockTransactions(int lastId)
    {
        // Mock 5 processed rows per transaction
        return [.. Enumerable.Range(lastId + 1, 5)];
    }
}
