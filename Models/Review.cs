using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Review
    {
        [Key]
        public int Review_Id { get; set; }

        public string User_Id { get; set; }

       public int Product_Id { get; set; }

        public int Review_Ratings { get; set; }

        public string Review_Comments { get; set; }

        public DateTime Review_Datesubmitted    { get; set; }




    }
}
