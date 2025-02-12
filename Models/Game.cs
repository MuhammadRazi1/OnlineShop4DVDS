using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop4DVDS.Models
{
    public class Game
    {
        [Key]
        public int GameId { get; set; }

        [Required(ErrorMessage = "Developer Id is required")]
        public int DeveloperId { get; set; }

        [Required(ErrorMessage = "Game name is required")]
        public string GameName { get; set; }

        [Required(ErrorMessage = "Game description is required")]
        public string GameDescription { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(100, 100000, ErrorMessage = "Price must be between 100 and 100000")]
        public decimal GamePrice { get; set; }

        [Required(ErrorMessage = "Release date is required")]
        [DataType(DataType.Date)]
        public DateTime GameReleaseDate { get; set; }

        public string? GameFilePath { get; set; }

        public string? GameImage { get; set; }

        [ForeignKey("DeveloperId")]
        public virtual Developer? Developer { get; set; }

        public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();

        public ICollection<GamePlatform> GamePlatforms { get; set; } = new List<GamePlatform>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}
