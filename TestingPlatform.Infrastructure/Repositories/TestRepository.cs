using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestingPlatform.Application;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Models;
namespace TestingPlatform.Infrastructure.Repositories;
public class TestRepository(AppDbContext db, IMapper mapper, ILogger<TestRepository> logger)
    : DtoRepository<Test, TestDto>(db, mapper, logger), ITestRepository
{
    protected override IQueryable<Test> Query => Db.Tests
        .Include(t => t.Students).ThenInclude(s => s.User)
        .Include(t => t.Groups).ThenInclude(g => g.Direction)
        .Include(t => t.Groups).ThenInclude(g => g.Course)
        .Include(t => t.Groups).ThenInclude(g => g.Project)
        .Include(t => t.Courses).Include(t => t.Projects).Include(t => t.Directions).AsSplitQuery();

    // UTC DateTime comparisons are translated by SQLite; no client-side table scan.
    private async Task RefreshPublicationStatusesAsync()
    {
        var now = DateTime.UtcNow;
        await Db.Tests.Where(t => t.IsPublic != (t.PublishedAt <= now && t.Deadline > now))
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.IsPublic, t => t.PublishedAt <= now && t.Deadline > now));
    }

    public override Task<List<TestDto>> GetAllAsync() => GetAllAsync(null, [], []);
    public async Task<List<TestDto>> GetAllAsync(bool? isPublic, List<int> groupIds, List<int> studentIds)
    {
        await RefreshPublicationStatusesAsync();
        var query = Query;
        if (isPublic.HasValue) query = query.Where(t => t.IsPublic == isPublic);
        if (groupIds.Count > 0) query = query.Where(t => t.Groups.Any(g => groupIds.Contains(g.Id)));
        if (studentIds.Count > 0) query = query.Where(t => t.Students.Any(s => studentIds.Contains(s.Id)));
        return await ReadAsync(query.OrderByDescending(t => t.PublishedAt).ThenBy(t => t.Title).ThenBy(t => t.Id));
    }
    public override async Task<TestDto> GetByIdAsync(int id)
    {
        await RefreshPublicationStatusesAsync();
        return await base.GetByIdAsync(id);
    }

    public async Task<List<TestDto>> GetAllForStudent(int studentId)
    {
        await ResolveAsync<Student>([studentId]);
        await RefreshPublicationStatusesAsync();
        return await ReadAsync(Query.Where(t => t.IsPublic && (
            t.Students.Any(s => s.Id == studentId)
            || t.Groups.Any(g => g.Students.Any(s => s.Id == studentId))
            || t.Courses.Any(c => c.Groups.Any(g => g.Students.Any(s => s.Id == studentId)))
            || t.Projects.Any(p => p.Groups.Any(g => g.Students.Any(s => s.Id == studentId)))
            || t.Directions.Any(d => d.Groups.Any(g => g.Students.Any(s => s.Id == studentId)))))
            .OrderByDescending(t => t.PublishedAt).ThenBy(t => t.Title));
    }
    public Task<List<TestDto>> GetPublishedAsync() => GetAllAsync(true, [], []);
    public Task<List<TestDto>> GetByGroupAsync(int groupId) => GetAllAsync(null, [groupId], []);
    private async Task<List<TestDto>> ReadRefreshedAsync(IQueryable<Test> query)
    {
        await RefreshPublicationStatusesAsync();
        return await ReadAsync(query);
    }
    public Task<List<TestDto>> GetByCourseAsync(int courseId) => ReadRefreshedAsync(Query.Where(t => t.Courses.Any(c => c.Id == courseId)));
    public Task<List<TestDto>> GetByProjectAsync(int projectId) => ReadRefreshedAsync(Query.Where(t => t.Projects.Any(p => p.Id == projectId)));
    public Task<List<TestDto>> GetByDirectionAsync(int directionId) => ReadRefreshedAsync(Query.Where(t => t.Directions.Any(d => d.Id == directionId)));
    public Task<List<TestDto>> GetTopRecentAsync(int count = 5)
    {
        if (count is < 1 or > 100) throw new ArgumentException("Количество должно быть от 1 до 100.");
        return ReadRefreshedAsync(Query.Where(t => t.IsPublic).OrderByDescending(t => t.PublishedAt).ThenByDescending(t => t.Id).Take(count));
    }

    protected override async Task PrepareAsync(Test entity, TestDto dto, bool creating)
    {
        // Validate every list before replacing any relation. SaveChanges commits the graph atomically.
        var students = await ResolveAsync<Student>(dto.Students.Select(s => s.Id));
        var groups = await ResolveAsync<Group>(dto.Groups.Select(g => g.Id));
        var courses = await ResolveAsync<Course>(dto.Courses.Select(c => c.Id));
        var projects = await ResolveAsync<Project>(dto.Projects.Select(p => p.Id));
        var directions = await ResolveAsync<Direction>(dto.Directions.Select(d => d.Id));
        entity.Students.Clear(); entity.Students.AddRange(students);
        entity.Groups.Clear(); entity.Groups.AddRange(groups);
        entity.Courses.Clear(); entity.Courses.AddRange(courses);
        entity.Projects.Clear(); entity.Projects.AddRange(projects);
        entity.Directions.Clear(); entity.Directions.AddRange(directions);
        var now = DateTime.UtcNow;
        entity.IsPublic = dto.PublishedAt <= now && dto.Deadline > now;
    }

    public async Task<List<TestTypeCountDto>> GetTestCountByTypeAsync()
    {
        await RefreshPublicationStatusesAsync();
        return await Db.Tests.AsNoTracking().GroupBy(t => t.Type)
            .Select(g => new TestTypeCountDto(g.Key, g.Count())).ToListAsync();
    }
    public async Task<List<AssignmentStatsDto>> GetCourseStatsAsync()
    {
        await RefreshPublicationStatusesAsync();
        return await Db.Courses.AsNoTracking().Select(c => new AssignmentStatsDto(c.Name,
            c.Tests.Average(t => (double?)t.DurationMinutes) ?? 0,
            c.Tests.Average(t => (double?)t.PassingScore) ?? 0)).ToListAsync();
    }
    public async Task<List<AssignmentStatsDto>> GetDirectionAveragesAsync()
    {
        await RefreshPublicationStatusesAsync();
        return await Db.Directions.AsNoTracking().Select(d => new {
            d.Name,
            AvgDuration = d.Tests.Average(t => (double?)t.DurationMinutes) ?? 0,
            AvgPassingScore = d.Tests.Average(t => (double?)t.PassingScore) ?? 0 })
            .OrderByDescending(x => x.AvgPassingScore)
            .Select(x => new AssignmentStatsDto(x.Name, x.AvgDuration, x.AvgPassingScore)).ToListAsync();
    }
    public async Task<List<PublicationMonthDto>> GetTestTimelineByPublicAsync()
    {
        await RefreshPublicationStatusesAsync();
        return await Db.Tests.AsNoTracking().GroupBy(t => new {t.IsPublic, t.PublishedAt.Year, t.PublishedAt.Month})
            .Select(g => new {g.Key.IsPublic, g.Key.Year, g.Key.Month, Count = g.Count()})
            .OrderBy(x => x.Year).ThenBy(x => x.Month).ThenByDescending(x => x.IsPublic)
            .Select(x => new PublicationMonthDto(x.IsPublic, x.Year, x.Month, x.Count)).ToListAsync();
    }
    public async Task<List<GroupTestCountDto>> GetTopGroupsByTestCountAsync(int top = 10)
    {
        if (top is < 1 or > 100) throw new ArgumentException("Количество должно быть от 1 до 100.");
        await RefreshPublicationStatusesAsync();
        return await Db.Tests.AsNoTracking().SelectMany(t => t.Groups, (t, g) => new {g.Name, TestId = t.Id})
            .GroupBy(x => x.Name).Select(g => new {Group = g.Key, TestCount = g.Select(x => x.TestId).Distinct().Count()})
            .OrderByDescending(x => x.TestCount).ThenBy(x => x.Group).Take(top)
            .Select(x => new GroupTestCountDto(x.Group, x.TestCount)).ToListAsync();
    }
}
