using CarparkInfo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarparkInfo.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<CarPark> CarParks => Set<CarPark>();
    public DbSet<CarParkType> CarParkTypes => Set<CarParkType>();
    public DbSet<ParkingSystem> ParkingSystems => Set<ParkingSystem>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserFavourite> UserFavourites => Set<UserFavourite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
