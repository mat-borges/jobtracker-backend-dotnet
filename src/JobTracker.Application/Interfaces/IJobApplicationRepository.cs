using JobTracker.Domain.Entities;

namespace JobTracker.Application.Interfaces
{
    public interface IJobApplicationRepository
    {
        Task<JobApplication?> GetByIdAsync(Guid id);
        Task<List<JobApplication>> GetAllByUserAsync(Guid userId);
        Task AddAsync(JobApplication application);
        Task UpdateAsync(JobApplication application);
        Task DeleteAsync(JobApplication application);
    }
}
