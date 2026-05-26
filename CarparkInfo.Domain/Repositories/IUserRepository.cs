using CarparkInfo.Domain.Entities;

namespace CarparkInfo.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId);
    Task<User> GetOrCreateAsync(int userId);
}
