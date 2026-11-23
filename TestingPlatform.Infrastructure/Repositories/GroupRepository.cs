using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestingPlatform.Application;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure.Exceptions;
namespace TestingPlatform.Infrastructure.Repositories;
public class GroupRepository(AppDbContext db, IMapper mapper, ILogger<GroupRepository> logger)
    : DtoRepository<Group, GroupDto>(db, mapper, logger), IGroupRepository
{
    protected override IQueryable<Group> Query => Db.Groups.Include(g => g.Direction).Include(g => g.Course).Include(g => g.Project);
    public Task<List<GroupDto>> GetByDirectionAsync(int directionId) => ReadAsync(Query.Where(g => g.DirectionId == directionId));
    public Task<List<GroupDto>> GetByCourseAsync(int courseId) => ReadAsync(Query.Where(g => g.CourseId == courseId));
    public Task<List<GroupDto>> GetByProjectAsync(int projectId) => ReadAsync(Query.Where(g => g.ProjectId == projectId));
    protected override async Task PrepareAsync(Group entity, GroupDto dto, bool creating)
    {
        if (await Db.Groups.AnyAsync(g => g.Id != (creating ? 0 : dto.Id) && g.Name == dto.Name))
            throw new EntityConflictException("Группа с таким названием уже существует.");
        await ResolveAsync<Direction>([dto.Direction.Id]);
        await ResolveAsync<Course>([dto.Course.Id]);
        await ResolveAsync<Project>([dto.Project.Id]);
    }
}
