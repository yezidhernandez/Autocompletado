using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PiedraAzul.Domain.Entities.Config;

namespace PiedraAzul.Infrastructure.Persistence.Configurations;

public class DoctorScheduleConfigConfig : IEntityTypeConfiguration<DoctorScheduleConfig>
{
    public void Configure(EntityTypeBuilder<DoctorScheduleConfig> builder)
    {
        builder.ToTable("DoctorScheduleConfigs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.DoctorId)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(x => x.SlotIntervalMinutes)
            .IsRequired();

        builder.HasIndex(x => x.DoctorId)
            .IsUnique();

        builder.Navigation("_days")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(x => x.Days)
            .WithOne()
            .HasForeignKey(x => x.DoctorScheduleConfigId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
