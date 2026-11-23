using TestingPlatform.Application.Dtos;
namespace TestingPlatform.Responses.Test;
public class TestForManagerResponse : TestResponse
{
    public DateTime CreatedAt { get; set; }
    public bool IsPublic { get; set; }
    public List<TestingPlatform.Responses.Student.StudentResponse> Students { get; set; } = new();
    public List<TestingPlatform.Responses.Group.GroupResponse> Groups { get; set; } = new();
    public List<CourseDto> Courses { get; set; } = new();
    public List<ProjectDto> Projects { get; set; } = new();
    public List<DirectionDto> Directions { get; set; } = new();
}
public class TestForStudentResponse : TestResponse;
