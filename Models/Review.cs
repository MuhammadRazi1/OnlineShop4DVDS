using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop4DVDS.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        public int? AlbumId { get; set; }

        public int? GameId { get; set; }

        [Required(ErrorMessage = "User Id is required")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Review text is required")]
        public string ReviewText { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int ReviewRating { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("AlbumId")]
        public virtual Album? Album { get; set; }

        [ForeignKey("GameId")]
        public virtual Game? Game { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
