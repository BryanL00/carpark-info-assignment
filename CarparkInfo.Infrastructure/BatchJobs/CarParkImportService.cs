using System.Globalization;
using CarparkInfo.Domain.Entities;
using CarparkInfo.Domain.Services;
using CarparkInfo.Infrastructure.Data;
using CsvHelper;
using Microsoft.EntityFrameworkCore;

namespace CarparkInfo.Infrastructure.BatchJobs;

public class CarParkImportService : ICarParkImportService
{
    private readonly AppDbContext _db;

    public CarParkImportService(AppDbContext db) => _db = db;

    public async Task ImportAsync(Stream csvStream)
    {
        // Parse the entire file before touching the DB.
        // Any malformed row throws here, leaving the DB untouched.
        var rows = ParseCsv(csvStream);

        await using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            var typeMap = await UpsertLookupAsync<CarParkType>(
                _db.CarParkTypes,
                rows.Select(r => r.CarParkType).Distinct());

            var systemMap = await UpsertLookupAsync<ParkingSystem>(
                _db.ParkingSystems,
                rows.Select(r => r.TypeOfParkingSystem).Distinct());

            await _db.Database.ExecuteSqlRawAsync("DELETE FROM CarParks");

            var carParks = rows.Select(r => new CarPark
            {
                CarParkNo = r.CarParkNo,
                Address = r.Address,
                XCoord = r.XCoord,
                YCoord = r.YCoord,
                CarParkTypeId = typeMap[r.CarParkType],
                ParkingSystemId = systemMap[r.TypeOfParkingSystem],
                ShortTermParking = r.ShortTermParking,
                FreeParking = r.FreeParking,
                NightParking = r.NightParking.Equals("YES", StringComparison.OrdinalIgnoreCase),
                CarParkDecks = r.CarParkDecks,
                GantryHeight = r.GantryHeight,
                IsBasement = r.CarParkBasement.Equals("Y", StringComparison.OrdinalIgnoreCase)
            }).ToList();

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

    private static List<CarParkCsvRow> ParseCsv(Stream stream)
    {
        using var reader = new StreamReader(stream);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.RegisterClassMap<CarParkCsvMap>();
        return csv.GetRecords<CarParkCsvRow>().ToList();
    }

    // Ensures all lookup values exist in the DB and returns a name→id map.
    private async Task<Dictionary<string, int>> UpsertLookupAsync<T>(
        DbSet<T> dbSet, IEnumerable<string> names) where T : class, ILookupEntity, new()
    {
        var existing = await dbSet.ToDictionaryAsync(e => e.Name, e => e.Id);
        var result = new Dictionary<string, int>(existing);

        foreach (var name in names.Where(n => !existing.ContainsKey(n)))
        {
            var entity = new T { Name = name };
            dbSet.Add(entity);
            await _db.SaveChangesAsync();
            result[name] = entity.Id;
        }

        return result;
    }
}
