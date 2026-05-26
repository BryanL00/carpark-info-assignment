using CarparkInfo.Domain.Entities;

namespace CarparkInfo.Domain.Repositories;

public interface IUserFavouriteRepository
{
    Task<IEnumerable<UserFavourite>> GetByUserIdAsync(int userId);
    Task<bool> ExistsAsync(int userId, string carParkNo);
    Task AddAsync(UserFavourite favourite);
    Task RemoveAsync(int userId, string carParkNo);
}
