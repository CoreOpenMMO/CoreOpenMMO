using Microsoft.EntityFrameworkCore;
using SharpServer.Domain.Entities;

namespace SharpServer.Database
{
    public class ServerContext : DbContext
    {
        public DbSet<User> User { get; set; }
        public DbSet<Character> Character { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Context");
        }
    }
}