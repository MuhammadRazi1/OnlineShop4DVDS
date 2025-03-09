using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop4DVDS.Models
{
    public class Artist
    {
        [Key]
        public int ArtistId { get; set; }

        [Required(ErrorMessage = "Artist role is required.")]
        public int ArtistRoleId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string ArtistName { get; set; }

        [ForeignKey("ArtistRoleId")]
        public virtual ArtistRole? ArtistRole { get; set; }

        public ICollection<Album> Albums { get; set; } = new List<Album>();
    }
}
