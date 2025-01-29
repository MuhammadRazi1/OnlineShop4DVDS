using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class News
    {
        [Key]
        public int News_Id { get; set; }

        public string News_Title { get; set; }

        public string News_Content { get; set; }

        public DateTime News_Datepublished { get; set; }

        public int Category_Id { get; set; }




    }
}
