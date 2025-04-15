using BackgroundJobCodingChallenge.Constants;
using BackgroundJobCodingChallenge.Messages;
using BackgroundJobCodingChallenge.Orchestrators;
using BackgroundJobCodingChallenge.ServiceImplementations;
using BackgroundJobCodingChallenge.Workers;

// Mock implementations for the services, to simulate a live db, queue service, and trigger service
var database = new InMemoryDatabaseService();
var queue = new InMemoryQueueService();
var trigger = new InMemoryTriggerService();

// Register workers. Injecting the services into them so they can access the data they need, and also update the data store as they process
var financialDataSyncWorker = new FinancialSyncWorker(database, queue, trigger);
var customerActionWorker = new CustomerActionWorker(database, queue, trigger);
var employeeUploadWorker = new EmployeeUploadWorker(database, queue, trigger);

// Subscribe to queue messages (explicit handler wiring)
queue.SubscribeToMessages<CustomerActionMessage>((int)QueueIds.CustomerActions, customerActionWorker.ExecuteWorkerLogicAsync);
queue.SubscribeToMessages<EmployeeUploadMessage>((int)QueueIds.EmployeeUpload, employeeUploadWorker.ExecuteWorkerLogicAsync);
queue.SubscribeToMessages<FinancialSyncMessage>((int)QueueIds.FinancialSync, financialDataSyncWorker.ExecuteWorkerLogicAsync);

// Register orchestrators that queue the messages and mock the data we want
var customerOrchestrator = new CustomerActionOrchestrator(queue);
var employeeOrchestrator = new EmployeeUploadOrchestrator(queue);
var financialOrchestrator = new FinancialSyncOrchestrator(database, queue);

var financeSub = trigger.Subscribe(async ct => { Console.WriteLine("Triggering Finance Orchestrator"); await financialOrchestrator.RunAsync(ct); });

await trigger.FireAsync(); // Fires all subscribed trigger jobs
await Task.Delay(1000);

await queue.QueueMessageAsync((int)QueueIds.CustomerActions, new CustomerActionMessage
{
    TenantId = "A1",
    CustomerId = Guid.NewGuid()
});
await queue.QueueMessageAsync((int)QueueIds.CustomerActions, new CustomerActionMessage
{
    TenantId = "B2",
    CustomerId = Guid.NewGuid()
});

Console.WriteLine("[Main] Manually queuing a few employee uploads...");
await queue.QueueMessageAsync((int)QueueIds.EmployeeUpload, new EmployeeUploadMessage
{
    TenantId = "A1",
    CsvRowId = 201
});
await queue.QueueMessageAsync((int)QueueIds.EmployeeUpload, new EmployeeUploadMessage
{
    TenantId = "B2",
    CsvRowId = 202
});

await queue.QueueMessageAsync((int)QueueIds.FinancialSync, new FinancialSyncMessage
{
    TenantId = "A1",
    TransactionId = 1
});
await queue.QueueMessageAsync((int)QueueIds.FinancialSync, new FinancialSyncMessage
{
    TenantId = "B2",
    TransactionId = 2
});

var ct = new CancellationToken();

// Simulate the customer actions and employee upload workers
Console.WriteLine($"About to simulate customer actions being queued");
await Task.Delay(5000);
await customerOrchestrator.RunAsync(ct);

Console.WriteLine($"About to simulate employee uploads being queued");
await Task.Delay(5000);
await employeeOrchestrator.RunAsync(ct);


await Task.Delay(10000);
Console.WriteLine("[Main] Done.");

