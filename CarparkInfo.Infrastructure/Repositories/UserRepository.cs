using CarparkInfo.Domain.Entities;
using CarparkInfo.Domain.Repositories;
using CarparkInfo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CarparkInfo.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public async Task<User?> GetByIdAsync(Guid userId) =>
        await _db.Users.FindAsync(userId);

    public async Task<User> GetOrCreateAsync(Guid userId)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user is not null) return user;

        user = new User { Id = userId, Username = userId.ToString() };
        _db.Users.Add(user);
        await _db.SaveChangesAsync();
        return user;
    }
}
