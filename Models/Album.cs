using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Album
    {
        [Key]
        public int Album_Id { get; set; }
    
        public int Album_Title { get; set; }

        public int Artist_Id { get; set; }

        public DateTime Album_Release_Date { get;set; }

        public string Album_Genre { get; set; }

        public string Album_Cover_Image { get;set; }




    
    }
}
