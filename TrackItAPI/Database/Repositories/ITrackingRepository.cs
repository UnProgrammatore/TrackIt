using TrackItCommon;

namespace TrackItAPI.Database.Repositories;

public interface ITrackingRepository {
    Task<IEnumerable<Tracker>> GetTrackersAsync();
    Task<Tracker> GetTrackerAsync(string code);
    Task AddTrackerAsync(Tracker tracker);
}