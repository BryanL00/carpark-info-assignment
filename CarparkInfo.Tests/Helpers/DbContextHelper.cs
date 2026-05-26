using CarparkInfo.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CarparkInfo.Tests.Helpers;

public static class DbContextHelper
{
    /// <summary>
    /// Creates a real in-memory SQLite DbContext with the full schema applied.
    /// Supports raw SQL and transactions — required for import service tests.
    /// The caller is responsible for disposing both the context and the connection.
    /// </summary>
    public static (AppDbContext db, SqliteConnection connection) CreateSqliteInMemory()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var db = new AppDbContext(options);
        db.Database.EnsureCreated();
        return (db, connection);
    }
}
