using LeaveManager.Api.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LeaveManager.Api.Tests;

/// <summary>
/// Creates an isolated in-memory SQLite database per test, already seeded with the demo data
/// (alice@demo, bob@demo with 2 annual days, manager@demo, holidays, etc.).
/// Dispose the factory at the end of the test to drop the database.
/// </summary>
/// <example>
/// using var factory = new TestDbFactory();
/// await using var db = factory.CreateContext();
/// var service = new LeaveRequestService(db, NullLogger&lt;LeaveRequestService&gt;.Instance);
/// </example>
public sealed class TestDbFactory : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<LeaveDbContext> _options;

    public TestDbFactory(bool seed = true)
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<LeaveDbContext>()
            .UseSqlite(_connection)
            .Options;

        using var db = new LeaveDbContext(_options);
        db.Database.EnsureCreated();
        if (seed)
        {
            SeedData.Apply(db);
        }
    }

    public LeaveDbContext CreateContext() => new(_options);

    public void Dispose() => _connection.Dispose();
}
