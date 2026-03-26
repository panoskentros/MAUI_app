using MAUI_app.Model;

namespace MAUI_app.Data;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


public class PostgreRepository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public PostgreRepository(AppDbContext context)
    {
       _context = context;
       _dbSet = _context.Set<T>();
    }

    public async Task<AppDbContext> GetDbContext()
    {
        await Task.FromResult<bool>(true);
        return this._context;
    }

    public IQueryable<T> GetQueryable()
    {
        return _context.Set<T>();
    }

    public async Task<IResult<IEnumerable<T>>> GetAllAsync(
       CancellationToken cancellationToken = default,
       params Expression<Func<T, object>>[] includes)
    {
       IQueryable<T> query = _dbSet.AsNoTracking();

       foreach (var include in includes)
          query = query.Include(include);

       var entities = await query.ToListAsync(cancellationToken);
       return Result<IEnumerable<T>>.Ok(entities);
    }
    
    public async Task<IResult<IEnumerable<T>>> GetAllAsync(
       Expression<Func<T, bool>> predicate, 
       CancellationToken cancellationToken = default,
       params Expression<Func<T, object>>[] includes)
    {
       IQueryable<T> query = _dbSet.AsNoTracking();

       if (predicate != null)
       {
          query = query.Where(predicate);
       }

       foreach (var include in includes)
       {
          query = query.Include(include);
       }

       var entities = await query.ToListAsync(cancellationToken);
    
       return Result<IEnumerable<T>>.Ok(entities);
    }

    public async Task<IResult<T>> GetByIdAsync(
    object id,
    CancellationToken cancellationToken = default,
    IEnumerable<Expression<Func<T, object>>>? includes = null,
    IEnumerable<Func<IQueryable<T>, IQueryable<T>>>? includeChains = null)
    {
       var entityType = _context.Model.FindEntityType(typeof(T));
       if (entityType == null)
          return Result<T>.Fail("Entity type not found in EF model metadata.");

       var key = entityType.FindPrimaryKey();
       if (key == null || key.Properties.Count == 0)
          return Result<T>.Fail("Entity does not have a defined primary key.");

       var keyProperty = key.Properties.First();
       string keyName = keyProperty.Name;
       Type keyType = keyProperty.ClrType;

       IQueryable<T> query = _dbSet.AsNoTracking();

       if (includes is not null)
       {
          foreach (var include in includes)
             query = query.Include(include);
       }

       if (includeChains is not null)
       {
          foreach (var chain in includeChains)
             query = chain(query);
       }

       var typedId = Convert.ChangeType(id, keyType);

       var parameter = Expression.Parameter(typeof(T), "e");
       var property = Expression.Property(parameter, keyName);
       var constant = Expression.Constant(typedId, keyType);
       var equals = Expression.Equal(property, constant);
       var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);

       var entity = await query.FirstOrDefaultAsync(lambda, cancellationToken);

       return entity == null
          ? Result<T>.Fail($"Entity with {keyName} = {id} not found.")
          : Result<T>.Ok(entity);
    }

    public async Task<IResult<T>?> GetByIdAsync(object[] keyValues)
    {
        await Task.FromResult<bool>(true);

        var entityType = _context.Model.FindEntityType(typeof(T));
        var keyProps = entityType.FindPrimaryKey().Properties;

        var param = Expression.Parameter(typeof(T), "e");
        Expression body = null;

        for (int i = 0; i < keyProps.Count; i++)
        {
            var prop = keyProps[i];
            var value = Expression.Constant(keyValues[i], prop.ClrType);

            var efProp = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                new Type[] { prop.ClrType },
                param,
                Expression.Constant(prop.Name)
            );

            var equal = Expression.Equal(efProp, value);

            body = body == null ? equal : Expression.AndAlso(body, equal);
        }

        var lambda = Expression.Lambda<Func<T, bool>>(body, param);

        var entity = _dbSet.FirstOrDefault(lambda);

        return Result<T>.Ok(entity);
    }

    public async Task<IResult<T>> AddAsync(T entity, CancellationToken cancellationToken = default, bool asDetached = false)
    {
       await _dbSet.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        if (asDetached)
            _context.ChangeTracker.Clear(); 
        return Result<T>.Ok(entity, "Entity added successfully.");
    }

    public async Task<IResult<T>> UpdateAsync(T entity, CancellationToken cancellationToken = default, bool asDetached = false)
    {
       _dbSet.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);

        if (asDetached)
            _context.ChangeTracker.Clear();

        return Result<T>.Ok(entity, "Entity updated successfully.");
    }

    public async Task<IResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
       var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
       if (entity == null)
          return Result.Fail($"Entity with ID {id} not found.");

       _dbSet.Remove(entity);
       await _context.SaveChangesAsync(cancellationToken);
       return Result.Ok("Entity deleted successfully.");
    }

    public async Task<IResult> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
       var entity = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
       if (entity == null)
          return Result.Fail($"Entity with ID {id.ToString()} not found.");

       _dbSet.Remove(entity);
       await _context.SaveChangesAsync(cancellationToken);
       return Result.Ok("Entity deleted successfully");
    }

    public async Task<bool> DeleteAsync(int key1, int key2, int key3)
    {
        var _entity = await GetByIdAsync(new object[] { key1, key2, key3 });

        if (!_entity.Success)
            return false;

        _dbSet.Remove(_entity.Data);
        await _context.SaveChangesAsync();
        return true;
    }
    
    public async Task<IResult<T>> GetByIdAsync(int id, string keyName = "Id", CancellationToken cancellationToken = default, IEnumerable<Expression<Func<T, object>>>? includes = null, IEnumerable<Func<IQueryable<T>, IQueryable<T>>>? thenIncludes = null)
    {
       IQueryable<T> query = _dbSet;

       if (includes is not null)
       {
          foreach (var include in includes)
             query = query.Include(include);
       }

       if (thenIncludes is not null)
       {
          foreach (var thenInclude in thenIncludes)
             query = thenInclude(query);
       }

       var entity = await query.FirstOrDefaultAsync(
          e => EF.Property<int>(e, keyName) == id,
          cancellationToken);

       return entity == null
          ? Result<T>.Fail($"Entity with ID {id} not found.")
          : Result<T>.Ok(entity);
    }
    
    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
       return await _dbSet.CountAsync(predicate, cancellationToken);
    }
    
} 