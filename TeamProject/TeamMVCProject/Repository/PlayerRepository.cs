using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using TeamMVCProject.Context;
using TeamMVCProject.Models;

namespace TeamMVCProject.Repository
{
    public class PlayerRepository
    {
        private TeamsPlayersContext db = new TeamsPlayersContext();
       
        public async Task<Player> FindAsync(int id)
        {
            return await db.Players.FindAsync(id);
        }

        public async Task<List<Player>> IncludePlayerTeams()
        {
            return await db.Players.Include(p => p.Teams).ToListAsync();
        }

        public void AddPlayer(Player player)
        {
            db.Players.Add(player);
            db.SaveChanges();
        }
        
        public async Task SetPlayerTeams(Player player, int[] selectedTeams)
        {
            player.Teams = new List<Team>();
            foreach (var team in selectedTeams)
            {
                var teamToAdd = await db.Teams.FindAsync(team);
                player.Teams.Add(teamToAdd);
            }
        }

        public async Task<List<Team>> GetTeams()
        {
            return await db.Teams.ToListAsync();
        }

        public async Task<List<Player>> GetPlayers()
        {
            return await db.Players.ToListAsync();
        }

        public void Edit(Player player)
        {
            db.Entry(player).State = EntityState.Modified;
            db.SaveChanges();
        }

        public async Task Remove(int id)
        {
            var player = await this.FindAsync(id);
            db.Players.Remove(player);
            db.SaveChanges();
        }
    }
}