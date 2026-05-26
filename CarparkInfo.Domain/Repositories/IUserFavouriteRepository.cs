using CarparkInfo.Domain.Entities;

namespace CarparkInfo.Domain.Repositories;

public interface IUserFavouriteRepository
{
    Task<IEnumerable<UserFavourite>> GetByUserIdAsync(Guid userId);
    Task<bool> ExistsAsync(Guid userId, string carParkNo);
    Task AddAsync(UserFavourite favourite);
    Task RemoveAsync(Guid userId, string carParkNo);
}
