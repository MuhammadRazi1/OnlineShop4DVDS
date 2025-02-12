using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Developer
    {
        [Key]
        public int DeveloperId { get; set; }

        [Required(ErrorMessage = "Developer name is required")]
        [StringLength(100, ErrorMessage = "Developer name must be less than 100 characters.")]
        public string DeveloperName { get; set; }

        public ICollection<Game> Games { get; set; } = new List<Game>();
    }
}
