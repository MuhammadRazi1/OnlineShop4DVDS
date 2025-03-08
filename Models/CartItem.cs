namespace OnlineShop4DVDS.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public string ItemType { get; set; } // "Game", "Movie", "Album"
        public int ItemId { get; set; } // GameId, MovieId, AlbumId
        public int? PlatformId { get; set; } // Only for games
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }

        public Cart Cart { get; set; }
    }
}
