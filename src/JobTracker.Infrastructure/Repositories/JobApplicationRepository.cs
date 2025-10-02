using JobTracker.Application.Interfaces;
using JobTracker.Domain.Entities;
using JobTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Repositories
{
    public class JobApplicationRepository(JobTrackerDbContext context) : IJobApplicationRepository
    {
        private readonly JobTrackerDbContext _context = context;

		public async Task AddAsync(JobApplication application)
        {
            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(JobApplication application)
        {
            _context.JobApplications.Remove(application);
            await _context.SaveChangesAsync();
        }

        public async Task<List<JobApplication>> GetAllByUserAsync(Guid userId)
        {
            return await _context.JobApplications
                .Where(j => j.UserId == userId)
                .ToListAsync();
        }

        public async Task<JobApplication?> GetByIdAsync(Guid id)
        {
            return await _context.JobApplications
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task UpdateAsync(JobApplication application)
        {
            _context.JobApplications.Update(application);
            await _context.SaveChangesAsync();
        }
    }
}
