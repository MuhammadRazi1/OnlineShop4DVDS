using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Promotion
    {
        [Key]
        public int Promotion_Id { get; set; }

        public string Promotion_Description { get; set; }

        public DateTime Promotion_Startdate { get; set; }

        public DateTime Promotion_EndDate { get; set; }

        public int Promotion_Discountpercentage { get; set; }






    }
}
