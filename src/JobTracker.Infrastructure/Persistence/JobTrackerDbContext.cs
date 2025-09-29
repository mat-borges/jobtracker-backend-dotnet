using JobTracker.Domain.Common;
using JobTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace JobTracker.Infrastructure.Persistence
{
    public class JobTrackerDbContext(DbContextOptions<JobTrackerDbContext> options) : DbContext(options)
    {
		public DbSet<User> Users => Set<User>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<ApplicationEvent> ApplicationEvents => Set<ApplicationEvent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobTrackerDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is AuditableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var e in entries)
            {
                var entity = (AuditableEntity)e.Entity;
                if (e.State == EntityState.Added) entity.SetCreatedNow();
                entity.SetUpdatedNow();
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
