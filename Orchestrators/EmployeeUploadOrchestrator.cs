using BackgroundJobCodingChallenge.Constants;
using BackgroundJobCodingChallenge.Messages;
using BackgroundJobCodingChallenge.Services;

namespace BackgroundJobCodingChallenge.Orchestrators;

public class EmployeeUploadOrchestrator(IQueueService queue)
{
    private readonly IQueueService _queue = queue;
    private readonly List<string> _tenants = ["A1", "B2"];

    public async Task RunAsync(CancellationToken ct)
    {
        foreach (var tenantId in _tenants)
        {
            for (int i = 1; i <= 50; i++) // Simulate 200 CSV rows
            {
                await _queue.QueueMessageAsync((int)QueueIds.EmployeeUpload, new EmployeeUploadMessage
                {
                    TenantId = tenantId,
                    CsvRowId = i
                }, ct);
            }
        }
    }
}

