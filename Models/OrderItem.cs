using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; } // Primary Key

        [ForeignKey("Order")]
        public int OrderId { get; set; } // Foreign Key to Orders table
        public Order Order { get; set; } // Navigation Property

        public string ItemType { get; set; } // "Game", "Movie", or "Album"
        public int ItemId { get; set; } // ID of the purchased item

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
