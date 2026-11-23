using TestingPlatform.Application.Dtos;
namespace TestingPlatform.Application;
public interface IStudentRepository : IDtoRepository<StudentDto>
{
    Task<List<StudentDto>> GetByGroupAsync(int groupId);
}
