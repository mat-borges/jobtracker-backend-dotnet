using JobTracker.Application.DTOs;

namespace JobTracker.Application.Interfaces
{
    public interface IJobApplicationService
    {
        Task<JobApplicationResponseDto> CreateAsync(Guid userId, JobApplicationCreateDto dto);
        Task<List<JobApplicationResponseDto>> GetAllByUserAsync(Guid userId);
        Task<JobApplicationResponseDto?> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, JobApplicationUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}
