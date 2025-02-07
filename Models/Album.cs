using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop4DVDS.Models
{
    public class Album
    {
        [Key]
        public int AlbumId { get; set; }

        [Required(ErrorMessage = "Artist Id is required")]
        public int ArtistId { get; set; }

        [Required(ErrorMessage = "Album title is required")]
        [StringLength(100, ErrorMessage = "Title must be less than 100 characters.")]
        public string AlbumTitle { get; set; }

        [Required(ErrorMessage = "Release date is required")]
        [DataType(DataType.Date)]
        public DateTime AlbumReleaseDate { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string AlbumDescription { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(100, 100000, ErrorMessage = "Price must be between 100 and 100000")]
        public decimal AlbumPrice { get; set; }

        [ForeignKey("ArtistId")]
        public virtual Artist? Artist { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
