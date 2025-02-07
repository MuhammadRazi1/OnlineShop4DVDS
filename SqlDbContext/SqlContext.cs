using Microsoft.EntityFrameworkCore;
using OnlineShop4DVDS.Models;
using OnlineShop4DVDS.Seeders;

namespace OnlineShop4DVDS.SqlDbContext
{
    public class SqlContext : DbContext
    {
        public SqlContext(DbContextOptions<SqlContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ArtistRole> ArtistRoles { get; set; }
        public DbSet<Artist> Artists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ArtistRoleSeeder.Seed(modelBuilder);

            modelBuilder.Entity<Artist>()
                .HasOne(a => a.ArtistRole)
                .WithMany() 
                .HasForeignKey(a => a.ArtistRoleId);

            modelBuilder.Entity<Album>()
                .HasOne(a => a.Artist)
                .WithMany(ar => ar.Albums)
                .HasForeignKey(a => a.ArtistId);
        }
    }
}
