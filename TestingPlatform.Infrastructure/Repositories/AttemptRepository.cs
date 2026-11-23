using Microsoft.EntityFrameworkCore;
using TestingPlatform.Application;
using TestingPlatform.Domain.Models;

namespace TestingPlatform.Infrastructure.Repositories
{
    public class AttemptRepository : Repository<Attempt>, IAttemptRepository
    {
        public AttemptRepository(AppDbContext db) : base(db) { }

        public async Task<List<Attempt>> GetByTestAsync(int testId)
        {
            return await _set.Where(a => a.TestId == testId).ToListAsync();
        }

        public async Task<List<Attempt>> GetByStudentAsync(int studentId)
        {
            return await _set.Where(a => a.StudentId == studentId).ToListAsync();
        }
    }
}