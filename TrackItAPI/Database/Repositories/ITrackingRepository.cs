using TrackItCommon;

namespace TrackItAPI.Database.Repositories;

public interface ITrackingRepository {
    Task<IEnumerable<Tracker>> GetTrackersAsync();
    Task<Tracker> GetTrackerAsync(string code);
    Task AddTrackerAsync(Tracker tracker);
    Task AddPositionAsync(string trackerCode, Position position);
    Task<IEnumerable<Position>> GetLastPositionsAsync(string trackerCode, int count, int offset, DateTime from);
    Task<IEnumerable<Position>> GetNewPositionsAsync(string trackerCode, DateTime from);
    Task<IEnumerable<Position>> GetFullDayPositionsAsync(string trackerCode, DateTime day);
}