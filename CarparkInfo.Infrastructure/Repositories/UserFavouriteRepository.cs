using CarparkInfo.Domain.Entities;
using CarparkInfo.Domain.Repositories;
using CarparkInfo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarparkInfo.Infrastructure.Repositories;

public class UserFavouriteRepository : IUserFavouriteRepository
{
    private readonly AppDbContext _db;

    public UserFavouriteRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<UserFavourite>> GetByUserIdAsync(int userId) =>
        await _db.UserFavourites
            .Include(f => f.CarPark)
                .ThenInclude(c => c.CarParkType)
            .Include(f => f.CarPark)
                .ThenInclude(c => c.ParkingSystem)
            .Where(f => f.UserId == userId)
            .ToListAsync();

    public async Task<bool> ExistsAsync(int userId, string carParkNo) =>
        await _db.UserFavourites.AnyAsync(f => f.UserId == userId && f.CarParkNo == carParkNo);

    public async Task AddAsync(UserFavourite favourite)
    {
        _db.UserFavourites.Add(favourite);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(int userId, string carParkNo)
    {
        var favourite = await _db.UserFavourites
            .FirstOrDefaultAsync(f => f.UserId == userId && f.CarParkNo == carParkNo);
        if (favourite is not null)
        {
            _db.UserFavourites.Remove(favourite);
            await _db.SaveChangesAsync();
        }
    }
}
