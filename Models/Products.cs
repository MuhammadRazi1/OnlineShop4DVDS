using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Products
    {
        [Key]
        public int Product_Id { get; set; }

        public string Product_Title { get; set; }

        public string Product_Description { get; set; }

        public int Category_Id { get; set; }
        
        public string Artist_Id { get; set; }

        public DateTime  Release_Date { get; set; }

        public int Price { get; set; }

        public int Product_StockQuantity { get; set; }
        public string Product_Cover_Image { get; set; }

        public string Product_Type { get; set; }

        public string Product_Ratings {  get; set; }




    }
}
