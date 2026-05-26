using System.Text;
using CarparkInfo.Infrastructure.BatchJobs;
using CarparkInfo.Tests.Helpers;
using Xunit;

namespace CarparkInfo.Tests.Services;

public class CarParkImportServiceTests : IDisposable
{
    private readonly Microsoft.Data.Sqlite.SqliteConnection _connection;
    private readonly Infrastructure.Data.AppDbContext _db;
    private readonly CarParkImportService _service;

    public CarParkImportServiceTests()
    {
        (_db, _connection) = DbContextHelper.CreateSqliteInMemory();
        _service = new CarParkImportService(_db);
    }

    public void Dispose()
    {
        _db.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task ImportAsync_ValidCsv_InsertsAllRows()
    {
        var csv = BuildCsv(
            ("ACB", "ALBERT CENTRE", "30314.79", "31490.49", "BASEMENT CAR PARK", "ELECTRONIC PARKING", "WHOLE DAY", "NO", "YES", "1", "1.80", "Y"),
            ("ACM", "ALJUNIED CRESCENT",  "33758.41", "33695.51", "MULTI-STOREY CAR PARK", "ELECTRONIC PARKING", "WHOLE DAY", "SUN & PH FR 7AM-10.30PM", "YES", "5", "2.10", "N")
        );

        await _service.ImportAsync(csv);

        Assert.Equal(2, _db.CarParks.Count());
        Assert.Equal(2, _db.CarParkTypes.Count());   // BASEMENT and MULTI-STOREY
        Assert.Equal(1, _db.ParkingSystems.Count()); // both share ELECTRONIC PARKING
    }

    [Fact]
    public async Task ImportAsync_CalledTwice_ReplacesNotDuplicates()
    {
        var csv1 = BuildCsv(("ACB", "ALBERT CENTRE", "30314.79", "31490.49", "BASEMENT CAR PARK", "ELECTRONIC PARKING", "WHOLE DAY", "NO", "YES", "1", "1.80", "Y"));
        var csv2 = BuildCsv(("XYZ", "SOME OTHER PARK", "10000.00", "20000.00", "SURFACE CAR PARK", "COUPON PARKING", "7AM-7PM", "NO", "NO", "0", "0.00", "N"));

        await _service.ImportAsync(csv1);
        await _service.ImportAsync(csv2);

        Assert.Equal(1, _db.CarParks.Count());
        Assert.Equal("XYZ", _db.CarParks.Single().CarParkNo);
    }

    [Fact]
    public async Task ImportAsync_MalformedCsv_ThrowsAndPreservesExistingData()
    {
        // Seed one valid row first
        var validCsv = BuildCsv(("ACB", "ALBERT CENTRE", "30314.79", "31490.49", "BASEMENT CAR PARK", "ELECTRONIC PARKING", "WHOLE DAY", "NO", "YES", "1", "1.80", "Y"));
        await _service.ImportAsync(validCsv);

        // Attempt import with a row that has a non-numeric value in a numeric field
        var badCsv = BuildCsv(("BAD", "BAD PARK", "NOT_A_NUMBER", "31490.49", "SURFACE CAR PARK", "COUPON PARKING", "WHOLE DAY", "NO", "YES", "0", "0.00", "N"));

        await Assert.ThrowsAnyAsync<Exception>(() => _service.ImportAsync(badCsv));

        // Original data must still be intact
        Assert.Equal(1, _db.CarParks.Count());
        Assert.Equal("ACB", _db.CarParks.Single().CarParkNo);
    }

    [Fact]
    public async Task ImportAsync_NightParkingYes_MapsToTrue()
    {
        var csv = BuildCsv(("ACB", "ALBERT CENTRE", "30314.79", "31490.49", "BASEMENT CAR PARK", "ELECTRONIC PARKING", "WHOLE DAY", "NO", "YES", "1", "1.80", "Y"));

        await _service.ImportAsync(csv);

        Assert.True(_db.CarParks.Single().NightParking);
        Assert.True(_db.CarParks.Single().IsBasement);
    }

    [Fact]
    public async Task ImportAsync_NightParkingNo_MapsToFalse()
    {
        var csv = BuildCsv(("AK19", "ANG MO KIO ST 21", "28185.43", "39012.66", "SURFACE CAR PARK", "COUPON PARKING", "7AM-7PM", "NO", "NO", "0", "0.00", "N"));

        await _service.ImportAsync(csv);

        Assert.False(_db.CarParks.Single().NightParking);
        Assert.False(_db.CarParks.Single().IsBasement);
    }

    // Builds a CSV stream with a header row followed by the supplied data rows.
    private static Stream BuildCsv(params (string no, string addr, string x, string y, string type, string system, string shortTerm, string free, string night, string decks, string height, string basement)[] rows)
    {
        var sb = new StringBuilder();
        sb.AppendLine("\"car_park_no\",\"address\",\"x_coord\",\"y_coord\",\"car_park_type\",\"type_of_parking_system\",\"short_term_parking\",\"free_parking\",\"night_parking\",\"car_park_decks\",\"gantry_height\",\"car_park_basement\"");
        foreach (var r in rows)
            sb.AppendLine($"\"{r.no}\",\"{r.addr}\",\"{r.x}\",\"{r.y}\",\"{r.type}\",\"{r.system}\",\"{r.shortTerm}\",\"{r.free}\",\"{r.night}\",\"{r.decks}\",\"{r.height}\",\"{r.basement}\"");
        return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
    }
}
