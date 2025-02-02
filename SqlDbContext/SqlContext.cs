using Microsoft.EntityFrameworkCore;
using OnlineShop4DVDS.Models;

namespace OnlineShop4DVDS.SqlDbContext
{
    public class SqlContext : DbContext
    {
        public SqlContext(DbContextOptions<SqlContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
