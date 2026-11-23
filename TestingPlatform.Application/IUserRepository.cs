using TestingPlatform.Application.Dtos;
namespace TestingPlatform.Application;
public interface IUserRepository : IDtoRepository<UserDto>
{
    Task<UserDto?> GetByLoginAsync(string login);
    Task<UserDto?> GetByEmailAsync(string email);
}
