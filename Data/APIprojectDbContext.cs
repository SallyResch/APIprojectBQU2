using APIprojectBQU.Model;
using Microsoft.EntityFrameworkCore;

namespace APIprojectBQU.Data
{
    public class APIprojectDbContext : DbContext
    {
        public APIprojectDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
    }
}
