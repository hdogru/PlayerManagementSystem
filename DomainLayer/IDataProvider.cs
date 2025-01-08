using System.Linq.Expressions;

namespace DomainLayer
{

    public interface IDataProvider
    {
        // Generic CRUD operations
        Task<T> GetByIdAsync<T>(string id) where T : class;
        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;
        Task AddAsync<T>(T entity) where T : class;
        Task UpdateAsync<T>(T entity) where T : class;
        Task DeleteByIdAsync<T>(string id) where T : class;
        Task DeleteAsync<T>(Expression<Func<T, bool>> predicate) where T : class;
    }
   
}