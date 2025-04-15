namespace BackgroundJobCodingChallenge.ServiceImplementations;

using System.Collections.Concurrent;
using BackgroundJobCodingChallenge.Services;

public class InMemoryDatabaseService : IDatabaseService
{
    private readonly ConcurrentDictionary<Type, List<object>> _store = new();

    public delegate IQueryable<TResult> FCreateQuery<TEntity, TResult>(IQueryable<TEntity> query);

    public Task<TResult> GetAsync<TEntity, TResult>(IDatabaseService.FCreateQuery<TEntity, TResult> createQuery)
    {
        var data = _store.GetOrAdd(typeof(TEntity), _ => [])
                         .OfType<TEntity>()
                         .AsQueryable();

        var result = createQuery(data).FirstOrDefault();

        return Task.FromResult(result!);
    }

    public Task<TEntity> CreateAsync<TEntity>(TEntity entity)
    {
        _store.AddOrUpdate(
            typeof(TEntity),
            _ => [entity!],
            (_, list) =>
            {
                list.Add(entity!);
                return list;
            });

        return Task.FromResult(entity);
    }

    public Task<TEntity> CreateAsync<TEntity>(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            CreateAsync(entity);
        }
        return Task.FromResult(entities.First());
    }

    public Task<TEntity> UpdateAsync<TEntity>(TEntity entity)
    {
        var type = typeof(TEntity);
        if (!_store.TryGetValue(type, out List<object>? list)) return Task.FromResult(entity);
        var existing = list.OfType<TEntity>().FirstOrDefault();

        if (existing != null)
        {
            list.Remove(existing!);
        }

        list.Add(entity!);
        return Task.FromResult(entity);
    }

    public Task<TEntity> UpdateAsync<TEntity>(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            UpdateAsync(entity);
        }
        return Task.FromResult(entities.First());
    }

    public Task DeleteAsync<TEntity>(TEntity entity)
    {
        var list = _store.GetOrAdd(typeof(TEntity), _ => []);
        list.Remove(entity!);
        return Task.CompletedTask;
    }

    public Task DeleteAsync<TEntity>(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            DeleteAsync(entity);
        }
        return Task.CompletedTask;
    }

}
