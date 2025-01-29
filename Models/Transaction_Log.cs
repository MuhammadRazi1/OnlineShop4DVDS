using System.ComponentModel.DataAnnotations;

namespace OnlineShop4DVDS.Models
{
    public class Transaction_Log
    {
        [Key]
        public int Transactionlog_Id { get; set; }

        public int User_Id {  get; set; }

        public DateTime Transactionallog_Transactiondate { get; set; }

        public int Transactionlog_Amount { get;  set; }

        public int Transactionallog_Transactiontype { get; set; }


        public string Transactionallog_Description { get; set; }





    }
}
