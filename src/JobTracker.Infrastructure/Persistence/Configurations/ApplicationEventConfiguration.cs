using JobTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JobTracker.Infrastructure.Persistence.Configurations
{
    public class ApplicationEventConfiguration : IEntityTypeConfiguration<ApplicationEvent>
    {
        public void Configure(EntityTypeBuilder<ApplicationEvent> builder)
        {
            builder.ToTable("application_events");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            builder.Property(x => x.Note).HasColumnType("text");

            builder.Property(x => x.PreviousStage).HasConversion<string>();
            builder.Property(x => x.NewStage).HasConversion<string>();
            builder.Property(x => x.PreviousStatus).HasConversion<string>();
            builder.Property(x => x.NewStatus).HasConversion<string>();

            builder.HasIndex(x => x.ApplicationId);
        }
    }
}
