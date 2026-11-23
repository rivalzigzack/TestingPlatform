using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestingPlatform.Application;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Enums;
using TestingPlatform.Domain.Models;
namespace TestingPlatform.Infrastructure.Repositories;
public class StudentRepository(AppDbContext db, IMapper mapper, ILogger<StudentRepository> logger)
    : DtoRepository<Student, StudentDto>(db, mapper, logger), IStudentRepository
{
    protected override IQueryable<Student> Query => Db.Students.Include(s => s.User)
        .Include(s => s.Groups).ThenInclude(g => g.Direction)
        .Include(s => s.Groups).ThenInclude(g => g.Course)
        .Include(s => s.Groups).ThenInclude(g => g.Project).AsSplitQuery();

    public Task<List<StudentDto>> GetByGroupAsync(int groupId)
        => ReadAsync(Query.Where(s => s.Groups.Any(g => g.Id == groupId)));

    protected override async Task PrepareAsync(Student entity, StudentDto dto, bool creating)
    {
        dto.User.Role = UserRole.Student;
        await UserPersistence.ValidateAsync(Db, dto.User, creating ? 0 : entity.UserId, creating);
        var groups = await ResolveAsync<Group>(dto.Groups.Select(g => g.Id));
        var user = creating ? Mapper.Map<User>(dto.User) : entity.User!;
        if (!creating) Mapper.Map(dto.User, user);
        user.Role = UserRole.Student;
        UserPersistence.SetPassword(user, dto.User.Password);
        entity.User = user;
        entity.Groups.Clear();
        entity.Groups.AddRange(groups);
    }

    public override async Task DeleteAsync(int id)
    {
        var student = await FindAsync(id);
        Db.Users.Remove(student.User!); // The User -> Student FK cascades in this direction.
        await Db.SaveChangesAsync();
        Logger.LogInformation("Удалён студент Id={StudentId} вместе с пользователем", id);
    }
}
