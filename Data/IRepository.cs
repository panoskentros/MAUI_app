using System.Linq.Expressions;
using MAUI_app.Model;

namespace MAUI_app.Data;

public interface IRepository<T> where T : class
{

    Task<AppDbContext> GetDbContext();

    Task<IResult<IEnumerable<T>>> GetAllAsync(
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);
    Task<IResult<IEnumerable<T>>> GetAllAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includes);

    Task<IResult<T>> GetByIdAsync(
        int id,
        string keyName = "Id",
        CancellationToken cancellationToken = default,
        IEnumerable<Expression<Func<T, object>>>? includes = null,
        IEnumerable<Func<IQueryable<T>, IQueryable<T>>>? thenIncludes = null);

    IQueryable<T> GetQueryable();

    Task<IResult<T>> GetByIdAsync(
        object id,
        CancellationToken cancellationToken = default,
        IEnumerable<Expression<Func<T, object>>>? includes = null,
        IEnumerable<Func<IQueryable<T>, IQueryable<T>>>? includeChains = null);

    Task<IResult<T>?> GetByIdAsync(object[] keyValues);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int key1, int key2, int key3);

    Task<IResult<T>> AddAsync(T entity, CancellationToken cancellationToken = default, bool asDetached = false);
    Task<IResult<T>> UpdateAsync(T entity, CancellationToken cancellationToken = default, bool asDetached = false);
    Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default);


}