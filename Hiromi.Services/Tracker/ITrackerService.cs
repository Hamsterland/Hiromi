using System.Threading.Tasks;

namespace Hiromi.Services.Tracker
{
    public interface ITrackerService
    {
        ClaimDistribution GetClaimDistribution(Tracker tracker, string username);
        ProgressActivity GetProgressActivity(Tracker tracker, string username);
        ArchiveActivity GetArchiveActivity(Tracker tracker, string username);
        Task<TrackerStatistics> GetUserActivity(string username);
        Task<Tracker> GetTrackerAsync();
    }
}