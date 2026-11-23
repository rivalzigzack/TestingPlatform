using TestingPlatform.Application.Dtos;
namespace TestingPlatform.Application;
public interface ITestRepository : IDtoRepository<TestDto>
{
    Task<List<TestDto>> GetAllAsync(bool? isPublic, List<int> groupIds, List<int> studentIds);
    Task<List<TestDto>> GetAllForStudent(int studentId);
    Task<List<TestDto>> GetPublishedAsync();
    Task<List<TestDto>> GetByGroupAsync(int groupId);
    Task<List<TestDto>> GetByCourseAsync(int courseId);
    Task<List<TestDto>> GetByProjectAsync(int projectId);
    Task<List<TestDto>> GetByDirectionAsync(int directionId);
    Task<List<TestDto>> GetTopRecentAsync(int count = 5);
    Task<List<TestTypeCountDto>> GetTestCountByTypeAsync();
    Task<List<AssignmentStatsDto>> GetCourseStatsAsync();
    Task<List<AssignmentStatsDto>> GetDirectionAveragesAsync();
    Task<List<PublicationMonthDto>> GetTestTimelineByPublicAsync();
    Task<List<GroupTestCountDto>> GetTopGroupsByTestCountAsync(int top = 10);
}
