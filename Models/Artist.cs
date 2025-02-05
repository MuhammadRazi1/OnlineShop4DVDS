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

        [Required]
        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
        public int ArtistAge { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ArtistDateOfBirth { get; set; }

        [Required]
        public string ArtistBio { get; set; }

        public string? ArtistImage { get; set; }

        [ForeignKey("ArtistRoleId")]
        public virtual ArtistRole? ArtistRole { get; set; }
    }
}
