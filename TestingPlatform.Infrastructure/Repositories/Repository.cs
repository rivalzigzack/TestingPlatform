using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application;
using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _db;
        protected readonly DbSet<T> _set;

        public Repository(AppDbContext db)
        {
            _db = db;
            _set = db.Set<T>();
        }

        public virtual async Task<List<T>> GetAllAsync()
        {
            return await _set.ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _set.FindAsync(id) ?? throw new TestingPlatform.Infrastructure.Exceptions.EntityNotFoundException($"{typeof(T).Name} с id={id} не найден.");
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            var entry = await _set.AddAsync(entity);
            await _db.SaveChangesAsync();
            return entry.Entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            var id = _db.Entry(entity).Property<int>("Id").CurrentValue;
            if (!await _set.AnyAsync(e => EF.Property<int>(e, "Id") == id))
                throw new TestingPlatform.Infrastructure.Exceptions.EntityNotFoundException($"{typeof(T).Name} с id={id} не найден.");
            _set.Update(entity);
            await _db.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entity = await _set.FindAsync(id) ?? throw new TestingPlatform.Infrastructure.Exceptions.EntityNotFoundException($"{typeof(T).Name} с id={id} не найден.");
            if (entity is not null)
            {
                _set.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }
    }
}
