using CarparkInfo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarparkInfo.Infrastructure.Data.Configurations;

public class UserFavouriteConfiguration : IEntityTypeConfiguration<UserFavourite>
{
    public void Configure(EntityTypeBuilder<UserFavourite> builder)
    {
        builder.HasKey(x => new { x.UserId, x.CarParkNo });

        builder.HasOne(x => x.User)
            .WithMany(u => u.Favourites)
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.CarPark)
            .WithMany(c => c.FavouritedBy)
            .HasForeignKey(x => x.CarParkNo);

        builder.Property(x => x.CreatedAt).IsRequired();
    }
}
