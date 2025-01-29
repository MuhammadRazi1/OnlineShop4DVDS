using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Supplier
    {
        [Key]

        public int Supplier_Id { get; set; }
        
        public string Supplier_Name { get; set; }

        public string Supplier_Contactinfo { get; set; }


        public string Supplier_Address { get; set; }    




    }
}
