using BackgroundJobCodingChallenge.Services;

namespace BackgroundJobCodingChallenge.Workers;

/// Base worker to be passed through to the similar implementations in the background processes
public abstract class BaseWorker<TState, TMessage>(
        IDatabaseService db,
        IQueueService queue,
        ITriggerService trigger,
        string workerName)
    where TState : class, new()
    where TMessage : class
{
    private readonly IDatabaseService _db = db;
    private readonly IQueueService _queue = queue;
    private readonly ITriggerService _trigger = trigger;
    private readonly string _workerName = workerName;

    public abstract Task ExecuteWorkerLogicAsync(TMessage? message, CancellationToken ct);

    public abstract Task LoadStateAsync();

    public Task QueueAsync(int queueId, TMessage message, CancellationToken ct)
    {
        return _queue.QueueMessageAsync(queueId, message, ct);
    }
}

