using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Category
    {
        [Key]
        public int Category_Id { get; set; }

        public string Category_Name { get; set; }

        public string Category_Description { get; set; }
    }
}
