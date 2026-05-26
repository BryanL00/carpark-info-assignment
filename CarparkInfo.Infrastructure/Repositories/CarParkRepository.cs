using CarparkInfo.Domain.Entities;
using CarparkInfo.Domain.Models;
using CarparkInfo.Domain.Repositories;
using CarparkInfo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarparkInfo.Infrastructure.Repositories;

public class CarParkRepository : ICarParkRepository
{
    private readonly AppDbContext _db;

    public CarParkRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<CarPark>> GetAllAsync(CarParkFilter? filter = null)
    {
        var query = _db.CarParks
            .Include(c => c.CarParkType)
            .Include(c => c.ParkingSystem)
            .AsQueryable();

        if (filter is not null)
        {
            if (filter.FreeParking == true)
                query = query.Where(c => c.FreeParking != "NO");

            if (filter.NightParking == true)
                query = query.Where(c => c.NightParking);

            if (filter.VehicleHeight.HasValue)
                query = query.Where(c => c.GantryHeight >= filter.VehicleHeight.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<CarPark?> GetByIdAsync(string carParkNo) =>
        await _db.CarParks
            .Include(c => c.CarParkType)
            .Include(c => c.ParkingSystem)
            .FirstOrDefaultAsync(c => c.CarParkNo == carParkNo);

    public async Task BulkReplaceAsync(IEnumerable<CarPark> carParks)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            await _db.Database.ExecuteSqlRawAsync("DELETE FROM CarParks");
            await _db.CarParks.AddRangeAsync(carParks);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
