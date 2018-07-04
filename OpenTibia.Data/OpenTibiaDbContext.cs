// <copyright file="OpenTibiaDbContext.cs" company="2Dudes">
// Copyright (c) 2018 2Dudes. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
// </copyright>

namespace OpenTibia.Data
{
    using Microsoft.EntityFrameworkCore;
    using OpenTibia.Data.Configurations;
    using OpenTibia.Data.Models;

    // [DbConfigurationType(typeof(OpenTibia.Db.MultipleDbConfiguration))]
    public class OpenTibiaDbContext : DbContext
    {
        // private static string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        // private static readonly object _lockDbObject = new object();

        // private static DbConnection _mysqlDbConnection;

        // private static DbConnection GetCommonConnection()
        // {
        //    if (_mysqlDbConnection == null || _mysqlDbConnection.State != System.Data.ConnectionState.Open)
        //    {
        //        lock (_lockDbObject)
        //        {
        //            if (_mysqlDbConnection == null || _mysqlDbConnection.State != System.Data.ConnectionState.Open)
        //            {
        //                _mysqlDbConnection = OpenTibia.Db.MultipleDbConfiguration.GetMySqlConnection(connectionString);
        //                _mysqlDbConnection.Open();
        //            }
        //        }
        //    }

        // return _mysqlDbConnection;
        // }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // TODO: to configuration
            optionsBuilder.UseSqlServer(@"<sql server connection string>");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new AssignedHouseConfiguration());
            builder.ApplyConfiguration(new BanishmentConfiguration());
            builder.ApplyConfiguration(new BuddyConfiguration());
            builder.ApplyConfiguration(new CipCreatureConfiguration());
            builder.ApplyConfiguration(new CreatureStatConfiguration());
            builder.ApplyConfiguration(new DeathConfiguration());
            builder.ApplyConfiguration(new GuildConfiguration());
            builder.ApplyConfiguration(new GuildMemberConfiguration());
            builder.ApplyConfiguration(new HouseConfiguration());
            builder.ApplyConfiguration(new HouseTransferConfiguration());
            builder.ApplyConfiguration(new OnlinePlayerConfiguration());
            builder.ApplyConfiguration(new PlayerModelConfiguration());
            builder.ApplyConfiguration(new RandPlayerConfiguration());
            builder.ApplyConfiguration(new StatConfiguration());
            builder.ApplyConfiguration(new UserConfiguration());

            base.OnModelCreating(builder);
        }

        public DbSet<Banishment> Banishments { get; set; }

        public DbSet<Buddy> Buddies { get; set; }

        public DbSet<CipCreature> Creatures { get; set; }

        public DbSet<CreatureStat> CreatureStats { get; set; }

        public DbSet<Death> Deaths { get; set; }

        public DbSet<Guild> Guilds { get; set; }

        public DbSet<GuildMember> GuildMembers { get; set; }

        public DbSet<House> Houses { get; set; }

        public DbSet<AssignedHouse> AssignedHouses { get; set; }

        public DbSet<HouseTransfer> HouseTransfers { get; set; }

        public DbSet<OnlinePlayer> Online { get; set; }

        public DbSet<PlayerModel> Players { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Stat> Stats { get; set; }

        public DbSet<RandPlayer> RandomPlayers { get; set; }

    }
}
