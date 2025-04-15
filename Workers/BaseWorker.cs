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

    public async Task ProcessMessageAsync(TMessage message, CancellationToken ct)
    {
        await ExecuteWorkerLogicAsync(message, ct);
    }

    public async Task ExecuteAsync(CancellationToken ct)
    {
        await ExecuteWorkerLogicAsync(null, ct);
    }

    public abstract Task ExecuteWorkerLogicAsync(TMessage? message, CancellationToken ct);

    public Task QueueAsync(int queueId, TMessage message, CancellationToken ct)
    {
        return _queue.QueueMessageAsync(queueId, message, ct);
    }

    public void RegisterQueue(int queueId)
    {
        _queue.SubscribeToMessages<TMessage>(queueId, ProcessMessageAsync);
    }

    public void RegisterTrigger()
    {
        _trigger.Subscribe(ExecuteAsync);
    }
}

