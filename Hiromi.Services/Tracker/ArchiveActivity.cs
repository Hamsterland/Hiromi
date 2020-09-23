namespace Hiromi.Services.Tracker
{
    public class ArchiveActivity : Acitivty
    {
        public int CoordinatorEdits { get; }
        
        public ArchiveActivity(int writes, int edits, int coordinatorEdits) : base(writes, edits)
        {
            CoordinatorEdits = coordinatorEdits;
        }
    }
}