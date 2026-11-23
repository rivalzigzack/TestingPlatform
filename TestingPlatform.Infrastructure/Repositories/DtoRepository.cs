using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestingPlatform.Application;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Infrastructure.Exceptions;
namespace TestingPlatform.Infrastructure.Repositories;

public abstract class DtoRepository<TEntity, TDto>(AppDbContext db, IMapper mapper, ILogger logger)
    : IDtoRepository<TDto> where TEntity : class, new() where TDto : EntityDto
{
    protected AppDbContext Db { get; } = db;
    protected IMapper Mapper { get; } = mapper;
    protected ILogger Logger { get; } = logger;
    protected virtual IQueryable<TEntity> Query => Db.Set<TEntity>();

    protected async Task<List<TDto>> ReadAsync(IQueryable<TEntity> query)
        => Mapper.Map<List<TDto>>(await query.AsNoTracking().ToListAsync());

    protected async Task<TEntity> FindAsync(int id)
    {
        if (id <= 0) throw new ArgumentException("Идентификатор должен быть положительным.");
        return await Query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id)
            ?? throw new EntityNotFoundException($"{typeof(TEntity).Name} с id={id} не найден.");
    }

    public virtual async Task<List<TDto>> GetAllAsync()
    {
        Logger.LogInformation("Чтение списка {Entity}", typeof(TEntity).Name);
        var result = await ReadAsync(Query);
        Logger.LogInformation("Получено {Count} записей {Entity}", result.Count, typeof(TEntity).Name);
        return result;
    }

    public virtual async Task<TDto> GetByIdAsync(int id)
    {
        Logger.LogInformation("Чтение {Entity} Id={EntityId}", typeof(TEntity).Name, id);
        if (id <= 0) throw new ArgumentException("Идентификатор должен быть положительным.");
        var entity = await Query.AsNoTracking().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id)
            ?? throw new EntityNotFoundException($"{typeof(TEntity).Name} с id={id} не найден.");
        return Mapper.Map<TDto>(entity);
    }

    protected virtual Task PrepareAsync(TEntity entity, TDto dto, bool creating) => Task.CompletedTask;

    public virtual async Task<int> CreateAsync(TDto dto)
    {
        var entity = Mapper.Map<TEntity>(dto);
        await PrepareAsync(entity, dto, true);
        await Db.Set<TEntity>().AddAsync(entity);
        await Db.SaveChangesAsync();
        var id = Db.Entry(entity).Property<int>("Id").CurrentValue;
        Logger.LogInformation("Создан {Entity} Id={EntityId}", typeof(TEntity).Name, id);
        return id;
    }

    public virtual async Task UpdateAsync(TDto dto)
    {
        var entity = await FindAsync(dto.Id);
        await PrepareAsync(entity, dto, false);
        Mapper.Map(dto, entity);
        await Db.SaveChangesAsync();
        Logger.LogInformation("Обновлён {Entity} Id={EntityId}", typeof(TEntity).Name, dto.Id);
    }

    public virtual async Task DeleteAsync(int id)
    {
        Db.Set<TEntity>().Remove(await FindAsync(id));
        await Db.SaveChangesAsync();
        Logger.LogInformation("Удалён {Entity} Id={EntityId}", typeof(TEntity).Name, id);
    }

    protected async Task<List<T>> ResolveAsync<T>(IEnumerable<int>? ids) where T : class
    {
        var distinct = (ids ?? []).Distinct().ToArray();
        if (distinct.Any(id => id <= 0)) throw new ArgumentException("Идентификаторы связей должны быть положительными.");
        var entities = await Db.Set<T>().Where(x => distinct.Contains(EF.Property<int>(x, "Id"))).ToListAsync();
        if (entities.Count != distinct.Length) throw new EntityNotFoundException($"Некоторые {typeof(T).Name} не найдены.");
        return entities;
    }
}
