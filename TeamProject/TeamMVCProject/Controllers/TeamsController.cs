using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using TeamMVCProject.Context;
using TeamMVCProject.Models;
using TeamMVCProject.Repository;
using TeamMVCProject.ViewModels;

namespace TeamMVCProject.Controllers
{
    public class TeamsController : Controller
    {
        private TeamRepository teamRepository = new TeamRepository();

        // GET: Teams
        public async Task<ActionResult> Index()
        {
            return View(await teamRepository.IncludeTeamPlayers());        
        }

        // GET: Teams/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = await teamRepository.FindAsync(Convert.ToInt32(id));
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // GET: Teams/Create
        public async Task<ActionResult> Create()
        {
            ViewBag.TeamID = new SelectList(await teamRepository.GetTeams(), "ID", "TeamName");
            var team = new Team
            {
                Players = new List<Player>()
            };
            ViewBag.Players = await PopulateAssignedPlayerData(team);
            return View();
        }
            
        // POST: Teams/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,Name,Description")] Team team, int[] selectedPlayers)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Players = await PopulateAssignedPlayerData(team);
                return View(team);
            }

            if (selectedPlayers != null)
            {
                await teamRepository.SetTeamPlayers(team, selectedPlayers);
            }

            await teamRepository.AddTeam(team);
            return RedirectToAction("Index");
        }

        // GET: Teams/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var teams = await teamRepository.IncludeTeamPlayers();
            var teamToPopulate = teams.Where(t => t.ID == id).Single();

            if (teamToPopulate == null)
            {
                return HttpNotFound();
            }

            ViewBag.Players = await PopulateAssignedPlayerData(teamToPopulate);
            return View(teamToPopulate);
        }

        // POST: Teams/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int? id, string[] selectedPlayers)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var teams = await teamRepository.IncludeTeamPlayers();
            var teamToUpdate = teams.Where(i => i.ID == id).Single();

            if (ModelState.IsValid)
            {
                await UpdateTeamPlayers(selectedPlayers, teamToUpdate);
                await teamRepository.Edit(teamToUpdate);

                return RedirectToAction("Index");
            }

            ViewBag.Players = await PopulateAssignedPlayerData(teamToUpdate);

            return View(teamToUpdate);
        }

        private async Task<List<PlayerTeamVM>> PopulateAssignedPlayerData(Team team)
        {
            var allPlayers = await teamRepository.GetPlayers();
            var teamPlayers = new HashSet<int>(team.Players.Select(t => t.ID));
            var viewModel = new List<PlayerTeamVM>();
            foreach (var player in allPlayers)
            {
                viewModel.Add(new PlayerTeamVM
                {
                    ID = player.ID,
                    Name = player.Name,
                    isAssigned = teamPlayers.Contains(player.ID)
                });
            }
            return viewModel;
        }

        private async Task UpdateTeamPlayers(string[] selectedPlayers, Team teamToUpdate)
        {
            if (selectedPlayers == null)
            {
                teamToUpdate.Players = new List<Player>();
                return;
            }

            var allPlayers = await teamRepository.GetPlayers();
            var selectedPlayersHS = new HashSet<string>(selectedPlayers);
            var teamPlayers = new HashSet<int>
                (teamToUpdate.Players.Select(p => p.ID));
            foreach (var player in allPlayers)
            {
                if (selectedPlayersHS.Contains(player.ID.ToString()))
                {
                    if (!teamPlayers.Contains(player.ID))
                    {
                        teamToUpdate.Players.Add(player);
                    }
                }
                else
                {
                    if (teamPlayers.Contains(player.ID))
                    {
                        teamToUpdate.Players.Remove(player);
                    }
                }
            }
        }

        // GET: Teams/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = await teamRepository.FindAsync(Convert.ToInt32(id));
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await teamRepository.Remove(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
