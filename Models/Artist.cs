using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Artist
    {
        [Key]
        public int Artist_Id { get; set; }

        public string Artist_Name { get; set; }

        public string Artist_Bio { get; set; }

        public string Artist_Genre { get; set; }

        public string Artist_Profile_Picture { get; set; }

    }
}
