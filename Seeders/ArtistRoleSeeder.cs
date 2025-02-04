using Microsoft.EntityFrameworkCore;
using OnlineShop4DVDS.Models;

namespace OnlineShop4DVDS.Seeders
{
    public class ArtistRoleSeeder
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArtistRole>().HasData(
                new ArtistRole { ArtistRoleId = 1, ArtistRoleName = "Singer" },
                new ArtistRole { ArtistRoleId = 2, ArtistRoleName = "Composer" },
                new ArtistRole { ArtistRoleId = 3, ArtistRoleName = "Both" }
            );
        }
    }
}
