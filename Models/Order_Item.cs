using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Order_Item
    {
        [Key]
        public int Orderitem_Id { get; set; }

        public int Order_Id { get; set; }

        public int Product_Id { get; set; }

        public string Orderitem_Quantity { get; set; }

        public int Orderitem_Price { get; set; }


    }
}
