using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Movie
    {
        [Key]
        public  int Movie_Id { get; set; }

        public string Movie_Title { get; set; }

        public string Movie_Genre { get; set; }

        public DateTime Movie_Releasedate { get; set; }

        public string Movie_Trailerpath { get; set; }

        public string Movie_Filepath { get; set; }

        public string Movie_Isfreedownload { get; set; }





    }
}
