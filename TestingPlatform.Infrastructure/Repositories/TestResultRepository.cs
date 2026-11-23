using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application;
using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure.Repositories
{
    public class TestResultRepository : Repository<TestResult>, ITestResultRepository
    {
        public TestResultRepository(AppDbContext db) : base(db) { }

        public async Task<List<TestResult>> GetByTestAsync(int testId)
        {
            return await _set.Where(r => r.TestId == testId).ToListAsync();
        }

        public async Task<List<TestResult>> GetByStudentAsync(int studentId)
        {
            return await _set.Where(r => r.StudentId == studentId).ToListAsync();
        }

        public async Task<TestResult?> GetBestByTestAndStudentAsync(int testId, int studentId)
        {
            return await _set
                .Where(r => r.TestId == testId && r.StudentId == studentId)
                .OrderByDescending(r => r.Attempt!.Score)
                .ThenByDescending(r => r.AttemptId)
                .FirstOrDefaultAsync();
        }
    }
}