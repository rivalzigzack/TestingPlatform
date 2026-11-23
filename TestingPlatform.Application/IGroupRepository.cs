using TestingPlatform.Application.Dtos;
namespace TestingPlatform.Application;
public interface IGroupRepository : IDtoRepository<GroupDto>
{
    Task<List<GroupDto>> GetByDirectionAsync(int directionId);
    Task<List<GroupDto>> GetByCourseAsync(int courseId);
    Task<List<GroupDto>> GetByProjectAsync(int projectId);
}
