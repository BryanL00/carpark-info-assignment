using CarparkInfo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarparkInfo.Infrastructure.Data.Configurations;

public class CarParkConfiguration : IEntityTypeConfiguration<CarPark>
{
    public void Configure(EntityTypeBuilder<CarPark> builder)
    {
        builder.HasKey(x => x.CarParkNo);
        builder.Property(x => x.CarParkNo).HasMaxLength(20);
        builder.Property(x => x.Address).IsRequired().HasMaxLength(300);
        builder.Property(x => x.XCoord).HasColumnType("decimal(12,4)");
        builder.Property(x => x.YCoord).HasColumnType("decimal(12,4)");
        builder.Property(x => x.FreeParking).IsRequired().HasMaxLength(100);
        builder.Property(x => x.ShortTermParking).IsRequired().HasMaxLength(50);
        builder.Property(x => x.GantryHeight).HasColumnType("decimal(5,2)");

        builder.HasOne(x => x.CarParkType)
            .WithMany(t => t.CarParks)
            .HasForeignKey(x => x.CarParkTypeId);

        builder.HasOne(x => x.ParkingSystem)
            .WithMany(p => p.CarParks)
            .HasForeignKey(x => x.ParkingSystemId);

        // Index to speed up the filter queries
        builder.HasIndex(x => x.NightParking);
        builder.HasIndex(x => x.GantryHeight);
        builder.HasIndex(x => x.FreeParking);
    }
}
