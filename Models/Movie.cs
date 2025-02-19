using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Movie title is required")]
        public string MovieTitle { get; set; }

        [Required(ErrorMessage = "Movie description is required")]
        public string MovieDescription { get; set; }

        [Required(ErrorMessage = "Release date is required")]
        [DataType(DataType.Date)]
        public DateTime MovieReleaseDate { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(100, 100000, ErrorMessage = "Price must be between 100 and 100000")]
        public decimal MoviePrice { get; set; }

        public string? MovieFilePath { get; set; }

        public string? MovieImage { get; set; }

        [Required(ErrorMessage = "Movie rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public decimal MovieRating { get; set; }

        public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
