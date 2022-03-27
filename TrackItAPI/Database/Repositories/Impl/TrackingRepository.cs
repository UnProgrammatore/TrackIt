using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using TrackItAPI.Configuration;
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

    public Task<IEnumerable<Tracker>> GetTrackersAsync()
    {
        using var conn = GetConnection();
        return conn.QueryAsync<Tracker>("SELECT Code, Name, Notes FROM Trackers");
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

    public Task AddPositionAsync(string trackerCode, Position position)
    {
        using var conn = GetConnection();
        return conn.ExecuteAsync("INSERT INTO Positions (TrackerId, Latitude, Longitude, CollectDate) SELECT Id, @Latitude, @Longitude, @CollectDate FROM Trackers WHERE Code = @trackerCode", 
            new { trackerCode, position.Latitude, position.Longitude, position.CollectDate }
        );
    }

    public Task<IEnumerable<Position>> GetLastPositionsAsync(string trackerCode, int count, int offset, DateTime from) 
    {
        using var conn = GetConnection();
        return conn.QueryAsync<Position>("SELECT Latitude, Longitude, CollectDate FROM Positions WHERE TrackerId = (SELECT TrackerID FROM Trackers WHERE Code = @trackerCode) AND CollectDate <= @from ORDER BY CollectDate DESC LIMIT @count OFFSET @offset",
            new { trackerCode, from, count, offset }
        );
    }

    public Task<IEnumerable<Position>> GetNewPositionsAsync(string trackerCode, DateTime from)
    {
        using var conn = GetConnection();
        return conn.QueryAsync<Position>("SELECT Latitude, Longitude, CollectDate FROM Positions WHERE TrackerId = (SELECT TrackerID FROM Trackers WHERE Code = @trackerCode) AND CollectDate > @from ORDER BY CollectDate DESC",
            new { trackerCode, from }
        );
    }

    public Task<IEnumerable<Position>> GetFullDayPositionsAsync(string trackerCode, DateTime day)
    {
        day = day.Date;
        var nextDay = day.AddDays(1);
        using var conn = GetConnection();
        return conn.QueryAsync<Position>("SELECT Latitude, Longitude, CollectDate FROM Positions WHERE TrackerId = (SELECT TrackerID FROM Trackers WHERE Code = @trackerCode) AND CollectDate >= @day AND CollectDate < @nextDay ORDER BY CollectDate DESC",
            new { trackerCode, day, nextDay }
        );
    }
}
