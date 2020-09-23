namespace Hiromi.Services.Tracker
{
    public class TrackerStatistics
    {
        public ProgressActivity ProgressActivity { get; }
        public ArchiveActivity ArchiveActivity { get; }
        public ClaimDistribution ClaimDistribution { get; }

        public TrackerStatistics(ProgressActivity progressActivity, ArchiveActivity archiveActivity, ClaimDistribution claimDistribution)
        {
            ProgressActivity = progressActivity;
            ArchiveActivity = archiveActivity;
            ClaimDistribution = claimDistribution;
        }
        
        public int TotalWrites => ProgressActivity.Writes + ArchiveActivity.Writes;
        public int TotalEdits => ProgressActivity.Edits + ArchiveActivity.Edits + ArchiveActivity.CoordinatorEdits;
    }
}