using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Producer
    {
        [Key]
        public int ProducerId { get; set; }

        [Required(ErrorMessage = "Producer name is required")]
        public string ProducerName { get; set; }

        [Required(ErrorMessage = "Producer email is required")]
        [DataType(DataType.EmailAddress)]
        public string ProducerEmail { get; set; }

        [Required(ErrorMessage = "Producer contact is required")]
        public string ProducerPhone { get; set; }
    }
}
