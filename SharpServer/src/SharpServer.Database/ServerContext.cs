using Microsoft.EntityFrameworkCore;
using SharpServer.Domain.Entities;

namespace SharpServer.Database
{
    public class ServerContext : DbContext
    {
        public DbSet<Account> Account { get; set; }
        public DbSet<Player> Player { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Context");
        }
    }
}