using TestingPlatform.Domain.Models;

namespace TestingPlatform.Application
{
    public interface IAttemptRepository : IRepository<Attempt>
    {
        Task<List<Attempt>> GetByTestAsync(int testId);
        Task<List<Attempt>> GetByStudentAsync(int studentId);
    }
}