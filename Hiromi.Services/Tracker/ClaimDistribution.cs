namespace Hiromi.Services.Tracker
{
    public class ClaimDistribution
    {
        public int Anime { get; }
        public int Manga { get; }
        public int Novel { get; }

        public ClaimDistribution(int anime, int manga, int novel)
        {
            Anime = anime;
            Manga = manga;
            Novel = novel;
        }

        private int _total => Anime + Manga + Novel;

        public float AnimePercentage => (float) Anime / _total * 100; 
        public float MangaPercentage => (float) Manga / _total * 100; 
        public float NovelPercentage => (float) Novel / _total * 100;
    }
}