using BackgroundJobCodingChallenge.Constants;
using BackgroundJobCodingChallenge.Messages;
using BackgroundJobCodingChallenge.Services;
using BackgroundJobCodingChallenge.Workers;

namespace BackgroundJobCodingChallenge.Registries;

public class WorkerRegistry
{
    public static void RegisterWorkers(IQueueService queueService, CustomerActionWorker worker)
    {
        queueService.SubscribeToMessages<CustomerActionMessage>(
                (int)QueueIds.CustomerActions,
                worker.ProcessMessageAsync);
    }
}
