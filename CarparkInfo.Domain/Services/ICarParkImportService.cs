namespace CarparkInfo.Domain.Services;

public interface ICarParkImportService
{
    /// <summary>
    /// Parses the CSV stream and atomically replaces all carpark records.
    /// If any row fails to parse or persist, the entire operation is rolled back.
    /// </summary>
    Task ImportAsync(Stream csvStream);
}
