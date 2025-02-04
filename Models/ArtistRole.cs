using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class ArtistRole
    {
        [Key]
        public int ArtistRoleId { get; set; }

        [Required]
        public string ArtistRoleName { get; set; }
    }
}
