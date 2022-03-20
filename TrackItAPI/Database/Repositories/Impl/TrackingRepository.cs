using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using TrackItAPI.Configuration;
using TrackItAPI.Database.Repositories;
using TrackItCommon;

namespace TrackItAPI.Database.Repositories.Impl;

public class TrackingRepository : ITrackingRepository
{
    private readonly IOptions<DatabaseConfiguration> _options;
    private string _databaseFile => _options.Value.FileName ?? throw new InvalidOperationException("Database file name not set.");

    private readonly ILogger<TrackingRepository> _logger;

    public TrackingRepository(IOptions<DatabaseConfiguration> options, ILogger<TrackingRepository> logger)
    {
        _options = options;
        _logger = logger;
    }

    private SqliteConnection GetConnection()
        => new SqliteConnection($"Data Source={_databaseFile};");

    public async Task<IEnumerable<Tracker>> GetTrackersAsync()
    {
        using var conn = GetConnection();
        return await conn.QueryAsync<Tracker>("SELECT Code, Name, Notes FROM Trackers");
    }

    public Task AddTrackerAsync(Tracker tracker)
    {
        if (tracker == null)
        {
            throw new ArgumentNullException(nameof(tracker));
        }

        using var conn = GetConnection();
        return conn.ExecuteAsync("INSERT INTO Trackers (Code, Name, Notes) VALUES (@Code, @Name, @Notes)", tracker);
    }

    public Task<Tracker> GetTrackerAsync(string code)
    {
        using var conn = GetConnection();
        return conn.QueryFirstOrDefaultAsync<Tracker>("SELECT Code, Name, Notes FROM Trackers WHERE Code = @Code", new { Code = code });
    }
}
