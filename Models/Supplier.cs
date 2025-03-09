using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Supplier name is required")]
        public string SupplierName { get; set; }

        [Required(ErrorMessage = "Supplier email is required")]
        [DataType(DataType.EmailAddress)]
        public string SupplierEmail { get; set; }

        [Required(ErrorMessage = "Supplier contact is required")]
        public string SupplierPhone { get; set; }
    }
}
