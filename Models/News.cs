using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class News
    {
        [Key]
        public int NewsId { get; set; }

        [Required(ErrorMessage = "News title is required")]
        public string NewsTitle { get; set; }

        [Required(ErrorMessage = "News description is required")]
        public string NewsDescription { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        public DateTime NewsDate { get; set; }

        [Required(ErrorMessage = "Author is required")]
        public string NewsAuthor { get; set; }
    }
}
