using System.Collections.Concurrent;
using System.Text.Json;

namespace NOS.Engineering.Challenge.Database;

/**
*
*   Do not change this file.
*
*/
public class SlowDatabase<TOut, TIn>: IDatabase<TOut, TIn>
{
    private readonly ConcurrentDictionary<Guid, TOut?> _database;
    private readonly IMapper<TOut?, TIn> _mapper;

    public SlowDatabase(IMapper<TOut?, TIn> mapper, IMockData<TOut> mockData)
    {
        _mapper = mapper;
        _database = new ConcurrentDictionary<Guid, TOut?>();

        var mocks = mockData.GenerateMocks();
        foreach (var mock in mocks)
        {
            _database.TryAdd(mock.Key, mock.Value);
        }
    }

    public Task<TOut?> Create(TIn item)
    {
        //There is no need to worry about id clashes for this exercise.
        var id = Guid.NewGuid();
        Thread.Sleep(1000);
        var createdItem = _mapper.Map(id, item);
        if (!_database.TryAdd(id, createdItem))
        {
            throw new Exception("Could not add content to database");
        }
        return Task.FromResult(createdItem);
    }

    public Task<TOut?> Read(Guid id)
    {
        Thread.Sleep(3000);
        return Task.FromResult(_database.TryGetValue(id, out var item) ? item : default);
    }

    public Task<IEnumerable<TOut?>> ReadAll()
    {
        Thread.Sleep(5000);
        return Task.FromResult(_database.Values.AsEnumerable());
    }

    public Task<IEnumerable<TOut?>> SearchContents(String title, List<string> genres)
    {
        //Thread.Sleep(5000);
        var all = _database.Values.AsEnumerable();

        var filtered = _mapper.filter(all, title, genres);

        return Task.FromResult(filtered);
    }

    public Task<TOut?> Update(Guid id, TIn item)
    {
        Thread.Sleep(1000);
        if(!_database.TryGetValue(id, out var dbItem))
        {
            return Task.FromResult<TOut>(default!)!;
        }
        var updatedItem = _mapper.Patch(dbItem, item);
        if (!_database.TryUpdate(id, updatedItem, dbItem))
        {
            return Task.FromResult<TOut>(dbItem!)!;
        }
        return Task.FromResult(updatedItem);
    }

    public Task<Guid> Delete(Guid id)
    {
        Thread.Sleep(2000);

        return Task.FromResult(_database.TryRemove(id, out _) ? id : Guid.Empty);
    }
    
}