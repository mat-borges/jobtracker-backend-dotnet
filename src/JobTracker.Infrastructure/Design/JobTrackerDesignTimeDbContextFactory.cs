using JobTracker.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobTracker.Infrastructure.Design
{
    public class JobTrackerDesignTimeDbContextFactory : IDesignTimeDbContextFactory<JobTrackerDbContext>
    {
        public JobTrackerDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<JobTrackerDbContext>();

            var conn = Environment.GetEnvironmentVariable("JOBTRACKER_CONNECTION")
                       ?? "Host=localhost;Database=jobtracker;Username=postgres;Password=naolembro123";

            builder.UseNpgsql(conn, o => o.EnableRetryOnFailure());

            return new JobTrackerDbContext(builder.Options);
        }
    }
}
