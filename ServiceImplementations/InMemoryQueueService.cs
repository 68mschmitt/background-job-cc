namespace BackgroundJobCodingChallenge.ServiceImplementations;

using System.Collections.Concurrent;
using System.Reflection;
using BackgroundJobCodingChallenge.Services;

public class InMemoryQueueService : IQueueService, IDisposable
{
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

    private async Task ProcessMessagesLoopAsync()
    {
        Console.WriteLine("ProcessQueueStarted");
        while (!_cts.Token.IsCancellationRequested)
        {
            Console.WriteLine("QueueCalledAgain");
            if (_messageQueue.TryDequeue(out var queued))
            {
                var (queueId, message) = queued;
                var messageType = message.GetType();
                var key = (queueId, messageType);
                Console.WriteLine($"{queueId}:{message}");

                if (_handlers.TryGetValue(key, out var handlerObj))
                {
                    var method = typeof(InMemoryQueueService)
                        .GetMethod(nameof(InvokeHandler), BindingFlags.NonPublic | BindingFlags.Instance)!
                        .MakeGenericMethod(messageType);

                    await (Task)method.Invoke(this, [handlerObj, message, _cts.Token])!;
                }
            }

            await Task.Delay(10); // throttle loop
        }
        Console.WriteLine("Not processing anymore");
    }

    private static async Task InvokeHandler<TMessage>(
        object handlerObj,
        object rawMessage,
        CancellationToken ct
    )
    {
        var handler = (IQueueService.FProcessAsync<TMessage>)handlerObj;
        var msg = (TMessage)rawMessage;
        await handler(msg, ct);
    }

    public void Dispose()
    {
        _cts.Cancel();
        _processingLoop.Wait();
    }
}

