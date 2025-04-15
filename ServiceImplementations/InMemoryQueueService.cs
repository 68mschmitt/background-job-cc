namespace BackgroundJobCodingChallenge.ServiceImplementations;

using System.Collections.Concurrent;
using BackgroundJobCodingChallenge.Messages;
using BackgroundJobCodingChallenge.Services;

public class InMemoryQueueService : IQueueService, IDisposable
{
    private const int _MAX_CONCURRENT_COUNT = 50;

    private readonly ConcurrentQueue<(int queueId, object message)> _messageQueue = new();
    private readonly ConcurrentDictionary<(int queueId, Type messageType), object> _handlers = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _processingLoop;

    public InMemoryQueueService()
    {
        _processingLoop = Task.Run(ProcessMessagesLoopAsync);
    }

    public Task QueueMessageAsync<TMessage>(
        int queueId,
        TMessage message,
        CancellationToken cancellationToken = default
    ) where TMessage : class
    {
        _messageQueue.Enqueue((queueId, message!));
        return Task.CompletedTask;
    }

    public IQueueService.FUnsubscribe SubscribeToMessages<TMessage>(
        int queueId,
        IQueueService.FProcessAsync<TMessage> processAsync
    ) where TMessage : class
    {
        var key = (queueId, typeof(TMessage));
        _handlers[key] = processAsync;

        return () => _handlers.TryRemove(key, out _);
    }

    public void UnsubscribeFromMessages<TMessage>(
        IQueueService.FProcessAsync<TMessage> processAsync
    )
    {
        var key = _handlers
            .FirstOrDefault(kvp => kvp.Key.messageType == typeof(TMessage) &&
                                   kvp.Value.Equals(processAsync)).Key;

        if (key != default)
        {
            _handlers.TryRemove(key, out _);
        }
    }

    /// This runs each process one by one
    /// It is not great for a high throughput system
    /// There are a couple improvements that come to mind for production use
    /// 1. Parallelize the tasks (handlers) to take advantage of more threads
    /// 2. Segregate the long running processes from the high throughput processes and increase the batch size based on the estimated runtime
    private async Task ProcessMessagesLoopAsync()
    {
        while (!_cts.Token.IsCancellationRequested)
        {
            if (_messageQueue.TryDequeue(out var queued))
            {
                var (queueId, message) = queued;
                var messageType = message.GetType();
                var key = (queueId, messageType);

                if (_handlers.TryGetValue(key, out var handlerObj))
                {

                    if (messageType.ToString().Contains("CustomerActionMessage"))
                    {
                        var handler = (IQueueService.FProcessAsync<CustomerActionMessage>)handlerObj;
                        await handler((CustomerActionMessage)message, _cts.Token);
                    }
                    else if (messageType.ToString().Contains("FinancialSyncMessage"))
                    {
                        var handler = (IQueueService.FProcessAsync<FinancialSyncMessage>)handlerObj;
                        await handler((FinancialSyncMessage)message, _cts.Token);
                    }
                    else if (messageType.ToString().Contains("EmployeeUploadMessage"))
                    {
                        var handler = (IQueueService.FProcessAsync<EmployeeUploadMessage>)handlerObj;
                        await handler((EmployeeUploadMessage)message, _cts.Token);
                    }
                }

                if (_messageQueue.IsEmpty) await Task.Delay(100); // throttle loop
            }
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _processingLoop.Wait();
    }
}

