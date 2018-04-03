using System.Data.Entity;
using TeamMVCProject.Initializer;
using TeamMVCProject.Models;

namespace TeamMVCProject.Context
{

    public class TeamsPlayersContext : DbContext
    {
        public TeamsPlayersContext() : base("TeamsPlayersContext")
        {
            Database.SetInitializer(new ModelsInitializer());
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>()
                .HasMany(p => p.Teams)
                .WithMany(t => t.Players)
                .Map(x => x.MapLeftKey("PlayerID")
                    .MapRightKey("TeamID")
                    .ToTable("PlayerTeam"));

            //base.OnModelCreating(modelBuilder);
        }

    }
}