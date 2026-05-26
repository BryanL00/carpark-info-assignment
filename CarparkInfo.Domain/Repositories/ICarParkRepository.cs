using CarparkInfo.Domain.Entities;
using CarparkInfo.Domain.Models;

namespace CarparkInfo.Domain.Repositories;

public interface ICarParkRepository
{
    Task<IEnumerable<CarPark>> GetAllAsync(CarParkFilter? filter = null);
    Task<CarPark?> GetByIdAsync(string carParkNo);
    Task BulkReplaceAsync(IEnumerable<CarPark> carParks);
}
