using Microsoft.EntityFrameworkCore;

namespace COTS.Data.Context
{
    using Domain.Entities;

    public class COTSContext : DbContext
    {
        public DbSet<Account> Account { get; set; }
        public DbSet<Player> Player { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => 
            optionsBuilder.UseInMemoryDatabase("Context");
    }
}