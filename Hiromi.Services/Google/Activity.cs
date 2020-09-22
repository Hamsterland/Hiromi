namespace Hiromi.Services.Google
{
    public class Activity
    {
        public int ProgressWrites { get; }
        public int ProgressEdits { get; }
        public int ArchiveWrites { get; }
        public int ArchiveEdits { get;}
        public int TotalWrites => ProgressWrites + ArchiveWrites;
        public int TotalEdits => ProgressEdits + ArchiveEdits;

        public Activity(int progressWrites, int progressEdits, int archiveWrites, int archiveEdits)
        {
            ProgressWrites = progressWrites;
            ProgressEdits = progressEdits;
            ArchiveWrites = archiveWrites;
            ArchiveEdits = archiveEdits;
        }
    }
}