using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestingPlatform.Application;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Enums;
using TestingPlatform.Domain.Models;
namespace TestingPlatform.Infrastructure.Repositories;
public class UserRepository(AppDbContext db, IMapper mapper, ILogger<UserRepository> logger)
    : DtoRepository<User, UserDto>(db, mapper, logger), IUserRepository
{
    public async Task<UserDto?> GetByLoginAsync(string login)
        => Mapper.Map<UserDto?>(await Db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Login == login));
    public async Task<UserDto?> GetByEmailAsync(string email)
        => Mapper.Map<UserDto?>(await Db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email));

    protected override async Task PrepareAsync(User entity, UserDto dto, bool creating)
    {
        await UserPersistence.ValidateAsync(Db, dto, creating ? 0 : dto.Id, creating);
        var hasStudent = !creating && await Db.Students.AnyAsync(s => s.UserId == entity.Id);
        if ((dto.Role == UserRole.Student) != hasStudent)
            throw new ArgumentException("Роль Student требует профиля студента. Создайте его через /api/students; менять роль связанного пользователя нельзя.");
        UserPersistence.SetPassword(entity, dto.Password);
    }
}
