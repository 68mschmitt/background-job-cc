using BackgroundJobCodingChallenge.Constants;
using BackgroundJobCodingChallenge.Messages;
using BackgroundJobCodingChallenge.Orchestrators;
using BackgroundJobCodingChallenge.ServiceImplementations;
using BackgroundJobCodingChallenge.Workers;

// Mock implementations (assuming you have the interfaces and classes from earlier)
var database = new InMemoryDatabaseService();
var queue = new InMemoryQueueService();
var trigger = new InMemoryTriggerService();

// Register workers
var financialDataSyncWorker = new FinancialSyncWorker(database, queue, trigger);
var customerActionWorker = new CustomerActionWorker(database, queue, trigger);
var employeeUploadWorker = new EmployeeUploadWorker(database, queue, trigger);

// Register the worker's message handling function to the queue
financialDataSyncWorker.RegisterQueue((int)QueueIds.FinancialSync);
customerActionWorker.RegisterQueue((int)QueueIds.CustomerActions);
employeeUploadWorker.RegisterQueue((int)QueueIds.EmployeeUpload);

// Register orchestrators
var customerOrchestrator = new CustomerActionOrchestrator(queue);
var employeeOrchestrator = new EmployeeUploadOrchestrator(queue);
var financialOrchestrator = new FinancialSyncOrchestrator(database, queue);

// Register the orchestrator with the trigger system
// This means that when the trigger fires, the orchestrator runs
trigger.Subscribe(async ct => { Console.WriteLine("Triggering Finance Orchestrator"); await financialOrchestrator.RunAsync(ct); });
trigger.Subscribe(async ct => { Console.WriteLine("Triggering Customer Orchestrator"); await customerOrchestrator.RunAsync(ct); });
trigger.Subscribe(async ct => { Console.WriteLine("Triggering Employee Orchestrator"); await employeeOrchestrator.RunAsync(ct); });

// Subscribe to queue messages (explicit handler wiring)
queue.SubscribeToMessages<CustomerActionMessage>((int)QueueIds.CustomerActions, customerActionWorker.ExecuteWorkerLogicAsync);

queue.SubscribeToMessages<EmployeeUploadMessage>((int)QueueIds.EmployeeUpload, employeeUploadWorker.ExecuteWorkerLogicAsync);

// Simulate the "timer" going off once — as if it were running on a 5-minute schedule
Console.WriteLine("Triggering jobs manually...");
await trigger.FireAsync(); // Fires all subscribed trigger jobs

// Wait a bit to give the queue time to process messages
Console.WriteLine("Waiting for queued workers to process...");
await Task.Delay(1000);

// Optional: Trigger it again to simulate the next interval
Console.WriteLine("Triggering jobs manually...");
await trigger.FireAsync();

Console.WriteLine("Waiting again for workers...");
await Task.Delay(1000);

Console.WriteLine("Done!");

// Example: Manually queue some additional messages outside the orchestrator
Console.WriteLine("[Main] Manually queuing a few customer actions...");
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

await Task.Delay(1000);
Console.WriteLine("[Main] Done.");

