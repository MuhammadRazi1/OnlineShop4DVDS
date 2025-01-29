using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Game
    {
        [Key]
        public int Game_Id { get; set; }

        public string Game_Title { get; set; }

        public string Game_Genre { get; set; }

        public DateTime Game_ReleaseDate { get; set; }

        public string Game_Trailerpath { get; set; }

        public string Game_Filepath { get; set; }

        public Boolean Game_Isfreedownload { get; set; }


    }
}
