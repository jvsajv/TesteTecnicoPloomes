using Microsoft.EntityFrameworkCore;
using TesteTecnicoPloomes.Data.Map;
using TesteTecnicoPloomes.Models;

namespace TesteTecnicoPloomes.Infrastructure.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserMap());
            modelBuilder.ApplyConfiguration(new PostMap());
            base.OnModelCreating(modelBuilder);
        }
    }
}
