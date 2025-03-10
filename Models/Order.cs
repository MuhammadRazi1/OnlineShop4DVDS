using OnlineShop4DVDS.Models;

public class Order
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Phone { get; set; }
    public string Country { get; set; }
    public string City { get; set; }
    public string Address { get; set; }
    public bool Fulfilled { get; set; } // New field

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}

