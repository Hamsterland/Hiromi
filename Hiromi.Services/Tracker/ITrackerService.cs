using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Apis.Sheets.v4.Data;

namespace Hiromi.Services.Tracker
{
    public interface ITrackerService
    {
        Task<List<Synopsis>> GetUserSynopses(string username);
        Task<Spreadsheet> GetTrackerAsync();
        
    }
}