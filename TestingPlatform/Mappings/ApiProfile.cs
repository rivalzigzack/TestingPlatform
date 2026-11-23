using AutoMapper;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Requests.User;
using TestingPlatform.Responses.User;
using TestingPlatform.Requests.Student;
using TestingPlatform.Responses.Student;
using TestingPlatform.Requests.Group;
using TestingPlatform.Responses.Group;
using TestingPlatform.Requests.Question;
using TestingPlatform.Responses.Question;
using TestingPlatform.Requests.Answer;
using TestingPlatform.Responses.Answer;
using TestingPlatform.Requests.Test;
using TestingPlatform.Responses.Test;
namespace TestingPlatform.Mappings;
public class ApiProfile : Profile
{
    public ApiProfile()
    {
        CreateMap<UserDto, UserResponse>();
        CreateMap<CreateUserRequest, UserDto>(MemberList.None);
        CreateMap<UpdateUserRequest, UserDto>(MemberList.None);
        CreateMap<StudentDto, StudentResponse>()
            .ForMember(d => d.Login, m => m.MapFrom(s => s.User.Login))
            .ForMember(d => d.Email, m => m.MapFrom(s => s.User.Email))
            .ForMember(d => d.FirstName, m => m.MapFrom(s => s.User.FirstName))
            .ForMember(d => d.MiddleName, m => m.MapFrom(s => s.User.MiddleName))
            .ForMember(d => d.LastName, m => m.MapFrom(s => s.User.LastName));
        CreateMap<CreateStudentRequest, StudentDto>(MemberList.None)
            .ForMember(d => d.User, m => m.MapFrom(s => s))
            .ForMember(d => d.Groups, m => m.MapFrom(s => s.GroupIds.Select(id => new GroupDto { Id = id })));
        CreateMap<UpdateStudentRequest, StudentDto>(MemberList.None)
            .ForMember(d => d.User, m => m.MapFrom(s => s))
            .ForMember(d => d.Groups, m => m.MapFrom(s => s.GroupIds.Select(id => new GroupDto { Id = id })));
        CreateMap<GroupDto, GroupResponse>();
        CreateMap<CreateGroupRequest, GroupDto>(MemberList.None)
            .ForMember(d => d.Course, m => m.MapFrom(s => new CourseDto { Id = s.CourseId }))
            .ForMember(d => d.Direction, m => m.MapFrom(s => new DirectionDto { Id = s.DirectionId }))
            .ForMember(d => d.Project, m => m.MapFrom(s => new ProjectDto { Id = s.ProjectId }));
        CreateMap<UpdateGroupRequest, GroupDto>(MemberList.None)
            .ForMember(d => d.Course, m => m.MapFrom(s => new CourseDto { Id = s.CourseId }))
            .ForMember(d => d.Direction, m => m.MapFrom(s => new DirectionDto { Id = s.DirectionId }))
            .ForMember(d => d.Project, m => m.MapFrom(s => new ProjectDto { Id = s.ProjectId }));
        CreateMap<QuestionDto, QuestionResponse>();
        CreateMap<CreateQuestionRequest, QuestionDto>(MemberList.None);
        CreateMap<UpdateQuestionRequest, QuestionDto>(MemberList.None);
        CreateMap<AnswerDto, AnswerResponse>();
        CreateMap<CreateAnswerRequest, AnswerDto>(MemberList.None);
        CreateMap<UpdateAnswerRequest, AnswerDto>(MemberList.None);
        CreateMap<TestDto, TestResponse>();
        CreateMap<CreateTestRequest, TestDto>(MemberList.None)
            .ForMember(d => d.Students, m => m.MapFrom(s => s.Students.Select(id => new StudentDto { Id = id })))
            .ForMember(d => d.Groups, m => m.MapFrom(s => s.Groups.Select(id => new GroupDto { Id = id })))
            .ForMember(d => d.Courses, m => m.MapFrom(s => s.Courses.Select(id => new CourseDto { Id = id })))
            .ForMember(d => d.Projects, m => m.MapFrom(s => s.Projects.Select(id => new ProjectDto { Id = id })))
            .ForMember(d => d.Directions, m => m.MapFrom(s => s.Directions.Select(id => new DirectionDto { Id = id })));
        CreateMap<UpdateTestRequest, TestDto>(MemberList.None)
            .ForMember(d => d.Students, m => m.MapFrom(s => s.Students.Select(id => new StudentDto { Id = id })))
            .ForMember(d => d.Groups, m => m.MapFrom(s => s.Groups.Select(id => new GroupDto { Id = id })))
            .ForMember(d => d.Courses, m => m.MapFrom(s => s.Courses.Select(id => new CourseDto { Id = id })))
            .ForMember(d => d.Projects, m => m.MapFrom(s => s.Projects.Select(id => new ProjectDto { Id = id })))
            .ForMember(d => d.Directions, m => m.MapFrom(s => s.Directions.Select(id => new DirectionDto { Id = id })));
        CreateMap<CreateStudentRequest, UserDto>(MemberList.None);
        CreateMap<UpdateStudentRequest, UserDto>(MemberList.None);
        CreateMap<TestDto, TestForManagerResponse>();
        CreateMap<TestDto, TestForStudentResponse>();
    }
}
