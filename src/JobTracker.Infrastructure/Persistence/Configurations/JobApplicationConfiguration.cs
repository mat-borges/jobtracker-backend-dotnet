using JobTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace JobTracker.Infrastructure.Persistence.Configurations
{
    public class JobApplicationConfiguration : IEntityTypeConfiguration<JobApplication>
    {
        public void Configure(EntityTypeBuilder<JobApplication> builder)
        {
            builder.ToTable("applications");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();

            // DateOnly <-> Date mapping
            var dateOnlyConverter = new ValueConverter<DateOnly, DateTime>(
                d => d.ToDateTime(TimeOnly.MinValue),
                dt => DateOnly.FromDateTime(dt));

            builder.Property(p => p.ApplicationDate)
                   .HasConversion(dateOnlyConverter)
                   .HasColumnType("date")
                   .IsRequired();

            builder.Property(p => p.CompanyName).IsRequired().HasMaxLength(256);
            builder.Property(p => p.JobTitle).IsRequired().HasMaxLength(256);

            // map enums as strings
            builder.Property(p => p.ContractType)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(p => p.WorkStyle)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(p => p.CurrentStage)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(p => p.Status)
                   .HasConversion<string>()
                   .IsRequired();

            builder.Property(p => p.WorkLocationState).IsRequired().HasMaxLength(64);
            builder.Property(p => p.WorkLocationCity).HasMaxLength(128);
            builder.Property(p => p.WorkLocationCountry).HasMaxLength(128);

            builder.Property(p => p.JobOfferUrl).HasMaxLength(2048);
            builder.Property(p => p.Source).HasMaxLength(128);
            builder.Property(p => p.Notes).HasColumnType("text");
            builder.Property(p => p.SalaryExpectation).HasColumnType("numeric(12,2)");

            // relationships
            builder.HasOne<User>()
                   .WithMany(u => u.JobApplications)
                   .HasForeignKey("UserId")
                   .OnDelete(DeleteBehavior.Cascade);

            // indexes
            builder.HasIndex(p => new { p.UserId, p.ApplicationDate });
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.Source);
        }
    }
}
