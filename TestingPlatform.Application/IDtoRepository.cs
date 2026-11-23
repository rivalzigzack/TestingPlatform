using TestingPlatform.Application.Dtos;
namespace TestingPlatform.Application;
public interface IDtoRepository<TDto> where TDto : EntityDto
{
    Task<List<TDto>> GetAllAsync();
    Task<TDto> GetByIdAsync(int id);
    Task<int> CreateAsync(TDto dto);
    Task UpdateAsync(TDto dto);
    Task DeleteAsync(int id);
}
