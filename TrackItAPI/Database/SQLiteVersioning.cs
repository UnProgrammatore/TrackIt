using System.Text;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using TrackItAPI.Configuration;

namespace TrackItAPI.Database;
public class SQLiteVersioning
{
    public record MigrationEntry(long Id, string MigrationName, string Hash, string AppliedDate, string? Content = null);
    private readonly IOptions<DatabaseConfiguration> _options;
    private string _databaseFile => _options.Value.FileName ?? throw new InvalidOperationException("Database file name not set.");

    private readonly ILogger<SQLiteVersioning> _logger;

    public SQLiteVersioning(IOptions<DatabaseConfiguration> options, ILogger<SQLiteVersioning> logger)
    {
        _options = options;
        _logger = logger;
    }

    private SqliteConnection GetConnection()
        => new SqliteConnection($"Data Source={_databaseFile};");
    public async Task UpdateDatabaseAsync() 
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        using var conn = GetConnection();
        conn.Execute(@"
CREATE TABLE IF NOT EXISTS Migrations 
(
    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    MigrationName TEXT NOT NULL,
    Hash TEXT NOT NULL,
    AppliedDate TEXT NOT NULL
)");
        // The hash as content is a workaround because I can't be bothered to do things properly for dapper
        var appliedMigrations = await conn.QueryAsync<MigrationEntry>("SELECT Id, MigrationName, Hash, AppliedDate, Hash As Content FROM Migrations ORDER BY Id");
        var allMigrations = Directory.GetFiles("./Database/Migrations", "*.sql")
            .Select(a => new { FileName = Path.GetFileName(a), Content = File.ReadAllText(a) })
            .Select(a => new MigrationEntry(
                0, 
                a.FileName, 
                Convert.ToBase64String(md5.ComputeHash(Encoding.UTF8.GetBytes(a.Content))), 
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                a.Content
            ))
            .ToDictionary(a => a.MigrationName, a => a);

        foreach(var appliedMigration in appliedMigrations) {
            if(allMigrations.ContainsKey(appliedMigration.MigrationName)) {
                var migration = allMigrations[appliedMigration.MigrationName];
                allMigrations.Remove(appliedMigration.MigrationName);
                if(migration.Hash != appliedMigration.Hash) {
                    _logger.LogCritical($"Migration {appliedMigration.MigrationName} has been modified since it was applied. Please revert it and reapply it.");
                }
            }
        }

        foreach(var migration in allMigrations.OrderBy(a => a.Key)) {
            _logger.LogInformation($"Applying migration {migration.Key}");
            await conn.ExecuteAsync(migration.Value.Content);
            await conn.ExecuteAsync("INSERT INTO Migrations (MigrationName, Hash, AppliedDate) VALUES (@MigrationName, @Hash, @AppliedDate)", migration.Value);
        }
    }
}