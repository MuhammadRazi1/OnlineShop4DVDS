namespace OnlineShop4DVDS.Models
{
    public class SearchResultsViewModel
    {
        public List<Movie> Movies { get; set; } = new List<Movie>();
        public List<Album> Albums { get; set; } = new List<Album>();
        public List<Song> Songs { get; set; } = new List<Song>();
        public List<Game> Games { get; set; } = new List<Game>();
    }
}
