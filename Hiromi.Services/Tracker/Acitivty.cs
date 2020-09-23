namespace Hiromi.Services.Tracker
{
    public class Acitivty
    {
        public int Writes { get; }
        public int Edits { get; }

        protected Acitivty(int writes, int edits)
        {
            Writes = writes;
            Edits = edits;
        }
    }
}