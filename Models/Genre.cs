using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }

        [Required(ErrorMessage = "Genre name is required")]
        [StringLength(100, ErrorMessage = "Genre name must be less than 100 characters.")]
        public string GenreName { get; set; }

        public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
    }
}
