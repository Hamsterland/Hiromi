using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4.Data;

namespace Hiromi.Services.Tracker
{
    public interface ITrackerService
    {
        Task<List<Synopsis>> GetSynopsesAsync(string query);
        Task<List<Synopsis>> GetUserSynopsesAsync(string username);
        Task<Spreadsheet> GetTrackerAsync();
        
    }
}