namespace BackgroundJobCodingChallenge.ServiceImplementations;

using System.Collections.Concurrent;
using System.Threading;
using BackgroundJobCodingChallenge.Services;

public class InMemoryTriggerService : ITriggerService
{
    private readonly ConcurrentBag<ITriggerService.FExecuteAsync> _subscribers = [];

    public ITriggerService.FUnsubscribe Subscribe(ITriggerService.FExecuteAsync executeAsync)
    {
        _subscribers.Add(executeAsync);
        return () => Unsubscribe(executeAsync);
    }

    public void Unsubscribe(ITriggerService.FExecuteAsync executeAsync)
    {
        var remaining = _subscribers.Where(s => s != executeAsync).ToList();

        _subscribers.Clear(); // Not ideal, but okay for in-memory mock
        foreach (var sub in remaining)
            _subscribers.Add(sub);
    }

    /// Manually invoke all subscribed trigger callbacks.
    public async Task FireAsync(CancellationToken cancellationToken = default)
    {
        foreach (var subscriber in _subscribers)
        {
            await subscriber(cancellationToken);
        }
    }
}

