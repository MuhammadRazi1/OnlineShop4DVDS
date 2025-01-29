using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Song
    {
        [Key]
        public int Song_Id { get; set; }

        public string Song_Title{ get; set; }

        public int Album_Id { get; set; }

        public string Song_Album_Duration{ get;set; }

        public string Song_File_Path{ get; set; }

        public Boolean Song_Isfreedownload { get; set; }



    }
}
