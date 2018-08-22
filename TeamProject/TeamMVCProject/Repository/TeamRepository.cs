using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Data.Entity;
using TeamMVCProject.Context;
using TeamMVCProject.Models;

namespace TeamMVCProject.Repository
{
    public class TeamRepository
    {

        private TeamsPlayersContext db = new TeamsPlayersContext();

        public async Task<Team> FindAsync(int id)
        {
            return await db.Teams.FindAsync(id);
        }

        public async Task<List<Team>> IncludeTeamPlayers()
        {
            return await db.Teams.Include(t => t.Players).ToListAsync();
        }

        public async Task AddTeam(Team team)
        {
            db.Teams.Add(team);
            await db.SaveChangesAsync();
        }

        public async Task SetTeamPlayers(Team team, int[] selectedPlayers)
        {
            team.Players = new List<Player>();
            foreach (var player in selectedPlayers)
            {
                var playerToAdd = await db.Players.FindAsync(player);
                team.Players.Add(playerToAdd);
            }
        }

        public async Task<List<Player>> GetPlayers()
        {
            return await db.Players.ToListAsync();
        }

        public async Task<List<Team>> GetTeams()
        {
            return await db.Teams.ToListAsync();
        }

        public async Task Edit(Team team)
        {
            db.Entry(team).State = EntityState.Modified;
            await db.SaveChangesAsync();
        }

        public async Task Remove(int id)
        {
            var team = await this.FindAsync(id);
            db.Teams.Remove(team);
            await db.SaveChangesAsync();
        }

    }
}