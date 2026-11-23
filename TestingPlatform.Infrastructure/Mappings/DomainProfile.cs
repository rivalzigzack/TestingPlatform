using AutoMapper;
using TestingPlatform.Application.Dtos;
using TestingPlatform.Domain.Models;
namespace TestingPlatform.Infrastructure.Mappings;
public class DomainProfile : Profile
{
    public DomainProfile()
    {
        CreateMap<Direction, DirectionDto>();
        CreateMap<Course, CourseDto>();
        CreateMap<Project, ProjectDto>();
        CreateMap<User, UserDto>().ForMember(d => d.Password, m => m.Ignore());
        CreateMap<UserDto, User>()
            .ForMember(d => d.Id, m => m.Ignore())
            .ForMember(d => d.PasswordHash, m => m.Ignore())
            .ForMember(d => d.CreatedAt, m => m.Ignore())
            .ForMember(d => d.Student, m => m.Ignore());
        CreateMap<Student, StudentDto>();
        CreateMap<StudentDto, Student>()
            .ForMember(d => d.Id, m => m.Ignore())
            .ForMember(d => d.UserId, m => m.Ignore())
            .ForMember(d => d.User, m => m.Ignore())
            .ForMember(d => d.Groups, m => m.Ignore())
            .ForMember(d => d.Tests, m => m.Ignore())
            .ForMember(d => d.Attempts, m => m.Ignore())
            .ForMember(d => d.TestResults, m => m.Ignore());
        CreateMap<Group, GroupDto>();
        CreateMap<GroupDto, Group>()
            .ForMember(d => d.Id, m => m.Ignore())
            .ForMember(d => d.DirectionId, m => m.MapFrom(s => s.Direction.Id))
            .ForMember(d => d.CourseId, m => m.MapFrom(s => s.Course.Id))
            .ForMember(d => d.ProjectId, m => m.MapFrom(s => s.Project.Id))
            .ForMember(d => d.Direction, m => m.Ignore())
            .ForMember(d => d.Course, m => m.Ignore())
            .ForMember(d => d.Project, m => m.Ignore())
            .ForMember(d => d.Students, m => m.Ignore())
            .ForMember(d => d.Tests, m => m.Ignore());
        CreateMap<Test, TestDto>();
        CreateMap<TestDto, Test>()
            .ForMember(d => d.Id, m => m.Ignore())
            .ForMember(d => d.CreatedAt, m => m.Ignore())
            .ForMember(d => d.IsPublic, m => m.Ignore())
            .ForMember(d => d.Questions, m => m.Ignore())
            .ForMember(d => d.Students, m => m.Ignore())
            .ForMember(d => d.Groups, m => m.Ignore())
            .ForMember(d => d.Courses, m => m.Ignore())
            .ForMember(d => d.Projects, m => m.Ignore())
            .ForMember(d => d.Directions, m => m.Ignore());
        CreateMap<Question, QuestionDto>();
        CreateMap<QuestionDto, Question>()
            .ForMember(d => d.Id, m => m.Ignore())
            .ForMember(d => d.Test, m => m.Ignore())
            .ForMember(d => d.Answers, m => m.Ignore())
            .ForMember(d => d.UserAttemptAnswers, m => m.Ignore());
        CreateMap<Answer, AnswerDto>();
        CreateMap<AnswerDto, Answer>()
            .ForMember(d => d.Id, m => m.Ignore())
            .ForMember(d => d.Question, m => m.Ignore())
            .ForMember(d => d.UserSelectedOptions, m => m.Ignore());
    }
}
