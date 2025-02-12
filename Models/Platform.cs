using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Platform
    {
        [Key]
        public int PlatformId { get; set; }

        [Required(ErrorMessage = "Platform name is required")]
        [StringLength(50, ErrorMessage = "Platform name must be less than 50 characters.")]
        public string PlatformName { get; set; }

        public ICollection<GamePlatform> GamePlatforms { get; set; } = new List<GamePlatform>();
    }
}
