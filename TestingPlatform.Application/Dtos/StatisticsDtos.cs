using TestingPlatform.Domain.Enums;
namespace TestingPlatform.Application.Dtos;
public record TestTypeCountDto(TestType Type, int Count);
public record AssignmentStatsDto(string Name, double AvgDuration, double AvgPassingScore);
public record PublicationMonthDto(bool IsPublic, int Year, int Month, int Count);
public record GroupTestCountDto(string Group, int TestCount);
