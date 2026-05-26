using CarparkInfo.Domain.Entities;

namespace CarparkInfo.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid userId);
    Task<User> GetOrCreateAsync(Guid userId);
}
