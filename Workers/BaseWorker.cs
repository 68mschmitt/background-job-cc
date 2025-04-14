using BackgroundJobCodingChallenge.Services;

namespace BackgroundJobCodingChallenge.Workers;

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

    protected abstract Task ExecuteWorkerLogicAsync(TMessage? message, CancellationToken ct);

    protected Task QueueAsync(int queueId, TMessage message, CancellationToken ct)
    {
        return _queue.QueueMessageAsync(queueId, message, ct);
    }

    protected void RegisterQueue(int queueId)
    {
        _queue.SubscribeToMessages<TMessage>(queueId, ProcessMessageAsync);
    }

    protected void RegisterTrigger()
    {
        _trigger.Subscribe(ExecuteAsync);
    }
}

