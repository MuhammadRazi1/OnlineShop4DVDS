using Microsoft.EntityFrameworkCore;

namespace OnlineShop4DVDS.SqlDbContext
{
    public class SqlContext : DbContext
    {
        public SqlContext(DbContextOptions<SqlContext> options) : base(options)
        {

        }
    }
}
