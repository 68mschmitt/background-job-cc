using BackgroundJobCodingChallenge.Messages;
using BackgroundJobCodingChallenge.Services;
using BackgroundJobCodingChallenge.StateModels;

namespace BackgroundJobCodingChallenge.Workers;

public class EmployeeUploadWorker(
    IDatabaseService db,
    IQueueService queue,
    ITriggerService trigger
    ) : BaseWorker<EmployeeUploadState, EmployeeUploadMessage>(db, queue, trigger, "EmployeeUploadWorker")
{
    public override async Task ExecuteWorkerLogicAsync(EmployeeUploadMessage? message, CancellationToken ct)
    {
        if (message == null) return;

        // Simulate processing an uploaded employee row
        Console.WriteLine($"[Upload] Processing employee row {message.CsvRowId} for tenant {message.TenantId}");
        await Task.Delay(5, ct); // Simulate DB write or validation
    }

    public override Task LoadStateAsync()
    {
        throw new NotImplementedException();
    }
}
