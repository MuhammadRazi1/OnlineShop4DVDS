﻿namespace OnlineShop4DVDS.Models
{
    public class UserSong
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int SongId { get; set; }
        public Song Song { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
