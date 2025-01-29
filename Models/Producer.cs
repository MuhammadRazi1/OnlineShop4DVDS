using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Producer
    {
        [Key]
        public int Producer_Id {  get; set; }

        public string Producer_Name { get; set; }

        public string Producer_Contactinfo  { get; set; }

        public string Producer_Address { get; set; }



    }
}
