using CarparkInfo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarparkInfo.Infrastructure.Data.Configurations;

public class ParkingSystemConfiguration : IEntityTypeConfiguration<ParkingSystem>
{
    public void Configure(EntityTypeBuilder<ParkingSystem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}
