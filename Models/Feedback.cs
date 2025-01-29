using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OnlineShop4DVDS.Models
{
    public class Feedback
    {
        [Key]
        public int Feedback_Id { get; set; }

        public int User_Id { get; set; }

        public string Feedback_Content {  get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime  Feedback_Date_submitted { get; set; }

        public string Feedback_Response {  get; set; }










    }
}
