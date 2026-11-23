using TestingPlatform.Domain.Models;

namespace TestingPlatform.Application
{
    public interface ITestResultRepository : IRepository<TestResult>
    {
        Task<List<TestResult>> GetByTestAsync(int testId);
        Task<List<TestResult>> GetByStudentAsync(int studentId);
        Task<TestResult?> GetBestByTestAndStudentAsync(int testId, int studentId);
    }
}