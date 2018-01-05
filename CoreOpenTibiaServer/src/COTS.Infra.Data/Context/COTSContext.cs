using COTS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace COTS.Data.Context
{
    public class COTSContext : DbContext
    {
        public DbSet<Account> Account { get; set; }
        public DbSet<Player> Player { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase("Context");
        }
    }
}