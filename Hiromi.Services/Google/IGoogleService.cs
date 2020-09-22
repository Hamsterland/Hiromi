using System.Threading.Tasks;

namespace Hiromi.Services.Google
{
    public interface IGoogleService
    {
        Task<Activity> GetUserActivity(string user);
        Task<Tracker> GetTrackerAsync();
    }
}