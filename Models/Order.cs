using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Order
    {
        [Key]
        public int Order_Id { get; set; }

        public int User_Id { get; set; }
    
        public DateTime Order_Date { get; set; }

        public int Total_Amount { get; set; }

        public string Order_Status { get; set; }

    }
}
