using DomainLayer;

using System.Collections.Concurrent;
using System.Linq.Expressions;


namespace DataAccessLayer
{
    public class MemoryDataProvider : IDataProvider
    {
        private readonly ConcurrentDictionary<string, object> _dataStore = new();

        public Task<T> GetByIdAsync<T>(string id) where T : class
        {
            if (_dataStore.TryGetValue(id, out var obj))
                return Task.FromResult(obj as T);

            return Task.FromResult<T>(null);
        }

        public Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
            var results = _dataStore.Values.OfType<T>();
            return Task.FromResult(results.AsEnumerable());
        }

        public Task AddAsync<T>(T entity) where T : class
        {
            var id = Guid.NewGuid().ToString();
            _dataStore[id] = entity;
            return Task.CompletedTask; 
        }

        public Task UpdateAsync<T>(T entity) where T : class
        {
            var existing = _dataStore.FirstOrDefault(x => x.Value.Equals(entity)).Key;
            if (existing != null)
                _dataStore[existing] = entity;

            return Task.CompletedTask;
        }

        public Task DeleteByIdAsync<T>(string id) where T : class
        {
            _dataStore.TryRemove(id, out _);
            return Task.CompletedTask;
        }

        public Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            var results = _dataStore.Where(kvp => kvp.Value is T).ToList();

            // Compile the predicate into a delegate for evaluation
            var compiledPredicate = predicate.Compile();

            // Find the first matching entry
            var entryToRemove = results.FirstOrDefault(kvp => compiledPredicate((T)kvp.Value));

            if (!entryToRemove.Equals(default(KeyValuePair<string, object>)))
            {
                _dataStore.TryRemove(entryToRemove.Key, out _);
            }

            return Task.CompletedTask;
        }
    }
}
