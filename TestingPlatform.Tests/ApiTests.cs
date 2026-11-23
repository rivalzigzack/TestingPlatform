using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TestingPlatform.Domain.Models;
using TestingPlatform.Infrastructure;
using Xunit;

namespace TestingPlatform.Tests;

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection connection = new("Data Source=:memory:");
    public ApiFactory() => connection.Open();
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureServices(services => {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connection));
        });
    }
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) connection.Dispose();
    }
}

public class ApiTests
{
    private static HttpClient Client(ApiFactory factory) => factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
    private static object Student(string login = "student", int[]? groups = null) => new {
        login, password = "password", email = login + "@example.com", firstName = "Иван", lastName = "Иванов",
        phone = "+79991234567", vkProfileLink = "https://vk.com/test", groupIds = groups ?? new[] {1}
    };
    private static Dictionary<string, object?> Test(string title = "Тест", int[]? students = null, int[]? groups = null,
        int[]? courses = null, int[]? projects = null, int[]? directions = null, DateTime? publish = null, DateTime? deadline = null) => new() {
        ["title"] = title, ["description"] = "Описание", ["type"] = 1, ["isRepeatable"] = true,
        ["publishedAt"] = publish ?? DateTime.UtcNow.AddHours(-1), ["deadline"] = deadline ?? DateTime.UtcNow.AddDays(1),
        ["durationMinutes"] = 30, ["passingScore"] = 10, ["maxAttempts"] = 3,
        ["students"] = students ?? [], ["groups"] = groups ?? [], ["courses"] = courses ?? [],
        ["projects"] = projects ?? [], ["directions"] = directions ?? []
    };
    private static async Task<JsonElement> Json(HttpResponseMessage response, HttpStatusCode status = HttpStatusCode.OK)
    {
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.StatusCode == status, $"Expected {status}, got {response.StatusCode}: {body}");
        return JsonDocument.Parse(body).RootElement.Clone();
    }
    private static async Task<int> Create(HttpClient client, string path, object body)
    {
        var response = await client.PostAsJsonAsync(path, body);
        var json = await Json(response, HttpStatusCode.Created);
        Assert.NotNull(response.Headers.Location);
        Assert.DoesNotContain("password", json.GetRawText().ToLowerInvariant());
        return json.GetProperty("id").GetInt32();
    }

    [Fact]
    public async Task ExistingOffsetDatesMigrateWithoutLosingRows()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        using var db = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options);
        var migrator = db.GetService<IMigrator>();
        await migrator.MigrateAsync("20261012154709_Initial");
        await db.Database.ExecuteSqlRawAsync("""
            INSERT INTO Users (Login, PasswordHash, Email, FirstName, LastName, Role, CreatedAt)
            VALUES ('legacy', 'existing-hash', 'legacy@example.com', 'Иван', 'Иванов', 'Manager', '2026-01-01 12:00:00+03:00');
            """);
        await migrator.MigrateAsync();
        var user = await db.Users.AsNoTracking().SingleAsync();
        Assert.Equal(new DateTime(2026, 1, 1, 9, 0, 0), user.CreatedAt);
        Assert.Equal("existing-hash", user.PasswordHash);
        await migrator.MigrateAsync("20261012154709_Initial");
        using (var command = connection.CreateCommand())
        {
            command.CommandText = "SELECT CreatedAt FROM Users WHERE Login = 'legacy'";
            Assert.EndsWith("+00:00", (string)(await command.ExecuteScalarAsync())!);
        }
        await migrator.MigrateAsync();
        Assert.Equal(user.CreatedAt, (await db.Users.AsNoTracking().SingleAsync()).CreatedAt);
    }

    [Fact]
    public async Task FirstLessonRouteQueryAndHeadersWork()
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        Assert.Equal(7, (await Json(await client.GetAsync("/WeatherForecast/7"))).GetArrayLength());
        var forecasts = await Json(await client.GetAsync("/WeatherForecast/filter?minTemp=0&maxTemp=20"));
        Assert.All(forecasts.EnumerateArray(), item => Assert.InRange(item.GetProperty("temperatureC").GetInt32(), 0, 20));
        using var request = new HttpRequestMessage(HttpMethod.Get, "/WeatherForecast/check-header");
        request.Headers.Add("X-Client", "student");
        var response = await client.SendAsync(request);
        Assert.Equal("student", Assert.Single(response.Headers.GetValues("X-Echo-Client")));
        await Json(await client.GetAsync("/WeatherForecast/0"), HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task MigrationAndMapperAndSwaggerAreValid()
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        var swagger = await Json(await client.GetAsync("/swagger/v1/swagger.json"));
        Assert.True(swagger.GetProperty("paths").TryGetProperty("/api/Users", out _));
        using var scope = factory.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Empty(await db.Database.GetPendingMigrationsAsync());
        Assert.False(db.Database.HasPendingModelChanges());
        Assert.Equal(3, await db.Courses.CountAsync());
        Assert.Equal(1, await db.Groups.CountAsync());
        Assert.Equal(14, db.Model.GetEntityTypes().Count(e => !e.IsPropertyBag));
    }

    [Fact]
    public async Task StudentCrudUpdatesNestedUserAndGroupsAndDeletesUser()
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        var id = await Create(client, "/api/students", Student());
        var student = await Json(await client.GetAsync($"/api/students/{id}"));
        Assert.Equal("student", student.GetProperty("login").GetString());
        Assert.Single(student.GetProperty("groups").EnumerateArray());
        var userId = student.GetProperty("userId").GetInt32();
        string oldHash;
        using (var scope = factory.Services.CreateScope()) {
            var user = await scope.ServiceProvider.GetRequiredService<AppDbContext>().Users.FindAsync(userId);
            oldHash = user!.PasswordHash;
            Assert.StartsWith("PBKDF2-SHA256$", oldHash);
        }
        var update = new {id, login = "changed", email = "changed@example.com", firstName = "Пётр", lastName = "Петров",
            phone = "+79990000000", vkProfileLink = "https://vk.com/changed", groupIds = Array.Empty<int>()};
        Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsJsonAsync($"/api/students/{id}", update)).StatusCode);
        student = await Json(await client.GetAsync($"/api/students/{id}"));
        Assert.Equal("changed", student.GetProperty("login").GetString());
        Assert.Empty(student.GetProperty("groups").EnumerateArray());
        using (var scope = factory.Services.CreateScope())
            Assert.Equal(oldHash, (await scope.ServiceProvider.GetRequiredService<AppDbContext>().Users.FindAsync(userId))!.PasswordHash);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/students/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/users/{userId}")).StatusCode);
    }

    [Fact]
    public async Task StudentCreationIsAtomicAndConflictsReturn409()
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        await Json(await client.PostAsJsonAsync("/api/students", Student("bad", [999])), HttpStatusCode.NotFound);
        Assert.Empty((await Json(await client.GetAsync("/api/users"))).EnumerateArray());
        await Create(client, "/api/students", Student());
        await Json(await client.PostAsJsonAsync("/api/students", Student()), HttpStatusCode.Conflict);
        Assert.Single((await Json(await client.GetAsync("/api/users"))).EnumerateArray());
    }

    [Fact]
    public async Task UserCrudDoesNotExposePasswordsAndProtectsStudentInvariant()
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        var body = new {login="manager", password="password", email="manager@example.com", firstName="Менеджер", lastName="Тест", role=1};
        var id = await Create(client, "/api/users", body);
        await Json(await client.GetAsync("/api/users"));
        Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsJsonAsync($"/api/users/{id}", body)).StatusCode);
        await Json(await client.PostAsJsonAsync("/api/users", new {login="orphan", password="password", email="orphan@example.com", firstName="А", lastName="Б", role=2}), HttpStatusCode.BadRequest);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/users/{id}")).StatusCode);
    }

    [Fact]
    public async Task GroupCrudValidatesReferencesAndLoadsStudents()
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        var group = new {name="Новая", directionId=1, courseId=1, projectId=1};
        var id = await Create(client, "/api/groups", group);
        await Create(client, "/api/students", Student(groups:[id]));
        Assert.Single((await Json(await client.GetAsync($"/api/groups/{id}/students"))).EnumerateArray());
        await Json(await client.PostAsJsonAsync("/api/groups", group), HttpStatusCode.Conflict);
        await Json(await client.PostAsJsonAsync("/api/groups", new {name="Ошибка", directionId=999, courseId=1, projectId=1}), HttpStatusCode.NotFound);
        Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsJsonAsync($"/api/groups/{id}", new {name="Изменена", directionId=2, courseId=2, projectId=2})).StatusCode);
        var loaded = await Json(await client.GetAsync($"/api/groups/{id}"));
        Assert.Equal(2, loaded.GetProperty("direction").GetProperty("id").GetInt32());
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/groups/{id}")).StatusCode);
    }

    [Fact]
    public async Task AllFiveAssignmentsReachStudentAndReplacementClearsEveryList()
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        var student = await Create(client, "/api/students", Student());
        var group = await Json(await client.GetAsync("/api/groups/1"));
        var course = group.GetProperty("course").GetProperty("id").GetInt32();
        var project = group.GetProperty("project").GetProperty("id").GetInt32();
        var direction = group.GetProperty("direction").GetProperty("id").GetInt32();
        var ids = new List<int>();
        ids.Add(await Create(client, "/api/tests", Test("Студент", students:[student, student])));
        ids.Add(await Create(client, "/api/tests", Test("Группа", groups:[1])));
        ids.Add(await Create(client, "/api/tests", Test("Курс", courses:[course])));
        ids.Add(await Create(client, "/api/tests", Test("Проект", projects:[project])));
        ids.Add(await Create(client, "/api/tests", Test("Направление", directions:[direction])));
        await Create(client, "/api/tests", Test("Чужой"));
        Assert.Equal(5, (await Json(await client.GetAsync($"/api/tests/student/{student}"))).GetArrayLength());
        foreach (var id in ids)
            Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsJsonAsync($"/api/tests/{id}", Test("Очищено"))).StatusCode);
        Assert.Empty((await Json(await client.GetAsync($"/api/tests/student/{student}"))).EnumerateArray());
    }

    [Fact]
    public async Task AssignmentsUpdateAtomicallyAndManagerFiltersWork()
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        var student = await Create(client, "/api/students", Student());
        var id = await Create(client, "/api/tests", Test(students:[student], groups:[1], courses:[1], projects:[1], directions:[1]));
        var loaded = await Json(await client.GetAsync($"/api/tests/{id}"));
        foreach (var name in new[] {"students","groups","courses","projects","directions"}) Assert.Single(loaded.GetProperty(name).EnumerateArray());
        await Json(await client.PutAsJsonAsync($"/api/tests/{id}", Test("Не сохранять", groups:[999])), HttpStatusCode.NotFound);
        loaded = await Json(await client.GetAsync($"/api/tests/{id}"));
        Assert.Equal("Тест", loaded.GetProperty("title").GetString());
        Assert.Single(loaded.GetProperty("students").EnumerateArray());
        Assert.Single((await Json(await client.GetAsync($"/api/tests/manage?isPublic=true&groupIds=1&studentIds={student}"))).EnumerateArray());
        Assert.Empty((await Json(await client.GetAsync("/api/tests/manage?groupIds=999"))).EnumerateArray());
    }

    [Fact]
    public async Task PublicationWindowRefreshesOnReadAndStudentResponseHidesAssignments()
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        var student = await Create(client, "/api/students", Student());
        var active = await Create(client, "/api/tests", Test("Активный", students:[student]));
        var future = await Create(client, "/api/tests", Test("Будущий", students:[student], publish:DateTime.UtcNow.AddDays(1), deadline:DateTime.UtcNow.AddDays(2)));
        var expired = await Create(client, "/api/tests", Test("Истёкший", students:[student], publish:DateTime.UtcNow.AddDays(-2), deadline:DateTime.UtcNow.AddDays(-1)));
        using (var scope = factory.Services.CreateScope()) {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Tests.ExecuteUpdateAsync(s => s.SetProperty(t => t.IsPublic, true));
        }
        var visible = await Json(await client.GetAsync($"/api/tests/student/{student}"));
        Assert.Single(visible.EnumerateArray());
        Assert.Equal(active, visible[0].GetProperty("id").GetInt32());
        Assert.False(visible[0].TryGetProperty("students", out _));
        foreach (var id in new[] {future,expired})
            Assert.False((await Json(await client.GetAsync($"/api/tests/{id}"))).GetProperty("isPublic").GetBoolean());
    }

    [Fact]
    public async Task StatisticsAndRecentExecuteAsSqlOnSQLite()
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        await Create(client, "/api/tests", Test("Первый", groups:[1], courses:[1], directions:[1]));
        await Create(client, "/api/tests", Test("Второй", groups:[1], courses:[1], directions:[1]));
        await Create(client, "/api/tests", Test("Будущий", publish:DateTime.UtcNow.AddDays(1), deadline:DateTime.UtcNow.AddDays(2)));
        var stats = await Json(await client.GetAsync("/api/tests/statistics"));
        Assert.Equal(3, stats.GetProperty("byType")[0].GetProperty("count").GetInt32());
        Assert.Equal(30, stats.GetProperty("courses")[0].GetProperty("avgDuration").GetDouble());
        Assert.Equal(2, stats.GetProperty("topGroups")[0].GetProperty("testCount").GetInt32());
        Assert.Equal(3, stats.GetProperty("timeline").EnumerateArray().Sum(x => x.GetProperty("count").GetInt32()));
        var recent = await Json(await client.GetAsync("/api/tests/recent?count=1"));
        Assert.Single(recent.EnumerateArray());
        Assert.Equal("Второй", recent[0].GetProperty("title").GetString());
        await Json(await client.GetAsync("/api/tests/recent?count=0"), HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task QuestionAndAnswerCrudIncludesNumberAndMaxScoreUpdates()
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        var test = await Create(client, "/api/tests", Test());
        var question = new {text="Вопрос", number=1, answerType=1, isScoring=true, maxScore=1, testId=test};
        var id = await Create(client, "/api/questions", question);
        await Json(await client.PostAsJsonAsync("/api/questions", question), HttpStatusCode.Conflict);
        Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsJsonAsync($"/api/questions/{id}", new {text="Новый", number=2, answerType=2, isScoring=true, maxScore=5, testId=test})).StatusCode);
        var loaded = (await Json(await client.GetAsync($"/api/questions/by-test/{test}")))[0];
        Assert.Equal(2, loaded.GetProperty("number").GetInt32());
        Assert.Equal(5, loaded.GetProperty("maxScore").GetInt32());
        var answer = await Create(client, "/api/answers", new {text="Ответ", isCorrect=true, questionId=id});
        Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsJsonAsync($"/api/answers/{answer}", new {text="Изменён", isCorrect=false, questionId=id})).StatusCode);
        Assert.Single((await Json(await client.GetAsync($"/api/answers/by-question/{id}"))).EnumerateArray());
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/questions/{id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/answers/{answer}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/tests/{test}")).StatusCode);
    }

    [Fact]
    public async Task UnscoredQuestionKeepsFalseInsteadOfDatabaseDefaultTrue()
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        var test = await Create(client, "/api/tests", Test());
        var id = await Create(client, "/api/questions", new {text="Свободный ответ", number=1, answerType=3, isScoring=false, maxScore=0, testId=test});
        var question = await Json(await client.GetAsync($"/api/questions/{id}"));
        Assert.False(question.GetProperty("isScoring").GetBoolean());
        Assert.Equal(0, question.GetProperty("maxScore").GetInt32());
    }

    [Fact]
    public async Task LegacyRepositoriesRejectMissingEntitiesAndBestResultUsesScore()
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        var student = await Create(client, "/api/students", Student());
        var test = await Create(client, "/api/tests", Test());
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var best = new Attempt {TestId=test, StudentId=student, Score=10};
        var recent = new Attempt {TestId=test, StudentId=student, Score=2};
        db.Attempts.AddRange(best, recent);
        await db.SaveChangesAsync();
        db.TestResults.AddRange(
            new TestResult {TestId=test, StudentId=student, AttemptId=best.Id, Passed=true},
            new TestResult {TestId=test, StudentId=student, AttemptId=recent.Id, Passed=false});
        await db.SaveChangesAsync();
        var results = scope.ServiceProvider.GetRequiredService<TestingPlatform.Application.ITestResultRepository>();
        Assert.Equal(best.Id, (await results.GetBestByTestAndStudentAsync(test, student))!.AttemptId);
        var attempts = scope.ServiceProvider.GetRequiredService<TestingPlatform.Application.IAttemptRepository>();
        await Assert.ThrowsAsync<TestingPlatform.Infrastructure.Exceptions.EntityNotFoundException>(() => attempts.GetByIdAsync(999));
        await Assert.ThrowsAsync<TestingPlatform.Infrastructure.Exceptions.EntityNotFoundException>(() => attempts.UpdateAsync(new Attempt {Id=999}));
        await Assert.ThrowsAsync<TestingPlatform.Infrastructure.Exceptions.EntityNotFoundException>(() => attempts.DeleteAsync(999));
    }

    [Fact]
    public async Task RestrictiveDeleteReturnsConflictInsteadOf500()
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        var student = await Create(client, "/api/students", Student());
        var test = await Create(client, "/api/tests", Test());
        using (var scope = factory.Services.CreateScope()) {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Attempts.Add(new Attempt {StudentId=student, TestId=test});
            await db.SaveChangesAsync();
        }
        await Json(await client.DeleteAsync($"/api/tests/{test}"), HttpStatusCode.Conflict);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/students/{student}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/tests/{test}")).StatusCode);
    }

    [Theory]
    [InlineData("students")]
    [InlineData("users")]
    [InlineData("groups")]
    [InlineData("tests")]
    [InlineData("questions")]
    [InlineData("answers")]
    public async Task InvalidAndMissingIdsUseConsistentErrors(string path)
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        await Json(await client.GetAsync($"/api/{path}/0"), HttpStatusCode.BadRequest);
        var missing = await Json(await client.GetAsync($"/api/{path}/999"), HttpStatusCode.NotFound);
        Assert.True(missing.TryGetProperty("traceId", out _));
        await Json(await client.DeleteAsync($"/api/{path}/999"), HttpStatusCode.NotFound);
        await Json(await client.PostAsJsonAsync($"/api/{path}", new {}), HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DateOffsetsNormalizeAndInvalidPayloadsAreRejected()
    {
        using var factory = new ApiFactory(); using var client = Client(factory);
        var body = Test();
        body["publishedAt"] = "2026-01-01T12:00:00+03:00";
        body["deadline"] = "2030-01-01T12:00:00+03:00";
        var id = await Create(client, "/api/tests", body);
        var value = (await Json(await client.GetAsync($"/api/tests/{id}"))).GetProperty("publishedAt").GetString();
        Assert.Equal("2026-01-01T09:00:00Z", value);
        body["groups"] = null;
        await Json(await client.PostAsJsonAsync("/api/tests", body), HttpStatusCode.BadRequest);
        body["groups"] = new[] {-1};
        await Json(await client.PostAsJsonAsync("/api/tests", body), HttpStatusCode.BadRequest);
        body["groups"] = Array.Empty<int>(); body["type"] = 0;
        await Json(await client.PostAsJsonAsync("/api/tests", body), HttpStatusCode.BadRequest);
        await Json(await client.PutAsJsonAsync($"/api/tests/{id}", new {id=999}), HttpStatusCode.BadRequest);
    }
}
