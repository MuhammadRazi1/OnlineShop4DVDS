using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop4DVDS.Models
{
    public class Song
    {
        [Key]
        public int SongId { get; set; }

        [Required(ErrorMessage = "Category Id is required")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Album Id is required")]
        public int AlbumId { get; set; }

        [Required(ErrorMessage = "Song name is required")]
        [StringLength(100, ErrorMessage = "Title must be less than 100 characters")]
        public string SongName { get; set; }

        public string? SongFilePath { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        [ForeignKey("AlbumId")]
        public virtual Album? Album { get; set; }

        public ICollection<UserSong> UserSongs { get; set; }
    }
}
